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

    [Indexed(Sortable = true)] public string FirstName { get; set; }

    [Indexed(Sortable = true)] public string LastName { get; set; }

    [Indexed(Sortable = true)] public string Status { get; set; }
}