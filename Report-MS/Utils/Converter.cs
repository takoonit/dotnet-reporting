using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace Report_MS.Utils;

public static class Converter
{
    public static string ListToCsv<T, TMap>(IList<T> records) where TMap : ClassMap<T>
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
            {
                using (var csvWriter = new CsvWriter(streamWriter, new CsvConfiguration(CultureInfo.InvariantCulture)))
                {
                    csvWriter.Context.RegisterClassMap<TMap>();
                    csvWriter.WriteRecords(records);
                    streamWriter.Flush();
                    return Encoding.UTF8.GetString(memoryStream.ToArray());
                }
            }
        }
    }

    public static string ListToCsv<T>(IList<T> records)
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
            {
                using (var csvWriter = new CsvWriter(streamWriter, new CsvConfiguration(CultureInfo.InvariantCulture)))
                {
                    csvWriter.WriteRecords(records);
                    streamWriter.Flush();
                    return Encoding.UTF8.GetString(memoryStream.ToArray());
                }
            }
        }
    }
}