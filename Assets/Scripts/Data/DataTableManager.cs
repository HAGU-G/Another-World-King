using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public static class DataTableManager
{
    public static Dictionary<string, Pattern> Patterns { get; private set; } = new();
    public static Dictionary<string, MonsterAppare> MonsterAppares { get; private set; } = new();

    static DataTableManager()
    {
        //Pattern
        var textAsset = Resources.Load<TextAsset>(Paths.resourcesPatternTable);
        using (var reader = new StringReader(textAsset.text))
        using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csvReader.GetRecords<Pattern>();
            foreach (var record in records)
            {
                Patterns.Add(record.ID, record);
            }
        }

        //Appare
        textAsset = Resources.Load<TextAsset>(Paths.resourcesMonAppareTable);
        using (var reader = new StringReader(textAsset.text))
        using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csvReader.Read();
            var columnCount = csvReader.ColumnCount;
            int startPatternIndex = 3;
            int patternCount = (int)Mathf.Ceil((columnCount - startPatternIndex) / 3f);
            while (csvReader.Read())
            {
                var ma = new MonsterAppare();
                ma.ID = csvReader.GetField<string>(1);
                ma.Division = csvReader.GetField<int>(2);
                for (int i = 0; i < patternCount; i++)
                {
                    var ps = new PatternSet();
                    ps.pattern = csvReader.GetField<string>(startPatternIndex + 0 + i * 3);
                    ps.waitingTime = csvReader.GetField<float>(startPatternIndex + 1 + i * 3);
                    ps.weight = csvReader.GetField<int>(startPatternIndex + 2 + i * 3);
                    ma.PatternSets.Add(ps);
                }
                ma.Setting();
                MonsterAppares.Add(ma.ID, ma);
            }
            
        }
    }
}