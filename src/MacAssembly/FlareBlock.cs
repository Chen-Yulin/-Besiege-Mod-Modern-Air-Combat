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
using UnityEngine.Networking;

namespace ModernAirCombat
{
    class FlareMessageReciver : SingleInstance<FlareMessageReciver>
    {
        public override string Name { get; } = "FlareMessageReciver";

        public Dictionary<int, Dictionary<int, Vector3>> FlareLaunchData = new Dictionary<int, Dictionary<int, Vector3>>();

        public void ReceiveMsg(Message msg)
        {
            int guid_msg = (int)msg.GetData(0);
            int playerid_msg = (int)msg.GetData(1);
            Vector3 velocity_msg = (Vector3)msg.GetData(2);

            if (FlareLaunchData.ContainsKey(guid_msg))
            {
                if (FlareLaunchData[guid_msg].ContainsKey(playerid_msg))
                {
                    FlareLaunchData[guid_msg][playerid_msg] = velocity_msg;
                }
                else
                {
                    FlareLaunchData[guid_msg].Add(playerid_msg, velocity_msg);
                }
                
                //Debug.Log("update" + ((int)msg.GetData(0)).ToString() + " " + ((Vector3)msg.GetData(1)).ToString());
            }
            else
            {
                Dictionary<int, Vector3> tmpDic = new Dictionary<int, Vector3>
                {
                    { playerid_msg, velocity_msg }
                };
                FlareLaunchData.Add(guid_msg, tmpDic);
                
            }
 
        }

        public Vector3 GetLaunchPara(int guid, int playerID)
        {
            if (FlareLaunchData.ContainsKey(guid))
            {
                if (FlareLaunchData[guid].ContainsKey(playerID))
                {
                    return FlareLaunchData[guid][playerID];
                }
                else
                {
                    return Vector3.zero;
                }
                
            }
            else
            {
                return Vector3.zero;
            }
            
        }


    }

    public class FlareBlock : BlockScript
    {
        public MKey ReleaseKey;
        public MSlider ReleaseInterval;

        //public GameObject FlareAssembly;
        public GameObject FlareObject;
        public GameObject FlareFlame;
        public GameObject FlareSmoke;
        public List<GameObject> FlareAssembly = new List<GameObject>();

        public static MessageType LaunchPara = ModNetworking.CreateMessageType(DataType.Integer, DataType.Integer, DataType.Vector3);
        public int HostGuid;
        public Vector3 HostVelocity;

        protected float time = 0f;
        protected int myGuid;
        protected int myPlayerID;

        public void InitFlare()
        {
            try
            {
                Destroy(FlareObject);
            }
            catch { }
            FlareObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(FlareObject.GetComponent<MeshFilter>());
            FlareObject.GetComponent<BoxCollider>().size = new Vector3(0.1f,0.1f,0.1f);

            FlareFlame = (GameObject)Instantiate(AssetManager.Instance.Flare.FlameFlare, FlareObject.transform);
            FlareSmoke = (GameObject)Instantiate(AssetManager.Instance.Flare.SmokeFlare, FlareObject.transform);

            FlareFlame.transform.localPosition = Vector3.zero;
            FlareFlame.transform.localRotation = Quaternion.Euler(0,0,0);
            FlareFlame.transform.localScale = Vector3.one;

            FlareSmoke.transform.localPosition = Vector3.zero;
            FlareSmoke.transform.localRotation = Quaternion.Euler(0, 0, 0);
            FlareSmoke.transform.localScale = Vector3.one;

            FlareFlame.SetActive(true);
            FlareSmoke.SetActive(true);

            Rigidbody rig = FlareObject.AddComponent<Rigidbody>();
            rig.mass = 0.01f;
            rig.drag = 0.5f;

            FlareObject.SetActive(false);
        }
            
