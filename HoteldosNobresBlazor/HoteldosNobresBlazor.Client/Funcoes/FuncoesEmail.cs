using Google.Apis.PeopleService.v1.Data;
using HoteldosNobresBlazor.Client.FuncoesClient;
using System.Net;
using System.Net.Mail;
using static QRCoder.PayloadGenerator;

namespace HoteldosNobresBlazor;

public class FuncoesEmail
{  
    public static void EnviarEmailCPF(string from, string id, string nome)
    {
        string body = nome + ", Recebemos sua reserva porem precisamos do seu CPF e data de nascimento para fazer o cadastro no Ministério do Turismo(MTur) informações exigidas pela  pela Lei 11.771 de 2008 e Decreto 7.381  de 2010";
        body += @"  Clique abaixo para responder com os dados de forma segura. https://hoteldosnobres.com.br/reserva?" + id;
        string subject = "Dados faltando para reserva";

        EnviarEmail( from, body, subject);
    }


    public static void EnviarEmail(string from, string body, string subject)
    {  
        SmtpClient smtp = new SmtpClient("smtp.mail.me.com", 587);
        using (var message = new MailMessage(new MailAddress("reserva@hoteldosnobres.com.br", "Hotel dos Nobres"), new MailAddress(from))
        {
            Subject = subject,
            Body = body,
            
        })
        {   
            smtp.EnableSsl = true; 
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential(KEYs.KEY_EMAILCONTA,  KEYs.KEY_EMAILSENHA);
            smtp.UseDefaultCredentials = false;


            smtp.Send(message);
        }

    }


    public static void EnviarEmailHTML(string from, string body, string subject)
    {
        SmtpClient smtp = new SmtpClient("smtp.mail.me.com", 587);
        using (var message = new MailMessage(new MailAddress("reserva@hoteldosnobres.com.br", "Hotel dos Nobres"), new MailAddress(from))
        {
            Subject = subject,
            Body = body,
            IsBodyHtml = true

        })
        {
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential(KEYs.KEY_EMAILCONTA, KEYs.KEY_EMAILSENHA);
            smtp.UseDefaultCredentials = false;


            smtp.Send(message);
        }

    }

    public static void EnviarEmaiSuporte(string nome)
    {
        string body = "Recebemos a reclamação do hospede " + nome + " que ao reservar ficou muito incomodado com o BRAZIL com isso estou encaminhando essa reclamação para voces tambem. ";
        body += "   ";
        string subject = "BUG - country 'BRASIL' in PT-BR";

        EnviarEmail("support@cloudbeds.com", body, subject);
    }

    

}


