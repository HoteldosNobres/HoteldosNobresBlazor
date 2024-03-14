using HoteldosNobresBlazor.Classes;
using HoteldosNobresBlazor.Components.Pages;
using HoteldosNobresBlazor.Services;
using System.Collections.Generic;
using System.Net.NetworkInformation;

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

        public string CacheNovaReserva(string json)
        {
            try
            {
                CreateReservation create = FunctionAPICLOUDBEDs.LerRespostaComoObjetoAsync<CreateReservation>(json).Result;

                AppState.MyMessageReservation += "IDReserva: " + create.reservationId + "|/n";

                Reserva reserva2 = new Reserva();
                reserva2.IDReserva = create.reservationId;
                reserva2 = FunctionAPICLOUDBEDs.getReservationAsync(reserva2).Result;
                  
                if (!reserva2.Equals(null) && reserva2.Origem.Contains("Airbnb"))
                {
                    AppState.MyMessageReservation += " Status: " + reserva2.Status + "|";
                    Payment payment = FunctionAPICLOUDBEDs.getPaymentsAsync(reserva2).Result;
                    if (payment.Success)
                    {
                        if (payment.Data.Count() > 0)
                            AppState.MyMessageReservation += "Ja tem Pagamento!" + "\n";
                        else
                            AppState.MyMessageReservation += "Criado: " + FunctionAPICLOUDBEDs.postReservationNote(reserva2).Result + " \n";

                    }

                }

                if (string.IsNullOrEmpty(reserva2.SnNum) && reserva2.Status.ToUpper() != "CHECKED_OUT" && reserva2.Status.ToUpper() != "CANCELED")
                    AppState.MyMessageReservation += FuncoesFNRH.Inserir(reserva2);
                else if (!string.IsNullOrEmpty(reserva2.SnNum))
                    AppState.MyMessageReservation += FuncoesFNRH.Atualizar(reserva2);

                AppState.MyMessageReservation +=  "| /n";
                return "OK " + create.reservationId;
            }catch(Exception e)
            {
                return e.Message;
            }
           
        }

        public string CacheChangedStatus(string json)
        {
            try
            {
                ChangedReservation changed = FunctionAPICLOUDBEDs.LerRespostaComoObjetoAsync<ChangedReservation>(json).Result;

                AppState.MyMessageReservation += "Change IDReserva: " + changed.reservationId + " /n";

                Reserva reserva = new Reserva();
                reserva.IDReserva = changed.reservationId;
                reserva = FunctionAPICLOUDBEDs.getReservationAsync(reserva).Result;

                string retorno = "";
                TimeZoneInfo brazilTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
                DateTime brazilTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone);
                AppState.MyMessageFNRH = "Começou a Rodar" + " Data: " + brazilTime.ToString("yyyy-MM-dd HH:mm:ss") + "\n";

                if (!string.IsNullOrEmpty(reserva.SnNum))
                {
                    AppState.MyMessageReservation += " SnNum: " + reserva.SnNum + " ";
                    string reservationNoteID = reserva.Notas.Where(x => x.Texto.Contains("SNRHos-MS0001")).FirstOrDefault().Id.ToString();

                    if ((reserva.Status.ToUpper() == "HOSPEDADO" || reserva.Status.ToUpper() == "CHECKED_IN" || reserva.Status.ToUpper() == "CHECKED_OUT"
                    || reserva.Status.ToUpper() == "CHECK OUT FEITO")
                    && !string.IsNullOrEmpty(reservationNoteID))
                    {
                        // PEGAR HORARIO DO CHCKin reserva = FunctionAPICLOUDBEDs.getReservationAsync(reserva).Result;
                        reserva.DataCheckInRealizado = DateTime.Parse(brazilTime.ToString("yyyy-MM-dd H:mm:ss"));
                        retorno = FuncoesFNRH.CheckIn(reserva);

                        if (!string.IsNullOrEmpty(reservationNoteID))
                            retorno += FunctionAPICLOUDBEDs.putReservationNote(reserva.IDReserva, reservationNoteID, "SNRHos-MS0003(" + reserva.SnNum + ")").Result;
                    }


                    if ((reserva.Status.ToUpper() == "CHECK OUT FEITO" || reserva.Status.ToUpper() == "CHECKED_OUT")
                         && !string.IsNullOrEmpty(reserva.SnNum))
                    {
                        reserva.DataCheckOutRealizado = DateTime.Parse(brazilTime.ToString("yyyy-MM-dd H:mm:ss"));
                        retorno = FuncoesFNRH.CheckOut(reserva);
                    }

                    if (retorno.Contains("SNRHos-MS0004"))
                    {
                        retorno = retorno + FunctionAPICLOUDBEDs.deleteReservationNote(reserva, "SNRHos-MS0001").Result;
                    }
                }


                return "OK " + changed.reservationId;
            }
            catch (Exception e)
            {
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
                    AppState.ListReservas = FunctionAPICLOUDBEDs.getReservationsAsync(DateTime.Now.ToString("yyyy-MM-dd")).Result;
                    AppState.ListReservas.AddRange(FunctionAPICLOUDBEDs.getReservationsAsync(null, DateTime.Now.ToString("yyyy-MM-dd")).Result);


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
            while(true)
            {
                try
                { 
                    TimeZoneInfo brazilTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
                    DateTime brazilTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone);
                    AppState.MyMessageFNRH = "Começou a Rodar" + " Data: " + brazilTime.ToString("yyyy-MM-dd HH:mm:ss") + "\n";
                     
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
                        AppState.MyMessageFNRH += "IDReserva:" + reserva1.IDReserva + " Status: " + reserva1.Status + " ";

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
                            AppState.MyMessageFNRH += " SnNum: " + reserva.SnNum + " ";
                            string reservationNoteID = reserva.Notas.Where(x => x.Texto.Contains("SNRHos-MS0001")).FirstOrDefault().Id.ToString();

                            if ((reserva.Status.ToUpper() == "HOSPEDADO" || reserva.Status.ToUpper() == "CHECKED_IN" || reserva.Status.ToUpper() == "CHECKED_OUT"
                            || reserva.Status.ToUpper() == "CHECK OUT FEITO")
                            && !string.IsNullOrEmpty(reservationNoteID))
                            {
                                // PEGAR HORARIO DO CHCKin reserva = FunctionAPICLOUDBEDs.getReservationAsync(reserva).Result;
                                reserva.DataCheckInRealizado = DateTime.Parse(brazilTime.ToString("yyyy-MM-dd H:mm:ss"));
                                retorno = FuncoesFNRH.CheckIn(reserva);

                                if (!string.IsNullOrEmpty(reservationNoteID))
                                    retorno += FunctionAPICLOUDBEDs.putReservationNote(reserva.IDReserva, reservationNoteID, "SNRHos-MS0003(" + reserva.SnNum + ")").Result;
                            }


                            if ((reserva.Status.ToUpper() == "CHECK OUT FEITO" || reserva.Status.ToUpper() == "CHECKED_OUT")
                                 && !string.IsNullOrEmpty(reserva.SnNum))
                            {
                                reserva.DataCheckOutRealizado = DateTime.Parse(brazilTime.ToString("yyyy-MM-dd H:mm:ss"));
                                retorno = FuncoesFNRH.CheckOut(reserva);
                            }

                            if (retorno.Contains("SNRHos-MS0004"))
                            {
                                retorno = retorno + FunctionAPICLOUDBEDs.deleteReservationNote(reserva, "SNRHos-MS0001").Result;
                            }
                        }

                        retorno = retorno.Replace("SNRHos-ME0024", " Checkin realizado\não permitido");
                        retorno = retorno.Replace("SNRHos-ME0025", " Checkout realizado\não permitido");

                        AppState.MyMessageFNRH += retorno + "\n";
                    }


                    AppState.MyMessageFNRH += "Terminou!" + "\n";

                    Thread.Sleep(60000);
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

                    //Reserva reserva2 = new Reserva();
                    ////reserva2.IDReserva = "6034189965225";
                    ////reserva2 = FunctionAPICLOUDBEDs.getReservationAsync(reserva2).Result;
                    ////listReserva.Add(reserva2);

                    listReserva = FunctionAPICLOUDBEDs.getReservationsAsync(DateTime.Now.ToString("yyyy-MM-dd")).Result;
                    listReserva = FunctionAPICLOUDBEDs.getReservationsAsync(null, DateTime.Now.ToString("yyyy-MM-dd")).Result;
                    listReserva.AddRange(FunctionAPICLOUDBEDs.getReservationsAsync(null).Result);

                    foreach (Reserva reserva1 in listReserva)
                    {
                        if(!reserva1.Equals(null) && reserva1.Origem.Contains("Airbnb"))
                        {
                            AppState.MyMessagePagamento += "IDReserva: " + reserva1.IDReserva + " Status: " + reserva1.Status + " ";
                            Payment payment = FunctionAPICLOUDBEDs.getPaymentsAsync(reserva1).Result;
                            if(payment.Success)
                            {
                                if(payment.Data.Count() > 0)
                                    AppState.MyMessagePagamento += "Ja tem Pagamento!" + "\n";
                                else                                 
                                    AppState.MyMessagePagamento += "Criado: " +  FunctionAPICLOUDBEDs.postReservationNote(reserva1).Result  + " \n";
  
                            }

                        }     
                        string retorno = "";
                         
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