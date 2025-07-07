using Repository;
using Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
        {
            return await _paymentRepository.GetAllPaymentsAsync();
        }

        public async Task<Payment> GetPaymentByIdAsync(Guid id)
        {
            return await _paymentRepository.GetPaymentByIdAsync(id);
        }

        public async Task AddPaymentAsync(Payment payment)
        {
            await _paymentRepository.AddPaymentAsync(payment);
        }

        public async Task UpdatePaymentAsync(Payment payment)
        {
            await _paymentRepository.UpdatePaymentAsync(payment);
        }

        public async Task DeletePaymentAsync(Guid id)
        {
            await _paymentRepository.DeletePaymentAsync(id);
        }
    }
}
