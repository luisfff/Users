using System;

namespace Users.Application.Projections
{
    public record UserDetails
        {
            public string Id { get; init; }
            public string DisplayName { get; init; }
            public string FullName { get; set; }
            public static string GetDatabaseId(long id) => $"UserDetails/{id}";
        }
}