using Google.Apis.PeopleService.v1.Data;
using HoteldosNobresBlazor.Client.FuncoesClient;
using HoteldosNobresBlazor.Client.Pages;
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


    public static void EnviarEmailSuporte(string id, string nome, int tipo)
    {
        string body = string.Empty;
        string subject = string.Empty;
        string emailsuporte = "support@cloudbeds.com";
        if (tipo == 1)
        {
            body = "Ola, recebemos a reserva" + id;
            body += @"  com isso esta dando erro em nossa API pois nao tem o tipo de pagamento. Esse chamado vai ser aberto ate vim essa informacao pois e importante para a nossa API. Pois na  https://developers.cloudbeds.com/reference/get_getreservation não consta essa informação. Como consta https://developers.cloudbeds.com/docs/how-to-report-an-api-bug Se você ainda estiver enfrentando problemas, envie um e-mail. ";

            subject = "PMS - NOVA RESERVA POREM ERRO COM API POIS NÃO TEM O Tipo de pagamento NUMERO " + id;
            emailsuporte = "integrations@cloudbeds.com";
        }
        if (tipo == 2)
        {
            body = "Ola, recebemos a reserva " + id;
            body += @"  com isso o cliente  " +nome;
            body += @"  reclamou que o sistema estaca com os valores fora do padrao Brasileiro com isso solicitamos o ajuste do TK #2491327. Att Obrigado ";

            subject = "PMS - VISUAL BUG - COMMA FOR DECIMAL SEPARATION IN PT-BR na reserva NUMERO " + id;
        }
        if (tipo == 3)
        {
            body = "Ola, recebemos a reserva " + id; 
            body += @" com isso meu operacional tem problema em visualizar em  INGLES o SISTEMA DA CLOUBEDS. Quando foi contratado falou que o sistema estaria em portugues. Ja foi solicitamos o ajuste do TK #2647520 estou no aguardo de vim em portugues. Att Obrigado ";

            subject = @"PMS - Tradução para pt br em solicitações especiais Booking.com na Reserva de numero " + id;
        }
        if (tipo == 4)
        {
            body = "Ola, recebemos a reserva " + id;
            body += @" com isso precisamos que apagar o horario de chegada da reserva via API #2682643 OU PMS #2730774 Soliciatamos que seja ajustado esses TK.";

            subject = @"PMS e API - Não e possivel apagar o horario de chegada na Reserva de numero " + id;
            emailsuporte += "; integrations@cloudbeds.com";
        }

        if (!string.IsNullOrEmpty(body)) 
            EnviarEmailHTML(emailsuporte, "hoteldosnobres@hotmail.com", body, subject);
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

    public static void EnviarEmailHTML(string from, string copy, string body, string subject)
    {
        SmtpClient smtp = new SmtpClient("smtp.mail.me.com", 587);
        using (var message = new MailMessage(new MailAddress("reserva@hoteldosnobres.com.br", "Hotel dos Nobres"), new MailAddress(from))
        {
            Subject = subject,
            Body = body, 
            IsBodyHtml = true

        })
        {
            message.CC.Add(new MailAddress(copy)); 
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


