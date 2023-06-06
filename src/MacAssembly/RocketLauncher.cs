using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Collections;

using Modding.Modules;
using Modding;
using Modding.Blocks;
using UnityEngine;


namespace ModernAirCombat
{
    public class Rocket : MonoBehaviour
    {
        public float Thrust = 1000f;
        public bool MissileOn = false;
        public float ThrustTime = 3f;
        public float ExploPower = 12000f;
        public float ExploRange = 2f;

        private float _drag;

        
        private int _thrustTimeCount = 0;
        

        public void Launch()
        {
            MissileOn = true;
            
        }

        public void Start()
        {
            
        }

        public void FixedUpdate()
        {
            if (MissileOn)
            {
                if (_thrustTimeCount < ThrustTime)
                {
                    _thrustTimeCount++;
                    GetComponent<Rigidbody>().AddForce(transform.forward * Thrust);
                }
            }
        }

    }
    public class RocketLauncher : BlockScript
    {
        public MKey Fire;
        public MSlider Thrust;
        public MSlider Caliber;
        public MSlider FireRate;

        public GameObject[] Rockets = new GameObject[16];

        /// <summary>
        /// calculate the position of the rocket in the launcher
        /// </summary>
        /// <param name="index"> index for the rocket in the same layer </param>
        /// <param name="layer"> 0 for inner layer, 1 for outter layer </param>
        /// <returns></returns>
        public Vector2 CalculateLoadPosition(int index, int layer)
        {
            Vector2 result = new Vector2();
            float deltaAngle = 0;
            float radius = 0.1f;
            if (layer == 0)
            {
                deltaAngle = 72;
                radius = 0.1f;
            }else if (layer == 1) {
                deltaAngle = 32.73f;
                radius = 0.22f;
                index -= 5;
            }
            // for index 0 and 5, the pivot of each layer
            result = new Vector2(radius * Mathf.Cos(index * deltaAngle * Mathf.PI / 180f), radius * Mathf.Sin(index * deltaAngle * Mathf.PI / 180f));
            return result;
        }

        public override void SafeAwake()
        {
            Fire = AddKey("Fire", "FireRocket", KeyCode.C);
            Thrust = AddSlider("Rocket Thrust", "Rocket Thrust", 1000, 800, 2000);
            Caliber = AddSlider("Rocket Thrust", "Rocket Thrust", 122, 70, 200);
            FireRate = AddSlider("Fire Rate", "Fire Rate", 10, 1, 20);

        }
        public override void OnSimulateStart()
        {
            GameObject RocketSet = new GameObject("RocketSet");
            RocketSet.transform.SetParent(transform);
            RocketSet.transform.localPosition = new Vector3(0,0,0.3f);
            RocketSet.transform.localRotation = Quaternion.identity;
            RocketSet.transform.localScale = Vector3.one;
            

            for (int i = 0; i < 16; i++)
            {
                Rockets[i] = new GameObject("Rockets");
                Rockets[i].transform.SetParent(RocketSet.transform);

                Vector2 rocketPos = new Vector2();
                if (i<5)// for inner 5 rockets
                {
                    rocketPos = CalculateLoadPosition(i, 0);
                }
                else//for outer 11 rockets
                {
                    rocketPos = CalculateLoadPosition(i, 1);
                }
                Rockets[i].transform.localPosition = new Vector3(rocketPos.x,0,rocketPos.y);  
            }
        }

    }
}
