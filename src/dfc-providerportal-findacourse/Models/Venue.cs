
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Dfc.ProviderPortal.FindACourse.Interfaces;


namespace Dfc.ProviderPortal.FindACourse.Models
{
    public enum VenueStatus
    {
        Undefined = 0,
        Live = 1,
        Deleted = 2,
        Pending = 3,
        Uknown = 99
    }

    public class Venue
    {
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonProperty("id")]
        public Guid ID { get; }
        public int UKPRN { get; }
        [JsonProperty("PROVIDER_ID", Required = Required.AllowNull)]
        //[JsonIgnore]
        public int ProviderID { get; }
        [JsonProperty("VENUE_ID", Required = Required.AllowNull)]
        //[JsonIgnore]
        public int VenueID { get; }
        [JsonProperty("VENUE_NAME")]
        public string VenueName { get; }
        [JsonProperty("PROV_VENUE_ID", Required = Required.AllowNull)]
        //[JsonIgnore]
        public string ProvVenueID { get; }
        [JsonProperty("ADDRESS_1")]
        public string Address1 { get; }
        [JsonProperty("ADDRESS_2")]
        public string Address2 { get; }
        [JsonProperty("ADDRESS_3")]
        public string Address3 { get; }
        [JsonProperty("TOWN")]
        public string Town { get; }
        [JsonProperty("COUNTY")]
        public string County { get; }
        [JsonProperty("POSTCODE")]
        public string PostCode { get; }
        [JsonProperty("LATITUDE")]
        public double? Latitude { get; set; }
        [JsonProperty("LONGITUDE")]
        public double? Longitude { get; set; }
        public VenueStatus Status { get; set; }
        public DateTime DateAdded { get; }
        public DateTime DateUpdated { get; }
        public string UpdatedBy { get; }

        // Apprenticeship related
        public int? LocationId { get; set; }
        public int? TribalLocationId { get; set; }
        [JsonProperty("PHONE")]
        public string Telephone { get; set; }
        [JsonProperty("EMAIL")]
        public string Email { get; set; }
        [JsonProperty("WEBSITE")]
        public string Website { get; set; }
    }
}