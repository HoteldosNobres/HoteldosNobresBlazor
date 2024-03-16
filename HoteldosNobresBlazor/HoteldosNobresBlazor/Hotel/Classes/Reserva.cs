using MosaicoSolutions.ViaCep;
using MosaicoSolutions.ViaCep.Modelos;
using System.Globalization;

namespace HoteldosNobresBlazor.Classes
{
    public class Reserva
    {
        //nformações da reserva
        public string? CustomerID { get; set; }
        public DateTime DataCheckIn { get; set; }
        public DateTime DataCheckInRealizado { get; set; }
        public DateTime DataCheckOut { get; set; }
        public DateTime DataCheckOutRealizado { get; set; }
        public int? Noites { get; set; }
        public int? Hospedes { get; set; }
        public string? Origem { get; set; }
        public string? IDReserva { get; set; }
        public string? IDReservaAgencia { get; set; }
        public string? FonteReserva { get; set; }
        public string? Valor { get; set; }
        public string? Balance { get; set; }
        public string Status { get; set; }
        public string? SnNum { get; set; }
        public string? Snuhnum { get; set; }

        public List<Nota>? Notas { get; set; }

        public List<Quarto>? ListaQuartos { get; set; }

        /// Hospede
        public string? GuestID { get; set; }

        public string? NomeHospede { get; set; }
        public string? Cpf { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Genero { get; set; }

        //Contato
        public string? Email { get; set; }
        public string? Numerotelefone { get; set; }
        public string? Address { get; set; }
        public string? Postalcode { get; set; }
        public string? Contry { get; set; }
        public string? CEP { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public string? CodigoIBGE { get; set; }

        //Cartao
        public string? NumeroCartao { get; set; }
        public string? CVC { get; set; }
        public string? DataValidade { get; set; }

        public string? CelularDDI
        {
            get
            {
                if (string.IsNullOrEmpty(Numerotelefone))
                    return "55";
                return Numerotelefone.Count() >= 13 ? Numerotelefone.Replace("+", "").Substring(0, 2) : "55";
            }
        }

        public string? CelularDDD
        {
            get
            {
                if (string.IsNullOrEmpty(Numerotelefone))
                    return "35";
                return Numerotelefone.Replace("+", "").Count() > 11 ? Numerotelefone.Replace("+", "").Substring(2, 2) : Numerotelefone.Replace("+", "").Substring(0, 2);
            }
        }

        public string? Celular
        {
            get
            {
                if (string.IsNullOrEmpty(Numerotelefone))
                    return "37150180";
                return Numerotelefone.Replace("+", "").Count() > 11 ? Numerotelefone.Replace("+", "").Substring(4, Numerotelefone.Length - 5) : Numerotelefone.Replace("+", "").Substring(2, 9);
            }
        }

        public string? Obs
        {
            get
            {
                string OBS = "Informacoes: Numero " + IDReserva + " " + Origem + " " + IDReservaAgencia;
                return OBS;
            }
        }

        public string LinkReserva
        {
            get
            {
                if(string.IsNullOrEmpty(IDReserva))
                    return "";
                return "https://hotels.cloudbeds.com/connect/235132#/reservations/r" + IDReserva;
            }
        }
        

        public string ProxyPais
        {
            get
            {
                return !string.IsNullOrEmpty(Contry) ? Contry.ToUpper() : "BRASIL";
            }
        }

        public string ProxyCidade
        {
            get
            {
                return !string.IsNullOrEmpty(Cidade) ? Cidade.ToUpper() : "POÇOS DE CALDAS";
            }
        }

        public string ProxyEstado
        {
            get
            {
                return !string.IsNullOrEmpty(Estado) ? Estado.ToUpper() : "MINAS GERAIS";
            }
        }

        public string ProxyCodigoIBGE
        {
            get
            {
                return !string.IsNullOrEmpty(CodigoIBGE) ? CodigoIBGE : "3151800";
            }
        }

        public string ProxyCPF
        {
            get
            {
                if (string.IsNullOrEmpty(Cpf))
                    return "11122233300";
                return Convert.ToUInt64(Cpf.Replace(" ", "").Replace(".", "").Replace("-", "")).ToString(@"000\.000\.000\-00");
            }
        }

        public string ProxySnexcluirficha
        {
            get
            {
                return Status.ToUpper().Equals("CANCELADO") ? "1" : "0";
            }
        }

        public void Converte(Reservation reservation)
        {
            CustomerID = reservation.Data.ReservationId;
            DataCheckIn = DateTime.Parse(reservation.Data.StartDate.ToString());
            // DataCheckInRealizado = reserva.DataCheckInRealizado;
            DataCheckOut = DateTime.Parse(reservation.Data.EndDate.ToString());
            //  DataCheckOutRealizado = reserva.DataCheckOutRealizado;
            //Noites = reserva.Noites;
            if (reservation.Data.Assigned.Count() > 0 && reservation.Data.Assigned[0] != null)
                Hospedes = int.Parse(reservation.Data.Assigned[0].Adults.ToString());
            Origem = reservation.Data.Source;
            IDReserva = reservation.Data.ReservationId;
            IDReservaAgencia = reservation.Data.ThirdPartyIdentifier;
            FonteReserva = reservation.Data.Source;
            Valor = reservation.Data.Total.ToString();
            Balance = reservation.Data.Balance.ToString(); ;
            Status = reservation.Data.Status;
            //SnNum = reservation.Data.Assigned.Length.ToString(); //numero da fnhr
            Snuhnum = reservation.Data.Assigned.Length.ToString();

            
            NomeHospede = reservation.Data.GuestName;

            Guest guest = reservation.Data.GuestLista.First();
            GuestID = guest.GuestId.ToString();    
            if (!string.IsNullOrEmpty(guest.GuestBirthdate.ToString()))
                DataNascimento = DateTime.Parse(guest.GuestBirthdate.ToString());
             
            if (guest.CustomFields.Count() > 0)
            {
                foreach (var item in guest.CustomFields)
                {
                    if (item.CustomFieldName.Equals("CPF"))
                        Cpf = item.CustomFieldValue;

                    if (item.CustomFieldName.Equals("Data de Nascimento"))
                    {
                        try
                        {
                            DataNascimento = DateTime.ParseExact(item.CustomFieldValue.Replace("*", "").Replace(" ", "").Replace("-", "").Replace("/", ""), "ddMMyyyy", CultureInfo.InvariantCulture);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(IDReserva+ "-Erro ao buscar Data de Nascimento: " + ex.Message );
                        }
                    }

                }
            }


            Genero = guest.GuestGender.Equals("N/A") ? "M" : guest.GuestGender;
            Email = reservation.Data.GuestEmail;
            Numerotelefone = string.IsNullOrEmpty(guest.GuestPhone) ? guest.GuestCellPhone : guest.GuestPhone;
            Numerotelefone = !string.IsNullOrEmpty(guest.GuestCellPhone) && string.IsNullOrEmpty(Numerotelefone) ? guest.GuestCellPhone : Numerotelefone;

            Address = guest.GuestAddress;
            Postalcode = guest.GuestZip;
            Contry = guest.GuestCountry.Contains("BR") ? "BRASIL" : guest.GuestCountry;
            CEP = guest.GuestZip.Replace("-", "");
            Cidade = guest.GuestCity;
            Estado = guest.GuestState;

            if (!string.IsNullOrEmpty(CEP) && CEP.Length == 8)
            {
                try
                {
                    Cep cep = CEP;
                    var viaCepService = ViaCepService.Default();
                    var endereco = viaCepService.ObterEndereco(cep);
                    Cidade = endereco.Localidade;
                    Estado = endereco.UF;
                    CodigoIBGE = endereco.IBGE;
                    Contry = "BRASIL";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(" Erro ao buscar CEP: " + CEP + ex.Message);
                }
            }

            ListaQuartos = new List<Quarto>();
            foreach (var item in reservation.Data.Assigned)
            { 
                Quarto quarto = new Quarto();
                quarto.ID = item.RoomTypeId.ToString();
                quarto.Descricao = item.RoomTypeName;
                ListaQuartos.Add(quarto);
            }
             
        }

        public void Converte(ReservationsData reservation)
        {
            CustomerID = reservation.ReservationId;
            DataCheckIn = DateTime.Parse(reservation.StartDate.ToString());
            DataCheckOut = DateTime.Parse(reservation.EndDate.ToString());
            Origem = reservation.SourceName;
            IDReserva = reservation.ReservationId;
            IDReservaAgencia = reservation.ThirdPartyIdentifier;
            FonteReserva = reservation.SourceName;
            Status = reservation.Status;
            NomeHospede = reservation.GuestName;
            GuestID = reservation.GuestId.ToString();
            Balance = reservation.Balance.ToString();

        }


    }

    public class Nota
    {
        public string? Id { get; set; }
        public string? Texto { get; set; }

        public Nota(string id, string texto)
        {
            Id = id;
            Texto = texto;
        }
    }

}