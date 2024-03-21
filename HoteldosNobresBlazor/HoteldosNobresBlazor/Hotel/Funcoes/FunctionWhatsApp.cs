using HoteldosNobresBlazor.Classes;
using HoteldosNobresBlazor.Components.Pages;
using Newtonsoft.Json;
using System.Collections.Generic;
using static HoteldosNobresBlazor.Components.Pages.CallApi;

namespace HoteldosNobresBlazor.Funcoes
{
    public class FunctionWhatsApp
    { 
        static string urlapi = @"https://graph.facebook.com/v19.0/266344053226148/messages";
       

        #region  Messages

        public static async Task<Reserva> postMensagemTemplete(Reserva reserva, string note)
        {
            try
            {
                 
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, urlapi);
                request.Headers.Add("Authorization", "Bearer " + KEYs.TOKEN_WHATSAPP); 
                var content = new StringContent("{\n" +
                    "    \"messaging_product\": \"whatsapp\",\n  " +
                    "  \"to\": \"553537150180\",\n  " +
                    "  \"type\": \"template\",\n " +
                    "   \"template\": {\n   " +
                    "     \"name\": " +
                    "\"hello_world\",\n    " +
                    "    \"language\": {\n    " +
                    "        \"code\": \"en_US\"\n   " +
                    "     }\n    }\n}", null, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode(); 
                 
                return reserva;
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

        }

        public static async Task<string> postMensagem(string numero, string mensagem = null)
        {
            try
            {
                if (string.IsNullOrEmpty(mensagem)) 
                    mensagem = @"Para falar conosco no Whatsapp do link - > https://api.whatsapp.com/send/?phone=553537150180&text=Ol%C3%A1%2C%20vim%20pelo%20site!%20Gostaria%20de%20mais%20informa%C3%A7%C3%B5es%20sobre%20a%20consulta ";

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
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, urlapi);
                request.Headers.Add("Authorization", "Bearer " + KEYs.TOKEN_WHATSAPP);
                var content = new StringContent("{ \n  \"messaging_product\": \"whatsapp\",\n  \"to\": \"5535984151764\", \n  \"type\": \"INTERACTIVE\", \n  \"interactive\": { \n    \"type\": \"flow\", \n    \"header\": { \n      \"type\": \"text\", \n      \"text\": \"Preeche os dados Faltando\" \n    }, \n    \"body\": { \n      \"text\": \"Falta o CPF e Data de nascimento\" \n    }, \n    \"footer\": { \n      \"text\": \"Enviado pelo sistema.\" \n    }, \n    \"action\": { \n      \"name\": \"flow\", \n      \"parameters\": { \n        \"flow_message_version\": \"3\", \n        \"flow_token\": \"any_string_for_this_example\", \n        \"flow_id\": \"295956126850970\", \n        \"flow_cta\": \"Abrir prrenchimento\", \n        \"flow_action\": \"navigate\", \n        \"flow_action_payload\": { \n          \"screen\": \"SIGN_UP\", \n          \"data\": { \n                    \"type\": \"dynamic_object\" \n                  } \n        } \n      } \n    } \n  } \n}", null, "application/json");
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