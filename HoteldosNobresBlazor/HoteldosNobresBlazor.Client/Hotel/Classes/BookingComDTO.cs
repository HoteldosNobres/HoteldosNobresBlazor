using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HoteldosNobresBlazor.Classes.BookingComDTO;


public partial class BookingComDTO
{ 
    [JsonProperty("request_id")]
    public string request_id { get; set; }

    [JsonProperty("data")]
    public Accommodation[]? Accommodation { get; set; }

}

public partial class Accommodation
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("currency")]
    public string Currency { get; set; }

    [JsonProperty("deep_link_url")]
    public string Deep_link_url { get; set; }

    [JsonProperty("price")]
    public Price? Price { get; set; }
     
    [JsonProperty("url")]
    public string Url { get; set; }
}


public partial class Price
{
    [JsonProperty("total")]
    public decimal? Total { get; set; }
}