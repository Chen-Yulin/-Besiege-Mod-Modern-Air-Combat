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
    class HUDMsgReceiver : SingleInstance<HUDMsgReceiver>
    {
        public override string Name { get; } = "HUDMsgReceiver";

        public Vector3[] TargetPosition = new Vector3[16];
        public Vector3[] TargetVelocity = new Vector3[16];
        public float[] myVelocity = new float[16];
        public float[] Gvalue = new float[16];

        public void DataReceiver(Message msg)
        {
            int playerID = (int)msg.GetData(0);
            TargetPosition[playerID] = (Vector3)msg.GetData(1);
            TargetVelocity[playerID] = (Vector3)msg.GetData(2);
            myVelocity[playerID] = (float)msg.GetData(3);
            Gvalue[playerID] = (float)msg.GetData(4);
        }

    }
    public class HUDBlock:BlockScript
    {
        public MSlider BulletSpeed;

        //On panel fixed
        public GameObject Panel;
        public GameObject Aimer;
        public GameObject PitchBase;
        public GameObject PitchIcon;
        public MeshRenderer PitchIconMR;
        public GameObject SpeedBox;
        public GameObject GunAim;
        public GameObject A2GAim;
        public MeshRenderer A2GAimMR;

        //on panel unfixed
        public GameObject PitchInfo;
        public GameObject OverloadInfo;
        public GameObject SpeedInfo;
        public GameObject HeightBox;
        public GameObject HeightInfo;


        public Vector3 overload = Vector3.zero;
        public Vector3 preVelocity;

        public Rigidbody myRigid;
        public Transform myTransform;
        public int myPlayerID;

        public static MessageType ClientHUDBVRMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Vector3, DataType.Vector3
                                                                                    , DataType.Single, DataType.Single);
                                                                                    //playerID, targetPosition, targetVelocity, myVelocity, myG

        public IEnumerator refreshData;

        public Vector3 GunPredictPosition;
        public bool showPrediction;

        protected bool RefreshNeeded = false;
        protected bool GunAimStatus = false;
        protected bool PitchIconStatus = false;

        public void InitPanel()
        {
            if (!transform.FindChild("Panel"))
            {
                Panel = new GameObject("Panel");
                Panel.transform.SetParent(transform);
                Panel.transform.localPosition = new Vector3(0, 0f, 0.35f);
                Panel.transform.localRotation = Quaternion.Euler(180, 0, 0);
                Panel.transform.localScale = new Vector3(1f, 1f, 1f);
                Panel.SetActive(true);
            }
            if (!Panel.transform.FindChild("Aimer"))
            {
                Aimer = new GameObject("Aimer");
                Aimer.transform.SetParent(Panel.transform);
                Aimer.transform.localPosition = new Vector3(0, 0, 0f);
                Aimer.transform.localRotation = Quaternion.Euler(0, 0, 0);
                Aimer.transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);

                MeshFilter AimerMF = Aimer.AddComponent<MeshFilter>();
                AimerMF.mesh = ModResource.GetMesh("Plane Mesh").Mesh;
                MeshRenderer AimerMR = Aimer.AddComponent<MeshRenderer>();
                AimerMR.material = new Material(Shader.Find("Particles/Alpha Blended"));
                AimerMR.material.SetTexture("_MainTex", ModResource.GetTexture("HUDAimer Texture"));
                AimerMR.material.SetColor("_TintColor", Color.green);

                Aimer.SetActive(true);
            }
            if (!Panel.transform.FindChild("PitchBase"))
            {


                PitchBase = new GameObject("PitchBase");
                PitchBase.transform.SetParent(Panel.transform);
                PitchBase.transform.localPosition = new Vector3(0, 0, 0f);
                PitchBase.transform.localRotation = Quaternion.Euler(0, 0, 0);
                PitchBase.transform.localScale = new Vector3(1f, 1f, 1f);

                
            }
            if (!PitchBase.transform.Find("PitchIcon"))
            {
                PitchIcon = new GameObject("PitchIcon");
                PitchIcon.transform.SetParent(PitchBase.transform);
                PitchIcon.transform.localPosition = new Vector3(0, 0, 0f);
                PitchIcon.transform.localRotation = Quaternion.Euler(90, 0, 0);
                PitchIcon.transform.localScale = new Vector3(0.04f, 0.006f, 0.005f);

                MeshFilter PitchIconMF = PitchIcon.AddComponent<MeshFilter>();
                PitchIconMF.mesh = ModResource.GetMesh("Plane Mesh").Mesh;
                PitchIconMR = PitchIcon.AddComponent<MeshRenderer>();
                PitchIconMR.material = new Material(Shader.Find("Particles/Alpha Blended"));
                PitchIconMR.material.SetTexture("_MainTex", ModResource.GetTexture("HUDPitchUp Texture"));
                PitchIconMR.material.SetColor("_TintColor", Color.green);

                PitchIcon.SetActive(true);
            }
            if (!Panel.transform.Find("SpeedBox"))
            {
                SpeedBox = new GameObject("SpeedBox");
                SpeedBox.transform.SetParent(Panel.transform);
                SpeedBox.transform.localPosition = new Vector3(0.097f, 0, -0.1f);
                SpeedBox.transform.localRotation = Quaternion.Euler(0, 0, 0);
                SpeedBox.transform.localScale = new Vector3(0.032f, 0.006f, 0.015f);

                MeshFilter SpeedBoxMF = SpeedBox.AddComponent<MeshFilter>();
                SpeedBoxMF.mesh = ModResource.GetMesh("Plane Mesh").Mesh;
                MeshRenderer SpeedBoxMR = SpeedBox.AddComponent<MeshRenderer>();
                SpeedBoxMR.material = new Material(Shader.Find("Particles/Alpha Blended"));
                SpeedBoxMR.material.SetTexture("_MainTex", ModResource.GetTexture("DataBox Texture"));
                SpeedBoxMR.material.SetColor("_TintColor", Color.green);

                SpeedBox.SetActive(true);
            }
            if (!Panel.transform.Find("HeightBox"))
            {
                HeightBox = new GameObject("HeightBox");
                HeightBox.transform.SetParent(Panel.transform);
                HeightBox.transform.localPosition = new Vector3(-0.097f, 0, -0.1f);
                HeightBox.transform.localRotation = Quaternion.Euler(0, 0, 0);
                HeightBox.transform.localScale = new Vector3(0.032f, 0.006f, 0.015f);

                MeshFilter HeightBoxMF = HeightBox.AddComponent<MeshFilter>();
                HeightBoxMF.mesh = ModResource.GetMesh("Plane Mesh").Mesh;
                MeshRenderer HeightBoxMR = HeightBox.AddComponent<MeshRenderer>();
                HeightBoxMR.material = new Material(Shader.Find("Particles/Alpha Blended"));
                HeightBoxMR.material.SetTexture("_MainTex", ModResource.GetTexture("DataBox Texture"));
                HeightBoxMR.material.SetColor("_TintColor", Color.green);

                HeightBox.SetActive(true);
            }
            if (!Panel.transform.Find("GunAim"))
            {
                GunAim = new GameObject("GunAim");
                GunAim.transform.SetParent(Panel.transform);
                GunAim.transform.localPosition = new Vector3(0f, 0, -0f);
                GunAim.transform.localRotation = Quaternion.Euler(0, 0, 0);
                GunAim.transform.localScale = new Vector3(0.035f, 0.035f, 0.035f);

                MeshFilter GunAimMF = GunAim.AddComponent<MeshFilter>();
                GunAimMF.mesh = ModResource.GetMesh("Plane Mesh").Mesh;
                MeshRenderer GunAimMR = GunAim.AddComponent<MeshRenderer>();
                GunAimMR.material = new Material(Shader.Find("Particles/Alpha Blended"));
                GunAimMR.material.SetTexture("_MainTex", ModResource.GetTexture("GunAim Texture"));
                GunAimMR.material.SetColor("_TintColor", Color.green);

                GunAim.SetActive(false);
            }
            if (!Panel.transform.Find("A2GAim"))
            {
                A2GAim = new GameObject("A2GAim");
                A2GAim.transform.SetParent(Panel.transform);
                A2GAim.transform.localPosition = new Vector3(0f, 0, -0f);
                A2GAim.transform.localRotation = Quaternion.Euler(0, 0, 0);
                A2GAim.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

                MeshFilter A2GAimMF = A2GAim.AddComponent<MeshFilter>();
                A2GAimMF.mesh = ModResource.GetMesh("Plane Mesh").Mesh;
                A2GAimMR = A2GAim.AddComponent<MeshRenderer>();
                A2GAimMR.material = new Material(Shader.Find("Particles/Alpha Blended"));
                A2GAimMR.material.SetTexture("_MainTex", ModResource.GetTexture("HUDA2GAim Texture"));
                A2GAimMR.material.SetColor("_TintColor", Color.green);

                A2GAim.SetActive(false);
            }

        }

        IEnumerator RefreshData()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.2f);
                RefreshNeeded = true;
            }
        }
        public void UpdateNumber()
        {
            //control the updating rate
            if (!RefreshNeeded)
            {
                return;
            }
            else
            {
                RefreshNeeded = false;
            }

            //pitchInfo
            try
            {
                Destroy(PitchBase.transform.FindChild("PitchInfo").gameObject);
            }
            catch { }
            PitchInfo = new GameObject("PitchInfo");
            PitchInfo.transform.SetParent(PitchBase.transform);
            PitchInfo.transform.localPosition = new Vector3(0.05f, 0f, 0f);
            PitchInfo.transform.localRotation = Quaternion.Euler(0, 180, 0);
            PitchInfo.transform.localScale = new Vector3(1f, 1f, 1f);
            TextMesh PitchInfoText = PitchInfo.AddComponent<TextMesh>();
            PitchInfoText.text = Math.Round(-Vector3.Angle(Vector3.up, transform.up) + 90).ToString();
            PitchInfoText.fontSize = 16;
            PitchInfoText.fontStyle = FontStyle.Normal;
            PitchInfoText.anchor = TextAnchor.MiddleRight;
            PitchInfoText.characterSize = 0.015f;
            PitchInfoText.color = Color.green;
            PitchInfo.SetActive(true);
            if (Math.Round(-Vector3.Angle(Vector3.up, transform.up) + 90)<0 && PitchIconStatus == false)
            {
                PitchIconMR.material.SetTexture("_MainTex", ModResource.GetTexture("HUDPitchDown Texture"));
                PitchIconStatus = true;
            }
            else if(Math.Round(-Vector3.Angle(Vector3.up, transform.up) + 90) >= 0 && PitchIconStatus == true)
            {
                PitchIconMR.material.SetTexture("_MainTex", ModResource.GetTexture("HUDPitchUp Texture"));
                PitchIconStatus = false;
            }

            //overload info
            try
            {
                Destroy(Panel.transform.FindChild("OverloadInfo").gameObject);
            }
            catch { }
            OverloadInfo = new GameObject("OverloadInfo");
            OverloadInfo.transform.SetParent(Panel.transform);
            OverloadInfo.transform.localPosition = new Vector3(0.12f, -0.0f, 0.1f);
            OverloadInfo.transform.localRotation = Quaternion.Euler(90, 180, 0);
            OverloadInfo.transform.localScale = new Vector3(1f, 1f, 1f);
            TextMesh OverloadInfoText = OverloadInfo.AddComponent<TextMesh>();
            if (!StatMaster.isClient)
            {
                OverloadInfoText.text = "G  " + Math.Round(overload.magnitude / 9.8f, 1).ToString();
            }
            else
            {
                OverloadInfoText.text = "G  " + Math.Round(HUDMsgReceiver.Instance.Gvalue[myPlayerID] / 9.8f, 1).ToString();
            }
            OverloadInfoText.fontSize = 16;
            OverloadInfoText.fontStyle = FontStyle.Normal;
            OverloadInfoText.anchor = TextAnchor.MiddleLeft;
            OverloadInfoText.characterSize = 0.015f;
            OverloadInfoText.color = Color.green;
            OverloadInfo.SetActive(true);

            try
            {
                Destroy(Panel.transform.FindChild("SpeedInfo").gameObject);
            }
            catch { }
            SpeedInfo = new GameObject("SpeedInfo");
            SpeedInfo.transform.SetParent(Panel.transform);
            SpeedInfo.transform.localPosition = new Vector3(0.07f, -0.0f, -0.1f);
            SpeedInfo.transform.localRotation = Quaternion.Euler(90, 180, 0);
            SpeedInfo.transform.localScale = new Vector3(1f, 1f, 1f);
            TextMesh SpeedInfoText = SpeedInfo.AddComponent<TextMesh>();
            if (!StatMaster.isClient)
            {
                SpeedInfoText.text = Math.Min((Math.Round(myRigid.velocity.magnitude * 0.36f) * 10f), 9999).ToString();
            }
            else
            {
                SpeedInfoText.text = Math.Min((Math.Round(HUDMsgReceiver.Instance.myVelocity[myPlayerID] * 0.36f) * 10f), 9999).ToString();
            }
            SpeedInfoText.fontSize = 16;
            SpeedInfoText.fontStyle = FontStyle.Normal;
            SpeedInfoText.anchor = TextAnchor.MiddleRight;
            SpeedInfoText.characterSize = 0.015f;
            SpeedInfoText.color = Color.green;
            SpeedInfo.SetActive(true);

            try
            {
                Destroy(Panel.transform.FindChild("HeightInfo").gameObject);
            }
            catch { }
            HeightInfo = new GameObject("HeightInfo");
            HeightInfo.transform.SetParent(Panel.transform);
            HeightInfo.transform.localPosition = new Vector3(-0.123f, -0.0f, -0.1f);
            HeightInfo.transform.localRotation = Quaternion.Euler(90, 180, 0);
            HeightInfo.transform.localScale = new Vector3(1f, 1f, 1f);
            TextMesh HeightInfoText = HeightInfo.AddComponent<TextMesh>();
            HeightInfoText.text = Math.Min((Math.Round(transform.position.y * 0.1f) * 10f), 9999).ToString();
            HeightInfoText.fontSize = 16;
            HeightInfoText.fontStyle = FontStyle.Normal;
            HeightInfoText.anchor = TextAnchor.MiddleRight;
            HeightInfoText.characterSize = 0.015f;
            HeightInfoText.color = Color.green;
            HeightInfo.SetActive(true);
        }

        public void UpdateGunPrediction()
        {
            if (!StatMaster.isClient)
            {
                if (DataManager.Instance.BVRData[myPlayerID].position == Vector3.zero)
                {
                    showPrediction = false;
                    if (GunAim.activeSelf)
                    {
                        GunAim.SetActive(false);
                    }
                }
                else
                {
                    showPrediction = true;
                    calculatePredictPosition(   DataManager.Instance.BVRData[myPlayerID].position, 
                                                DataManager.Instance.BVRData[myPlayerID].velocity, 
                                                myTransform.position, 
                                                myRigid.velocity.magnitude+BulletSpeed.Value);

                    Vector3 originLocal = GunAim.transform.localPosition;
                    GunAim.transform.position = calculatePredictedIcon(DataManager.Instance.BVRData[myPlayerID].position);
                    if (Mathf.Abs(GunAim.transform.localPosition.x) > 0.1f || Mathf.Abs(GunAim.transform.localPosition.z) > 0.1f)
                    {
                        Vector3 localpositiontmp = Vector3.zero;
                        localpositiontmp.x = Mathf.Min(Mathf.Max(GunAim.transform.localPosition.x, -0.1f), 0.1f);
                        localpositiontmp.z = Mathf.Min(Mathf.Max(GunAim.transform.localPosition.z, -0.1f), 0.1f);
                        if (!GunAimStatus)
                        {
                            GunAim.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", ModResource.GetTexture("GunAim2 Texture"));
                            GunAimStatus = true;
                        }
                        GunAim.transform.localPosition = Vector3.Lerp(originLocal, localpositiontmp, 0.1f);
                    }
                    else
                    {
                        if (GunAimStatus)
                        {
                            GunAim.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", ModResource.GetTexture("GunAim Texture"));
                            GunAimStatus = false;
                        }
                        GunAim.transform.localPosition = Vector3.Lerp(originLocal, GunAim.transform.localPosition, 0.1f);

                    }

                    if (!GunAim.activeSelf)
                    {
                        GunAim.SetActive(true);
                    }
                }
            }
            else // is Client
            {
                if (HUDMsgReceiver.Instance.TargetPosition[myPlayerID] == Vector3.zero)
                {
                    showPrediction = false;
                    if (GunAim.activeSelf)
                    {
                        GunAim.SetActive(false);
                    }
                }
                else
                {
                    showPrediction = true;
                    calculatePredictPosition(HUDMsgReceiver.Instance.TargetPosition[myPlayerID], 
                                            HUDMsgReceiver.Instance.TargetVelocity[myPlayerID], 
                                            myTransform.position,
                                            HUDMsgReceiver.Instance.myVelocity[myPlayerID]+BulletSpeed.Value);

                    Vector3 originLocal = GunAim.transform.localPosition;
                    GunAim.transform.position = calculatePredictedIcon(HUDMsgReceiver.Instance.TargetPosition[myPlayerID]);
                    if (Mathf.Abs(GunAim.transform.localPosition.x) > 0.1f || Mathf.Abs(GunAim.transform.localPosition.z) > 0.1f)
                    {
                        Vector3 localpositiontmp = Vector3.zero;
                        localpositiontmp.x = Mathf.Min(Mathf.Max(GunAim.transform.localPosition.x, -0.1f), 0.1f);
                        localpositiontmp.z = Mathf.Min(Mathf.Max(GunAim.transform.localPosition.z, -0.1f), 0.1f);
                        if (!GunAimStatus)
                        {
                            GunAim.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", ModResource.GetTexture("GunAim2 Texture"));
                            GunAimStatus = true;
                        }
                        GunAim.transform.localPosition = Vector3.Lerp(originLocal, localpositiontmp, 0.1f);
                    }
                    else
                    {
                        if (GunAimStatus)
                        {
                            GunAim.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", ModResource.GetTexture("GunAim Texture"));
                            GunAimStatus = false;
                        }
                        GunAim.transform.localPosition = Vector3.Lerp(originLocal, GunAim.transform.localPosition, 0.1f);

                    }

                    if (!GunAim.activeSelf)
                    {
                        GunAim.SetActive(true);
                    }
                }
            }
            
            
        }

        public void updateA2GIcon()
        {
            if (DataManager.Instance.TV_Lock[myPlayerID])
            {
                A2GAim.SetActive(true);
            }
            else
            {
                A2GAim.SetActive(false);
            }
            A2GAim.transform.position = calculateA2GIcon();

            if (Mathf.Abs(A2GAim.transform.localPosition.x) > 0.1f || Mathf.Abs(A2GAim.transform.localPosition.z) > 0.1f)
            {
                Vector3 localpositiontmp = Vector3.zero;
                localpositiontmp.x = Mathf.Min(Mathf.Max(A2GAim.transform.localPosition.x, -0.1f), 0.1f);
                localpositiontmp.z = Mathf.Min(Mathf.Max(A2GAim.transform.localPosition.z, -0.1f), 0.1f);
                A2GAimMR.material.SetTexture("_MainTex", ModResource.GetTexture("HUDA2GAim2 Texture"));
                A2GAim.transform.localPosition = localpositiontmp;
            }
            else
            {
                A2GAimMR.material.SetTexture("_MainTex", ModResource.GetTexture("HUDA2GAim Texture"));

            }
        }

        public static Vector3 GetIntersectWithLineAndPlane(Vector3 point, Vector3 direct, Vector3 planeNormal, Vector3 planePoint)
        {
            float d = Vector3.Dot(planePoint - point, planeNormal) / Vector3.Dot(direct.normalized, planeNormal);
            return d * direct.normalized + point;
        }
        public void calculateOverload()//put in fixed update
        {
            overload = (myRigid.velocity - preVelocity) / Time.fixedDeltaTime;
            overload += new Vector3(0, 9.8f, 0);
            preVelocity = myRigid.velocity;
        }

        public void calculatePredictPosition(Vector3 targetPosition, Vector3 targetVelocity, Vector3 myPosition, float BulletSpeed)
        {
            float estimatedTime;
            estimatedTime = (targetPosition - transform.position).magnitude / BulletSpeed;
            GunPredictPosition = targetPosition + targetVelocity * estimatedTime;
            estimatedTime = (GunPredictPosition - transform.position).magnitude / BulletSpeed;
            GunPredictPosition = targetPosition + targetVelocity * estimatedTime;
        }

        public Vector3 calculatePredictedIcon(Vector3 targetPosition)
        {
            Vector3 res = Vector3.zero;

            Vector3 targetOnScreen = GetIntersectWithLineAndPlane(targetPosition, Camera.main.transform.position - targetPosition, transform.up, transform.position);
            Vector3 PredictionOnScreen = GetIntersectWithLineAndPlane(GunPredictPosition, Camera.main.transform.position - GunPredictPosition, transform.up, transform.position);
            res = Panel.transform.position + (targetOnScreen - PredictionOnScreen);
            return res;
        }
        public Vector3 calculateA2GIcon()
        {
            Vector3 A2GtargetPosition = DataManager.Instance.A2G_TargetData[myPlayerID].position;
            return GetIntersectWithLineAndPlane( A2GtargetPosition,
                                                Camera.main.transform.position - A2GtargetPosition,
                                                transform.up, transform.position);
        }

        public override void SafeAwake()
        {
            BulletSpeed = AddSlider("Bullet Initial Speed", "Bullet Initial Speed", 800, 400f, 1000f);

            myTransform = transform;
            myRigid = GetComponent<Rigidbody>();
            myPlayerID = BlockBehaviour.ParentMachine.PlayerID;
            refreshData = RefreshData();

            InitPanel();
        }
        protected void Update()
        {
            if (IsSimulating)
            {
                PitchBase.transform.rotation = Quaternion.LookRotation((Panel.transform.rotation * Vector3.up).normalized);
                UpdateNumber();

            }
        }
        public override void OnSimulateStart()
        {
            StartCoroutine(refreshData);
        }

        public override void OnSimulateStop()
        {
            StopCoroutine(refreshData);
        }

        public override void SimulateFixedUpdateHost()
        {
            if (StatMaster.isMP)
            {
                ModNetworking.SendToAll(ClientHUDBVRMsg.CreateMessage(  myPlayerID, 
                                                                        DataManager.Instance.BVRData[myPlayerID].position, 
                                                                        DataManager.Instance.BVRData[myPlayerID].velocity,
                                                                        myRigid.velocity.magnitude,
                                                                        overload.magnitude));

            }
            calculateOverload();

            UpdateGunPrediction();
            //updateA2GIcon();
        }

        public override void SimulateFixedUpdateClient()
        {
            UpdateGunPrediction();
            //updateA2GIcon();
        }

        void OnGUI()
        {
            //GUI.Box(new Rect(100, 200, 200, 50), DataManager.Instance.A2G_TargetData[myPlayerID].position.ToString());
        }



    }
}
