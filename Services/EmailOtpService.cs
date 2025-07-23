using Services.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class EmailOtpService : IEmailOtpService
    {
        // Store tạm OTP với expiry (10 phút)
        private static ConcurrentDictionary<string, (string Otp, DateTime Expiry)> _store
            = new();

        private readonly IEmailSender _emailSender;

        public EmailOtpService(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task SendOtpAsync(string email)
        {
            var otp = new Random().Next(100000, 999999).ToString();
            _store[email] = (otp, DateTime.UtcNow.AddMinutes(10));

            var subject = "One‑Time Password for SmartWear";
            var body = $"Mã OTP của bạn là: {otp}. Hết hạn sau 10 phút.";
            await _emailSender.SendEmailAsync(email, subject, body);
        }

        public Task<bool> ValidateOtpAsync(string email, string otp)
        {
            if (_store.TryGetValue(email, out var entry)
             && entry.Otp == otp
             && entry.Expiry > DateTime.UtcNow)
            {
                _store.TryRemove(email, out _);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}
