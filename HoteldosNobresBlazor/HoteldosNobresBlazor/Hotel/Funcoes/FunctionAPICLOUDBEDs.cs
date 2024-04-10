using HoteldosNobresBlazor.Classes;
using HoteldosNobresBlazor.Components.Pages;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using static HoteldosNobresBlazor.Components.Pages.CallApi;

namespace HoteldosNobresBlazor.Funcoes
{
    public class FunctionAPICLOUDBEDs
    { 
        static string urlapi = @"https://api.cloudbeds.com/api/v1.2";

        #region RatePlan
        public static async Task<Rate> getRatesAsync(string roomTypeID, DateTime DataCheckIn, DateTime DataCheckOut)
        {
            try
            {
                string url = urlapi + "/getRate?roomTypeID="+ roomTypeID + "&detailedRates=true&startDate=" + DataCheckIn.ToString("yyyy-MM-dd") + "&endDate=" +  DataCheckOut.ToString("yyyy-MM-dd");
                HttpResponseMessage response = GetApi(url).Result;

                Rate rate = await LerRespostaComoObjetoAsync<Rate>(response);

                return rate;
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return new Rate();
            }

        }

        public static async Task<String> postRateAsync(string rateID, DateTime DataStart, DateTime DataEnd, string valor)
        {
            try
            {
                string url = urlapi + @"/putRate"; 

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Add("Authorization", "Bearer " + KEYs.TOKEN_CLOUDBEDS);
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(rateID), "rates[0][rateID]");
                content.Add(new StringContent(DataStart.ToString("yyyy-MM-dd")), "rates[0][interval][0][startDate]");
                content.Add(new StringContent(DataEnd.ToString("yyyy-MM-dd")), "rates[0][interval][0][endDate]");
                content.Add(new StringContent(valor), "rates[0][interval][0][rate]");
                request.Content = content;
                var response = await client.SendAsync(request);

                RespostPayment responsepayment = await LerRespostaComoObjetoAsync<RespostPayment>(response);

                return string.Empty;
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return e.Message;
            }

        }

        #endregion RatePlan

        #region Payment
        public static async Task<Payment> getPaymentsAsync(Reserva reserva)
        {
            try
            {
                string url = urlapi + "/getPayments?reservationID=" + reserva.IDReserva + "&guestID=" + reserva.GuestID;
                HttpResponseMessage response = GetApi(url).Result;

                Payment payments = await LerRespostaComoObjetoAsync<Payment>(response);
                 
                return payments;
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

        }

        public static async Task<String> postReservationNote(Reserva reserva)
        {
            try
            {
                string url = urlapi + @"/postPayment";
                string reservationID = reserva.IDReserva;

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Add("Authorization", "Bearer " + KEYs.TOKEN_CLOUDBEDS);
                 
                var collection = new List<KeyValuePair<string, string>>();
                collection.Add(new("reservationID", reservationID));
                collection.Add(new("type", "airbnb"));
                collection.Add(new("amount", reserva.Balance.ToString("N", new CultureInfo("en-US"))));
                collection.Add(new("cardType", "airbnb"));
                collection.Add(new("description", "AirBnB Prepaid Card"));
                var content = new FormUrlEncodedContent(collection);
                request.Content = content;
                var response = await client.SendAsync(request);
                 
                RespostPayment responsepayment = await LerRespostaComoObjetoAsync<RespostPayment>(response);

                if(responsepayment.Message != null)
                    return responsepayment.Message.ToString();
                else 
                    return responsepayment.TransactionId;
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

        }

        #endregion Payment


        #region Reservation
        public static async Task<List<Reserva>> getReservationsAsync(string checkInFrom = null, string checkOutFrom = null)
        {
            try
            { 
                string url = urlapi + "/getReservations";

                if (!string.IsNullOrEmpty(checkInFrom))
                    url += "?checkInFrom=" + checkInFrom;

                if (!string.IsNullOrEmpty(checkOutFrom))
                    url += "?checkOutFrom=" + checkOutFrom;

                HttpResponseMessage response = GetApi(url).Result;

                Reservations resevations = await LerRespostaComoObjetoAsync<Reservations>(response);

                List<Reserva> listareserva = new List<Reserva>();
                if(resevations.Data != null) 
                    foreach (var item in resevations.Data)
                    {
                        Reserva reserva = new Reserva();
                        reserva.Converte(item);
                        listareserva.Add(reserva);
                    }

                return listareserva;
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return new List<Reserva>();
            }

        }

        public static async Task<List<Reserva>> getReservationsAsyncGuestDetails()
        {
            try
            {
                string url = urlapi + "/getReservations?includeGuestsDetails=true";
                 
                HttpResponseMessage response = GetApi(url).Result;

                Reservations resevations = await LerRespostaComoObjetoAsync<Reservations>(response);

                List<Reserva> listareserva = new List<Reserva>();
                if (resevations.Data != null)
                    foreach (var item in resevations.Data)
                    {
                        Reserva reserva = new Reserva();
                        reserva.Converte(item);
                        listareserva.Add(reserva);
                    }

                return listareserva;
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return new List<Reserva>();
            }

        }

        public static async Task<Reserva> getReservationAsync(Reserva reserva)
        {
            try
            {
                string url = urlapi + "/getReservation?reservationID=" + reserva.IDReserva; 
                HttpResponseMessage response = GetApi(url).Result;

                Reservation resevation = await LerRespostaComoObjetoAsync<Reservation>(response);

                reserva.Converte(resevation);

                url = urlapi + "/getReservationNotes?reservationID=" + reserva.IDReserva; 

                response = GetApi(url).Result;

                Notes notes = await LerRespostaComoObjetoAsync<Notes>(response);

                if(notes.Data.Length > 0)
                {
                    if(reserva.Notas == null)
                        reserva.Notas = new List<Nota>();

                    foreach(var note in notes.Data)
                    {
                        reserva.Notas.Add(new Nota(note.ReservationNoteId.ToString(), note.ReservationNote));
                        if (note.ReservationNote.Contains("SNRHos"))
                        {
                            reserva.SnNum = note.ReservationNote.Replace("SNRHos-MS0001(", "").Replace("SNRHos-MS0003(", "").Replace(")", "");
                        }

                    }  
                }
                return reserva;
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

        }

        public static async Task<string> putReservationNote(string reservationID, string reservationNoteID, string note)
        {
            try
            { 
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Put, urlapi + "/putReservationNote");
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("Authorization", "Bearer " + KEYs.TOKEN_CLOUDBEDS); 
                var collection = new List<KeyValuePair<string, string>>();
                collection.Add(new("reservationID", reservationID));
                collection.Add(new("reservationNoteID", reservationNoteID));
                collection.Add(new("reservationNote", note));
                var content = new FormUrlEncodedContent(collection);
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                //Console.WriteLine(await response.Content.ReadAsStringAsync());

                return "";
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return e.Message;
            }

        }
        public static async Task<string> postReservationNote(string reservationID, string note)
        {
            try
            { 
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, urlapi + @"/postReservationNote");
                request.Headers.Add("Authorization", "Bearer " + KEYs.TOKEN_CLOUDBEDS);
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(reservationID), "reservationID");
                content.Add(new StringContent(note), "reservationNote");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                return string.Empty;
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return e.Message;
            }

        }

