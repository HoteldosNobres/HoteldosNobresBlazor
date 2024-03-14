using HoteldosNobresBlazor.Classes;

namespace HoteldosNobresBlazor.Services
{
    public class AppState
    {
        public string MyMessage { get; set; } = "Hello from AppState";

        public List<Reserva> ListReservas { get; set; } = new List<Reserva>();


        public string MyMessageFNRH { get; set; } = "Execução do FNRH";
        public string MyMessageReservation { get; set; } = " ";
        public string MyMessagePagamento { get; set; } = "Pagamento do FNRH";
         

        public List<LogSistema> ListLogSistemaFNRH { get; set; } = new List<LogSistema>();
        public List<LogSistema> ListLogSistemaAddReserva { get; set; } = new List<LogSistema>();
        public List<LogSistema> ListLogSistemaPagamentoAirbnb { get; set; } = new List<LogSistema>();


    }
    public class LogSistema
    {
        public string IDReserva { get; set; } = "0";

        public string LinkReserva
        {
            get
            {
                if (string.IsNullOrEmpty(IDReserva))
                    return "";
                return "https://hotels.cloudbeds.com/connect/235132#/reservations/r" + IDReserva;
            }
        }
        public DateTime DataLog { get; set; } = DateTime.Now;

        public string Status { get; set; } = "";
        public string Log { get; set; } = "";

    }
     

}
