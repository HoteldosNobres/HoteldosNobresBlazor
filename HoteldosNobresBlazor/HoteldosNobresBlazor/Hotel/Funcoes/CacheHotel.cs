using Google.Apis.PeopleService.v1.Data;
using HoteldosNobresBlazor.Classes;
using HoteldosNobresBlazor.Classes.PagSeguroRecebe;
using HoteldosNobresBlazor.Classes.SicoobRecebe;
using HoteldosNobresBlazor.Components.Pages;
using HoteldosNobresBlazor.Services;
using Microsoft.Win32;
using Newtonsoft.Json;
using OpenAC.Net.Core.Extensions;
using pix_payload_generator.net.Models.CobrancaModels;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using WhatsappBusiness.CloudApi.Messages.Requests;
using WhatsappBusiness.CloudApi.Webhook;

namespace HoteldosNobresBlazor.Funcoes
{
    public class CacheHotel
    {
        static TimeZoneInfo brazilTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
        static string cache = "cache";
        static int count = 0;
        static AppState AppState;

        public CacheHotel()
        {
        }

        public CacheHotel(AppState appState)
        {
            AppState = appState;
        }

        #region Whatsapp 

        public string RecebeMensagem(string json)
        {
            try
            {
                Thread thread = new Thread(new ParameterizedThreadStart(RecebeMensagemMetodo));
                thread.Start(json);

                return "OK ";
            }
            catch (Exception e)
            {
                AppState.MyMessageLogWhatsapp = e.Message + "\n";
                return e.Message;
            }

        }

        public void RecebeMensagemMetodo(object objeto)
        {
            string json = (string)objeto;
            try
            {
                var teste = FunctionWhatsApp.ReceiveWhatsAppTextMessage(json);

                MensagemWhatsApp mensagem = FunctionAPICLOUDBEDs.LerRespostaComoObjetoAsync<MensagemWhatsApp>(json).Result;

                string from = mensagem.Entry[0].Changes[0].Value.Messages != null ? mensagem.Entry[0].Changes[0].Value.Messages[0].From :
                    mensagem.Entry[0].Changes[0].Value.Statuses != null ? mensagem.Entry[0].Changes[0].Value.Statuses[0].RecipientId : string.Empty;
                string texto = "";
                string cpf = "";
                string datadenascimento = "";
                string hotelrating = "";
                string resultado = "";

                LogSistema log = new LogSistema()
                {
                    DataLog = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone),
                };


                // Identificar Texto
                if (mensagem.Entry[0].Changes[0].Value.Messages != null && mensagem.Entry[0].Changes[0].Value.Messages[0].Text != null)
                {
                    texto = " WHATSAPP CHAT - Falou: " + mensagem.Entry[0].Changes[0].Value.Messages[0].Text.Body;
                    log.Log = texto;
                    log.Status = "texto";
                }
                else if (mensagem.Entry[0].Changes[0].Value.Messages != null && mensagem.Entry[0].Changes[0].Value.Messages[0].Interactive != null)
                {
                    string jasonresposta = mensagem.Entry[0].Changes[0].Value.Messages[0].Interactive.NfmReply.ResponseJson;
                    Response_Json respostajson = FunctionAPICLOUDBEDs.LerRespostaComoObjetoAsync<Response_Json>(jasonresposta).Result;
                    cpf = respostajson.Cpf;
                    datadenascimento = respostajson.Datadenascimento;

                    hotelrating = respostajson.Hotelrating;
                    string comment_text = respostajson.Comment_text;

                    if (!string.IsNullOrEmpty(cpf) && !string.IsNullOrEmpty(datadenascimento))
                        texto = " CPF: " + cpf + " Data Nascimento: " + datadenascimento + " From: " + from;
                    else
                        texto = " Nota: " + hotelrating + " de 10 Comentario: " + comment_text + " From: " + from;

                    log.Log = texto;
                    log.Status = "texto";
                }
                else if (mensagem.Entry[0].Changes[0].Value.Statuses != null && mensagem.Entry[0].Changes[0].Value.Statuses[0].StatusStatus != null)
                {
                    texto = " WHATSAPP STATUS " + mensagem.Entry[0].Changes[0].Value.Statuses[0].StatusStatus;
                    log.Log = texto;
                    log.Status = mensagem.Entry[0].Changes[0].Value.Statuses[0].StatusStatus;
                }

                if (from != "553584151764" && from != "553537150180" && !texto.ToUpper().Contains("WHATSAPP STATUS"))
                    resultado += FunctionWhatsApp.postMensagem("553537150180", "Numero " + from.Substring(from.Length - 4) + texto).Result;

                List<Reserva> listaReserva = FunctionAPICLOUDBEDs.getReservationsAsyncGuestDetails().Result;
                Reserva reserva = listaReserva.Where(x => x.ProxyCelular.Contains(from)).FirstOrDefault();
                if (reserva is not null)
                {
                    reserva = FunctionAPICLOUDBEDs.getReservationAsync(reserva).Result;
                    log.IDReserva = reserva.IDReserva!;
                }


                // Distribuir Texto
                if (!string.IsNullOrEmpty(cpf))
                {
                    if (reserva is not null)
                    {
                        if (!string.IsNullOrEmpty(datadenascimento))
                        {
                            DateTime birthdate = DateTime.Parse(datadenascimento);
                            reserva.DataNascimento = birthdate;
                            AjustarDataNascimento(reserva);
                        }
                        if (!string.IsNullOrEmpty(cpf))
                        {
                            reserva.Cpf = cpf;
                            AjustarCPF(reserva);
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(hotelrating))
                {
                    if (reserva is not null)
                    {
                        reserva = FunctionAPICLOUDBEDs.postReservationNote(reserva, texto).Result;
                    }
                }
                else if (!string.IsNullOrEmpty(from) && !texto.ToUpper().Contains("WHATSAPP STATUS"))
                    resultado += FunctionWhatsApp.postMensagemTempleteConverse(from).Result;


                //Salvar Texto 
                if (from != "553584151764" && from != "553537150180")
                {
                    if (reserva != null && !string.IsNullOrEmpty(reserva.IDReserva))
                    {
                        if (reserva.Notas.Where(x => x.Texto.Contains("WHATSAPP CHAT")).Count() > 0)
                        {
                            Nota nota = reserva.Notas.Where(x => x.Texto.Contains("WHATSAPP CHAT")).FirstOrDefault();
                            nota.Texto += log.Log;
                            AppState.MyMessageLogWhatsapp += FunctionAPICLOUDBEDs.putReservationNote(reserva.IDReserva!, nota.Id, nota.Texto).Result;
                        }
                        else
                        {
                            if (!texto.Contains("WHATSAPP STATUS"))
                                reserva = FunctionAPICLOUDBEDs.postReservationNote(reserva, log.Log).Result;

                        }

                    }

                }


                if (!string.IsNullOrEmpty(from))
                    log.Log = texto + " From: " + from;

                AppState.ListLogWhatsapp.Add(log);

            }
            catch (Exception e)
            {
                AppState.MyMessageLogWhatsapp = "Erro-" + e.Message + "\n" + json + "\n";
            }
        }


        #endregion Whatsapp

        #region Pagseguro 

        public string RecebePagSeguro(string json)
        {
            try
            {
                Thread thread = new Thread(new ParameterizedThreadStart(RecebePagSeguroMetodo));
                thread.Start(json);

                return "OK ";
            }
            catch (Exception e)
            {
                AppState.MyMessageLogWhatsapp = e.Message + "\n";
                return e.Message;
            }

        }

        public void RecebePagSeguroMetodo(object objeto)
        {
            string json = (string)objeto;
            try
            {
                var log = new LogSistema()
                {
                    DataLog = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone),
                    Log = json,
                    Status = "PagSeguro"
                };

                //FuncoesEmail.EnviarEmail("hoteldosnobres@hotmail.com", json, "Pagseguro");

                AppState.ListLogPagSeguro.Add(log);

            }
            catch (Exception e)
            {
                AppState.MyMessageLogPagSeguro = "Erro-" + e.Message + "\n" + json + "\n";
            }
        }