        public static async Task<Reserva> postReservationNote(Reserva reserva, string note)
        {
            try
            {
                string url = "https://api.cloudbeds.com/api/v1.2/postReservationNote";
                string reservationID = reserva.IDReserva;

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.cloudbeds.com/api/v1.2/postReservationNote");
                request.Headers.Add("Authorization", "Bearer " + KEYs.TOKEN_CLOUDBEDS);
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(reservationID), "reservationID");
                content.Add(new StringContent(note), "reservationNote");
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

        public static async Task<string> deleteReservationNote(Reserva reserva, string deletar)
        {
            try
            {
                string  reservationNoteID = string.Empty;
                string url  = urlapi + "/getReservationNotes?reservationID=" + reserva.IDReserva; //+ "5356003227500";
                HttpResponseMessage response = GetApi(url).Result;
                Notes notes = await LerRespostaComoObjetoAsync<Notes>(response);

                if (notes.Data.Length > 0)
                {
                    foreach(var note in notes.Data)
                    {
                        if (note.ReservationNote.Contains(deletar))
                        {
                            reservationNoteID = note.ReservationNoteId.ToString();
                            break;
                        }
                    }
                    
                }
                 
                response = GetApi(url).Result;
                url = urlapi + "/deleteReservationNote?reservationID=" + reserva.IDReserva + "&reservationNoteID=" + reservationNoteID;    

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Delete, url );
                request.Headers.Add("Authorization", "Bearer " + KEYs.TOKEN_CLOUDBEDS);
                response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                  
                return "";
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return e.Message;
            }

        }

        public static async Task<string> deleteReservationNote(string reservationID, string reservationNoteID)
        {
            try
            {
                var url = urlapi + "/deleteReservationNote?reservationID=" + reservationID + "&reservationNoteID=" + reservationNoteID;
                HttpResponseMessage response = GetApi(url).Result; 
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Delete, url);
                request.Headers.Add("Authorization", "Bearer " + KEYs.TOKEN_CLOUDBEDS);
                response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                return "";
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return e.Message;
            }

        }

        public static async Task<string> putReservation(string reservationID, string status)
        {
            try
            { 
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Put, urlapi + @"/putReservation");
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("Authorization", "Bearer " + KEYs.TOKEN_CLOUDBEDS); 
                var collection = new List<KeyValuePair<string, string>>();
                collection.Add(new("reservationID", reservationID));
                collection.Add(new("status", status));
                var content = new FormUrlEncodedContent(collection);
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                return "";
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return e.Message;
            }

        }

        #endregion Reservation


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