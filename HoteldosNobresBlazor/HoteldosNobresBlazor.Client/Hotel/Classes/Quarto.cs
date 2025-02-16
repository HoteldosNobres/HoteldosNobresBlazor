using MosaicoSolutions.ViaCep;
using MosaicoSolutions.ViaCep.Modelos;
using System.Globalization;

namespace HoteldosNobresBlazor.Classes;

public class Quarto
{
    //nformações da reserva
    public string? ID { get; set; }
    public string? TypeID { get; set; }
    public string? Descricao { get; set; }

    public long? Adults { get; set; }
    public long? Children { get; set; }

    public decimal? Total { get; set; }

}