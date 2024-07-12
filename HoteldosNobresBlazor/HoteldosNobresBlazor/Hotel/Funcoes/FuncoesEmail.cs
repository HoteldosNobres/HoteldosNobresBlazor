using HoteldosNobresBlazor.Client.FuncoesClient;
using System.Net;
using System.Net.Mail;

namespace HoteldosNobresBlazor;

public class FuncoesEmail
{
    static MailAddress fromAddress = new MailAddress("hoteldosnobres@hotmail.com", "Hotel dos Nobres");
   

    static SmtpClient smtp = new SmtpClient
    {
        Host = "smtp-mail.outlook.com",
        Port = 587,
        EnableSsl = true,
        Credentials = new NetworkCredential(fromAddress.Address, KEYs.KEY_EMAIL)
    };

    public static void EnviarEmailCPF(string from, string id, string nome)
    {
        string body = nome + ", Recebemos sua reserva! Para completar seu cadastro no Ministério do Turismo (MTur), precisamos do seu CPF e data de nascimento. Essas informações são exigidas pela Lei 11.771/2008 e pelo Decreto 7.381/2010.  ";
        body += @" 
                Clique no link abaixo para enviar seus dados de forma segura. 
                https://hoteldosnobres.azurewebsites.net/booking/" + id;
        string subject = "Informações pendentes para completar sua reserva";

        EnviarEmail( from, body, subject);
    }


    public static void EnviarEmaiSuporte(string nome)
    {
        string body =  "Recebemos a reclamação do hospede " + nome + " que ao reservar ficou muito incomodado com o BRAZIL com isso estou encaminhando essa reclamação para voces tambem. ";
        body += "   ";
        string subject = "BUG - country 'BRASIL' in PT-BR";

        EnviarEmail("support@cloudbeds.com", body, subject);
    }

    public static void EnviarEmail(string from, string body, string subject)
    {
        MailAddress toAddress = new MailAddress(from, "");
        using (var message = new MailMessage(fromAddress, toAddress)
        {
            Subject = subject,
            Body = body,

        })
        {
            smtp.Send(message);
        }

    }

    //public static void EnviarEmail(string pathToAttachment, string body, string subject)
    //{
    //    MailAddress toAddress = new MailAddress("fabiohcnobre@hotmail.com", "");
    //    using (var message = new MailMessage(fromAddress, toAddress)
    //    {
    //        Subject = subject,
    //        Body = body,

    //    })
    //    {
    //        message.Attachments.Add(new Attachment(pathToAttachment));

    //        smtp.Send(message);
    //    }

    //}

}


