using Google.Apis.PeopleService.v1.Data;
using MosaicoSolutions.ViaCep;
using MosaicoSolutions.ViaCep.Modelos;
using MudBlazor.Extensions;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace HoteldosNobresBlazor.Classes;

public class Reserva
{
    //nformações da reserva
    public string? CustomerID { get; set; }
    public DateTime? DataCheckIn { get; set; }
    public DateTime DataCheckInRealizado { get; set; }
    public DateTime? DataCheckOut { get; set; }
    public DateTime DataCheckOutRealizado { get; set; }
    public DateTime? HorarioAproximado { get; set; }
    public int? Noites { get; set; }
    public int? Hospedes { get; set; }
    public string? Origem { get; set; }
    public string? IDReserva { get; set; }
    public string? IDReservaAgencia { get; set; }
    public string? FonteReserva { get; set; }
    public string? Valor { get; set; }
    public decimal? Balance { get; set; }
    public string? Status { get; set; }
    public string? SnNum { get; set; }
    public string? Snuhnum { get; set; }

    public List<Nota>? Notas { get; set; }

    public List<Quarto>? ListaQuartos { get; set; }

    public List<Quarto>? ListaQuartosCancelados { get; set; }

    public BalanceDetailed? BalanceDetailed { get; set; }

    public Source? Source { get; set; }

    /// Hospede
    public string? GuestID { get; set; }

    public string? NomeHospede { get; set; }
    public string? Cpf { get; set; }
    public DateTime DataNascimento { get; set; }
    public string? Genero { get; set; }

