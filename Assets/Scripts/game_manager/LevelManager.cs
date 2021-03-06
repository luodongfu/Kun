﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
    public static string[] LevelNames = {"level1", "level2", "level3", "level4", "level5", "CG_56", "level6", "CG_通关"};
    public static int maxlevel
    {
        get
        {
            if(_maxlevel == -1)
            {
                Init();
            }

            return _maxlevel;
        }
        set
        {
            _maxlevel = value;
        }
    }
    static bool develop = false;
    static int _maxlevel = -1;

    public static void UpdateMaxLevel(int newMax)
    {
        maxlevel = newMax;
        PlayerPrefs.SetInt("maxlevel", newMax);
    }

    static void Init()
    {
        if (develop)
        {
            PlayerPrefs.DeleteAll();
        }

        if (!PlayerPrefs.HasKey("maxlevel"))
        {
            PlayerPrefs.SetInt("maxlevel", 0);
        }

        maxlevel = PlayerPrefs.GetInt("maxlevel", 0);
        //maxlevel = 6;
    }
 

    public void FirstGame()
    {
        int time = PlayerPrefs.GetInt("FirstGame", 0);

        if(time < 2)
        {
            PlayerPrefs.SetInt("FirstGame", time + 1);
            SceneManager.LoadScene("新手引导");
        }
        else
        {
            Enter(0);
        }
    }

    public static void EnterMax()
    {
        Enter(maxlevel);
    }

    public static void NewGame()
    {
        PlayerPrefs.SetInt("maxlevel", 0);
        Enter(0);
    }

    public static void Enter(int index)
    {
        if(index >= LevelNames.Length)
        {
            return;
        }

        SceneManager.LoadScene(LevelNames[index]);
    }

}
