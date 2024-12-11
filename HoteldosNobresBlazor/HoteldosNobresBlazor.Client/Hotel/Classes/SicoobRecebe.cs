using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HoteldosNobresBlazor.Classes.SicoobRecebe;


public partial class SicoobRecebe
{ 
    [JsonProperty("pix")]
    public PixSicoob[]? Pix { get; set; }
     
}

public partial class PixSicoob
{
    [JsonProperty("endToEndId")]
    public string EndToEndId { get; set; }

    [JsonProperty("txid")]
    public string Txid { get; set; }

    [JsonProperty("valor")]
    public decimal Valor { get; set; }

    [JsonProperty("horario")]
    public string Horario { get; set; }

    [JsonProperty("infoPagador")]
    public string InfoPagador { get; set; }

    [JsonProperty("devolucoes")]
    public DevolucoesSicoob[]? DevolucoesSicoob { get; set; }
}

public partial class DevolucoesSicoob
{
    [JsonProperty("id")]
    public string? Id { get; set; }
}