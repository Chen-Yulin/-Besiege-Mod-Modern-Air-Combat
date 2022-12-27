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
    //class Radar2CCData : SingleInstance<Radar2CCData>
    //{
    //    public override string Name { get; } = "Radar2CC Data";
    //    public class CC2Radar
    //    {
    //        public float radarPitch = 0;
    //        public float radarAngle = 0;
    //        public void Reset()
    //        {
    //            radarPitch = 0;
    //            radarAngle = 0;
    //        }
    //    }
    //    public class Radar2CC
    //    {

    //    }

    //    public CC2Radar cc2radar;

    //    public Radar2CCData(){
    //        cc2radar = new CC2Radar();
    //    }

    //}
    class CC2RadarDisplayerData : SingleInstance<CC2RadarDisplayerData>
    {
        public override string Name { get; } = "CC2RadarDisplayer Data";
    }
    // assist controller
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

    //simulator
    public class RadarDisplayerSimulator : BlockScript
    {
        public bool isClient = false;
        // for RadarDisplayer
        public ScanLineController SLController;
        public Target[] RadarTarget;
        public IEnumerator sendPanelMsg;
        public static MessageType ClientTargetPositionMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Integer, DataType.Vector3);
        public static MessageType ClientTargetDistanceMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Integer, DataType.Single);
        //playerID, leftScanAngle, rightScanAngle, currAngle, SLDirection, radarPitch, deltaScanAngle
        public static MessageType ClientPanelMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Single, DataType.Single, DataType.Single, DataType.Boolean, DataType.Single, DataType.Single);
        public static MessageType ClientChooserMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Single, DataType.Single);
        public static MessageType ClientLockingMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Boolean);
        public static MessageType ClientLockedTargetMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Single, DataType.Boolean, DataType.String);
        public float radarPitch = 0;
        public float leftScanAngle = -60f;
        public float rightScanAngle = 60f;
        public float middleScanAngle = 0f;
        public float realMiddleScanAngle = 0f;
        public float deltaScanAngle = 60f;
        public Vector2 ChooserPosition = new Vector2(0, 0);
        public bool smallerAngle = false;
        public bool biggerAngle = false;
        public bool upChooser = false;
        public bool downChooser = false;
        public bool leftChooser = false;
        public bool rightChooser = false;
        public bool downScan = false;
        public bool upScan = false;
        public int currRegion;
        public bool locking; //whether the radar keeps tracking an object
        public int lockRegion = 0;
        public float deltaPitch = 0;
        public int currLockedPlayerID = -1;
        public displayerData DisplayerData = new displayerData(0, 0);
        public RadarTargetData sendBVRData = new RadarTargetData();

        // for displayer

        public void InitCCRadarDisplayer()
        {
            SLController = transform.gameObject.AddComponent<ScanLineController>();
        }
        
        public void UnlockedChooserMotion_FixedUpdateHost()
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
        public void UnlockedPitchMotion_FixedUpdateHost()
        {
            if (upScan && radarPitch < 35)
            {
                radarPitch += 0.4f;
            }
            else if (downScan && radarPitch > -35)
            {
                radarPitch -= 0.4f;
            }
        }
        public void AdjustScanAngle_FixedUpdate()
        {
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
        }

        public void SyncSLcontroller()
        {
            SLController.angleLeft = leftScanAngle;
            SLController.angleRight = rightScanAngle;
        }

        public void Start()
        {
            InitCCRadarDisplayer();
        }
        public void Update()
        {
            SyncSLcontroller();
        }
        public void FixedUpdate()
        {
            if (!isClient)
            {
                if (!locking)
                {
                    UnlockedChooserMotion_FixedUpdateHost();
                    UnlockedPitchMotion_FixedUpdateHost();
                }
                else
                {

                }
                AdjustScanAngle_FixedUpdate();
            }
            else
            {

            }
        }

    }

    class CentralController : BlockScript
    {
        //simulators
        public RadarDisplayerSimulator radarDisplayerSimulator;
        public GameObject radarDisplayerSimulatorObject;

        // general
        public int myPlayerID;
        public MToggle GTolerance;
        public Rigidbody myRigid;
        public GameObject BlackOut;
        public Vector3 preVeclocity = Vector3.zero;
        public Vector3 overLoad = Vector3.zero;
        public float blackoutIndex = 0;
        public Vector3 OnGuiTargetPosition;
        protected int iconSize = 28;
        public static MessageType ClientOnGuiTargetMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Vector3);
        public static MessageType ClientBlackoutMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Single);

        // for radar displayer
        public MKey EnlargeScanAngle;
        public MKey ReduceScanAngle;
        public MKey ChooserUp;
        public MKey ChooserDown;
        public MKey ChooserLeft;
        public MKey ChooserRight;
        public MKey scanUp;
        public MKey scanDown;
        public MKey RadarLock;
        public MSlider ScanRegionAfterLock;

        // for blackout
        public void addGeneralKey()
        {
            GTolerance = AddToggle("G-Tolerence", "G-Tolerence", true);
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
                blackoutMaterial.SetColor("_TintColor", new Color(0, 0, 0, 0f));
                BlackOut.GetComponent<MeshRenderer>().sharedMaterial = blackoutMaterial;
                BlackOut.SetActive(GTolerance.isDefaultValue);

            }
        }
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

        // for radar displayer
        public void addRadarDisplayerKey()
        {
            RadarLock = AddKey("Lock", "Lock Target", KeyCode.X);
            EnlargeScanAngle = AddKey("Expand scan range", "EnlargeScanAngle", KeyCode.T);
            ReduceScanAngle = AddKey("Reduce scan range", "ReduceScanAngle", KeyCode.U);
            ChooserUp = AddKey("Cursor up", "ChooserUp", KeyCode.Y);
            ChooserDown = AddKey("Cursor down", "ChooserDown", KeyCode.H);
            ChooserLeft = AddKey("Cursor left", "ChooserLeft", KeyCode.G);
            ChooserRight = AddKey("Cursor right", "ChooserRight", KeyCode.J);
            scanUp = AddKey("Radar pitch up", "scan up", KeyCode.I);
            scanDown = AddKey("Radar pitch down", "scan down", KeyCode.K);
            ScanRegionAfterLock = AddSlider("Default scan angle when locking", "Default scan angle when locking", 20f, 5f, 60f);
        }

        public void RadarDisplayerKey_Update()
        {
            //judge whether the key for adjusting Scan angle is pressed
            if (EnlargeScanAngle.IsPressed)
            {
                radarDisplayerSimulator.biggerAngle = true;
            }
            else if (EnlargeScanAngle.IsReleased)
            {
                radarDisplayerSimulator.biggerAngle = false;
            }
            if (ReduceScanAngle.IsPressed)
            {
                radarDisplayerSimulator.smallerAngle = true;
            }
            else if (ReduceScanAngle.IsReleased)
            {
                radarDisplayerSimulator.smallerAngle = false;
            }

            //judge wether the key for adjusting chooser is pressed
            if (ChooserUp.IsPressed)
            {
                radarDisplayerSimulator.upChooser = true;
            }
            else if (ChooserUp.IsReleased)
            {
                radarDisplayerSimulator.upChooser = false;
            }
            if (ChooserDown.IsPressed)
            {
                radarDisplayerSimulator.downChooser = true;
            }
            else if (ChooserDown.IsReleased)
            {
                radarDisplayerSimulator.downChooser = false;
            }
            if (ChooserLeft.IsPressed)
            {
                radarDisplayerSimulator.leftChooser = true;
            }
            else if (ChooserLeft.IsReleased)
            {
                radarDisplayerSimulator.leftChooser = false;
            }
            if (ChooserRight.IsPressed)
            {
                radarDisplayerSimulator.rightChooser = true;
            }
            else if (ChooserRight.IsReleased)
            {
                radarDisplayerSimulator.rightChooser = false;
            }

            //judage wether the key for adjusting pitch of radar is pressed
            if (scanUp.IsPressed)
            {
                radarDisplayerSimulator.upScan = true;
            }
            else if (scanUp.IsReleased)
            {
                radarDisplayerSimulator.upScan = false;
            }
            if (scanDown.IsPressed)
            {
                radarDisplayerSimulator.downScan = true;
            }
            else if (scanDown.IsReleased)
            {
                radarDisplayerSimulator.downScan = false;
            }
        }

        public void SendRadarRara()
        {
            displayerData DisplayerData = new displayerData(0,0);
            DisplayerData.radarPitch = radarDisplayerSimulator.radarPitch;
            DisplayerData.radarAngle = radarDisplayerSimulator.SLController.currAngle;
            DataManager.Instance.DisplayerData[myPlayerID] = DisplayerData;
        }

        public override void SafeAwake()
        {
            myPlayerID = BlockBehaviour.ParentMachine.PlayerID;
            addRadarDisplayerKey();
        }
        public override void OnSimulateStart()
        {
            radarDisplayerSimulator = new RadarDisplayerSimulator();
            if (!transform.Find("Radar Displayer Simulator"))
            {
                radarDisplayerSimulatorObject = new GameObject("Radar Displayer Simulator");
                radarDisplayerSimulatorObject.transform.SetParent(transform);
                radarDisplayerSimulator = radarDisplayerSimulatorObject.AddComponent<RadarDisplayerSimulator>();
            }
        }
        public override void OnSimulateStop()
        {
            //Radar2CCData.Instance.cc2radar.Reset();
        }
        public override void SimulateUpdateHost()
        {
            RadarDisplayerKey_Update();
            SendRadarRara();
        }
        public void OnGUI()
        {
            //GUI.Box(new Rect(100, 200, 200, 50), radarDisplayerSimulator.radarPitch.ToString());
            //GUI.Box(new Rect(100, 250, 200, 50), radarDisplayerSimulator.SLController.currAngle.ToString());
        }

    }
}