        #endregion Whatsapp

        #region Siboob 

        public string RecebeSiboob(string json)
        {
            try
            {
                Thread thread = new Thread(new ParameterizedThreadStart(RecebeSicoobMetodo));
                thread.Start(json);

                return "OK ";
            }
            catch (Exception e)
            {
                AppState.MyMessageLogWhatsapp = e.Message + "\n";
                return e.Message;
            }

        }

        public void RecebeSicoobMetodo(object objeto)
        {
            string json = (string)objeto;
            try
            {
                var log = new LogSistema()
                {
                    DataLog = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone),
                    Log = json,
                    Status = "RecebeSicoobMetodo"
                };

                if (json is not null && json.Contains("pix"))
                {
                    SicoobRecebe sicoobRecebe = FunctionAPICLOUDBEDs.LerRespostaComoObjetoAsync<SicoobRecebe>(json).Result;

                    string corpoemail = @"Pix Recebido no SICOOB, " +
                    "<br><br>  ID DA RESERVA: " + sicoobRecebe.Pix[0].Txid +
                    "<br><br>  Valor recebido: " + sicoobRecebe.Pix[0].Valor +
                    "<br><br>  InfoPagador: " + sicoobRecebe.Pix[0].InfoPagador +
                    "<br><br>  Horario: " + sicoobRecebe.Pix[0].Horario +
                    "<br><br>  Obrigado ";

                    FuncoesEmail.EnviarEmailHTML("hoteldosnobres@hotmail.com", corpoemail, "PIX PAGO  - Recebe Sicoob");

                    var reserva = new Reserva();
                    reserva.IDReserva = sicoobRecebe.Pix[0].Txid;
                    reserva = FunctionAPICLOUDBEDs.getReservationAsync(reserva).Result;
                    if (reserva is not null)
                    {
                        reserva.Balance = sicoobRecebe.Pix[0].Valor;
                        reserva.Origem = "Pix";
                        log.Log += FunctionAPICLOUDBEDs.PostReservationPagamento(reserva).Result;
                        log.IDReserva = reserva.IDReserva!;
                    }
                }

                AppState.ListLogPagSeguro.Add(log);

            }
            catch (Exception e)
            {
                AppState.MyMessageLogPagSeguro = "Erro-" + e.Message + "\n" + json + "\n";
            }
        }


        #endregion Whatsapp

        #region Actions 

        public string GetActions(string parametros)
        {
            try
            {
                Thread thread = new Thread(new ParameterizedThreadStart(GetActionsMetodo));
                thread.Start(parametros);

                return "OK ";
            }
            catch (Exception e)
            {
                AppState.MyMessageLogWhatsapp = e.Message + "\n";
                return e.Message;
            }

        }

