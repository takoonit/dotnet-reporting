using Redis.OM.Modeling;
using Report_MS.Common;

namespace Report_MS.Models;

[Document(StorageType = StorageType.Json, Prefixes = new[] { "profiles" })]
public class Profile
{
    public Profile()
    {
        FirstName = "unknown";
        LastName = "unknown";
        Status = ProfileStatus.Active;
    }

    [RedisIdField] [Indexed] public string? Id { get; set; }

    [Indexed] public string FirstName { get; set; }

    [Indexed] public string LastName { get; set; }

    [Searchable] public string Status { get; set; }
}