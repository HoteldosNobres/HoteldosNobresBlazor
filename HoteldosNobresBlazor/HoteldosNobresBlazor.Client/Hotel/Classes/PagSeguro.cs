using Newtonsoft.Json;
namespace HoteldosNobresBlazor.Classes.PagSeguro;
 
public class OrderDTO
{
    [JsonProperty("reference_id")]
    public string ReferenceId { get; set; }
     
    [JsonProperty("customer")]
    public Customer Customer { get; set; }
     
    [JsonProperty("items")]
    public List<ItemOrder> Items { get; set; }

    [JsonProperty("qr_codes")]
    public List<QrCode> QrCodes { get; set; }
     
    [JsonProperty("notification_urls")]
    public List<string> NotificationUrls { get; set; }
}

public class Customer
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("tax_id")]
    public string TaxId { get; set; }

    [JsonProperty("phones")]
    public PhonePagSeguro[]? Phones { get; set; }
}

public partial class PhonePagSeguro
{
    [JsonProperty("country")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long? Country { get; set; }

    [JsonProperty("area")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long? Area { get; set; }

    [JsonProperty("number")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long? Number { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }
}

public class ItemOrder
{ 

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("quantity")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long? Quantity { get; set; }

    [JsonProperty("unit_amount")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long? UnitAmount { get; set; }
}

public class QrCode
{
    [JsonProperty("amount")]
    public Amount Amount { get; set; }

    //[JsonProperty("expiration_date")]
    //public DateTime? ExpirationDate { get; set; }
}

public class Amount
{
    [JsonProperty("value")]
    public long Value { get; set; }
}

public partial class LinkPagseguro
{
    [JsonProperty("rel")]
    public string Rel { get; set; }

    [JsonProperty("href")]
    public string Href { get; set; }

    [JsonProperty("media")]
    public string Media { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }
}

public partial class QrCodes
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("expiration_date")]
    public DateTimeOffset? ExpirationDate { get; set; }

    [JsonProperty("amount")]
    public Amount? Amount { get; set; }

    [JsonProperty("text")]
    public string Text { get; set; }

    [JsonProperty("links")]
    public LinkPagseguro[]? Links { get; set; }
}
 