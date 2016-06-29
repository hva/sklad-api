using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Warehouse.Server.Models
{
    public class Product
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("size")]
        public string Size { get; set; }

        [BsonElement("k")]
        public double K { get; set; }

        [BsonElement("price_opt")]
        public double PriceOpt { get; set; }

        [BsonElement("price_rozn")]
        public double PriceRozn { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        [BsonElement("weight")]
        public double Weight { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        [BsonElement("count")]
        public int Count { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [BsonElement("nd")]
        public double[] Nd { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        [BsonElement("length")]
        public double Length { get; set; }

        [BsonElement("1C")]
        public string Internal { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        [BsonElement("price_income")]
        public double PriceIcome { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        [BsonElement("is_sheet")]
        public bool IsSheet { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        [BsonElement("firma")]
        public string Firma { get; set; }
    }
}
