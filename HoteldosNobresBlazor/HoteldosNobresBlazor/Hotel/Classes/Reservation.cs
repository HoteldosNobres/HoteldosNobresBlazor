﻿using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HoteldosNobresBlazor.Classes
{
    public partial class Reservation
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }
    }

    public partial class Data
    {
        [JsonProperty("propertyID")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long PropertyId { get; set; }

        [JsonProperty("guestName")]
        public string GuestName { get; set; }

        [JsonProperty("guestEmail")]
        public string GuestEmail { get; set; }

        [JsonProperty("isAnonymized")]
        public bool IsAnonymized { get; set; }

        [JsonProperty("guestList")]
        public Dictionary<string, Guest> GuestList { get; set; }

        [JsonProperty("reservationID")]
        public string ReservationId { get; set; }

        [JsonProperty("dateCreated")]
        public DateTimeOffset DateCreated { get; set; }

        [JsonProperty("dateModified")]
        public DateTimeOffset DateModified { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("thirdPartyIdentifier")]
        public string ThirdPartyIdentifier { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("total")]
        public decimal Total { get; set; }

        [JsonProperty("balance")]
        public decimal Balance { get; set; }

        [JsonProperty("balanceDetailed")]
        public BalanceDetailed BalanceDetailed { get; set; }

        [JsonProperty("assigned")]
        public Assigned[] Assigned { get; set; }

        [JsonProperty("unassigned")]
        public Unassigned[] Unassigned { get; set; }

        [JsonProperty("cardsOnFile")]
        public object[] CardsOnFile { get; set; }

        [JsonProperty("customFields")]
        public object[] CustomFields { get; set; }

        [JsonProperty("startDate")]
        public DateTimeOffset? StartDate { get; set; }

        [JsonProperty("endDate")]
        public DateTimeOffset? EndDate { get; set; }

        [JsonProperty("allotmentBlockCode")]
        public object AllotmentBlockCode { get; set; }

        [JsonProperty("orderId")]
        public string OrderId { get; set; }

        public List<Guest> GuestLista => GuestList.Values.ToList();
    }

    public partial class Assigned
    {
        [JsonProperty("roomTypeName")]
        public string RoomTypeName { get; set; }

        [JsonProperty("roomTypeNameShort")]
        public string RoomTypeNameShort { get; set; }

        [JsonProperty("roomTypeID")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long RoomTypeId { get; set; }

        [JsonProperty("subReservationID")]
        public string SubReservationId { get; set; }

        [JsonProperty("startDate")]
        public DateTimeOffset StartDate { get; set; }

        [JsonProperty("endDate")]
        public DateTimeOffset EndDate { get; set; }

        [JsonProperty("adults")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Adults { get; set; }

        [JsonProperty("children")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Children { get; set; }

        [JsonProperty("dailyRates")]
        public DailyRate[] DailyRates { get; set; }

        [JsonProperty("roomTotal")]
        public string RoomTotal { get; set; }

        [JsonProperty("roomName")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long RoomName { get; set; }

        [JsonProperty("roomID")]
        public string RoomId { get; set; }
    }

    public partial class Unassigned
    {
        [JsonProperty("roomTypeName")]
        public string RoomTypeName { get; set; }

        [JsonProperty("roomTypeNameShort")]
        public string RoomTypeNameShort { get; set; }

        [JsonProperty("roomTypeID")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long RoomTypeId { get; set; }

        [JsonProperty("subReservationID")]
        public string SubReservationId { get; set; }

        [JsonProperty("startDate")]
        public DateTimeOffset StartDate { get; set; }

        [JsonProperty("endDate")]
        public DateTimeOffset EndDate { get; set; }

        [JsonProperty("adults")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Adults { get; set; }

        [JsonProperty("children")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Children { get; set; }

        [JsonProperty("dailyRates")]
        public DailyRate[] DailyRates { get; set; }

        [JsonProperty("roomTotal")]
        public string RoomTotal { get; set; }

    }

    public partial class DailyRate
    {
        [JsonProperty("date")]
        public DateTimeOffset Date { get; set; }

        [JsonProperty("rate")]
        public long Rate { get; set; }
    }

    public partial class BalanceDetailed
    {
        [JsonProperty("suggestedDeposit")]
        public string SuggestedDeposit { get; set; }

        [JsonProperty("subTotal")]
        public double SubTotal { get; set; }

        [JsonProperty("additionalItems")]
        public long AdditionalItems { get; set; }

        [JsonProperty("taxesFees")]
        public double TaxesFees { get; set; }

        [JsonProperty("grandTotal")]
        public long GrandTotal { get; set; }

        [JsonProperty("paid")]
        public long Paid { get; set; }


    }

    public partial class Guest
    {
        [JsonProperty("guestID")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long GuestId { get; set; }

        [JsonProperty("guestFirstName")]
        public string GuestFirstName { get; set; }

        [JsonProperty("guestLastName")]
        public string GuestLastName { get; set; }

        [JsonProperty("guestGender")]
        public string GuestGender { get; set; }

        [JsonProperty("guestEmail")]
        public string GuestEmail { get; set; }

        [JsonProperty("guestPhone")]
        public string GuestPhone { get; set; }

        [JsonProperty("guestCellPhone")]
        public string GuestCellPhone { get; set; }

        [JsonProperty("guestCountry")]
        public string GuestCountry { get; set; }

        [JsonProperty("guestAddress")]
        public string GuestAddress { get; set; }

        [JsonProperty("guestAddress2")]
        public string GuestAddress2 { get; set; }

        [JsonProperty("guestCity")]
        public string GuestCity { get; set; }

        [JsonProperty("guestZip")]
        public string GuestZip { get; set; }

        [JsonProperty("guestState")]
        public string GuestState { get; set; }

        [JsonProperty("guestStatus")]
        public string GuestStatus { get; set; }

        [JsonProperty("guestBirthdate")]
        public string GuestBirthdate { get; set; }

        [JsonProperty("guestDocumentType")]
        public string GuestDocumentType { get; set; }

        [JsonProperty("guestDocumentNumber")]
        public string GuestDocumentNumber { get; set; }

        [JsonProperty("guestDocumentIssueDate")]
        public string GuestDocumentIssueDate { get; set; }

        [JsonProperty("guestDocumentIssuingCountry")]
        public string GuestDocumentIssuingCountry { get; set; }

        [JsonProperty("guestDocumentExpirationDate")]
        public string GuestDocumentExpirationDate { get; set; }

        [JsonProperty("assignedRoom")]
        public bool AssignedRoom { get; set; }

        [JsonProperty("roomID")]
        public string RoomId { get; set; }

        [JsonProperty("roomName")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long RoomName { get; set; }

        [JsonProperty("roomTypeName")]
        public string RoomTypeName { get; set; }

        [JsonProperty("isMainGuest")]
        public bool IsMainGuest { get; set; }

        [JsonProperty("isAnonymized")]
        public bool IsAnonymized { get; set; }

        [JsonProperty("taxID")]
        public string TaxId { get; set; }

        [JsonProperty("companyTaxID")]
        public string CompanyTaxId { get; set; }

        [JsonProperty("companyName")]
        public string CompanyName { get; set; }

        [JsonProperty("customFields")]
        public CustomField[] CustomFields { get; set; }

        [JsonProperty("unassignedRooms")]
        public object[] UnassignedRooms { get; set; }

        [JsonProperty("rooms")]
        public Room[] Rooms { get; set; }
    }

    public partial class CustomField
    {
        [JsonProperty("customFieldName")]
        public string CustomFieldName { get; set; }

        [JsonProperty("customFieldValue")]
        public string CustomFieldValue { get; set; }
    }

    public partial class Room
    {
        [JsonProperty("roomTypeID")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long RoomTypeId { get; set; }

        [JsonProperty("roomTypeName")]
        public string RoomTypeName { get; set; }

        [JsonProperty("roomID")]
        public string RoomId { get; set; }

        [JsonProperty("roomName")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long RoomName { get; set; }

        [JsonProperty("subReservationID")]
        public string SubReservationId { get; set; }
    }

     

    

     

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return new object();
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}