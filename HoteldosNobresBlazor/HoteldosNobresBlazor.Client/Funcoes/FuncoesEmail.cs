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
            body = $@"
            <p>Olá,</p>
            <p>Recebemos a reserva <a href='https://hotels.cloudbeds.com/connect/235132#/reservations/r{id}'><strong>{id}</strong></a>.</p>
            <p>Com isso, está dando erro em nossa API pois não tem o tipo de pagamento. Esse chamado vai ser aberto até vir essa informação, pois é importante para a nossa API. Pois na <a href='https://developers.cloudbeds.com/reference/get_getreservation'>documentação</a> não consta essa informação. Como consta <a href='https://developers.cloudbeds.com/docs/how-to-report-an-api-bug'>aqui</a>, se você ainda estiver enfrentando problemas, envie um e-mail.</p>
            <p>Atenciosamente,<br>Hotel dos Nobres</p>";

            subject = $"NÃO TEM O Tipo de pagamento NUMERO {id} gerando ERRO";
            emailsuporte = "integrations@cloudbeds.com";
        }
        if (tipo == 2)
        {
            body = $@"
            <p>Olá,</p>
            <p>Recebemos a reserva <a href='https://hotels.cloudbeds.com/connect/235132#/reservations/r{id}'><strong>{id}</strong></a>.</p>
            <p>Com isso, o cliente <strong>{nome}</strong> reclamou que o sistema estava com os valores fora do padrão Brasileiro. Solicitamos o ajuste do TK #2491327.</p>
            <p>Atenciosamente,<br>Hotel dos Nobres</p>";

            subject = $"PMS - VISUAL BUG - COMMA FOR DECIMAL SEPARATION IN PT-BR na reserva NUMERO {id}";
        }
        if (tipo == 3)
        {
            body = $@"
            <p>Olá,</p>
            <p>Recebemos a reserva <a href='https://hotels.cloudbeds.com/connect/235132#/reservations/r{id}'><strong>{id}</strong></a>.</p>
            <p>Com isso, meu operacional tem problema em visualizar em inglês o sistema da Cloudbeds. Quando foi contratado, foi informado que o sistema estaria em português. Já solicitamos o ajuste do TK #2647520 e estamos no aguardo.</p>
            <p>Atenciosamente,<br>Hotel dos Nobres</p>";

            subject = $"PMS - Tradução para pt br em solicitações especiais Booking.com na Reserva de numero {id}";
        }
        if (tipo == 4)
        {
            body = $@"
            <p>Olá,</p>
            <p>Recebemos a reserva <a href='https://hotels.cloudbeds.com/connect/235132#/reservations/r{id}'><strong>{id}</strong></a>.</p>
            <p>Com isso, precisamos apagar o horário de chegada da reserva via API #2682643 ou PMS #2730774. Solicitamos que seja ajustado esses TK.</p>
            <p>Atenciosamente,<br>Hotel dos Nobres</p>";

            subject = $"PMS e API - Não é possível apagar o horário de chegada na Reserva de número {id}";
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


