using CsvHelper;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;

public class DataTable
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
        var textAsset = Resources.Load<TextAsset>(Paths.dataTablePlayer);

        using (var reader = new StringReader(textAsset.text))
        {
            using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
            {

                var records = csvReader.GetRecords<InitStats_Csv>();
                foreach (var record in records)
                {
                    record.ToScriptable();
                }
            }
        }
    }


    [MenuItem("���������̺�/����/�Ʊ� ĳ����")]
    public static void SavePlayerCharacters()
    {
        using (var writer = new StreamWriter("Assets/Resources/DataTables/Player.csv"))
        using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            var stats = Resources.LoadAll<InitStats>(string.Format(Paths.resourcesPlayer, string.Empty));

            csvWriter.WriteHeader<InitStats_Csv>();
            foreach (var record in stats)
            {
                csvWriter.NextRecord();
                csvWriter.WriteRecord(new InitStats_Csv(record));
            }

        }
        AssetDatabase.Refresh();
    }
}
