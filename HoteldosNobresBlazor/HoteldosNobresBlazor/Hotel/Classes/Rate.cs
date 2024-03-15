using Newtonsoft.Json;


namespace HoteldosNobresBlazor.Classes
{
    public partial class Payment
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("data")]
        public PaymentData[] Data { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("total")]
        public long Total { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public partial class PaymentData
    {
        [JsonProperty("transactionID")]
        public string TransactionId { get; set; }

        [JsonProperty("paymentID")]
        public string PaymentId { get; set; }

        [JsonProperty("propertyID")]
        public string PropertyId { get; set; }

        [JsonProperty("transactionDateTime")]
        public DateTimeOffset TransactionDateTime { get; set; }

        [JsonProperty("transactionDateTimeUTC")]
        public DateTimeOffset TransactionDateTimeUtc { get; set; }

        [JsonProperty("userID")]
        public string UserId { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("roomID")]
        public string RoomId { get; set; }

        [JsonProperty("roomName")]
        public string RoomName { get; set; }

        [JsonProperty("guestID")]
        public string GuestId { get; set; }

        [JsonProperty("guestName")]
        public string GuestName { get; set; }

        [JsonProperty("guestCheckIn")]
        public DateTimeOffset GuestCheckIn { get; set; }

        [JsonProperty("guestCheckOut")]
        public DateTimeOffset GuestCheckOut { get; set; }

        [JsonProperty("reservationID")]
        public string ReservationId { get; set; }

        [JsonProperty("subReservationID")]
        public string SubReservationId { get; set; }

        [JsonProperty("reservationStatus")]
        public string ReservationStatus { get; set; }

        [JsonProperty("houseAccountID")]
        public string HouseAccountId { get; set; }

        [JsonProperty("houseAccountName")]
        public string HouseAccountName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("paymentMethod")]
        public string PaymentMethod { get; set; }

        [JsonProperty("quantity")]
        public long Quantity { get; set; }

        [JsonProperty("amount")]
        public long Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("isPosted")]
        public bool IsPosted { get; set; }

        [JsonProperty("isVoided")]
        public bool IsVoided { get; set; }

        [JsonProperty("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("isAllocated")]
        public bool IsAllocated { get; set; }

        [JsonProperty("totalAllocated")]
        public long TotalAllocated { get; set; }

        [JsonProperty("totalUnallocated")]
        public long TotalUnallocated { get; set; }

        [JsonProperty("paymentAllocation")]
        public PaymentAllocation[] PaymentAllocation { get; set; }
    }

    public partial class PaymentAllocation
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("reference")]
        public string Reference { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("amount")]
        public long Amount { get; set; }

        [JsonProperty("taxes")]
        public string[] Taxes { get; set; }

        [JsonProperty("fees")]
        public string[] Fees { get; set; }
    }

    public partial class RespostPayment
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("paymentID")]
        public string PaymentId { get; set; }

        [JsonProperty("transactionID")]
        public string TransactionId { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }

}