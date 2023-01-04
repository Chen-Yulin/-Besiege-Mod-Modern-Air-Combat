using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;

using Modding.Modules;
using Modding;
using Modding.Blocks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace ModernAirCombat
{
    //receiver
    class RadarDisplayerSimulator_MsgReceiver : SingleInstance<RadarDisplayerSimulator_MsgReceiver>
    {
        public override string Name { get; } = "RadarDisplayerSimulator MsgReceiver";

        public bool[] NormalUpdated = new bool[16];
        public bool[] LockUpdated = new bool[16];
        // lock
        public bool[] locking = new bool[16];
        public float[] closingRate = new float[16];
        // normal
        public float[] pitch = new float[16];
        public float[] deltaScanAngle = new float[16];
        public Vector2[] ChooserPosition = new Vector2[16];
        public float[] SLcurrAngle = new float[16];
        public bool[] SLcurrDirection = new bool[16];



        public void NormalPanelReceiver(Message msg)
        {
            int playerID = (int)msg.GetData(0);
            NormalUpdated[playerID] = true;
            pitch[playerID] = (float)msg.GetData(1);
            deltaScanAngle[playerID] = (float)msg.GetData(2);
            ChooserPosition[playerID].x = (float)msg.GetData(3);
            ChooserPosition[playerID].y = (float)msg.GetData(4);
            SLcurrAngle[playerID] = (float)msg.GetData(5);
            SLcurrDirection[playerID] = (bool)msg.GetData(6);
        }
        public void LockPanelReceiver(Message msg)
        {
            int playerID = (int)msg.GetData(0);
            LockUpdated[playerID] = true;
            pitch[playerID] = (float)msg.GetData(1);
            closingRate[playerID] = (float)msg.GetData(2);
            ChooserPosition[playerID].x = (float)msg.GetData(3);
            ChooserPosition[playerID].y = (float)msg.GetData(4);
        }
        public void LockStatusReceiver(Message msg)
        {
            locking[(int)msg.GetData(0)] = (bool)msg.GetData(1);
        }

        public void tmpTargetDataReceiver(Message msg)
        {
            int playerID = (int)msg.GetData(0);
            int targetIndex = (int)msg.GetData(1);
            float targetDistance = (float)msg.GetData(2);
            DataManager.Instance.TargetData[playerID].targets[targetIndex].distance = targetDistance;
            if (targetDistance == 0)
            {
                DataManager.Instance.TargetData[playerID].targets[targetIndex].hasObject = false;
            }
            else
            {
                DataManager.Instance.TargetData[playerID].targets[targetIndex].hasObject = true;
            }
        }
        public void CleartmpTargetData(int playerID)
        {
            for (int i = 0; i < 100; i++)
            {
                DataManager.Instance.TargetData[playerID].targets[i] = new Target();
            }
        }


    }
    class CCDataReceiver : SingleInstance<CCDataReceiver>
    {
        public override string Name { get; } = "CC Data Receiver";
        public void RadarOnGUIMsgReceiver(Message msg)
        {
            CCData.Instance.OnGuiTarget[(int)msg.GetData(0)] = (Vector3)msg.GetData(1);
        }
        public void BlackoutMsgReceiver(Message msg)
        {
            CCData.Instance.BlackoutData[(int)msg.GetData(0)] = (float)msg.GetData(1);
        }
    }
    // data
    class CCData : SingleInstance<CCData>
    {
        public override string Name { get; } = "CC Data";
        public Vector3[] OnGuiTarget = new Vector3[16];
        public float[] BlackoutData = new float[16];

    }
    class CC2RadarDisplayerData : SingleInstance<CC2RadarDisplayerData>
    {
        public override string Name { get; } = "CC2RadarDisplayer Data";
        public bool[] locked = new bool[16];
        public Vector2[] ChooserPosition = new Vector2[16];
        public float[] ClosingRate = new float[16];
        public float[] currRegion = new float[16];
        public float[] pitch = new float[16];
        public float[] leftAngle = new float[16];
        public float[] rightAngle = new float[16];


        public CC2RadarDisplayerData()
        {
            for (int i = 0; i < 16; i++)
            {
                locked[i] = false;
                ChooserPosition[i] = Vector2.zero;
                ClosingRate[i] = 0f;
                currRegion[i] = 0f;
                pitch[i] = 0f;
            }
        }
    }
    class CC2LoadDisplayerData : SingleInstance<CC2LoadDisplayerData>
    {
        public override string Name { get; } = "CC2LoadDisplayer Data";
        public int[] BulletsLeft = new int[16];
        public int[] FlareLeft = new int[16];
        public int[] ChaffLeft = new int[16];
        public bool[] noMachineGun = new bool[16];
        public bool[] noFlare = new bool[16];
        public bool[] noChaff = new bool[16];
        public List<LoadDataManager.WeaponLoad>[] leftWingLoad = new List<LoadDataManager.WeaponLoad>[16];
        public List<LoadDataManager.WeaponLoad>[] rightWingLoad = new List<LoadDataManager.WeaponLoad>[16];
        public bool[] LoadListReady = new bool[16];

        public CC2LoadDisplayerData()
        {
            for (int i = 0; i < 16; i++)
            {
                leftWingLoad[i] = new List<LoadDataManager.WeaponLoad>();
                rightWingLoad[i] = new List<LoadDataManager.WeaponLoad>();
                noMachineGun[i] = true;
                noFlare[i] = true;
                noChaff[i] = true;
            }
        }


    }
    class CC2NavDisplayerData : SingleInstance<CC2NavDisplayerData>
    {
        public override string Name { get; } = "CC2NavDisplayer Data";
        public Vector3[][] dist = new Vector3[16][];
        public bool[][] hasWP = new bool[16][];
        public string[][] WPName = new string[16][];
        public float[] orientation = new float[16];
        public Vector3[] myPosition = new Vector3[16];

        public bool[] ScaleIncPressed = new bool[16];
        public bool[] ScaleDecPressed = new bool[16];
        public bool[] ChangeSelection = new bool[16];

        public CC2NavDisplayerData()
        {
            for (int i = 0; i < 16; i++)
            {
                dist[i] = new Vector3[8];
                hasWP[i] = new bool[8];
                WPName[i] = new string[8];
            }
        }
 
    }
    // assist controller
    public class KneeboardController : MonoBehaviour
    {
        public int myPlayerID;

        public GameObject canvas;
        public float size;
        Vector3 appearPosition;
        Vector3 disappearPosition;
        public bool boardOn = false;

        public GameObject customText;
        public KeyCode RadarPU;
        public KeyCode RadarPD;
        public KeyCode RadarSI;
        public KeyCode RadarSD;
        public KeyCode RadarLock;
        public KeyCode RadarTDCU;
        public KeyCode RadarTDCD;
        public KeyCode RadarTDCL;
        public KeyCode RadarTDCR;
        public KeyCode A2GLock;
        public KeyCode A2GTrack;
        public KeyCode A2GZI;
        public KeyCode A2GZO;
        public KeyCode A2GPU;
        public KeyCode A2GPD;
        public KeyCode A2GYL;
        public KeyCode A2GYR;
        public KeyCode SRAAM;
        public KeyCode MRAAM;
        public KeyCode AGM;
        public KeyCode GBU;
        public KeyCode NavSwitch;
        public KeyCode NavScaleInc;
        public KeyCode NavScaleDec;
        public string ownNotes;
        public int FontSize = 28;

        public void InitText()
        {
            customText.transform.Find("RadarPitchUp").gameObject.GetComponent<Text>().text = KeyText(RadarPU);
            customText.transform.Find("RadarPitchDown").gameObject.GetComponent<Text>().text = KeyText(RadarPD); ;
            customText.transform.Find("RadarScanInc").gameObject.GetComponent<Text>().text = KeyText(RadarSI); ;
            customText.transform.Find("RadarScanDec").gameObject.GetComponent<Text>().text = KeyText(RadarSD);
            customText.transform.Find("RadarLock").gameObject.GetComponent<Text>().text = KeyText(RadarLock);
            customText.transform.Find("TDCUp").gameObject.GetComponent<Text>().text = KeyText(RadarTDCU);
            customText.transform.Find("TDCDown").gameObject.GetComponent<Text>().text = KeyText(RadarTDCD);
            customText.transform.Find("TDCLeft").gameObject.GetComponent<Text>().text = KeyText(RadarTDCL);
            customText.transform.Find("TDCRight").gameObject.GetComponent<Text>().text = KeyText(RadarTDCR);
            customText.transform.Find("A2GLock").gameObject.GetComponent<Text>().text = KeyText(A2GLock);
            customText.transform.Find("A2GTrack").gameObject.GetComponent<Text>().text = KeyText(A2GTrack);
            customText.transform.Find("A2GZoomIn").gameObject.GetComponent<Text>().text = KeyText(A2GZI);
            customText.transform.Find("A2GZoomOut").gameObject.GetComponent<Text>().text = KeyText(A2GZO);
            customText.transform.Find("A2GPitchUp").gameObject.GetComponent<Text>().text = KeyText(A2GPU);
            customText.transform.Find("A2GPitchDown").gameObject.GetComponent<Text>().text = KeyText(A2GPD);
            customText.transform.Find("A2GYawLeft").gameObject.GetComponent<Text>().text = KeyText(A2GYL);
            customText.transform.Find("A2GYawRight").gameObject.GetComponent<Text>().text = KeyText(A2GYR);
            customText.transform.Find("SRAAM").gameObject.GetComponent<Text>().text = KeyText(SRAAM);
            customText.transform.Find("MRAAM").gameObject.GetComponent<Text>().text = KeyText(MRAAM);
            customText.transform.Find("AGM").gameObject.GetComponent<Text>().text = KeyText(AGM);
            customText.transform.Find("GBU").gameObject.GetComponent<Text>().text = KeyText(GBU);
            customText.transform.Find("NavSwitch").gameObject.GetComponent<Text>().text = KeyText(NavSwitch);
            customText.transform.Find("NavScaleInc").gameObject.GetComponent<Text>().text = KeyText(NavScaleInc);
            customText.transform.Find("NavScaleDec").gameObject.GetComponent<Text>().text = KeyText(NavScaleDec);

            for (int i = 0; i < 8; i++)
            {
                if (CC2NavDisplayerData.Instance.hasWP[myPlayerID][i])
                {
                    customText.transform.Find("WPName" + i.ToString()).gameObject.GetComponent<Text>().text = CC2NavDisplayerData.Instance.WPName[myPlayerID][i];
                    customText.transform.Find("WP" + i.ToString()).gameObject.GetComponent<Text>().text = CC2NavDisplayerData.Instance.dist[myPlayerID][i].ToString("f0");
                }
                else
                {
                    customText.transform.Find("WPName" + i.ToString()).gameObject.GetComponent<Text>().text = "";
                    customText.transform.Find("WP" + i.ToString()).gameObject.GetComponent<Text>().text = "";
                }
                
            }

        }

        public string KeyText(KeyCode key)
        {
            if (key.ToString() == "LeftArrow")
            {
                return "←";
            }
            if (key.ToString() == "RightArrow")
            {
                return "→";
            }
            if (key.ToString() == "UpArrow")
            {
                return "↑";
            }
            if (key.ToString() == "DownArrow")
            {
                return "↓";
            }

            return key.ToString();
        }

        public void Start()
        {
            canvas = transform.parent.gameObject;
            transform.Find("woodbase").gameObject.SetActive(true);
            transform.Find("paperBase").gameObject.SetActive(true);
            float height = canvas.transform.localPosition.y * 2;
            float width = canvas.transform.localPosition.x * 2;
            transform.position = new Vector3(width - transform.localScale.x * 340 - 20, -transform.localScale.y * 490 - 20, 0);
            customText = transform.Find("paperBase").Find("custom").gameObject;
            InitText();
        }
        public void Update()
        {
            float height = canvas.transform.localPosition.y * 2;
            float width = canvas.transform.localPosition.x * 2;
            transform.localScale = height / 980 * Vector3.one * size;
            appearPosition = new Vector3(width - transform.localScale.x * 340 - 20, transform.localScale.y * 490 + 20, 0);
            disappearPosition = new Vector3(width - transform.localScale.x * 340 - 20, -transform.localScale.y * 490 - 20, 0);
            if (boardOn)
            {
                transform.position = Vector3.Lerp(transform.position, appearPosition, 0.2f);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, disappearPosition, 0.3f);
            }
        }
        public void FixedUpdate()
        {

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
    //simulator
    public class RadarDisplayerSimulator : MonoBehaviour
    {
        public int myPlayerID = 0;
        public bool isClient = false;
        // for RadarDisplayer
        public ScanLineController SLController;
        public Target[] RadarTarget;
        public IEnumerator sendPanelMsg;
        public static MessageType ClientTargetPositionMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Integer, DataType.Vector3);
        public static MessageType ClientTargetDistanceMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Integer, DataType.Single);
        //playerID, pitch, deltaScanAngle, ChooserPosition, SLcurrAngle, SLcurrDirection
        public static MessageType ClientNormalPanelMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Single, DataType.Single,
                                                                                            DataType.Single, DataType.Single,
                                                                                            DataType.Single, DataType.Boolean);
        //playerID, pitch, closingRate, ChooserPosition, 
        public static MessageType ClientLockPanelMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Single, DataType.Single,
                                                                                            DataType.Single, DataType.Single);

        // lock status
        public static MessageType ClientLockStatusMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Boolean);

        //tmp target data
        // playerID, targetIndex, distance, hasObject
        public static MessageType ClientTmpTargetData = ModNetworking.CreateMessageType(DataType.Integer, DataType.Integer, DataType.Single, DataType.Boolean);

        public static MessageType ClientOnGuiTargetMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Vector3);

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

        public bool LockPressed = false;
        public float ScanRegionAfterLock = 60f;

        public int currRegion;
        public bool locking; //whether the radar keeps tracking an object
        public int lockRegion = 0;
        public float deltaPitch = 0;
        public int currLockedPlayerID = -1;
        public float closingRate = 0;
        public Vector3 OnGuiTargetPosition;
        public displayerData DisplayerData = new displayerData(0, 0);
        public RadarTargetData sendBVRData = new RadarTargetData();
        public RadarTargetData BVRData = new RadarTargetData();

        //msg sender
        public IEnumerator NormalClock;
        public bool NormalClockhit = false;
        public IEnumerator LockClock;
        public bool LockClockhit = false;

        public int iconSize = 28;
        protected Texture LockIconOnScreen;


        IEnumerator SendClockNormal(float time)
        {
            while (true)
            {
                yield return new WaitForSeconds(time);
                NormalClockhit = true;
            }
        }
        IEnumerator SendClockLock(float time)
        {
            while (true)
            {
                yield return new WaitForSeconds(time);
                LockClockhit = true;
            }
        }
        public void SendNormalPanelMsg() // call in host
        {
            if (NormalClockhit)
            {
                NormalClockhit = false;
                ModNetworking.SendToAll(ClientNormalPanelMsg.CreateMessage(myPlayerID, radarPitch, deltaScanAngle,
                                                                            ChooserPosition.x, ChooserPosition.y,
                                                                            SLController.currAngle, SLController.direction));
            }
        }
        public void SendLockPanelMsg() // call in host
        {
            if (LockClockhit)
            {
                LockClockhit = false;
                ModNetworking.SendToAll(ClientLockPanelMsg.CreateMessage(myPlayerID, radarPitch, closingRate,
                                                                            ChooserPosition.x, ChooserPosition.y));
            }
        }
        public void SendLockStatusMsg()
        {
            ModNetworking.SendToAll(ClientLockStatusMsg.CreateMessage(myPlayerID, locking));
        }
        public void ClientSyncHostNormalPanel()
        {
            if (RadarDisplayerSimulator_MsgReceiver.Instance.NormalUpdated[myPlayerID])
            {
                RadarDisplayerSimulator_MsgReceiver.Instance.NormalUpdated[myPlayerID] = false;
                radarPitch = RadarDisplayerSimulator_MsgReceiver.Instance.pitch[myPlayerID];
                deltaScanAngle = RadarDisplayerSimulator_MsgReceiver.Instance.deltaScanAngle[myPlayerID];
                ChooserPosition.x = RadarDisplayerSimulator_MsgReceiver.Instance.ChooserPosition[myPlayerID].x;
                ChooserPosition.y = RadarDisplayerSimulator_MsgReceiver.Instance.ChooserPosition[myPlayerID].y;
                SLController.currAngle = RadarDisplayerSimulator_MsgReceiver.Instance.SLcurrAngle[myPlayerID];
                SLController.direction = RadarDisplayerSimulator_MsgReceiver.Instance.SLcurrDirection[myPlayerID];
            }

        }
        public void ClientSyncHostLockPanel()
        {
            if (RadarDisplayerSimulator_MsgReceiver.Instance.LockUpdated[myPlayerID])
            {
                RadarDisplayerSimulator_MsgReceiver.Instance.LockUpdated[myPlayerID] = false;
                radarPitch = RadarDisplayerSimulator_MsgReceiver.Instance.pitch[myPlayerID];
                closingRate = RadarDisplayerSimulator_MsgReceiver.Instance.closingRate[myPlayerID];
                ChooserPosition.x = RadarDisplayerSimulator_MsgReceiver.Instance.ChooserPosition[myPlayerID].x;
                ChooserPosition.y = RadarDisplayerSimulator_MsgReceiver.Instance.ChooserPosition[myPlayerID].y;
            }
        }
        public void ClientSyncHostLockStatus()
        {
            locking = RadarDisplayerSimulator_MsgReceiver.Instance.locking[myPlayerID];
        }
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
        public void SyncRadarTarget()
        {
            RadarTarget = DataManager.Instance.TargetData[myPlayerID].targets;
        }
        public void SendRadarPara()
        {
            displayerData DisplayerData = new displayerData(0, 0);
            DisplayerData.radarPitch = radarPitch;
            DisplayerData.radarAngle = SLController.currAngle;
            DataManager.Instance.DisplayerData[myPlayerID] = DisplayerData;
        }
        public void SendRadarScreenPara()
        {
            CC2RadarDisplayerData.Instance.locked[myPlayerID] = locking;
            CC2RadarDisplayerData.Instance.ChooserPosition[myPlayerID] = ChooserPosition;
            CC2RadarDisplayerData.Instance.currRegion[myPlayerID] = currRegion;
            CC2RadarDisplayerData.Instance.pitch[myPlayerID] = radarPitch;
            CC2RadarDisplayerData.Instance.leftAngle[myPlayerID] = leftScanAngle;
            CC2RadarDisplayerData.Instance.rightAngle[myPlayerID] = rightScanAngle;
            if (locking)
            {
                CC2RadarDisplayerData.Instance.ClosingRate[myPlayerID] = closingRate;
            }
            else
            {
                CC2RadarDisplayerData.Instance.ClosingRate[myPlayerID] = 0;
            }
        }
        public void SendBVRData()// before Sync RadarTargetData && after FindTarget
        {
            if (locking)
            {
                BVRData.position = RadarTarget[lockRegion].position;
                BVRData.velocity = RadarTarget[lockRegion].velocity;
            }
            else
            {
                BVRData.position = Vector3.zero;
                BVRData.velocity = Vector3.zero;
            }
            DataManager.Instance.BVRData[myPlayerID] = BVRData;
        }
        public void ClearOtherTargets()
        {
            float leftRegion = (leftScanAngle + 60) / 1.2f;
            float rightRegion = (rightScanAngle + 60) / 1.2f;
            for (int i = 0; i < leftRegion; i++)
            {
                DataManager.Instance.TargetData[myPlayerID].RemoveTarget(i);
            }
            for (int i = 100; i > rightRegion; i--)
            {
                DataManager.Instance.TargetData[myPlayerID].RemoveTarget(i);
            }
        }
        public bool FindLockedTarget()
        {
            bool res = false;

            for (int i = 0; i < 15; i++)
            {
                if (lockRegion - i >= 0 && lockRegion + i <= 100)
                {
                    if (RadarTarget[lockRegion + i].hasObject && !RadarTarget[lockRegion + i].isMissle && (currLockedPlayerID == -1 || currLockedPlayerID == RadarTarget[lockRegion + i].playerID))
                    {
                        currLockedPlayerID = RadarTarget[lockRegion + i].playerID;
                        lockRegion = lockRegion + i;
                        res = true;
                    }
                    else if (RadarTarget[lockRegion - i].hasObject && !RadarTarget[lockRegion - i].isMissle && (currLockedPlayerID == -1 || currLockedPlayerID == RadarTarget[lockRegion - i].playerID))
                    {
                        currLockedPlayerID = RadarTarget[lockRegion - i].playerID;
                        lockRegion = lockRegion - i;
                        res = true;
                    }

                    if (res)
                    {
                        ChooserPosition.x = (lockRegion - 50) * 1.2f;
                        ChooserPosition.y = (RadarTarget[lockRegion].distance - 6000) * 0.01f;

                        radarPitch = RadarTarget[lockRegion].pitch;
                        closingRate = RadarTarget[lockRegion].closingRate;
                        CCData.Instance.OnGuiTarget[myPlayerID] = RadarTarget[lockRegion].position;
                        if (StatMaster.isMP)
                        {
                            ModNetworking.SendToAll(ClientOnGuiTargetMsg.CreateMessage(myPlayerID, CCData.Instance.OnGuiTarget[myPlayerID]));
                        }

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

        public void SendTargetData()
        {
            ModNetworking.SendToAll(ClientTmpTargetData.CreateMessage(myPlayerID, currRegion, RadarTarget[currRegion].distance, RadarTarget[currRegion].hasObject));
        }


        public void Start()
        {
            LockIconOnScreen = ModResource.GetTexture("LockIconScreen Texture").Texture;
            InitCCRadarDisplayer();
            NormalClock = SendClockNormal(0.5f);
            StartCoroutine(NormalClock);
            LockClock = SendClockLock(0.05f);
            StartCoroutine(LockClock);
        }
        public void OnDestry()
        {
            if (isClient)
            {
                DataManager.Instance.TargetData[myPlayerID] = new targetManager();
            }

        }
        public void Update()
        {
            if (!isClient)
            {
                SyncSLcontroller();
                if (LockPressed)
                {
                    LockPressed = false;
                    currLockedPlayerID = -1;
                    locking = !locking;
                    if (locking)
                    {
                        deltaScanAngle = ScanRegionAfterLock;
                        NormalClockhit = true;
                    }
                    else
                    {
                        deltaScanAngle = 60;
                        radarPitch = 0f;
                        NormalClockhit = true;
                    }
                }
            }
            else
            {
                SyncSLcontroller();
            }

        }
        public void FixedUpdate()
        {
            if (!isClient)
            {
                SyncRadarTarget();
                //SendTargetData();
                // update region
                currRegion = (int)Math.Floor((SLController.currAngle + 60) / 1.2f + 0.5f); // scan line
                lockRegion = (int)Math.Floor((ChooserPosition.x + 60) / 1.2f + 0.5f); // TDC

                if (!locking)
                {
                    UnlockedChooserMotion_FixedUpdateHost();
                    UnlockedPitchMotion_FixedUpdateHost();
                }
                else
                {
                    if (!FindLockedTarget())
                    {
                        currLockedPlayerID = -1;
                        deltaScanAngle = 60f;
                        radarPitch = 0f;
                        locking = false;
                    }
                    // send to client
                    SendLockPanelMsg();
                }
                ClearOtherTargets();
                SendLockStatusMsg();
                AdjustScanAngle_FixedUpdate();
                // tell radar
                SendRadarPara();
                // tell radar displayer
                SendRadarScreenPara();
                // send BVR
                SendBVRData();

                // send to client
                SendNormalPanelMsg();

            }
            else    // for client
            {
                SyncRadarTarget();
                ClientSyncHostNormalPanel();
                ClientSyncHostLockStatus();
                ClientSyncHostLockPanel();

                currRegion = (int)Math.Floor((SLController.currAngle + 60) / 1.2f + 0.5f); // scan line
                lockRegion = (int)Math.Floor((ChooserPosition.x + 60) / 1.2f + 0.5f); // TDC
                if (!locking)
                {
                    UnlockedChooserMotion_FixedUpdateHost();
                    UnlockedPitchMotion_FixedUpdateHost();
                }
                else
                {

                }
                ClearOtherTargets();
                AdjustScanAngle_FixedUpdate();
                SendRadarPara();
                SendRadarScreenPara();
            }


        }
        private void OnGUI()
        {
            if (locking)
            {
                if (StatMaster.isMP)
                {
                    if (PlayerData.localPlayer.networkId != myPlayerID)
                    {
                        return;
                    }
                }
                GUI.color = Color.green;
                Vector3 onScreenPosition = Camera.main.WorldToScreenPoint(CCData.Instance.OnGuiTarget[myPlayerID]);
                if (onScreenPosition.z >= 0)
                    GUI.DrawTexture(new Rect(onScreenPosition.x - iconSize / 2, Camera.main.pixelHeight - onScreenPosition.y - iconSize / 2, iconSize, iconSize), LockIconOnScreen);
            }
        }

    }
    public class A2GDisplayerSimulator : MonoBehaviour
    {
        public int myPlayerID = 0;
        public bool isClient = false;
        //key
        public bool LockPressed;
        public bool ZoomInPressed;
        public bool ZoomOutPressed;
        public bool PitchUpPressed;
        public bool PitchDownPressed;
        public bool YawLeftPressed;
        public bool YawRightPressed;
        public bool TrackPressed;
        public int iconSize = 24;
        public Texture LockIcon;

        public static MessageType ClientTrackMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Boolean);
        public void Start()
        {
            LockIcon = ModResource.GetTexture("HUDA2GAim Texture").Texture;
        }
        public void Update()
        {
            if (LockPressed)
            {
                DataManager.Instance.TV_Lock[myPlayerID] = !DataManager.Instance.TV_Lock[myPlayerID];
                LockPressed = false;
            }
            if (TrackPressed)
            {
                TrackPressed = false;
                if (DataManager.Instance.TV_Lock[myPlayerID])
                {
                    DataManager.Instance.TV_Track[myPlayerID] = !DataManager.Instance.TV_Track[myPlayerID];
                    ModNetworking.SendToAll(ClientTrackMsg.CreateMessage(myPlayerID, DataManager.Instance.TV_Track[myPlayerID]));
                }
            }
            if (!DataManager.Instance.TV_Lock[myPlayerID])
            {
                DataManager.Instance.TV_Track[myPlayerID] = false;
                ModNetworking.SendToAll(ClientTrackMsg.CreateMessage(myPlayerID, DataManager.Instance.TV_Track[myPlayerID]));
            }
        }
        public void FixedUpdate()
        {
            if (YawLeftPressed)
            {
                DataManager.Instance.TV_LeftRight[myPlayerID] = -1;
            }
            else if (YawRightPressed)
            {
                DataManager.Instance.TV_LeftRight[myPlayerID] = 1;
            }
            else
            {
                DataManager.Instance.TV_LeftRight[myPlayerID] = 0;
            }

            if (PitchUpPressed)
            {
                DataManager.Instance.TV_UpDown[myPlayerID] = 1;
            }
            else if (PitchDownPressed)
            {
                DataManager.Instance.TV_UpDown[myPlayerID] = -1;
            }
            else
            {
                DataManager.Instance.TV_UpDown[myPlayerID] = 0;
            }

            if (ZoomInPressed)
            {
                DataManager.Instance.TV_FOV[myPlayerID] *= 0.98f;
            }
            else if (ZoomOutPressed)
            {
                DataManager.Instance.TV_FOV[myPlayerID] /= 0.98f;
            }
            DataManager.Instance.TV_FOV[myPlayerID] = Mathf.Clamp(DataManager.Instance.TV_FOV[myPlayerID], 0.5f, 40);
        }
        void OnGUI()
        {
            //GUI.Box(new Rect(100, 200, 200, 50), DataManager.Instance.EO_ThermalOn[myPlayerID].ToString());
            //GUI.Box(new Rect(100, 300, 200, 50), DataManager.Instance.EO_InverseThermal[myPlayerID].ToString());
            //GUI.Box(new Rect(100, 400, 200, 50), FOV.ToString());
            if (DataManager.Instance.TV_Lock[myPlayerID])
            {
                if (StatMaster.isMP)
                {
                    if (PlayerData.localPlayer.networkId != myPlayerID)
                    {
                        return;
                    }
                }
                GUI.color = Color.green;
                Vector3 onScreenPosition = Camera.main.WorldToScreenPoint(DataManager.Instance.A2G_TargetData[myPlayerID].position);
                if (onScreenPosition.z >= 0)
                    GUI.DrawTexture(new Rect(onScreenPosition.x - iconSize / 2, Camera.main.pixelHeight - onScreenPosition.y - iconSize / 2, iconSize, iconSize), LockIcon);
            }
        }
    }
    public class LoadDisplayerSimulator : MonoBehaviour
    {
        public int myPlayerID;
        public bool isClient;
        public bool initialized = false;

        public Plane selfPlane;

        public bool LaunchSRAAM;
        public bool LaunchMRAAM;
        public bool LaunchAGM;
        public bool LaunchGBU;
        public float Region;
        public float Offset;

        public MKey KeyToBeEmulated;
        public bool hasEmulateKey;

        public bool noMachineGun = true;
        public bool noFlare = true;
        public bool noChaff = true;


        public List<LoadDataManager.WeaponLoad> leftWingLoad = new List<LoadDataManager.WeaponLoad>();
        public List<LoadDataManager.WeaponLoad> rightWingLoad = new List<LoadDataManager.WeaponLoad>();


        public int BulletsLeft;
        public int FlareLeft;
        public int ChaffLeft;

        public int leftRemain = 0;
        public int rightRemain = 0;

        public void SendLoadDisplayerData()
        {
            CC2LoadDisplayerData.Instance.LoadListReady[myPlayerID] = true;
            CC2LoadDisplayerData.Instance.leftWingLoad[myPlayerID] = leftWingLoad;
            CC2LoadDisplayerData.Instance.rightWingLoad[myPlayerID] = rightWingLoad;
            CC2LoadDisplayerData.Instance.noMachineGun[myPlayerID] = noMachineGun;
            CC2LoadDisplayerData.Instance.noFlare[myPlayerID] = noFlare;
            CC2LoadDisplayerData.Instance.noChaff[myPlayerID] = noChaff;
            CC2LoadDisplayerData.Instance.BulletsLeft[myPlayerID] = BulletsLeft;
            CC2LoadDisplayerData.Instance.FlareLeft[myPlayerID] = FlareLeft;
            CC2LoadDisplayerData.Instance.ChaffLeft[myPlayerID] = ChaffLeft;
        }

        public void InitLoad() // call in simlulate update
        {
            LoadDataManager.Instance.InitLoad(myPlayerID);
            // filter the weapons out of region
            Dictionary<int, LoadDataManager.WeaponLoad> filtered = new Dictionary<int, LoadDataManager.WeaponLoad>();
            foreach (var weaponLoad in LoadDataManager.Instance.Weapons[myPlayerID])
            {
                if ((weaponLoad.Value.weaponTransform.position - transform.position).sqrMagnitude <= Mathf.Pow(Region, 2))
                {
                    filtered.Add(weaponLoad.Key, weaponLoad.Value);
                }
            }
            selfPlane = new Plane(transform.right, transform.position + transform.right * Offset);
            var sorted = from pair in filtered orderby Mathf.Abs(selfPlane.GetDistanceToPoint(pair.Value.weaponTransform.position)) ascending select pair;
            foreach (var pair in sorted)
            {
                if (selfPlane.GetDistanceToPoint(pair.Value.weaponTransform.position) <= 0)
                {
                    leftWingLoad.Add(pair.Value);

                }
                else
                {
                    rightWingLoad.Add(pair.Value);
                }
            }
            leftRemain = leftWingLoad.Count;
            rightRemain = rightWingLoad.Count;
            //Debug.Log("Left:");
            //foreach (var weapon in leftWingLoad)
            //{
            //    Debug.Log(weapon.weapon);
            //}
            //Debug.Log("right:");
            //foreach (var weapon in rightWingLoad)
            //{
            //    Debug.Log(weapon.weapon);
            //}
        }

        public void myEmulateKey(MKey Key, bool t)
        {
            KeyToBeEmulated = Key;
            hasEmulateKey = true;
        }
        public void finishEmulate()
        {
            hasEmulateKey = false;
        }
        public void LaunchWeapon() // before updateLoadAndIcon
        {
            //sraam
            if (LaunchSRAAM)
            {
                LaunchSRAAM = false;
                if (leftRemain >= rightRemain)
                {
                    if (leftRemain == 0)
                    {
                        return;
                    }
                    for (int i = leftWingLoad.Count - 1; i >= 0; i--)
                    {
                        if (leftWingLoad[i].weapon == LoadDataManager.WeaponType.SRAAM && leftWingLoad[i].released == false)
                        {
                            myEmulateKey(leftWingLoad[i].weaponTransform.gameObject.GetComponent<SRAAMBlock>().Launch, true);
                            return;
                        }
                    }
                    if (rightRemain == 0)
                    {
                        return;
                    }
                    for (int i = rightWingLoad.Count - 1; i >= 0; i--)
                    {
                        if (rightWingLoad[i].weapon == LoadDataManager.WeaponType.SRAAM && rightWingLoad[i].released == false)
                        {
                            myEmulateKey(rightWingLoad[i].weaponTransform.gameObject.GetComponent<SRAAMBlock>().Launch, true);
                            return;
                        }
                    }
                }
                else
                {
                    if (rightRemain == 0)
                    {
                        return;
                    }
                    for (int i = rightWingLoad.Count - 1; i >= 0; i--)
                    {
                        if (rightWingLoad[i].weapon == LoadDataManager.WeaponType.SRAAM && rightWingLoad[i].released == false)
                        {
                            myEmulateKey(rightWingLoad[i].weaponTransform.gameObject.GetComponent<SRAAMBlock>().Launch, true);
                            return;
                        }
                    }
                    if (leftRemain == 0)
                    {
                        return;
                    }
                    for (int i = leftWingLoad.Count - 1; i >= 0; i--)
                    {
                        if (leftWingLoad[i].weapon == LoadDataManager.WeaponType.SRAAM && leftWingLoad[i].released == false)
                        {
                            myEmulateKey(leftWingLoad[i].weaponTransform.gameObject.GetComponent<SRAAMBlock>().Launch, true);
                            return;
                        }
                    }
                }
            }
            //mraam
            if (LaunchMRAAM)
            {
                LaunchMRAAM = false;
                if (leftRemain >= rightRemain)
                {
                    if (leftRemain == 0)
                    {
                        return;
                    }
                    for (int i = leftWingLoad.Count - 1; i >= 0; i--)
                    {
                        if (leftWingLoad[i].weapon == LoadDataManager.WeaponType.MRAAM && leftWingLoad[i].released == false)
                        {
                            myEmulateKey(leftWingLoad[i].weaponTransform.gameObject.GetComponent<MRAAMBlock>().Launch, true);
                            return;
                        }
                    }
                    if (rightRemain == 0)
                    {
                        return;
                    }
                    for (int i = rightWingLoad.Count - 1; i >= 0; i--)
                    {
                        if (rightWingLoad[i].weapon == LoadDataManager.WeaponType.MRAAM && rightWingLoad[i].released == false)
                        {
                            myEmulateKey(rightWingLoad[i].weaponTransform.gameObject.GetComponent<MRAAMBlock>().Launch, true);
                            return;
                        }
                    }
                }
                else
                {
                    if (rightRemain == 0)
                    {
                        return;
                    }
                    for (int i = rightWingLoad.Count - 1; i >= 0; i--)
                    {
                        if (rightWingLoad[i].weapon == LoadDataManager.WeaponType.MRAAM && rightWingLoad[i].released == false)
                        {
                            myEmulateKey(rightWingLoad[i].weaponTransform.gameObject.GetComponent<MRAAMBlock>().Launch, true);
                            return;
                        }
                    }
                    if (leftRemain == 0)
                    {
                        return;
                    }
                    for (int i = leftWingLoad.Count - 1; i >= 0; i--)
                    {
                        if (leftWingLoad[i].weapon == LoadDataManager.WeaponType.MRAAM && leftWingLoad[i].released == false)
                        {
                            myEmulateKey(leftWingLoad[i].weaponTransform.gameObject.GetComponent<MRAAMBlock>().Launch, true);
                            return;
                        }
                    }
                }
            }
            //agm
            if (LaunchAGM)
            {
                LaunchAGM = false;
                if (leftRemain >= rightRemain)
                {
                    if (leftRemain == 0)
                    {
                        return;
                    }
                    for (int i = leftWingLoad.Count - 1; i >= 0; i--)
                    {
                        if (leftWingLoad[i].weapon == LoadDataManager.WeaponType.AGM && leftWingLoad[i].released == false)
                        {
                            myEmulateKey(leftWingLoad[i].weaponTransform.gameObject.GetComponent<AGMBlock>().Launch, true);
                            return;
                        }
                    }
                    if (rightRemain == 0)
                    {
                        return;
                    }
                    for (int i = rightWingLoad.Count - 1; i >= 0; i--)
                    {
                        if (rightWingLoad[i].weapon == LoadDataManager.WeaponType.AGM && rightWingLoad[i].released == false)
                        {
                            myEmulateKey(rightWingLoad[i].weaponTransform.gameObject.GetComponent<AGMBlock>().Launch, true);
                            return;
                        }
                    }
                }
                else
                {
                    if (rightRemain == 0)
                    {
                        return;
                    }
                    for (int i = rightWingLoad.Count - 1; i >= 0; i--)
                    {
                        if (rightWingLoad[i].weapon == LoadDataManager.WeaponType.AGM && rightWingLoad[i].released == false)
                        {
                            myEmulateKey(rightWingLoad[i].weaponTransform.gameObject.GetComponent<AGMBlock>().Launch, true);
                            return;
                        }
                    }
                    if (leftRemain == 0)
                    {
                        return;
                    }
                    for (int i = leftWingLoad.Count - 1; i >= 0; i--)
                    {
                        if (leftWingLoad[i].weapon == LoadDataManager.WeaponType.AGM && leftWingLoad[i].released == false)
                        {
                            myEmulateKey(leftWingLoad[i].weaponTransform.gameObject.GetComponent<AGMBlock>().Launch, true);
                            return;
                        }
                    }
                }
            }
            //gbu
            if (LaunchGBU)
            {
                LaunchGBU = false;
                if (leftRemain >= rightRemain)
                {
                    if (leftRemain == 0)
                    {
                        return;
                    }
                    for (int i = leftWingLoad.Count - 1; i >= 0; i--)
                    {
                        if (leftWingLoad[i].weapon == LoadDataManager.WeaponType.GBU && leftWingLoad[i].released == false)
                        {
                            myEmulateKey(leftWingLoad[i].weaponTransform.gameObject.GetComponent<GuidedBombBlock>().Launch, true);
                            return;
                        }
                    }
                    if (rightRemain == 0)
                    {
                        return;
                    }
                    for (int i = rightWingLoad.Count - 1; i >= 0; i--)
                    {
                        if (rightWingLoad[i].weapon == LoadDataManager.WeaponType.GBU && rightWingLoad[i].released == false)
                        {
                            myEmulateKey(rightWingLoad[i].weaponTransform.gameObject.GetComponent<GuidedBombBlock>().Launch, true);
                            return;
                        }
                    }
                }
                else
                {
                    if (rightRemain == 0)
                    {
                        return;
                    }
                    for (int i = rightWingLoad.Count - 1; i >= 0; i--)
                    {
                        if (rightWingLoad[i].weapon == LoadDataManager.WeaponType.GBU && rightWingLoad[i].released == false)
                        {
                            myEmulateKey(rightWingLoad[i].weaponTransform.gameObject.GetComponent<GuidedBombBlock>().Launch, true);
                            return;
                        }
                    }
                    if (leftRemain == 0)
                    {
                        return;
                    }
                    for (int i = leftWingLoad.Count - 1; i >= 0; i--)
                    {
                        if (leftWingLoad[i].weapon == LoadDataManager.WeaponType.GBU && leftWingLoad[i].released == false)
                        {
                            myEmulateKey(leftWingLoad[i].weaponTransform.gameObject.GetComponent<GuidedBombBlock>().Launch, true);
                            return;
                        }
                    }
                }
            }
        }
        public void updateLoad()
        {
            for (int i = 0; i < leftWingLoad.Count; i++)
            {
                SRAAMBlock.status currStatus = SRAAMBlock.status.stored;
                switch (leftWingLoad[i].weapon)
                {
                    case LoadDataManager.WeaponType.SRAAM:
                        currStatus = leftWingLoad[i].weaponTransform.GetComponent<SRAAMBlock>().myStatus;
                        break;
                    case LoadDataManager.WeaponType.MRAAM:
                        currStatus = leftWingLoad[i].weaponTransform.GetComponent<MRAAMBlock>().myStatus;
                        break;
                    case LoadDataManager.WeaponType.AGM:
                        currStatus = leftWingLoad[i].weaponTransform.GetComponent<AGMBlock>().myStatus;
                        break;
                    case LoadDataManager.WeaponType.GBU:
                        currStatus = leftWingLoad[i].weaponTransform.GetComponent<GuidedBombBlock>().myStatus;
                        break;
                    default:
                        break;
                }
                if (currStatus != SRAAMBlock.status.stored)
                {
                    if (leftWingLoad[i].released == false)
                    {
                        leftWingLoad[i].released = true;
                        leftRemain--;
                    }
                }

            }
            for (int i = 0; i < rightWingLoad.Count; i++)
            {
                SRAAMBlock.status currStatus = SRAAMBlock.status.stored;
                switch (rightWingLoad[i].weapon)
                {
                    case LoadDataManager.WeaponType.SRAAM:
                        currStatus = rightWingLoad[i].weaponTransform.GetComponent<SRAAMBlock>().myStatus;
                        break;
                    case LoadDataManager.WeaponType.MRAAM:
                        currStatus = rightWingLoad[i].weaponTransform.GetComponent<MRAAMBlock>().myStatus;
                        break;
                    case LoadDataManager.WeaponType.AGM:
                        currStatus = rightWingLoad[i].weaponTransform.GetComponent<AGMBlock>().myStatus;
                        break;
                    case LoadDataManager.WeaponType.GBU:
                        currStatus = rightWingLoad[i].weaponTransform.GetComponent<GuidedBombBlock>().myStatus;
                        break;
                    default:
                        break;
                }
                if (currStatus != SRAAMBlock.status.stored)
                {
                    if (rightWingLoad[i].released == false)
                    {
                        rightWingLoad[i].released = true;
                        rightRemain--;
                    }
                }

            }
        }
        public void updateMachineGunBullets()// call in LateUpdate
        {
            BulletsLeft = Mathf.Clamp(LoadDataManager.Instance.MachineGunBullets[myPlayerID], 0, 99999);
            LoadDataManager.Instance.ClearMachineGunBullet(myPlayerID);
        }
        public void updateFlareNum()// call in LateUpdate
        {
            FlareLeft = Mathf.Clamp(LoadDataManager.Instance.FlareNum[myPlayerID], 0, 99999);
            LoadDataManager.Instance.ClearFlareNum(myPlayerID);
        }
        public void updateChaffNum()// call in LateUpdate
        {
            ChaffLeft = Mathf.Clamp(LoadDataManager.Instance.ChaffNum[myPlayerID], 0, 99999);
            LoadDataManager.Instance.ClearChaffNum(myPlayerID);
        }
        public void Start()
        {
        }

        public void Update()
        {
            if (!isClient)
            {
                LaunchWeapon();
            }
        }
        public void FixedUpdate()
        {
            if (!initialized)// call one time after OnsimulateStart
            {
                initialized = true;
                InitLoad();
            }
            updateLoad();
        }
        public void LateUpdate()
        {
            try
            {
                updateMachineGunBullets();
                noMachineGun = false;
            }
            catch
            {
                noMachineGun = true;
            }
            try
            {
                updateFlareNum();
                noFlare = false;
            }
            catch
            {
                noFlare = true;
            }
            try
            {
                updateChaffNum();
                noChaff = false;
            }
            catch
            {
                noChaff = true;
            }
            if (initialized)
            {
                SendLoadDisplayerData();
            }

        }
    }
    public class NavDisplayerSimulator : MonoBehaviour
    {
        public Vector3[] dist = new Vector3[8];
        public bool[] hasWP = new bool[8];
        public string[] WPName = new string[8];
        public float orientation = 0;
        public Vector3 myPosition = Vector3.zero;

        public bool ScaleIncPressed;
        public bool ScaleDecPressed;
        public bool ChangeSelectionPressed;

        public void Start()
        {

        }
        public void Update()
        {
            if (ScaleIncPressed)
            {
                ScaleIncPressed = false;
            }
        }
    }

    // camera
    public class PilotCameraController : MonoBehaviour
    {
        public int myPlayerID;

        public bool activeKeyPressed;
        public float defaultFOV = 50f;
        public float minFOV = 20;
        public float maxFOV = 60;
        public float Sensitivity = 1;

        public bool CameraOn;
        public float targetFOV;
        public float originFOV;
        public float rotationX;
        public float rotationY;

        public Camera _viewCamera;
        public Camera _hudCamera;

        public Texture CameraOnIcon;

        public bool IsFixedCameraActive
        {
            get
            {
                return SingleInstance<FixedCameraController>.Instance.activeCamera;
            }
        }
        public Camera MainCamera
        {
            get
            {
                bool flag;
                if (this._viewCamera == null)
                {
                    MouseOrbit instance = SingleInstanceFindOnly<MouseOrbit>.Instance;
                    flag = (((instance != null) ? instance.cam : null) != null);
                }
                else
                {
                    flag = false;
                }
                bool flag2 = flag;
                if (flag2)
                {
                    this._viewCamera = SingleInstanceFindOnly<MouseOrbit>.Instance.cam;
                }
                bool flag3 = this._viewCamera == null;
                if (flag3)
                {
                    this._viewCamera = Camera.main;
                }
                return this._viewCamera;
            }
        }
        public Camera HUDCamera
        {
            get
            {
                bool flag;
                if (this._hudCamera == null)
                {
                    MouseOrbit instance = SingleInstanceFindOnly<MouseOrbit>.Instance;
                    flag = (((instance != null) ? instance.hud3Dcam : null) != null);
                }
                else
                {
                    flag = false;
                }
                bool flag2 = flag;
                if (flag2)
                {
                    this._hudCamera = SingleInstanceFindOnly<MouseOrbit>.Instance.hud3Dcam;
                }
                bool flag3 = this._hudCamera == null;
                if (flag3)
                {
                    this._hudCamera = Camera.main.transform.GetChild(0).GetComponent<Camera>();
                }
                return this._hudCamera;
            }
        }

        public void Awake()
        {
            originFOV = MainCamera.fieldOfView;
            CameraOnIcon = ModResource.GetTexture("Cursor Texture").Texture;
        }

        public void Update()
        {
            if (StatMaster.isMP)
            {
                if (StatMaster.isClient)
                {
                    return;
                }
                if (PlayerData.localPlayer.networkId != myPlayerID)
                {
                    return;
                }
            }
            if (activeKeyPressed)
            {
                activeKeyPressed = false;
                CameraOn = !CameraOn;
                if (CameraOn)
                {
                    rotationX = 0;
                    rotationY = 0;
                    targetFOV = defaultFOV;
                    MainCamera.transform.rotation = transform.rotation * new Quaternion(1, 0, 0, 1f);
                }
            }

            if (CameraOn)
            {
                if (DataManager.Instance.StickOn[myPlayerID])
                {
                    SingleInstanceFindOnly<MouseOrbit>.Instance.isActive = false;
                    targetFOV -= Input.GetAxis("Mouse ScrollWheel") * 70f;
                    targetFOV = Mathf.Clamp(targetFOV, minFOV, maxFOV);
                    MainCamera.transform.position = transform.position + 0.9f * transform.forward;
                    Cursor.lockState = CursorLockMode.None;
                    MainCamera.transform.rotation = Quaternion.Lerp(MainCamera.transform.rotation, transform.rotation * new Quaternion(1, 0, 0, 1f) * Quaternion.Euler(-rotationY, rotationX, 0), 0.2f);
                    Cursor.visible = true;
                    MainCamera.fieldOfView = MainCamera.fieldOfView + 0.2f * (targetFOV - MainCamera.fieldOfView);
                }
                else
                {
                    SingleInstanceFindOnly<MouseOrbit>.Instance.isActive = false;
                    targetFOV -= Input.GetAxis("Mouse ScrollWheel") * 60f;
                    targetFOV = Mathf.Clamp(targetFOV, minFOV, maxFOV);
                    MainCamera.transform.position = transform.position + 0.9f * transform.forward;
                    rotationX += Input.GetAxis("Mouse X") * Sensitivity;
                    rotationY += Input.GetAxis("Mouse Y") * Sensitivity;
                    rotationX = Mathf.Clamp(rotationX, -170, 170);
                    rotationY = Mathf.Clamp(rotationY, -70, 70);
                    MainCamera.transform.rotation = Quaternion.Lerp(MainCamera.transform.rotation, transform.rotation * new Quaternion(1, 0, 0, 1f) * Quaternion.Euler(-rotationY, rotationX, 0), 0.2f);
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    MainCamera.fieldOfView = MainCamera.fieldOfView + 0.2f * (targetFOV - MainCamera.fieldOfView);
                }
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                targetFOV = originFOV;
                MainCamera.fieldOfView = originFOV;
                SingleInstanceFindOnly<MouseOrbit>.Instance.isActive = true;
            }
        }
        public void LateUpdate()
        {
            if (StatMaster.isMP)
            {
                if (!StatMaster.isClient)
                {
                    return;
                }
                if (PlayerData.localPlayer.networkId != myPlayerID)
                {
                    return;
                }
            }
            if (activeKeyPressed)
            {
                activeKeyPressed = false;
                CameraOn = !CameraOn;
                if (CameraOn)
                {
                    rotationX = 0;
                    rotationY = 0;
                    targetFOV = defaultFOV;
                    MainCamera.transform.rotation = transform.rotation * new Quaternion(1, 0, 0, 1f);
                }
            }

            if (CameraOn)
            {
                if (DataManager.Instance.StickOn[myPlayerID]) // when stick active
                {
                    SingleInstanceFindOnly<MouseOrbit>.Instance.isActive = false;
                    targetFOV -= Input.GetAxis("Mouse ScrollWheel") * 70f;
                    targetFOV = Mathf.Clamp(targetFOV, minFOV, maxFOV);
                    MainCamera.transform.position = transform.position + 0.9f * transform.forward;
                    Cursor.lockState = CursorLockMode.None;
                    MainCamera.transform.rotation = Quaternion.Lerp(MainCamera.transform.rotation, transform.rotation * new Quaternion(1, 0, 0, 1f) * Quaternion.Euler(-rotationY, rotationX, 0), 0.2f);
                    Cursor.visible = true;
                    MainCamera.fieldOfView = MainCamera.fieldOfView + 0.2f * (targetFOV - MainCamera.fieldOfView);
                }
                else
                {
                    SingleInstanceFindOnly<MouseOrbit>.Instance.isActive = false;
                    targetFOV -= Input.GetAxis("Mouse ScrollWheel") * 60f;
                    targetFOV = Mathf.Clamp(targetFOV, minFOV, maxFOV);
                    MainCamera.transform.position = transform.position + 0.9f * transform.forward;
                    rotationX += Input.GetAxis("Mouse X") * Sensitivity * targetFOV/70;
                    rotationY += Input.GetAxis("Mouse Y") * Sensitivity * targetFOV / 70;
                    rotationX = Mathf.Clamp(rotationX, -170, 170);
                    rotationY = Mathf.Clamp(rotationY, -70, 70);
                    MainCamera.transform.rotation = Quaternion.Lerp(MainCamera.transform.rotation, transform.rotation * new Quaternion(1, 0, 0, 1f) * Quaternion.Euler(-rotationY, rotationX, 0), 0.2f);
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    MainCamera.fieldOfView = MainCamera.fieldOfView + 0.2f * (targetFOV - MainCamera.fieldOfView);
                }
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                targetFOV = originFOV;
                MainCamera.fieldOfView = originFOV;
                SingleInstanceFindOnly<MouseOrbit>.Instance.isActive = true;
            }
        }
        
        public void OnDestroy()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            targetFOV = originFOV;
            MainCamera.fieldOfView = originFOV;
            SingleInstanceFindOnly<MouseOrbit>.Instance.isActive = true;
        }

        public void OnGUI()
        {
            if (StatMaster.isMP)
            {
                if (PlayerData.localPlayer.networkId != myPlayerID)
                {
                    return;
                }
            }

            if (CameraOn)
            {
                if (DataManager.Instance.StickOn[myPlayerID])
                {
                    Cursor.visible = true;
                }
                else
                {
                    Cursor.visible = false;
                }
            }
            else
            {
                Cursor.visible = true;
            }
            if (CameraOn)
            {
                GUI.color = Color.yellow;
                GUI.DrawTexture(new Rect(Screen.width/2 - 20 / 2, Screen.height/2 - 20 / 2, 20, 20), CameraOnIcon);
            }
        }

    }

    // main
    class CentralController : BlockScript
    {
        //camera
        public MKey CamKey;
        public MSlider minFOV;
        public MSlider maxFOV;
        public MSlider CameraSensitivity;
        public MSlider DefaultFOV;
        public GameObject PilotCam;
        public PilotCameraController PCC;

        //kneeboard
        public MKey ToggleKneeboard;
        public MSlider KneeboardSize;
        public GameObject KneeboardCanvas;
        public GameObject Kneeboard;


        //simulators
        public RadarDisplayerSimulator radarDisplayerSimulator;
        public A2GDisplayerSimulator a2gDisplayerSimulator;
        public LoadDisplayerSimulator loadDisplayerSimulator;
        //public NavDisplayerSimulator navDisplayerSimulator;
        public GameObject radarDisplayerSimulatorObject;
        public GameObject a2gDisplayerSimulatorObject;
        public GameObject loadDisplayerSimulatorObject;
        public GameObject navDisplayerSimulatorObject;

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

        // for A2G displayer
        public MKey A2GLock;
        public MKey A2GZoomIn;
        public MKey A2GZoomOut;
        public MKey A2GPitchUp;
        public MKey A2GPitchDown;
        public MKey A2GYawLeft;
        public MKey A2GYawRight;
        public MKey A2GTrack;
        public MMenu TVColor;

        // for Load displayer
        public MKey LaunchSRAAM;
        public MKey LaunchMRAAM;
        public MKey LaunchAGM;
        public MKey LaunchGBU;
        public MSlider LoadRegion;
        public MSlider LoadOffset;

        // for Navigation displayer
        public MKey SwitchWP;
        public MKey NavScaleInc;
        public MKey NavScaleDec;
        public MText[] WPName = new MText[8];
        public MText[] WPPos = new MText[8];




        public override bool EmulatesAnyKeys { get { return true; } }

        //for Camera
        public void addCameraMapper()
        {
            CamKey = AddKey("Camera Activate", "CameraActivate", KeyCode.F);
            DefaultFOV = AddSlider("Camera Default FOV", "CameraDefaultFOV", 50f, 40f, 90f);
            minFOV = AddSlider("Camera Min FOV", "CameraMinFOV", 20f, 5f, 40f);
            maxFOV = AddSlider("Camera Max FOV", "CameraMaxFOV", 60f, 40f, 90f);
            CameraSensitivity = AddSlider("Camera Sensitivity", "CameraSensitivity", 1f, 0.2f, 2f);
        }
        public void InitCam()
        {
            if (!transform.FindChild("PilotCam"))
            {
                PilotCam = new GameObject("PilotCam");
                PilotCam.transform.SetParent(transform);
                PilotCam.transform.localPosition = new Vector3(0, 0, 0);
                PilotCam.transform.localRotation = Quaternion.identity;
                PilotCam.transform.localScale = Vector3.one;
                PCC = PilotCam.AddComponent<PilotCameraController>();
                PCC.myPlayerID = myPlayerID;
                PCC.minFOV = minFOV.Value;
                PCC.maxFOV = maxFOV.Value;
                PCC.Sensitivity = CameraSensitivity.Value;
            }
        }
        public void CamKey_Update()
        {
            if (CamKey.IsPressed)
            {
                PCC.activeKeyPressed = true;
            }
        }

        // for kneeboard
        public void addKneeboardMapper()
        {
            ToggleKneeboard = AddKey("Kneeboard Toggle", "KneeboardToggle", KeyCode.P);
            KneeboardSize = AddSlider("Kneeboard Size", "KneeboardSize", 0.5f, 0.1f, 1f);
            //customTextSize = AddSlider("Kneeboard note size", "KneeboardcustomTextSize", 28f, 12f, 64f);
            //CustomText = AddText("Kneeboard custom text", "KneeboardCustomText", "Leave your own notes here.");
        }
        public void addKeyTextToKneeboard()
        {
            KneeboardController controller = Kneeboard.GetComponent<KneeboardController>();
            controller.RadarPU = scanUp.GetKey(0);
            controller.RadarPD = scanDown.GetKey(0);
            controller.RadarSI = EnlargeScanAngle.GetKey(0);
            controller.RadarSD = ReduceScanAngle.GetKey(0);
            controller.RadarLock = RadarLock.GetKey(0);
            controller.RadarTDCU = ChooserUp.GetKey(0);
            controller.RadarTDCD = ChooserDown.GetKey(0);
            controller.RadarTDCL = ChooserLeft.GetKey(0);
            controller.RadarTDCR = ChooserDown.GetKey(0);
            controller.A2GLock = A2GLock.GetKey(0);
            controller.A2GTrack = A2GTrack.GetKey(0);
            controller.A2GZI = A2GZoomIn.GetKey(0);
            controller.A2GZO = A2GZoomOut.GetKey(0);
            controller.A2GPU = A2GPitchUp.GetKey(0);
            controller.A2GPD = A2GPitchDown.GetKey(0);
            controller.A2GYL = A2GYawLeft.GetKey(0);
            controller.A2GYR = A2GYawRight.GetKey(0);
            controller.SRAAM = LaunchSRAAM.GetKey(0);
            controller.MRAAM = LaunchMRAAM.GetKey(0);
            controller.AGM = LaunchAGM.GetKey(0);
            controller.GBU = LaunchGBU.GetKey(0);
            controller.NavSwitch = SwitchWP.GetKey(0);
            controller.NavScaleInc = NavScaleInc.GetKey(0);
            controller.NavScaleDec = NavScaleDec.GetKey(0);
        }
        public void InitKneeboard()
        {
            if (!GameObject.Find("KneeboardCanvas"))
            {
                KneeboardCanvas = Instantiate(AssetManager.Instance.Kneeboard.KneeboardCanvas);
                KneeboardCanvas.name = "KneeboardCanvas";
                Kneeboard = KneeboardCanvas.transform.FindChild("kneeboard").gameObject;
                Kneeboard.AddComponent<KneeboardController>().size = KneeboardSize.Value;
                Kneeboard.GetComponent<KneeboardController>().myPlayerID = myPlayerID;
                Kneeboard.transform.Find("woodbase").gameObject.SetActive(false);
                Kneeboard.transform.Find("paperBase").gameObject.SetActive(false);
            }
            KneeboardCanvas.SetActive(true);
            addKeyTextToKneeboard();

        }
        public void KneeboardKey_Update()
        {
            if (ToggleKneeboard.IsPressed)
            {
                Kneeboard.GetComponent<KneeboardController>().boardOn = !Kneeboard.GetComponent<KneeboardController>().boardOn;
            }
        }


        // for blackout
        public void addGeneralMapper()
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
                blackoutIndex = CCData.Instance.BlackoutData[(int)PlayerData.localPlayer.networkId];
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

        // for displayer
        public void addRadarDisplayerMapper()
        {
            RadarLock = AddKey("Radar Lock", "Lock Target", KeyCode.X);
            EnlargeScanAngle = AddKey("Radar Expand scan range", "EnlargeScanAngle", KeyCode.T);
            ReduceScanAngle = AddKey("Radar Reduce scan range", "ReduceScanAngle", KeyCode.U);
            ChooserUp = AddKey("Radar Cursor up", "ChooserUp", KeyCode.Y);
            ChooserDown = AddKey("Radar Cursor down", "ChooserDown", KeyCode.H);
            ChooserLeft = AddKey("Radar Cursor left", "ChooserLeft", KeyCode.G);
            ChooserRight = AddKey("Radar Cursor right", "ChooserRight", KeyCode.J);
            scanUp = AddKey("Radar pitch up", "scan up", KeyCode.I);
            scanDown = AddKey("Radar pitch down", "scan down", KeyCode.K);
            ScanRegionAfterLock = AddSlider("Radar Default scan angle when locking", "Default scan angle when locking", 20f, 5f, 60f);
        }
        public void addA2GDisplayerMapper()
        {
            A2GLock = AddKey("A2G Lock", "A2GLock", KeyCode.X);
            A2GZoomIn = AddKey("A2G ZoomIn", "A2GZoomIn", KeyCode.N);
            A2GZoomOut = AddKey("A2G ZoomOut", "A2GZoomOut", KeyCode.M);
            A2GPitchUp = AddKey("A2G Pitch Up", "A2GPitchUp", KeyCode.Y);
            A2GPitchDown = AddKey("A2G Pitch Down", "A2GPitchDown", KeyCode.H);
            A2GYawLeft = AddKey("A2G Yaw Left", "A2GYawLeft", KeyCode.G);
            A2GYawRight = AddKey("A2G Yaw Right", "A2GYawRight", KeyCode.J);
            A2GTrack = AddKey("A2G Track", "A2GTrack", KeyCode.T);
        }
        public void addLoadDisplayerMapper()
        {
            LaunchSRAAM = AddKey("Launch SRAAM", "LaunchSRAAM", KeyCode.Alpha1);
            LaunchMRAAM = AddKey("Launch MRAAM", "LaunchMRAAM", KeyCode.Alpha2);
            LaunchAGM = AddKey("Launch AGM", "LaunchAGM", KeyCode.Alpha3);
            LaunchGBU = AddKey("Launch GBU", "LaunchGBU", KeyCode.Alpha4);
            LoadRegion = AddSlider("Load Region", "Region", 20f, 0f, 40f);
            LoadOffset = AddSlider("Load Offset", "Offset", 0f, -2f, 2f);
        }
        public void addNavDisplayerMapper()
        {
            SwitchWP = AddKey("Switch Way Point", "SwitchWayPoint", KeyCode.RightShift);
            NavScaleInc = AddKey("Nav Scale Increase", "NacScaleInc", KeyCode.N);
            NavScaleDec = AddKey("Nav Scale Decrease", "NacScaleDec", KeyCode.M);
            WPName[0] = AddText("WP0 Name", "WPName0", "Origin Point");
            WPPos[0] = AddText("WP0 Coordinate", "WPCoordinate0", "0,0,0");
            for (int i = 1; i < 8; i++)
            {
                WPName[i] = AddText("WP"+i.ToString()+" Name", "WPName" + i.ToString(), "");
                WPPos[i] = AddText("WP" + i.ToString() + " Coordinate", "WPCoordinate" + i.ToString(), "");
            }
        }
        public void RadarDisplayerKey_Update()
        {
            // lock key
            if (RadarLock.IsPressed)
            {
                radarDisplayerSimulator.LockPressed = true;
            }
            else if (RadarLock.IsReleased)
            {
                radarDisplayerSimulator.LockPressed = false;
            }

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
        public void A2GDisplayerKey_Update()
        {
            if (A2GLock.IsPressed)
            {
                a2gDisplayerSimulator.LockPressed = true;
            }
            else if (A2GLock.IsReleased)
            {
                a2gDisplayerSimulator.LockPressed = false;
            }
            if (A2GTrack.IsPressed)
            {
                a2gDisplayerSimulator.TrackPressed = true;
            }
            else if (A2GTrack.IsReleased)
            {
                a2gDisplayerSimulator.TrackPressed = false;
            }

            if (A2GYawLeft.IsHeld)
            {
                a2gDisplayerSimulator.YawLeftPressed = true;
            }
            else
            {
                a2gDisplayerSimulator.YawLeftPressed = false;
            }
            if (A2GYawRight.IsHeld)
            {
                a2gDisplayerSimulator.YawRightPressed = true;
            }
            else
            {
                a2gDisplayerSimulator.YawRightPressed = false;
            }

            if (A2GPitchUp.IsHeld)
            {
                a2gDisplayerSimulator.PitchUpPressed = true;
            }
            else
            {
                a2gDisplayerSimulator.PitchUpPressed = false;
            }
            if (A2GPitchDown.IsHeld)
            {
                a2gDisplayerSimulator.PitchDownPressed = true;
            }
            else
            {
                a2gDisplayerSimulator.PitchDownPressed = false;
            }

            if (A2GZoomIn.IsHeld)
            {
                a2gDisplayerSimulator.ZoomInPressed = true;
            }
            else
            {
                a2gDisplayerSimulator.ZoomInPressed = false;
            }
            if (A2GZoomOut.IsHeld)
            {
                a2gDisplayerSimulator.ZoomOutPressed = true;
            }
            else
            {
                a2gDisplayerSimulator.ZoomOutPressed = false;
            }
        }
        public void LoadDisplayerKey_Update()
        {
            if (LaunchSRAAM.IsPressed)
            {
                loadDisplayerSimulator.LaunchSRAAM = true;
            }
            else if (LaunchSRAAM.IsReleased)
            {
                loadDisplayerSimulator.LaunchSRAAM = false;
            }

            if (LaunchMRAAM.IsPressed)
            {
                loadDisplayerSimulator.LaunchMRAAM = true;
            }
            else if (LaunchMRAAM.IsReleased)
            {
                loadDisplayerSimulator.LaunchMRAAM = false;
            }

            if (LaunchAGM.IsPressed)
            {
                loadDisplayerSimulator.LaunchAGM = true;
            }
            else if (LaunchAGM.IsReleased)
            {
                loadDisplayerSimulator.LaunchAGM = false;
            }
            if (LaunchGBU.IsPressed)
            {
                loadDisplayerSimulator.LaunchGBU = true;
            }
            else if (LaunchGBU.IsReleased)
            {
                loadDisplayerSimulator.LaunchGBU = false;
            }
        }
        public void NavDisplayerDataAndKey_Update()
        {
            if (SwitchWP.IsPressed)
            {
                CC2NavDisplayerData.Instance.ChangeSelection[myPlayerID] = true;
            }
            if (NavScaleDec.IsPressed)
            {
                CC2NavDisplayerData.Instance.ScaleDecPressed[myPlayerID] = true;
            }
            if (NavScaleInc.IsPressed)
            {
                CC2NavDisplayerData.Instance.ScaleIncPressed[myPlayerID] = true;
            }
            CC2NavDisplayerData.Instance.myPosition[myPlayerID] = transform.position;
            float angle = -SignedAngle(new Vector2(0,1), -new Vector2(transform.up.x,transform.up.z));
            if (angle < 0)
            {
                angle += 360;
            }
            CC2NavDisplayerData.Instance.orientation[myPlayerID] = angle;
        }
        public void EmulateLoadDisplayerKey_FixedUpdateHost()
        {
            if (loadDisplayerSimulator.hasEmulateKey)
            {
                base.EmulateKeys(new MKey[0], loadDisplayerSimulator.KeyToBeEmulated, true);
                loadDisplayerSimulator.finishEmulate();
            }
        }

        // assist 
        private float SignedAngle(Vector3 v1, Vector3 v2)
        {
            if (v1.x * v2.y - v1.y * v2.x < 0)
            {
                return -Vector2.Angle(v1, v2);
            }
            else
            {
                return Vector2.Angle(v1, v2);
            }
        }


        public override void SafeAwake()
        {
            name = "Central Controller";
            myPlayerID = BlockBehaviour.ParentMachine.PlayerID;
            GTolerance = AddToggle("GTolerance", "GTolerance", true);

            addCameraMapper();
            addRadarDisplayerMapper();
            addA2GDisplayerMapper();
            addLoadDisplayerMapper();
            addNavDisplayerMapper();
            addKneeboardMapper();
            
        }
        public override void OnSimulateStart()
        {
            myRigid = BlockBehaviour.GetComponent<Rigidbody>();
            InitBlackOut();
            InitCam();
            
            if (!transform.Find("Radar Displayer Simulator"))
            {
                radarDisplayerSimulatorObject = new GameObject("Radar Displayer Simulator");
                radarDisplayerSimulatorObject.transform.SetParent(transform);
                radarDisplayerSimulatorObject.transform.localPosition = Vector3.zero;
                radarDisplayerSimulatorObject.transform.localRotation = Quaternion.identity;
                radarDisplayerSimulatorObject.transform.localScale = Vector3.one;
                radarDisplayerSimulator = radarDisplayerSimulatorObject.AddComponent<RadarDisplayerSimulator>();
                radarDisplayerSimulator.myPlayerID = myPlayerID;
                radarDisplayerSimulator.isClient = StatMaster.isClient;
                radarDisplayerSimulator.ScanRegionAfterLock = ScanRegionAfterLock.Value;
            }
            if (!transform.Find("A2G Displayer Simulator"))
            {
                a2gDisplayerSimulatorObject = new GameObject("A2G Displayer Simulator");
                a2gDisplayerSimulatorObject.transform.SetParent(transform);
                a2gDisplayerSimulatorObject.transform.localPosition = Vector3.zero;
                a2gDisplayerSimulatorObject.transform.localRotation = Quaternion.identity;
                a2gDisplayerSimulatorObject.transform.localScale = Vector3.one;
                a2gDisplayerSimulator = a2gDisplayerSimulatorObject.AddComponent<A2GDisplayerSimulator>();
                a2gDisplayerSimulator.myPlayerID = myPlayerID;
                a2gDisplayerSimulator.isClient = StatMaster.isClient;
            }
            if (!transform.Find("Load Displayer Simulator"))
            {
                loadDisplayerSimulatorObject = new GameObject("Load Displayer Simulator");
                loadDisplayerSimulatorObject.transform.SetParent(transform);
                loadDisplayerSimulatorObject.transform.localPosition = Vector3.zero;
                loadDisplayerSimulatorObject.transform.localRotation = Quaternion.identity;
                loadDisplayerSimulatorObject.transform.localScale = Vector3.one;
                loadDisplayerSimulator = loadDisplayerSimulatorObject.AddComponent<LoadDisplayerSimulator>();
                loadDisplayerSimulator.myPlayerID = myPlayerID;
                loadDisplayerSimulator.isClient = StatMaster.isClient;
                loadDisplayerSimulator.Region = LoadRegion.Value;
                loadDisplayerSimulator.Offset = LoadOffset.Value;
            }
            if (!transform.Find("Navigation Displayer Simulator"))
            {
                navDisplayerSimulatorObject = new GameObject("Navigation Displayer Simulator");
                navDisplayerSimulatorObject.transform.SetParent(transform);
                navDisplayerSimulatorObject.transform.localPosition = Vector3.zero;
                navDisplayerSimulatorObject.transform.localRotation = Quaternion.identity;
                navDisplayerSimulatorObject.transform.localScale = Vector3.one;
                //navDisplayerSimulator = navDisplayerSimulatorObject.AddComponent<NavDisplayerSimulator>();
                for (int i = 0; i < 8; i++)
                {
                    string[] coordText;
                    coordText = WPPos[i].Value.Split(',');
                    try
                    {
                        CC2NavDisplayerData.Instance.dist[myPlayerID][i].x = float.Parse(coordText[0]);
                        CC2NavDisplayerData.Instance.dist[myPlayerID][i].y = float.Parse(coordText[1]);
                        CC2NavDisplayerData.Instance.dist[myPlayerID][i].z = float.Parse(coordText[2]);
                        CC2NavDisplayerData.Instance.hasWP[myPlayerID][i] = true;
                        CC2NavDisplayerData.Instance.WPName[myPlayerID][i] = WPName[i].Value;
                    }
                    catch {
                        CC2NavDisplayerData.Instance.hasWP[myPlayerID][i] = false;
                    }
                }

            }
            InitKneeboard();// after init nav CC2Nav
        }
        public override void OnSimulateStop()
        {
            Destroy(KneeboardCanvas);
            preVeclocity = Vector3.zero;
            overLoad = Vector3.zero;
            blackoutIndex = 0;
            ModNetworking.SendToAll(ClientBlackoutMsg.CreateMessage(myPlayerID, 0f));
            Destroy(BlackOut);

            loadDisplayerSimulator.initialized = false;
            loadDisplayerSimulator.leftWingLoad.Clear();
            loadDisplayerSimulator.rightWingLoad.Clear();
            CC2LoadDisplayerData.Instance.LoadListReady[myPlayerID] = false;
            //Radar2CCData.Instance.cc2radar.Reset();
        }
        public override void SimulateUpdateHost()
        {
            CamKey_Update();
            RadarDisplayerKey_Update();
            A2GDisplayerKey_Update();
            LoadDisplayerKey_Update();
            NavDisplayerDataAndKey_Update();
            KneeboardKey_Update();
        }
        public override void SimulateUpdateClient()
        {
            CamKey_Update();
            RadarDisplayerKey_Update();
            A2GDisplayerKey_Update();
            NavDisplayerDataAndKey_Update();
            KneeboardKey_Update();
        }
        public override void SimulateFixedUpdateHost()
        {
            DisplayBlackout();
            EmulateLoadDisplayerKey_FixedUpdateHost();
        }
        public override void SimulateFixedUpdateClient()
        {
            DisplayBlackout();
        }
        public void OnGUI()
        {
            //GUI.Box(new Rect(100, 200, 200, 50), ShouldShowGUI().ToString());
            //GUI.Box(new Rect(100, 250, 200, 50), radarDisplayerSimulator.SLController.currAngle.ToString());
        }

    }
}