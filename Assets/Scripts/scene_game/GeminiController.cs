﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeminiController : MonoBehaviour {
    public GameObject kun;
    public GameObject energe;
    public float[] boundary;    // 边界
    public float boundaryRange = 1.5f;      // 边界阻力开始范围
    public float boundaryForce = 0.5f;      // 边界阻力
    public float coefficient;   // 加速度系数
    public float gravitation;   // 向左恒定力
    public float threshold;     // threshold小于min，在阀值之内不会受到任何力的影响，在阀值之外均会受到恒定力
    public float min, max;     // 在min, max范围之内距离变远会受到推力
    public float maxVelocity;   // 最大速度
    public float transVelocity = 0.1f;     // 粒子转移速度

    ParticleSystem particleSys;
    ParticleSystem.Particle[] particles;
    KunController kunController;
    float lastDistance, distance;
    float accelerate = 1;       // 突破速度上限加速
    float k = 1;
    float breakTimer = 0;
    
    void Awake()
    {
        particleSys = energe.GetComponentInChildren<ParticleSystem>();
        if(particleSys)
        {
            particles = new ParticleSystem.Particle[particleSys.main.maxParticles];
            ParticleSystem.MainModule main = particleSys.main;
            main.simulationSpace = ParticleSystemSimulationSpace.Custom;
            main.customSimulationSpace = transform;
        }
    }

    void particleTrans()
    {
        if(particleSys)
        {
            int count = particleSys.GetParticles(particles);
            for(int i = 0; i < count; i++)
            {
                particles[i].position = Vector3.Lerp(particles[i].position, kun.transform.position, transVelocity);
                particleSys.SetParticles(particles, count);
            }
        }
    }

	// Use this for initialization
	void Start () {
        lastDistance = (kun.transform.position - energe.transform.position).magnitude;
        distance = lastDistance;
        kunController = kun.GetComponent<KunController>();
	}
	
	// Update is called once per frame

    void LimitVelocity(GameObject obj, float maxVelocity)
    {
        float velocity = obj.GetComponent<Rigidbody2D>().velocity.magnitude;
        if(velocity > maxVelocity)
        {
            obj.GetComponent<Rigidbody2D>().velocity = obj.GetComponent<Rigidbody2D>().velocity * maxVelocity / velocity;
        }
    }

    void LimitGameObject(GameObject obj)
    {
        float x = obj.transform.position.x;
        float y = obj.transform.position.y;
        float z = obj.transform.position.z;
        bool limit = false;

        if(obj.transform.position.y > boundary[0])
        {
            y = boundary[0];
            limit = true;
        }

        if(obj.transform.position.y < boundary[1])
        {
            y = boundary[1];
            limit = true;
        }

        if(obj.transform.position.x < boundary[2])
        {
            x = boundary[2];
            limit = true;
        }

        if(obj.transform.position.x > boundary[3])
        {
            x = boundary[3];
            limit = true;
        }

        if(limit)
        {
            obj.transform.position = new Vector3(x, y, z);
        }
    }

    void AddRepulsion(GameObject obj, int mode, float boundary, float range, float force)
    {
        if(mode == 0)       // x
        {
            if(obj.transform.position.x > boundary - range && obj.transform.position.x < boundary + range)
            {
                float dis = Mathf.Abs(boundary - obj.transform.position.x);
                int forward = (obj.transform.position.x - boundary) >= 0 ? 1 : -1;  // 斥力方向
                int objForward = obj.GetComponent<Rigidbody2D>().velocity.x >= 0 ? 1 : -1;

                if(objForward != forward)
                {
                    if (dis < range / 4)
                    {
                        force *= 4;
                    }
                    else if (dis < range / 2)
                    {
                        force *= 2;
                    }

                    obj.GetComponent<Rigidbody2D>().AddForce(new Vector2(forward * force, 0), ForceMode2D.Impulse);
                }

            }
        }
        else if(mode == 1)
        {
            if (obj.transform.position.y > boundary - range && obj.transform.position.y < boundary + range)
            {
                float dis = Mathf.Abs(boundary - obj.transform.position.y);
                int forward = (obj.transform.position.y - boundary) >= 0 ? 1 : -1;
                int objForward = obj.GetComponent<Rigidbody2D>().velocity.y >= 0 ? 1 : -1;

                if(objForward != forward)
                {
                    if (dis < range / 4)
                    {
                        force *= 4;
                    }
                    else if (dis < range / 2)
                    {
                        force *= 2;
                    }

                    obj.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, forward * force), ForceMode2D.Impulse);
                }

            }
        }

    }

    

	void Update () {
        distance = (kun.transform.position - energe.transform.position).magnitude;
        k = 1;

        // 边界
        AddRepulsion(kun, 0, boundary[2], boundaryRange, boundaryForce);
        AddRepulsion(kun, 0, boundary[3], boundaryRange, boundaryForce);
        AddRepulsion(kun, 1, boundary[0], boundaryRange, boundaryForce);
        AddRepulsion(kun, 1, boundary[1], boundaryRange, boundaryForce);
        AddRepulsion(energe, 0, boundary[2], boundaryRange, boundaryForce);
        AddRepulsion(energe, 0, boundary[3], boundaryRange, boundaryForce);
        AddRepulsion(energe, 1, boundary[0], boundaryRange, boundaryForce);
        AddRepulsion(energe, 1, boundary[1], boundaryRange, boundaryForce);
        LimitGameObject(kun);
        LimitGameObject(energe);

        // 向左恒定力
        if(distance > min + (max - min) / 4)
        {
            kun.GetComponent<Rigidbody2D>().AddForce(new Vector2(-gravitation, 0) * k * 4, ForceMode2D.Impulse);
            energe.GetComponent<Rigidbody2D>().AddForce(new Vector2(-gravitation, 0) * k * 4, ForceMode2D.Impulse);
        }
        else if (distance > min)
        {
            kun.GetComponent<Rigidbody2D>().AddForce(new Vector2(-gravitation, 0) * k * 2, ForceMode2D.Impulse);
            energe.GetComponent<Rigidbody2D>().AddForce(new Vector2(-gravitation, 0) * k * 2, ForceMode2D.Impulse);
        }
        else if(distance > threshold)
        {
            kun.GetComponent<Rigidbody2D>().AddForce(new Vector2(-gravitation, 0) * k, ForceMode2D.Impulse);
            energe.GetComponent<Rigidbody2D>().AddForce(new Vector2(-gravitation, 0) * k, ForceMode2D.Impulse);
        }

        int forwardKun = kunController.moveState;
        int forwardEnerge = (kun.transform.position.x - energe.transform.position.x) >= 0 ? 1 : -1;

        // 双子推力
        if(lastDistance > min && lastDistance < max)
        {
            if(distance > lastDistance)
            {
                float force = Mathf.Abs(distance - lastDistance) / Time.deltaTime * coefficient;
                kun.GetComponent<Rigidbody2D>().AddForce(new Vector2(forwardKun * force, 0) * k, ForceMode2D.Impulse);
                energe.GetComponent<Rigidbody2D>().AddForce(new Vector2(forwardEnerge * force, 0) * k, ForceMode2D.Impulse);
            }
        }
        else
        {
            if (distance < lastDistance)
            {
                float force = Mathf.Abs(distance - lastDistance) / Time.deltaTime * coefficient;
                kun.GetComponent<Rigidbody2D>().AddForce(new Vector2(forwardKun * force, 0) * k, ForceMode2D.Impulse);
                energe.GetComponent<Rigidbody2D>().AddForce(new Vector2(forwardEnerge * force, 0) * k, ForceMode2D.Impulse);
            }
        }

        // 双子引力
        if(distance > min + (max - min) / 2)
        {
            energe.GetComponent<Rigidbody2D>().AddForce((kun.transform.position - energe.transform.position).normalized * 1.5f * k, ForceMode2D.Impulse);
        }
        else if (distance > min + (max - min) / 4)
        {
            energe.GetComponent<Rigidbody2D>().AddForce((kun.transform.position - energe.transform.position).normalized * 1 * k, ForceMode2D.Impulse);
        }
        else if(distance > min)
        {
            energe.GetComponent<Rigidbody2D>().AddForce((kun.transform.position - energe.transform.position).normalized * 0.5f * k, ForceMode2D.Impulse);
        }

        
        if(distance < threshold)
        {
            // 借助能量球突破加速
            if (energe.GetComponent<EnergeController>().canAccelerate())
            {
                breakTimer = 2;     // 设置突破时间
                kun.GetComponent<Rigidbody2D>().AddForce(2 * new Vector2(forwardKun * gravitation, 0), ForceMode2D.Impulse);
                accelerate = 4;     // 突破倍数
            }
            else if(distance < threshold / 2)
            {
                breakTimer = 2;
                kun.GetComponent<Rigidbody2D>().AddForce(1.5f * new Vector2(forwardKun * gravitation, 0), ForceMode2D.Impulse);
                accelerate = 4;
            }

            // 吸收能量
            if(kun.GetComponent<KunController>().energe > 0 )
            {
                kun.GetComponent<KunController>().Absorb(energe);
                particleTrans();
            }
        }

        
        // 速度上限
        if(breakTimer > 0)
        {
            breakTimer -= Time.deltaTime;
        }
        else
        {
            if(accelerate >= 4)
            {
                accelerate /= 2;
                breakTimer = 1;
            }
            else
            {
                accelerate = 1;
            }
        }

        LimitVelocity(kun, maxVelocity * accelerate);
        LimitVelocity(energe, maxVelocity);

        lastDistance = distance;
	}
}