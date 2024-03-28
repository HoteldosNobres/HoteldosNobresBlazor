using HoteldosNobresBlazor.Classes;
using HoteldosNobresBlazor.Services;
using System.Globalization;
using System.Net.NetworkInformation;

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
                TimeZoneInfo brazilTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
                MensagemWhatsApp mensagem = FunctionAPICLOUDBEDs.LerRespostaComoObjetoAsync<MensagemWhatsApp>(json).Result;

                string from = mensagem.Entry[0].Changes[0].Value.Messages[0].From;
                string texto = "";
                string cpf = "";
                string datadenascimento = "";
                string hotelrating = "";
                string resultado = "";

                if (mensagem.Entry[0].Changes[0].Value.Messages[0].Text != null)
                    texto = mensagem.Entry[0].Changes[0].Value.Messages[0].Text.Body + " From: " + from;
                else
                {
                    string jasonresposta = mensagem.Entry[0].Changes[0].Value.Messages[0].Interactive.NfmReply.ResponseJson;
                    Response_Json respostajson = FunctionAPICLOUDBEDs.LerRespostaComoObjetoAsync<Response_Json>(jasonresposta).Result;
                    cpf = respostajson.Datadenascimento;
                    datadenascimento = respostajson.Datadenascimento;

                    hotelrating = respostajson.Hotelrating;
                    string comment_text = respostajson.Comment_text;

                    if (!string.IsNullOrEmpty(cpf) && !string.IsNullOrEmpty(datadenascimento))
                        texto = "CPF: " + cpf + " Data Nascimento: " + datadenascimento + " From: " + from;
                    else
                        texto = " Nota: " + hotelrating + " de 10 Comentario: " + comment_text + " From: " + from;

                }

                if(!string.IsNullOrEmpty(cpf))
                {
                    resultado += FunctionWhatsApp.postMensagem("5535984151764", texto).Result; 
                    resultado += FunctionWhatsApp.postMensagemTemplete(from, "inf_inicial").Result;
                    Reserva reserva = AppState.ListReservas.Where(x => x.ProxyCelular == from).FirstOrDefault();
                    if (reserva != null)
                    {
                       reserva = FunctionAPICLOUDBEDs.postReservationNote(reserva, texto).Result;
                    }
                } 
                else if(!string.IsNullOrEmpty(hotelrating))
                {
                    resultado += FunctionWhatsApp.postMensagem("5535984151764", texto).Result; 

                    Reserva reserva = AppState.ListReservas.Where(x => x.ProxyCelular == from).FirstOrDefault();
                    if (reserva != null)
                    {
                        reserva = FunctionAPICLOUDBEDs.postReservationNote(reserva, texto).Result;
                    }
                }
                else
                    resultado += FunctionWhatsApp.postMensagem(from).Result;

                if(from == "553584151764" && texto == "postMensageFlowCPF")                
                    resultado += FunctionWhatsApp.postMensageFlowCPF(from).Result;
                else if (from == "553584151764" && texto == "postMensageFlowAvaliacao")
                    resultado += FunctionWhatsApp.postMensageFlowAvaliacao(from).Result;
                else if (from == "553584151764" && texto == "postMensagemTemplete")
                    resultado += FunctionWhatsApp.postMensagemTemplete(from, "inf_mtur").Result;

                resultado += FunctionWhatsApp.postMensagem("553537150180", texto).Result;

                LogSistema log = new LogSistema() { 
                    DataLog = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone),
                    Log = "Numero " + from + " Texto:" + texto + " "
                };

                AppState.ListLogWhatsapp.Add(log);
                 
                return "OK ";
            }
            catch (Exception e)
            {
                AppState.MyMessageLogWhatsapp = e.Message + "\n";
                return e.Message;
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


                Thread thread = new Thread(new ParameterizedThreadStart(CreateReservation_changedMetodo));
                thread.Start(reserva);

                return "OK " + create.reservationId;

            }
            catch (Exception e)
            {
                AppState.MyMessageReservation = e.Message + "\n";
                return e.Message;
            }
        }

        private void CreateReservation_changedMetodo(object objeto)
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

                if (!novareserva.Equals(null) && novareserva.Origem.Contains("Airbnb"))
                {
                    logSistema.Log += PagamentoReserva(novareserva);
                }

                novareserva = FunctionAPICLOUDBEDs.getReservationAsync(novareserva).Result;

                if (string.IsNullOrEmpty(novareserva.SnNum) && novareserva.Status.ToUpper() != "CHECKED_OUT" && novareserva.Status.ToUpper() != "CANCELED")
                    logSistema.Log += FuncoesFNRH.Inserir(novareserva);
                else if (!string.IsNullOrEmpty(novareserva.SnNum))
                    logSistema.Log += FuncoesFNRH.Atualizar(novareserva);

                string retorno = "";
                if (logSistema.Log.Contains("SNRHos-MS0001") || logSistema.Log.Contains("SNRHos-ME0026"))
                {
                    logSistema.Log += logSistema.Log.ToString().Contains("SNRHos-ME0026") ? " CPF inválido " : retorno;
                    if (novareserva.Notas == null)
                        novareserva.Notas = new List<Nota>();
                    if (novareserva.Notas.Where(x => x.Texto == retorno).Count() == 0)
                    {
                        novareserva.Notas.Add(new Nota("", retorno));
                        if (retorno.Contains("SNRHos-MS0001"))
                        {
                            novareserva.SnNum = retorno.Replace("SNRHos-MS0001(", "").Replace(")", "");
                            if (novareserva.Notas.Where(x => x.Texto == "CPF inválido").Count() > 0)
                            {
                                retorno += FunctionAPICLOUDBEDs.deleteReservationNote(novareserva, "CPF inválido").Result;
                            }
                        }
                        novareserva = FunctionAPICLOUDBEDs.postReservationNote(novareserva, retorno).Result;
                    }

                }
                logSistema.Log += retorno + " ";


                novareserva = FunctionAPICLOUDBEDs.getReservationAsync(novareserva).Result;
                if (novareserva.ListaQuartos.Count() > 0)
                {
                    foreach (Quarto quarto in novareserva.ListaQuartos)
                    {
                        Rate rate = FunctionAPICLOUDBEDs.getRatesAsync(quarto.ID.ToString(), novareserva.DataCheckIn, novareserva.DataCheckOut).Result;
                        if (rate.Success)
                        { 
                            foreach( RoomRateDetailed roomRateDetailed in rate.Data.RoomRateDetailed)
                            {
                                decimal valornovo = (roomRateDetailed.Rate * 10 / 100) + roomRateDetailed.Rate;
                                string stringvalor = valornovo.ToString("N", new CultureInfo("en-US")); 
                                logSistema.Log += "UpdateRate: " + FunctionAPICLOUDBEDs.postRateAsync(rate.Data.RateId, novareserva.DataCheckIn, novareserva.DataCheckOut.AddDays(-1), stringvalor).Result + " \n";

                            }

                        }
                    } 
                } 

                if (logSistema.Log.Contains("SNRHos-MS0001"))
                {
                    string mensagem = "Olá, " + novareserva.NomeHospede + "! Seja Bem vindo em nosso Hotel.";
                    logSistema.Log += FunctionWhatsApp.postMensagem(novareserva.ProxyCelular, mensagem).Result;
                    logSistema.Log += FunctionWhatsApp.postMensagemTemplete(novareserva.ProxyCelular, "inf_inicial").Result;
                    logSistema.Log += FunctionWhatsApp.postMensagem(novareserva.ProxyCelular).Result;
                }
                else
                  if (logSistema.Log.Contains("CPF inválido") )
                    { 
                        logSistema.Log += FunctionWhatsApp.postMensagemTemplete(novareserva.ProxyCelular, "inf_mtur").Result;
                        logSistema.Log += FunctionWhatsApp.postMensageFlowCPF(novareserva.ProxyCelular).Result;
                    }


                if (!string.IsNullOrEmpty(novareserva.Celular))
                {
                    logSistema.Log += FunctionGoogle.AddPeople(novareserva.NomeHospede, novareserva.Origem, novareserva.CelularDDD + novareserva.Celular, novareserva.Email.ToString());
                }

                AppState.ListLogSistemaAddReserva.Add(logSistema);  
            }
            catch (Exception e)
            {
                AppState.MyMessageReservation = e.Message + "\n";
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

                if (!string.IsNullOrEmpty(reserva.SnNum))
                {
                    logSistema.Log += " SnNum: " + reserva.SnNum + " ";
                    string reservationNoteID = reserva.Notas.Where(x => x.Texto.Contains("SNRHos-MS0001")).FirstOrDefault().Id.ToString();

                    if ((reserva.Status.ToUpper() == "HOSPEDADO" || reserva.Status.ToUpper() == "CHECKED_IN" || reserva.Status.ToUpper() == "CHECKED_OUT"
                    || reserva.Status.ToUpper() == "CHECK OUT FEITO")
                    && !string.IsNullOrEmpty(reservationNoteID))
                    {
                        // PEGAR HORARIO DO CHCKin reserva = FunctionAPICLOUDBEDs.getReservationAsync(reserva).Result;
                        reserva.DataCheckInRealizado = DateTime.Parse(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone).ToString("yyyy-MM-dd HH:mm:ss"));
                        retorno = FuncoesFNRH.CheckIn(reserva);

                        if (!string.IsNullOrEmpty(reservationNoteID))
                            retorno += FunctionAPICLOUDBEDs.putReservationNote(reserva.IDReserva, reservationNoteID, "SNRHos-MS0003(" + reserva.SnNum + ")").Result;
                    }


                    if ((reserva.Status.ToUpper() == "CHECK OUT FEITO" || reserva.Status.ToUpper() == "CHECKED_OUT")
                         && !string.IsNullOrEmpty(reserva.SnNum))
                    {
                        reserva.DataCheckOutRealizado = DateTime.Parse(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone).ToString("yyyy-MM-dd HH:mm:ss"));
                        retorno = FuncoesFNRH.CheckOut(reserva);
                    }

                    if (retorno.Contains("SNRHos-MS0004"))
                    {
                        retorno = retorno + FunctionAPICLOUDBEDs.deleteReservationNote(reserva, "SNRHos-MS0001").Result;
                         
                        string mensagem = "Obrigado por reservar conosco!!! Voltem sempre ";
                        logSistema.Log += FunctionWhatsApp.postMensagem(reserva.ProxyCelular, mensagem).Result;
                        logSistema.Log += FunctionWhatsApp.postMensageFlowAvaliacao(reserva.ProxyCelular).Result;
                      
                    }
                }


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

                if (string.IsNullOrEmpty(changed.reservationId))
                    throw new Exception("ID da reserva não informado");
                 
                Thread thread = new Thread(new ParameterizedThreadStart(ChangedStatusMetodo));
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

                List<Reserva> listReserva = FunctionAPICLOUDBEDs.getReservationsAsync(null).Result;
                Reserva reserva = listReserva.Where(x => x.GuestID == changed.guestID).FirstOrDefault();
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

                if (string.IsNullOrEmpty(reserva.SnNum) && reserva.Status.ToUpper() != "CHECKED_OUT" && reserva.Status.ToUpper() != "CANCELED")
                    retorno = FuncoesFNRH.Inserir(reserva);
                else if (!string.IsNullOrEmpty(reserva.SnNum))
                    retorno = FuncoesFNRH.Atualizar(reserva);

                if (retorno.Contains("SNRHos-MS0001") || retorno.Contains("SNRHos-ME0026"))
                {
                    retorno = retorno.Contains("SNRHos-ME0026") ? "CPF inválido" : retorno;
                    if (reserva.Notas == null)
                        reserva.Notas = new List<Nota>();
                    if (reserva.Notas.Where(x => x.Texto == retorno).Count() == 0)
                    {
                        reserva.Notas.Add(new Nota("", retorno));
                        if (retorno.Contains("SNRHos-MS0001"))
                        {
                            reserva.SnNum = retorno.Replace("SNRHos-MS0001(", "").Replace(")", "");
                            if (reserva.Notas.Where(x => x.Texto == "CPF inválido").Count() > 0)
                            {
                                retorno += FunctionAPICLOUDBEDs.deleteReservationNote(reserva, "CPF inválido").Result;
                            }
                        }
                        reserva = FunctionAPICLOUDBEDs.postReservationNote(reserva, retorno).Result;
                    }


                }

                if (logSistema.Log.Contains("SNRHos-MS0001"))
                {
                    logSistema.Log += FunctionGoogle.AddPeople(reserva.NomeHospede, reserva.Origem, reserva.CelularDDD + reserva.Celular, reserva.Email.ToString());

                    string mensagem = "Olá, " + reserva.NomeHospede + "! Seja Bem vindo em nosso Hotel.";
                    logSistema.Log += FunctionWhatsApp.postMensagem(reserva.ProxyCelular, mensagem).Result;
                    logSistema.Log += FunctionWhatsApp.postMensagemTemplete(reserva.ProxyCelular, "inf_inicial").Result;
                    logSistema.Log += FunctionWhatsApp.postMensagem(reserva.ProxyCelular).Result;
                }

                logSistema.Log += retorno + " ";
                AppState.ListLogSistemaAddReserva.Add(logSistema); 
            }
            catch (Exception e)
            {
                AppState.MyMessageReservation = e.Message + "\n"; 
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

            //string teste = FunctionWhatsApp.postMensagem("35984151764").Result; 
        }

        #endregion CloudBeds
        static void ListaReservaMetodo()
        { 
            try
            {
                List<Reserva> listReserva  = FunctionAPICLOUDBEDs.getReservationsAsync(null).Result;
                listReserva = FunctionAPICLOUDBEDs.getReservationsAsync(DateTime.Now.ToString("yyyy-MM-dd")).Result;
                listReserva.AddRange(FunctionAPICLOUDBEDs.getReservationsAsync(null, DateTime.Now.ToString("yyyy-MM-dd")).Result);
                AppState.ListReservas = listReserva;
                 

                AppState.MyMessage = cache + " Data: " + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone).ToString("yyyy-MM-dd HH:mm:ss") +"\n";
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
                    AppState.ListReservas = FunctionAPICLOUDBEDs.getReservationsAsync(null).Result;

                    // Dorme por 5000 milissegundos, ou seja, 5 segundos

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
                    Reserva reserva2 = new Reserva();
                    reserva2.IDReserva = "5356003227500";
                    reserva2.IDReserva = "0936839622347";
                    //listReserva.Add(reserva2);

                    listReserva = FunctionAPICLOUDBEDs.getReservationsAsync(DateTime.Now.ToString("yyyy-MM-dd")).Result;
                    listReserva.AddRange(FunctionAPICLOUDBEDs.getReservationsAsync(null, DateTime.Now.ToString("yyyy-MM-dd")).Result); 
                     
                    foreach (Reserva reserva1 in listReserva)
                    {
                        LogSistema logSistema = new LogSistema();
                        logSistema.IDReserva = reserva1.IDReserva.ToString();
                        logSistema.Status = reserva1.Status;
                        logSistema.DataLog = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone);

                        Reserva reserva = FunctionAPICLOUDBEDs.getReservationAsync(reserva1).Result;

                        string retorno = "";
                        if (string.IsNullOrEmpty(reserva.SnNum) && reserva.Status.ToUpper() != "CHECKED_OUT" && reserva.Status.ToUpper() != "CANCELED")
                            retorno = FuncoesFNRH.Inserir(reserva);
                        else if (!string.IsNullOrEmpty(reserva.SnNum))
                            retorno = FuncoesFNRH.Atualizar(reserva);

                        if (retorno.Contains("SNRHos-MS0001") || retorno.Contains("SNRHos-ME0026"))
                        {
                            retorno = retorno.Contains("SNRHos-ME0026") ? "CPF inválido" : retorno;
                            if (reserva.Notas == null)
                                reserva.Notas = new List<Nota>();
                            if (reserva.Notas.Where(x => x.Texto == retorno).Count() == 0)
                            {
                                reserva.Notas.Add(new Nota("", retorno));
                                if (retorno.Contains("SNRHos-MS0001"))
                                {
                                    reserva.SnNum = retorno.Replace("SNRHos-MS0001(", "").Replace(")", "");
                                    if (reserva.Notas.Where(x => x.Texto == "CPF inválido").Count() > 0)
                                    {
                                        retorno += FunctionAPICLOUDBEDs.deleteReservationNote(reserva, "CPF inválido").Result;
                                    }
                                }
                                reserva = FunctionAPICLOUDBEDs.postReservationNote(reserva, retorno).Result;
                            }


                        }

                        if (!string.IsNullOrEmpty(reserva.SnNum))
                        {
                            retorno += " SnNum: " + reserva.SnNum + " ";
                            string reservationNoteID = reserva.Notas.Where(x => x.Texto.Contains("SNRHos-MS0001")).FirstOrDefault().Id.ToString();

                            if ((reserva.Status.ToUpper() == "HOSPEDADO" || reserva.Status.ToUpper() == "CHECKED_IN" || reserva.Status.ToUpper() == "CHECKED_OUT"
                            || reserva.Status.ToUpper() == "CHECK OUT FEITO")
                            && !string.IsNullOrEmpty(reservationNoteID))
                            {
                                // PEGAR HORARIO DO CHCKin reserva = FunctionAPICLOUDBEDs.getReservationAsync(reserva).Result;
                                reserva.DataCheckInRealizado = DateTime.Parse(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone).ToString("yyyy-MM-dd HH:mm:ss"));
                                retorno = FuncoesFNRH.CheckIn(reserva);

                                if (!string.IsNullOrEmpty(reservationNoteID))
                                    retorno += FunctionAPICLOUDBEDs.putReservationNote(reserva.IDReserva, reservationNoteID, "SNRHos-MS0003(" + reserva.SnNum + ")").Result;
                            }


                            if ((reserva.Status.ToUpper() == "CHECK OUT FEITO" || reserva.Status.ToUpper() == "CHECKED_OUT")
                                 && !string.IsNullOrEmpty(reserva.SnNum))
                            {
                                reserva.DataCheckOutRealizado = DateTime.Parse(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone).ToString("yyyy-MM-dd HH:mm:ss"));
                                retorno = FuncoesFNRH.CheckOut(reserva);
                            }

                            if (retorno.Contains("SNRHos-MS0004"))
                            {
                                retorno = retorno + FunctionAPICLOUDBEDs.deleteReservationNote(reserva, "SNRHos-MS0001").Result;
                            }
                        }

                        retorno = retorno.Replace("SNRHos-ME0024", " Checkin realizado\não permitido");
                        retorno = retorno.Replace("SNRHos-ME0025", " Checkout realizado\não permitido");

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
                Payment payment = FunctionAPICLOUDBEDs.getPaymentsAsync(reservapagemtento).Result;
                if (payment.Success)
                {
                    Reserva reservaairbnb1 = FunctionAPICLOUDBEDs.getReservationAsync(reservapagemtento).Result;
                    if (reservaairbnb1.Balance == 0)
                        retorno += "Ja tem Pagamento!" + "\n";
                    else
                        retorno += "Criado: " + FunctionAPICLOUDBEDs.postReservationNote(reservaairbnb1).Result + " \n";

                }
                return retorno;
            }
            catch (Exception e)
            {
                return e.Message + "\n";
            }
        }
    }
}