    //Contato
    public string? Email { get; set; }
    public string? Numerotelefone { get; set; }
    public string? NumeroCelular { get; set; }
    public string? Address { get; set; }
    public string? Postalcode { get; set; }
    public string? Contry { get; set; }
    public string? CEP { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? CodigoIBGE { get; set; }
    public EUF? UF { get; set; }

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
            return Numerotelefone.Replace("+", "").Count() > 11 ? Numerotelefone.Replace("+", "").Substring(4, Numerotelefone.Length - 5) : Numerotelefone.Replace("+", "").Count() <= 9 ? Numerotelefone :  Numerotelefone.Replace("+", "").Substring(2, 9);
        }
    }
     
    public string? ProxyCelular
    {
        get
        {
            if (string.IsNullOrEmpty(Numerotelefone) && string.IsNullOrEmpty(NumeroCelular))
                return "553537150180";
            string cellphone = string.Empty;
            if ( Numerotelefone != null)
                cellphone =  !string.IsNullOrEmpty(NumeroCelular) ? NumeroCelular.Replace("+", "").Replace("-", "").Replace(".", "").Replace(" ", "").ToString() : Numerotelefone.Replace("+", "").Replace(".", "").Replace(" ", "").ToString();
            return cellphone.Length <= 10 ? "55" + cellphone : cellphone;
        }
    }

    public string Obs
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

    public string LinkReservaID
    {
        get
        {
            if (string.IsNullOrEmpty(IDReserva))
                return "";
            return "/reserva/" + IDReserva;
        }
    }

    public string LinkReservaOrigem
    {
        get
        {
            if (string.IsNullOrEmpty(IDReserva))
                return "";

            if (!string.IsNullOrEmpty(Origem) && Origem.ToUpper().Equals("BOOKING.COM"))
                return "https://admin.booking.com/hotel/hoteladmin/extranet_ng/manage/booking.html?hotel_id=6135187&lang=xb&res_id=" + IDReservaAgencia;
            else
                return "";
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
            if(Status != null)
                return Status.ToUpper().Equals("CANCELADO") ? "1" : "0";
            else 
                return "0";
        }
    }

    public string? ProxyStatus
    {
        get
        {
            EStatus status = Status != null ? (EStatus)Enum.Parse(typeof(EStatus), Status) : EStatus.None;
            return status.GetEnumDescription(); 
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

        //Pagamento Cobrança
        Valor = reservation.Data.Total.ToString();
        Balance = reservation.Data.Balance;
        Status = reservation.Data.Status;
        BalanceDetailed = reservation.Data.BalanceDetailed;

        //SnNum = reservation.Data.Assigned.Length.ToString(); //numero da fnhr
        Snuhnum = reservation.Data.Assigned.Length.ToString();
         
        NomeHospede = reservation.Data.GuestName;

        if(!string.IsNullOrEmpty(reservation.Data.EstimatedArrivalTime))
            HorarioAproximado = DateTime.Parse(reservation.Data.EstimatedArrivalTime);

        Guest guest = reservation.Data.GuestLista.First();
        GuestID = guest.GuestId.ToString();    
        if (!string.IsNullOrEmpty(guest.GuestBirthdate.ToString()))
            DataNascimento = DateTime.Parse(guest.GuestBirthdate.ToString());
         
        if (guest.CustomFields.Count() > 0)
        {
            foreach (var item in guest.CustomFields)
            {
                if (item.CustomFieldName.Equals("CPF"))
                    Cpf = Regex.Replace(item.CustomFieldValue, @"[^\d]", "");

                if (item.CustomFieldName.Equals("Data de Nascimento"))
                {
                    try
                    { 
                        DataNascimento = item.CustomFieldValue.Replace("*", "").Replace(" ", "").Replace("-", "").Replace("/", "").Length <= 6
                            ? DateTime.ParseExact(item.CustomFieldValue.Replace("*", "").Replace(" ", "").Replace("-", "").Replace("/", ""), "ddMMyy", CultureInfo.InvariantCulture)
                            : DateTime.ParseExact(item.CustomFieldValue.Replace("*", "").Replace(" ", "").Replace("-", "").Replace("/", ""), "ddMMyyyy", CultureInfo.InvariantCulture);
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
        NumeroCelular = guest.GuestCellPhone;
        Numerotelefone = string.IsNullOrEmpty(guest.GuestPhone) ? guest.GuestCellPhone : guest.GuestPhone;
        Numerotelefone = !string.IsNullOrEmpty(guest.GuestCellPhone) && string.IsNullOrEmpty(Numerotelefone) ? guest.GuestCellPhone : Numerotelefone;

        Address = guest.GuestAddress;
        Postalcode = guest.GuestZip;

        Contry = guest.GuestCountry == null || guest.GuestCountry.Contains("BR")  ? "BRASIL" : guest.GuestCountry;
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
                if(endereco.UF != null)
                    UF = (EUF)Enum.Parse(typeof(EUF), endereco.UF); 
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
            quarto.Adults = item.Adults;
            quarto.Children = item.Children;
            quarto.Total = item.RoomTotal;
            ListaQuartos.Add(quarto);
        }

        ListaQuartosCancelados = new List<Quarto>();
        foreach (var item in reservation.Data.Unassigned)
        {
            Quarto quarto = new Quarto();
            quarto.ID = item.RoomTypeId.ToString();
            quarto.Descricao = item.RoomTypeName;
            ListaQuartosCancelados.Add(quarto);
        }
         
    }

    public void Converte(Reservations reservationdata)
    {
        ReservationsData reservation = reservationdata.Data[0];
        CustomerID = reservation.ReservationId;
        DataCheckIn = DateTime.Parse(reservation.StartDate.ToString());
        // DataCheckInRealizado = reserva.DataCheckInRealizado;
        DataCheckOut = DateTime.Parse(reservation.EndDate.ToString());
        //  DataCheckOutRealizado = reserva.DataCheckOutRealizado;
        //Noites = reserva.Noites;
        //if (reservation.Assigned.Count() > 0 && reservation.Assigned[0] != null)
        //    Hospedes = int.Parse(reservation.Assigned[0].Adults.ToString());
        Origem = reservation.SourceName;
        IDReserva = reservation.ReservationId;
        IDReservaAgencia = reservation.ThirdPartyIdentifier;
        FonteReserva = reservation.SourceName;

        //Pagamento Cobrança
        //Valor = reservation.Total.ToString();
        Balance = reservation.Balance;
        Status = reservation.Status;
        BalanceDetailed = reservation.BalanceDetailed;
         
        //Snuhnum = reservation.Assigned.Length.ToString();

        NomeHospede = reservation.GuestName;

        //if (!string.IsNullOrEmpty(reservation.EstimatedArrivalTime))
        //    HorarioAproximado = DateTime.Parse(reservation.EstimatedArrivalTime);

        //Guest guest = reservation.GuestLista.First();
        //GuestID = guest.GuestId.ToString();
        //if (!string.IsNullOrEmpty(guest.GuestBirthdate.ToString()))
        //    DataNascimento = DateTime.Parse(guest.GuestBirthdate.ToString());

        //if (guest.CustomFields.Count() > 0)
        //{
        //    foreach (var item in guest.CustomFields)
        //    {
        //        if (item.CustomFieldName.Equals("CPF"))
        //            Cpf = Regex.Replace(item.CustomFieldValue, @"[^\d]", "");

        //        if (item.CustomFieldName.Equals("Data de Nascimento"))
        //        {
        //            try
        //            {
        //                DataNascimento = item.CustomFieldValue.Replace("*", "").Replace(" ", "").Replace("-", "").Replace("/", "").Length <= 6
        //                    ? DateTime.ParseExact(item.CustomFieldValue.Replace("*", "").Replace(" ", "").Replace("-", "").Replace("/", ""), "ddMMyy", CultureInfo.InvariantCulture)
        //                    : DateTime.ParseExact(item.CustomFieldValue.Replace("*", "").Replace(" ", "").Replace("-", "").Replace("/", ""), "ddMMyyyy", CultureInfo.InvariantCulture);
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine(IDReserva + "-Erro ao buscar Data de Nascimento: " + ex.Message);
        //            }
        //        }

        //    }
        //}

        //Genero = guest.GuestGender.Equals("N/A") ? "M" : guest.GuestGender;
        //Email = reservation.GuestEmail;
        //NumeroCelular = guest.GuestCellPhone;
        //Numerotelefone = string.IsNullOrEmpty(guest.GuestPhone) ? guest.GuestCellPhone : guest.GuestPhone;
        //Numerotelefone = !string.IsNullOrEmpty(guest.GuestCellPhone) && string.IsNullOrEmpty(Numerotelefone) ? guest.GuestCellPhone : Numerotelefone;

        //Address = guest.GuestAddress;
        //Postalcode = guest.GuestZip;

        //Contry = guest.GuestCountry == null || guest.GuestCountry.Contains("BR") ? "BRASIL" : guest.GuestCountry;
        //CEP = guest.GuestZip.Replace("-", "");
        //Cidade = guest.GuestCity;
        //Estado = guest.GuestState;

        //if (!string.IsNullOrEmpty(CEP) && CEP.Length == 8)
        //{
        //    try
        //    {
        //        Cep cep = CEP;
        //        var viaCepService = ViaCepService.Default();
        //        var endereco = viaCepService.ObterEndereco(cep);
        //        Cidade = endereco.Localidade;
        //        if (endereco.UF != null)
        //            UF = (EUF)Enum.Parse(typeof(EUF), endereco.UF);
        //        Estado = endereco.UF;
        //        CodigoIBGE = endereco.IBGE;
        //        Contry = "BRASIL";
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(" Erro ao buscar CEP: " + CEP + ex.Message);
        //    }
        //}

        //ListaQuartos = new List<Quarto>();
        //foreach (var item in reservation.Assigned)
        //{
        //    Quarto quarto = new Quarto();
        //    quarto.ID = item.RoomTypeId.ToString();
        //    quarto.Descricao = item.RoomTypeName;
        //    quarto.Adults = item.Adults;
        //    quarto.Children = item.Children;
        //    quarto.Total = item.RoomTotal;
        //    ListaQuartos.Add(quarto);
        //}

        //ListaQuartosCancelados = new List<Quarto>();
        //foreach (var item in reservation.Unassigned)
        //{
        //    Quarto quarto = new Quarto();
        //    quarto.ID = item.RoomTypeId.ToString();
        //    quarto.Descricao = item.RoomTypeName;
        //    ListaQuartosCancelados.Add(quarto);
        //}

        if (reservation.Source is not null)
        {
            Source = reservation.Source;
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
        Balance = reservation.Balance;

        if(reservation.GuestList != null)
        {
            GuestList guest = reservation.GuestList.Values.FirstOrDefault();
            NumeroCelular = guest != null && !string.IsNullOrEmpty(guest.GuestCellPhone) ? guest.GuestCellPhone : string.Empty;
            Numerotelefone = guest != null && !string.IsNullOrEmpty(guest.GuestPhone) ? guest.GuestPhone : string.Empty;
            Email = guest != null && !string.IsNullOrEmpty(guest.GuestEmail) ? guest.GuestEmail : string.Empty;

        }


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