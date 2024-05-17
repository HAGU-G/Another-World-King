using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SaveLoadScriptable
{
    [MenuItem("���������̺�/�ҷ�����/���", priority = 0)]
    public static void LoadAll()
    {
        LoadEnemyCharacters(LoadPlayerCharacters(true));
        LoadStages();
    }
    [MenuItem("���������̺�/�����ϱ�/���", priority = 0)]
    public static void SaveAll()
    {
        SavePlayerCharacters();
        SaveEnemyCharacters();
        SaveSkills();
        SaveStages();
    }

    [MenuItem("���������̺�/�ҷ�����/�Ʊ� ĳ����, ��ų")]
    public static void LoadPlayerCharacters() => LoadPlayerCharacters(false);
    public static Dictionary<string, Skill_Csv> LoadPlayerCharacters(bool loadAll)
    {
        var skills = LoadSkills();
        var textAsset = Resources.Load<TextAsset>(Paths.resourcesCharTable);

        AssetDatabase.DeleteAsset(
            string.Concat(
                Paths.folderResources,
                string.Format(Paths.resourcesPlayer, string.Empty)
                ));
        AssetDatabase.Refresh();
        AssetDatabase.CreateFolder(Paths.folderScriptableObjects, "Player");
        AssetDatabase.Refresh();

        int count = 0;
        using (var reader = new StringReader(textAsset.text))
        using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csvReader.GetRecords<Player_Csv>();
            foreach (var record in records)
            {
                count++;
                if (record.Skill != Strings.zero
                    && skills.ContainsKey(record.Skill))
                {
                    record.Heal = skills[record.Skill].Hp_Healing;
                    record.EnemyCount = skills[record.Skill].Wide_Area_Range;
                }
                switch ((UnitData.DIVISION)record.Division)
                {
                    case UnitData.DIVISION.MELEE:
                        record.TypeCounter = "101";
                        break;
                    case UnitData.DIVISION.TANKER:
                        break;
                    case UnitData.DIVISION.MARKSMAN:
                        record.TypeCounter = "102";
                        break;
                    case UnitData.DIVISION.HEALER:
                        record.TypeCounter = "104";
                        break;
                    case UnitData.DIVISION.MAGIC:
                        record.TypeCounter = "103";
                        break;
                    case UnitData.DIVISION.SPECIAL:
                        record.TypeCounter = "105";
                        break;
                }

                record.ToScriptable(true);
            }
        }
        Debug.Log($"�Ʊ� ĳ���� {count}�� �ε� �Ϸ�.");
        return loadAll ? skills : null;
    }


    [MenuItem("���������̺�/�����ϱ�/�Ʊ� ĳ����")]
    public static void SavePlayerCharacters()
    {
        int count = 0;
        using (var writer = new StreamWriter(string.Concat(Paths.folderResources, Paths.resourcesCharTable, Paths._csv)))
        using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            var stats = Resources.LoadAll<UnitData>(string.Format(Paths.resourcesPlayer, string.Empty));

            csvWriter.WriteHeader<Player_Csv>();
            foreach (var record in stats)
            {
                count++;
                csvWriter.NextRecord();
                csvWriter.WriteRecord(new Player_Csv(record));
            }

        }
        AssetDatabase.Refresh();
        Debug.Log($"�Ʊ� ĳ���� {count}�� ���� �Ϸ�.");
    }

    [MenuItem("���������̺�/�ҷ�����/�� ĳ����, ��ų")]
    public static void LoadEnemyCharacters() => LoadEnemyCharacters(null);
    public static void LoadEnemyCharacters(Dictionary<string, Skill_Csv> skills = null)
    {
        if (skills == null)
            skills = LoadSkills();
        var textAsset = Resources.Load<TextAsset>(Paths.resourcesMonTable);

        AssetDatabase.DeleteAsset(
            string.Concat(
                Paths.folderResources,
                string.Format(Paths.resourcesEnemy, string.Empty)
                ));
        AssetDatabase.Refresh();
        AssetDatabase.CreateFolder(Paths.folderScriptableObjects, "Enemy");
        AssetDatabase.Refresh();

        int count = 0;
        using (var reader = new StringReader(textAsset.text))
        using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csvReader.GetRecords<Enemy_Csv>();
            foreach (var record in records)
            {
                count++;
                if (record.Skill != Strings.zero
                    && skills.ContainsKey(record.Skill))
                {
                    record.Heal = skills[record.Skill].Hp_Healing;
                    record.EnemyCount = skills[record.Skill].Wide_Area_Range;
                }
                switch ((UnitData.DIVISION)record.Division)
                {
                    case UnitData.DIVISION.MELEE:
                        record.TypeCounter = "101";
                        break;
                    case UnitData.DIVISION.TANKER:
                        break;
                    case UnitData.DIVISION.MARKSMAN:
                        record.TypeCounter = "102";
                        break;
                    case UnitData.DIVISION.HEALER:
                        record.TypeCounter = "104";
                        break;
                    case UnitData.DIVISION.MAGIC:
                        record.TypeCounter = "103";
                        break;
                    case UnitData.DIVISION.SPECIAL:
                        record.TypeCounter = "105";
                        break;
                }
                record.ToScriptable(false);
            }
        }
        Debug.Log($"�� ĳ���� {count}�� �ε� �Ϸ�.");
    }


    [MenuItem("���������̺�/�����ϱ�/�� ĳ����")]
    public static void SaveEnemyCharacters()
    {
        int count = 0;
        using (var writer = new StreamWriter(string.Concat(Paths.folderResources, Paths.resourcesMonTable, Paths._csv)))
        using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            var stats = Resources.LoadAll<UnitData>(string.Format(Paths.resourcesEnemy, string.Empty));

            csvWriter.WriteHeader<Enemy_Csv>();
            foreach (var record in stats)
            {
                count++;
                csvWriter.NextRecord();
                csvWriter.WriteRecord(new Enemy_Csv(record));
            }

        }
        AssetDatabase.Refresh();
        Debug.Log($"�� ĳ���� {count}�� ���� �Ϸ�.");
    }

    [MenuItem("���������̺�/�ҷ�����/��������")]
    public static void LoadStages()
    {
        var textAsset = Resources.Load<TextAsset>(Paths.resourcesStageTable);

        AssetDatabase.DeleteAsset(
            string.Concat(
                Paths.folderResources,
                string.Format(Paths.resourcesStage, string.Empty)
                ));
        AssetDatabase.Refresh();
        AssetDatabase.CreateFolder(Paths.folderScriptableObjects, "Stage");
        AssetDatabase.Refresh();

        int count = 0;
        using (var reader = new StringReader(textAsset.text))
        using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csvReader.GetRecords<Stage>();
            foreach (var record in records)
            {
                count++;
                record.ToScriptable();
            }
        }
        Debug.Log($"�������� {count}�� �ε� �Ϸ�.");
    }

    [MenuItem("���������̺�/�����ϱ�/��������")]
    public static void SaveStages()
    {
        int count = 0;
        using (var writer = new StreamWriter(string.Concat(Paths.folderResources, Paths.resourcesStageTable, Paths._csv)))
        using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            var stats = Resources.LoadAll<UnitData>(string.Format(Paths.resourcesStage, string.Empty));

            csvWriter.WriteHeader<Stage>();
            foreach (var record in stats)
            {
                count++;
                csvWriter.NextRecord();
                csvWriter.WriteRecord(new Stage(record));
            }

        }
        AssetDatabase.Refresh();
        Debug.Log($"�������� {count}�� ���� �Ϸ�.");
    }


    [MenuItem("���������̺�/�ҷ�����/��ų, ��")]
    private static Dictionary<string, Skill_Csv> LoadSkills()
    {
        var textAsset = Resources.Load<TextAsset>(Paths.resourcesSkillTable);

        AssetDatabase.DeleteAsset(
            string.Concat(
                Paths.folderResources,
                string.Format(Paths.resourcesSkill, string.Empty)
                ));
        AssetDatabase.Refresh();
        AssetDatabase.CreateFolder(Paths.folderScriptableObjects, "Skill");
        AssetDatabase.Refresh();

        //��ų
        int count = 0;
        var skills = new Dictionary<string, Skill_Csv>();
        using (var reader = new StringReader(textAsset.text))
        using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csvReader.GetRecords<Skill_Csv>();
            foreach (var record in records)
            {
                count++;
                if (!skills.ContainsKey(record.ID))
                    skills.Add(record.ID, record);
                record.ToScriptable();
            }
        }
        Debug.Log($"��ų {count}�� �ε� �Ϸ�.");

        //��
        textAsset = Resources.Load<TextAsset>(Paths.resourcesCounterTable);

        count = 0;
        using (var reader = new StringReader(textAsset.text))
        using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csvReader.GetRecords<Counter_Csv>();
            foreach (var record in records)
            {
                count++;
                record.ToScriptable();
            }
        }
        Debug.Log($"�� {count}�� �ε� �Ϸ�.");
        return skills;
    }

    [MenuItem("���������̺�/�����ϱ�/��ų, ��")]
    public static void SaveSkills()
    {
        int count = 0;
        var stats = Resources.LoadAll<SkillData>(string.Format(Paths.resourcesSkill, string.Empty));
        using (var writer = new StreamWriter(string.Concat(Paths.folderResources, Paths.resourcesSkillTable, Paths._csv)))
        using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csvWriter.WriteHeader<Skill_Csv>();
            foreach (var record in stats)
            {
                if (!record.isCounterData)
                {
                    count++;
                    csvWriter.NextRecord();
                    csvWriter.WriteRecord(new Skill_Csv(record));
                }
            }

        }
        Debug.Log($"��ų {count}�� ���� �Ϸ�.");

        count = 0;
        using (var writer = new StreamWriter(string.Concat(Paths.folderResources, Paths.resourcesCounterTable, Paths._csv)))
        using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csvWriter.WriteHeader<Counter_Csv>();
            foreach (var record in stats)
            {
                if (record.isCounterData)
                {
                    count++;
                    csvWriter.NextRecord();
                    csvWriter.WriteRecord(new Counter_Csv(record));
                }
            }

        }
        Debug.Log($"�� {count}�� ���� �Ϸ�.");
        AssetDatabase.Refresh();
    }
}
