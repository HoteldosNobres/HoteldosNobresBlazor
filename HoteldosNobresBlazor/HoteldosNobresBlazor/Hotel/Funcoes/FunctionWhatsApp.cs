using HoteldosNobresBlazor.Classes;
using HoteldosNobresBlazor.Components.Pages;
using Newtonsoft.Json;
using System.Collections.Generic;
using static HoteldosNobresBlazor.Components.Pages.CallApi;

namespace HoteldosNobresBlazor.Funcoes
{
    public class FunctionWhatsApp
    { 
        static string urlapi = @"https://graph.facebook.com/v19.0/227958023743323/messages";
       

        #region  Messages

        public static async Task<string> postMensagemTemplete(string numero, string nametemplete)
        {
            try
            {
                 
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, urlapi);
                request.Headers.Add("Authorization", "Bearer " + KEYs.TOKEN_WHATSAPP); 
                var content = new StringContent("{ \"messaging_product\": \"whatsapp\", \"to\": \""+ numero + "\", \"type\": \"template\", \"template\": { \"name\": \""+ nametemplete + "\", \"language\": { \"code\": \"pt_BR\" } } }", null, "application/json");
                request.Content = content;
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

        public static async Task<string> postMensagem(string numero, string mensagem)
        {
            try
            {
                if (string.IsNullOrEmpty(mensagem)) 
                    mensagem = @" Converse com Hotel dos Nobres no WhatsApp: https://wa.me/message/EKQWQ3LJBWUNA1 ";

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, urlapi);
                request.Headers.Add("Authorization", "Bearer " + KEYs.TOKEN_WHATSAPP); 
                var content = new StringContent("{\n    \"messaging_product\": \"whatsapp\",\n  " +
                    "  \"to\": \"" + numero + "\",\n    \"type\": \"text\",\n  " +
                    "  \"text\": {\n " +
                    "  \"body\": \"" + mensagem + "\"\n  }\n}", null, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();  
                Console.WriteLine(await response.Content.ReadAsStringAsync());
                return "";
            }
            catch (FileNotFoundException e)
            { 
                return e.Message + "\n";
            }

        }

        public static async Task<string> postMensageFlowCPF(string numero)
        {
            try
            { 
                //{
                //                  "messaging_product": "whatsapp",
                //"to": "5516981000673", 
                //"type": "INTERACTIVE", 
                //"interactive": {
                //                      "type": "flow", 
                //  "header": {
                //                          "type": "text", 
                //    "text": "Preeche os dados Faltando"
                //  }, 
                //  "body": {
                //                          "text": "Falta o CPF e Data de nascimento"
                //  }, 
                //  "footer": {
                //                          "text": "Enviado pelo sistema."
                //  }, 
                //  "action": {
                //                          "name": "flow", 
                //    "parameters": {
                //                              "flow_message_version": "3", 
                //      "flow_token": "any_string_for_this_example", 
                //      "flow_id": "7196830407096265", 
                //      "flow_cta": "Abrir prenchimento", 
                //      "flow_action": "navigate", 
                //      "flow_action_payload": {
                //                                  "screen": "SIGN_UP", 
                //        "data": {
                //                                      "type": "dynamic_object"
                //                }
                //                              }
                //                          }
                //                      }
                //                  }
                //              }


                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, urlapi);
                request.Headers.Add("Authorization", "Bearer " + KEYs.TOKEN_WHATSAPP);
                var content = new StringContent("{ \n  \"messaging_product\": \"whatsapp\"," +
                    "\n  \"to\": \"" + numero + "\", \n  \"type\": \"INTERACTIVE\", \n" +
                    "  \"interactive\": { \n    \"type\": \"flow\", \n    \"header\": { \n" +
                    "      \"type\": \"text\", \n      \"text\": \"Preeche os dados Faltando\" \n" +
                    "    }, \n    \"body\": { \n      \"text\": \"Falta o CPF e Data de nascimento\" \n" +
                    "    }, \n    \"footer\": { \n      \"text\": \"Enviado pelo sistema.\" \n    }, \n  " +
                    "  \"action\": { \n      \"name\": \"flow\", \n      \"parameters\": { \n   " +
                    "     \"flow_message_version\": \"3\", \n        \"flow_token\": \"any_string_for_this_example\", \n    " +
                    "    \"flow_id\": \"302483089525226\", \n        \"flow_cta\": \"Abrir prenchimento\", \n   " +
                    "     \"flow_action\": \"navigate\", \n        \"flow_action_payload\": { \n   " +
                    "       \"screen\": \"SIGN_UP\", \n          \"data\": { \n      " +
                    "              \"type\": \"dynamic_object\" \n          " +
                    "        } \n        } \n      } \n    } \n  } \n}", null, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                Console.WriteLine(await response.Content.ReadAsStringAsync());
                return "";
            }
            catch (FileNotFoundException e)
            {
                return e.Message + "\n";
            }

        }

        public static async Task<string> postMensageFlowAvaliacao(string numero)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, urlapi);
                request.Headers.Add("Authorization", "Bearer " + KEYs.TOKEN_WHATSAPP);
                var content = new StringContent("{ \n  \"messaging_product\": \"whatsapp\"," +
                    "\n  \"to\": \"" + numero + "\", \n  \"type\": \"INTERACTIVE\", \n" +
                    "  \"interactive\": { \n    \"type\": \"flow\", \n    \"header\": { \n" +
                    "      \"type\": \"text\", \n      \"text\": \"Preencha a avaliacao do Hotel\" \n" +
                    "    }, \n    \"body\": { \n      \"text\": \"Queremos seu comentario\" \n" +
                    "    }, \n    \"footer\": { \n      \"text\": \"Enviado pelo sistema.\" \n    }, \n  " +
                    "  \"action\": { \n      \"name\": \"flow\", \n      \"parameters\": { \n   " +
                    "     \"flow_message_version\": \"3\", \n        \"flow_token\": \"any_string_for_this_example\", \n    " +
                    "    \"flow_id\": \"1068116050956044\", \n        \"flow_cta\": \"Abrir prenchimento\", \n   " +
                    "     \"flow_action\": \"navigate\", \n        \"flow_action_payload\": { \n   " +
                    "       \"screen\": \"FEEDBACK\", \n          \"data\": { \n      " +
                    "              \"type\": \"dynamic_object\" \n          " +
                    "        } \n        } \n      } \n    } \n  } \n}", null, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                Console.WriteLine(await response.Content.ReadAsStringAsync());
                return "";
            }
            catch (FileNotFoundException e)
            {
                return e.Message + "\n";
            }

        }

