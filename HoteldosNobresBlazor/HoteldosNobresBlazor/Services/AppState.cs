using HoteldosNobresBlazor.Classes;

namespace HoteldosNobresBlazor.Services
{
    public class AppState
    {
        public string MyMessage { get;  set; } = "Hello from AppState";

        public List<Reserva> ListReservas { get; set; } = new List<Reserva>();


        public string MyMessageFNRH { get; set; } = "Execução do FNRH";
        public string MyMessageReservation { get; set; } = " ";
        public string MyMessagePagamento { get; set; } = "Pagamento do FNRH";


    }
}
