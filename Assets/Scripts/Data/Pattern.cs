using CsvHelper.Configuration.Attributes;
using System.Collections.Generic;
using UnityEngine;

public class Pattern
{
    public string ID { get; set; }
    public string Monster_1 { get; set; }
    public string Monster_2 { get; set; }
    public string Monster_3 { get; set; }
}

public class PatternSet
{
    public string pattern;
    public float waitingTime;
    public int weight;
}

public class MonsterAppare
{
    public string ID { get; set; }
    public int Division { get; set; }
    [Ignore] public List<PatternSet> PatternSets { get; set; } = new();
    public int weightSum;
    private List<int> weightMarks = new();

    public void Setting()
    {
        foreach (var pattern in PatternSets)
        {
            weightSum += pattern.weight;
            weightMarks.Add(weightSum);
        }
    }

    public PatternSet GetPattern()
    {
        int randomWeight = Random.Range(0, weightSum);
        foreach (var i in weightMarks)
        {
            if (randomWeight < i)
                return PatternSets[i];
        }
        return null;
    }
}