using System.Collections.Generic;
using UnityEngine;

public class UIWindowManager : MonoBehaviour
{
    public List<UIWindow> windows;
    public int defaulIndex = 0;

    private void Awake()
    {
        for (int i = 0; i < windows.Count; i++)
        {
            if (i == defaulIndex)
                windows[i].Open();
            else
                windows[i].Close();
        }
    }
}
