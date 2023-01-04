using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Collections;

using Modding.Modules;
using Modding;
using Modding.Blocks;
using UnityEngine;
using UnityEngine.UI;

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

    public class HUDPanelFollowCamera : MonoBehaviour
    {
        Camera myCamera;
        Transform HUDTransform;
        Transform BlockTransform;
        public Vector3 myInitialScale;
        public float BaseScale = 1;

        public void UpdatePosition()
        {
            Vector3 Blocklossy = BlockTransform.lossyScale;
            transform.localScale = BaseScale * new Vector3(myInitialScale.x / Blocklossy.x, myInitialScale.y / Blocklossy.z, myInitialScale.z / Blocklossy.y);
            transform.position = myCamera.transform.position + 0.7f * HUDTransform.forward;
        }
        // Use this for initialization
        void Start()
        {
            myInitialScale = transform.localScale;
            BlockTransform = transform.parent.parent.parent;
            myCamera = Camera.main;
            HUDTransform = transform.parent.parent.transform;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (StatMaster.isClient)
            {
                return;
            }
            UpdatePosition();
        }
        void OnGUI()
        {
            if (StatMaster.isClient)
            {
                UpdatePosition();
            }
        }
    }
    public class HUDPitchFollowCamera : MonoBehaviour
    {
        Camera myCamera;
        Transform HUDTransform;
        Transform BlockTransform;
        public Vector3 myInitialScale;

        public void UpdatePosition()
        {
            Vector3 Blocklossy = BlockTransform.lossyScale;
            transform.localScale = new Vector3(myInitialScale.x / Blocklossy.x, myInitialScale.y / Blocklossy.z, myInitialScale.z / Blocklossy.y);
            transform.position = myCamera.transform.position;
            transform.rotation = Quaternion.LookRotation(BlockTransform.up);
            transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
        }
        // Use this for initialization
        void Start()
        {
            myInitialScale = transform.localScale;
            BlockTransform = transform.parent.parent.parent;
            myCamera = Camera.main;
            HUDTransform = transform.parent.parent.transform;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (StatMaster.isClient)
            {
                return;
            }
            UpdatePosition();
        }
        void OnGUI()
        {
            if (StatMaster.isClient)
            {
                UpdatePosition();
            }
        }
    }
    class MeterText
    {
        bool useComma, hasDynamicTexts;
        int scaleDigits, scale;
        Text m, t0, t1, t2, t3, t4;
        public MeterText(Transform stationary, Transform scroller, bool useComma, int scaleDigits)
        {
            // stationary text, like "1,234"
            this.m = stationary.GetComponent<Text>();
            // dynamic text
            t0 = scroller.Find("T0").GetComponent<Text>(); // "1,0"
            t1 = scroller.Find("T1").GetComponent<Text>(); // "1,1"
            t2 = scroller.Find("T2").GetComponent<Text>(); // "1,2"
            t3 = scroller.Find("T3").GetComponent<Text>(); // "1,3"
            t4 = scroller.Find("T4").GetComponent<Text>(); // "1,4"
            this.useComma = useComma; // false: "1234", true: "1,234"; use it for altitude text
            this.scaleDigits = scaleDigits;
            // scale=10^scaleDigits
            scale = 1;
            for (int i = 0; i < scaleDigits; i++)
            {
                scale *= 10;
            }
            hasDynamicTexts = true;
        }

        // initializer for stationary text only
        public MeterText(Transform m, int scaleDigits)
        {
            this.m = m.GetComponent<Text>();
            this.scaleDigits = scaleDigits;
            scale = 1;
            for (int i = 0; i < scaleDigits; i++)
            {
                scale *= 10;
            }
            hasDynamicTexts = false;
        }

        // format value into string


        // Update text in meter, return offset in (-0.5, 0.5)
        public float Update(float v)
        {
            m.text = v.ToString(); // stationary text, "1234.5" -> "1234"
            int mid = Mathf.RoundToInt(v / scale); // dynamic text, "1234.5" -> "12"
            if (hasDynamicTexts)
            {
                t0.text = ((mid - 2) * scale).ToString(); // "1,0"
                t1.text = ((mid - 1) * scale).ToString();
                t2.text = (mid * scale).ToString();
                t3.text = ((mid + 1) * scale).ToString();
                t4.text = ((mid + 2) * scale).ToString();
            }
            // (1234.5 - 1200) / 100 = 0.34; (1267.8 - 1300) / 100 = -0.32
            return (v - mid * scale) / scale;
        }
    }

    public class MeterController : MonoBehaviour
    {
        public float speed, alt, g, mach, gMax;
        float yAir, yAlt, vertScale = 0.12f;

        Transform airSpeed, altimeter;
        MeterText tSpeed, tAlt;
        void Awake()
        {
            Transform mask = transform;
            airSpeed = mask.Find("Airspeed");
            altimeter = mask.Find("Altimeter");

            Transform staticText = mask.parent.Find("StaticText");
            tSpeed = new MeterText(staticText.Find("VelocityText"), airSpeed, false, 2);
            tAlt = new MeterText(staticText.Find("HeightText"), altimeter, true, 2);
        }

        // update text in static regions


        // update position/text of dynamic components
        void UpdateUI()
        {
            Vector3 pos;
            pos = airSpeed.localPosition;
            // texture offset(yAir) + scroll offset
            pos.y = yAir + tSpeed.Update(speed*3.6f) * vertScale;
            airSpeed.localPosition = pos;

            pos = altimeter.localPosition;
            pos.y = yAlt + tAlt.Update(alt) * vertScale;
            altimeter.localPosition = pos;
        }



        // editor scene support
        void OnValidate()
        {
            if (tSpeed == null)
            {
                Awake();
            }
            // default scale of HUD image/draw plane in scene
            FixedUpdate();
        }

        void FixedUpdate()
        {
            UpdateUI();
        }
    }
    public class PredictionAimBehavior : MonoBehaviour
    {
        public bool AimOn;
        private GameObject Solid;
        private GameObject Imaginary;
        private GameObject Line;
        public float maxIconDist = 0.4f;
        private float distFromCenter;
        // Use this for initialization
        void Start()
        {
            Solid = transform.Find("Solid").gameObject;
            Imaginary = transform.Find("Imaginary").gameObject;
            Line = transform.Find("Line").gameObject;
        }
        private float SignedAngle(Vector2 v1, Vector2 v2)
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
        private void UpdateLine()
        {
            Vector3 origin = Vector3.zero;
            Vector3 end = transform.localPosition;
            Line.transform.localPosition = (origin + end) / 2 - transform.localPosition;
            float Angle = SignedAngle(Vector2.right,
                new Vector2((origin - end).x, (origin - end).y));
            Line.transform.localRotation = Quaternion.Euler(0, 0, Angle);
            Line.transform.localScale = new Vector3(distFromCenter, 0.01f, 0.01f);
            //Debug.Log (Angle);
        }
        // Update is called once per frame
        void LateUpdate()
        {
            if (AimOn)
            {
                distFromCenter = transform.localPosition.magnitude;
                if (distFromCenter >= maxIconDist)
                {
                    transform.localPosition = (maxIconDist+0.01f) * transform.localPosition.normalized;
                    distFromCenter = maxIconDist;
                    Solid.SetActive(false);
                    Imaginary.SetActive(true);
                }
                else
                {
                    Solid.SetActive(true);
                    Imaginary.SetActive(false);
                }
                Line.SetActive(true);
                UpdateLine();
            }
            else
            {
                Solid.SetActive(false);
                Imaginary.SetActive(false);
                Line.SetActive(false);
            }

        }
    }



    public class HUDController : MonoBehaviour
    {
        public float velocity = 0;
        public float height = 0;
        public float overload = 0;
        public Text VelocityText;
        public Text HeightText;
        public Text OverloadText;
        // Use this for initialization
        void Start()
        {
            VelocityText = transform.FindChild("Mask").FindChild("Base").FindChild("StaticText").FindChild("VelocityText").GetComponent<Text>();
            HeightText = transform.FindChild("Mask").FindChild("Base").FindChild("StaticText").FindChild("HeightText").GetComponent<Text>();
            OverloadText = transform.FindChild("Mask").FindChild("Base").FindChild("StaticText").FindChild("OverLoad").GetComponent<Text>();
        }

        // Update is called once per frame
        void LateUpdate()
        {
            VelocityText.text = Mathf.Clamp(Mathf.Round(velocity * 0.36f) * 10f, 0, 9999).ToString();
            HeightText.text = Mathf.Clamp(Mathf.Round(height * 0.1f) * 10f, 0, 99999).ToString();
            OverloadText.text = "G  " + Mathf.Clamp(Mathf.Round(overload * 1.02f) / 10, 0, 99).ToString();

            transform.FindChild("Mask").FindChild("Base").FindChild("Mask").gameObject.GetComponent<MeterController>().speed = velocity;
            transform.FindChild("Mask").FindChild("Base").FindChild("Mask").gameObject.GetComponent<MeterController>().alt = height;
        }
    }

    public class HUDBlock:BlockScript
    {
        public MSlider BulletSpeed;
        public MSlider maxIconDist;
        public MColourSlider HUDColor;
        public MSlider HUDTransparency;
        public MSlider PanelBaseScale;

        //On panel fixed
        public GameObject Panel;
        public GameObject Panelbase;
        public GameObject Aimer;
        public GameObject PitchBase;
        public GameObject PitchIcon;
        public MeshRenderer PitchIconMR;
        public GameObject SpeedBox;
        public GameObject GunAim;
        public GameObject A2GAim;
        public MeshRenderer A2GAimMR;

        //on panel unfixed
        public Text OverloadInfo;
        public Text SpeedInfo;
        public Text HeightInfo;

        public HUDController hudController;
        public PredictionAimBehavior GunAimController;
        

        public Vector3 overload = Vector3.zero;
        public Vector3 preVelocity;

        public Rigidbody myRigid;
        public Transform myTransform;
        public int myPlayerID;

        public static MessageType ClientHUDBVRMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Vector3, DataType.Vector3
                                                                                    , DataType.Single, DataType.Single);
                                                                                    //playerID, targetPosition, targetVelocity, myVelocity, myG

        public Vector3 GunPredictPosition;
        public bool showPrediction;
        protected bool GunAimStatus = false;

        public void InitPanel()
        {
            if (!transform.FindChild("HUD(Clone)"))
            {
                Panel = Instantiate(AssetManager.Instance.HUD.HUD);
                Panel.transform.SetParent(transform);
                Panel.transform.localPosition = new Vector3(0, 0f, 0.35f);
                Panel.transform.localRotation = Quaternion.Euler(-90, 0, 180);
                Panel.transform.localScale = new Vector3(1f, 1f, 1f);
                Panel.SetActive(true);

                hudController = Panel.AddComponent<HUDController>();
                hudController.velocity = 0;
                hudController.height = 0;

                GunAim = Panel.transform.FindChild("Mask").FindChild("Base").FindChild("PredictionAim").gameObject;
                GunAimController = GunAim.AddComponent<PredictionAimBehavior>();
                GunAimController.AimOn = false;

                Panelbase = Panel.transform.FindChild("Mask").FindChild("Base").gameObject;
                DataManager.Instance.hudPanelFollowCamera.Add(Panelbase.AddComponent<HUDPanelFollowCamera>());
                DataManager.Instance.hudPitchFollowCamera.Add(Panel.transform.FindChild("Mask").FindChild("PitchIconBase").gameObject.AddComponent<HUDPitchFollowCamera>());
                Panel.transform.FindChild("Mask").FindChild("Base").FindChild("Mask").gameObject.AddComponent<MeterController>();
                

            }

        }
        public void UpdateNumber()
        {

            //overload info
            if (!StatMaster.isClient)
            {
                hudController.overload = overload.magnitude;
            }
            else
            {
                hudController.overload = HUDMsgReceiver.Instance.Gvalue[myPlayerID];
            }

            // speed info
            if (!StatMaster.isClient)
            {
                hudController.velocity = myRigid.velocity.magnitude;
            }
            else
            {
                hudController.velocity = HUDMsgReceiver.Instance.myVelocity[myPlayerID];
            }

            //height info
            hudController.height = transform.position.y;
        }
        public void UpdateGunPrediction()
        {
            if (!StatMaster.isClient)
            {
                if (DataManager.Instance.BVRData[myPlayerID].position == Vector3.zero)
                {
                    GunAimController.AimOn = false;
                }
                else
                {
                    GunAimController.AimOn = true;
                    calculatePredictPosition(   DataManager.Instance.BVRData[myPlayerID].position, 
                                                DataManager.Instance.BVRData[myPlayerID].velocity, 
                                                myTransform.position, 
                                                myRigid.velocity.magnitude+BulletSpeed.Value);

                    Vector3 originLocal = GunAim.transform.localPosition;
                    GunAim.transform.position = calculatePredictedIcon(DataManager.Instance.BVRData[myPlayerID].position);
                    GunAim.transform.localPosition = Vector3.Lerp(originLocal, GunAim.transform.localPosition, 0.2f);
                }
            }
            else // is Client
            {
                if (HUDMsgReceiver.Instance.TargetPosition[myPlayerID] == Vector3.zero)
                {
                    GunAimController.AimOn = false;
                }
                else
                {
                    GunAimController.AimOn = true;
                    calculatePredictPosition(HUDMsgReceiver.Instance.TargetPosition[myPlayerID], 
                                            HUDMsgReceiver.Instance.TargetVelocity[myPlayerID], 
                                            myTransform.position,
                                            HUDMsgReceiver.Instance.myVelocity[myPlayerID]+BulletSpeed.Value);

                    Vector3 originLocal = GunAim.transform.localPosition;
                    GunAim.transform.position = calculatePredictedIcon(HUDMsgReceiver.Instance.TargetPosition[myPlayerID]);
                    GunAim.transform.localPosition = Vector3.Lerp(originLocal, GunAim.transform.localPosition, 0.2f);
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
            estimatedTime = (GunPredictPosition - transform.position).magnitude / BulletSpeed;
            GunPredictPosition = targetPosition + targetVelocity * estimatedTime;
            //gravity modification
            GunPredictPosition += estimatedTime * estimatedTime * 0.5f * 32 * Vector3.up;
        }
        public Vector3 calculatePredictedIcon(Vector3 targetPosition)
        {
            Vector3 res = Vector3.zero;

            Vector3 targetOnScreen = GetIntersectWithLineAndPlane(targetPosition, Camera.main.transform.position - targetPosition, transform.up, Camera.main.transform.position + 0.7f * transform.up);
            Vector3 PredictionOnScreen = GetIntersectWithLineAndPlane(GunPredictPosition, Camera.main.transform.position - GunPredictPosition, transform.up, Camera.main.transform.position + 0.7f * transform.up);
            res = Panelbase.transform.position + (targetOnScreen - PredictionOnScreen);
            return res;
        }
        public Vector3 calculateA2GIcon()
        {
            Vector3 A2GtargetPosition = DataManager.Instance.A2G_TargetData[myPlayerID].position;
            return GetIntersectWithLineAndPlane( A2GtargetPosition,
                                                Camera.main.transform.position - A2GtargetPosition,
                                                transform.up, transform.position);
        }
        public void resetHUDOnSimulate()
        {
            Transform Base = Panel.transform.FindChild("Mask").FindChild("Base");
            Transform PitchIconBase = Panel.transform.FindChild("Mask").FindChild("PitchIconBase");
            Base.localScale = Base.gameObject.GetComponent<HUDPanelFollowCamera>().myInitialScale;
            PitchIconBase.localScale = PitchIconBase.gameObject.GetComponent<HUDPitchFollowCamera>().myInitialScale;
            DestroyImmediate(Base.gameObject.GetComponent<HUDPanelFollowCamera>());
            DestroyImmediate(PitchIconBase.gameObject.GetComponent<HUDPitchFollowCamera>());
            Base.gameObject.AddComponent<HUDPanelFollowCamera>();
            PitchIconBase.gameObject.AddComponent<HUDPitchFollowCamera>();
            hudController = Panel.GetComponent<HUDController>();
            GunAimController.maxIconDist = maxIconDist.Value;
        }

        public override void SafeAwake()
        {
            PanelBaseScale = AddSlider("Panel Scale", "PanelScale", 1f, 0.5f, 2f);
            maxIconDist = AddSlider("Float Icon Range", "maxIconDist", 0.4f, 0.3f, 1f);
            BulletSpeed = AddSlider("Bullet Initial Speed", "Bullet Initial Speed", 800, 400f, 1600f);
            HUDColor = AddColourSlider("HUD Color", "HUDColor", Color.black, false);
            HUDTransparency = AddSlider("HUD Transparency", "HUDTransparency", 0.8f, 0f, 1f);

            myTransform = transform;
            myRigid = GetComponent<Rigidbody>();
            myPlayerID = BlockBehaviour.ParentMachine.PlayerID;

            InitPanel();
        }
        protected void Update()
        {
            if (IsSimulating)
            {
                UpdateNumber();
            }
        }
        public override void BuildingUpdate()
        {
            RawImage glass = Panel.transform.FindChild("Mask").GetComponent<RawImage>();
            Color tmpColor = HUDColor.Value;
            tmpColor.a = 1 - HUDTransparency.Value;
            glass.color = tmpColor;
            Panelbase.GetComponent<HUDPanelFollowCamera>().BaseScale = PanelBaseScale.Value;
        }
        public override void OnSimulateStart()
        {
            resetHUDOnSimulate();
            RawImage glass = Panel.transform.FindChild("Mask").GetComponent<RawImage>();
            Color tmpColor = HUDColor.Value;
            tmpColor.a = 1 - HUDTransparency.Value;
            glass.color = tmpColor;
            Panelbase.GetComponent<HUDPanelFollowCamera>().BaseScale = PanelBaseScale.Value;
        }

        public override void OnSimulateStop()
        {
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
        }

        public override void SimulateFixedUpdateClient()
        {
            UpdateGunPrediction();
        }

        void OnGUI()
        {
            //GUI.Box(new Rect(100, 200, 200, 50), transform.up.ToString());
        }



    }
}
