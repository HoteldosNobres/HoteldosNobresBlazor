using Newtonsoft.Json;


namespace HoteldosNobresBlazor.Classes
{
    public partial class CreateReservation
    {
        [JsonProperty("version")]
        public string version { get; set; }

        [JsonProperty("timestamp")]
        public string timestamp { get; set; }

        [JsonProperty("event")]
        public string evento { get; set; }

        [JsonProperty("propertyId")]
        public string propertyId { get; set; }

        [JsonProperty("reservationId")]
        public string reservationId { get; set; }
         
        [JsonProperty("startDate")]
        public string startDate { get; set; }
         
        [JsonProperty("endDate")]
        public string endDate { get; set; }

    }

    public partial class ChangedReservation
    {
        [JsonProperty("version")]
        public string version { get; set; }

        [JsonProperty("timestamp")]
        public string timestamp { get; set; }

        [JsonProperty("event")]
        public string evento { get; set; }

        [JsonProperty("propertyId")]
        public string propertyId { get; set; }

        [JsonProperty("reservationId")]
        public string reservationId { get; set; }

        [JsonProperty("status")]
        public string status { get; set; }
         

    }

    public partial class Accommodation_changed
    {
        [JsonProperty("version")]
        public string version { get; set; }

        [JsonProperty("timestamp")]
        public string timestamp { get; set; }

        [JsonProperty("event")]
        public string evento { get; set; }

        [JsonProperty("propertyId")]
        public string propertyId { get; set; }

        [JsonProperty("guestID")]
        public string guestID { get; set; }

        [JsonProperty("reservationId")]
        public string reservationId { get; set; }

        [JsonProperty("roomID")]
        public string roomID { get; set; }


    }

    public partial class Details_changed
    {
        [JsonProperty("version")]
        public string version { get; set; }

        [JsonProperty("timestamp")]
        public string timestamp { get; set; }

        [JsonProperty("event")]
        public string evento { get; set; }

        [JsonProperty("propertyId")]
        public string propertyId { get; set; }

        [JsonProperty("reservationId")]
        public string reservationId { get; set; }

        [JsonProperty("guestID")]
        public string guestID { get; set; }

        [JsonProperty("roomID")]
        public string roomID { get; set; }

        [JsonProperty("startDate")]
        public string startDate { get; set; }

        [JsonProperty("endDate")]
        public string endDate { get; set; }

    }

}