        public static async Task<string> postMensagemTempleteNovaReserva(string numero, string reservaID, string origemID, string origem, string mtur)
        {
            try
            {

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, urlapi);
                request.Headers.Add("Authorization", "Bearer " + KEYs.TOKEN_WHATSAPP);
                var content = new StringContent("{ \r\n    \"messaging_product\": \"whatsapp\", \r\n" +
                    "    \"to\": \"" + numero + "\", \r\n" +
                    "    \"type\": \"template\", \r\n" +
                    "    \"template\": \r\n    { \r\n" +
                    "        \"name\": \"novareserva\",\r\n" +
                    "         \"language\": \r\n " +
                    "        { \"code\": \"pt_BR\" \r\n" +
                    "         },\r\n         \"components\":  [{\r\n" +
                    "            \"type\": \"body\",\r\n" +
                    "            \"parameters\": [{\r\n " +
                    "                           \"type\": \"text\",\r\n" +
                    "                            \"text\": \"" + reservaID + "\"\r\n" +
                    "                            },\r\n" +
                    "                            {\r\n " +
                    "                           \"type\": \"text\",\r\n " +
                    "                           \"text\": \"" + origemID + "\"\r\n " +
                    "                           },\r\n " +
                    "                           { \r\n " + 
                    "                           \"type\": \"text\",\r\n" +
                    "                            \"text\": \"" + origem + "\"\r\n" +
                    "                            }],\r\n " +
                    "                           {\r\n" +
                    "                            \"type\": \"text\",\r\n" +
                    "                            \"text\": \"" + mtur + "\"\r\n" +
                    "                            }]\r\n                }] \r\n        \r\n    } \r\n}", null, "application/json"); request.Content = content;
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

        public static async Task<string> postMensagemTempleteConfirmaReserva(string numero, string reservaID)
        {
            try
            {

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, urlapi);
                request.Headers.Add("Authorization", "Bearer " + KEYs.TOKEN_WHATSAPP);
                var content = new StringContent("{     \"messaging_product\": \"whatsapp\"," +
                    "     \"to\": \"" + numero + "\",   " +
                    "   \"type\":  \"template\",  " +
                    "   \"template\":      {      " +
                    "    \"name\": \"reserva_confirmacao\",   " +
                    "      \"language\":               {           " +
                    "        \"code\": \"pt_BR\"              },   " +
                    "      \"components\":  [             {     " +
                    "            \"type\": \"HEADER\",        " +
                    "         \"parameters\": [{             " +
                    "                  \"type\": \"text\",    " +
                    "                          \"text\": \"" + reservaID + "\" }] }] } }", null, "application/json");
                request.Content = content;
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


        #endregion Messages


        public static async Task<T> LerRespostaComoObjetoAsync<T>(HttpResponseMessage response)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            T obj = JsonConvert.DeserializeObject<T>(jsonString);
            return obj;
        }

        public static async Task<T> LerRespostaComoObjetoAsync<T>(string jsonString)
        { 
            T obj = JsonConvert.DeserializeObject<T>(jsonString);
            return obj;
        }

        private static async Task<HttpResponseMessage> GetApi(string url)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Authorization", "Bearer " + KEYs.TOKEN_CLOUDBEDS); 
                request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

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

    }
}