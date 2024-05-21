using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public static class DataTableManager
{
    public static Dictionary<string, Pattern> Patterns { get; private set; } = new();
    public static Dictionary<int, MonsterAppare> MonsterAppares { get; private set; } = new();
    public static Dictionary<int, List<int>> StageUnlockID { get; private set; } = new();
    public static Dictionary<int, Upgrade> Upgrades { get; private set; } = new();

    private static Dictionary<string, string> stringTable = new();
    public static Dictionary<int, Stage> Stages { get; private set; } = new();
    public static int MinStageID { get; private set; } = int.MaxValue;
    public static int MaxStageID { get; private set; } = int.MinValue;

    static DataTableManager()
    {
        //Patterns
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

        //MonsterAppares
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
                ma.ID = csvReader.GetField<int>(1);
                ma.Division = csvReader.GetField<int>(2);
                for (int i = 0; i < patternCount; i++)
                {
                    var pattern = csvReader.GetField<string>(startPatternIndex + 0 + i * 3);
                    if (pattern == Defines.zero)
                        continue;

                    var ps = new PatternSet();
                    ps.pattern = pattern;
                    ps.waitingTime = csvReader.GetField<float>(startPatternIndex + 1 + i * 3);
                    ps.weight = csvReader.GetField<int>(startPatternIndex + 2 + i * 3);
                    ma.PatternSets.Add(ps);
                }
                ma.Setting();
                MonsterAppares.Add(ma.ID, ma);
            }
        }

        //Stages
        var asset = Resources.LoadAll<UnitData>(string.Format(Paths.resourcesStage, string.Empty));
        foreach (var stage in asset)
        {
            MaxStageID = stage.id > MaxStageID ? stage.id : MaxStageID;
            MinStageID = stage.id < MinStageID ? stage.id : MinStageID;
        }
        MinStageID++;

        //StageUnlockID
        textAsset = Resources.Load<TextAsset>(Paths.resourcesStageTable);
        using (var reader = new StringReader(textAsset.text))
        using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csvReader.GetRecords<Stage>();
            foreach (var record in records)
            {
                List<int> ids = new()
                {
                    record.Reward_Char1,
                    record.Reward_Char2,
                    record.Reward_Char3,
                    record.Reward_Char4
                };
                Stages.Add(record.ID, record);
                StageUnlockID.Add(record.ID, ids);
            }
        }

        //Upgrade
        textAsset = Resources.Load<TextAsset>(Paths.resourcesUpgradeTable);
        using (var reader = new StringReader(textAsset.text))
        using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csvReader.GetRecords<Upgrade>();
            foreach (var record in records)
            {
                Upgrades.Add(record.ID, record);
            }
        }

        //StringTable
        textAsset = Resources.Load<TextAsset>(Paths.resourcesStringTable);
        using (var reader = new StringReader(textAsset.text))
        using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var desc = new { List = string.Empty, String_ID = string.Empty, String_Info = string.Empty };
            var records = csvReader.GetRecords(desc);
            foreach (var record in records)
            {
                stringTable.Add(record.String_ID, record.String_Info);
            }
        }
    }


    public static string GetString(string id)
    {
        if (stringTable.ContainsKey(id))
            return stringTable[id];
        else
            return string.Empty;
    }
}