using Newtonsoft.Json;
using System.Xml.Serialization;

namespace HoteldosNobresBlazor.Classes;

[XmlRoot(ElementName = "paymentMethod")]
public class PaymentMethod
{

    [XmlElement(ElementName = "type")]
    public int Type { get; set; }

    [XmlElement(ElementName = "code")]
    public int Code { get; set; }
}

[XmlRoot(ElementName = "creditorFees")]
public class CreditorFees
{

    [XmlElement(ElementName = "intermediationRateAmount")]
    public double IntermediationRateAmount { get; set; }

    [XmlElement(ElementName = "intermediationFeeAmount")]
    public double IntermediationFeeAmount { get; set; }
}

[XmlRoot(ElementName = "item")]
public class Item
{

    [XmlElement(ElementName = "id")]
    public int Id { get; set; }

    [XmlElement(ElementName = "description")]
    public string Description { get; set; }

    [XmlElement(ElementName = "quantity")]
    public int Quantity { get; set; }

    [XmlElement(ElementName = "amount")]
    public double Amount { get; set; }
}

[XmlRoot(ElementName = "items")]
public class Items
{

    [XmlElement(ElementName = "item")]
    public List<Item> Item { get; set; }
}

[XmlRoot(ElementName = "phone")]
public class Phone
{

    [XmlElement(ElementName = "areaCode")]
    public int AreaCode { get; set; }

    [XmlElement(ElementName = "number")]
    public int Number { get; set; }
}

[XmlRoot(ElementName = "sender")]
public class Sender
{

    [XmlElement(ElementName = "name")]
    public string Name { get; set; }

    [XmlElement(ElementName = "email")]
    public string Email { get; set; }

    [XmlElement(ElementName = "phone")]
    public Phone Phone { get; set; }
}

[XmlRoot(ElementName = "address")]
public class Address
{

    [XmlElement(ElementName = "street")]
    public string Street { get; set; }

    [XmlElement(ElementName = "number")]
    public int Number { get; set; }

    [XmlElement(ElementName = "complement")]
    public string Complement { get; set; }

    [XmlElement(ElementName = "district")]
    public string District { get; set; }

    [XmlElement(ElementName = "postalCode")]
    public int PostalCode { get; set; }

    [XmlElement(ElementName = "city")]
    public string City { get; set; }

    [XmlElement(ElementName = "state")]
    public string State { get; set; }

    [XmlElement(ElementName = "country")]
    public string Country { get; set; }
}

[XmlRoot(ElementName = "shipping")]
public class Shipping
{

    [XmlElement(ElementName = "address")]
    public Address Address { get; set; }

    [XmlElement(ElementName = "type")]
    public int Type { get; set; }

    [XmlElement(ElementName = "cost")]
    public double Cost { get; set; }
}

[XmlRoot(ElementName = "liquidation")]
public class Liquidation
{

    [XmlElement(ElementName = "contractType")]
    public string ContractType { get; set; }

    [XmlElement(ElementName = "contractDescription")]
    public string ContractDescription { get; set; }
}

[XmlRoot(ElementName = "transaction")]
public class Transaction
{

    [XmlElement(ElementName = "date")]
    public DateTime Date { get; set; }

    [XmlElement(ElementName = "code")]
    public string Code { get; set; }

    [XmlElement(ElementName = "reference")]
    public string Reference { get; set; }

    [XmlElement(ElementName = "type")]
    public int Type { get; set; }

    [XmlElement(ElementName = "status")]
    public int Status { get; set; }

    [XmlElement(ElementName = "paymentMethod")]
    public PaymentMethod PaymentMethod { get; set; }

    [XmlElement(ElementName = "grossAmount")]
    public double GrossAmount { get; set; }

    [XmlElement(ElementName = "discountAmount")]
    public double DiscountAmount { get; set; }

    [XmlElement(ElementName = "creditorFees")]
    public CreditorFees CreditorFees { get; set; }

    [XmlElement(ElementName = "netAmount")]
    public double NetAmount { get; set; }

    [XmlElement(ElementName = "extraAmount")]
    public double ExtraAmount { get; set; }

    [XmlElement(ElementName = "installmentCount")]
    public int InstallmentCount { get; set; }

    [XmlElement(ElementName = "itemCount")]
    public int ItemCount { get; set; }

    [XmlElement(ElementName = "items")]
    public Items Items { get; set; }

    [XmlElement(ElementName = "sender")]
    public Sender Sender { get; set; }

    [XmlElement(ElementName = "shipping")]
    public Shipping Shipping { get; set; }

    [XmlElement(ElementName = "liquidation")]
    public Liquidation Liquidation { get; set; }
}
