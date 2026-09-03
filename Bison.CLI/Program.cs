
using System;
using CsvHelper;
using System.IO;
using System.Globalization;
using System.ComponentModel.Design;
using Bison.Cheep;

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

static void read()
{
    // a reader that can read from the bison observe file
    using StreamReader reader = new StreamReader("bison_observe_cli_db.csv");
    
    // a CsvReader being made
    using CsvReader csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);


    // using a ClassMap to map "Observation" => "Message"
    csvReader.Context.RegisterClassMap<CheepMap>();

    // for each loop going through every line, and making them of type Cheep
     foreach (Cheep record in csvReader.GetRecords<Cheep>())
    {
        // Timestamp from the CSV file in unix time
        long unixTime = record.Timestamp;
        
        // Unix time converted to normal time
        DateTimeOffset date = DateTimeOffset.FromUnixTimeSeconds(unixTime);
        
        // Print the way specified in week 1 project part
        Console.WriteLine($"{record.Author} @ {date:MM'/'dd'/'yy HH':'mm':'ss}: {record.Message}");
    }
}

static void observe(string observation)
{
    // make a writer that writes whatever i want in the end of the bison observe file
    using StreamWriter writer = File.AppendText("bison_observe_cli_db.csv");

    // write a line containing the currently logged in users username, the text you run the program with and the current time in unix
    writer.WriteLine(Environment.UserName + ",\"" + observation + "\"," + DateTimeOffset.UtcNow.ToUnixTimeSeconds());
}
