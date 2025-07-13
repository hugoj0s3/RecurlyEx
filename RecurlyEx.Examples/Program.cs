// See https://aka.ms/new-console-template for more information

Console.WriteLine("Hello, World!");


// Define a recurrence rule using natural language
var recurlyEx = RecurlyEx.RecurlyEx.Parse("@every 25 min @on friday @between 1:00pm and 03:00pm");

// Get the next 11 occurrences starting from Jan 1, 2020
var nextOccurrences = recurlyEx.TryGetNextOccurrencesInUtc(
    DateTime.Parse("2020-01-01 00:00:00"), 11
);

foreach (var nextOccurrence in nextOccurrences)
{
    Console.WriteLine(nextOccurrence.ToString("dddd, dd MMMM yyyy HH:mm:ss"));
}
