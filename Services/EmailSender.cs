using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Services.Interfaces;
using System.Threading.Tasks;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _config;
    public EmailSender(IConfiguration config) => _config = config;

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var host = _config["Smtp:Host"];
        var port = int.Parse(_config["Smtp:Port"]);
        var user = _config["Smtp:User"];
        var pass = _config["Smtp:Pass"];
        var from = new MailboxAddress(_config["Smtp:FromName"], user);

        var message = new MimeMessage();
        message.From.Add(from);
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = body };

        using var smtp = new SmtpClient();
        smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
        await smtp.ConnectAsync(host, 587, SecureSocketOptions.StartTls);
        Console.WriteLine($"SMTP AUTH→ user={user}, pass={pass}, host={host}, port={port}");

        await smtp.AuthenticateAsync(user, pass);
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);
    }
}
