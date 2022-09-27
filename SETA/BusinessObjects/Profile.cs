using Newtonsoft.Json;

namespace SETA.BusinessObjects
{
    public class Profile
    {
        [JsonProperty("ID", NullValueHandling = NullValueHandling.Ignore)]
        public int? Id { get; set; }

        [JsonProperty("MAIL", NullValueHandling = NullValueHandling.Ignore)]
        public string Mail { get; set; }

        [JsonProperty("NAME", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        /*
        [JsonProperty("FIRSTNAME", NullValueHandling = NullValueHandling.Ignore)]
        public string FirstName { get; set; } = null!;
        */

        /*
        [JsonProperty("TITEL", NullValueHandling = NullValueHandling.Ignore)]
        public string? Titel { get; set; }
        */

        [JsonProperty("OPTOUT", NullValueHandling = NullValueHandling.Ignore)]
        public int? Optout { get; set; }
    }
}