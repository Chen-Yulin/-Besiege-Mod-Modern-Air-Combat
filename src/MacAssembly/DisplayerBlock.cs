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
    class DisplayerMsgReceiver : SingleInstance<DisplayerMsgReceiver>
    {
        public override string Name { get; } = "DisplayerMsgReceiver";

        public Vector3[,] ClientTargetPosition = new Vector3[16, 101];//[playerID,region]
        public float[,] ClientTargetDistance = new float[16, 101];

        public float[] leftScanAngle = new float[16];
        public float[] rightScanAngle = new float[16];
        public float[] currAngle = new float[16];
        public bool[] SLDirection = new bool[16];
        public float[] radarPitch = new float[16];
        public float[] deltaScanAngle = new float[16];

        public bool[] panelUpdated = new bool[16];
            
        public bool[] locking = new bool[16];
        public Vector2[] ChooserPosition = new Vector2[16];

        public bool[] TargetLocked = new bool[16];

        public string[] LockInfo = new string[16];

        public Vector3[] OnGuiTargetPosition = new Vector3[16];

        public float[] BlackoutData = new float[16];
 
        public void DistanceReceiver(Message msg)
        {
            ClientTargetDistance[(int)msg.GetData(0), (int)msg.GetData(1)] = (float)msg.GetData(2);
        }
        public void PositionReceiver(Message msg)
        {
            ClientTargetPosition[(int)msg.GetData(0),(int)msg.GetData(1)] = (Vector3)msg.GetData(2);
        }
        public void PanelReceiver(Message msg)
        {
            int playerID = (int)msg.GetData(0);
            panelUpdated[playerID] = true;
            leftScanAngle[playerID] = (float)msg.GetData(1);
            rightScanAngle[playerID] = (float)msg.GetData(2);
            currAngle[playerID] = (float)msg.GetData(3);
            SLDirection[playerID] = (bool)msg.GetData(4);
            radarPitch[playerID] = (float)msg.GetData(5);
            deltaScanAngle[playerID] = (float)msg.GetData(6);
        }
        public void ChooserPositionReceiver(Message msg)
        {
            ChooserPosition[(int)msg.GetData(0)] = new Vector2((float)msg.GetData(1), (float)msg.GetData(2));
        }
        public void LockingReceiver(Message msg)
        {
            locking[(int)msg.GetData(0)] = (bool)msg.GetData(1);
        }

        public void LockedTargetReceiver(Message msg)
        {
            radarPitch[(int)msg.GetData(0)] = (float)msg.GetData(1);
            TargetLocked[(int)msg.GetData(0)] = (bool)msg.GetData(2);
            LockInfo[(int)msg.GetData(0)] = (string)msg.GetData(3);
        }
        public void OnGuiTargetPositionReceiver(Message msg)
        {
            OnGuiTargetPosition[(int)msg.GetData(0)] = (Vector3)msg.GetData(1);
        }

        public void BlackoutReceiver(Message msg)
        {
            BlackoutData[(int)msg.GetData(0)] = (float)msg.GetData(1);
        }

    }



    public class ScanLineController : MonoBehaviour
    {
        public float frequency = 1;
        public float angleLeft = -60;
        public float angleRight = 60;
        public float currAngle = 0;
        public bool direction = false;

        protected void Start()
        {
            currAngle = angleLeft;
        }

        protected void FixedUpdate()
        {
            if (direction == false)
            {
                currAngle += 1.2f * frequency;
                if (currAngle >= angleRight)
                {
                    direction = true;
                }
            }
            else
            {
                currAngle -= 1.2f * frequency;
                if (currAngle <= angleLeft)
                {
                    direction = false;
                }
            }
            //Debug.Log(currAngle);
        }

    }

    class DisplayerBlock : BlockScript
    {
        public MKey EnlargeScanAngle;
        public MKey ReduceScanAngle;
        public MKey ChooserUp;
        public MKey ChooserDown;
        public MKey ChooserLeft;
        public MKey ChooserRight;
        public MKey scanUp;
        public MKey scanDown;
        public MKey Lock;

        public MToggle GTolerance;

        public MSlider ScanRegionAfterLock;

        public GameObject BaseGrid;
        public ScanLineController SLController;
        public GameObject ScanLine;
        public GameObject LeftAngleIndicator;
        public GameObject RightAngleIndicator;
        public GameObject Chooser;
        public GameObject LockIcon;
        public GameObject LockInfo;
        public GameObject Mode;
        public float radarPitch = 0;
        public GameObject EnemyDisplayTWS; // gameobject for managing enemy icons
        public GameObject[] EnemyIconsTWS;
        public GameObject PitchIndicatorSelf;
        public GameObject PitchIndicatorTarget;
        public GameObject BlackOut;

        public Rigidbody myRigid;

        public Target[] RadarTarget;

        public Vector3 OnGuiTargetPosition;


        public static MessageType ClientTargetPositionMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Integer, DataType.Vector3);
        public static MessageType ClientTargetDistanceMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Integer, DataType.Single);

        //playerID, leftScanAngle, rightScanAngle, currAngle, SLDirection, radarPitch, deltaScanAngle
        public static MessageType ClientPanelMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Single, DataType.Single, DataType.Single, DataType.Boolean, DataType.Single, DataType.Single);
        public static MessageType ClientChooserMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Single, DataType.Single);
        public static MessageType ClientLockingMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Boolean);
        public static MessageType ClientLockedTargetMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Single, DataType.Boolean, DataType.String);
        public static MessageType ClientOnGuiTargetMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Vector3);

        public static MessageType ClientBlackoutMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Single);

        public Vector3 preVeclocity = Vector3.zero;
        public Vector3 overLoad = Vector3.zero;
        public float blackoutIndex = 0;

        public IEnumerator sendPanelMsg;
        


        protected MeshFilter BaseGridMeshFilter;
        protected MeshRenderer BaseGridRenderer;
        protected MeshFilter ScanLineMeshFilter;
        protected MeshRenderer ScanLineRenderer;
        protected MeshFilter LeftAngleIndicatorMeshFilter;
        protected MeshRenderer LeftAngleIndicatorRenderer;
        protected MeshFilter RightAngleIndicatorMeshFilter;
        protected MeshRenderer RightAngleIndicatorRenderer;
        protected MeshFilter ChooserMeshFilter;
        protected MeshRenderer ChooserRenderer;
        protected TextMesh ModeTextMesh;
        protected TextMesh InfoText;
        protected Texture LockIconOnScreen;
        protected float leftScanAngle = -60f;
        protected float rightScanAngle = 60f;
        protected float middleScanAngle = 0f;
        protected float realMiddleScanAngle = 0f;
        protected float deltaScanAngle = 60f;
        protected Vector2 ChooserPosition = new Vector2(0, 0);
        protected bool smallerAngle = false;
        protected bool biggerAngle = false;
        protected bool upChooser = false;
        protected bool downChooser = false;
        protected bool leftChooser = false;
        protected bool rightChooser = false;
        protected bool downScan = false;
        protected bool upScan = false;
        protected string mode = "TWS";
        public displayerData DisplayerData = new displayerData(0, 0);
        public RadarTargetData sendBVRData = new RadarTargetData();
        protected int myPlayerID;
        protected int currRegion;
        
        protected bool locking; //whether the radar keeps tracking an object
        protected int lockRegion = 0;
        protected int iconSize = 28;
        protected float deltaPitch = 0;

        public void DisplayBlackout()
        {
            if (!StatMaster.isMP || (StatMaster.isMP && !StatMaster.isClient))
            {
                overLoad = (myRigid.velocity - preVeclocity) / Time.fixedDeltaTime;
                //Debug.Log(overLoad.magnitude);

                preVeclocity = myRigid.velocity;

                if (overLoad.magnitude > 10 * 10)
                {
                    blackoutIndex += (overLoad.magnitude - 10 * 10) / (50 * 150f);
                }
                else
                {
                    blackoutIndex -= 0.005f;
                }
                blackoutIndex = Math.Min(blackoutIndex, 2);
                blackoutIndex = Math.Max(blackoutIndex, 0);
                if (!StatMaster.isClient)
                {
                    ModNetworking.SendToAll(ClientBlackoutMsg.CreateMessage(myPlayerID, blackoutIndex));
                    
                }
            }
            else
            {
                blackoutIndex = DisplayerMsgReceiver.Instance.BlackoutData[(int)PlayerData.localPlayer.networkId];
            }


            if (StatMaster.isMP)
            {
                if ((int)PlayerData.localPlayer.networkId == myPlayerID)
                {
                    BlackOut.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_TintColor", new Color(0, 0, 0, blackoutIndex));
                }
            }
            else
            {
                BlackOut.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_TintColor", new Color(0, 0, 0, blackoutIndex));
            }

        }

        public void DisplayLockedTargetClient()
        {
            radarPitch = DisplayerMsgReceiver.Instance.radarPitch[myPlayerID];
            if (LockIcon.activeSelf != DisplayerMsgReceiver.Instance.TargetLocked[myPlayerID])
            {
                LockIcon.SetActive(DisplayerMsgReceiver.Instance.TargetLocked[myPlayerID]);
                PitchIndicatorTarget.SetActive(DisplayerMsgReceiver.Instance.TargetLocked[myPlayerID]);
            }
            if (LockIcon.activeSelf)
            {
                LockIcon.transform.localPosition = new Vector3(ChooserPosition.x * 0.00175f, -ChooserPosition.y * 0.00175f, 0.095f);
                PitchIndicatorTarget.transform.localPosition = new Vector3(-0.1f, -radarPitch / (350 + 150 / 2), 0.095f);
                try
                {
                    Destroy(LockIcon.transform.FindChild("LockInfo").gameObject);
                }
                catch { }

                LockInfo = new GameObject("LockInfo");
                LockInfo.transform.SetParent(LockIcon.transform);
                LockInfo.transform.localPosition = new Vector3(2f, 1f, 0f);
                LockInfo.transform.localRotation = Quaternion.Euler(180, 0, 0);
                LockInfo.transform.localScale = new Vector3(1f, 1f, 1f);
                InfoText = LockInfo.AddComponent<TextMesh>();
                InfoText.text = DisplayerMsgReceiver.Instance.LockInfo[myPlayerID];
                InfoText.fontSize = 32;
                InfoText.fontStyle = FontStyle.Normal;
                InfoText.anchor = TextAnchor.LowerLeft;
                InfoText.characterSize = 0.6f;
                InfoText.color = Color.yellow;
                LockInfo.SetActive(true);
            }
            else
            {
                LockInfo.SetActive(false);
            }
        }

        public void PanelRectifyClient()
        {
            if (DisplayerMsgReceiver.Instance.panelUpdated[myPlayerID])
            {
                DisplayerMsgReceiver.Instance.panelUpdated[myPlayerID] = false;
                leftScanAngle = DisplayerMsgReceiver.Instance.leftScanAngle[myPlayerID];
                rightScanAngle = DisplayerMsgReceiver.Instance.rightScanAngle[myPlayerID];
                SLController.currAngle = DisplayerMsgReceiver.Instance.currAngle[myPlayerID];
                SLController.direction = DisplayerMsgReceiver.Instance.SLDirection[myPlayerID];
                radarPitch = DisplayerMsgReceiver.Instance.radarPitch[myPlayerID];
                deltaScanAngle = DisplayerMsgReceiver.Instance.deltaScanAngle[myPlayerID];
            }
            ChooserPosition = DisplayerMsgReceiver.Instance.ChooserPosition[myPlayerID];


        }

        public void InitEnemyIcons()
        {
            if (!transform.FindChild("EnemyDisplayTWS"))
            {
                EnemyDisplayTWS = new GameObject("EnemyDisplayTWS");
                EnemyDisplayTWS.transform.SetParent(transform);
                EnemyDisplayTWS.transform.localPosition = new Vector3(0f, 0f, 0.095f);
                EnemyDisplayTWS.transform.localRotation = Quaternion.Euler(0, 180, 180);
                EnemyDisplayTWS.transform.localScale = new Vector3(1f, 1f, 1f);
                EnemyIconsTWS = new GameObject[101];
                for (int i = 0; i < 101; i++)
                {
                    EnemyIconsTWS[i] = new GameObject("enemy" + i.ToString());
                    EnemyIconsTWS[i].transform.SetParent(EnemyDisplayTWS.transform);
                    EnemyIconsTWS[i].transform.localPosition = new Vector3(0f, 0f, 0f);
                    EnemyIconsTWS[i].transform.localRotation = Quaternion.Euler(0, 180, 180);
                    EnemyIconsTWS[i].transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                    TextMesh textMesh;
                    textMesh = EnemyIconsTWS[i].AddComponent<TextMesh>();
                    textMesh.text = "-";
                    textMesh.color = Color.green;
                    textMesh.characterSize = 0.5f;
                    textMesh.fontSize = 64;
                    textMesh.fontStyle = FontStyle.Bold;
                    textMesh.anchor = TextAnchor.MiddleCenter;
                    EnemyIconsTWS[i].SetActive(false);

                }
            }
        }

        public void InitPitchIndicator()
        {
            if (!transform.FindChild("PitchIndicatorSelf"))
            {
                PitchIndicatorSelf = new GameObject("PitchIndicatorSelf");
                PitchIndicatorSelf.transform.SetParent(transform);
                PitchIndicatorSelf.transform.localPosition = new Vector3(-0.1f, 0f, 0.095f);
                PitchIndicatorSelf.transform.localRotation = Quaternion.Euler(180, 0, 0);
                PitchIndicatorSelf.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                TextMesh textMesh;
                textMesh = PitchIndicatorSelf.AddComponent<TextMesh>();
                textMesh.text = "]";
                textMesh.color = Color.green;
                textMesh.characterSize = 1.2f;
                textMesh.fontSize = 32;
                textMesh.fontStyle = FontStyle.Normal;
                textMesh.anchor = TextAnchor.MiddleCenter;

                PitchIndicatorSelf.SetActive(false);
            }

            if (!transform.FindChild("PitchIndicatorTarget"))
            {
                PitchIndicatorTarget = new GameObject("PitchIndicatorTarget");
                PitchIndicatorTarget.transform.SetParent(transform);
                PitchIndicatorTarget.transform.localPosition = new Vector3(-0.1f, 0f, 0.095f);
                PitchIndicatorTarget.transform.localRotation = Quaternion.Euler(0, 0, 0);
                PitchIndicatorTarget.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                TextMesh textMesh;
                textMesh = PitchIndicatorTarget.AddComponent<TextMesh>();
                textMesh.text = "O";
                textMesh.color = Color.yellow;
                textMesh.characterSize = 0.6f;
                textMesh.fontSize = 32;
                textMesh.fontStyle = FontStyle.Normal;
                textMesh.anchor = TextAnchor.MiddleCenter;

                PitchIndicatorTarget.SetActive(false);
            }
        }
        public void DisplayEnemyHost()
        {
            for (int i = currRegion-2; i < currRegion+3; i++)
            {
                if (i<0 || i>100)
                {
                    return;
                }
                if (RadarTarget[i].hasObject)
                {
                    EnemyIconsTWS[i].transform.localPosition = new Vector3(0.0021f * (i - 50), - 0.0025f -0.105f + RadarTarget[i].distance * 0.000035f, 0f);
                    EnemyIconsTWS[i].SetActive(true);
                    if (StatMaster.isMP && i==currRegion)
                    {
                        Message TargetDistanceMsg = ClientTargetDistanceMsg.CreateMessage((int)myPlayerID, (int)i, (Single)RadarTarget[currRegion].distance);
                        ModNetworking.SendToAll(TargetDistanceMsg);
                    }
                }
                else
                {
                    EnemyIconsTWS[i].SetActive(false);
                    if (StatMaster.isMP && i == currRegion)
                    {
                        Message TargetDistanceMsg = ClientTargetDistanceMsg.CreateMessage((int)myPlayerID, (int)i, (Single)0);
                        ModNetworking.SendToAll(TargetDistanceMsg);
                    }

                }
            }

            
            ClearBlank();
        }
        public void DisplayEnemyClient()
        {
            float clientTargetDistance;
            for (int i = 0; i < 101; i++)
            {
                clientTargetDistance = DisplayerMsgReceiver.Instance.ClientTargetDistance[myPlayerID, i];
                if (clientTargetDistance != 0)
                {
                    EnemyIconsTWS[i].transform.localPosition = new Vector3(0.002f * (i - 50), -0.0025f - 0.105f + clientTargetDistance * 0.000035f, 0f);
                    EnemyIconsTWS[i].SetActive(true);
                }
                else
                {
                    EnemyIconsTWS[i].SetActive(false);
                }
            }
            
            ClearBlank();
        }

        public void DisplayPitchIndicatorHost()
        {
            PitchIndicatorSelf.transform.localPosition = new Vector3(-0.1f, -radarPitch / (350 + 150 / 2), 0.095f);
            if (locking)
            {
                PitchIndicatorTarget.SetActive(true);
                deltaPitch = Vector3.Angle(Vector3.up, DataManager.Instance.RadarTransformForward[myPlayerID]) - Vector3.Angle(Vector3.up, RadarTarget[lockRegion].position - transform.position);
                deltaPitch = Math.Max(deltaPitch, -35);
                deltaPitch = Math.Min(deltaPitch, 35);
                PitchIndicatorTarget.transform.localPosition = new Vector3(-0.1f, -deltaPitch / (350 + 150 / 2), 0.095f);
                radarPitch = deltaPitch;
            }

        }

        public void DisplayPitchIndicatorClient()
        {
            PitchIndicatorSelf.transform.localPosition = new Vector3(-0.1f, -radarPitch / (350 + 150 / 2), 0.095f);
        }
        IEnumerator SendPanelMsg()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                if (!StatMaster.isClient)
                {
                    Message PanelMessage = ClientPanelMsg.CreateMessage(myPlayerID, leftScanAngle, rightScanAngle, SLController.currAngle, SLController.direction, radarPitch, deltaScanAngle);
                    ModNetworking.SendToAll(PanelMessage);
                    //Debug.Log("Panel Message sent"+ SLController.currAngle.ToString());
                }
            }
        }

        public void InitGrid()
        {
            if (!transform.FindChild("Grid"))
            {
                BaseGrid = new GameObject("Grid");
                BaseGrid.transform.SetParent(transform);
                BaseGrid.transform.localPosition = new Vector3(0f, 0f, 0.095f);
                BaseGrid.transform.localRotation = Quaternion.Euler(270, 0, 180);
                BaseGrid.transform.localScale = new Vector3(0.105f, 0.105f, 0.105f);
                BaseGridMeshFilter = BaseGrid.AddComponent<MeshFilter>();
                BaseGridMeshFilter.mesh = ModResource.GetMesh("Plane Mesh").Mesh;
                BaseGridRenderer = BaseGrid.AddComponent<MeshRenderer>();
                BaseGridRenderer.material = new Material(Shader.Find("Particles/Alpha Blended"));
                BaseGridRenderer.material.SetTexture("_MainTex", ModResource.GetTexture("BaseGrid Texture"));
                BaseGridRenderer.material.SetColor("_TintColor", Color.green);
                BaseGrid.SetActive(true);
            }
        }

        public void InitPanel()
        {
            if (!transform.FindChild("ScanLine"))
            {
                ScanLine = new GameObject("ScanLine");
                ScanLine.transform.SetParent(transform);
                ScanLine.transform.localPosition = new Vector3(0f, 0f, 0.095f);
                ScanLine.transform.localRotation = Quaternion.Euler(270, 0, 180);
                ScanLine.transform.localScale = new Vector3(0.105f, 0.105f, 0.105f);
                ScanLineMeshFilter = ScanLine.AddComponent<MeshFilter>();
                ScanLineMeshFilter.mesh = ModResource.GetMesh("Plane Mesh").Mesh;
                ScanLineRenderer = ScanLine.AddComponent<MeshRenderer>();
                ScanLineRenderer.material = new Material(Shader.Find("Particles/Alpha Blended"));
                ScanLineRenderer.material.SetTexture("_MainTex", ModResource.GetTexture("ScanLine Texture"));
                ScanLineRenderer.material.SetColor("_TintColor", Color.green);
                ScanLine.SetActive(false);
            }
            if (!transform.FindChild("LeftAngleIndicator"))
            {
                LeftAngleIndicator = new GameObject("LeftAngleIndicator");
                LeftAngleIndicator.transform.SetParent(transform);
                LeftAngleIndicator.transform.localPosition = new Vector3(-0.105f, 0f, 0.095f);
                LeftAngleIndicator.transform.localRotation = Quaternion.Euler(270, 0, 180);
                LeftAngleIndicator.transform.localScale = new Vector3(0.105f, 0.110f, 0.110f);
                LeftAngleIndicatorMeshFilter = LeftAngleIndicator.AddComponent<MeshFilter>();
                LeftAngleIndicatorMeshFilter.mesh = ModResource.GetMesh("Plane Mesh").Mesh;
                LeftAngleIndicatorRenderer = LeftAngleIndicator.AddComponent<MeshRenderer>();
                LeftAngleIndicatorRenderer.material = new Material(Shader.Find("Particles/Alpha Blended"));
                LeftAngleIndicatorRenderer.material.SetTexture("_MainTex", ModResource.GetTexture("AngleIndicator Texture"));
                LeftAngleIndicatorRenderer.material.SetColor("_TintColor", Color.green);
                LeftAngleIndicator.SetActive(false);
            }
            if (!transform.FindChild("RightAngleIndicator"))
            {
                RightAngleIndicator = new GameObject("RightAngleIndicator");
                RightAngleIndicator.transform.SetParent(transform);
                RightAngleIndicator.transform.localPosition = new Vector3(0.105f, 0f, 0.095f);
                RightAngleIndicator.transform.localRotation = Quaternion.Euler(270, 0, 180);
                RightAngleIndicator.transform.localScale = new Vector3(0.105f, 0.110f, 0.110f);
                RightAngleIndicatorMeshFilter = RightAngleIndicator.AddComponent<MeshFilter>();
                RightAngleIndicatorMeshFilter.mesh = ModResource.GetMesh("Plane Mesh").Mesh;
                RightAngleIndicatorRenderer = RightAngleIndicator.AddComponent<MeshRenderer>();
                RightAngleIndicatorRenderer.material = new Material(Shader.Find("Particles/Alpha Blended"));
                RightAngleIndicatorRenderer.material.SetTexture("_MainTex", ModResource.GetTexture("AngleIndicator Texture"));
                RightAngleIndicatorRenderer.material.SetColor("_TintColor", Color.green);
                RightAngleIndicator.SetActive(false);
            }
            if (!transform.FindChild("Chooser"))
            {
                Chooser = new GameObject("Chooser");
                Chooser.transform.SetParent(transform);
                Chooser.transform.localPosition = new Vector3(0f, 0f, 0.095f);
                Chooser.transform.localRotation = Quaternion.Euler(270, 0, 180);
                Chooser.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                ChooserMeshFilter = Chooser.AddComponent<MeshFilter>();
                ChooserMeshFilter.mesh = ModResource.GetMesh("Plane Mesh").Mesh;
                ChooserRenderer = Chooser.AddComponent<MeshRenderer>();
                ChooserRenderer.material = new Material(Shader.Find("Particles/Alpha Blended"));
                ChooserRenderer.material.SetTexture("_MainTex", ModResource.GetTexture("Chooser Texture"));
                ChooserRenderer.material.SetColor("_TintColor", Color.yellow);
                Chooser.SetActive(false);
            }
            if (!transform.FindChild("LockIcon"))
            {
                LockIcon = new GameObject("LockIcon");
                LockIcon.transform.SetParent(transform);
                LockIcon.transform.localPosition = new Vector3(0f, 0f, 0.095f);
                LockIcon.transform.localRotation = Quaternion.Euler(0, 0, 0);
                LockIcon.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                TextMesh textMesh;
                textMesh = LockIcon.AddComponent<TextMesh>();
                textMesh.text = "O";
                textMesh.color = Color.yellow;
                textMesh.characterSize = 0.6f;
                textMesh.fontSize = 32;
                textMesh.fontStyle = FontStyle.Normal;
                textMesh.anchor = TextAnchor.MiddleCenter;


                LockIcon.SetActive(false);
            }



        }

        public void InitBlackOut()
        {
            if (StatMaster.isMP)
            {
                Debug.Log("Player " + myPlayerID.ToString() + " G-tolerance: " + GTolerance.isDefaultValue.ToString());
            }
            if (BlackOut == null)
            {
                BlackOut = GameObject.CreatePrimitive(PrimitiveType.Plane);
                BlackOut.name = "Blackout";
                BlackOut.transform.SetParent(Camera.main.transform);
                BlackOut.transform.localPosition = new Vector3(0, 0, 0.4f);

                //calculate camera height and width

                BlackOut.transform.localScale = new Vector3(1, 0.1f, 1);
                BlackOut.transform.localRotation = Quaternion.Euler(90, 0, 0);
                Destroy(BlackOut.GetComponent<Collider>());
                Destroy(BlackOut.GetComponent<Rigidbody>());
                Material blackoutMaterial = new Material(Shader.Find("Particles/Alpha Blended"));
                blackoutMaterial.mainTexture = ModResource.GetTexture("BlackOut Texture").Texture;
                blackoutMaterial.SetColor("_TintColor", new Color(0,0,0,0f));
                BlackOut.GetComponent<MeshRenderer>().sharedMaterial = blackoutMaterial;
                BlackOut.SetActive(GTolerance.isDefaultValue);

            }
        }

        public void InitMode(string mode)
        {
            if (!transform.FindChild("Mode"))
            {
                Mode = new GameObject("Mode");
                Mode.transform.SetParent(transform);
                Mode.transform.localPosition = new Vector3(-0.08f, 0.09f, 0.095f);
                Mode.transform.localRotation = Quaternion.Euler(0, 180, 180);
                Mode.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                ModeTextMesh = Mode.AddComponent<TextMesh>();
                ModeTextMesh.text = mode;
                ModeTextMesh.characterSize = 0.25f;
                ModeTextMesh.fontSize = 64;
                ModeTextMesh.anchor = TextAnchor.MiddleCenter;
                ModeTextMesh.color = Color.green;
                Mode.SetActive(false);
            }

        }

        public bool FindLockedTarget()
        {
            bool res = false;

            for (int i = 0; i < 15; i++)
            {
                if (lockRegion - i >= 0 && lockRegion + i <= 100)
                {
                    if (RadarTarget[lockRegion + i].hasObject && !RadarTarget[lockRegion + i].isMissle)
                    {
                        lockRegion = lockRegion + i;
                        res = true;
                    }
                    else if (RadarTarget[lockRegion - i].hasObject && !RadarTarget[lockRegion - i].isMissle)
                    {

                        lockRegion = lockRegion - i;
                        res = true;
                    }

                    if (res)
                    {
                        OnGuiTargetPosition = RadarTarget[lockRegion].position;
                        if (StatMaster.isMP)
                        {
                            ModNetworking.SendToAll(ClientOnGuiTargetMsg.CreateMessage(myPlayerID, OnGuiTargetPosition));
                        }

                        

                        ChooserPosition.x = (lockRegion - 50) * 1.2f;
                        ChooserPosition.y = (RadarTarget[lockRegion].distance - 3000) * 0.02f;
                        Chooser.transform.localPosition = new Vector3(ChooserPosition.x * 0.00175f, 0.105f - RadarTarget[lockRegion].distance * 0.000035f, 0.095f);
                        LockIcon.transform.localPosition = new Vector3(ChooserPosition.x * 0.00175f, 0.105f - RadarTarget[lockRegion].distance * 0.000035f, 0.095f);

                        LockIcon.SetActive(true);
                        try
                        {
                            DestroyImmediate(LockIcon.transform.FindChild("LockInfo").gameObject);
                        }
                        catch { }

                        LockInfo = new GameObject("LockInfo");
                        LockInfo.transform.SetParent(LockIcon.transform);
                        LockInfo.transform.localPosition = new Vector3(2f, 1f, 0f);
                        LockInfo.transform.localRotation = Quaternion.Euler(180, 0, 0);
                        LockInfo.transform.localScale = new Vector3(1f, 1f, 1f);
                        InfoText = LockInfo.AddComponent<TextMesh>();
                        InfoText.text = Math.Floor(RadarTarget[lockRegion].closingRate).ToString();
                        InfoText.fontSize = 32;
                        InfoText.fontStyle = FontStyle.Normal;
                        InfoText.anchor = TextAnchor.LowerLeft;
                        InfoText.characterSize = 0.6f;
                        InfoText.color = Color.yellow;
                        LockInfo.SetActive(true);
                        return true;
                    }
                }
            }
            if (lockRegion != currRegion)
            {
                return true;
            }

            return false;
        }

        public void ClearBlank()
        {
            float leftRegion = (leftScanAngle + 60) / 1.2f;
            float rightRegion = (rightScanAngle + 60) / 1.2f;
            for (int i = 0; i <= 100; i++)
            {
                if (i < leftRegion)
                {

                    EnemyIconsTWS[i].SetActive(false);
                }
                else
                {
                    break;
                }
            }
            for (int i = 100; i >= 0; i--)
            {
                if (i > rightRegion)
                {
                    EnemyIconsTWS[i].SetActive(false);
                }
                else
                {
                    break;
                }
            }
        }


        public override void SafeAwake()
        {
            myRigid = BlockBehaviour.GetComponent<Rigidbody>();
            myPlayerID = BlockBehaviour.ParentMachine.PlayerID;
            Lock = AddKey("Lock", "Lock Target", KeyCode.X);
            EnlargeScanAngle = AddKey("Expand scan range", "EnlargeScanAngle", KeyCode.T);
            ReduceScanAngle = AddKey("Reduce scan range", "ReduceScanAngle", KeyCode.U);
            ChooserUp = AddKey("Cursor up", "ChooserUp", KeyCode.Y);
            ChooserDown = AddKey("Cursor down", "ChooserDown", KeyCode.H);
            ChooserLeft = AddKey("Cursor left", "ChooserLeft", KeyCode.G);
            ChooserRight = AddKey("Cursor right", "ChooserRight", KeyCode.J);
            scanUp = AddKey("Radar pitch up", "scan up", KeyCode.I);
            scanDown = AddKey("Radar pitch down", "scan down", KeyCode.K);
            GTolerance = AddToggle("G-Tolerence", "G-Tolerence", true);
            ScanRegionAfterLock = AddSlider("Default scan angle when locking", "Default scan angle when locking", 20f, 5f, 60f);

            sendPanelMsg = SendPanelMsg();

            InitGrid();
            InitPanel();
            InitMode(mode);
            InitEnemyIcons();
            InitPitchIndicator();
            
            LockIconOnScreen = ModResource.GetTexture("LockIconScreen Texture").Texture;
        }

        
        public override void OnSimulateStart()
        {
            DataManager.Instance.BVRData[myPlayerID] = new RadarTargetData();
            preVeclocity = Vector3.zero;
            overLoad = Vector3.zero;
            blackoutIndex = 0;
            InitBlackOut();
            SLController = transform.gameObject.AddComponent<ScanLineController>();
            ScanLine.SetActive(true);
            LeftAngleIndicator.SetActive(true);
            RightAngleIndicator.SetActive(true);
            Chooser.SetActive(true);
            Mode.SetActive(true);
            PitchIndicatorSelf.SetActive(true);
            if (!StatMaster.isClient && StatMaster.isMP)
            {
                StartCoroutine(sendPanelMsg);
            }
        }

        public override void OnSimulateStop()
        {
            preVeclocity = Vector3.zero;
            overLoad = Vector3.zero;
            blackoutIndex = 0;
            ModNetworking.SendToAll(ClientBlackoutMsg.CreateMessage(myPlayerID, 0f));
            Destroy(BlackOut);
            if (!StatMaster.isClient && StatMaster.isMP)
            {
                StopCoroutine(sendPanelMsg);
            }
            
        }

        protected void Update()
        {
            if (IsSimulating)
            {
                //set the position of scanLine
                ScanLine.transform.localPosition = new Vector3(SLController.currAngle * 0.00175f, 0, 0.095f);
                //set the position of chooser
                if (!locking || StatMaster.isClient)
                {
                    Chooser.transform.localPosition = new Vector3(ChooserPosition.x * 0.00175f, -ChooserPosition.y * 0.00175f, 0.095f);
                }

                //set the position of left&right angleIndicator
                LeftAngleIndicator.transform.localPosition = new Vector3(leftScanAngle * 0.00175f, 0, 0.095f);
                RightAngleIndicator.transform.localPosition = new Vector3(rightScanAngle * 0.00175f, 0, 0.095f);
                //modify ScanLineController
                SLController.angleLeft = leftScanAngle;
                SLController.angleRight = rightScanAngle;

                //key 
                {
                    //judge whether the key for adjusting Scan angle is pressed
                    if (EnlargeScanAngle.IsPressed)
                    {
                        biggerAngle = true;
                    }
                    else if (EnlargeScanAngle.IsReleased)
                    {
                        biggerAngle = false;
                    }
                    if (ReduceScanAngle.IsPressed)
                    {
                        smallerAngle = true;
                    }
                    else if (ReduceScanAngle.IsReleased)
                    {
                        smallerAngle = false;
                    }

                    //judge wether the key for adjusting chooser is pressed
                    if (ChooserUp.IsPressed)
                    {
                        upChooser = true;
                    }
                    else if (ChooserUp.IsReleased)
                    {
                        upChooser = false;
                    }
                    if (ChooserDown.IsPressed)
                    {
                        downChooser = true;
                    }
                    else if (ChooserDown.IsReleased)
                    {
                        downChooser = false;
                    }
                    if (ChooserLeft.IsPressed)
                    {
                        leftChooser = true;
                    }
                    else if (ChooserLeft.IsReleased)
                    {
                        leftChooser = false;
                    }
                    if (ChooserRight.IsPressed)
                    {
                        rightChooser = true;
                    }
                    else if (ChooserRight.IsReleased)
                    {
                        rightChooser = false;
                    }

                    //judage wether the key for adjusting pitch of radar is pressed
                    if (scanUp.IsPressed)
                    {
                        upScan = true;
                    }
                    else if (scanUp.IsReleased)
                    {
                        upScan = false;
                    }
                    if (scanDown.IsPressed)
                    {
                        downScan = true;
                    }
                    else if (scanDown.IsReleased)
                    {
                        downScan = false;
                    }
                }

                //judage wether the key for toggle lock mode is pressed
                if (Lock.IsPressed && !StatMaster.isClient)
                {
                    locking = !locking;
                    deltaScanAngle = ScanRegionAfterLock.Value;
                    if (StatMaster.isMP)
                        ModNetworking.SendToAll(ClientPanelMsg.CreateMessage(myPlayerID, leftScanAngle, rightScanAngle, SLController.currAngle, SLController.direction, radarPitch, deltaScanAngle));
                    if (!locking)//what to do when mode is switched manuelly to unlock
                    {
                        deltaScanAngle = 60f;
                        if(StatMaster.isMP)
                            ModNetworking.SendToAll(ClientPanelMsg.CreateMessage(myPlayerID, leftScanAngle, rightScanAngle, SLController.currAngle, SLController.direction, radarPitch, deltaScanAngle));
                        radarPitch = 0f;
                        LockIcon.SetActive(false);
                        PitchIndicatorTarget.SetActive(false);
                    }
                }

            }
        }
        public override void SimulateFixedUpdateHost()
        {
            DisplayBlackout();
            try
            {
                currRegion = (int)Math.Floor((SLController.currAngle + 60) / 1.2f + 0.5f);
                lockRegion = (int)Math.Floor((ChooserPosition.x + 60) / 1.2f + 0.5f);

                if (!StatMaster.isClient)
                {
                    ModNetworking.SendToAll(ClientLockingMsg.CreateMessage(myPlayerID, locking));
                    ModNetworking.SendToAll(ClientChooserMsg.CreateMessage((int)myPlayerID, ChooserPosition.x, ChooserPosition.y));
                }


                //start track if lock mode on
                if (locking)
                {
                    if (!FindLockedTarget())//what to do when mode is switched passively to unlock
                    {
                        deltaScanAngle = 60f;
                        if (StatMaster.isMP)
                            ModNetworking.SendToAll(ClientPanelMsg.CreateMessage(myPlayerID, leftScanAngle, rightScanAngle, SLController.currAngle, SLController.direction, radarPitch, deltaScanAngle));
                        radarPitch = 0f;
                        LockIcon.SetActive(false);
                        PitchIndicatorTarget.SetActive(false);
                        locking = false;

                    }
                    if (!StatMaster.isClient)
                    {
                        ModNetworking.SendToAll(ClientLockedTargetMsg.CreateMessage((int)myPlayerID, radarPitch, (bool)LockIcon.activeSelf, (string)InfoText.text));
                    }
                }
                else
                {
                    //adjust the Chooser and the effected scan angle
                    if (upChooser && ChooserPosition.y < 60)
                    {
                        ChooserPosition.y += 0.4f;
                    }
                    if (downChooser && ChooserPosition.y > -60)
                    {
                        ChooserPosition.y -= 0.4f;
                    }
                    if (leftChooser && ChooserPosition.x > -60)
                    {
                        ChooserPosition.x -= 0.4f;
                    }
                    if (rightChooser && ChooserPosition.x < 60)
                    {
                        ChooserPosition.x += 0.4f;
                    }

                }
                middleScanAngle = ChooserPosition.x;
                //adjust the scan angle according to key
                if (biggerAngle && deltaScanAngle < 60)
                {
                    deltaScanAngle += 0.4f;
                }
                else if (smallerAngle && deltaScanAngle > 0)
                {
                    deltaScanAngle -= 0.4f;
                }


                //calculate real left and right angle
                if (middleScanAngle - deltaScanAngle >= -60)
                {
                    realMiddleScanAngle = middleScanAngle;
                    if (middleScanAngle + deltaScanAngle <= 60)
                    {
                        realMiddleScanAngle = middleScanAngle;
                    }
                    else
                    {
                        realMiddleScanAngle = 60 - deltaScanAngle;
                    }
                }
                else
                {
                    realMiddleScanAngle = -60 + deltaScanAngle;
                }
                leftScanAngle = realMiddleScanAngle - deltaScanAngle;
                rightScanAngle = realMiddleScanAngle + deltaScanAngle;


                //adjust the pitch of radar according to key
                if (upScan && radarPitch < 35)
                {
                    radarPitch += 0.4f;
                }
                else if (downScan && radarPitch > -35)
                {
                    radarPitch -= 0.4f;
                }

            
                DisplayerData.radarPitch = radarPitch;
                DisplayerData.radarAngle = SLController.currAngle;
                DataManager.Instance.DisplayerData[myPlayerID] = DisplayerData;

            
                RadarTarget = DataManager.Instance.TargetData[myPlayerID].targets;
            
            
            
                //Todo: send target display message to client


                DisplayEnemyHost();
                DisplayPitchIndicatorHost();//always after displayEnemy


                if (locking)
                {
                    sendBVRData.position = RadarTarget[lockRegion].position;
                    sendBVRData.velocity = RadarTarget[lockRegion].velocity;
                    DataManager.Instance.BVRData[myPlayerID] = sendBVRData;
                }
                else
                {
                    DataManager.Instance.BVRData[myPlayerID] = new RadarTargetData();
                }
            }
            catch
            {
            }


        }

        public override void SimulateFixedUpdateClient()
        {
            DisplayBlackout();
            PanelRectifyClient();
            locking = DisplayerMsgReceiver.Instance.locking[myPlayerID];

            currRegion = (int)Math.Floor((SLController.currAngle + 60) / 1.2f + 0.5f);
            lockRegion = (int)Math.Floor((ChooserPosition.x + 60) / 1.2f + 0.5f);



            //start track if lock mode on

            if (locking)
            {
                DisplayLockedTargetClient();

                OnGuiTargetPosition = DisplayerMsgReceiver.Instance.OnGuiTargetPosition[myPlayerID];
            }
            else
            {
                //adjust the Chooser and the effected scan angle
                if (upChooser && ChooserPosition.y < 60)
                {
                    ChooserPosition.y += 0.4f;
                }
                if (downChooser && ChooserPosition.y > -60)
                {
                    ChooserPosition.y -= 0.4f;
                }
                if (leftChooser && ChooserPosition.x > -60)
                {
                    ChooserPosition.x -= 0.4f;
                }
                if (rightChooser && ChooserPosition.x < 60)
                {
                    ChooserPosition.x += 0.4f;
                }
                if (LockIcon.activeSelf)
                {
                    LockIcon.SetActive(false);
                    PitchIndicatorTarget.SetActive(false);
                    LockInfo.SetActive(false);
                }
                
            }
            middleScanAngle = ChooserPosition.x;
            //adjust the scan angle according to key
            if (biggerAngle && deltaScanAngle < 60)
            {
                deltaScanAngle += 0.4f;
            }
            else if (smallerAngle && deltaScanAngle > 0)
            {
                deltaScanAngle -= 0.4f;
            }

            if (middleScanAngle - deltaScanAngle >= -60)
            {
                realMiddleScanAngle = middleScanAngle;
                if (middleScanAngle + deltaScanAngle <= 60)
                {
                    realMiddleScanAngle = middleScanAngle;
                }
                else
                {
                    realMiddleScanAngle = 60 - deltaScanAngle;
                }
            }
            else
            {
                realMiddleScanAngle = -60 + deltaScanAngle;
            }


            //leftScanAngle = Math.Max(-60f, middleScanAngle - deltaScanAngle);
            //rightScanAngle = Math.Min(60f, middleScanAngle + deltaScanAngle);
            leftScanAngle = realMiddleScanAngle - deltaScanAngle;
            rightScanAngle = realMiddleScanAngle + deltaScanAngle;

            //adjust the pitch of radar according to key
            if (upScan && radarPitch < 35)
            {
                radarPitch += 0.4f;
            }
            else if (downScan && radarPitch > -35)
            {
                radarPitch -= 0.4f;
            }


            //Todo: recieive message from host and update TargetDisplay data
            DisplayEnemyClient();
            DisplayPitchIndicatorClient();//always after displayEnemy

            
            

        }

        void OnGUI()
        {
            if (locking && IsSimulating)
            {
                if (StatMaster.isMP)
                {
                    if (PlayerData.localPlayer.networkId != myPlayerID)
                    {
                        return;
                    }
                }
                GUI.color = Color.green;
                Vector3 onScreenPosition = Camera.main.WorldToScreenPoint(OnGuiTargetPosition);
                if (onScreenPosition.z >= 0)
                    GUI.DrawTexture(new Rect(onScreenPosition.x - iconSize / 2, Camera.main.pixelHeight - onScreenPosition.y - iconSize / 2, iconSize, iconSize), LockIconOnScreen);
            }
            //if (myPlayerID == PlayerData.localPlayer.networkId)
            //{
            //    GUI.Box(new Rect(100, 100, 200, 50), myPlayerID.ToString() + "   " + DisplayerMsgReceiver.Instance.BlackoutData[PlayerData.localPlayer.networkId].ToString());
            //}


        }

    }
}