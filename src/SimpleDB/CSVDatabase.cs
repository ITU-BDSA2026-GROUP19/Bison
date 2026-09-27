
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace SimpleDB;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    private readonly string filePath;
    private readonly ClassMap<T>? map;

// Constructor for the CSVDatabase class that takes a file path as a parameter
    private CSVDatabase(String filePath, ClassMap<T>? map = null)
    {
        this.filePath = filePath;
        this.map = map;
        /*if (File.Exists(filePath))
        {
            using StreamReader reader = new StreamReader(filePath);
            using CsvReader csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
        }*/
    } 

// Singleton instance of the CSVDatabase class
    private static CSVDatabase<T>? instance;

    // Checks if singleton instance exists, if not creates a new instance and returns it
    public static CSVDatabase<T> GetInstance(string filePath, ClassMap<T>? map = null)
    {
        if (instance == null)
        {
            instance = new CSVDatabase<T>(filePath, map);
        }
        return instance;
    }


    // Implementation of the Read method from the IDatabaseRepository interface
	public IEnumerable<T> Read(int? limit = null)
    {
        if (!File.Exists(filePath))
        {
            return [];
        }
        // Makes a StreamReader and a CsvReader to read the CSV file
        using StreamReader reader = new StreamReader(filePath);
        using CsvReader csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

        if (map != null)
        {
            csvReader.Context.RegisterClassMap(map);
        }

        var records = csvReader.GetRecords<T>();

        if (limit.HasValue)
        {
            return records.Take(limit.Value);
        }
        else
        {
            return records.ToList();
        }
    }
    // Implementation of the Store method from the IDatabaseRepository interface
    public void Store(T record)
    {
        
        bool writeHeader = !File.Exists(filePath) || new FileInfo(filePath).Length == 0;

        // Makes a StreamWriter and a CsvWriter to write to the CSV file
        using StreamWriter writer = new StreamWriter(filePath, append: true);
        using CsvWriter csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);

        if (map != null)
        {
            csvWriter.Context.RegisterClassMap(map);
        }

        if (writeHeader)
        {
            csvWriter.WriteHeader<T>();
            csvWriter.NextRecord();
        }

        csvWriter.WriteRecord(record);
        csvWriter.NextRecord();
    }
}