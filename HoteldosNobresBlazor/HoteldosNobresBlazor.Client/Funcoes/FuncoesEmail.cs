using HoteldosNobresBlazor.Client.FuncoesClient;
using System.Net;
using System.Net.Mail;

namespace HoteldosNobresBlazor;

public class FuncoesEmail
{ 
    static MailAddress fromAddress = new MailAddress("fabiohcnobre@hotmail.com");
     
    public static void EnviarEmailCPF(string from, string id, string nome)
    {
        string body = nome + ", Recebemos sua reserva porem precisamos do seu CPF e data de nascimento para fazer o cadastro no Ministério do Turismo(MTur) informações exigidas pela  pela Lei 11.771 de 2008 e Decreto 7.381  de 2010";
        body += @"  Clique abaixo para responder com os dados de forma segura. https://hoteldosnobres.com.br/reserva?" + id;
        string subject = "Dados faltando para reserva";

        EnviarEmail( from, body, subject);
    }


    public static void EnviarEmail(string from, string body, string subject)
    {
        MailAddress toAddress = new MailAddress(from, "");
        SmtpClient smtp = new SmtpClient();
        smtp.Host = "smtp-mail.outlook.com";
        using (var message = new MailMessage(fromAddress, toAddress)
        {
            Subject = subject,
            Body = body,

        })
        {
            smtp.Credentials = new NetworkCredential("hoteldosnobres@hotmail.com", KEYs.KEY_EMAILSENHA);
            smtp.Port = 587;
            smtp.EnableSsl = true; 
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


