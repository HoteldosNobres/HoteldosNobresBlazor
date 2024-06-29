using HoteldosNobresBlazor.Classes;
using HoteldosNobresBlazor.Components.Pages;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using WhatsappBusiness.CloudApi.Configurations;
using WhatsappBusiness.CloudApi.Exceptions;
using WhatsappBusiness.CloudApi.Interfaces;
using WhatsappBusiness.CloudApi.Messages.ReplyRequests;
using WhatsappBusiness.CloudApi.Messages.Requests;
using WhatsappBusiness.CloudApi.Webhook;
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
                var content = new StringContent("{ \"messaging_product\": \"whatsapp\", \"to\": \"" + numero + "\", \"type\": \"template\", \"template\": { \"name\": \"" + nametemplete + "\", \"language\": { \"code\": \"pt_BR\" } } }", null, "application/json");
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

        //public static async Task<string> postMensageFlowCPF(string numero)
        //{
        //    try
        //    {
        //        var client = new HttpClient();
        //        var request = new HttpRequestMessage(HttpMethod.Post, urlapi);
        //        request.Headers.Add("Authorization", "Bearer " + KEYs.TOKEN_WHATSAPP);
        //        var content = new StringContent("{ \n  \"messaging_product\": \"whatsapp\"," +
        //            "\n  \"to\": \"" + numero + "\", \n  \"type\": \"INTERACTIVE\", \n" +
        //            "  \"interactive\": { \n    \"type\": \"flow\", \n    \"header\": { \n" +
        //            "      \"type\": \"text\", \n      \"text\": \"Preeche os dados Faltando\" \n" +
        //            "    }, \n    \"body\": { \n      \"text\": \"Recebemos sua reserva, porem de acordo com Ministério do Turismo(MTur) precisamos do CPF e Data de nascimento. Abre o preenchimento para envio automático para o sistema do Hotel dos Nobres.\" \n" +
        //            "    }, \n    \"footer\": { \n      \"text\": \"Enviado pelo sistema do Hotel dos Nobres\" \n    }, \n  " +
        //            "  \"action\": { \n      \"name\": \"flow\", \n      \"parameters\": { \n   " +
        //            "     \"flow_message_version\": \"3\", \n        \"flow_token\": \"any_string_for_this_example\", \n    " +
        //            "    \"flow_id\": \"302483089525226\", \n        \"flow_cta\": \"Abrir prenchimento\", \n   " +
        //            "     \"flow_action\": \"navigate\", \n        \"flow_action_payload\": { \n   " +
        //            "       \"screen\": \"SIGN_UP\", \n          \"data\": { \n      " +
        //            "              \"type\": \"dynamic_object\" \n          " +
        //            "        } \n        } \n      } \n    } \n  } \n}", null, "application/json");
        //        request.Content = content;
        //        var response = await client.SendAsync(request);
        //        response.EnsureSuccessStatusCode();
        //        Console.WriteLine(await response.Content.ReadAsStringAsync());
        //        return "";
        //    }
        //    catch (FileNotFoundException e)
        //    {
        //        return e.Message + "\n";
        //    }

        //}

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

        public static async Task<string> postMensagemTempleteDadosFaltando(string numero, string reservaID, string Nome)
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
                    "        \"name\": \"event_dados_faltando\",\r\n" +
                    "         \"language\": \r\n " +
                    "        { \"code\": \"pt_BR\" \r\n" +
                    "         },\r\n         \"components\":  [{\r\n" +
                    "            \"type\": \"body\",\r\n" +
                    "            \"parameters\": [{\r\n" +
                    "                               \"type\": \"text\",\r\n" +
                    "                                \"text\": \"" + Nome + "\"\r\n" +
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

        public static async Task<IActionResult> ReceiveWhatsAppTextMessage([FromBody] dynamic messageReceived)
        {
            ActionResult actionResult = null;
            IWhatsAppBusinessClient _whatsAppBusinessClient;
            WhatsAppBusinessCloudApiConfig _whatsAppConfig;
            IWebHostEnvironment _webHostEnvironment;
            string VerifyToken = "<YOUR VERIFY TOKEN STRING>";
            List<TextMessage> textMessage;
            List<AudioMessage> audioMessage;
            List<ImageMessage> imageMessage;
            List<DocumentMessage> documentMessage;
            List<StickerMessage> stickerMessage;
            List<ContactMessage> contactMessage;
            List<LocationMessage> locationMessage;
            List<QuickReplyButtonMessage> quickReplyButtonMessage;
            List<ReplyButtonMessage> replyButtonMessage;
            List<ListReplyButtonMessage> listReplyButtonMessage;

            try
            {
                if (messageReceived is null)
                {
                    //return BadRequest(new
                    //{
                    //    Message = "Message not received"
                    //});
                }

                var mensage = JsonConvert.DeserializeObject(messageReceived);
                var changesResult = messageReceived["entry"][0]["changes"][0]["value"];

                if (changesResult["statuses"] != null)
                {
                    var messageStatus = Convert.ToString(messageReceived["entry"][0]["changes"][0]["value"]["statuses"][0]["status"]);

                    if (messageStatus.Equals("sent"))
                    {
                        var messageStatusReceived = JsonConvert.DeserializeObject<UserInitiatedMessageSentStatus>(Convert.ToString(messageReceived)) as UserInitiatedMessageSentStatus;
                        var messageStatusResults = new List<UserInitiatedStatus>(messageStatusReceived.Entry.SelectMany(x => x.Changes).SelectMany(x => x.Value.Statuses));
                       
                        //return Ok(new
                        //{
                        //    Message = $"Message Status Received: {messageStatus}"
                        //});
                    }

                    if (messageStatus.Equals("delivered"))
                    {
                        var messageStatusReceived = JsonConvert.DeserializeObject<UserInitiatedMessageDeliveredStatus>(Convert.ToString(messageReceived)) as UserInitiatedMessageDeliveredStatus;
                        var messageStatusResults = new List<UserInitiatedMessageDeliveryStatus>(messageStatusReceived.Entry.SelectMany(x => x.Changes).SelectMany(x => x.Value.Statuses));
                         
                    }

                    if (messageStatus.Equals("read"))
                    {
                        //return Ok(new
                        //{
                        //    Message = $"Message Status Received: {messageStatus}"
                        //});
                    }
                }
                else
                {
                    var messageType = Convert.ToString(messageReceived["entry"][0]["changes"][0]["value"]["messages"][0]["type"]);

                    if (messageType.Equals("text"))
                    {
                        var textMessageReceived = JsonConvert.DeserializeObject<TextMessageReceived>(Convert.ToString(messageReceived)) as TextMessageReceived;
                        textMessage = new List<TextMessage>(textMessageReceived.Entry.SelectMany(x => x.Changes).SelectMany(x => x.Value.Messages));
                        
                        MarkMessageRequest markMessageRequest = new MarkMessageRequest();
                        markMessageRequest.MessageId = textMessage.SingleOrDefault().Id;
                        markMessageRequest.Status = "read";

                        //await _whatsAppBusinessClient.MarkMessageAsReadAsync(markMessageRequest);

                        TextMessageReplyRequest textMessageReplyRequest = new TextMessageReplyRequest();
                        textMessageReplyRequest.Context = new WhatsappBusiness.CloudApi.Messages.ReplyRequests.TextMessageContext();
                        textMessageReplyRequest.Context.MessageId = textMessage.SingleOrDefault().Id;
                        textMessageReplyRequest.To = textMessage.SingleOrDefault().From;
                        textMessageReplyRequest.Text = new WhatsAppText();
                        textMessageReplyRequest.Text.Body = "Your Message was received. Processing the request shortly";
                        textMessageReplyRequest.Text.PreviewUrl = false;

                        //await _whatsAppBusinessClient.SendTextMessageAsync(textMessageReplyRequest);

                        //return Ok(new
                        //{
                        //    Message = "Text Message received"
                        //});
                    }

                    if (messageType.Equals("image"))
                    {
                        var imageMessageReceived = JsonConvert.DeserializeObject<ImageMessageReceived>(Convert.ToString(messageReceived)) as ImageMessageReceived;
                        imageMessage = new List<ImageMessage>(imageMessageReceived.Entry.SelectMany(x => x.Changes).SelectMany(x => x.Value.Messages));
                          
                        MarkMessageRequest markMessageRequest = new MarkMessageRequest();
                        markMessageRequest.MessageId = imageMessage.SingleOrDefault().Id;
                        markMessageRequest.Status = "read";

                        //await _whatsAppBusinessClient.MarkMessageAsReadAsync(markMessageRequest);

                        //return Ok(new
                        //{
                        //    Message = "Image Message received"
                        //});
                    }

                    if (messageType.Equals("audio"))
                    {
                        var audioMessageReceived = JsonConvert.DeserializeObject<AudioMessageReceived>(Convert.ToString(messageReceived)) as AudioMessageReceived;
                        audioMessage = new List<AudioMessage>(audioMessageReceived.Entry.SelectMany(x => x.Changes).SelectMany(x => x.Value.Messages));
                         
                        MarkMessageRequest markMessageRequest = new MarkMessageRequest();
                        markMessageRequest.MessageId = audioMessage.SingleOrDefault().Id;
                        markMessageRequest.Status = "read";

                        //await _whatsAppBusinessClient.MarkMessageAsReadAsync(markMessageRequest);

                        //var mediaUrlResponse = await _whatsAppBusinessClient.GetMediaUrlAsync(audioMessage.SingleOrDefault().Audio.Id);
 
                        // To download media received sent by user
                        // var mediaFileDownloaded = await _whatsAppBusinessClient.DownloadMediaAsync(mediaUrlResponse.Url);

                        //var rootPath = Path.Combine(_webHostEnvironment.WebRootPath, "Application_Files\\MediaDownloads\\");

                        //if (!Directory.Exists(rootPath))
                        //{
                        //    Directory.CreateDirectory(rootPath);
                        //}

                        // Get the path of filename
                        string filename = string.Empty;

                        //if (mediaUrlResponse.MimeType.Contains("audio/mpeg"))
                        //{
                        //    filename = $"{mediaUrlResponse.Id}.mp3";
                        //}

                        //if (mediaUrlResponse.MimeType.Contains("audio/ogg"))
                        //{
                        //    filename = $"{mediaUrlResponse.Id}.ogg";
                        //}

                        //var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Application_Files\\MediaDownloads\\", filename);

                        //await System.IO.File.WriteAllBytesAsync(filePath, mediaFileDownloaded);

                        //return Ok(new
                        //{
                        //    Message = "Audio Message received"
                        //});
                    }

                    if (messageType.Equals("document"))
                    {
                        var documentMessageReceived = JsonConvert.DeserializeObject<DocumentMessageReceived>(Convert.ToString(messageReceived)) as DocumentMessageReceived;
                        documentMessage = new List<DocumentMessage>(documentMessageReceived.Entry.SelectMany(x => x.Changes).SelectMany(x => x.Value.Messages));
                        
                        MarkMessageRequest markMessageRequest = new MarkMessageRequest();
                        markMessageRequest.MessageId = documentMessage.SingleOrDefault().Id;
                        markMessageRequest.Status = "read";

                        //await _whatsAppBusinessClient.MarkMessageAsReadAsync(markMessageRequest);

                        //var mediaUrlResponse = await _whatsAppBusinessClient.GetMediaUrlAsync(documentMessage.SingleOrDefault().Document.Id);
                         
                        //// To download media received sent by user
                        //var mediaFileDownloaded = await _whatsAppBusinessClient.DownloadMediaAsync(mediaUrlResponse.Url);

                        //var rootPath = Path.Combine(_webHostEnvironment.WebRootPath, "Application_Files\\MediaDownloads\\");

                        //if (!Directory.Exists(rootPath))
                        //{
                        //    Directory.CreateDirectory(rootPath);
                        //}

                        // Get the path of filename
                        string filename = string.Empty;

                        //if (mediaUrlResponse.MimeType.Contains("audio/mpeg"))
                        //{
                        //    filename = $"{mediaUrlResponse.Id}.mp3";
                        //}

                        //if (mediaUrlResponse.MimeType.Contains("audio/ogg"))
                        //{
                        //    filename = $"{mediaUrlResponse.Id}.ogg";
                        //}

                        //var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Application_Files\\MediaDownloads\\", filename);

                        //await System.IO.File.WriteAllBytesAsync(filePath, mediaFileDownloaded);

                        //return Ok(new
                        //{
                        //    Message = "Document Message received"
                        //});
                    }

                    if (messageType.Equals("sticker"))
                    {
                        var stickerMessageReceived = JsonConvert.DeserializeObject<StickerMessageReceived>(Convert.ToString(messageReceived)) as StickerMessageReceived;
                        stickerMessage = new List<StickerMessage>(stickerMessageReceived.Entry.SelectMany(x => x.Changes).SelectMany(x => x.Value.Messages));
                       

                        MarkMessageRequest markMessageRequest = new MarkMessageRequest();
                        markMessageRequest.MessageId = stickerMessage.SingleOrDefault().Id;
                        markMessageRequest.Status = "read";

                        //await _whatsAppBusinessClient.MarkMessageAsReadAsync(markMessageRequest);

                        //return Ok(new
                        //{
                        //    Message = "Image Message received"
                        //});
                    }

                    if (messageType.Equals("contacts"))
                    {
                        var contactMessageReceived = JsonConvert.DeserializeObject<ContactMessageReceived>(Convert.ToString(messageReceived)) as ContactMessageReceived;
                        contactMessage = new List<ContactMessage>(contactMessageReceived.Entry.SelectMany(x => x.Changes).SelectMany(x => x.Value.Messages));
                        
                        MarkMessageRequest markMessageRequest = new MarkMessageRequest();
                        markMessageRequest.MessageId = contactMessage.SingleOrDefault().Id;
                        markMessageRequest.Status = "read";

                        //await _whatsAppBusinessClient.MarkMessageAsReadAsync(markMessageRequest);

                        //return Ok(new
                        //{
                        //    Message = "Contact Message Received"
                        //});
                    }


                    if (messageType.Equals("location"))
                    {
                        var locationMessageReceived = JsonConvert.DeserializeObject<StaticLocationMessageReceived>(Convert.ToString(messageReceived)) as StaticLocationMessageReceived;
                        locationMessage = new List<LocationMessage>(locationMessageReceived.Entry.SelectMany(x => x.Changes).SelectMany(x => x.Value.Messages));
                         
                        MarkMessageRequest markMessageRequest = new MarkMessageRequest();
                        markMessageRequest.MessageId = locationMessage.SingleOrDefault().Id;
                        markMessageRequest.Status = "read";

                        //await _whatsAppBusinessClient.MarkMessageAsReadAsync(markMessageRequest);

                        LocationMessageReplyRequest locationMessageReplyRequest = new LocationMessageReplyRequest();
                        locationMessageReplyRequest.Context = new WhatsappBusiness.CloudApi.Messages.ReplyRequests.LocationMessageContext();
                        locationMessageReplyRequest.Context.MessageId = locationMessage.SingleOrDefault().Id;
                        locationMessageReplyRequest.To = locationMessage.SingleOrDefault().From;
                        locationMessageReplyRequest.Location = new WhatsappBusiness.CloudApi.Messages.Requests.Location();
                        locationMessageReplyRequest.Location.Name = "Location Test";
                        locationMessageReplyRequest.Location.Address = "Address Test";
                        locationMessageReplyRequest.Location.Longitude = -122.425332;
                        locationMessageReplyRequest.Location.Latitude = 37.758056;

                        //await _whatsAppBusinessClient.SendLocationMessageAsync(locationMessageReplyRequest);

                        //return Ok(new
                        //{
                        //    Message = "Location Message Received"
                        //});
                    }

                    if (messageType.Equals("button"))
                    {
                        var quickReplyMessageReceived = JsonConvert.DeserializeObject<QuickReplyButtonMessageReceived>(Convert.ToString(messageReceived)) as QuickReplyButtonMessageReceived;
                        quickReplyButtonMessage = new List<QuickReplyButtonMessage>(quickReplyMessageReceived.Entry.SelectMany(x => x.Changes).SelectMany(x => x.Value.Messages));
                         
                        MarkMessageRequest markMessageRequest = new MarkMessageRequest();
                        markMessageRequest.MessageId = quickReplyButtonMessage.SingleOrDefault().Id;
                        markMessageRequest.Status = "read";

                        //await _whatsAppBusinessClient.MarkMessageAsReadAsync(markMessageRequest);

                        //return Ok(new
                        //{
                        //    Message = "Quick Reply Button Message Received"
                        //});
                    }

                    if (messageType.Equals("interactive"))
                    {
                        var getInteractiveType = Convert.ToString(messageReceived["entry"][0]["changes"][0]["value"]["messages"][0]["interactive"]["type"]);

                        if (getInteractiveType.Equals("button_reply"))
                        {
                            var replyMessageReceived = JsonConvert.DeserializeObject<ReplyButtonMessageReceived>(Convert.ToString(messageReceived)) as ReplyButtonMessageReceived;
                            replyButtonMessage = new List<ReplyButtonMessage>(replyMessageReceived.Entry.SelectMany(x => x.Changes).SelectMany(x => x.Value.Messages));
                            
                            MarkMessageRequest markMessageRequest = new MarkMessageRequest();
                            markMessageRequest.MessageId = replyButtonMessage.SingleOrDefault().Id;
                            markMessageRequest.Status = "read";

                            //await _whatsAppBusinessClient.MarkMessageAsReadAsync(markMessageRequest);

                            //return Ok(new
                            //{
                            //    Message = "Reply Button Message Received"
                            //});
                        }

                        if (getInteractiveType.Equals("list_reply"))
                        {
                            var listReplyMessageReceived = JsonConvert.DeserializeObject<ListReplyButtonMessageReceived>(Convert.ToString(messageReceived)) as ListReplyButtonMessageReceived;
                            listReplyButtonMessage = new List<ListReplyButtonMessage>(listReplyMessageReceived.Entry.SelectMany(x => x.Changes).SelectMany(x => x.Value.Messages));
                            
                            MarkMessageRequest markMessageRequest = new MarkMessageRequest();
                            markMessageRequest.MessageId = listReplyButtonMessage.SingleOrDefault().Id;
                            markMessageRequest.Status = "read";

                            //await _whatsAppBusinessClient.MarkMessageAsReadAsync(markMessageRequest);

                            //return Ok(new
                            //{
                            //    Message = "List Reply Message Received"
                            //});
                        }
                    }
                }
                return actionResult;
            }
            catch (WhatsappBusinessCloudAPIException ex)
            { 
                return null;
            }
        }

    }
}