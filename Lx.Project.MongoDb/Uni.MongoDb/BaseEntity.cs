using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Uni.MongoDb
{
    public class BaseEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonPropertyName("id")]
        public string _id { get; set; }

        //public string UniId { get; set; } = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 15);
    }


    /// <summary>
    /// 时间特别处理
    /// </summary>
    public class SpecialHandleTimeParam
    {
        public string? Symbol { get; set; }

        public string? ColoumName { get; set; }

        public DateTime Value { get; set; }
    }

    /// <summary>
    /// 排序名称
    /// </summary>
    public class OrderByParam
    {
        public string? FieldName { get; set; }

        public bool IsAscending { get; set; }
    }
}
