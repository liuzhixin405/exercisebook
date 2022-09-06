using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MongoDB.BooksApi.Models
{
    public class BaseEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonPropertyName("id")]
        public string _id { get; set; }
    }

    /// <summary>
    /// 时间特别处理
    /// </summary>
    public class SpecialHandleTimeParam
    {
        public string Symbol { get; set; }

        public string ColoumName { get; set; }

        public DateTime Value { get; set; }
    }
}
