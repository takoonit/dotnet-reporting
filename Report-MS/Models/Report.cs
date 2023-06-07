using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Report_MS.Models;

public class Report
{
    public Report(string reportData)
    {
        Id = ObjectId.GenerateNewId().ToString();
        Timestamp = DateTime.UtcNow;
        Data = reportData;
    }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime Timestamp { get; set; }

    public object Data { get; set; }
}