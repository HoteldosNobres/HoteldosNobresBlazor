using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HoteldosNobresBlazor.Classes.PagSeguroRecebe;


public partial class OrderPagSeguroRecebe
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("reference_id")]
    public string ReferenceId { get; set; }

    [JsonProperty("created_at")]
    public string CreatedAt { get; set; }

    [JsonProperty("customer")]
    public Customer? Customer { get; set; }

    [JsonProperty("items")]
    public Item[]? Items { get; set; }

    [JsonProperty("qr_codes")]
    public Qr_Codes[]? QrCodes { get; set; }

    [JsonProperty("notification_urls")]
    public Uri[]? NotificationUrls { get; set; }

    [JsonProperty("links")]
    public Link[]? Links { get; set; }

    [JsonProperty("error_messages")]
    public ErrorMessages[]? Error_messages { get; set; }
}

public partial class Customer
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("tax_id")]
    public string TaxId { get; set; }

    [JsonProperty("phones")]
    public Phone[] Phones { get; set; }
}

public partial class Phone
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("country")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long Country { get; set; }

    [JsonProperty("area")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long Area { get; set; }

    [JsonProperty("number")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long Number { get; set; }
}

public partial class Item
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("quantity")]
    public long Quantity { get; set; }

    [JsonProperty("unit_amount")]
    public long UnitAmount { get; set; }
}

public partial class Link
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

public partial class Qr_Codes
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("expiration_date")]
    public string ExpirationDate { get; set; }

    [JsonProperty("amount")]
    public Amount? Amount { get; set; }

    [JsonProperty("text")]
    public string Text { get; set; }

    [JsonProperty("links")]
    public Link[]? Links { get; set; }
}

public class Amount
{
    [JsonProperty("value")]
    public int Value { get; set; }
}

public class ErrorMessages
{
    [JsonProperty("code")]
    public string Code { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("parameter_name")]
    public string ParameterName { get; set; }

}