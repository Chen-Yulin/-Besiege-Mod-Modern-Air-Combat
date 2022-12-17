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
    }
    public class ElectroOpticalBlock:BlockScript
    {
        public GameObject CameraBase;
        public GameObject ThermalCamera;
        public GameObject NormalCamera;
        public Camera ThermalCam;
        public Camera NormalCam;
        public float FOV = 40f;
        public bool Lock = false;

        public ThermalVision CameraTV;
        public GameObject LockPoint;
        public Vector3 LockPosition;

        public static MessageType ClientLockPointPositionMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Vector3);
        public static MessageType ClientLockPointVelocityMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Vector3);
        public static MessageType ClientFOVMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Single);
        public static MessageType ClientLockMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Boolean);


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
                LockPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);

                Destroy(LockPoint.GetComponent<Collider>());
                LockPoint.name = "LockPoint";
                LockPoint.transform.SetParent(CameraBase.transform);
                LockPoint.transform.localPosition = Vector3.zero;
                LockPoint.transform.localRotation = Quaternion.identity;
                LockPoint.transform.localScale = Vector3.one;

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
                ThermalCam.farClipPlane = 7000f;
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
                NormalCam.farClipPlane = 7000f;
                NormalCam.targetTexture = DataManager.Instance.output[myPlayerID];

                CameraTV = NormalCamera.AddComponent<ThermalVision>();
                CameraTV.OtherTex = DataManager.Instance.highlight[myPlayerID];
                CameraTV.ThermalOn = true;

                CameraBase.SetActive(false);
            }
        }



        public void CameraOn()
        {
            FOV = 40;
            LockPosition = CameraBase.transform.position + 10 * CameraBase.transform.forward;
            CameraBase.SetActive(true);
            LockPoint.SetActive(true);
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
            if (Physics.Raycast(CameraRay, out hit, 7000))
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
        }

        protected void Update()
        {
            
        }

        public override void SimulateUpdateClient()
        {
            // get data
            LockPosition = EOMsgReceiver.Instance.LockPointPosition[myPlayerID];
            Lock = EOMsgReceiver.Instance.Lock[myPlayerID];
            FOV = EOMsgReceiver.Instance.FOV[myPlayerID];
            // send data to local data manager
            DataManager.Instance.TV_Lock[myPlayerID] = Lock;
            DataManager.Instance.TV_LockPosition[myPlayerID] = LockPosition;
            DataManager.Instance.TV_FOV[myPlayerID] = FOV;

            LockPoint.transform.position = LockPosition;
            AxisLookAt(CameraBase.transform, LockPosition, Vector3.forward, 1f);
        }


        public override void SimulateUpdateHost()
        {
            if (!Lock)
            {
                LockPosition = CameraBase.transform.position + 10 * CameraBase.transform.forward;
            }

            LockPoint.transform.position = LockPosition;

            AxisLookAt(CameraBase.transform, LockPosition, Vector3.forward, 1f);

            ModNetworking.SendToAll(ClientLockPointPositionMsg.CreateMessage(myPlayerID, LockPosition));
            ModNetworking.SendToAll(ClientLockPointVelocityMsg.CreateMessage(myPlayerID, Vector3.zero));
            ModNetworking.SendToAll(ClientFOVMsg.CreateMessage(myPlayerID, FOV));
            ModNetworking.SendToAll(ClientLockMsg.CreateMessage(myPlayerID, Lock));
        }
        public override void SimulateFixedUpdateClient()
        {
            updateCameraParaClient();
        }
        public override void SimulateFixedUpdateHost()
        {
            updateCameraPara();
            if (!Lock)
            {
                DataManager.Instance.A2G_TargetData[myPlayerID].position = Vector3.zero;
            }
            else
            {
                DataManager.Instance.A2G_TargetData[myPlayerID].position = LockPosition;
            }
            
        }

        void OnGUI()
        {
            //GUI.Box(new Rect(100, 200, 200, 50), LockPosition.ToString());
            //GUI.Box(new Rect(100, 300, 200, 50), Lock.ToString());
            //GUI.Box(new Rect(100, 400, 200, 50), FOV.ToString());
        }


    }
}
