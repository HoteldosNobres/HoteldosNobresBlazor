using Newtonsoft.Json;


namespace HoteldosNobresBlazor.Classes
{
    public partial class Reservations
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("data")]
        public ReservationsData[] Data { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("total")]
        public long Total { get; set; }
    }

    public partial class ReservationsData
    {
        [JsonProperty("propertyID")]
        public string PropertyId { get; set; }

        [JsonProperty("reservationID")]
        public string ReservationId { get; set; }

        [JsonProperty("dateCreated")]
        public DateTimeOffset DateCreated { get; set; }

        [JsonProperty("dateModified")]
        public DateTimeOffset DateModified { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("guestID")]
        public string GuestId { get; set; }

        [JsonProperty("guestName")]
        public string GuestName { get; set; }

        [JsonProperty("startDate")]
        public DateTimeOffset StartDate { get; set; }

        [JsonProperty("endDate")]
        public DateTimeOffset EndDate { get; set; }

        [JsonProperty("adults")]
        public long Adults { get; set; }

        [JsonProperty("children")]
        public long Children { get; set; }

        [JsonProperty("balance")]
        public decimal Balance { get; set; }

        [JsonProperty("sourceName")]
        public string SourceName { get; set; }

        [JsonProperty("thirdPartyIdentifier")]
        public string ThirdPartyIdentifier { get; set; }

        [JsonProperty("subReservationID")]
        public string SubReservationId { get; set; }

        [JsonProperty("guestList")]
        public System.Collections.Generic.Dictionary<string, GuestList>? GuestList { get; set; }
    }

    public partial class GuestList
    {
        [JsonProperty("guestID")]
        public string GuestId { get; set; }

        [JsonProperty("guestName")]
        public string GuestName { get; set; }

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

        [JsonProperty("guestAddress")]
        public string GuestAddress { get; set; }

        [JsonProperty("guestAddress2")]
        public string GuestAddress2 { get; set; }

        [JsonProperty("guestCity")]
        public string GuestCity { get; set; }

        [JsonProperty("guestState")]
        public string GuestState { get; set; }

        [JsonProperty("guestCountry")]
        public string GuestCountry { get; set; }

        [JsonProperty("guestZip")]
        public string GuestZip { get; set; }

        [JsonProperty("guestBirthdate")]
        public DateTimeOffset? GuestBirthdate { get; set; }

        [JsonProperty("guestDocumentType")]
        public string GuestDocumentType { get; set; }

        [JsonProperty("guestDocumentNumber")]
        public string GuestDocumentNumber { get; set; }

        [JsonProperty("guestDocumentIssueDate")]
        public DateTimeOffset? GuestDocumentIssueDate { get; set; }

        [JsonProperty("guestDocumentIssuingCountry")]
        public string GuestDocumentIssuingCountry { get; set; }

        [JsonProperty("guestDocumentExpirationDate")]
        public DateTimeOffset? GuestDocumentExpirationDate { get; set; }

        [JsonProperty("taxID")]
        public string TaxId { get; set; }

        [JsonProperty("companyTaxID")]
        public string CompanyTaxId { get; set; }

        [JsonProperty("companyName")]
        public string CompanyName { get; set; }

        [JsonProperty("subReservationID")]
        public string SubReservationId { get; set; }

        [JsonProperty("startDate")]
        public DateTimeOffset? StartDate { get; set; }

        [JsonProperty("endDate")]
        public DateTimeOffset? EndDate { get; set; }

        [JsonProperty("assignedRoom")]
        public string AssignedRoom { get; set; }

        [JsonProperty("roomID")]
        public string RoomId { get; set; }

        [JsonProperty("roomName")]
        public string RoomName { get; set; }

        [JsonProperty("roomTypeName")]
        public string RoomTypeName { get; set; }

        [JsonProperty("rooms")]
        public Room[] Rooms { get; set; }

        [JsonProperty("customFields")]
        public CustomField[] CustomFields { get; set; }

        [JsonProperty("isAnonymized")]
        public bool IsAnonymized { get; set; }

        [JsonProperty("isMainGuest")]
        public bool IsMainGuest { get; set; }
    }

}