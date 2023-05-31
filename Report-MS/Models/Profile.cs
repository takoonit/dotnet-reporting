using System;
using Redis.OM.Modeling;
using System.Net;

namespace Report_MS.Models
{
    [Document(StorageType = StorageType.Json, Prefixes = new[] { "profiles" })]
    public class Profile
    {
        public Profile()
        {
            this.FirstName = "unknown";
            this.LastName = "unknown";
        }

        [RedisIdField][Indexed] public string? Id { get; set; }

        [Indexed] public string FirstName { get; set; }

        [Indexed] public string LastName { get; set; }

        [Indexed] public int Age { get; set; }

        [Searchable] public string? Status { get; set; }

        [Indexed] public string[] Skills { get; set; } = Array.Empty<string>();
    }

}

