using Newtonsoft.Json;
 
namespace HoteldosNobresBlazor.Classes
{
    public partial class Rate
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("data")]
        public RateData Data { get; set; }
    }

    public partial class RateData
    {
        [JsonProperty("rateID")]
        public string RateId { get; set; }

        [JsonProperty("isDerived")]
        public bool IsDerived { get; set; }

        [JsonProperty("roomRate")]
        public decimal RoomRate { get; set; }

        [JsonProperty("totalRate")]
        public decimal TotalRate { get; set; }

        [JsonProperty("roomsAvailable")]
        public long RoomsAvailable { get; set; }

        [JsonProperty("daysOfWeek")]
        public string[] DaysOfWeek { get; set; }

        [JsonProperty("roomRateDetailed")]
        public RoomRateDetailed[] RoomRateDetailed { get; set; }
    }

    public partial class RoomRateDetailed
    {
        [JsonProperty("date")]
        public DateTimeOffset Date { get; set; }

        [JsonProperty("rate")]
        public long Rate { get; set; }

        [JsonProperty("totalRate")]
        public long TotalRate { get; set; }

        [JsonProperty("roomsAvailable")]
        public long RoomsAvailable { get; set; }

        [JsonProperty("closedToArrival")]
        public bool ClosedToArrival { get; set; }

        [JsonProperty("closedToDeparture")]
        public bool ClosedToDeparture { get; set; }

        [JsonProperty("minLos")]
        public long MinLos { get; set; }

        [JsonProperty("maxLos")]
        public long MaxLos { get; set; }
    }

}