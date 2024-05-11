using HoteldosNobresBlazor.Classes;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace HoteldosNobresBlazor.Client.API;

public class APICloudbeds
{

    private readonly HttpClient _httpClient;

    public APICloudbeds(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("CloudbedsAPI");
    }

    public async Task<Reserva> GetReservaAsync(string ID)
    {
        Reserva reserva = new Reserva();
        try
        {
            var retorno = await _httpClient.GetStringAsync(_httpClient.BaseAddress + "/getReservation?reservationID=" + ID);
            Reservation resevation;
            if (retorno != null)
            {
                resevation = await LerRespostaComoObjetoAsync<Reservation>(retorno);
                reserva.Converte(resevation);
            }

            var retornonotes = await _httpClient.GetStringAsync(_httpClient.BaseAddress + "/getReservationNotes?reservationID=" + ID);
            Notes notes = await LerRespostaComoObjetoAsync<Notes>(retornonotes);

            if (notes.Data.Length > 0)
            {
                if (reserva.Notas == null)
                    reserva.Notas = new List<Nota>();

                foreach (var note in notes.Data)
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
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    public async Task<List<Reserva>> getReservationsAsyncGuestDetails()
    {
        try
        {
            string response = await _httpClient.GetStringAsync(_httpClient.BaseAddress + "/getReservations?includeGuestsDetails=true");

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

    public static async Task<T> LerRespostaComoObjetoAsync<T>(string jsonString)
    {
        T obj = JsonConvert.DeserializeObject<T>(jsonString);
        return obj;
    }

    #region Guest
    public async Task<Guest> GetGuestAsync(string Id)
    {
        try
        { 
            string retorno = await _httpClient.GetStringAsync(_httpClient.BaseAddress + "/getGuest?guestID=" + Id);
            GuestData response = await LerRespostaComoObjetoAsync<GuestData>(retorno);
             
            if (response.Success)
                return response.Guest!;

            return null;

        }
        catch (FileNotFoundException e)
        {
            return null;
        }
    }

    #endregion

}
