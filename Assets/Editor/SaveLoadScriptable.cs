using CsvHelper;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SaveLoadScriptable
{
    [MenuItem("���������̺�/�ҷ�����/���", priority = 0)]
    public static void LoadAll()
    {
        LoadPlayerCharacters();
    }
    [MenuItem("���������̺�/����/���", priority = 0)]
    public static void SaveAll()
    {
        SavePlayerCharacters();
    }


    [MenuItem("���������̺�/�ҷ�����/�Ʊ� ĳ����")]
    public static void LoadPlayerCharacters()
    {
        var textAsset = Resources.Load<TextAsset>(Paths.resourcesCharTable);

        using (var reader = new StringReader(textAsset.text))
        using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
        {

            var records = csvReader.GetRecords<Player_Csv>();
            foreach (var record in records)
            {
                record.ToScriptable(true);
            }
        }
    }


    [MenuItem("���������̺�/����/�Ʊ� ĳ����")]
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

    [MenuItem("���������̺�/�ҷ�����/�� ĳ����")]
    public static void LoadEnemyCharacters()
    {
        var textAsset = Resources.Load<TextAsset>(Paths.resourcesMonTable);

        using (var reader = new StringReader(textAsset.text))
        using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csvReader.GetRecords<Enemy_Csv>();
            foreach (var record in records)
            {
                record.ToScriptable(false);
            }
        }
    }


    [MenuItem("���������̺�/����/�� ĳ����")]
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
}