        public void Release(Vector3 velocity)
        {
            if (FlareAssembly.Count >=8)
            {
                return;
            }
            else
            {
                FlareAssembly.Add((GameObject)Instantiate(FlareObject, transform.position + 2*transform.forward, transform.rotation));
                GameObject tmpFlare = FlareAssembly[FlareAssembly.Count - 1];
                tmpFlare.SetActive(true);
                Rigidbody tmpRig = tmpFlare.GetComponent<Rigidbody>();
                tmpRig.velocity = velocity;
                tmpRig.AddRelativeForce(new Vector3(5 * UnityEngine.Random.value - 2.5f, 5 * UnityEngine.Random.value - 2.5f, 30 + 10 * UnityEngine.Random.value));
                tmpFlare.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().Play();
                tmpFlare.transform.GetChild(1).gameObject.GetComponent<ParticleSystem>().Play();
                Destroy(tmpFlare, 5);
            }
        }


        //public override void BuildingUpdate()
        //{
        //    if (BlockBehaviour.Guid.GetHashCode() != 0 && BlockBehaviour.Guid.GetHashCode() != myGuid)
        //        myGuid = BlockBehaviour.Guid.GetHashCode();
        //}


        public override void SafeAwake()
        {
            myPlayerID = BlockBehaviour.ParentMachine.PlayerID;
            ReleaseKey = AddKey("Launch", "Launch Flare", KeyCode.C);
            ReleaseInterval = AddSlider("Release Interval", "release interval", 0.2f, 0.05f, 0.5f);
            InitFlare();

        }
        public override void OnSimulateStart()
        {
            myGuid = BlockBehaviour.BuildingBlock.Guid.GetHashCode();

            if (!StatMaster.isClient)
            {
                KeymsgController.Instance.keyheld[myPlayerID].Add(myGuid, false);
            }
           
        }

        public override void SimulateUpdateHost()
        {
            if (BlockBehaviour.BuildingBlock.Guid.GetHashCode() != 0 && BlockBehaviour.BuildingBlock.Guid.GetHashCode() != myGuid)
                myGuid = BlockBehaviour.BuildingBlock.Guid.GetHashCode();

            if (StatMaster.isMP)
            {
                Message HostLaunchPara = LaunchPara.CreateMessage(myGuid, (int)BlockBehaviour.ParentMachine.PlayerID, BlockBehaviour.Rigidbody.velocity);
                ModNetworking.SendToAll(HostLaunchPara);

                ModNetworking.SendToAll(KeymsgController.SendHeld.CreateMessage((int)myPlayerID, (int)myGuid, ReleaseKey.IsHeld));
            }

            if (ReleaseKey.IsHeld)
            {
                time += Time.deltaTime;
                if (time > ReleaseInterval.Value)
                {
                    Release(BlockBehaviour.Rigidbody.velocity);
                    time = 0f;
                }
            }
            else
            {
                time = ReleaseInterval.Value;
            }
            
        }

        public override void SimulateUpdateClient()
        {
            if (BlockBehaviour.BuildingBlock.Guid.GetHashCode() != 0 && BlockBehaviour.BuildingBlock.Guid.GetHashCode() != myGuid)
                myGuid = BlockBehaviour.BuildingBlock.Guid.GetHashCode();

            HostVelocity = FlareMessageReciver.Instance.GetLaunchPara(myGuid,myPlayerID);
            try
            {
                if (KeymsgController.Instance.keyheld[myPlayerID][myGuid])
                {
                    time += Time.deltaTime;
                    if (time > ReleaseInterval.Value)
                    {
                        Release(HostVelocity);
                        time = 0f;
                    }
                }
                else
                {
                    time = ReleaseInterval.Value;
                }
            }
            catch { }

        }

        public override void OnSimulateStop()
        {
            for (int i = 0; i < FlareAssembly.Count; i++)
            {
                if (FlareAssembly[i])
                {
                    Destroy(FlareAssembly[i]);
                }
            }
            KeymsgController.Instance.keyheld[myPlayerID].Remove(myGuid);
        }

        void OnGUI()
        {
            //GUI.Box(new Rect(100, 100, 200, 200), myGuid.ToString());
        }
    }
}