        public void GetActionsMetodo(object objeto)
        {
            string parametros = (string)objeto;
            try
            {
                AppState.MyMessageReservation += parametros;

                if (parametros.Contains("confirmareserva"))
                {

                    var listReservacheckin = FunctionAPICLOUDBEDs.getReservationsCheckinAsync(DateTime.Now.ToString("yyyy-MM-dd")).Result;
                    foreach (Reserva reserva in listReservacheckin)
                    {
                        var reservaaqui = FunctionAPICLOUDBEDs.getReservationAsync(reserva).Result;

                        LogSistema logSistema = new LogSistema();
                        logSistema.Log = "confirmareserva-";
                        logSistema.IDReserva = reservaaqui.IDReserva.ToString();
                        logSistema.Status = reservaaqui.Status;
                        logSistema.DataLog = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone);

                        string mensagem = "Olá " + reservaaqui.NomeHospede + " , sua reserva foi confirmada, estamos aguardando sua chegada. Confirme seu horario de chegada no link abaixo ou conversa conosco pelo WhatsApp +55 35 37150180" +
                            "Mais informações no link " + reservaaqui.LinkPublico;
                        AppState.MyMessageReservation += FunctionWhatsApp.postMensagem(reservaaqui.ProxyCelular!, mensagem).Result;
                        AppState.MyMessageReservation += FunctionWhatsApp.postMensagemTempleteInicial(reservaaqui.ProxyCelular!, reservaaqui.IDReserva!, reservaaqui.NomeHospede!).Result;

                        if (reservaaqui is not null && reservaaqui.Origem is not null && reservaaqui.Origem!.ToUpper().Contains("BOOKING.COM"))
                        {
                            FuncoesEmail.EnviarEmail(reservaaqui.Email, mensagem, "HORARIO DO CHECK-IN");
                        }

                        logSistema.Log += "Enviado";
                        AppState.ListLogSistemaAddReserva.Add(logSistema);
                    }

                }

                if (parametros.Contains("limparHotel"))
                {

                    var listReservacheckin = FunctionAPICLOUDBEDs.getReservationsCheckinAsync(DateTime.Now.ToString("yyyy-MM-dd")).Result;
                    foreach (Reserva reserva in listReservacheckin)
                    {
                        var reservaaqui = FunctionAPICLOUDBEDs.getReservationAsync(reserva).Result;

                        LogSistema logSistema = new LogSistema();
                        logSistema.Log = "confirmareserva-";
                        logSistema.IDReserva = reservaaqui.IDReserva.ToString();
                        logSistema.Status = reservaaqui.Status;
                        logSistema.DataLog = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone);

                        foreach(Quarto quarto in reservaaqui.ListaQuartos)
                        {
                            if(quarto.ID is not null)
                                logSistema.Log += FunctionAPICLOUDBEDs.PostHousekeepingStatus(quarto.ID!, "clean").Result;
                        } 
                        logSistema.Log += "- Quarto marcado como Limpo";
                        AppState.ListLogSistemaAddReserva.Add(logSistema);
                    }

                }

            }
            catch (Exception e)
            {
                AppState.MyMessageReservation = "Erro-" + e.Message + "\n" + parametros + "\n";
            }
        }


        #endregion Whatsapp

        #region CloudBeds 

        public async Task<string> CacheCreateReservationAsync(string json)
        {
            try
            {
                CreateReservation create = FunctionAPICLOUDBEDs.LerRespostaComoObjetoAsync<CreateReservation>(json).Result;

                if (string.IsNullOrEmpty(create.reservationId))
                    throw new Exception("ID da reserva não informado");

                Reserva reserva = new Reserva();
                reserva.IDReserva = create.reservationId;


                Thread thread = new Thread(new ParameterizedThreadStart(CreateReservationMetodo));
                thread.Start(reserva);

                return "OK ";

            }
            catch (Exception e)
            {
                AppState.MyMessageReservation = "CacheCreateReservationAsyncÇ " + e.Message + "\n";
                return e.Message;
            }
        }

        private void CreateReservationMetodo(object objeto)
        {
            try
            {
                Reserva novareserva = (Reserva)objeto;
                novareserva = FunctionAPICLOUDBEDs.getReservationAsync(novareserva).Result;

                LogSistema logSistema = new LogSistema();
                logSistema.Log = "CreateReservation-";
                logSistema.IDReserva = novareserva.IDReserva.ToString();
                logSistema.Status = novareserva.Status;
                logSistema.DataLog = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone);
                novareserva = FunctionAPICLOUDBEDs.getReservationAsync(novareserva).Result;

                if (!novareserva.Equals(null) && novareserva.Origem is not null && novareserva.Origem.Contains("Despegar/Decolar"))
                {
                    logSistema.Log += AjustarEndereco(novareserva);
                    if (novareserva.GuestID != null)
                        logSistema.Log += FunctionAPICLOUDBEDs.putGuest(novareserva.GuestID, "guestEmail", "reserva@DESPEGAR.com").Result;
                    logSistema.Log += FunctionAPICLOUDBEDs.putGuest(novareserva.GuestID, "guestState", "Minas Gerais").Result;
                    logSistema.Log += FunctionAPICLOUDBEDs.putGuest(novareserva.GuestID, "guestCountry", "BR").Result;

                    logSistema.Log += PagamentoReserva(novareserva);
                }

                if (!novareserva.Equals(null) && novareserva.Origem is not null && novareserva.Origem.Contains("CVC"))
                {
                    logSistema.Log += AjustarEndereco(novareserva);
                    if (novareserva.GuestID != null)
                        logSistema.Log += FunctionAPICLOUDBEDs.putGuest(novareserva.GuestID, "guestEmail", "reserva@cvc.com").Result;
                    logSistema.Log += FunctionAPICLOUDBEDs.putGuest(novareserva.GuestID, "guestState", "Minas Gerais").Result;
                    logSistema.Log += FunctionAPICLOUDBEDs.putGuest(novareserva.GuestID, "guestCountry", "BR").Result;

                    logSistema.Log += PagamentoReserva(novareserva);
                }

                if (!novareserva.Equals(null) && novareserva.Origem is not null && novareserva.Origem.Contains("Airbnb"))
                {
                    logSistema.Log += AjustarEndereco(novareserva);
                    if (novareserva.GuestID != null)
                        logSistema.Log += FunctionAPICLOUDBEDs.putGuest(novareserva.GuestID, "guestEmail", novareserva.IDReservaAgencia + "@airbnb.com").Result;
                    logSistema.Log += FunctionAPICLOUDBEDs.putGuest(novareserva.GuestID, "guestState", "Minas Gerais").Result;
                    logSistema.Log += FunctionAPICLOUDBEDs.putGuest(novareserva.GuestID, "guestCountry", "BR").Result;

                    logSistema.Log += PagamentoReserva(novareserva);
                }

                if (novareserva is not null && novareserva.Origem is not null && novareserva.Origem!.ToUpper().Contains("BOOKING.COM"))
                {
                    logSistema.Log += FunctionWhatsApp.postMensagemTempleteDadosFaltando(novareserva.ProxyCelular!, novareserva.IDReserva!, novareserva.NomeHospede!, novareserva.LinkPublico).Result;
                    FuncoesEmail.EnviarEmailCPF(novareserva.Email, novareserva.IDReserva, novareserva.NomeHospede);
                    FuncoesEmail.EnviarEmailSuporte(novareserva.IDReserva, novareserva.NomeHospede, 3);

                    var novareservaratedetails = FunctionAPICLOUDBEDs.getReservationsWithRateDetailsAsync(novareserva).Result;
                    if (novareservaratedetails is not null && novareservaratedetails.Source is not null && novareservaratedetails.Source.PaymentCollect.ToLower().Equals("collect"))
                    {
                        logSistema.Log += PagamentoReserva(novareserva);
                    }

                }

                if (novareserva is not null && novareserva.Origem is not null && novareserva.Origem!.ToUpper().Contains("EXPEDIA"))
                {
                    logSistema.Log += FunctionWhatsApp.postMensagemTempleteDadosFaltando(novareserva.ProxyCelular!, novareserva.IDReserva!, novareserva.NomeHospede!, novareserva.LinkPublico).Result;
                    FuncoesEmail.EnviarEmailCPF(novareserva.Email, novareserva.IDReserva, novareserva.NomeHospede);

                }

                if (novareserva is not null && novareserva.Origem is not null &&
                    (novareserva.Origem!.Contains("Website/Booking Engine") || novareserva.Origem!.Contains("WhatsApp")))
                {
                    if (novareserva.Balance > 0)
                    {
                        var valordecimal = novareserva!.BalanceDetailed!.GrandTotal - novareserva!.BalanceDetailed!.Paid;
                        var valor = valordecimal.ToString("N2", new CultureInfo("pt-BR"));

                        string stringToQrCode = FunctionSicoob.QrCode(novareserva!, valor!).Result;

                        logSistema.Log += stringToQrCode;

                        logSistema.Log += FunctionWhatsApp.postMensagemTempletePIX(novareserva.ProxyCelular!, novareserva.IDReserva!, stringToQrCode).Result;

                        string corpoemail = @"Ola recebemos sua reserva, " +
                        "<br><br>  Temos mais informações no link " + novareserva.LinkPublico +
                        "<br><br>  Obrigado ";

                        FuncoesEmail.EnviarEmailHTML(novareserva.Email!, corpoemail, "MAIS INFORMAÇÕES E QRCODE DO PIX");


                    }

                    FuncoesEmail.EnviarEmailSuporte(novareserva.IDReserva, novareserva.NomeHospede, 3);
                }

                if (novareserva != null && novareserva.Estado != null && novareserva.CEP != null)
                {
                    logSistema.Log += AjustarEndereco(novareserva);
                }

                novareserva = FunctionAPICLOUDBEDs.getReservationAsync(novareserva).Result;

                logSistema.Log = SaveOrUpdateFNRH(novareserva);


                if (logSistema.Log.Contains("SNRHos-MS0001") && !novareserva.ProxyCelular!.Equals("553537150180"))
                {
                    logSistema.Log += FunctionWhatsApp.postMensagemTempleteInicial(novareserva.ProxyCelular!, novareserva.IDReserva!, novareserva.NomeHospede!).Result;
                }
                else
                  if (logSistema.Log.Contains("CPF inválido"))
                {
                    if (novareserva.Cpf != null && !ValidarCPF(novareserva.Cpf) && !novareserva.ProxyCelular!.Equals("553537150180"))
                        logSistema.Log += FunctionWhatsApp.postMensagemTempleteDadosFaltando(novareserva.ProxyCelular!, novareserva.IDReserva!, novareserva.NomeHospede!, novareserva.LinkPublico).Result;
                }

                if (novareserva is not null && string.IsNullOrEmpty(novareserva.Cpf) && !novareserva.ProxyCelular!.Equals("553537150180"))
                {
                    logSistema.Log += FunctionWhatsApp.postMensagemTempleteDadosFaltando(novareserva.ProxyCelular!, novareserva.IDReserva!, novareserva.NomeHospede!, novareserva.LinkPublico).Result;
                }


                if (!string.IsNullOrEmpty(novareserva.ProxyCelular))
                {
                    logSistema.Log += FunctionGoogle.AddPeople(novareserva.NomeHospede, novareserva.Origem, novareserva.ProxyCelular, novareserva.Email.ToString());
                }

                logSistema.Log += AjusteRate(novareserva.IDReserva);

                //E-mail para o suporte Cloubeds
                FuncoesEmail.EnviarEmailSuporte(novareserva.IDReserva, novareserva.NomeHospede, 1);



                AppState.ListLogSistemaAddReserva.Add(logSistema);
            }
            catch (Exception e)
            {
                AppState.MyMessageReservation = "CreateReservation_Metodo " + e.Message + "\n";
            }

        }

        private string AjusteRate(string reservaId)
        {
            try
            {
                int desconto = 2;
                string retorno = "";
                Reserva reservalocal = new Reserva();
                reservalocal.IDReserva = reservaId;
                reservalocal = FunctionAPICLOUDBEDs.getReservationAsync(reservalocal).Result;

                if (!reservalocal.Equals(null) && reservalocal.Origem is not null
                    && !reservalocal.Origem.Contains("Phone")
                    && !reservalocal.Origem.Contains("Walk-In"))
                {
                    if (reservalocal.ListaQuartos != null && (reservalocal.ListaQuartos.Count() > 0))
                    {
                        foreach (Quarto quarto in reservalocal.ListaQuartos)
                        {
                            Rate rate = FunctionAPICLOUDBEDs.getRatesAsync(quarto.TypeID.ToString(), reservalocal.DataCheckIn.GetValueOrDefault(), reservalocal.DataCheckOut.GetValueOrDefault()).Result;
                            if (rate.Success)
                            {
                                foreach (RoomRateDetailed roomRateDetailed in rate.Data.RoomRateDetailed)
                                {
                                    decimal valornovo = roomRateDetailed.Rate;
                                    valornovo += (roomRateDetailed.Rate * desconto / 100);
                                    string stringvalor = valornovo.ToString("N", new CultureInfo("en-US"));
                                    retorno = FunctionAPICLOUDBEDs.postRateAsync(rate.Data.RateId, roomRateDetailed.Date.DateTime, roomRateDetailed.Date.DateTime, stringvalor).Result + " \n";

                                }

                            }
                        }
                    }

                    if (reservalocal.ListaQuartosCancelados != null && (reservalocal.ListaQuartosCancelados.Count() > 0))
                    {
                        foreach (Quarto quarto in reservalocal.ListaQuartosCancelados)
                        {
                            Rate rate = FunctionAPICLOUDBEDs.getRatesAsync(quarto.TypeID.ToString(), reservalocal.DataCheckIn.GetValueOrDefault(), reservalocal.DataCheckOut.GetValueOrDefault()).Result;
                            if (rate.Success)
                            {
                                foreach (RoomRateDetailed roomRateDetailed in rate.Data.RoomRateDetailed)
                                {
                                    decimal valornovo = roomRateDetailed.Rate;
                                    valornovo -= (roomRateDetailed.Rate * desconto / 100);
                                    string stringvalor = valornovo.ToString("N", new CultureInfo("en-US"));
                                    if (reservalocal.Status != null && reservalocal.Status.ToUpper() == "CANCELED")
                                        retorno = FunctionAPICLOUDBEDs.postRateAsync(rate.Data.RateId, roomRateDetailed.Date.DateTime, roomRateDetailed.Date.DateTime, stringvalor).Result + " \n";

                                }

                            }
                        }
                    }
                }


                return retorno;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string CacheAccommodation_changed(string json)
        {
            try
            {
                Accommodation_changed changed = FunctionAPICLOUDBEDs.LerRespostaComoObjetoAsync<Accommodation_changed>(json).Result;

                if (string.IsNullOrEmpty(changed.reservationId))
                    throw new Exception("ID da reserva não informado");

                Reserva reserva = new Reserva();
                reserva.IDReserva = changed.reservationId;


                Thread thread = new Thread(new ParameterizedThreadStart(Accommodation_changedMetodo));
                thread.Start(reserva);

                return "OK " + changed.reservationId;
            }
            catch (Exception e)
            {
                AppState.MyMessageReservation = e.Message + "\n";
                return e.Message;
            }

        }

        private void Accommodation_changedMetodo(object objeto)
        {
            try
            {
                Reserva reserva = (Reserva)objeto;
                reserva = FunctionAPICLOUDBEDs.getReservationAsync(reserva).Result;

                LogSistema logSistema = new LogSistema();
                logSistema.IDReserva = reserva.IDReserva.ToString();
                logSistema.Status = reserva.Status;
                logSistema.DataLog = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone);
                logSistema.Log = "Accommodation_changed-";

                string retorno = "";

                logSistema.Log += retorno + " ";
                AppState.ListLogSistemaAddReserva.Add(logSistema);

            }
            catch (Exception e)
            {
                AppState.MyMessageReservation = e.Message + "\n";
            }

        }

        public string CacheChangedStatus(string json)
        {
            try
            {
                ChangedReservation changed = FunctionAPICLOUDBEDs.LerRespostaComoObjetoAsync<ChangedReservation>(json).Result;

                if (string.IsNullOrEmpty(changed.reservationId))
                    throw new Exception("ID da reserva não informado");

                Reserva reserva = new Reserva();
                reserva.IDReserva = changed.reservationId;


                Thread thread = new Thread(new ParameterizedThreadStart(ChangedStatusMetodo));
                thread.Start(reserva);

                return "OK " + changed.reservationId;
            }
            catch (Exception e)
            {
                AppState.MyMessageReservation = e.Message + "\n";
                return e.Message;
            }

        }

        private void ChangedStatusMetodo(object objeto)
        {
            try
            {
                Reserva reserva = (Reserva)objeto;
                reserva = FunctionAPICLOUDBEDs.getReservationAsync(reserva).Result;

                LogSistema logSistema = new LogSistema();
                logSistema.Log = "ChangedReservation-";
                logSistema.IDReserva = reserva.IDReserva.ToString();
                logSistema.Status = reserva.Status;
                logSistema.DataLog = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone);

                string retorno = "";
                DateTime brazilTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone);

                if (reserva is null || reserva.Status is null)
                    throw new Exception("Reserva ou Status não informado");

                if (reserva.Status.ToUpper() == "CHECKED_IN")
                {
                    bool voltar = false;
                    if (reserva != null)
                    {
                        if (string.IsNullOrEmpty(reserva.Cpf) || (reserva.Cpf != null && !ValidarCPF(reserva.Cpf)))
                        {
                            if (reserva.Notas is not null && reserva.Notas.Where(x => x.Texto == "CPF inválido").Count() > 0)
                            {
                                retorno += FunctionAPICLOUDBEDs.putReservationNote(reserva.IDReserva!, reserva.Notas.FirstOrDefault(x => x.Texto == "CPF inválido").Id!, retorno).Result;
                            }
                            else
                                logSistema.Log += FunctionAPICLOUDBEDs.postReservationNote(reserva.IDReserva, "CPF inválido").Result;
                            voltar = true;
                        }

                    }

                    if (reserva!.ProxyCelular is null || reserva.ProxyCelular.Equals("5535999429553")
                        || reserva.ProxyCelular.Equals("553537150180"))
                    {
                        logSistema.Log += FunctionAPICLOUDBEDs.postReservationNote(reserva.IDReserva, "Reserva sem telefone").Result;
                        voltar = true;
                    }

                    if (reserva.Email is not null && reserva.Email.Equals("hoteldosnobres@hotmail.com"))
                    {
                        logSistema.Log += FunctionAPICLOUDBEDs.postReservationNote(reserva.IDReserva, "Reserva e-mail do hotel").Result;
                        voltar = true;
                    }

                    if (voltar)
                    {
                        if (!reserva.ProxyCelular!.Equals("553537150180"))
                            logSistema.Log += FunctionWhatsApp.postMensagemTempleteDadosFaltando(reserva.ProxyCelular!, reserva.IDReserva!, reserva.NomeHospede!, reserva.LinkPublico!).Result;


                        logSistema.Log += FunctionAPICLOUDBEDs.putReservation(reserva.IDReserva, "confirmed").Result;
                    }

                }

                if (reserva.Status.ToUpper() == "CANCELED")
                {
                    logSistema.Log += AjusteRate(reserva.IDReserva);

                    if (reserva is not null && reserva.Origem is not null && reserva.Origem!.ToUpper().Contains("BOOKING.COM"))
                    {
                        var novareservaratedetails = FunctionAPICLOUDBEDs.getReservationsWithRateDetailsAsync(reserva).Result;
                        if (novareservaratedetails is not null && novareservaratedetails.Source is not null && novareservaratedetails.Source.PaymentCollect.ToLower().Equals("collect"))
                        {
                            logSistema.Log += RemovePagamentoReserva(reserva);
                        }

                    }
                }

                if (reserva!.Status.ToUpper() == "CHECKED_OUT" && !reserva.ProxyCelular!.Equals("553537150180"))
                {
                    logSistema.Log += FunctionWhatsApp.postMensagemTempleteFeedback(reserva.ProxyCelular!, reserva.IDReserva, reserva.NomeHospede!).Result;
                }

                logSistema.Log += CheckInOrCheckOutFNRH(reserva!);

                logSistema.Log += retorno + "\n";

                AppState.ListLogSistemaAddReserva.Add(logSistema);
            }
            catch (Exception e)
            {
                AppState.MyMessageReservation = e.Message + "\n";
            }
        }

        public string CacheDetails_changed(string json)
        {
            try
            {
                Details_changed changed = FunctionAPICLOUDBEDs.LerRespostaComoObjetoAsync<Details_changed>(json).Result;

                Thread thread = new Thread(new ParameterizedThreadStart(CacheDetails_changedMetodo));
                thread.Start(changed);

                return "OK " + changed.reservationId;
            }
            catch (Exception e)
            {
                AppState.MyMessageReservation = e.Message + "\n";
                return e.Message;
            }

        }

        public async void CacheDetails_changedMetodo(object objeto)
        {
            try
            {
                Details_changed changed = (Details_changed)objeto;
                TimeZoneInfo brazilTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");

                List<Reserva> listaReserva = await FunctionAPICLOUDBEDs.getReservationsAsyncGuestDetails();
                Reserva reserva = listaReserva.FirstOrDefault(x => changed.guestID == x.GuestID);
                reserva = await FunctionAPICLOUDBEDs.getReservationAsync(reserva);

                LogSistema logSistema = new LogSistema();
                logSistema.Log = "Details_changed-";
                logSistema.IDReserva = reserva.IDReserva.ToString();
                logSistema.Status = reserva.Status;

                Random random = new Random();
                int numeroSorteado = random.Next(10, 50); // Sorteia um número entre 10 e 50
                Thread.Sleep(numeroSorteado * 1000);

                logSistema.DataLog = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone);

                string retorno = "";
                reserva = await FunctionAPICLOUDBEDs.getReservationAsync(reserva);

                logSistema.Log += SaveOrUpdateFNRH(reserva);

                if (logSistema.Log.Contains("SNRHos-MS0001"))
                {
                    logSistema.Log += FunctionGoogle.AddPeople(reserva.NomeHospede, reserva.Origem, reserva.ProxyCelular, reserva.Email.ToString());

                    logSistema.Log += FunctionWhatsApp.postMensagemTempleteInicial(reserva.ProxyCelular!, reserva.IDReserva!, reserva.NomeHospede!).Result;
                    logSistema.Log += FunctionWhatsApp.postMensagemTempleteConverse(reserva.ProxyCelular).Result;
                }

                logSistema.Log += retorno + " ";

                AppState.ListLogSistemaAddReserva.Add(logSistema);
            }
            catch (Exception e)
            {
                AppState.MyMessageReservation = "Id: " + ((Details_changed)objeto).reservationId + " e.Message:" + e.Message + "\n";
            }

        }

        public void CacheExecutanado()
        {
            Thread threadreserva = new Thread(ListaReservaMetodo);
            threadreserva.Start();

            Thread thread = new Thread(NovoMetodo);
            thread.Start();

            Thread thread2 = new Thread(FNRHMetodo);
            thread2.Start();

            Thread thread3 = new Thread(PagamentoMetodo);
            thread3.Start();

            //Reserva reserva = new Reserva();
            //reserva.IDReserva = "9317965093589";
            //reserva = FunctionAPICLOUDBEDs.getReservationAsync(reserva).Result;
            //AjustarEndereco(reserva);
            //string log;
            ////log = FunctionWhatsApp.postMensagemTemplete(reserva.ProxyCelular, "inf_mtur").Result;
            ////log += FunctionWhatsApp.postMensageFlowCPF(reserva.ProxyCelular).Result;
            //log = FunctionWhatsApp.postMensagemTemplete("5511998958811", "inf_inicial").Result;



        }

        #endregion CloudBeds

        static void ListaReservaMetodo()
        {
            try
            {
                List<Reserva> listReserva = FunctionAPICLOUDBEDs.getReservationsAsyncGuestDetails().Result;

                string datanow = DateTime.Now.ToString("yyyy-MM-dd");
                listReserva.AddRange(FunctionAPICLOUDBEDs.getReservationsCheckinAsync(datanow).Result);
                listReserva.AddRange(FunctionAPICLOUDBEDs.getReservationsCheckOutAsync(datanow).Result);

                datanow = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
                listReserva.AddRange(FunctionAPICLOUDBEDs.getReservationsCheckinAsync(datanow).Result);
                listReserva.AddRange(FunctionAPICLOUDBEDs.getReservationsCheckOutAsync(datanow).Result);

                datanow = DateTime.Now.AddDays(2).ToString("yyyy-MM-dd");
                listReserva.AddRange(FunctionAPICLOUDBEDs.getReservationsCheckinAsync(datanow).Result);
                listReserva.AddRange(FunctionAPICLOUDBEDs.getReservationsCheckOutAsync(datanow).Result);


                datanow = DateTime.Now.AddDays(3).ToString("yyyy-MM-dd");
                listReserva.AddRange(FunctionAPICLOUDBEDs.getReservationsCheckinAsync(datanow).Result);
                listReserva.AddRange(FunctionAPICLOUDBEDs.getReservationsCheckOutAsync(datanow).Result);

                datanow = DateTime.Now.AddDays(4).ToString("yyyy-MM-dd");
                listReserva.AddRange(FunctionAPICLOUDBEDs.getReservationsCheckinAsync(datanow).Result);
                listReserva.AddRange(FunctionAPICLOUDBEDs.getReservationsCheckOutAsync(datanow).Result);


                listReserva = listReserva.GroupBy(x => x.IDReserva).Select(x => x.First()).ToList();

                AppState.ListReservas = listReserva;

                AppState.MyMessage = cache + " Data: " + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone).ToString("yyyy-MM-dd HH:mm:ss") + "\n";
            }
            catch (Exception e)
            {
                AppState.MyMessage = e.Message + "\n";
            }
        }

        static void NovoMetodo()
        {
            while (true)
            {
                try
                {
                    AppState.ListReservas = FunctionAPICLOUDBEDs.getReservationsAsyncGuestDetails().Result;
                    AppState.ListReservas.AddRange(FunctionAPICLOUDBEDs.getReservationsCheckinAsync(DateTime.Now.ToString("yyyy-MM-dd")).Result);
                    AppState.ListReservas.AddRange(FunctionAPICLOUDBEDs.getReservationsCheckOutAsync(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd")).Result);
                    AppState.ListReservas.AddRange(FunctionAPICLOUDBEDs.getReservationsCheckOutAsync(DateTime.Now.AddDays(2).ToString("yyyy-MM-dd")).Result);
                    AppState.ListReservas.AddRange(FunctionAPICLOUDBEDs.getReservationsCheckOutAsync(DateTime.Now.AddDays(3).ToString("yyyy-MM-dd")).Result);
                    AppState.ListReservas.AddRange(FunctionAPICLOUDBEDs.getReservationsCheckOutAsync(DateTime.Now.AddDays(4).ToString("yyyy-MM-dd")).Result);
                    AppState.ListReservas.AddRange(FunctionAPICLOUDBEDs.getReservationsCheckOutAsync(DateTime.Now.AddDays(5).ToString("yyyy-MM-dd")).Result);
                    AppState.ListReservas.AddRange(FunctionAPICLOUDBEDs.getReservationsCheckOutAsync(DateTime.Now.AddDays(6).ToString("yyyy-MM-dd")).Result);
                    AppState.ListReservas.AddRange(FunctionAPICLOUDBEDs.getReservationsCheckOutAsync(DateTime.Now.AddDays(7).ToString("yyyy-MM-dd")).Result);

                    //Cache addcontato
                    foreach (Reserva reserva in AppState.ListReservas)
                    {
                        Reserva novareserva = FunctionAPICLOUDBEDs.getReservationAsync(reserva).Result;
                        if (!string.IsNullOrEmpty(novareserva.ProxyCelular))
                        {
                            FunctionGoogle.AddPeople(novareserva.NomeHospede, novareserva.Origem, novareserva.ProxyCelular, novareserva.Email.ToString());
                        }
                    }

                    count = count + 1;
                    cache = count.ToString();
                    //Console.WriteLine("Cache executando");

                    AppState.MyMessage = cache + " Data: " + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone).ToString("yyyy-MM-dd HH:mm:ss") + "\n";

                    Thread.Sleep(60000);
                }
                catch (Exception e)
                {
                    AppState.MyMessage = e.Message + "\n";
                    Thread.Sleep(5000);
                    continue;
                }

            }
        }

        static void FNRHMetodo()
        {
            while (true)
            {
                try
                {
                    TimeZoneInfo brazilTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
                    DateTime brazilTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone);
                    AppState.MyMessageFNRH = "Começou a Rodar" + " Data: " + brazilTime.ToString("yyyy-MM-dd HH:mm:ss") + "\n";
                    AppState.ListLogSistemaFNRH.Clear();

                    List<Reserva> listReserva = new List<Reserva>();
                    listReserva = FunctionAPICLOUDBEDs.getReservationsAsync(DateTime.Now.ToString("yyyy-MM-dd")).Result;
                    listReserva.AddRange(FunctionAPICLOUDBEDs.getReservationsAsync(null, DateTime.Now.ToString("yyyy-MM-dd")).Result);
                    listReserva.AddRange(FunctionAPICLOUDBEDs.getReservationsAsyncGuestDetails().Result);

                    foreach (Reserva reserva1 in listReserva)
                    {
                        LogSistema logSistema = new LogSistema();
                        logSistema.IDReserva = reserva1.IDReserva.ToString();
                        logSistema.Status = reserva1.Status;
                        logSistema.DataLog = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone);

                        Reserva reserva = FunctionAPICLOUDBEDs.getReservationAsync(reserva1).Result;
                        string retorno = string.Empty;

                        retorno += SaveOrUpdateFNRH(reserva);

                        retorno += CheckInOrCheckOutFNRH(reserva);

                        logSistema.Log += retorno + " ";
                        AppState.ListLogSistemaFNRH.Add(logSistema);
                    }

                    AppState.MyMessageFNRH += "Terminou! " + " Data: " + brazilTime.ToString("yyyy-MM-dd HH:mm:ss") + "\n";

                    Thread.Sleep(1800 * 1000);
                }
                catch (Exception e)
                {
                    AppState.MyMessage = e.Message + "\n";
                    Thread.Sleep(5000);

                }
            }


        }

        private static string SaveOrUpdateFNRH(Reserva reserva)
        {
            string retorno = string.Empty;
            string mensagem = string.Empty;

            if (string.IsNullOrEmpty(reserva.SnNum) && reserva.Status!.ToUpper() != "CHECKED_OUT" && reserva.Status.ToUpper() != "CANCELED")
                retorno = FuncoesFNRH.Inserir(reserva);
            else if (!string.IsNullOrEmpty(reserva.SnNum) && reserva.Status.ToUpper() != "CANCELED")
                retorno = FuncoesFNRH.Atualizar(reserva);

            retorno = retorno.Replace("Não foi possivel gravar o registro", "");

            if (retorno.Contains("SNRHos-MS0001") || retorno.Contains("SNRHos-MS0002"))
            {
                reserva.SnNum = Regex.Replace(retorno, @"SNRHos-MS000[1-4]\(|\)", "");
                mensagem = "SNRHos-MS0001(" + reserva.SnNum + ")"; 
            }

            if (retorno.Contains("SNRHos-ME"))
            {
                var match = Regex.Match(retorno!, @"SNRHos-ME00[0-9][0-9]");
                string sNRHosME = match.Success ? match.Groups[0].Value : string.Empty;

                int mensagemerro = Convert.ToInt32(Regex.Replace(sNRHosME, @"SNRHos-ME0", ""));
                var sNRHosException = (SNRHosException)mensagemerro;

                mensagem = sNRHosException.GetDescription();

                if (mensagemerro == 31)
                    return mensagem; 

            } 

            reserva.Notas = reserva.Notas ?? new List<Nota>();
            retorno = mensagem;

            if (reserva.Notas.Where(x => x.Texto == mensagem).Count() == 0)
            {  

                if (reserva.Notas.Where(x => x.Texto.Contains("SNRHos-ME")).Count() > 0)
                {
                    retorno += FunctionAPICLOUDBEDs.putReservationNote(reserva.IDReserva!, reserva.Notas!.FirstOrDefault(x => x.Texto.Contains("SNRHos-ME")).Id!, mensagem).Result;
                }
                else if (reserva.Notas.Where(x => x.Texto.Contains("SNRHos-MS0002")).Count() > 0)
                {
                    retorno += FunctionAPICLOUDBEDs.putReservationNote(reserva.IDReserva!, reserva.Notas!.FirstOrDefault(x => x.Texto.Contains("SNRHos-MS0002")).Id!, mensagem).Result;
                }else if (reserva.Notas.Where(x => x.Texto!.Contains(reserva.SnNum!)).Count() > 0)
                {
                    retorno = mensagem;
                }
                else
                {
                    reserva = FunctionAPICLOUDBEDs.postReservationNote(reserva, retorno).Result; 
                }
            }

            return retorno;
        }

        private static string CheckInOrCheckOutFNRH(Reserva reserva)
        {
            string retorno = string.Empty;

            if (!string.IsNullOrEmpty(reserva.SnNum))
            {
                retorno += " SnNum: " + reserva.SnNum + " ";
                CheckInFNRH(reserva);

                if (reserva.Status.ToUpper() == "CHECKED_OUT" || reserva.Status.ToUpper() == "CANCELED")
                    CheckOutFNRH(reserva);

            }

            retorno = retorno.Replace("SNRHos-ME0024", " Checkin realizado não permitido \n");
            retorno = retorno.Replace("SNRHos-ME0025", " Checkout realizado não permitido \n");

            return retorno;
        }

        private static string CheckInFNRH(Reserva reserva)
        {
            string retorno = string.Empty;

            if ((reserva.Status!.ToUpper() == "HOSPEDADO" || reserva.Status.ToUpper() == "CHECKED_IN" || reserva.Status.ToUpper() == "CHECKED_OUT" || reserva.Status.ToUpper() == "CHECK OUT FEITO")
                && reserva.Notas is not null)
            {
                reserva.Notas = reserva.Notas.Where(x => x.Texto is not null && x.Texto.Contains("SNRHos-")).ToList();

                foreach (var nota in reserva.Notas)
                {
                    if (nota.Texto is not null && nota.Texto!.Contains("SNRHos-MS0001(") && !string.IsNullOrEmpty(nota.Id))
                    {
                        string reservationNoteID = nota.Id.ToString();

                        if (nota.Texto.Contains("SNRHos-MS"))
                            reserva.SnNum = Regex.Replace(nota.Texto, @"SNRHos-MS000[1-4]\(|\)", "");

                        if (nota.Texto.Contains("SNRHos-MS0001(" + reserva.SnNum + ")"))
                        {
                            // PEGAR HORARIO DO CHCKin reserva = FunctionAPICLOUDBEDs.getReservationAsync(reserva).Result;
                            reserva.DataCheckInRealizado = DateTime.Parse(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone).ToString("yyyy-MM-dd HH:mm:ss"));
                            retorno += FuncoesFNRH.CheckIn(reserva);
                            retorno += FunctionAPICLOUDBEDs.putReservationNote(reserva.IDReserva!, reservationNoteID, "SNRHos-MS0003(" + reserva.SnNum + ")").Result;

                        }
                        else
                        {
                            reserva.ObsFnhr += " SNRHos duplicado ";
                            retorno = FuncoesFNRH.Atualizar(reserva);

                            reserva.DataCheckInRealizado = DateTime.Parse(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone).ToString("yyyy-MM-dd HH:mm:ss"));
                            retorno += FuncoesFNRH.CheckIn(reserva);

                            reserva.DataCheckOutRealizado = DateTime.Parse(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone).ToString("yyyy-MM-dd HH:mm:ss"));
                            retorno += FuncoesFNRH.CheckOut(reserva);

                            retorno += FunctionAPICLOUDBEDs.putReservationNote(reserva.IDReserva!, reservationNoteID, nota.Texto!.Replace("SNRHos-MS0001(", "SNRHos-MS0004(")).Result;
                        }
                    }
                }




            }
            return retorno;
        }

        private static string CheckOutFNRH(Reserva reserva)
        {
            string retorno = string.Empty;
            string reservationNoteID = reserva.Notas.Where(x => x.Texto.Contains("SNRHos-")).FirstOrDefault().Id!.ToString();

            if (reserva.Status is not null
                && (reserva.Status.ToUpper() == "CHECKED_OUT" || reserva.Status.ToUpper() == "CANCELED")
                && !string.IsNullOrEmpty(reserva.SnNum))
            {
                reserva.DataCheckOutRealizado = DateTime.Parse(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone).ToString("yyyy-MM-dd HH:mm:ss"));
                retorno = FuncoesFNRH.CheckOut(reserva);

                if (retorno.Contains("SNRHos-MS0004") && !string.IsNullOrEmpty(reservationNoteID))
                    retorno += FunctionAPICLOUDBEDs.putReservationNote(reserva.IDReserva!, reservationNoteID, "SNRHos-MS0004(" + reserva.SnNum + ")").Result;
            }

            if (retorno.Contains("SNRHos-MS0004"))
            {
                retorno += FunctionAPICLOUDBEDs.deleteReservationNote(reserva, "SNRHos-MS0004").Result;
            }
            return retorno;
        }



        static void PagamentoMetodo()
        {
            while (true)
            {
                try
                {
                    string brazilTimeZoneId = "E. South America Standard Time";
                    TimeZoneInfo brazilTimeZone = TimeZoneInfo.FindSystemTimeZoneById(brazilTimeZoneId);
                    DateTime brazilTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone);
                    AppState.MyMessagePagamento = "Começou a Rodar" + " Data: " + brazilTime.ToString("yyyy-MM-dd HH:mm:ss") + "\n";

                    List<Reserva> listReserva = new List<Reserva>();

                    listReserva = FunctionAPICLOUDBEDs.getReservationsAsync(null).Result;

                    foreach (Reserva reserva1 in listReserva)
                    {
                        if (!reserva1.Equals(null) && reserva1.Origem.Contains("Airbnb") && reserva1.Balance > 0)
                        {
                            LogSistema logSistema = new LogSistema();
                            logSistema.IDReserva = reserva1.IDReserva.ToString();
                            logSistema.Status = reserva1.Status;
                            logSistema.DataLog = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone);

                            logSistema.Log += PagamentoReserva(reserva1);

                            AppState.ListLogSistemaPagamentoAirbnb.Add(logSistema);
                        }
                    }

                    DateTime endbrazilTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone);
                    AppState.MyMessagePagamento += "Terminou!" + " Data: " + endbrazilTime.ToString("yyyy-MM-dd HH:mm:ss") + "\n";

                    Thread.Sleep(60000);
                }
                catch (Exception e)
                {
                    AppState.MyMessage = e.Message + "\n";
                    Thread.Sleep(5000);

                }
            }


        }

        private static string PagamentoReserva(Reserva reservapagemtento)
        {
            try
            {
                string retorno = "";
                Payment payment = FunctionAPICLOUDBEDs.GetPaymentsAsync(reservapagemtento).Result;
                if (payment.Success)
                {
                    Reserva reserva = FunctionAPICLOUDBEDs.getReservationAsync(reservapagemtento).Result;
                    if (reserva.Balance == 0)
                        retorno += "Ja tem Pagamento!" + "\n";
                    else
                        retorno += "Criado: " + FunctionAPICLOUDBEDs.PostReservationPagamento(reserva).Result + " \n";

                    LogSistema logSistemapagamento = new LogSistema();
                    logSistemapagamento.IDReserva = reserva.IDReserva!.ToString();
                    logSistemapagamento.Status = reserva.Status!;
                    logSistemapagamento.DataLog = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone);
                    logSistemapagamento.Log += retorno;
                    AppState.ListLogSistemaPagamentoAirbnb.Add(logSistemapagamento);
                }

                return retorno;
            }
            catch (Exception e)
            {
                return e.Message + "\n";
            }
        }

        private static string RemovePagamentoReserva(Reserva reservapagemtento)
        {
            try
            {
                string retorno = "";
                Payment payment = FunctionAPICLOUDBEDs.GetPaymentsAsync(reservapagemtento).Result;
                if (payment.Success)
                {
                    Reserva reserva = FunctionAPICLOUDBEDs.getReservationAsync(reservapagemtento).Result;
                    if (reserva.Balance > 0)
                        retorno += "Removeu o pagamento" + "\n";
                    else
                        retorno += "Removeu o pagamento: " + FunctionAPICLOUDBEDs.PostVoidPayment(reserva.IDReserva!, payment.Data[0].PaymentId).Result + " \n";

                    LogSistema logSistemapagamento = new LogSistema();
                    logSistemapagamento.IDReserva = reserva.IDReserva!.ToString();
                    logSistemapagamento.Status = reserva.Status!;
                    logSistemapagamento.DataLog = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone);
                    logSistemapagamento.Log += retorno;
                    AppState.ListLogSistemaPagamentoAirbnb.Add(logSistemapagamento);
                }

                return retorno;
            }
            catch (Exception e)
            {
                return e.Message + "\n";
            }
        }

        public static bool ValidarCPF(string cpf)
        {


            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCpf;
            string digito;
            int soma;
            int resto;
            cpf = cpf.Trim();
            cpf = cpf.Replace(".", "").Replace("-", "");

            if (cpf.Length != 11)
                return false;

            if (cpf.Equals("12345678911"))
                return false;

            tempCpf = cpf.Substring(0, 9);
            soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto.ToString();
            return cpf.EndsWith(digito);


        }

        private static string AjustarEndereco(Reserva reserva)
        {
            try
            {
                string retorno = "";
                string ufdescrition = reserva.UF!.GetEnumDescription();
                retorno = FunctionAPICLOUDBEDs.putGuest(reserva.GuestID!, "guestState", ufdescrition).Result;

                return retorno;
            }
            catch (Exception e)
            {
                return e.Message + "\n";
            }
        }

        private static string AjustarDataNascimento(Reserva reserva)
        {
            try
            {
                string retorno = "";
                string ufdescrition = reserva.UF!.GetEnumDescription();

                if (!string.IsNullOrEmpty(reserva.DataNascimento.ToString("yyyy-MM-dd")))
                {
                    retorno = FunctionAPICLOUDBEDs.putGuest(reserva.GuestID!, "guestBirthDate", reserva.DataNascimento.ToString("yyyy-MM-dd")).Result;

                    CustomField? customFieldData = new CustomField();
                    customFieldData.CustomFieldName = "Data_de_Nascimento";
                    customFieldData.CustomFieldValue = reserva.DataNascimento.ToString("dd/MM/yyyy");
                    var jsoncustomFieldData = JsonConvert.SerializeObject(customFieldData);
                    FunctionAPICLOUDBEDs.putGuest(reserva.GuestID!, "guestCustomFields", "[" + jsoncustomFieldData.ToString() + "]");
                }

                retorno = FunctionAPICLOUDBEDs.putGuest(reserva.GuestID!, "guestGender", "M").Result;

                return retorno;
            }
            catch (Exception e)
            {
                return e.Message + "\n";
            }
        }

        private static string AjustarCPF(Reserva reserva)
        {
            try
            {
                string retorno = "";
                string ufdescrition = reserva.UF!.GetEnumDescription();

                CustomField? customField = new CustomField();
                customField.CustomFieldName = "CPF";
                customField.CustomFieldValue = reserva.ProxyCPF;
                var jsoncustomField = JsonConvert.SerializeObject(customField);
                FunctionAPICLOUDBEDs.putGuest(reserva.GuestID!, "guestCustomFields", "[" + jsoncustomField.ToString() + "]");

                retorno = FunctionAPICLOUDBEDs.putGuest(reserva.GuestID!, "guestDocumentType", "dni").Result;

                retorno = FunctionAPICLOUDBEDs.PutGuestDNI(reserva.GuestID!, reserva.ProxyCPF).Result;

                retorno = FunctionAPICLOUDBEDs.putGuest(reserva.GuestID!, "guestTaxId", reserva.ProxyCPF).Result;

                return retorno;
            }
            catch (Exception e)
            {
                return e.Message + "\n";
            }
        }

    }
}