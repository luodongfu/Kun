﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergeController : MonoBehaviour {
    public int energe = 100;
    public int minEnerge = 35;
    public float accelerateWaitTime = 2;
    int maxEnerge;
    int lastEnerge;
    float accelerateTimer = 0;

	// Use this for initialization
	void Start () {
        lastEnerge = energe;
        maxEnerge = energe;
	}
	
	// Update is called once per frame
	void Update () {
        if(accelerateTimer > 0)
        {
            accelerateTimer -= Time.deltaTime;
        }

        if(lastEnerge != energe)
        {
            lastEnerge = energe;
            Color color = GetComponent<SpriteRenderer>().color;
            //GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, 1.0f * energe / maxEnerge);
            //GetComponentInChildren<SpriteRenderer>().color = new Color(color.r, color.g, color.b, 1.0f * energe / maxEnerge);

            SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
            foreach(SpriteRenderer sprite in sprites)
            {
                sprite.color = new Color(color.r, color.g, color.b, 1.0f * energe / maxEnerge);
            }
        }
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Memory")
        {
            GetComponent<Animation>().Play();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Memory")
        {
            GetComponent<Animation>().Stop();
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Memory" && energe < maxEnerge)
        {
            energe += other.GetComponent<MemoryController>().Exploit();
        }
    }

    public int getEnerge(int demand)
    {
        if(energe - minEnerge > demand)
        {
            energe -= demand;
            return demand;
        }
        else
        {
            int getvalue = energe - minEnerge;
            energe = minEnerge;
            return getvalue;
        }
    }

    public bool canAccelerate()
    {
        if(energe >= minEnerge && accelerateTimer <= 0)
        {
            accelerateTimer = accelerateWaitTime;
            return true;
        }
        else
        {
            return false;
        }
    }
}