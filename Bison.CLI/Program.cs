// if i run it with read
using System;
using CsvHelper;
using System.IO;
using System.Globalization;

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
    
    // making a variable that can either be a string or null
    string? line;

    // read the first line without printing to avoid printing the column names
    reader.ReadLine();

    // while there is stuff to print, do it
    while((line = reader.ReadLine()) != null)
    {
        // put each column value in an array called words
        string[] words = line.Split(",");

        // get the unix time in a long value
        long unixTime = Convert.ToInt64(words[2]);

        // get a normal date time
        DateTimeOffset date = DateTimeOffset.FromUnixTimeSeconds(unixTime);

        // print the author, the date and time formatted as MM/dd/yy HH:mm:ss and then the text
        Console.WriteLine($"{words[0]} @ {date:MM'/'dd'/'yy HH':'mm':'ss}: {words[1]}");
    }  
}

static void observe(string observation)
{
    // make a writer that writes whatever i want in the end of the bison observe file
    using StreamWriter writer = File.AppendText("bison_observe_cli_db.csv");

    // write a line containing the currently logged in users username, the text you run the program with and the current time in unix
    writer.WriteLine(Environment.UserName + ",\"" + observation + "\"," + DateTimeOffset.UtcNow.ToUnixTimeSeconds());
}
