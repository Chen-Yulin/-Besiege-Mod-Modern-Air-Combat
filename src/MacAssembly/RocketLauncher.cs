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
        public float ThrustTime = 100f;
        public float ExploPower = 12000f;
        public float ExploRange = 2f;

        private float _drag;
        private Rigidbody _rigid;
        private GameObject _smoke;
        private GameObject _flame;

        
        private int _thrustTimeCount = 0;

        public void Explo(Vector3 pos)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<MeshRenderer>().enabled = false;

            GameObject ExploParticleEffect = (GameObject)Instantiate(AssetManager.Instance.AGMExplo.AGMExplo, transform.position, Quaternion.identity);
            ExploParticleEffect.SetActive(true);
            Destroy(ExploParticleEffect, 3);

        }

        public void DetectCollisionHost()
        {
            Ray RocketRay = new Ray(transform.position - _rigid.velocity.normalized * 2, transform.up);
            RaycastHit hit;
            if (Physics.Raycast(RocketRay, out hit, _rigid.velocity.magnitude / 100f))
            {
                if (hit.collider.isTrigger)
                {
                    return;
                }
                Explo(hit.point);
            }
        }

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
                if (!_rigid)
                {
                    _rigid = GetComponent<Rigidbody>();
                }

                if (_thrustTimeCount < ThrustTime)
                {
                    _thrustTimeCount++;
                    _rigid.AddForce(transform.up * Thrust);
                }
                else
                {
                    if (!_smoke)
                    {
                        _smoke = transform.GetChild(0).gameObject;
                        _smoke.GetComponent<ParticleSystem>().Stop();
                        _flame = transform.GetChild(1).gameObject;
                        _flame.GetComponent<ParticleSystem>().Stop();
                    }
                }
                // modified gravity
                _rigid.AddForce(Vector3.down * 0.15f);
                if (StatMaster.isClient)
                {

                }
                else
                {
                    if (_thrustTimeCount > 5)
                    {
                        DetectCollisionHost();
                    }
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

        public int currRocketIndex = 0;
        public int TimeStep = 0;

        public int currTime = 0;

        private bool isLaunching = false;
        private int _maxRocketNum = 16;

        
        

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

        public void Launch()
        {
            if (currRocketIndex < _maxRocketNum)
            {
                Rockets[currRocketIndex].transform.SetParent(GetComponent<BlockBehaviour>().ParentMachine.gameObject.transform);
                Rigidbody rocketRigid = Rockets[currRocketIndex].AddComponent<Rigidbody>();
                rocketRigid.mass = 0.1f;
                rocketRigid.drag = 0.05f;
                Rigidbody.useGravity = false;
                Rockets[currRocketIndex].GetComponent<Rocket>().Launch();

                GameObject TrailSmoke = Instantiate(AssetManager.Instance.Trail.RocketTail);
                GameObject TrailFlame = Instantiate(AssetManager.Instance.Trail.FlameTrail);

                TrailSmoke.transform.SetParent(Rockets[currRocketIndex].transform);
                TrailSmoke.transform.localPosition = Vector3.zero;

                TrailFlame.transform.SetParent(Rockets[currRocketIndex].transform);
                TrailFlame.transform.localPosition = Vector3.zero;
                TrailFlame.transform.localRotation = Quaternion.Euler(90, 0, 0);

                
                TrailSmoke.SetActive(true);
                TrailFlame.SetActive(true);
                TrailSmoke.GetComponent<ParticleSystem>().Play();
                TrailFlame.GetComponent<ParticleSystem>().Play();

                Destroy(Rockets[currRocketIndex], 10f);

                currRocketIndex++;
            }
        }

        public override void SafeAwake()
        {
            Fire = AddKey("Fire", "FireRocket", KeyCode.C);
            Thrust = AddSlider("Rocket Thrust", "Rocket Thrust", 500, 200, 1500);
            Caliber = AddSlider("Rocket Caliber", "Rocket Caliber", 122, 70, 200);
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
                Rockets[i].transform.localRotation = Quaternion.identity;
                Rocket rocket = Rockets[i].AddComponent<Rocket>();
                rocket.Thrust = Thrust.Value;
                rocket.ExploPower = Caliber.Value * 100;
                rocket.ExploRange = Caliber.Value / 30;

                MeshFilter rocketMF = Rockets[i].AddComponent<MeshFilter>();
                MeshRenderer rockeyMR = Rockets[i].AddComponent<MeshRenderer>();

                rocketMF.sharedMesh = ModResource.GetMesh("Rocket Mesh");
                rockeyMR.material.mainTexture = ModResource.GetTexture("Rocket Texture");
               

            }

            // set some fixed value
            TimeStep = (int)(100f / FireRate.Value);
        }
        public override void OnSimulateStop()
        {
            for (int i = 0; i < _maxRocketNum; i++)
            {
                if (Rockets[i])
                {
                    Destroy(Rockets[i]);
                }
            }
        }

        public override void SimulateUpdateHost()
        {
            if (Fire.IsHeld)
            {
                isLaunching = true;
            }
            else
            {
                isLaunching = false;
                currTime = int.MaxValue;
            }

        }

        public override void SimulateFixedUpdateHost()
        {
            if (isLaunching)
            {
                if (currTime < TimeStep)
                {
                    currTime++;
                }
                else
                {
                    currTime = 0;
                    Launch();
                }
            }
        }


    }
}
