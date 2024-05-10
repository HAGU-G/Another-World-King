using CsvHelper;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;

public class DataTable
{
    [MenuItem("데이터테이블/불러오기/모두", priority = 0)]
    public static void LoadAll()
    {
        LoadPlayerCharacters();
    }
    [MenuItem("데이터테이블/저장/모두", priority = 0)]
    public static void SaveAll()
    {
        SavePlayerCharacters();
    }


    [MenuItem("데이터테이블/불러오기/아군 캐릭터")]
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


    [MenuItem("데이터테이블/저장/아군 캐릭터")]
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
