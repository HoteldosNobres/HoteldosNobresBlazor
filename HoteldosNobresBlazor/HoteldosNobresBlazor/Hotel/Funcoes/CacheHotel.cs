using HoteldosNobresBlazor.Classes;
using HoteldosNobresBlazor.Components.Pages;
using HoteldosNobresBlazor.Services;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Channels;

namespace HoteldosNobresBlazor.Funcoes
{
    public class CacheHotel
    {
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

        public async Task<string> CacheCreateReservationAsync(string json)
        {
            try
            {
                TimeZoneInfo brazilTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
                CreateReservation create = FunctionAPICLOUDBEDs.LerRespostaComoObjetoAsync<CreateReservation>(json).Result;

                Reserva novareserva = new Reserva();
                novareserva.IDReserva = create.reservationId;
                novareserva = await FunctionAPICLOUDBEDs.getReservationAsync(novareserva);

                LogSistema logSistema = new LogSistema();
                logSistema.Log = "CreateReservation-";
                logSistema.IDReserva = novareserva.IDReserva.ToString();
                logSistema.Status = novareserva.Status;
                logSistema.DataLog = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone); 
                novareserva = await FunctionAPICLOUDBEDs.getReservationAsync(novareserva);

                if (!novareserva.Equals(null) && novareserva.Origem.Contains("Airbnb"))
                {
                    Payment payment = FunctionAPICLOUDBEDs.getPaymentsAsync(novareserva).Result;
                    if (payment.Success)
                    {
                        if (payment.Data.Count() > 0)
                            logSistema.Log += "Ja tem Pagamento!" + "\n";
                        else
                            logSistema.Log += "Criado: " + FunctionAPICLOUDBEDs.postReservationNote(novareserva).Result + " \n";

                    }

                }

                novareserva = await FunctionAPICLOUDBEDs.getReservationAsync(novareserva);

                if (string.IsNullOrEmpty(novareserva.SnNum) && novareserva.Status.ToUpper() != "CHECKED_OUT" && novareserva.Status.ToUpper() != "CANCELED")
                    logSistema.Log += FuncoesFNRH.Inserir(novareserva);
                else if (!string.IsNullOrEmpty(novareserva.SnNum))
                    logSistema.Log += FuncoesFNRH.Atualizar(novareserva);

                string retorno = "";
                if (logSistema.Log.Contains("SNRHos-MS0001") || logSistema.Log.Contains("SNRHos-ME0026"))
                {
                    logSistema.Log = logSistema.Log.ToString().Contains("SNRHos-ME0026") ? "CPF inválido" : retorno;
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
                if(novareserva.ListaQuartos.Count() > 0)
                {
                    foreach (Quarto quarto in novareserva.ListaQuartos)
                    {
                        Rate rate = FunctionAPICLOUDBEDs.getRatesAsync(quarto.ID.ToString(), novareserva.DataCheckIn, novareserva.DataCheckOut).Result;
                        if (rate.Success)
                        { 
                            logSistema.Log += "UpdateRate: " + FunctionAPICLOUDBEDs.postRateAsync(rate.Data.RateId, novareserva.DataCheckIn, novareserva.DataCheckOut, (rate.Data.RoomRate +10).ToString() ).Result + " \n";

                        }
                    }
                   

                }

                AppState.ListLogSistemaAddReserva.Add(logSistema);
                return "OK " + create.reservationId;
            }
            catch (Exception e)
            {
                AppState.MyMessageReservation = e.Message + "\n";
                return e.Message;
            }

        }

        public string CacheChangedStatus(string json)
        {
            try
            {
                TimeZoneInfo brazilTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
                ChangedReservation changed = FunctionAPICLOUDBEDs.LerRespostaComoObjetoAsync<ChangedReservation>(json).Result;

                Reserva reserva = new Reserva();
                reserva.IDReserva = changed.reservationId;
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
                    }
                }


                logSistema.Log += retorno + "\n";

