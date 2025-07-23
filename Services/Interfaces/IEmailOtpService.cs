using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IEmailOtpService
    {
        Task SendOtpAsync(string email);
        Task<bool> ValidateOtpAsync(string email, string otp);
    }
}
