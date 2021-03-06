﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOperate : MonoBehaviour {
    static List<Pauser> pausers;
    static bool ObjectPause = false;

    public void Awake()
    {
        pausers = new List<Pauser>();
        ObjectPause = false;
    }

	public void Return()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("levelChoose");
    }

    public void PauseTime()
    {
        if(Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }
    }

    public void PauseObjects()
    {
        if(!ObjectPause)
        {
            for (int i = pausers.Count - 1; i >= 0; i--)
            {
                if (pausers[i] != null)
                {
                    pausers[i].Pause();
                }
                else
                {
                    pausers.Remove(pausers[i]);
                }
            }

            ObjectPause = true;
        }
    }

    public void ContinueObjects()
    {
        if(ObjectPause)
        {
            for (int i = pausers.Count - 1; i >= 0; i--)
            {
                if (pausers[i] != null)
                {
                    pausers[i].Continue();
                }
                else
                {
                    pausers.Remove(pausers[i]);
                }
            }

            ObjectPause = false;
        }
    }


    public void ReStart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PauserRegister(Pauser pauser)
    {
        pausers.Add(pauser);
    }
}
