using HoteldosNobresBlazor.Classes;
using HoteldosNobresBlazor.Classes.BookingComDTO;
using HoteldosNobresBlazor.Client.FuncoesClient;
using Newtonsoft.Json;
using pix_payload_generator.net.Models.CobrancaModels;
using pix_payload_generator.net.Models.PayloadModels;
using System.Globalization;

namespace HoteldosNobresBlazor.Funcoes;

public class FunctionBookingCom
{
    #region Pedido
    static string urlapi = @"https://demandapi.booking.com/3.1/";

    public static async Task<BookingComDTO> postBookingCom(string checkin, string checkout)
    {
        try
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, urlapi + "accommodations/search");
            request.Headers.Add("Authorization", "Bearer " + KEYs.TOKEN_BookingCom);
            request.Headers.Add("X-Affiliate-id", " 7945173");

            var stringcontet = @"{
                " + "\n" +
                @"    ""booker"": {
                " + "\n" +
                @"      ""country"": ""nl"",
                " + "\n" +
                @"      ""platform"": ""desktop"" 
                " + "\n" +
                @"    },
                " + "\n" +
                @"    ""checkin"": """ + checkin + @""",
                " + "\n" +
                @"    ""checkout"": """ + checkout + @""",
                " + "\n" +
                @"    ""city"": -662921,
                " + "\n" +
                @"    ""currency"": ""BRL"",
                " + "\n" +
                @"    ""extras"": [
                " + "\n" +
                @"      ""extra_charges""
                " + "\n" +
                @"    ],
                " + "\n" +
                @"    ""guests"": {
                " + "\n" +
                @"      ""number_of_adults"": 1,
                " + "\n" +
                @"      ""number_of_rooms"": 1
                " + "\n" +
                @"    }
                " + "\n" +
                @"  }";

            var content = new StringContent(stringcontet, null, "application/json"); request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            BookingComDTO booking = await LerRespostaComoObjetoAsync<BookingComDTO>(response);

            return booking;
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine(e.Message);
            return null;
        }

    }


    #endregion Pedido


    public static async Task<T> LerRespostaComoObjetoAsync<T>(HttpResponseMessage response)
    {
        var jsonString = await response.Content.ReadAsStringAsync();
        T? obj = JsonConvert.DeserializeObject<T>(jsonString);
        return obj;
    }

}