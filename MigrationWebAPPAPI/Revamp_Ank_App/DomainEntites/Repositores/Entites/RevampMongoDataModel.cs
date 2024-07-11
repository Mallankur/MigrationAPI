using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Revamp_Ank_App.DomainEntites.Repositores.Entites;

namespace Revamp_Ank_App.Domain.Repositores.Entites
{

    /// <summary>
    /// Mongo Entites . 
    /// </summary>
    [BsonIgnoreExtraElements]
    public class RevampMongoDataModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        [Newtonsoft.Json.JsonProperty("Sapunitno")]

        public int? Sapunitno { get; set; }
        [JsonProperty("Cycleno")]
        [JsonConverter(typeof(IntToLongConverter))]
        public long? Cycleno { get; set; }
        [JsonProperty("CycleId")]
        [JsonConverter(typeof(IntToLongConverter))]
        public long? CycleId { get; set; }
        public int? DOS { get; set; }

        public long? DM { get; set; }
        private DateTime? _dosDate;

        public long? DOSDATE
        {
            get
            {
                return _dosDate.HasValue ? ConvertToMilliseconds(_dosDate.Value) : (long?)null;
            }
            set
            {
                _dosDate = value.HasValue ? ConvertFromMilliseconds(value.Value) : (DateTime?)null;
            }
        }

        private long ConvertToMilliseconds(DateTime date)
        {
            return (long)(date - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        private DateTime ConvertFromMilliseconds(long milliseconds)
        {
            return new DateTime(1970, 1, 1).AddMilliseconds(milliseconds);
        }
        [JsonProperty("CC_Fields_Defs_Id")]
        public int? CC_Fields_Defs_Id { get; set; }

        [JsonProperty("CSISValue")]
        // [BsonRepresentation(BsonType.Decimal128)] 
        public double? CSISValue { get; set; }
        [JsonProperty("ImputedValue")]
        public double? ImputedValue { get; set; }
        [JsonProperty("ImputedValueMetric")]
        public double? ImputedValueMetric { get; set; }
        [JsonProperty("ImputedValueImperial")]
        public double? ImputedValueImperial { get; set; }
        [JsonProperty("CleansedValue")]
        public double? CleansedValue { get; set; }
        [JsonProperty("ValueMetric")]
        public double? ValueMetric { get; set; }
        [JsonProperty("ImportedValue")]
        public double? ImportedValue { get; set; }

        [JsonProperty("CSISDataTypeId")]
        [JsonConverter(typeof(IntToLongConverter))]
        public long? CSISDataTypeId { get; set; }
        [JsonProperty("CSISTestRunId")]
        [JsonConverter(typeof(IntToLongConverter))]
        public long? CSISTestRunId { get; set; }
        [JsonProperty("CSISPredictionId")]
        [JsonConverter(typeof(IntToLongConverter))]
        public long? CSISPredictionId { get; set; }
        [JsonProperty("EOMobileLabId")]
        [JsonConverter(typeof(IntToLongConverter))]
        public long? EOMobileLabId { get; set; }
        [JsonProperty("ReportDataEntityId")]
        [JsonConverter(typeof(IntToLongConverter))]
        public long? ReportDataEntityId { get; set; }


        public bool? IgnoreError { get; set; }
        [JsonProperty("ApplicationId")]
        public int? ApplicationId { get; set; }
        [JsonProperty("Mode")]
        public int? Mode { get; set; }
        [JsonProperty("Value")]
        public double? Value { get; set; }
        [JsonProperty("ReportDataHeaderId")]
        public int? ReportDataHeaderId { get; set; }
        [JsonProperty("CatCheckConnectData")]
        public double? CatCheckConnectData { get; set; }

    }
}

