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

        public async Task SendRequestResponseNotificationAsync(string requesterEmail, string requesterName, string requestDescription, string responseMessage, string responseType)
        {
            try
            {
                using var client = new SmtpClient(_smtpSettings.SmtpServer, _smtpSettings.Port);
                client.EnableSsl = _smtpSettings.EnableSsl;
                client.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName),
                    Subject = "CMK IT Talep - Talebinize Cevap Verildi",
                    Body = CreateRequestResponseEmailBody(requesterName, requestDescription, responseMessage, responseType),
                    IsBodyHtml = true
                };

                mailMessage.To.Add(requesterEmail);

                await client.SendMailAsync(mailMessage);
                
                _logger.LogInformation($"Request response notification sent successfully to: {requesterEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send request response notification to: {requesterEmail}");
                throw new Exception("Bildirim emaili gönderimi başarısız oldu. Lütfen daha sonra tekrar deneyin.");
            }
        }

        public async Task SendNewRequestNotificationAsync(List<string> supportEmails, string requesterName, string requestDescription, string requestType, string supportType, bool isCCNotification = false)
        {
            try
            {
                Console.WriteLine($"DEBUG: EmailService - Sending email to {supportEmails.Count} users");
                Console.WriteLine($"DEBUG: EmailService - SMTP Server: {_smtpSettings.SmtpServer}, Port: {_smtpSettings.Port}");
                Console.WriteLine($"DEBUG: EmailService - From Email: {_smtpSettings.FromEmail}");
                
                using var client = new SmtpClient(_smtpSettings.SmtpServer, _smtpSettings.Port);
                client.EnableSsl = _smtpSettings.EnableSsl;
                client.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName),
                    Subject = isCCNotification ? "CMK IT Talep - Talep Hakkında Bilgilendirme" : "CMK IT Talep - Yeni Talep Oluşturuldu",
                    Body = CreateNewRequestEmailBody(requesterName, requestDescription, requestType, supportType, isCCNotification),
                    IsBodyHtml = true
                };

                foreach (var email in supportEmails)
                {
                    mailMessage.To.Add(email);
                    Console.WriteLine($"DEBUG: EmailService - Added recipient: {email}");
                }

                Console.WriteLine("DEBUG: EmailService - Attempting to send email...");
                await client.SendMailAsync(mailMessage);
                Console.WriteLine("DEBUG: EmailService - Email sent successfully!");
                
                _logger.LogInformation($"New request notification sent successfully to {supportEmails.Count} support users");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DEBUG: EmailService - Error sending email: {ex.Message}");
                Console.WriteLine($"DEBUG: EmailService - Stack trace: {ex.StackTrace}");
                _logger.LogError(ex, $"Failed to send new request notification to support users");
                throw new Exception("Bildirim emaili gönderimi başarısız oldu. Lütfen daha sonra tekrar deneyin.");
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

        private string CreateRequestResponseEmailBody(string requesterName, string requestDescription, string responseMessage, string responseType)
        {
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <div style='background-color: #f8f9fa; padding: 20px; border-radius: 10px; text-align: center;'>
                            <h2 style='color: #007bff; margin-bottom: 20px;'>CMK IT Talep Sistemi</h2>
                            <h3 style='color: #28a745; margin-bottom: 20px;'>Talebinize Cevap Verildi</h3>
                        </div>
                        
                        <div style='padding: 30px 20px;'>
                            <p>Merhaba <strong>{requesterName}</strong>,</p>
                            <p>CMK IT Talep sisteminde oluşturduğunuz talebe cevap verilmiştir.</p>
                            
                            <div style='background-color: #f8f9fa; padding: 20px; border-radius: 5px; margin: 20px 0;'>
                                <h4 style='color: #333; margin-top: 0;'>Talep Detayları:</h4>
                                <p style='margin: 10px 0;'><strong>Açıklama:</strong> {requestDescription}</p>
                                <p style='margin: 10px 0;'><strong>Cevap Türü:</strong> {responseType}</p>
                            </div>
                            
                            <div style='background-color: #e8f5e8; padding: 20px; border-radius: 5px; margin: 20px 0; border-left: 4px solid #28a745;'>
                                <h4 style='color: #28a745; margin-top: 0;'>Cevap:</h4>
                                <p style='margin: 0; white-space: pre-line;'>{responseMessage}</p>
                            </div>
                            
                            <p>Detaylı bilgi için sisteme giriş yapabilirsiniz.</p>
                            
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

        public async Task SendRequestUpdateNotificationAsync(string requesterEmail, string requesterName, string requestDescription, string oldStatus, string newStatus, string updateType)
        {
            try
            {
                Console.WriteLine($"DEBUG: EmailService - Sending request update notification to: {requesterEmail}");
                
                using var client = new SmtpClient(_smtpSettings.SmtpServer, _smtpSettings.Port);
                client.EnableSsl = _smtpSettings.EnableSsl;
                client.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName),
                    Subject = "CMK IT Talep - Talebinizde Güncelleme Yapıldı",
                    Body = CreateRequestUpdateEmailBody(requesterName, requestDescription, oldStatus, newStatus, updateType),
                    IsBodyHtml = true
                };

                mailMessage.To.Add(requesterEmail);
                Console.WriteLine($"DEBUG: EmailService - Added recipient: {requesterEmail}");

                Console.WriteLine("DEBUG: EmailService - Attempting to send request update email...");
                await client.SendMailAsync(mailMessage);
                Console.WriteLine("DEBUG: EmailService - Request update email sent successfully!");
                
                _logger.LogInformation($"Request update notification sent successfully to: {requesterEmail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DEBUG: EmailService - Error sending request update email: {ex.Message}");
                Console.WriteLine($"DEBUG: EmailService - Stack trace: {ex.StackTrace}");
                _logger.LogError(ex, $"Failed to send request update notification to: {requesterEmail}");
                throw new Exception("Bildirim emaili gönderimi başarısız oldu. Lütfen daha sonra tekrar deneyin.");
            }
        }

        private string CreateNewRequestEmailBody(string requesterName, string requestDescription, string requestType, string supportType, bool isCCNotification = false)
        {
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <div style='background-color: #f8f9fa; padding: 20px; border-radius: 10px; text-align: center;'>
                            <h2 style='color: #007bff; margin-bottom: 20px;'>CMK IT Talep Sistemi</h2>
                            <h3 style='color: #ffc107; margin-bottom: 20px;'>{(isCCNotification ? "Talep Hakkında Bilgilendirme" : "Yeni Talep Oluşturuldu")}</h3>
                        </div>
                        
                        <div style='padding: 30px 20px;'>
                            <p>Merhaba,</p>
                            <p>{(isCCNotification ? "CMK IT Talep sisteminde oluşturulan bir talep hakkında bilgilendirilmektesiniz." : "CMK IT Talep sisteminde yeni bir talep oluşturulmuştur ve sizin desteğinize ihtiyaç duyulmaktadır.")}</p>
                            
                            <div style='background-color: #f8f9fa; padding: 20px; border-radius: 5px; margin: 20px 0;'>
                                <h4 style='color: #333; margin-top: 0;'>Talep Detayları:</h4>
                                <p style='margin: 10px 0;'><strong>Talep Eden:</strong> {requesterName}</p>
                                <p style='margin: 10px 0;'><strong>Destek Tipi:</strong> {supportType}</p>
                                <p style='margin: 10px 0;'><strong>Talep Tipi:</strong> {requestType}</p>
                            </div>
                            
                            <div style='background-color: #fff3cd; padding: 20px; border-radius: 5px; margin: 20px 0; border-left: 4px solid #ffc107;'>
                                <h4 style='color: #856404; margin-top: 0;'>Açıklama:</h4>
                                <p style='margin: 0; white-space: pre-line;'>{requestDescription}</p>
                            </div>
                            
                            <p>{(isCCNotification ? "Detaylı bilgi için sisteme giriş yapabilirsiniz." : "Lütfen sisteme giriş yaparak talebi inceleyin ve gerekli işlemleri gerçekleştirin.")}</p>
                            
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

        private string CreateRequestUpdateEmailBody(string requesterName, string requestDescription, string oldStatus, string newStatus, string updateType)
        {
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <div style='background-color: #f8f9fa; padding: 20px; border-radius: 10px; text-align: center;'>
                            <h2 style='color: #007bff; margin-bottom: 20px;'>CMK IT Talep Sistemi</h2>
                            <h3 style='color: #17a2b8; margin-bottom: 20px;'>Talep Güncellendi</h3>
                        </div>
                        
                        <div style='padding: 30px 20px;'>
                            <p>Merhaba <strong>{requesterName}</strong>,</p>
                            <p>CMK IT Talep sisteminde oluşturduğunuz talepte güncelleme yapılmıştır.</p>
                            
                            <div style='background-color: #f8f9fa; padding: 20px; border-radius: 5px; margin: 20px 0;'>
                                <h4 style='color: #333; margin-top: 0;'>Talep Detayları:</h4>
                                <p style='margin: 10px 0;'><strong>Açıklama:</strong> {requestDescription}</p>
                                <p style='margin: 10px 0;'><strong>Güncelleme Türü:</strong> {updateType}</p>
                            </div>
                            
                            <div style='background-color: #d1ecf1; padding: 20px; border-radius: 5px; margin: 20px 0; border-left: 4px solid #17a2b8;'>
                                <h4 style='color: #0c5460; margin-top: 0;'>Durum Değişikliği:</h4>
                                <p style='margin: 10px 0;'><strong>Önceki Durum:</strong> {oldStatus}</p>
                                <p style='margin: 10px 0;'><strong>Yeni Durum:</strong> {newStatus}</p>
                            </div>
                            
                            <p>Detaylı bilgi için sisteme giriş yapabilirsiniz.</p>
                            
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
