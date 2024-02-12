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
    class RadarMsgReceiver : SingleInstance<RadarMsgReceiver>
    {
        public override string Name { get; } = "RadarMsgReceiver";

        public Vector3[] ScanColLocalRotation = new Vector3[16];
        
        public void RadarHeadMsgReceiver(Message msg)
        {
            ScanColLocalRotation[(int)msg.GetData(0)] = (Vector3)msg.GetData(1);
        }


    }


    public class Target
    {
        public bool isMissle;
        public bool hasObject;
        public bool enemy;
        public Vector3 position;
        public Vector3 velocity;
        public float closingRate;
        public float distance;
        public int playerID = -1;
        public float pitch = 0;

        public Target()
        {
            hasObject = false;
        }

        public float calculateClossingRate(Vector3 position, Vector3 velocity, Vector3 radarPosition, Vector3 radarVelocity)
        {
            Vector3 relativeVelocity = velocity - radarVelocity;
            Vector3 relativePositionNormalized = (radarPosition - position).normalized;
            float closingRate = relativeVelocity.x * relativePositionNormalized.x + relativeVelocity.y * relativePositionNormalized.y + relativeVelocity.z * relativePositionNormalized.z;
            return closingRate;
        }
        
        public Target(Collider col, BlockBehaviour radar)
        {
            playerID = col.attachedRigidbody.gameObject.GetComponent<BlockBehaviour>().ParentMachine.PlayerID;
            hasObject = true;
            if (col.attachedRigidbody.gameObject.name == "missle")
            {
                isMissle = true;
            }
            else
            {
                isMissle = false;
            }

            position = col.gameObject.transform.position;
            velocity = col.attachedRigidbody.velocity;
            distance = Vector3.Distance(radar.transform.position, position);
            //calculating the closingRate
            closingRate = calculateClossingRate(position, velocity, radar.transform.position, radar.GetComponent<Rigidbody>().velocity);
            //calculate the angle displayed on displayer
            //displayAngle = vector2angle(new Vector2(myVelocity.x, myVelocity.z))-vector2angle(new Vector2(-relativePositionNormalized.x,-relativePositionNormalized.z));
            float deltaPitch = Vector3.Angle(Vector3.up, radar.transform.forward) 
                                - Vector3.Angle(Vector3.up, col.transform.position - radar.transform.position);
            pitch = Mathf.Clamp(deltaPitch, -35, 35);

            if (col.attachedRigidbody.gameObject.GetComponent<BlockBehaviour>().Team == radar.Team)
            {
                enemy = false;
            }
            else
            {
                enemy = true;
            }
        }
        protected float vector2angle(Vector2 vec)
        {
            if (vec.y>0)
            {
                return Vector2.Angle(vec, Vector2.right);
            }
            else
            {
                return 360 - Vector2.Angle(vec, Vector2.right);
            }
        }
    }

    public class targetManager : MonoBehaviour
    {
        public Target[] targets = new Target[101];

        public float calculateClossingRate(Vector3 position, Vector3 velocity, Vector3 radarPosition, Vector3 radarVelocity)
        {
            Vector3 relativeVelocity = velocity - radarVelocity;
            Vector3 relativePositionNormalized = (radarPosition - position).normalized;
            float closingRate = relativeVelocity.x * relativePositionNormalized.x + relativeVelocity.y * relativePositionNormalized.y + relativeVelocity.z * relativePositionNormalized.z;
            return closingRate;
        }

        public targetManager()
        {
            for (int i = 0; i < 101; i++)
            {
                
                targets[i] = new Target();
            }
        }
        
        public void AddTarget(int currRegion, Collider col, BlockBehaviour radar, bool DopplerFeature)
        {
            
            if (DopplerFeature)
            {
                Target tmpTarget = new Target(col, radar);
                float staticClosingRate = calculateClossingRate(tmpTarget.position, Vector3.zero, radar.transform.position, radar.GetComponent<Rigidbody>().velocity);
                if (Math.Abs(tmpTarget.closingRate - staticClosingRate) > 35 || col.transform.position.y > 500)
                {
                    targets[currRegion] = tmpTarget;
                }
            }
            else
            {
                targets[currRegion] = new Target(col, radar);
            }
            
            
            //Debug.Log(currRegion.ToString() + " add a target");
        }

        public void RemoveTarget(int currRegion)
        {
            if (targets[currRegion].hasObject)
            {
                targets[currRegion].hasObject = false;
                targets[currRegion].distance = 0;
            }
        }
    }
    

    
    public class RadarBlock : BlockScript
    {
        public MToggle IFF;
        public MToggle ShowScan;
        public MToggle DopplerFeature;

        public targetManager targetManagerRadar;
        public float scanAngle = 0;
        public float scanPitch = 0;
        public GameObject ScanCollider;
        public GameObject ScanColliderFar;
        public MeshCollider radarScan;
        public BoxCollider radarScanFar;
        public MeshFilter radarMF;
        public MeshRenderer radarMR;
        public MeshFilter radarFarMF;
        public MeshRenderer radarFarMR;
        public GameObject RadarScanDisplayer;
        public GameObject RadarScanFarDisplayer;
        public GameObject RadarBase;
        public ScanCollisonHit radarHit;
        public ScanCollisonHit radarFarHit;
        public GameObject RadarHead;
        public MeshFilter RadarHeadMF;
        public MeshRenderer RadarHeadMR;

        public IEnumerator SendRadarHead;

        public static MessageType ClientRadarHeadMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Vector3);
        //tmp target data
        // playerID, targetIndex, distance, hasObject
        public static MessageType ClientTmpTargetData = ModNetworking.CreateMessageType(DataType.Integer, DataType.Integer, DataType.Single);

        protected Mesh scannerMesh;
        protected Mesh radarHeadMesh;
        protected Texture radarHeadTexture;
        
        protected Color displayColor;

        protected int myPlayerID;
        protected Transform myTransform;

        IEnumerator SendRadarMsg()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.05f);
                if (!StatMaster.isClient)
                {
                    ModNetworking.SendToAll(ClientRadarHeadMsg.CreateMessage((int)myPlayerID, (Vector3)ScanCollider.transform.localRotation.eulerAngles));
                }
            }
        }
        public bool OnClockWise(Vector2 from, Vector2 to)
        {
            if (from.x * to.y - to.x * from.y < 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        protected void InitScan()
        {
            if (transform.FindChild("Radar Base") == null)
            {
                RadarBase = new GameObject("Radar Base");
                RadarBase.transform.SetParent(transform);
                RadarBase.transform.localPosition = new Vector3(0, 0, 0.5f);
                //RadarBase.transform.rotation = Quaternion.Euler(180f, 0f, 0f);
                RadarBase.transform.localScale = new Vector3(1, 1, 1);
            }


            if (RadarBase.transform.FindChild("RadarScanCol") == null)
            {
                ScanCollider = (GameObject)Instantiate(AssetManager.Instance.MeshCol.RadarScanCol, RadarBase.transform);
                ScanCollider.name = "RadarScanCol";
                ScanCollider.transform.SetParent(RadarBase.transform);
                ScanCollider.transform.localPosition = new Vector3(0f, 0f, 0f);
                ScanCollider.transform.localRotation = Quaternion.Euler(270f, 0f, 0f);
                ScanCollider.transform.localScale = new Vector3(2000, 2000, 2000);
                
                
                radarScan = ScanCollider.GetComponent<MeshCollider>();
                scannerMesh = radarScan.sharedMesh;
                radarHit = ScanCollider.AddComponent<ScanCollisonHit>();
                radarHit.Reset();
                radarHit.myTeam = BlockBehaviour.Team;
                radarHit.IFF = !IFF.isDefaultValue;
                radarHit.isRadarScan = true;
                
            }

            if (ScanCollider.transform.FindChild("RadarScanFarCol") == null)
            {
                ScanColliderFar = new GameObject("RadarScanFarCol");
                ScanColliderFar.transform.SetParent(ScanCollider.transform);
                ScanColliderFar.transform.localPosition = new Vector3(0f, 4f, 0f);
                ScanColliderFar.transform.localRotation = Quaternion.Euler(0, 0, 0);
                ScanColliderFar.transform.localScale = new Vector3(0.11f, 4.1f, 1.3f);
                radarScanFar = ScanColliderFar.AddComponent<BoxCollider>();
                radarScanFar.isTrigger = true;
                radarFarHit = ScanColliderFar.AddComponent<ScanCollisonHit>();
                radarFarHit.Reset();
                radarFarHit.myTeam = BlockBehaviour.Team;
                radarFarHit.IFF = !IFF.isDefaultValue;
                radarFarHit.isRadarScan= true;

            }

            if (ScanCollider.transform.FindChild("RadarScanDisplay") == null)
            {
                RadarScanDisplayer = new GameObject("RadarScanDisplay");
                RadarScanDisplayer.transform.SetParent(ScanCollider.transform);
                RadarScanDisplayer.transform.localPosition = new Vector3(0f, 0f, 0f);
                RadarScanDisplayer.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                RadarScanDisplayer.transform.localScale = Vector3.one;
                radarMF = RadarScanDisplayer.AddComponent<MeshFilter>();
                radarMF.sharedMesh = scannerMesh;
                radarMR = RadarScanDisplayer.AddComponent<MeshRenderer>();
                radarMR.material = new Material(Shader.Find("Particles/Alpha Blended"));
                displayColor = Color.green;
                displayColor.a = 0.05f;
                radarMR.material.SetColor("_TintColor", displayColor);
                RadarScanDisplayer.SetActive(false);
            }

            //if (ScanColliderFar.transform.FindChild("RadarScanFarDisplay") == null)
            //{
            //    RadarScanFarDisplayer = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //    RadarScanFarDisplayer.name = "RadarScanFarDisplay";
            //    RadarScanFarDisplayer.transform.SetParent(ScanColliderFar.transform);
            //    Destroy(RadarScanFarDisplayer.GetComponent<Collider>());
            //    Destroy(RadarScanFarDisplayer.GetComponent<Rigidbody>());
            //    RadarScanFarDisplayer.transform.localPosition = new Vector3(0f, 0f, 0f);
            //    RadarScanFarDisplayer.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            //    RadarScanFarDisplayer.transform.localScale = Vector3.one;
            //    RadarScanFarDisplayer.SetActive(false);
            //}


            if (ScanCollider.transform.FindChild("RadarHead") == null)
            {
                radarHeadMesh = ModResource.GetMesh("RadarHead Mesh").Mesh;
                radarHeadTexture = ModResource.GetTexture("RadarHead Texture").Texture;

                RadarHead = new GameObject("RadarHead");
                RadarHead.transform.SetParent(ScanCollider.transform);
                RadarHead.transform.localPosition = new Vector3(0f, 0f, 0f);
                RadarHead.transform.localRotation = Quaternion.Euler(270f, 0f, 0f);
                RadarHead.transform.localScale = new Vector3(0.00011f, 0.00011f, 0.00011f);
                radarMF = RadarHead.AddComponent<MeshFilter>();
                radarMF.sharedMesh = radarHeadMesh;
                radarMR = RadarHead.AddComponent<MeshRenderer>();
                radarMR.material.mainTexture = radarHeadTexture;
                RadarHead.SetActive(true);
            }
            ScanCollider.SetActive(false);
        }

        protected void GetTWSAim()
        {
            //Debug.Log(Math.Round((scanAngle + 60) / 2.4f));
            int currRegion = (int)Math.Floor((scanAngle+60)/1.2f+0.5f);
            //Debug.Log(currRegion);
            //Debug.Log(radarHit.targetCols.Count);
            if (radarHit.targetCols.Count == 0 && radarFarHit.targetCols.Count == 0)
            {
                targetManagerRadar.RemoveTarget(currRegion);
            }
            else
            {
                Collider targetCol;
                if (radarHit.targetCols.Count != 0)
                {
                    targetCol = radarHit.targetCols.Peek();
                }
                else
                {
                    targetCol = radarFarHit.targetCols.Peek();
                }
                
                //Debug.Log(targetCol.name);
                targetManagerRadar.AddTarget(currRegion, targetCol, BlockBehaviour, DopplerFeature.isDefaultValue);

                //determine RWR message
                try
                {
                    int TargetPlayerID = targetCol.attachedRigidbody.gameObject.GetComponentInParent<BlockBehaviour>().ParentMachine.PlayerID;
                    //Debug.Log("playerID:");
                    //Debug.Log(TargetPlayerID);
                    Vector2 radarPosition = new Vector2(myTransform.position.x, myTransform.position.z);
                    Vector2 targetPosition = new Vector2(targetCol.transform.position.x, targetCol.transform.position.z);
                    Vector2 radarDirection = radarPosition - targetPosition;
                    Vector2 targetDirection = new Vector2(targetCol.attachedRigidbody.velocity.x, targetCol.attachedRigidbody.velocity.z);
                    float angleBetween = Vector2.Angle(targetDirection, radarDirection);
                    int AngleIndex = (int)Math.Floor(angleBetween / 45f + 0.5f);
                    if (!OnClockWise(targetDirection, radarDirection))
                        AngleIndex = 8 - AngleIndex;
                    if (AngleIndex == 8)
                        AngleIndex = 0;
                    
                    //DataManager.Instance.RWRData[TargetPlayerID, AngleIndex].hasRadiation = true;
                    DataManager.Instance.RWRData[TargetPlayerID, AngleIndex] = 1;
                    //Debug.Log(AngleIndex);
                }
                catch { }
                
            }
            
            
        }

        public override void SafeAwake()
        {
            IFF = AddToggle("IFF (mupliplayer)", "IFF (mupliplayer)", false);
            ShowScan = AddToggle("Display Scanner", "display scanner", false);
            DopplerFeature = AddToggle("Doppler Feature", "Doppler Feature", true);

            myTransform = transform;
            myPlayerID = BlockBehaviour.ParentMachine.PlayerID;
            
            targetManagerRadar = new targetManager();
        }

        public override void OnSimulateStart()
        {
            InitScan();
            ScanCollider.SetActive(true);
            if (ShowScan.IsActive && !StatMaster.isMP)
            {
                RadarScanDisplayer.SetActive(true);
            }
            if (StatMaster.isMP && !StatMaster.isClient)
            {
                SendRadarHead = SendRadarMsg();
                StartCoroutine(SendRadarHead);
            }
        }


        public override void OnSimulateStop()
        {
            RadarScanDisplayer.SetActive(false);
            if (StatMaster.isMP && !StatMaster.isClient)
            {
                StopCoroutine(SendRadarHead);
            }
            RadarDisplayerSimulator_MsgReceiver.Instance.CleartmpTargetData(myPlayerID);
        }
        public void FixedUpdate()
        {
            if (BlockBehaviour.isSimulating)
            {
                if (StatMaster.isClient)
                {
                    MySimulateFixedUpdateClient();
                }
            }
        }

        public override void SimulateFixedUpdateHost()
        {
            try
            {
                GetTWSAim();
            }
            catch { }
            int currRegion = (int)Math.Floor((scanAngle + 60) / 1.2f + 0.5f);
            
            
            try
            {
                ModNetworking.SendToAll(ClientTmpTargetData.CreateMessage(myPlayerID, currRegion, targetManagerRadar.targets[currRegion].distance));
                //RadarBase.transform.position = myTransform.position+0.5f*myTransform.localScale.z*transform.forward;
                RadarBase.transform.rotation = Quaternion.LookRotation((myTransform.rotation * Vector3.back).normalized);
                //DisplayerBlock tmp;
                //tmp = transform.parent.FindChild("Displayer").GetComponent<DisplayerBlock>();
                scanAngle = DataManager.Instance.DisplayerData[myPlayerID].radarAngle;
                scanPitch = DataManager.Instance.DisplayerData[myPlayerID].radarPitch;

                ScanCollider.transform.localRotation = Quaternion.Euler(270f+scanPitch, scanAngle, 0);

                radarHit.Reset();
                radarFarHit.Reset();
                DataManager.Instance.TargetData[myPlayerID] = targetManagerRadar;
                DataManager.Instance.RadarTransformForward[myPlayerID] = transform.forward;

            }
            catch
            {}
        }
        public void MySimulateFixedUpdateClient()
        {
            RadarBase.transform.rotation = Quaternion.LookRotation((myTransform.rotation * Vector3.back).normalized);
            ScanCollider.transform.localRotation = Quaternion.Lerp(ScanCollider.transform.localRotation, Quaternion.Euler(RadarMsgReceiver.Instance.ScanColLocalRotation[myPlayerID]),0.2f);
            DataManager.Instance.RadarTransformForward[myPlayerID] = transform.forward;
        }
        private void OnGUI()
        {
            //GUI.Box(new Rect(200, 200, 100, 50), targetManagerRadar.targets[49].hasObject.ToString());
            //GUI.Box(new Rect(200, 250, 100, 50), targetManagerRadar.targets[50].hasObject.ToString());
            //GUI.Box(new Rect(200, 300, 100, 50), targetManagerRadar.targets[51].hasObject.ToString());
        }
    }

}
