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
    public class MFDMsgReceiver : SingleInstance<MFDMsgReceiver>
    {
        public override string Name { get; } = "MFD Msg Receiver";
        public Dictionary<int,int>[] ScreenType = new Dictionary<int, int>[16];

        public MFDMsgReceiver()
        {
            for (int i = 0; i < 16; i++)
            {
                ScreenType[i] = new Dictionary<int, int>();
            }
        }

        public void ScreenTypeMsgReceiver(Message msg)
        {
            if (ScreenType[(int)msg.GetData(0)].ContainsKey((int)msg.GetData(1)))
            {
                ScreenType[(int)msg.GetData(0)][(int)msg.GetData(1)] = (int)msg.GetData(2);
            }
            else
            {
                ScreenType[(int)msg.GetData(0)].Add((int)msg.GetData(1),(int)msg.GetData(2));
            }
        }
    }

    public class NAVController : MonoBehaviour
    {
        public int myPlayerID;

        public Vector3[] dist = new Vector3[8];
        public bool[] hasWP = new bool[8];
        public string[] WPName = new string[8];
        public float orientation = 0;
        public Vector3 myPosition = Vector3.zero;

        GameObject rotationObject;
        GameObject[] WP = new GameObject[8];
        GameObject Line;
        Text CurrWPx;
        Text CurrWPy;
        Text CurrWPz;
        Text CurrWPName;
        Text ScaleText;

        int selected = 0;
        public float scale = 0.0001f;

        public Vector2 PointRotate(Vector3 Point, float angle)
        {
            return new Vector2(Mathf.Cos(angle) * Point.x + Mathf.Sin(angle) * Point.y, -Mathf.Sin(angle) * Point.x + Mathf.Cos(angle) * Point.y);
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

        private void DrawLine(Vector2 origin, Vector2 end)
        {
            Vector2 backDirection = new Vector2(origin.x - end.x, origin.y - end.y);
            float xModifier, yModifier;
            if (end.x > 0.1f || end.x < -0.1f)
            {
                xModifier = 1 - Mathf.Abs(0.1f / end.x);
            }
            else
            {
                xModifier = 0;
            }
            end += xModifier * backDirection;

            backDirection = new Vector2(origin.x - end.x, origin.y - end.y);
            if (end.y > 0.1f)
            {
                yModifier = 1 - Mathf.Abs(0.16f / (end.y + 0.06f));
            }
            else if (end.y < -0.1f)
            {
                yModifier = 1 - Mathf.Abs(0.04f / (-end.y - 0.06f));
            }
            else
            {
                yModifier = 0;
            }
            end += yModifier * backDirection;

            Line.transform.localPosition = (origin + end) / 2;
            float Angle = SignedAngle(Vector2.right,
                new Vector2((origin - end).x, (origin - end).y));
            Line.transform.localRotation = Quaternion.Euler(0, 0, Angle);
            Line.transform.localScale = new Vector3((end - origin).magnitude, 0.001f, 0.001f);
        }

        public bool NoneWP()
        {
            for (int i = 0; i < 8; i++)
            {
                if (hasWP[i])
                {
                    return false;
                }
            }
            return true;
        }

        public void SelectNext()
        {
            if (!NoneWP())
            {
                selected++;
                while (!hasWP[selected])
                {
                    selected++;
                    if (selected >= 8)
                    {
                        selected = 0;
                    }
                }
            }
        }

        public void ScaleDec()
        {
            scale /= 2;
        }

        public void ScaleInc()
        {
            scale *= 2;
        }

        public void UpdateWP()
        {
            for (int i = 0; i < 8; i++)
            {
                if (hasWP[i])
                {
                    WP[i].SetActive(true);
                    WP[i].transform.localPosition = new Vector3(dist[i].x - myPosition.x, dist[i].z - myPosition.z, 0) * scale;
                    WP[i].transform.localRotation = Quaternion.Euler(   WP[i].transform.localEulerAngles.x,
                                                                        WP[i].transform.localEulerAngles.y, 
                                                                        -orientation);
                }
                else
                {
                    WP[i].SetActive(false);
                }
            }
        }
        public void UpdateLine()
        {
            Vector2 origin = new Vector2(rotationObject.transform.localPosition.x, rotationObject.transform.localPosition.y);
            Vector2 end = origin + PointRotate(WP[selected].transform.localPosition, -orientation * Mathf.PI / 180);
            DrawLine(origin, end);
        }
        public void UpdateText()
        {
            if (hasWP[selected])
            {
                CurrWPName.text = WPName[selected];
                CurrWPx.text = dist[selected].x.ToString();
                CurrWPy.text = dist[selected].y.ToString();
                CurrWPz.text = dist[selected].z.ToString();
            }
            else
            {
                CurrWPName.text = "";
                CurrWPx.text = "";
                CurrWPy.text = "";
                CurrWPz.text = "";
            }
            ScaleText.text = (0.0001f / scale).ToString() + "km";
        }
        public void SyncData()
        {
            for (int i = 0; i < 8; i++)
            {
                dist[i] = CC2NavDisplayerData.Instance.dist[myPlayerID][i];
                hasWP[i] = CC2NavDisplayerData.Instance.hasWP[myPlayerID][i];
                WPName[i] = CC2NavDisplayerData.Instance.WPName[myPlayerID][i];
                orientation = CC2NavDisplayerData.Instance.orientation[myPlayerID];
                myPosition = CC2NavDisplayerData.Instance.myPosition[myPlayerID];
            }
        }
        public void KeyAction()
        {
            if (CC2NavDisplayerData.Instance.ScaleIncPressed[myPlayerID])
            {
                CC2NavDisplayerData.Instance.ScaleIncPressed[myPlayerID] = false;
                ScaleInc();
            }
            if (CC2NavDisplayerData.Instance.ScaleDecPressed[myPlayerID])
            {
                CC2NavDisplayerData.Instance.ScaleDecPressed[myPlayerID] = false;
                ScaleDec();
            }
            if (CC2NavDisplayerData.Instance.ChangeSelection[myPlayerID])
            {
                CC2NavDisplayerData.Instance.ChangeSelection[myPlayerID] = false;
                SelectNext();
            }
        }

        // Use this for initialization
        void Start()
        {
            rotationObject = transform.Find("Mask").Find("rotate").gameObject;
            Line = transform.Find("Mask").Find("Line").gameObject;
            for (int i = 0; i < 8; i++)
            {
                WP[i] = rotationObject.transform.Find("WPs").Find("WP" + i.ToString()).gameObject;
            }
            Transform fixedObject = transform.Find("Mask").Find("fixed");
            CurrWPName = fixedObject.Find("WPName").GetComponent<Text>();
            CurrWPx = fixedObject.Find("WPx").GetComponent<Text>();
            CurrWPy = fixedObject.Find("WPy").GetComponent<Text>();
            CurrWPz = fixedObject.Find("WPz").GetComponent<Text>();
            ScaleText = fixedObject.Find("scale").GetComponent<Text>();
        }

        // Update is called once per frame
        void Update()
        {
            SyncData();
            KeyAction();
            rotationObject.transform.localRotation = Quaternion.Euler(0, 0, orientation);
            UpdateWP();
            if (hasWP[selected])
            {
                UpdateLine();
                Line.SetActive(true);
            }
            else
            {
                Line.SetActive(false);
            }
            UpdateText();
            

        }
    }

    public class RadarScreen_MFD : MonoBehaviour
    {
        public int myPlayerID;
        public bool isClient;
        public GameObject radarScreen;
        public GameObject scanLine;
        public GameObject TDC;
        public GameObject[] enemy = new GameObject[101];
        public Text closingRate;
        public GameObject LockIcon;
        public GameObject leftAngle;
        public GameObject rightAngle;
        public GameObject pitchIndicator;

        public GameObject RadarHeightUp;
        public GameObject RadarHeightDown;
        public GameObject RadarHeightTarget;

        public Text RadarHeightUpText;
        public Text RadarHeightDownText;
        public Text RadarHeightTargetText;

        Vector3 RadarForward;
        float RadarBasePitch;
        float RadarPitch;
        public void UpdateRadarHeight()
        {
            RadarForward = DataManager.Instance.RadarTransformForward[myPlayerID];
            RadarBasePitch = 90-Vector3.Angle(RadarForward, Vector3.up);
            RadarPitch = CC2RadarDisplayerData.Instance.pitch[myPlayerID]+ RadarBasePitch;

            float myHeight = transform.position.y/100;
            float dist = CC2RadarDisplayerData.Instance.ChooserPosition[myPlayerID].y + 60;

            float upperheight = Mathf.Clamp(myHeight + dist * Mathf.Sin(Mathf.Clamp((RadarPitch + 10),-90,90) * Mathf.PI / 180),0,60);
            float lowerheight = Mathf.Clamp(myHeight + dist * Mathf.Sin(Mathf.Clamp((RadarPitch - 10),-90,90) * Mathf.PI / 180),0,60);
            float targetheight = Mathf.Clamp(myHeight + dist * Mathf.Sin(RadarPitch * Mathf.PI / 180), 0, 60);

            //0~60 => 0~0.196
            RadarHeightDown.transform.localPosition = new Vector3(0, lowerheight * 0.003267f, 0);
            RadarHeightUp.transform.localPosition = new Vector3(0, upperheight * 0.003267f, 0);
            RadarHeightTarget.transform.localPosition = new Vector3(0, targetheight * 0.003267f, 0);
            RadarHeightUpText.text = Mathf.Round(upperheight).ToString();
            RadarHeightDownText.text = Mathf.Round(lowerheight).ToString();
            RadarHeightTargetText.text = Mathf.Round(targetheight).ToString();

            if (CC2RadarDisplayerData.Instance.locked[myPlayerID])
            {
                RadarHeightTarget.SetActive(true);
            }
            else
            {
                RadarHeightTarget.SetActive(false);
            }
        }

        public void Start()
        {
            radarScreen = Instantiate(AssetManager.Instance.RadarScreen.RadarScreen);
            radarScreen.transform.SetParent(transform);
            radarScreen.transform.localPosition = Vector3.zero;
            radarScreen.transform.localRotation = Quaternion.identity;
            radarScreen.transform.localScale = Vector3.one;
            radarScreen.name = "radarScreen";
            scanLine = radarScreen.transform.FindChild("scanLine").gameObject;
            TDC = radarScreen.transform.FindChild("TDC").gameObject;
            LockIcon = TDC.transform.FindChild("circle").gameObject;
            closingRate = TDC.transform.FindChild("closingRate").gameObject.GetComponent<Text>();
            pitchIndicator = radarScreen.transform.FindChild("pitchIndicator").gameObject;
            leftAngle = radarScreen.transform.FindChild("leftAngle").gameObject;
            rightAngle = radarScreen.transform.FindChild("rightAngle").gameObject;
            RadarHeightUp = radarScreen.transform.FindChild("Height").FindChild("up").gameObject;
            RadarHeightDown = radarScreen.transform.FindChild("Height").FindChild("down").gameObject;
            RadarHeightTarget = radarScreen.transform.FindChild("Height").FindChild("target").gameObject;
            RadarHeightUpText = RadarHeightUp.transform.FindChild("Text").GetComponent<Text>();
            RadarHeightDownText = RadarHeightDown.transform.FindChild("Text").GetComponent<Text>();
            RadarHeightTargetText = RadarHeightTarget.transform.FindChild("Text").GetComponent<Text>();
            GameObject enemies = radarScreen.transform.FindChild("enemies").gameObject;
            enemy[50] = enemies.transform.FindChild("enemy50").gameObject;
            enemy[50].SetActive(false);
            for (int i = 0; i < 101; i++)
            {
                if (i == 50)
                {
                    continue;
                }
                enemy[i] = (GameObject)Instantiate(enemy[50], enemies.transform);
                enemy[i].name = "enemy" + i.ToString();
                enemy[i].SetActive(false);
            }
        }
        public void LateUpdate()
        {
            UpdateRadarHeight();
            if (!isClient)
            {
                scanLine.transform.localPosition = new Vector3((CC2RadarDisplayerData.Instance.currRegion[myPlayerID] - 50) * 0.0021f, 0, 0f);
                TDC.transform.localPosition = new Vector3(CC2RadarDisplayerData.Instance.ChooserPosition[myPlayerID].x * 0.00175f,
                                                            CC2RadarDisplayerData.Instance.ChooserPosition[myPlayerID].y * 0.00175f, 0);
                bool locking = CC2RadarDisplayerData.Instance.locked[myPlayerID];
                LockIcon.SetActive(locking);
                if (locking)
                {
                    closingRate.text = Math.Floor(CC2RadarDisplayerData.Instance.ClosingRate[myPlayerID] + 0.5).ToString();
                }
                else
                {
                    closingRate.text = "";
                }
                pitchIndicator.transform.localPosition = new Vector3(0.1084f, CC2RadarDisplayerData.Instance.pitch[myPlayerID] / (350 + 150 / 2), 0);
                leftAngle.transform.localPosition = new Vector3(CC2RadarDisplayerData.Instance.leftAngle[myPlayerID] * 0.00175f, 0, 0);
                rightAngle.transform.localPosition = new Vector3(CC2RadarDisplayerData.Instance.rightAngle[myPlayerID] * 0.00175f, 0, 0);

                targetManager tmp = DataManager.Instance.TargetData[myPlayerID];

                float leftRegion = (CC2RadarDisplayerData.Instance.leftAngle[myPlayerID] + 60) / 1.2f;
                float rightRegion = (CC2RadarDisplayerData.Instance.rightAngle[myPlayerID] + 60) / 1.2f;
                for (int i = 0; i < 101; i++)
                {
                    if (tmp.targets[i].hasObject)
                    {
                        enemy[i].transform.localPosition = new Vector3(0.0021f * (i - 50), - 0.105f + tmp.targets[i].distance * 0.0000175f, 0f);
                        enemy[i].SetActive(true);
                    }
                    else
                    {
                        enemy[i].SetActive(false);
                    }
                }
            }
            else
            {
                scanLine.transform.localPosition = new Vector3((CC2RadarDisplayerData.Instance.currRegion[myPlayerID] - 50) * 0.0021f, 0, 0f);
                TDC.transform.localPosition = new Vector3(CC2RadarDisplayerData.Instance.ChooserPosition[myPlayerID].x * 0.00175f,
                                                            CC2RadarDisplayerData.Instance.ChooserPosition[myPlayerID].y * 0.00175f, 0);
                bool locking = CC2RadarDisplayerData.Instance.locked[myPlayerID];
                LockIcon.SetActive(locking);
                if (locking)
                {
                    closingRate.text = Math.Floor(CC2RadarDisplayerData.Instance.ClosingRate[myPlayerID] + 0.5).ToString();
                }
                else
                {
                    closingRate.text = "";
                }
                pitchIndicator.transform.localPosition = new Vector3(0.1084f, CC2RadarDisplayerData.Instance.pitch[myPlayerID] / (350 + 150 / 2), 0);
                leftAngle.transform.localPosition = new Vector3(CC2RadarDisplayerData.Instance.leftAngle[myPlayerID] * 0.00175f, 0, 0);
                rightAngle.transform.localPosition = new Vector3(CC2RadarDisplayerData.Instance.rightAngle[myPlayerID] * 0.00175f, 0, 0);

                for (int i = 0; i < 101; i++)
                {
                    if (DataManager.Instance.TargetData[myPlayerID].targets[i].hasObject)
                    {
                        enemy[i].transform.localPosition = new Vector3(0.0021f * (i - 50), - 0.105f + DataManager.Instance.TargetData[myPlayerID].targets[i].distance * 0.0000175f, 0f);
                        enemy[i].SetActive(true);
                    }
                    else
                    {
                        enemy[i].SetActive(false);
                    }
                }
            }
        }
        public void OnGUI()
        {
            //GUI.Box(new Rect(100, 200, 200, 50), RadarBasePitch.ToString());
            //GUI.Box(new Rect(100, 250, 200, 50), RadarPitch.ToString());
        }
    }

    public class A2GScreen_MFD : MonoBehaviour
    {
        public int myPlayerID;
        public bool isClient;
        public bool TVColor;

        public GameObject Screen;
        public MeshFilter ScreenMF;
        public MeshRenderer ScreenMR;
        public GameObject Cross;
        public MeshFilter CrossMF;
        public MeshRenderer CrossMR;
        public GameObject Aim;
        public MeshFilter AimMF;
        public MeshRenderer AimMR;
        public GameObject Compass;
        public MeshFilter CompassMF;
        public MeshRenderer CompassMR;
        public GameObject Direction;
        public MeshFilter DirectionMF;
        public MeshRenderer DirectionMR;
        public GameObject DistText;
        public TextMesh DistMesh;
        public GameObject VelocityText;
        public TextMesh VelocityMesh;
        public GameObject ThermalOnText;
        public TextMesh ThermalOnMesh;
        public GameObject InverseText;
        public TextMesh InverseMesh;

        public void initScreen()
        {
            if (!transform.FindChild("Screen"))
            {
                Screen = new GameObject("Screen");
                Screen.transform.SetParent(transform);
                Screen.transform.localPosition = new Vector3(0, 0, 0f);
                Screen.transform.localRotation = Quaternion.Euler(270, 0, 0);
                Screen.transform.localScale = new Vector3(0.12f, 0.12f, -0.12f);

                ScreenMF = Screen.AddComponent<MeshFilter>();
                ScreenMR = Screen.AddComponent<MeshRenderer>();
                ScreenMF.mesh = ModResource.GetMesh("Plane Mesh").Mesh;
                //ScreenMR.material.shader = Shader.Find("Particles/Alpha Blended");
                //ScreenMR.material.shader = AssetManager.Instance.Shader.GrayShader;
                ScreenMR.material.shader = AssetManager.Instance.Shader.GreenShader;
                ScreenMR.material.mainTexture = DataManager.Instance.output[myPlayerID];
                ScreenMR.sortingOrder = 50;

                Screen.SetActive(false);

                Cross = new GameObject("Cross");
                Cross.transform.SetParent(Screen.transform);
                Cross.transform.localPosition = new Vector3(0, 0.01f, 0);
                Cross.transform.localRotation = Quaternion.Euler(0, 0, 0);
                Cross.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

                CrossMF = Cross.AddComponent<MeshFilter>();
                CrossMR = Cross.AddComponent<MeshRenderer>();
                CrossMF.mesh = ModResource.GetMesh("Plane Mesh").Mesh;
                CrossMR.material = new Material(Shader.Find("Particles/Alpha Blended"));
                CrossMR.material.SetTexture("_MainTex", ModResource.GetTexture("A2GCross Texture"));
                CrossMR.material.SetColor("_TintColor", Color.green);

                Aim = new GameObject("Aim");
                Aim.transform.SetParent(Screen.transform);
                Aim.transform.localPosition = new Vector3(0, 0.01f, 0);
                Aim.transform.localRotation = Quaternion.Euler(0, 45, 0);
                Aim.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

                AimMF = Aim.AddComponent<MeshFilter>();
                AimMR = Aim.AddComponent<MeshRenderer>();
                AimMF.mesh = ModResource.GetMesh("Plane Mesh").Mesh;
                AimMR.material = new Material(Shader.Find("Particles/Alpha Blended"));
                AimMR.material.SetTexture("_MainTex", ModResource.GetTexture("A2GAim Texture"));
                AimMR.material.SetColor("_TintColor", Color.green);

                Aim.SetActive(false);

                Compass = new GameObject("Compass");
                Compass.transform.SetParent(Screen.transform);
                Compass.transform.localPosition = new Vector3(-0.55f, 0.01f, 0.55f);
                Compass.transform.localRotation = Quaternion.Euler(0, 0, 0);
                Compass.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);

                CompassMF = Compass.AddComponent<MeshFilter>();
                CompassMR = Compass.AddComponent<MeshRenderer>();
                CompassMF.mesh = ModResource.GetMesh("Plane Mesh").Mesh;
                CompassMR.material = new Material(Shader.Find("Particles/Alpha Blended"));
                CompassMR.material.SetTexture("_MainTex", ModResource.GetTexture("A2GCompass Texture"));
                CompassMR.material.SetColor("_TintColor", Color.green);

                Direction = new GameObject("Direction");
                Direction.transform.SetParent(Screen.transform);
                Direction.transform.localPosition = new Vector3(-0.55f, 0.01f, 0.55f);
                Direction.transform.localRotation = Quaternion.Euler(0, 0, 0);
                Direction.transform.localScale = new Vector3(0.33f, 0.33f, 0.33f);

                DirectionMF = Direction.AddComponent<MeshFilter>();
                DirectionMR = Direction.AddComponent<MeshRenderer>();
                DirectionMF.mesh = ModResource.GetMesh("Plane Mesh").Mesh;
                DirectionMR.material = new Material(Shader.Find("Particles/Alpha Blended"));
                DirectionMR.material.SetTexture("_MainTex", ModResource.GetTexture("A2GDirection Texture"));
                DirectionMR.material.SetColor("_TintColor", Color.green);

                //information Text

                ThermalOnText = new GameObject("ThermalOnText");
                ThermalOnText.transform.SetParent(Screen.transform);
                ThermalOnText.transform.localPosition = new Vector3(0.7f, 0.02f, 0.8f);
                ThermalOnText.transform.localRotation = Quaternion.Euler(270, 0, 0);
                ThermalOnText.transform.localScale = new Vector3(0.022f, 0.022f, 0.1f);
                ThermalOnMesh = ThermalOnText.AddComponent<TextMesh>();
                ThermalOnMesh.text = "□ Thermal";
                ThermalOnMesh.color = Color.green;
                ThermalOnMesh.characterSize = 0.7f;
                ThermalOnMesh.fontSize = 64;
                ThermalOnMesh.fontStyle = FontStyle.Normal;
                ThermalOnMesh.anchor = TextAnchor.MiddleCenter;

                InverseText = new GameObject("InverseText");
                InverseText.transform.SetParent(Screen.transform);
                InverseText.transform.localPosition = new Vector3(0.7f, 0.02f, 0.9f);
                InverseText.transform.localRotation = Quaternion.Euler(270, 0, 0);
                InverseText.transform.localScale = new Vector3(0.022f, 0.022f, 0.1f);
                InverseMesh = InverseText.AddComponent<TextMesh>();
                InverseMesh.text = " ╰□ Inverse";
                InverseMesh.color = Color.green;
                InverseMesh.characterSize = 0.7f;
                InverseMesh.fontSize = 64;
                InverseMesh.fontStyle = FontStyle.Normal;
                InverseMesh.anchor = TextAnchor.MiddleCenter;
                InverseText.SetActive(false);

                VelocityText = new GameObject("VelocityText");
                VelocityText.transform.SetParent(Screen.transform);
                VelocityText.transform.localPosition = new Vector3(0.3f, 0.02f, 0);
                VelocityText.transform.localRotation = Quaternion.Euler(270, 0, 0);
                VelocityText.transform.localScale = new Vector3(0.022f, 0.022f, 0.1f);
                VelocityMesh = VelocityText.AddComponent<TextMesh>();
                VelocityMesh.text = "0";
                VelocityMesh.color = Color.green;
                VelocityMesh.characterSize = 0.7f;
                VelocityMesh.fontSize = 64;
                VelocityMesh.fontStyle = FontStyle.Normal;
                VelocityMesh.anchor = TextAnchor.MiddleLeft;
                VelocityText.SetActive(false);

                DistText = new GameObject("DistText");
                DistText.transform.SetParent(Screen.transform);
                DistText.transform.localPosition = new Vector3(-0.3f, 0.02f, 0);
                DistText.transform.localRotation = Quaternion.Euler(270, 0, 0);
                DistText.transform.localScale = new Vector3(0.022f, 0.022f, 0.1f);
                DistMesh = DistText.AddComponent<TextMesh>();
                DistMesh.text = "0";
                DistMesh.color = Color.green;
                DistMesh.characterSize = 0.7f;
                DistMesh.fontSize = 64;
                DistMesh.fontStyle = FontStyle.Normal;
                DistMesh.anchor = TextAnchor.MiddleRight;
                DistText.SetActive(false);

            }

        }

        public void ScreenOn()
        {
            DataManager.Instance.TV_FOV[myPlayerID] = 40;
            Screen.SetActive(true);
        }

        public void Start()
        {
            initScreen();
            ScreenOn();
            if (!TVColor)
            {
                ScreenMR.material.shader = AssetManager.Instance.Shader.GreenShader;
                CrossMR.material.SetColor("_TintColor", Color.green);
                AimMR.material.SetColor("_TintColor", Color.green);
                CompassMR.material.SetColor("_TintColor", Color.green);
                DirectionMR.material.SetColor("_TintColor", Color.green);
                ThermalOnMesh.color = Color.green;
                InverseMesh.color = Color.green;
                VelocityMesh.color = Color.green;
                DistMesh.color = Color.green;
            }
            else
            {
                ScreenMR.material.shader = AssetManager.Instance.Shader.GrayShader;
                CrossMR.material.SetColor("_TintColor", Color.white);
                AimMR.material.SetColor("_TintColor", Color.white);
                CompassMR.material.SetColor("_TintColor", Color.white);
                DirectionMR.material.SetColor("_TintColor", Color.white);
                ThermalOnMesh.color = Color.white;
                InverseMesh.color = Color.white;
                VelocityMesh.color = Color.white;
                DistMesh.color = Color.white;
            }
        }
        protected void Update()
        {
            Aim.SetActive(DataManager.Instance.TV_Lock[myPlayerID]);
            if (DataManager.Instance.TV_Lock[myPlayerID])
            {
                DistText.SetActive(true);
                DistMesh.text = Math.Round(DataManager.Instance.EO_Distance[myPlayerID] / 1000, 1).ToString() + " km";
            }
            else
            {
                DistText.SetActive(false);
            }
            if (DataManager.Instance.TV_Track[myPlayerID])
            {
                Aim.transform.localRotation = Quaternion.Lerp(Aim.transform.localRotation, Quaternion.Euler(0, 0, 0), 0.1f);
                VelocityText.SetActive(true);
                VelocityMesh.text = Math.Round(DataManager.Instance.A2G_TargetData[myPlayerID].velocity.magnitude * 3.6f, 1).ToString() + " kph";
            }
            else
            {
                Aim.transform.localRotation = Quaternion.Lerp(Aim.transform.localRotation, Quaternion.Euler(0, 45, 0), 0.1f);
                VelocityMesh.text = "0";
                VelocityText.SetActive(false);
            }
            //update direction arrow
            Direction.transform.localRotation = Quaternion.Euler(0, -DataManager.Instance.A2G_Orientation[myPlayerID], 0);
            Direction.transform.localScale = new Vector3(0.33f, 0.33f, 0.33f * Mathf.Clamp((DataManager.Instance.A2G_Pitch[myPlayerID] / 90f), 0.1f, 1f));
            if (DataManager.Instance.EO_ThermalOn[myPlayerID])
            {
                ThermalOnMesh.text = "■ Thermal";

                InverseText.SetActive(true);
                if (DataManager.Instance.EO_InverseThermal[myPlayerID])
                {
                    InverseMesh.text = " ╰■ Inverse";
                }
                else
                {
                    InverseMesh.text = " ╰□ Inverse";
                }
            }
            else
            {
                ThermalOnMesh.text = "□ Thermal";
                InverseText.SetActive(false);
            }

        }


    }

    public class LoadScreen_MFD : MonoBehaviour
    {
        public int myPlayerID;
        public bool initialized = false;

        public bool noMachineGun = true;
        public bool noFlare = true;
        public bool noChaff = true;

        public int BulletsLeft;
        public int FlareLeft;
        public int ChaffLeft;

        public GameObject WeaponIcons;
        public GameObject MachineGunText;
        public TextMesh MachineGunMesh;
        public GameObject FlareText;
        public TextMesh FlareMesh;
        public GameObject ChaffText;
        public TextMesh ChaffMesh;

        public List<LoadDataManager.WeaponLoad> leftWingLoad = new List<LoadDataManager.WeaponLoad>();
        public List<LoadDataManager.WeaponLoad> rightWingLoad = new List<LoadDataManager.WeaponLoad>();

        public List<GameObject> leftWingLoadIcons = new List<GameObject>();
        public List<GameObject> rightWingLoadIcons = new List<GameObject>();

        public Vector3 leftRoot = new Vector3(-0.03f, -0.32f, 0f);
        public Vector3 leftTip = new Vector3(-1f, 0f, 0f);
        public Vector3 rightRoot = new Vector3(0.03f, -0.32f, 0f);
        public Vector3 rightTip = new Vector3(1f, 0f, 0f);

        public bool SyncCCData()
        {
            if (CC2LoadDisplayerData.Instance.LoadListReady[myPlayerID])
            {
                CC2LoadDisplayerData.Instance.LoadListReady[myPlayerID] = false;
                leftWingLoad = CC2LoadDisplayerData.Instance.leftWingLoad[myPlayerID];
                rightWingLoad = CC2LoadDisplayerData.Instance.rightWingLoad[myPlayerID];
                noMachineGun = CC2LoadDisplayerData.Instance.noMachineGun[myPlayerID];
                noFlare = CC2LoadDisplayerData.Instance.noFlare[myPlayerID];
                noChaff = CC2LoadDisplayerData.Instance.noChaff[myPlayerID];
                BulletsLeft = CC2LoadDisplayerData.Instance.BulletsLeft[myPlayerID];
                FlareLeft = CC2LoadDisplayerData.Instance.FlareLeft[myPlayerID]; 
                ChaffLeft = CC2LoadDisplayerData.Instance.ChaffLeft[myPlayerID];
                return true;
            }
            else
            {
                return false;
            }
        }
        public void InitLoadIcon() //call in simlulate update after InitLoad()
        {
            for (int i = 0; i < leftWingLoad.Count; i++)
            {
                leftWingLoadIcons.Add(new GameObject("WeaponIcon"));
                leftWingLoadIcons[i].transform.SetParent(WeaponIcons.transform);
                Vector3 IconPosition = new Vector3();
                IconPosition = leftRoot + (leftTip - leftRoot) * (i + 1) / (leftWingLoad.Count + 1);
                leftWingLoadIcons[i].transform.localPosition = IconPosition;
                leftWingLoadIcons[i].transform.localRotation = Quaternion.Euler(270, 0, 0);
                leftWingLoadIcons[i].transform.localScale = new Vector3(0.105f, 0.105f, 0.105f);
                MeshFilter leftWingLoadIconsMeshFilter = leftWingLoadIcons[i].AddComponent<MeshFilter>();
                leftWingLoadIconsMeshFilter.mesh = ModResource.GetMesh("Plane Mesh").Mesh;
                MeshRenderer leftWingLoadIconsRenderer = leftWingLoadIcons[i].AddComponent<MeshRenderer>();
                leftWingLoadIconsRenderer.material = new Material(Shader.Find("Particles/Alpha Blended"));
                switch (leftWingLoad[i].weapon)
                {
                    case LoadDataManager.WeaponType.SRAAM:
                        leftWingLoadIconsRenderer.material.SetTexture("_MainTex", ModResource.GetTexture("SRAAMLoad Texture"));
                        break;
                    case LoadDataManager.WeaponType.MRAAM:
                        leftWingLoadIconsRenderer.material.SetTexture("_MainTex", ModResource.GetTexture("MRAAMLoad Texture"));
                        break;
                    case LoadDataManager.WeaponType.AGM:
                        leftWingLoadIconsRenderer.material.SetTexture("_MainTex", ModResource.GetTexture("AGMLoad Texture"));
                        break;
                    case LoadDataManager.WeaponType.GBU:
                        leftWingLoadIconsRenderer.material.SetTexture("_MainTex", ModResource.GetTexture("GBULoad Texture"));
                        break;
                    default:
                        break;
                }
                leftWingLoadIconsRenderer.material.SetColor("_TintColor", Color.green);
                leftWingLoadIcons[i].SetActive(true);
            }
            for (int i = 0; i < rightWingLoad.Count; i++)
            {
                rightWingLoadIcons.Add(new GameObject("WeaponIcon"));
                rightWingLoadIcons[i].transform.SetParent(WeaponIcons.transform);
                Vector3 IconPosition = new Vector3();
                IconPosition = rightRoot + (rightTip - rightRoot) * (i + 1) / (rightWingLoad.Count + 1);
                rightWingLoadIcons[i].transform.localPosition = IconPosition;
                rightWingLoadIcons[i].transform.localRotation = Quaternion.Euler(270, 0, 0);
                rightWingLoadIcons[i].transform.localScale = new Vector3(0.105f, 0.105f, 0.105f);
                MeshFilter rightWingLoadIconsMeshFilter = rightWingLoadIcons[i].AddComponent<MeshFilter>();
                rightWingLoadIconsMeshFilter.mesh = ModResource.GetMesh("Plane Mesh").Mesh;
                MeshRenderer rightWingLoadIconsRenderer = rightWingLoadIcons[i].AddComponent<MeshRenderer>();
                rightWingLoadIconsRenderer.material = new Material(Shader.Find("Particles/Alpha Blended"));
                switch (rightWingLoad[i].weapon)
                {
                    case LoadDataManager.WeaponType.SRAAM:
                        rightWingLoadIconsRenderer.material.SetTexture("_MainTex", ModResource.GetTexture("SRAAMLoad Texture"));
                        break;
                    case LoadDataManager.WeaponType.MRAAM:
                        rightWingLoadIconsRenderer.material.SetTexture("_MainTex", ModResource.GetTexture("MRAAMLoad Texture"));
                        break;
                    case LoadDataManager.WeaponType.AGM:
                        rightWingLoadIconsRenderer.material.SetTexture("_MainTex", ModResource.GetTexture("AGMLoad Texture"));
                        break;
                    case LoadDataManager.WeaponType.GBU:
                        rightWingLoadIconsRenderer.material.SetTexture("_MainTex", ModResource.GetTexture("GBULoad Texture"));
                        break;
                    default:
                        break;
                }
                rightWingLoadIconsRenderer.material.SetColor("_TintColor", Color.green);
                rightWingLoadIcons[i].SetActive(true);
            }
        }
        public void InitPanel()
        {
            if (!transform.FindChild("Wing"))
            {
                GameObject Wing = new GameObject("Wing");
                Wing.transform.SetParent(transform);
                Wing.transform.localPosition = new Vector3(0f, 0f, 0f);
                Wing.transform.localRotation = Quaternion.Euler(270, 0, 0);
                Wing.transform.localScale = new Vector3(0.105f, 0.105f, 0.105f);
                MeshFilter WingMeshFilter = Wing.AddComponent<MeshFilter>();
                WingMeshFilter.mesh = ModResource.GetMesh("Plane Mesh").Mesh;
                MeshRenderer WingRenderer = Wing.AddComponent<MeshRenderer>();
                WingRenderer.material = new Material(Shader.Find("Particles/Alpha Blended"));
                WingRenderer.material.SetTexture("_MainTex", ModResource.GetTexture("LoadWing Texture"));
                WingRenderer.material.SetColor("_TintColor", Color.green);
                Wing.SetActive(true);
            }
            if (!transform.FindChild("WeaponIcons"))
            {
                WeaponIcons = new GameObject("WeaponIcons");
                WeaponIcons.transform.SetParent(transform);
                WeaponIcons.transform.localPosition = new Vector3(0f, 0.01f, 0f);
                WeaponIcons.transform.localRotation = Quaternion.Euler(0, 0, 0);
                WeaponIcons.transform.localScale = new Vector3(0.11f, 0.11f, 0.11f);
            }
            if (!transform.FindChild("MachineGunText"))
            {
                MachineGunText = new GameObject("MachineGunText");
                MachineGunText.transform.SetParent(transform);
                MachineGunText.transform.localPosition = new Vector3(-0.06f, 0.05f, 0f);
                MachineGunText.transform.localRotation = Quaternion.Euler(180, 0, 0);
                MachineGunText.transform.localScale = new Vector3(0.003f, 0.003f, 0.003f);
                MachineGunMesh = MachineGunText.AddComponent<TextMesh>();
                MachineGunMesh.text = "Bullets: ";
                MachineGunMesh.color = Color.green;
                MachineGunMesh.characterSize = 0.7f;
                MachineGunMesh.fontSize = 64;
                MachineGunMesh.fontStyle = FontStyle.Normal;
                MachineGunMesh.anchor = TextAnchor.MiddleLeft;
                MachineGunText.SetActive(false);
            }
            if (!transform.FindChild("FlareText"))
            {
                FlareText = new GameObject("FlareText");
                FlareText.transform.SetParent(transform);
                FlareText.transform.localPosition = new Vector3(-0.06f, 0.07f, 0f);
                FlareText.transform.localRotation = Quaternion.Euler(180, 0, 0);
                FlareText.transform.localScale = new Vector3(0.003f, 0.003f, 0.003f);
                FlareMesh = FlareText.AddComponent<TextMesh>();
                FlareMesh.text = "Flares: ";
                FlareMesh.color = Color.green;
                FlareMesh.characterSize = 0.7f;
                FlareMesh.fontSize = 64;
                FlareMesh.fontStyle = FontStyle.Normal;
                FlareMesh.anchor = TextAnchor.MiddleLeft;
                FlareText.SetActive(false);
            }
            if (!transform.FindChild("ChaffText"))
            {
                ChaffText = new GameObject("ChaffText");
                ChaffText.transform.SetParent(transform);
                ChaffText.transform.localPosition = new Vector3(-0.06f, 0.09f, 0f);
                ChaffText.transform.localRotation = Quaternion.Euler(180, 0, 0);
                ChaffText.transform.localScale = new Vector3(0.003f, 0.003f, 0.003f);
                ChaffMesh = ChaffText.AddComponent<TextMesh>();
                ChaffMesh.text = "Chaff: ";
                ChaffMesh.color = Color.green;
                ChaffMesh.characterSize = 0.7f;
                ChaffMesh.fontSize = 64;
                ChaffMesh.fontStyle = FontStyle.Normal;
                ChaffMesh.anchor = TextAnchor.MiddleLeft;
                ChaffText.SetActive(false);
            }
        }
        public void updateLoadAndIcon()
        {
            for (int i = 0; i < leftWingLoad.Count; i++)
            {
                if (leftWingLoad[i].released)
                {
                    leftWingLoadIcons[i].SetActive(false);
                }

            }
            for (int i = 0; i < rightWingLoad.Count; i++)
            {
                if (rightWingLoad[i].released)
                {
                    rightWingLoadIcons[i].SetActive(false);
                }

            }
        }
        public void updateText()
        {
            if (!noMachineGun)
            {
                MachineGunMesh.text = "Bullets:     " + BulletsLeft.ToString();
            }
            else
            {
                MachineGunMesh.text = "Bullets:     x";
            }
            if (!noFlare)
            {
                FlareMesh.text = "Flares:      " + FlareLeft.ToString();
            }
            else
            {
                FlareMesh.text = "Flares:      x";
            }
            if (!noChaff)
            {
                ChaffMesh.text = "Chaff:       " + ChaffLeft.ToString();
            }
            else
            {
                ChaffMesh.text = "Chaff:       x";
            }
        }
        
        public void Start()
        {
            InitPanel();
            MachineGunText.SetActive(true);
            FlareText.SetActive(true);
            ChaffText.SetActive(true);
        }
        public void LateUpdate()
        {
            if (SyncCCData())
            {
                if (!initialized)
                {
                    initialized = true;
                    InitLoadIcon();
                }
                updateLoadAndIcon();
                updateText();
            }
        }
    }

    public class NavScreen_MFD : MonoBehaviour
    {
        public int myPlayerID;
        GameObject NavScreen;
        NAVController controller;
        public void Start()
        {
            NavScreen = Instantiate(AssetManager.Instance.NavScreen.NavScreen);
            NavScreen.transform.SetParent(transform);
            NavScreen.transform.localPosition = Vector3.zero;
            NavScreen.transform.localRotation = Quaternion.identity;
            NavScreen.transform.localScale = Vector3.one;
            NavScreen.name = "NavScreen";
            NavScreen.AddComponent<NAVController>().myPlayerID = myPlayerID;
        }
    }

    class MFD : BlockScript
    {
        class ScreenType
        {
            public enum ScreenTypes { None, Radar, A2G, Load, Navigation };
            public ScreenTypes Type;
            int typeNum = 5;
            public ScreenType()
            {
                Type = ScreenTypes.None;
            }
            public ScreenType(ScreenTypes type)
            {
                Type = type;
            }
            public void Next()
            {
                if ((int)Type == typeNum - 1)
                {
                    Type = 0;
                }
                else
                {
                    Type++;
                }
            }

        }

        public int myPlayerID;
        public int myGuid;
        public bool isClient;

        public MKey Switch;
        public MMenu DefaultScreen;
        public MMenu TVColor;

        public MToggle DisableNone;
        public MToggle DisableRadar;
        public MToggle DisableA2G;
        public MToggle DisableLoad;
        public MToggle DisableNavigation;

        ScreenType screenType;

        public GameObject RadarDisplayer;
        public GameObject A2GDisplayer;
        public GameObject LoadDisplayer;
        public GameObject NavDisplayer;

        public static MessageType clientMFDType = ModNetworking.CreateMessageType(DataType.Integer, DataType.Integer, DataType.Integer);

        
        public override void SafeAwake()
        {
            myPlayerID = BlockBehaviour.ParentMachine.PlayerID;
            isClient = StatMaster.isClient;

            Switch = AddKey("Switch Screen", "SwitchScreen", KeyCode.LeftShift);
            DefaultScreen = AddMenu("DefaultScreen",0, new List<String>
            {
                "None",
                "Radar",
                "A2G",
                "Load",
                "Navigation"
            }, false);
            TVColor = AddMenu("TV Color Type", 0, new List<string>
            {
                "Green TV",
                "Gray TV"
            }, false);
            DisableNone = AddToggle("Disable \"None Screen\"", "DisableNone", false);
            DisableRadar = AddToggle("Disable \"Radar Screen\"", "DisableRadar", false);
            DisableA2G = AddToggle("Disable \"A2G Screen\"", "DisableA2G", false);
            DisableLoad = AddToggle("Disable \"Load Screen\"", "DisableLoad", false);
            DisableNavigation = AddToggle("Disable \"Navigation Screen\"", "DisableNavigation", false);

            gameObject.name = "MFD";
            if (!transform.Find("RadarDisplayer"))
            {
                RadarDisplayer = new GameObject("RadarDisplayer");
                RadarDisplayer.transform.SetParent(transform);
                RadarDisplayer.transform.localPosition = new Vector3(0,0,0.096f);
                RadarDisplayer.transform.localRotation = Quaternion.Euler(0, 180, 180);
                RadarDisplayer.transform.localScale = Vector3.one;
                RadarDisplayer.SetActive(false);
            }
            if (!transform.Find("A2GDisplayer"))
            {
                A2GDisplayer = new GameObject("A2GDisplayer");
                A2GDisplayer.transform.SetParent(transform);
                A2GDisplayer.transform.localPosition = new Vector3(0, 0, 0.096f);
                A2GDisplayer.transform.localRotation = Quaternion.Euler(0, 180, 180);
                A2GDisplayer.transform.localScale = Vector3.one;
                A2GDisplayer.SetActive(false);
            }
            if (!transform.Find("LoadDisplayer"))
            {
                LoadDisplayer = new GameObject("LoadDisplayer");
                LoadDisplayer.transform.SetParent(transform);
                LoadDisplayer.transform.localPosition = new Vector3(0, 0, 0.096f);
                LoadDisplayer.transform.localRotation = Quaternion.Euler(0, 0, 0);
                LoadDisplayer.transform.localScale = Vector3.one;
                LoadDisplayer.SetActive(false);
            }
            if (!transform.Find("NavDisplayer"))
            {
                NavDisplayer = new GameObject("NavDisplayer");
                NavDisplayer.transform.SetParent(transform);
                NavDisplayer.transform.localPosition = new Vector3(0, 0, 0.096f);
                NavDisplayer.transform.localRotation = Quaternion.Euler(0, 180, 180);
                NavDisplayer.transform.localScale = Vector3.one*1.25f;
                NavDisplayer.SetActive(false);
            }

        }
        public void Start()
        {
            
        }
        public void Update()
        {
            
        }
        public override void OnSimulateStart()
        {
            myGuid = GetComponent<BlockBehaviour>().BuildingBlock.Guid.GetHashCode();

            screenType = new ScreenType((ScreenType.ScreenTypes)DefaultScreen.Value);

            RadarDisplayer.AddComponent<RadarScreen_MFD>().myPlayerID = myPlayerID;
            RadarDisplayer.GetComponent<RadarScreen_MFD>().isClient = isClient;

            A2GDisplayer.AddComponent<A2GScreen_MFD>().myPlayerID = myPlayerID;
            A2GDisplayer.GetComponent<A2GScreen_MFD>().isClient = isClient;
            if (TVColor.Selection == "Green TV")
            {
                A2GDisplayer.GetComponent<A2GScreen_MFD>().TVColor = false;
            }
            else
            {
                A2GDisplayer.GetComponent<A2GScreen_MFD>().TVColor = true;
            }
            ModNetworking.SendToAll(clientMFDType.CreateMessage(myPlayerID,myGuid,(int)screenType.Type));

            LoadDisplayer.AddComponent<LoadScreen_MFD>().myPlayerID = myPlayerID;
            NavDisplayer.AddComponent<NavScreen_MFD>().myPlayerID = myPlayerID;

        }
        public override void SimulateUpdateHost()
        {
            if (!DisableNone.isDefaultValue&&!DisableRadar.isDefaultValue&&!DisableA2G.isDefaultValue&&!DisableLoad.isDefaultValue && !DisableNavigation.isDefaultValue)
            {
                RadarDisplayer.SetActive(false);
                A2GDisplayer.SetActive(false);
                LoadDisplayer.SetActive(false);
                NavDisplayer.SetActive(false);
                return;
            }
            if (Switch.IsPressed)
            {
                while (true)
                {
                    screenType.Next();
                    if (screenType.Type == ScreenType.ScreenTypes.None && DisableNone.isDefaultValue)
                    {
                        break;
                    }
                    if (screenType.Type == ScreenType.ScreenTypes.Radar && DisableRadar.isDefaultValue)
                    {
                        break;
                    }
                    if (screenType.Type == ScreenType.ScreenTypes.A2G && DisableA2G.isDefaultValue)
                    {
                        break;
                    }
                    if (screenType.Type == ScreenType.ScreenTypes.Load && DisableLoad.isDefaultValue)
                    {
                        break;
                    }
                    if (screenType.Type == ScreenType.ScreenTypes.Navigation && DisableNavigation.isDefaultValue)
                    {
                        break;
                    }
                }
                
                ModNetworking.SendToAll(clientMFDType.CreateMessage(myPlayerID,myGuid,(int)screenType.Type));
            }

            if (screenType.Type == ScreenType.ScreenTypes.Radar)
            {
                RadarDisplayer.SetActive(true);
            }
            else
            {
                RadarDisplayer.SetActive(false);
            }
            if (screenType.Type == ScreenType.ScreenTypes.A2G)
            {
                A2GDisplayer.SetActive(true);
            }
            else
            {
                A2GDisplayer.SetActive(false);
            }
            if (screenType.Type == ScreenType.ScreenTypes.Load)
            {
                LoadDisplayer.SetActive(true);
            }
            else
            {
                LoadDisplayer.SetActive(false);
            }
            if (screenType.Type == ScreenType.ScreenTypes.Navigation)
            {
                NavDisplayer.SetActive(true);
            }
            else
            {
                NavDisplayer.SetActive(false);
            }
        }
        public override void SimulateUpdateClient()
        {
            if (!DisableNone.isDefaultValue && !DisableRadar.isDefaultValue && !DisableA2G.isDefaultValue && !DisableLoad.isDefaultValue && !DisableNavigation.isDefaultValue)
            {
                RadarDisplayer.SetActive(false);
                A2GDisplayer.SetActive(false);
                LoadDisplayer.SetActive(false);
                NavDisplayer.SetActive(false);
                return;
            }
            if (MFDMsgReceiver.Instance.ScreenType[myPlayerID][myGuid] == 1)
            {
                RadarDisplayer.SetActive(true);
            }
            else
            {
                RadarDisplayer.SetActive(false);
            }
            if (MFDMsgReceiver.Instance.ScreenType[myPlayerID][myGuid] == 2)
            {
                A2GDisplayer.SetActive(true);
            }
            else
            {
                A2GDisplayer.SetActive(false);
            }
            if (MFDMsgReceiver.Instance.ScreenType[myPlayerID][myGuid] == 3)
            {
                LoadDisplayer.SetActive(true);
            }
            else
            {
                LoadDisplayer.SetActive(false);
            }
            if (MFDMsgReceiver.Instance.ScreenType[myPlayerID][myGuid] == 4)
            {
                NavDisplayer.SetActive(true);
            }
            else
            {
                NavDisplayer.SetActive(false);
            }
        }


    }
}
