using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HoteldosNobresBlazor.Classes
{
    public partial class Notes
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("data")]
        public Datum[] Data { get; set; }
    }

    public partial class Datum
    {
        [JsonProperty("reservationNoteID")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long ReservationNoteId { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("dateCreated")]
        public DateTimeOffset DateCreated { get; set; }

        [JsonProperty("dateModified")]
        public object DateModified { get; set; }

        [JsonProperty("reservationNote")]
        public string ReservationNote { get; set; }
    }
     
}
