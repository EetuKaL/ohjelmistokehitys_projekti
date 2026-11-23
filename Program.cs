// This app's architecture uses list to preserve the data of the users at the run time.
// Even tho Dictionaries would be more performance effective, the task's assignment was to use list, 
// So I decided to use Lists to store the data.
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;

List<List<object>> shifts = [
    [
        "Eetu",
        new List<List<string>> {
            new() { "12122026", "0800", "1400" },
            new() { "13122026", "0800", "1500" }
        }
    ]
];


void printAddShiftInstructions()
{
    const string instructions = """
    Lisää vuoro seuraavassa muodossa:
    etunimi Sukunimi ppMMvvvv HHmm HHmm
    
    esimerkki:
    Matti Meikäläinen 21122026 0826 1420
    ------------------------------------
    """;
    Console.WriteLine(instructions);
}

int getChoice()
{
    bool success = int.TryParse(Console.ReadLine(), out int selection);
    if (success && selection < 4)
    {
        return selection;
    }
    else
    {
        return 0;
    }
}

int tryFindUser(string name)
{

    // Try to find the entry by name 
    return shifts.FindIndex(e => (string)e[0] == name);
}

void addShift(string name, string date, string begin, string end)
{
    int i = tryFindUser(name);
    if (i != -1)
    {
        try
        {

            ((List<List<string>>)shifts[i][1]).Add([date, begin, end]);

            Console.WriteLine("Työvuoro lisättiin onnistuneesti järjestelmään.");

        }
        catch (System.Exception e)
        {
            Console.WriteLine($"Datan eheys ongelma: {e}");
        }

    }
    else
    {
        // Entry not found, maybe typo or the name was not yet registered.
        var decided = "";
        while (decided == "")
        {
            Console.WriteLine($"Käyttäjää '{name}' ei löytynyt tietokannasta.\nHaluatko Lisätä käyttäjän? (y/n)");
            var input = Console.ReadLine()?.ToLower();

            if (input == "y" || input == "n")
            {
                decided = input;
            }

        }

        if (decided == "y")
        {
            registerUser(name);
            addShift(name, date, begin, end);
        }
    }
}



void registerUser(string name)
{
    bool found = tryFindUser(name) != -1;
    if (found)
    {
        Console.WriteLine($"Käyttäjä '{name}' on jo rekisteröity järjestelmään.");
    }
    else
    {
        shifts.Add([name, new List<List<string>> { }]);
        Console.WriteLine($"'{name}' rekisteröitiin onnistuneesti järjestelmään.");
    }
}

bool calculateDuration(string start, string end, out TimeSpan duration)
{
    var s1 = TimeOnly.TryParse(start, out TimeOnly startTime);
    var s2 = TimeOnly.TryParse(end, out TimeOnly endTime);
    if (s1 && s2)
    {
        duration = endTime - startTime;
        return true;
    }
    else
    {
        duration = new TimeSpan();
        return false;
    }
}

string prettierDate(string date)
{
    return $"{date[..2]}.{date[2..4]}.{date[4..]}";
}

string prettierTime(string time)
{
    return $"{time[..2]}:{time[2..]}";
}

void printShifts(string name)
{
    var index = tryFindUser(name);
    if (index != -1)
    {
        if (shifts[index][1] is List<List<string>> _shifts)
        {
            var hours = TimeSpan.Zero;
            Console.WriteLine($"\n\n{name}|PVM       |ALKU |LOPPU|KESTO");
            for (var i = 0; i < _shifts.Count; i++)
            {
                var e = _shifts[i];
                var prettierStart = prettierTime(e[1]);
                var prettierEnd = prettierTime(e[2]);
                calculateDuration(prettierStart, prettierEnd, out TimeSpan dur);
                hours += dur;
                var row = $"{new String(' ', name.Length)}|{prettierDate(e[0])}|{prettierStart}|{prettierEnd}|{dur.ToString(@"hh\:mm")}";
                Console.WriteLine(row);
                if (i == _shifts.Count - 1)
                {
                    Console.WriteLine($"{new String(' ', row.Length - 5)}{hours.ToString(@"hh\:mm")}");
                    break;

                }

            }
        }
    }
    else
    {
        Console.WriteLine($"Käyttäjää '{name}' ei löytynyt tietokannasta.");
    }
}

string askName()
{
    Console.WriteLine("Anna käyttäjän nimi");
    return Console.ReadLine() ?? "";
}

bool parseShiftInput(string input, out List<string> parsed)
{
    parsed = [];
    // Expecting following format.
    // name: NameLastname
    // date: ppMMvvvv
    // start: HHmm
    // end: HHmm
    // Example input: EetuKallioniemi0202202608001400

    input = input.Replace(" ", "");

    // Because all names is different sizes, we need to start parsing backwards:
    try
    {
        var l = input.Length;

        // slice Range for end time.
        var e = l - 4;
        parsed.Insert(0, input[e..]);
        // slice range for start time
        var s = l - 8;
        parsed.Insert(0, input[s..e]);
        // slice range for date
        var d = l - 16;
        parsed.Insert(0, input[d..s]);
        // slice range for full name
        parsed.Insert(0, input[..d]);
        // We could validate here that the name is in the correct format. But let's leave it for now.
        return parsed.Count == 4;
    }
    catch (System.Exception)
    {
        return false;
    }
}



Console.WriteLine("Tervetuloa Shift Trackeriin!\n");
while (true)
{
    const string userGuide = """
    ---------------------------------------------
    Valitse jokin seuraavista (1-4):
    1: Lisää työvuoro
    2: Lisää käyttäjä
    3: Tulosta käyttäjän työvuorot

    Tai kirjoita mitä vain muuta sulkeaksesi ohjelman!
    ---------------------------------------------
    """;

    Console.WriteLine(userGuide);

    int choice = getChoice();
    string name;
    switch (choice)
    {
        case 1:
            bool success = false;
            while (!success)
            {
                Console.WriteLine("Syötä vuoro:");
                success = parseShiftInput(Console.ReadLine() ?? "", out List<string> parsed);
                if (success)
                {

                    addShift(parsed[0], parsed[1], parsed[2], parsed[3]);
                    Console.ReadLine();
                }
                else
                {
                    printAddShiftInstructions();
                }
            }
            break;
        case 2:
            name = askName();
            registerUser(name);
            Console.ReadLine();
            break;
        case 3:
            name = askName();
            printShifts(name);
            Console.ReadLine();
            break;
        default:
            Console.WriteLine("Sovellus suljetaan..");
            return;
    }
}