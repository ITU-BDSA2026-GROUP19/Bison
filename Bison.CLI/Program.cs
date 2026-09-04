using CsvHelper;
using System.Globalization;
using Bison.Cheep;
using SimpleDB;

CSVDatabase<Cheep> database = new CSVDatabase<Cheep>("bison_observe_cli_db.csv");

// if i run it with read
if (args[0] == "read")
{
   read();
}
// if i run the program with observe
else if (args[0] == "observe")
{   
    observe(args[1]);
}

void read()
{
    // for each loop going through every line, and making them of type Cheep
     foreach (Cheep record in database.Read())
    {
        // Timestamp from the CSV file in unix time
        long unixTime = record.Timestamp;
        
        // Unix time converted to normal time
        DateTimeOffset date = DateTimeOffset.FromUnixTimeSeconds(unixTime);
        
        // Print the way specified in week 1 project part
        Console.WriteLine($"{record.Author} @ {date:MM'/'dd'/'yy HH':'mm':'ss}: {record.Message}");
    }
}

void observe(string observation)
{
    
    // Make a Cheep object that matches the Command-line input
    Cheep cheep = new Cheep(Environment.UserName, observation, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
    database.Store(cheep);
}
