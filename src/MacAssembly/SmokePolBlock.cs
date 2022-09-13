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
    class SmokePolBlock : BlockScript
    {
        public MKey PullSmoke;
        public MSlider SmokeLifeTime;
        public MSlider SmokeSize;
        public MColourSlider SmokeColor;
        public GameObject Smoke;
        public ParticleSystem SmokeParticle;

        public int myPlayerID;
        public int myGuid;

        public void InitSmoke()
        {
            Smoke = Instantiate(AssetManager.Instance.PerformSmoke.PerformSmoke);
            Smoke.transform.SetParent(transform);
            Smoke.transform.localPosition = new Vector3(0, 4, 0.3f);
            Smoke.transform.localRotation = Quaternion.Euler(90, 0, 0);
            Smoke.transform.localScale = Vector3.one;
            Smoke.SetActive(false);
        }

        public void InitParticleSystem()
        {
            SmokeParticle = Smoke.GetComponent<ParticleSystem>();
        }

        public override void SafeAwake()
        {
            myPlayerID = BlockBehaviour.ParentMachine.PlayerID;
            PullSmoke = AddKey("Pull Smoke", "Pull Smoke", KeyCode.B);
            SmokeLifeTime = AddSlider("Smoke Life Time", "Smoke Life Time", 1.5f, 1f, 5f);
            SmokeSize = AddSlider("Smoke Size", "Smoke Size", 2f, 1f, 10f);
            SmokeColor = AddColourSlider("Smoke Color", "Smoke Color", Color.red, false);
            InitSmoke();
        }

        public void Start()
        {
            InitParticleSystem();
            Smoke.SetActive(true);
            SmokeParticle.startColor = SmokeColor.Value;
            SmokeParticle.startLifetime = SmokeLifeTime.Value;
            SmokeParticle.startSize = SmokeSize.Value;
        }

        public override void OnSimulateStart()
        {
            myGuid = BlockBehaviour.BuildingBlock.Guid.GetHashCode();
            try
            {
                if (StatMaster.isClient)
                {
                    KeymsgController.Instance.keyheld[myPlayerID].Add(myGuid, false);
                }
            }
            catch { }

        }

        public override void OnSimulateStop()
        {
            KeymsgController.Instance.keyheld[myPlayerID].Remove(myGuid);
            try
            {
                Destroy(Smoke);
            }
            catch { }
        }

        public void Update()
        {


        }

        public override void SimulateUpdateHost()
        {
            if (PullSmoke.IsPressed)
            {
                
                if (!SmokeParticle.isPlaying)
                {
                    SmokeParticle.Play();
                    ModNetworking.SendToAll(KeymsgController.SendHeld.CreateMessage(myPlayerID, myGuid, true));
                }
                else
                {
                    SmokeParticle.Stop();
                    ModNetworking.SendToAll(KeymsgController.SendHeld.CreateMessage(myPlayerID, myGuid, false));
                }
            }
        }
        public override void SimulateUpdateClient()
        {
            if (KeymsgController.Instance.keyheld[myPlayerID][myGuid])
            {
                if (!SmokeParticle.isPlaying)
                {
                    SmokeParticle.Play();
                }
            }
            else
            {
                if (SmokeParticle.isPlaying)
                {
                    SmokeParticle.Stop();
                }
            }
        }
    }
}
