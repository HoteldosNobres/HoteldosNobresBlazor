using HoteldosNobresBlazor.Classes;
using HoteldosNobresBlazor.Classes.PagSeguro;
using System.Xml.Serialization;
using Newtonsoft.Json;
using HoteldosNobresBlazor.Classes.PagSeguroRecebe;
using HoteldosNobresBlazor.Client.FuncoesClient;
using pix_payload_generator.net.Models.CobrancaModels;
using System.Text.RegularExpressions;
using System.Diagnostics.Eventing.Reader;


namespace HoteldosNobresBlazor.Funcoes;

public class FunctionPagSeguro
{
    static string urlapi = @"https://ws.pagseguro.uol.com.br/v3";
    //static string apiurl = @"https://sandbox.api.pagseguro.com/";
    static string apiurl = @"https://api.pagseguro.com/";

    #region Pedido
    public static async Task<OrderPagSeguroRecebe> PostOrder(Reserva reserva, string valor)
    {
        decimal valorcomdecimal = !string.IsNullOrEmpty(valor) ? decimal.Parse(valor) : 0;
        var value = !string.IsNullOrEmpty(valor) ? long.Parse(Regex.Replace(valorcomdecimal.ToString("F2"), @"[^\d]", "")) : 0;

        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, apiurl + "orders/");
        request.Headers.Add("Authorization", "Bearer " + KEYs.TOKEN_PAGSEGURO);
        request.Headers.Add("accept", "*/*");
        var orderDTO = new OrderDTO()
        {
            ReferenceId = reserva.IDReserva!,
            Customer = new Classes.PagSeguro.Customer()
            {
                Name = reserva.NomeHospede!,
                Email = reserva.Email!,
                TaxId = reserva.Cpf!,
                Phones = new PhonePagSeguro[]
                {
                        new PhonePagSeguro(){
                            Country = long.Parse(reserva.CelularDDI!),
                            Area = long.Parse(reserva.CelularDDD!),
                            Number = long.Parse(reserva.Celular!),
                            Type = "MOBILE"
                        }
                }
            },
            Items = new List<ItemOrder>()
                {
                    new ItemOrder()
                    {
                        Name = "Reserva",
                        Quantity = 1,
                        UnitAmount = value
                    }
                },
            QrCodes = new List<Classes.PagSeguro.QrCode>()
                {
                    new Classes.PagSeguro.QrCode()
                    {
                        Amount = new Classes.PagSeguro.Amount()
                        {
                            Value = value
                        }
                        //,ExpirationDate = DateTime.Now.AddYears(1),
                    }
                },
            NotificationUrls = new List<string>()
                {
                    "https://apihoteldosnobres.azurewebsites.net/pagseguro"
                }
        };

        var json = JsonConvert.SerializeObject(orderDTO);
        var content = new StringContent(json, null, "application/json");
        request.Content = content;
        var response = client.Send(request);

        OrderPagSeguroRecebe? responseorder = await LerRespostaJsonComoObjetoAsync<OrderPagSeguroRecebe>(response);

        if (responseorder != null && responseorder.Error_messages != null && responseorder.Error_messages.Length > 0)
        {
            throw new Exception(responseorder.Error_messages.FirstOrDefault().Description + " - " + responseorder.Error_messages.FirstOrDefault().ParameterName);
        }

        if (responseorder != null)
            return responseorder;
        else
            return new OrderPagSeguroRecebe();

    }


    public static async Task<string> getOrder(string checkInFrom)
    {
        try
        {
            string url = urlapi + "/transactions/notifications/A5CA10-CB09200920BE-19949C0FB69F-F8393E?email=hoteldosnobres@hotmail.com&token=" + KEYs.TOKEN_PAGSEGURO;

            HttpResponseMessage response = GetApi(url).Result;

            Transaction transaction = await LerRespostaComoObjetoAsync<Transaction>(response);

            return transaction.GrossAmount.ToString();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return null;
        }

    }

    #endregion Pedido

    #region Pagamento
    public static async Task<string> getPagamentoAsync(string checkInFrom)
    {
        try
        {
            string url = urlapi + "/transactions/notifications/A5CA10-CB09200920BE-19949C0FB69F-F8393E?email=hoteldosnobres@hotmail.com&token=" + KEYs.TOKEN_PAGSEGURO;

            HttpResponseMessage response = GetApi(url).Result;

            Transaction transaction = await LerRespostaComoObjetoAsync<Transaction>(response);

            return transaction.GrossAmount.ToString();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return null;
        }

    }


    #endregion Reservation 

    #region Api
    public static async Task<T> LerRespostaComoObjetoAsync<T>(HttpResponseMessage response)
    {
        var xml = await response.Content.ReadAsStringAsync();
        T returnedXmlClass = default(T);

        try
        {
            using (TextReader reader = new StringReader(xml))
            {
                try
                {
                    returnedXmlClass =
                        (T)new XmlSerializer(typeof(T)).Deserialize(reader);
                }
                catch (InvalidOperationException)
                {
                    // String passed is not XML, simply return defaultXmlClass
                }
            }
        }
        catch (Exception ex)
        {
        }

        return returnedXmlClass;
    }

    public static async Task<T> LerRespostaJsonComoObjetoAsync<T>(HttpResponseMessage response)
    {
        var jsonString = await response.Content.ReadAsStringAsync();
        T? obj = JsonConvert.DeserializeObject<T>(jsonString);
        return obj;
    }

    private static async Task<HttpResponseMessage> GetApi(string url)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            //request.Headers.Add("Authorization", "Bearer " + KEYs.TOKEN_CLOUDBEDS);
            //request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

            var client = new HttpClient();

            var response = client.Send(request);

            return
                response;

        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
    }

    #endregion Api

}