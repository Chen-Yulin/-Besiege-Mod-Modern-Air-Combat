using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections;
using System.Numerics;

using Modding.Modules;
using Modding;
using Modding.Blocks;
using UnityEngine;

namespace ModernAirCombat
{
    class FlareBlock : BlockScript
    {
        public MKey ReleaseKey;
        public MSlider ReleaseInterval;

        //public GameObject FlareAssembly;
        public GameObject[] FlareObject;
        public GameObject[] FlareFlame;
        public GameObject[] FlareSmoke;

        private ParticleSystem[] FlareFlameParticle;
        private ParticleSystem[] FlareSmokeParticle;
        private BoxCollider[] FlareCollider;
        private float time = 0f;
        private int nextFlare = 0;

        public void InitFlare()
        {
            if (true)
            {
                //FlareAssembly = new GameObject("FlareAssembly");
                //FlareAssembly.transform.SetParent(transform);
                //FlareAssembly.transform.localPosition = new Vector3(0, 0, 1f);
                //FlareAssembly.transform.localRotation = Quaternion.Euler(0, 0, 0);
                //FlareAssembly.transform.localScale = new Vector3(1, 1, 1);
                FlareObject = new GameObject[8];
                FlareFlame = new GameObject[8];
                FlareSmoke = new GameObject[8];
                FlareCollider = new BoxCollider[8];
                FlareFlameParticle = new ParticleSystem[8];
                FlareSmokeParticle = new ParticleSystem[8];
                for (int i = 0; i < 8; i++)
                {
                    FlareObject[i] = new GameObject("flare");
                    FlareObject[i].transform.SetParent(BlockBehaviour.transform);
                    FlareObject[i].transform.localPosition = new Vector3(0, 0, 1);
                    FlareObject[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
                    FlareObject[i].transform.localScale = Vector3.one;
                    FlareCollider[i] = FlareObject[i].AddComponent<BoxCollider>();
                    FlareCollider[i].name = "flareCol";
                    FlareCollider[i].size = 0.01f * Vector3.one;
                    FlareCollider[i].center = Vector3.zero;
                    FlareCollider[i].enabled = false;
                    

                    FlareFlame[i] = Instantiate(AssetManager.Instance.Flare.FlameFlare);
                    FlareSmoke[i] = Instantiate(AssetManager.Instance.Flare.SmokeFlare);

                    FlareFlame[i].transform.SetParent(FlareObject[i].transform);
                    FlareFlame[i].transform.localPosition = Vector3.zero;
                    FlareFlame[i].transform.localRotation = Quaternion.Euler(0,0,0);
                    FlareFlame[i].transform.localScale = Vector3.one;

                    FlareSmoke[i].transform.SetParent(FlareObject[i].transform);
                    FlareSmoke[i].transform.localPosition = Vector3.zero;
                    FlareSmoke[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
                    FlareSmoke[i].transform.localScale = Vector3.one;

                    FlareFlameParticle[i] = FlareFlame[i].GetComponent<ParticleSystem>();
                    FlareSmokeParticle[i] = FlareSmoke[i].GetComponent<ParticleSystem>();

                    FlareFlame[i].SetActive(true);
                    FlareSmoke[i].SetActive(true);
                }
                //FlareAssembly.SetActive(true);
            }
            
        }

        public void Release()
        {
            if (nextFlare >= 8)
                return;

            FlareObject[nextFlare].transform.SetParent(transform.parent.transform);
            FlareObject[nextFlare].SetActive(true);
            FlareCollider[nextFlare].enabled = true;
            Rigidbody rig = FlareObject[nextFlare].AddComponent<Rigidbody>();
            rig.velocity = BlockBehaviour.Rigidbody.velocity;
            rig.mass = 0.01f;
            rig.drag = 0.5f;
            rig.AddRelativeForce(new Vector3(5 * UnityEngine.Random.value-2.5f, 5*UnityEngine.Random.value-2.5f, 30+10*UnityEngine.Random.value));
            FlareFlameParticle[nextFlare].Play();
            FlareSmokeParticle[nextFlare].Play();
            Destroy(FlareObject[nextFlare],5);
            nextFlare++;
        }

        public override void SafeAwake()
        {
            ReleaseKey = AddKey("Launch", "Launch FLare", KeyCode.C);
            ReleaseInterval = AddSlider("Release Interval", "release interval", 0.2f, 0.05f, 0.5f);
            InitFlare();

        }
        public void Start()
        {

        }
        
        private void Update()
        {
            try
            {
                if (ReleaseKey.IsPressed)
                {
                    time = 0f;
                    Release();
                }
                if (ReleaseKey.IsHeld)
                {
                    time += Time.deltaTime;
                    if (time > ReleaseInterval.Value)
                    {
                        Release();
                        time = 0f;
                    }
                }
            }
            catch { }
            
        }
    }
}