                AppState.ListLogSistemaAddReserva.Add(logSistema);
                return "OK " + changed.reservationId;
            }
            catch (Exception e)
            {
                AppState.MyMessageReservation = e.Message + "\n";
                return e.Message;
            }

        }

        public string CacheAccommodation_changed(string json)
        {
            try
            {
                TimeZoneInfo brazilTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
                Accommodation_changed changed = FunctionAPICLOUDBEDs.LerRespostaComoObjetoAsync<Accommodation_changed>(json).Result;

                Reserva reserva = new Reserva();
                reserva.IDReserva = changed.reservationId;
                reserva = FunctionAPICLOUDBEDs.getReservationAsync(reserva).Result;

                LogSistema logSistema = new LogSistema();
                logSistema.IDReserva = reserva.IDReserva.ToString();
                logSistema.Status = reserva.Status;
                logSistema.DataLog = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone);
                logSistema.Log = "Accommodation_changed-";

                string retorno = "";

                if (string.IsNullOrEmpty(reserva.SnNum) && reserva.Status.ToUpper() != "CHECKED_OUT" && reserva.Status.ToUpper() != "CANCELED")
                    retorno = FuncoesFNRH.Inserir(reserva);
                else if (!string.IsNullOrEmpty(reserva.SnNum))
                    retorno = FuncoesFNRH.Atualizar(reserva);

                logSistema.Log += retorno + " ";
                AppState.ListLogSistemaAddReserva.Add(logSistema);
                return "OK " + changed.reservationId;
            }
            catch (Exception e)
            {
                AppState.MyMessageReservation = e.Message + "\n";
                return e.Message;
            }

        }

        public string CacheDetails_changed(string json)
        {
            try
            {
                TimeZoneInfo brazilTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
                Details_changed changed = FunctionAPICLOUDBEDs.LerRespostaComoObjetoAsync<Details_changed>(json).Result;

                List<Reserva> listReserva = FunctionAPICLOUDBEDs.getReservationsAsync(null).Result;
                Reserva reserva = listReserva.Where(x => x.GuestID == changed.guestID).FirstOrDefault(); 
                reserva = FunctionAPICLOUDBEDs.getReservationAsync(reserva).Result;

                LogSistema logSistema = new LogSistema();
                logSistema.Log = "Details_changed-";
                logSistema.IDReserva = reserva.IDReserva.ToString();
                logSistema.Status = reserva.Status;
                logSistema.DataLog = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone);

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

                logSistema.Log += retorno + " ";
                AppState.ListLogSistemaAddReserva.Add(logSistema);
                return "OK " + changed.reservationId;
            }
            catch (Exception e)
            {
                AppState.MyMessageReservation = e.Message + "\n";
                return e.Message;
            }

        }

        public void CacheExecutanado()
        {
            Thread thread = new Thread(NovoMetodo);
            thread.Start();

            Thread thread2 = new Thread(FNRHMetodo);
            thread2.Start();

            Thread thread3 = new Thread(PagamentoMetodo);
            thread3.Start();
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

                    AppState.MyMessage = cache + " Data: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ";

                    Thread.Sleep(60000);
                }
                catch (Exception e)
                {
                    AppState.MyMessage = e.Message;
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
                    listReserva = FunctionAPICLOUDBEDs.getReservationsAsync(null, DateTime.Now.ToString("yyyy-MM-dd")).Result;
                    listReserva.AddRange(FunctionAPICLOUDBEDs.getReservationsAsync(null).Result);


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
                    AppState.MyMessage = e.Message;
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

                    listReserva = FunctionAPICLOUDBEDs.getReservationsAsync(DateTime.Now.ToString("yyyy-MM-dd")).Result;
                    listReserva = FunctionAPICLOUDBEDs.getReservationsAsync(null, DateTime.Now.ToString("yyyy-MM-dd")).Result;
                    listReserva.AddRange(FunctionAPICLOUDBEDs.getReservationsAsync(null).Result);

                    foreach (Reserva reserva1 in listReserva)
                    {
                        if (!reserva1.Equals(null) && reserva1.Origem.Contains("Airbnb"))
                        {
                            LogSistema logSistema = new LogSistema();
                            logSistema.IDReserva = reserva1.IDReserva.ToString();
                            logSistema.Status = reserva1.Status;
                            logSistema.DataLog = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone);

                            Payment payment = FunctionAPICLOUDBEDs.getPaymentsAsync(reserva1).Result;
                            if (payment.Success)
                            {
                                if (payment.Data.Count() > 0)
                                    logSistema.Log += "Ja tem Pagamento!" + "\n";
                                else
                                    logSistema.Log += "Criado: " + FunctionAPICLOUDBEDs.postReservationNote(reserva1).Result + " \n";

                            }
                            AppState.ListLogSistemaPagamentoAirbnb.Add(logSistema);
                        }
                    }

                    DateTime endbrazilTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone);
                    AppState.MyMessagePagamento += "Terminou!" + " Data: " + endbrazilTime.ToString("yyyy-MM-dd HH:mm:ss") + "\n";

                    Thread.Sleep(60000);
                }
                catch (Exception e)
                {
                    AppState.MyMessage = e.Message;
                    Thread.Sleep(5000);

                }
            }


        }
    }
}