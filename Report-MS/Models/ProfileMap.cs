// using statement for the CsvHelper.Configuration namespace

using CsvHelper.Configuration;

namespace Report_MS.Models;

// Sealed class called ProfileMap, inherited from ClassMap<Profile>
public sealed class ProfileMap : ClassMap<Profile>
{
    // Constructor for ProfileMap
    public ProfileMap()
    {
        // Map method is used to map the properties of Profile class to respective column names in the CSV file
        // The Name method is used to set the column name to a readable one to make it more user-friendly
        Map(m => m.Id).Name("ID");
        Map(m => m.FirstName).Name("First Name");
        Map(m => m.LastName).Name("Last Name");
        Map(m => m.Status).Name("Status");
    }
}