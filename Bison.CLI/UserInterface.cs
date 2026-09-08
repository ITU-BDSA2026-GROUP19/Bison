using System;
using System.Collections.Generic;
using Bison.Cheep;


public static class UserInterface
{
    //no instance here, just a bunch of behavior executing, hence the static
    
// all printing-to-terminal logic now lives here, not in Program.cs
    public static void PrintCheeps(IEnumerable<Cheep> cheeps) //for any type lists of cheeps
    {
        foreach (Cheep record in cheeps)
        {
            // Timestamp from the CSV file in unix time
            long unixTime = record.Timestamp;
 
            // Unix time converted to normal time
            DateTimeOffset date = DateTimeOffset.FromUnixTimeSeconds(unixTime);
 
            
            Console.WriteLine($"{record.Author} @ {date:MM'/'dd'/'yy HH':'mm':'ss}: {record.Message}");
        }
    }


}

