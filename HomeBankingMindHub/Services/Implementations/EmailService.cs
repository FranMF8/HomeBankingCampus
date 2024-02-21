using HomeBankingMindHub.Services.Interfaces;
using System.Globalization;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;

namespace HomeBankingMindHub.Services.Implementations
{
    public class EmailService : IEmailService
    {
        public async Task<bool> SendEmail(string email)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient("smtp.office365.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(GetConfigValue("OriginEmail"), GetConfigValue("Password")),
                    EnableSsl = true,
                };

                MailMessage mensaje = new MailMessage
                {
                    From = new MailAddress(GetConfigValue("OriginEmail")),
                    Subject = "Verificar cuenta Banco Vino Tinto",
                    Body = "<p>Verificar correo</p>" +
                   $"<p><a href=\"http://localhost:5041/api/client/verify/{email}\">" +
                   "<button style=\"padding:10px; background-color: #4CAF50; color: white; border: none;\">Verificar</button></a></p>",
                    IsBodyHtml = true,
                };

                mensaje.To.Add(email);

                await smtpClient.SendMailAsync(mensaje);

                Console.WriteLine("Correo electrónico enviado exitosamente.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar el correo electrónico: {ex.Message}");
                return false;
            }
        }

        public bool ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                string DomainMapper(Match match)
                {
                    var idn = new IdnMapping();

                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        private string GetConfigValue(string key)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            return configuration[key];
        }

    }
}
