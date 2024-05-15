using CsvHelper;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SaveLoadScriptable
{
    [MenuItem("데이터테이블/불러오기/모두", priority = 0)]
    public static void LoadAll()
    {
        LoadPlayerCharacters();
        LoadEnemyCharacters();
        LoadStages();
    }
    [MenuItem("데이터테이블/저장/모두", priority = 0)]
    public static void SaveAll()
    {
        SavePlayerCharacters();
        SaveEnemyCharacters();
    }


    [MenuItem("데이터테이블/불러오기/아군 캐릭터")]
    public static void LoadPlayerCharacters()
    {
        var skills = LoadSkills();
        var textAsset = Resources.Load<TextAsset>(Paths.resourcesCharTable);

        using (var reader = new StringReader(textAsset.text))
        using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
        {

            var records = csvReader.GetRecords<Player_Csv>();
            foreach (var record in records)
            {
                if (record.Skill != Strings.zero
                    && skills.ContainsKey(record.Skill))
                {
                    record.Heal = skills[record.Skill].Hp_Healing;
                    record.EnemyCount = skills[record.Skill].Wide_Area_Range;
                }
                record.ToScriptable(true);
            }
        }
    }


    [MenuItem("데이터테이블/저장/아군 캐릭터")]
    public static void SavePlayerCharacters()
    {
        using (var writer = new StreamWriter(string.Concat(Paths.folderResources, Paths.resourcesCharTable, Paths._csv)))
        using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            var stats = Resources.LoadAll<UnitData>(string.Format(Paths.resourcesPlayer, string.Empty));

            csvWriter.WriteHeader<Player_Csv>();
            foreach (var record in stats)
            {
                csvWriter.NextRecord();
                csvWriter.WriteRecord(new Player_Csv(record));
            }

        }
        AssetDatabase.Refresh();
    }

    [MenuItem("데이터테이블/불러오기/적 캐릭터")]
    public static void LoadEnemyCharacters()
    {
        var skills = LoadSkills();
        var textAsset = Resources.Load<TextAsset>(Paths.resourcesMonTable);

        using (var reader = new StringReader(textAsset.text))
        using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csvReader.GetRecords<Enemy_Csv>();
            foreach (var record in records)
            {
                if (record.Skill != Strings.zero
                    && skills.ContainsKey(record.Skill))
                {
                    record.Heal = skills[record.Skill].Hp_Healing;
                    record.EnemyCount = skills[record.Skill].Wide_Area_Range;
                }
                record.ToScriptable(false);
            }
        }
    }


    [MenuItem("데이터테이블/저장/적 캐릭터")]
    public static void SaveEnemyCharacters()
    {
        using (var writer = new StreamWriter(string.Concat(Paths.folderResources, Paths.resourcesMonTable, Paths._csv)))
        using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            var stats = Resources.LoadAll<UnitData>(string.Format(Paths.resourcesEnemy, string.Empty));

            csvWriter.WriteHeader<Enemy_Csv>();
            foreach (var record in stats)
            {
                csvWriter.NextRecord();
                csvWriter.WriteRecord(new Enemy_Csv(record));
            }

        }
        AssetDatabase.Refresh();
    }

    [MenuItem("데이터테이블/불러오기/스테이지")]
    public static void LoadStages()
    {
        var textAsset = Resources.Load<TextAsset>(Paths.resourcesStageTable);

        using (var reader = new StringReader(textAsset.text))
        using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csvReader.GetRecords<Stage>();
            foreach (var record in records)
            {
                record.ToScriptable();
            }
        }
    }

    private static Dictionary<string, Skill_Csv> LoadSkills()
    {
        var textAsset = Resources.Load<TextAsset>(Paths.resourcesSkillTable);

        using (var reader = new StringReader(textAsset.text))
        using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csvReader.GetRecords<Skill_Csv>();
            var skills = new Dictionary<string, Skill_Csv>();
            foreach (var record in records)
            {
                if (!skills.ContainsKey(record.ID))
                    skills.Add(record.ID, record);
                record.ToScriptable();
            }
            return skills;
        }
    }
}
