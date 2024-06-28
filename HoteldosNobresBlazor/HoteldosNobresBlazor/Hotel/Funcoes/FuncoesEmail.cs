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
        string body = nome + ", Recebemos sua reserva porem precisamos do seu CPF e data de nascimento para fazer o cadastro no Ministério do Turismo(MTur) informações exigidas pela  pela Lei 11.771 de 2008 e Decreto 7.381  de 2010";
        body += @"  Clique abaixo para responder com os dados de forma segura. https://hoteldosnobres.azurewebsites.net/booking/" + id;
        string subject = "Dados faltando para reserva";

        EnviarEmail( from, body, subject);
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


