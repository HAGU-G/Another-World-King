using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt(Defines.playerfrabsTutorial, 0) == 0)
        {

        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}