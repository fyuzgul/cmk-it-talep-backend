using System.Net;
using System.Net.Mail;
using CMKITTalep.API.Models;

namespace CMKITTalep.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly SmtpSettings _smtpSettings;

        public EmailService(ILogger<EmailService> logger, SmtpSettings smtpSettings)
        {
            _logger = logger;
            _smtpSettings = smtpSettings;
        }

        public async Task SendPasswordResetEmailAsync(string email, string resetToken)
        {
            try
            {
                using var client = new SmtpClient(_smtpSettings.SmtpServer, _smtpSettings.Port);
                client.EnableSsl = _smtpSettings.EnableSsl;
                client.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName),
                    Subject = "CMK IT Talep - Şifre Sıfırlama",
                    Body = CreatePasswordResetEmailBody(resetToken),
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);

                await client.SendMailAsync(mailMessage);
                
                _logger.LogInformation($"Password reset email sent successfully to: {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send password reset email to: {email}");
                throw new Exception("Email gönderimi başarısız oldu. Lütfen daha sonra tekrar deneyin.");
            }
        }

        private string CreatePasswordResetEmailBody(string resetToken)
        {
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <div style='background-color: #f8f9fa; padding: 20px; border-radius: 10px; text-align: center;'>
                            <h2 style='color: #007bff; margin-bottom: 20px;'>CMK IT Talep Sistemi</h2>
                            <h3 style='color: #333; margin-bottom: 20px;'>Şifre Sıfırlama</h3>
                        </div>
                        
                        <div style='padding: 30px 20px;'>
                            <p>Merhaba,</p>
                            <p>CMK IT Talep sisteminde şifrenizi sıfırlamak için aşağıdaki doğrulama kodunu kullanabilirsiniz:</p>
                            
                            <div style='background-color: #f8f9fa; padding: 20px; border-radius: 5px; text-align: center; margin: 20px 0;'>
                                <h2 style='color: #007bff; font-size: 28px; letter-spacing: 3px; margin: 0;'>{resetToken}</h2>
                            </div>
                            
                            <p><strong>Önemli Bilgiler:</strong></p>
                            <ul>
                                <li>Bu kod <strong>15 dakika</strong> geçerlidir</li>
                                <li>Kod sadece <strong>bir kez</strong> kullanılabilir</li>
                                <li>Bu işlemi siz yapmadıysanız, bu emaili görmezden gelebilirsiniz</li>
                            </ul>
                            
                            <p>Şifrenizi sıfırlamak için sisteme giriş yapın ve 'Şifre Sıfırla' seçeneğini kullanın.</p>
                            
                            <div style='margin-top: 30px; padding-top: 20px; border-top: 1px solid #dee2e6;'>
                                <p style='font-size: 12px; color: #6c757d; margin: 0;'>
                                    Bu email otomatik olarak gönderilmiştir. Lütfen yanıtlamayın.<br>
                                    CMK Kablo Sanayi ve Ticaret A.Ş.
                                </p>
                            </div>
                        </div>
                    </div>
                </body>
                </html>";
        }
    }
}
