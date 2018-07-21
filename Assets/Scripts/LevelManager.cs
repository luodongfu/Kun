﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
    public string[] LevelNames = {"level1", "level2", "level3", "level4", "level5"};
    public static int maxlevel = 0;
    public bool develop = false;
 
    void Start()
    {
        if(develop)
        {
            PlayerPrefs.DeleteAll();
        }

        if(!PlayerPrefs.HasKey("maxlevel"))
        {
            PlayerPrefs.SetInt("maxlevel", 0);
        }

        maxlevel = PlayerPrefs.GetInt("maxlevel", 0);
    }

    public void EnterMax()
    {
        Enter(maxlevel);
    }

    public void Enter(int index)
    {
        SceneManager.LoadScene(LevelNames[index]);
    }

}
