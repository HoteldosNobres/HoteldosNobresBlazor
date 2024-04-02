using Google.Apis.PeopleService.v1.Data;
using MosaicoSolutions.ViaCep;
using MosaicoSolutions.ViaCep.Modelos;
using Newtonsoft.Json;
using System.Globalization;

namespace HoteldosNobresBlazor.Classes
{
    public partial class MensagemWhatsApp
    {
        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("entry")]
        public Entry[] Entry { get; set; }
    }

    public partial class Entry
    {
        [JsonProperty("id")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Id { get; set; }

        [JsonProperty("changes")]
        public Change[] Changes { get; set; }
    }

    public partial class Change
    {
        [JsonProperty("field")]
        public string Field { get; set; }

        [JsonProperty("value")]
        public Value Value { get; set; }
    }

    public partial class Value
    {
        [JsonProperty("messaging_product")]
        public string MessagingProduct { get; set; }

        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }

        [JsonProperty("contacts")]
        public Contact[]? Contacts { get; set; }

        [JsonProperty("messages")]
        public Message[]? Messages { get; set; }

        [JsonProperty("statuses")]
        public Status[]? Statuses { get; set; }
    }

    public partial class Contact
    {
        [JsonProperty("profile")]
        public Profile Profile { get; set; }

        [JsonProperty("wa_id")]
        public string WaId { get; set; }
    }

    public partial class Profile
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public partial class Message
    {
        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("timestamp")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Timestamp { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("text")]
        public Text Text { get; set; }

        [JsonProperty("interactive")]
        public Interactive? Interactive { get; set; }
    }

    public partial class Text
    {
        [JsonProperty("body")]
        public string Body { get; set; }
    }

    public partial class Metadata
    {
        [JsonProperty("display_phone_number")]
        public string DisplayPhoneNumber { get; set; }

        [JsonProperty("phone_number_id")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long PhoneNumberId { get; set; }
    }

    public partial class Status
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("status")]
        public string StatusStatus { get; set; }

        [JsonProperty("timestamp")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Timestamp { get; set; }

        [JsonProperty("recipient_id")]
        public string RecipientId { get; set; }
    }




    public partial class Interactive
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("nfm_reply")]
        public NfmReply NfmReply { get; set; }
    }

    public partial class NfmReply
    {
        [JsonProperty("response_json")]
        public string ResponseJson { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }


    public partial class Responsejson
    {
        [JsonProperty("response_json")]
        public Response_Json ResponseJson { get; set; }
    }

    public partial class Response_Json
    {
        [JsonProperty("flow_token")]
        public string FlowToken { get; set; }

        [JsonProperty("datadenascimento")] 
        public string Datadenascimento { get; set; }

        [JsonProperty("cpf")] 
        public string Cpf { get; set; }

        [JsonProperty("hotelrating")]
        public string Hotelrating { get; set; }

        [JsonProperty("comment_text")]
        public string Comment_text { get; set; }
    }



}