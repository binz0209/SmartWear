using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    /// <summary>
    /// Interface chung cho việc gửi email.
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// Gửi email bất đồng bộ.
        /// </summary>
        /// <param name="to">Địa chỉ nhận.</param>
        /// <param name="subject">Tiêu đề email.</param>
        /// <param name="body">Nội dung email (có thể HTML).</param>
        Task SendEmailAsync(string to, string subject, string body);
    }
}
