using Newtonsoft.Json; 

namespace HoteldosNobresBlazor.Client.API;

public class APICloudbeds
{

    private readonly HttpClient _httpClient;

    public APICloudbeds(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("CloudbedsAPI");
    }

    public async Task<string> GetReservaAsync(string ID)
    {
       return await _httpClient.GetStringAsync(_httpClient.BaseAddress + "/getReservation?reservationID=" + ID);
        //Reserva reserva = new Reserva();
        //try
        //{
        //    var retorno = await _httpClient.GetStringAsync(_httpClient.BaseAddress + "/getReservation?reservationID=" + ID);
        //    Reservation resevation;
        //    if (retorno != null)
        //    {
        //        resevation = await LerRespostaComoObjetoAsync<Reservation>(retorno);
        //        reserva.Converte(resevation);
        //    }

        //    var retornonotes = await _httpClient.GetStringAsync(_httpClient.BaseAddress + "/getReservationNotes?reservationID=" + ID);
        //    Notes notes = await LerRespostaComoObjetoAsync<Notes>(retornonotes);

        //    if (notes.Data.Length > 0)
        //    {
        //        if (reserva.Notas == null)
        //            reserva.Notas = new List<Nota>();

        //        foreach (var note in notes.Data)
        //        {
        //            reserva.Notas.Add(new Nota(note.ReservationNoteId.ToString(), note.ReservationNote));
        //            if (note.ReservationNote.Contains("SNRHos"))
        //            {
        //                reserva.SnNum = note.ReservationNote.Replace("SNRHos-MS0001(", "").Replace("SNRHos-MS0003(", "").Replace(")", "");
        //            }

        //        }
        //    }

        //    return reserva;

        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine(ex.Message);
        //    return null;
        //}
    }

    public static async Task<T> LerRespostaComoObjetoAsync<T>(string jsonString)
    {
        T obj = JsonConvert.DeserializeObject<T>(jsonString);
        return obj;
    }

}
