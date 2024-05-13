using CsvHelper.Configuration.Attributes;
using System.Collections.Generic;
using UnityEngine;

public class Pattern
{
    public string ID { get; set; }
    public int Monster_1 { get; set; }
    public int Monster_2 { get; set; }
    public int Monster_3 { get; set; }
}

public class PatternSet
{
    public string pattern;
    public float waitingTime;
    public int weight;
}

public class MonsterAppare
{
    public int ID { get; set; }
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
        for (int i = 0; i < weightMarks.Count; i++)
        {
            if (i == 0)
            {
                if (randomWeight < weightMarks[i])
                    return PatternSets[i];
            }
            else
            {
                if (randomWeight >= weightMarks[i - 1] && randomWeight < weightMarks[i])
                    return PatternSets[i];
            }
        }
        return null;
    }
}