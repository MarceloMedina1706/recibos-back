using System.Net;
using System.Net.Mail;

namespace Template.Utils
{
    public class MailSender
    {
        public static bool send(string to, string name, string token)
        {
            var fromAddress = new MailAddress("", "");
            var toAddress = new MailAddress(to, name);
            const string fromPassword = "";
            const string subject = "Recuperación de contraseña";
            string link = "http://localhost:3000/recupero/" + token;
            //const string buttonStyle = "padding: 5px 10px; border: .1em solid #000; background: #fff; color: #000; text-decoration: none";

            const string btnstyle = "background: rgb(79, 65, 163);border: .1em solid rgb(79, 65, 163);border-radius: 3px;" +
                    "font-size: .8em;padding: 5px 10px;color: #fff;text-decoration: none";

            string button = "<div style='margin-top: 20px'><a style='" + btnstyle + "' href='" + link + "'>" +
                                    "REANUDAR LA RECUPERACIÓN" +
                                  "</a></div>";
            string opcion2 = "<div style='margin-top: 20px'>" +
                "<p>Si el botón no funciona copie el siguiente link y péguelo en su navegador.</p>" +
                link +
                "</div>";

            string body = "<div>Haga clic en el enlace de abajo para blanquear su clave:</div>" + button + opcion2;


            var smtp = new System.Net.Mail.SmtpClient
            {
                Host = "",
                Port = 587,
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                IsBodyHtml = true,
                Subject = subject,
                Body = body
            })
            {
                try
                {
                    smtp.Send(message);
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                }
            }
        }


        public static void SendCodigoVerificacion(string to, string name, string codigo)
        {
            var fromAddress = new MailAddress("", "");
            var toAddress = new MailAddress(to, name);
            const string fromPassword = "";
            const string subject = "Código de verificación";

            string cod = "<div style='margin-top: 20px'>" +
                codigo +
                "</div>";

            string body = "<div>Copie el siguiente código y péguelo en el portal.</div>" + cod;


            var smtp = new System.Net.Mail.SmtpClient
            {
                Host = "",
                Port = 587,
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                IsBodyHtml = true,
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }


    }
}
