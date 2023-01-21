using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Collections;

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

    public class RandomFlareForce : MonoBehaviour
    {
        Rigidbody myRigid;
        float randomness = 3;
        Vector3 Force = Vector3.zero;
        public void Start()
        {
            myRigid = transform.gameObject.GetComponent<Rigidbody>();
        }
        public void FixedUpdate()
        {
            Force = -Force + new Vector3(randomness * (UnityEngine.Random.value - 0.5f),
                                        randomness * (UnityEngine.Random.value - 0.5f),
                                        randomness * (UnityEngine.Random.value - 0.5f));
            myRigid.AddRelativeForce(Force);
        }
    }


    public class FlareBlock : BlockScript
    {
        public MMenu launchedType;
        public MKey ReleaseKey;
        public MSlider LaunchNum;
        public MSlider ReleaseInterval;
        public MSlider GroupSize;

        public GameObject FlareObject;
        public GameObject FlareFlame;
        public GameObject FlareSmoke;
        public List<GameObject> FlareAssembly = new List<GameObject>();
        public List<GameObject> ChaffAssembly = new List<GameObject>();

        public GameObject Chaff;

        public static MessageType LaunchPara = ModNetworking.CreateMessageType(DataType.Integer, DataType.Integer, DataType.Vector3);
        public int HostGuid;
        public Vector3 HostVelocity;

        protected float time = 0f;
        protected int myGuid;
        protected int myPlayerID;

        public void InitChaff()
        {
            if (!GameObject.Find("chaff"))
            {
                Chaff = (GameObject)Instantiate(AssetManager.Instance.Chaff.Chaff);
                Chaff.name = "chaff";
                BoxCollider chaffCol = Chaff.AddComponent<BoxCollider>();
                chaffCol.size = new Vector3(0.1f, 0.1f, 0.1f);
                Chaff.SetActive(false);
            }
        }

        public void InitFlare()
        {
            FlareObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            FlareObject.name = "flare";
            Destroy(FlareObject.GetComponent<MeshFilter>());
            FlareObject.GetComponent<BoxCollider>().size = new Vector3(0.1f,0.1f,0.1f);
            FlareObject.AddComponent<RandomFlareForce>();

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
            if (launchedType.Value == 0)
            {
                if (FlareAssembly.Count >= LaunchNum.Value)
                {
                    return;
                }
                else
                {
                    FlareAssembly.Add((GameObject)Instantiate(FlareObject, transform.position + 2 * transform.forward, transform.rotation));
                    GameObject tmpFlare = FlareAssembly[FlareAssembly.Count - 1];
                    tmpFlare.name = "flare";
                    tmpFlare.SetActive(true);
                    Rigidbody tmpRig = tmpFlare.GetComponent<Rigidbody>();
                    tmpRig.velocity = velocity;
                    tmpRig.AddRelativeForce(new Vector3(5 * UnityEngine.Random.value - 2.5f, 5 * UnityEngine.Random.value - 2.5f, 30 + 10 * UnityEngine.Random.value));
                    tmpFlare.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().Play();
                    tmpFlare.transform.GetChild(1).gameObject.GetComponent<ParticleSystem>().Play();
                    Destroy(tmpFlare, 5);
                }
            }
            else
            {
                if (ChaffAssembly.Count >= LaunchNum.Value)
                {
                    return;
                }
                else
                {
                    ChaffAssembly.Add((GameObject)Instantiate(Chaff, transform.position + 5 * transform.forward, transform.rotation));
                    GameObject tmpChaff = ChaffAssembly[ChaffAssembly.Count - 1];
                    tmpChaff.name = "chaff";
                    tmpChaff.SetActive(true);
                    Destroy(tmpChaff, 5);
                }
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
            launchedType = AddMenu("Type", 0, new List<string>
            {
                "Flare",
                "Chaff"
            }, false);
            ReleaseKey = AddKey("Launch", "Launch Flare", KeyCode.C);
            LaunchNum = AddSlider("Launch Num", "LaunchNum", 8f, 0f, 256f);
            ReleaseInterval = AddSlider("Release Interval", "release interval", 0.2f, 0.05f, 0.5f);
            GroupSize = AddSlider("Group Size", "Group Size", 1f, 1f, 8f);
            InitFlare();
            InitChaff();

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
            if (launchedType.Value == 0)
            {
                LoadDataManager.Instance.AddFlareNum(myPlayerID, (int)LaunchNum.Value - FlareAssembly.Count);
            }
            else
            {
                LoadDataManager.Instance.AddChaffNum(myPlayerID, (int)LaunchNum.Value - ChaffAssembly.Count);
            }

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
                    for (int i = 0; i < GroupSize.Value; i++)
                    {
                        Release(BlockBehaviour.Rigidbody.velocity);
                    }

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
            if (launchedType.Value == 0)
            {
                LoadDataManager.Instance.AddFlareNum(myPlayerID, (int)LaunchNum.Value - FlareAssembly.Count);
            }
            else
            {
                LoadDataManager.Instance.AddChaffNum(myPlayerID, (int)LaunchNum.Value - ChaffAssembly.Count);
            }
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
                        for (int i = 0; i < GroupSize.Value; i++)
                        {
                            Release(HostVelocity);
                        }
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
            for (int i = 0; i < ChaffAssembly.Count; i++)
            {
                if (ChaffAssembly[i])
                {
                    Destroy(ChaffAssembly[i]);
                }
            }
            Destroy(Chaff);
            Destroy(FlareObject);

            KeymsgController.Instance.keyheld[myPlayerID].Remove(myGuid);
        }

        void OnGUI()
        {
            //GUI.Box(new Rect(100, 100, 200, 200), myGuid.ToString());
        }
    }
}
