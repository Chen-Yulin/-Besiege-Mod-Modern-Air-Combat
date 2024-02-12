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
    
    public class EOMsgReceiver : SingleInstance<EOMsgReceiver>
    {
        public override string Name { get; } = "EoMsgReceiver";
        public Vector3[] LockPointPosition = new Vector3[16];
        public Vector3[] LockPointVelocity = new Vector3[16];
        public float[] FOV = new float[16];
        public bool[] Lock = new bool[16];
        public bool[] Track = new bool[16];
        public bool[] ThermalOn = new bool[16];
        public bool[] InverseThermal = new bool[16];
        public void PositionReceiver(Message msg)
        {
            LockPointPosition[(int)msg.GetData(0)] = (Vector3)msg.GetData(1);
        }
        public void VelocityReceiver(Message msg)
        {
            LockPointVelocity[(int)msg.GetData(0)] = (Vector3)msg.GetData(1);
        }
        public void FOVReceiver(Message msg)
        {
            FOV[(int)msg.GetData(0)] = (float)msg.GetData(1);
        }
        public void LockReceiver(Message msg)
        {
            Lock[(int)msg.GetData(0)] = (bool)msg.GetData(1);
        }
        public void TrackReveiver(Message msg)
        {
            Track[(int)msg.GetData(0)] = (bool)msg.GetData(1);
        }
        public void ThermalOnReceiver(Message msg)
        {
            ThermalOn[(int)msg.GetData(0)] = (bool)msg.GetData(1);
        }
        public void ThermalInverseReceiver(Message msg)
        {
            InverseThermal[(int)msg.GetData(0)] = (bool)msg.GetData(1);
        }
    }
    public class ElectroOpticalBlock:BlockScript
    {
        public MKey ToggleThermal;
        public MKey InverseThermal;
        public MKey Reset;
        public MSlider LockDistance;
        public GameObject CameraBase;
        public GameObject ThermalCamera;
        public GameObject NormalCamera;
        public Camera ThermalCam;
        public Camera NormalCam;
        public float FOV = 40f;
        public bool Lock = false;
        public IEnumerator ResettingCameraIE;

        public ThermalVision CameraTV;
        public GameObject LockPoint;
        public ScanCollisonHit TrackHit;
        public SphereCollider LockScan;
        public Vector3 LockPosition;
        public Vector3 LockVelocity;

        public static MessageType ClientLockPointPositionMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Vector3);
        public static MessageType ClientLockPointVelocityMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Vector3);
        public static MessageType ClientFOVMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Single);
        public static MessageType ClientLockMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Boolean);
        public static MessageType ClientTrackMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Boolean);
        public static MessageType ClientThermalOnMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Boolean);
        public static MessageType ClientInverseThermalMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Boolean);

        protected float detectFreqTime = 0;
        protected float destroyDelay = 0;
        protected bool Resetting = false;


        private int myPlayerID;

        public void AxisLookAt(Transform tr_self, Vector3 lookPos, Vector3 directionAxis, float speed)
        {
            var rotation = tr_self.rotation;
            var targetDir = lookPos - tr_self.position;
            //指定哪根轴朝向目标,自行修改Vector3的方向
            var fromDir = tr_self.rotation * directionAxis;
            //计算垂直于当前方向和目标方向的轴
            var axis = Vector3.Cross(fromDir, targetDir).normalized;
            //计算当前方向和目标方向的夹角
            var angle = Vector3.Angle(fromDir, targetDir);
            //将当前朝向向目标方向旋转一定角度，这个角度值可以做插值
            tr_self.rotation = Quaternion.Lerp(rotation, Quaternion.AngleAxis(angle, axis) * rotation, speed);

        }//from CSDN

        public void initLockPoint()
        {
            if (!transform.FindChild("LockPoint"))
            {
                LockPoint = new GameObject("LockPoint");
                LockPoint.transform.SetParent(CameraBase.transform);
                LockPoint.transform.localPosition = Vector3.zero;
                LockPoint.transform.localRotation = Quaternion.identity;
                LockPoint.transform.localScale = Vector3.one;

                LockScan = LockPoint.AddComponent<SphereCollider>();
                LockScan.radius = 10;
                LockScan.isTrigger = true;

                TrackHit = LockPoint.AddComponent<ScanCollisonHit>();

                LockPoint.SetActive(false);
            }
        }

        public void initCamera()
        {
            if (!transform.FindChild("CameraBase"))
            {
                CameraBase = new GameObject("CameraBase");
                CameraBase.transform.SetParent(transform);
                CameraBase.transform.localPosition = new Vector3(0, 2f, 2f);
                CameraBase.transform.localRotation = Quaternion.Euler(270,0,0);
                CameraBase.transform.localScale = Vector3.one;

                ThermalCamera = new GameObject("ThermalCamera");
                ThermalCamera.transform.SetParent(CameraBase.transform);
                ThermalCamera.transform.localPosition = Vector3.zero;
                ThermalCamera.transform.localRotation = Quaternion.identity;
                ThermalCamera.transform.localScale = Vector3.one;

                ThermalCam = ThermalCamera.AddComponent<Camera>();
                ThermalCam.CopyFrom(Camera.main);
                ThermalCam.transform.localPosition = Vector3.zero;
                ThermalCam.transform.localRotation = Quaternion.identity;
                ThermalCam.transform.localScale = Vector3.one;
                ThermalCam.fieldOfView = FOV;
                ThermalCam.clearFlags = CameraClearFlags.SolidColor;
                ThermalCam.backgroundColor = Color.black;
                ThermalCam.farClipPlane = ModController.Instance.ElectrOpticalCameraDistance;
                ThermalCam.cullingMask = 1<<25;
                ThermalCam.targetTexture = DataManager.Instance.highlight[myPlayerID];

                NormalCamera = new GameObject("NormalCamera");
                NormalCamera.transform.SetParent(CameraBase.transform);
                NormalCamera.transform.localPosition = Vector3.zero;
                NormalCamera.transform.localRotation = Quaternion.identity;
                NormalCamera.transform.localScale = Vector3.one;

                NormalCam = NormalCamera.AddComponent<Camera>();
                NormalCam.CopyFrom(Camera.main);
                NormalCam.transform.localPosition = Vector3.zero;
                NormalCam.transform.localRotation = Quaternion.identity;
                NormalCam.transform.localScale = Vector3.one;
                NormalCam.fieldOfView = FOV;
                NormalCam.farClipPlane = ModController.Instance.ElectrOpticalCameraDistance;
                NormalCam.targetTexture = DataManager.Instance.output[myPlayerID];

                CameraTV = NormalCamera.AddComponent<ThermalVision>();
                CameraTV.OtherTex = DataManager.Instance.highlight[myPlayerID];
                CameraTV.ThermalOn = false;
                CameraTV.IsInverse = false;

                CameraBase.SetActive(false);
            }
        }



        public void CameraOn()
        {
            FOV = 40;
            LockPosition = CameraBase.transform.position + 10 * CameraBase.transform.forward;
            CameraBase.SetActive(true);
            //LockPoint.SetActive(true);
            LockPoint.transform.position = LockPosition;
        }

        public void CameraOff()
        {
            CameraBase.SetActive(false);
            LockPoint.SetActive(false);
        }


        public bool findLockPosition()
        {
            Ray CameraRay = new Ray(CameraBase.transform.position + CameraBase.transform.forward * 20, LockPosition-CameraBase.transform.position);
            RaycastHit hit;
            if (Physics.Raycast(CameraRay, out hit, LockDistance.Value))
            {
                LockPoint.transform.position = hit.point;
                LockPosition = hit.point;
                return true;
            }
            else
            {
                return false;
            }
        }

        protected void TrackTarget()
        {
            
            LockPoint.SetActive(true);
            detectFreqTime += Time.fixedDeltaTime;

            if (detectFreqTime >= 0.03)
            {
                detectFreqTime = 0;
                //judge whether there is a target
                if (TrackHit.targetCols.Count == 0)
                {
                    LockVelocity = Vector3.zero;
                }
                else
                {
                    try
                    {
                        //judge whether use flare's collider or real target's collider
                        Collider targetCol;
                        targetCol = TrackHit.targetCols.Peek();
                        LockPosition = targetCol.transform.position;
                        LockVelocity = targetCol.attachedRigidbody.velocity;
                        //Debug.Log(targetVelocity);
                    }
                    catch { }
                }
                LockPoint.SetActive(false);
            }

        }

        protected float CalculateOrientation()
        {
            Vector3 horizonSelf = new Vector3(transform.up.x,0,transform.up.z);
            Vector3 horizonCamera = new Vector3(CameraBase.transform.forward.x, 0, CameraBase.transform.forward.z);
            float angle = Vector3.Angle(horizonSelf, horizonCamera); //求出两向量之间的夹角
            Vector3 normal = Vector3.Cross(horizonSelf, horizonCamera);//叉乘求出法线向量
            angle *= Mathf.Sign(Vector3.Dot(normal, -transform.forward));  //求法线向量与物体上方向向量点乘，结果为1或-1，修正旋转方向
            return angle;
        }
        protected float CalculatePitch()
        {
            float angle = Vector3.Angle(Vector3.down, CameraBase.transform.forward);
            return angle;
        }

        public void updateCameraPara()
        {
            FOV = DataManager.Instance.TV_FOV[myPlayerID];
            NormalCam.fieldOfView = FOV;
            ThermalCam.fieldOfView = FOV;
            //ThermalCamera.transform.rotation = Quaternion.Euler(ThermalCamera.transform.rotation.eulerAngles.x, ThermalCamera.transform.rotation.eulerAngles.y, 0);
            CameraBase.transform.rotation = Quaternion.Euler(CameraBase.transform.rotation.eulerAngles.x, CameraBase.transform.rotation.eulerAngles.y, 0);


            if (DataManager.Instance.TV_Lock[myPlayerID])
            {
                if (Lock == false)
                {
                    if (findLockPosition())
                    {
                        DataManager.Instance.TV_Lock[myPlayerID] = true;
                    }
                    else
                    {
                        DataManager.Instance.TV_Lock[myPlayerID] = false;
                        Lock = false;
                        return;
                    }

                }
                if (DataManager.Instance.TV_LeftRight[myPlayerID] != 0 || DataManager.Instance.TV_UpDown[myPlayerID] != 0)
                {
                    if (DataManager.Instance.TV_LeftRight[myPlayerID] == 1)
                    {
                        LockPosition += 0.0002f * FOV * CameraBase.transform.right*Vector3.Distance(LockPosition,CameraBase.transform.position);
                    }
                    else if (DataManager.Instance.TV_LeftRight[myPlayerID] == -1)
                    {
                        LockPosition -= 0.0002f * FOV * CameraBase.transform.right * Vector3.Distance(LockPosition, CameraBase.transform.position);
                    }

                    if (DataManager.Instance.TV_UpDown[myPlayerID] == 1)
                    {
                        LockPosition += 0.0002f * FOV * CameraBase.transform.up * Vector3.Distance(LockPosition, CameraBase.transform.position);
                    }
                    else if (DataManager.Instance.TV_UpDown[myPlayerID] == -1)
                    {
                        LockPosition -= 0.0002f * FOV * CameraBase.transform.up * Vector3.Distance(LockPosition, CameraBase.transform.position);
                    }

                    if (findLockPosition())
                    {
                        DataManager.Instance.TV_Lock[myPlayerID] = true;
                    }
                    else
                    {
                        DataManager.Instance.TV_Lock[myPlayerID] = false;
                    }
                }
                
            }
            else
            {
                if (DataManager.Instance.TV_LeftRight[myPlayerID] == 1)
                {
                    CameraBase.transform.rotation = Quaternion.Euler(CameraBase.transform.rotation.eulerAngles.x, CameraBase.transform.rotation.eulerAngles.y + 0.01f * FOV, 0);
                }
                else if (DataManager.Instance.TV_LeftRight[myPlayerID] == -1)
                {
                    CameraBase.transform.rotation = Quaternion.Euler(CameraBase.transform.rotation.eulerAngles.x, CameraBase.transform.rotation.eulerAngles.y - 0.01f * FOV, 0);
                }

                if (DataManager.Instance.TV_UpDown[myPlayerID] == 1)
                {
                    CameraBase.transform.rotation = Quaternion.Euler(CameraBase.transform.rotation.eulerAngles.x - 0.01f * FOV, CameraBase.transform.rotation.eulerAngles.y, 0);
                }
                else if (DataManager.Instance.TV_UpDown[myPlayerID] == -1)
                {
                    CameraBase.transform.rotation = Quaternion.Euler(CameraBase.transform.rotation.eulerAngles.x + 0.01f * FOV, CameraBase.transform.rotation.eulerAngles.y, 0);
                }


            }
            Lock = DataManager.Instance.TV_Lock[myPlayerID];

        }
        public void updateCameraParaClient()
        {
            NormalCam.fieldOfView = FOV;
            ThermalCam.fieldOfView = FOV;
            CameraBase.transform.rotation = Quaternion.Euler(CameraBase.transform.rotation.eulerAngles.x, CameraBase.transform.rotation.eulerAngles.y, 0);
        }

        public override void SafeAwake()
        {
            myPlayerID = BlockBehaviour.ParentMachine.PlayerID;
            Reset = AddKey("Reset", "Reset", KeyCode.R);
            ToggleThermal = AddKey("Toggle Thermal", "ToggleThermal", KeyCode.T);
            InverseThermal = AddKey("Toggle Thermal Inverse", "InverseThermal", KeyCode.I);
            LockDistance = AddSlider("Lock Distance", "LockDistance", 7000, 2000, 20000);
            initCamera();
            initLockPoint();
        }

        public void Start()
        {
            
        }
        public override void OnSimulateStart()
        {
            CameraOn();
        }

        public override void OnSimulateStop()
        {
            CameraOff();
            Lock = false;
            DataManager.Instance.TV_Lock[myPlayerID] = false;
            DataManager.Instance.EO_ThermalOn[myPlayerID] = false;
            DataManager.Instance.EO_InverseThermal[myPlayerID] = false;
            ModNetworking.SendToAll(ClientLockMsg.CreateMessage(myPlayerID, false));
            ModNetworking.SendToAll(ClientThermalOnMsg.CreateMessage(myPlayerID, false));
            ModNetworking.SendToAll(ClientInverseThermalMsg.CreateMessage(myPlayerID, false));
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

        public override void SimulateUpdateClient()
        {
            // get data
            LockPosition = EOMsgReceiver.Instance.LockPointPosition[myPlayerID];
            LockVelocity = EOMsgReceiver.Instance.LockPointVelocity[myPlayerID];
            Lock = EOMsgReceiver.Instance.Lock[myPlayerID];
            FOV = EOMsgReceiver.Instance.FOV[myPlayerID];
            CameraTV.ThermalOn = EOMsgReceiver.Instance.ThermalOn[myPlayerID];
            CameraTV.IsInverse = EOMsgReceiver.Instance.InverseThermal[myPlayerID];
            // send data to local data manager
            DataManager.Instance.TV_Lock[myPlayerID] = Lock;
            DataManager.Instance.TV_LockPosition[myPlayerID] = LockPosition;
            DataManager.Instance.TV_FOV[myPlayerID] = FOV;
            DataManager.Instance.A2G_TargetData[myPlayerID].velocity = LockVelocity;
            DataManager.Instance.A2G_TargetData[myPlayerID].position = LockPosition;
            DataManager.Instance.EO_InverseThermal[myPlayerID] = CameraTV.IsInverse;
            DataManager.Instance.EO_ThermalOn[myPlayerID] = CameraTV.ThermalOn;


            LockPoint.transform.position = LockPosition;
            AxisLookAt(CameraBase.transform, LockPosition, Vector3.forward, 1f);

            // send orentation data to A2GScreen
            // no need to send to network
            DataManager.Instance.A2G_Orientation[myPlayerID] = CalculateOrientation();
            DataManager.Instance.A2G_Pitch[myPlayerID] = CalculatePitch();
        }


        public override void SimulateUpdateHost()
        {
            if (Reset.IsPressed)
            {
                Debug.Log("Resetting Camera~");
                CameraBase.transform.localRotation = Quaternion.Euler(270, 0, 0);
                Lock = false;
                TrackHit.targetCols.Clear();
                LockPoint.SetActive(false);
                DataManager.Instance.TV_Lock[myPlayerID] = false;
                DataManager.Instance.TV_FOV[myPlayerID] = 40f;
                Resetting = false;
            }
            if (ToggleThermal.IsPressed)
            {
                CameraTV.ThermalOn = !CameraTV.ThermalOn;
                DataManager.Instance.EO_ThermalOn[myPlayerID] = CameraTV.ThermalOn;
                ModNetworking.SendToAll(ClientThermalOnMsg.CreateMessage(myPlayerID, CameraTV.ThermalOn));
            }
            if (CameraTV.ThermalOn)
            {
                if (InverseThermal.IsPressed)
                {
                    CameraTV.IsInverse = !CameraTV.IsInverse;
                    DataManager.Instance.EO_InverseThermal[myPlayerID] = CameraTV.IsInverse;
                    ModNetworking.SendToAll(ClientInverseThermalMsg.CreateMessage(myPlayerID, CameraTV.IsInverse));
                }
            }

            if (!Lock)
            {
                LockPosition = CameraBase.transform.position + 10 * CameraBase.transform.forward;
            }

            LockPoint.transform.position = LockPosition;

            AxisLookAt(CameraBase.transform, LockPosition, Vector3.forward, 1f);



            // send orentation data to A2GScreen
            DataManager.Instance.A2G_Orientation[myPlayerID] = CalculateOrientation();
            DataManager.Instance.A2G_Pitch[myPlayerID] = CalculatePitch();

            // send to network
            ModNetworking.SendToAll(ClientLockPointPositionMsg.CreateMessage(myPlayerID, LockPosition));
            ModNetworking.SendToAll(ClientLockPointVelocityMsg.CreateMessage(myPlayerID, LockVelocity));
            ModNetworking.SendToAll(ClientFOVMsg.CreateMessage(myPlayerID, FOV));
            ModNetworking.SendToAll(ClientLockMsg.CreateMessage(myPlayerID, Lock));
        }
        public void MySimulateFixedUpdateClient()
        {
            DataManager.Instance.EO_Distance[myPlayerID] = (LockPosition - transform.position).magnitude;
            updateCameraParaClient();
        }
        public override void SimulateFixedUpdateHost()
        {
            DataManager.Instance.EO_Distance[myPlayerID] = (LockPosition - transform.position).magnitude;

            if (DataManager.Instance.TV_Track[myPlayerID])
            {
                if (DataManager.Instance.A2G_TargetDestroyed[myPlayerID])
                {
                    DataManager.Instance.TV_Track[myPlayerID] = false;
                    ModNetworking.SendToAll(ClientTrackMsg.CreateMessage(myPlayerID, DataManager.Instance.TV_Track[myPlayerID]));
                    TrackHit.targetCols.Clear();
                    LockPoint.SetActive(false);
                }
                else
                {
                    TrackTarget();
                }
            }
            else
            {
                detectFreqTime = 0;
                TrackHit.targetCols.Clear();
                LockPoint.SetActive(false);
            }
            if (DataManager.Instance.A2G_TargetDestroyed[myPlayerID])
            {
                destroyDelay += Time.fixedDeltaTime;
                if (destroyDelay >= 0.06f)
                {
                    DataManager.Instance.A2G_TargetDestroyed[myPlayerID] = false;
                    destroyDelay = 0f;
                }
                return;
            }
            updateCameraPara();
            if (!Lock)
            {
                DataManager.Instance.A2G_TargetData[myPlayerID].position = Vector3.zero;
                DataManager.Instance.A2G_TargetData[myPlayerID].velocity = Vector3.zero;
            }
            else
            {
                DataManager.Instance.A2G_TargetData[myPlayerID].position = LockPosition;
                DataManager.Instance.A2G_TargetData[myPlayerID].velocity = LockVelocity;
            }


        }

        void OnGUI()
        {
            //GUI.Box(new Rect(100, 200, 200, 50), LockPoint.activeSelf.ToString());
            //GUI.Box(new Rect(100, 300, 200, 50), Lock.ToString());
            //GUI.Box(new Rect(100, 400, 200, 50), FOV.ToString());
        }


    }
}
