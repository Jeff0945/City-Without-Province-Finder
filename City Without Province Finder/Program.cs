using Microsoft.VisualBasic.FileIO;

namespace City_Without_Province_Finder;

class Program
{
    const string FilePath = "../PSGC-1Q-2024.csv";
    
    const int NameCol = 1;
    const int CodeCol = 2;
    const int LevelCol = 3;

    static string[][] GetCsvLines()
    {
        var csvLines = new List<string[]>();

        var parser = new TextFieldParser(FilePath);
        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters(",");
        parser.HasFieldsEnclosedInQuotes = true;

        while (!parser.EndOfData)
        {
            string[]? fields = parser.ReadFields();

            if (fields == null)
            {
                continue;
            }
            
            csvLines.Add(fields);
        }

        return csvLines.ToArray();
    }

    static bool IsProvince(string level)
    {
        return level switch
        {
            "Dist" => true,
            "Prov" => true,
            "" => true,
            _ => false
        };
    }

    static string[][] GetProvinces(string[][] csvLines)
    {
        return csvLines.Where(line => IsProvince(line[LevelCol])).ToArray();
    }

    static string[][] GetCities(string[][] csvLines)
    {
        return csvLines.Where(line => IsCity(line[LevelCol])).ToArray();
    }

    static bool HasProvince(string[][] provinces, string[] city)
    {
        foreach (string[] province in provinces)
        {
            string code = province[CodeCol];

            if (!IsValidCode(code))
            {
                continue;
            }

            string provinceCode = code[..4];

            if (city[CodeCol][..4] == provinceCode)
            {
                return true;
            }
        }

        return false;
    }

    static bool IsValidCode(string code)
    {
        return code.Length != 0;
    }

    static bool IsCity(string level)
    {
        return level switch
        {
            "City" => true,
            "SubMun" => true,
            "Mun" => true,
            _ => false
        };
    }
    
    static void Main(string[] args)
    {
        string[][] csvLines = GetCsvLines();
        string[][] provinces = GetProvinces(csvLines);
        string[][] cities = GetCities(csvLines);
        
        var noOrphanedCity = true;

        foreach (string[] city in cities)
        {
            if (!HasProvince(provinces, city))
            {
                Console.WriteLine($"No province: {city[NameCol]}");
                noOrphanedCity = false;
            }
        }

        if (noOrphanedCity)
        {
            Console.WriteLine("No cities orphaned!");
        }
    }
}