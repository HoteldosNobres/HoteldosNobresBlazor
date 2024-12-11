using HoteldosNobresBlazor.Client.FuncoesClient;

namespace HoteldosNobresBlazor.Funcoes
{
    public class FunctionWhatsApp
    {
        static string urlapi = @"https://graph.facebook.com/v19.0/227958023743323/messages";
         
        public static async Task<string> postMensagemTempletePIX(string numero, string reservaID, string Chave)
        {
            try
            { 
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, urlapi);
                request.Headers.Add("Authorization", "Bearer " + KEYs.TOKEN_WHATSAPP);
                string stringcontet = "{ \r\n    \"messaging_product\": \"whatsapp\", \r\n" +
                    "    \"to\": \"" + numero + "\", \r\n" +
                    "    \"type\": \"template\", \r\n" +
                    "    \"template\": \r\n    { \r\n" +
                    "        \"name\": \"event_pix\",\r\n" +
                    "         \"language\": \r\n " +
                    "        { \"code\": \"pt_BR\" \r\n" +
                    "         },\r\n         \"components\":  [{\r\n" +
                    "            \"type\": \"body\",\r\n" +
                    "            \"parameters\": [{\r\n" +
                    "                               \"type\": \"text\",\r\n" +
                    "                                \"text\": \"" + Chave + "\"\r\n" +
                    "                            }]\r\n " +
                    "              },{" +
                    "            \"type\": \"button\",\r\n" +
                    "            \"sub_type\": \"url\",\r\n" +
                    "            \"index\": \"0\",\r\n" +
                    "            \"parameters\": [{\r\n" +
                    "                               \"type\": \"text\",\r\n" +
                    "                                \"text\": \"" + reservaID + "\"\r\n" +
                    "                            }]\r\n " +
                    "           }] \r\n" +
                    " } \r\n}";
                var content = new StringContent(stringcontet, null, "application/json"); request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                return "";
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

        }
         

    }
}