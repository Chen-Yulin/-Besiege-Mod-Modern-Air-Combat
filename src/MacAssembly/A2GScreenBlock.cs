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

    class A2GScreenBlock:BlockScript
    {
        public MKey Lock;
        public MKey ZoomIn;
        public MKey ZoomOut;
        public MKey PitchUp;
        public MKey PitchDown;
        public MKey YawLeft;
        public MKey YawRight;
        public MKey Track;
        public MMenu TVColor;

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
        public Texture LockIcon;

        public float iconSize = 16;
        private int myPlayerID;

        public static MessageType ClientTrackMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Boolean);

        public void initScreen()
        {
            if (!transform.FindChild("Screen"))
            {
                Screen = new GameObject("Screen");
                Screen.transform.SetParent(transform);
                Screen.transform.localPosition = new Vector3(0, 0, 0.091f);
                Screen.transform.localRotation = Quaternion.Euler(90,0,0);
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

        public override void SafeAwake()
        {
            LockIcon = ModResource.GetTexture("HUDA2GAim Texture").Texture;
            TVColor = AddMenu("Missile Type", 0, new List<string>
            {
                "Green",
                "Gray"
            }, false);
            myPlayerID = BlockBehaviour.ParentMachine.PlayerID;

            Lock = AddKey("Lock", "Lock", KeyCode.X);
            Track = AddKey("Track", "Track", KeyCode.Z);

            ZoomIn = AddKey("ZoomIn", "ZoomIn", KeyCode.N);
            ZoomOut = AddKey("ZoomOut", "ZoomOut", KeyCode.M);

            PitchUp = AddKey("PitchUp", "PitchUp", KeyCode.Y);
            PitchDown = AddKey("PitchDown", "PitchDown", KeyCode.H);

            YawLeft = AddKey("YawLeft","YawLeft", KeyCode.G);
            YawRight = AddKey("YawRight", "YawRight", KeyCode.J);

            initScreen();
        }

        public void Start()
        {
            if (TVColor.Selection == "Green")
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
        public override void OnSimulateStart()
        {
            ScreenOn();
        }

        public override void OnSimulateStop()
        {
            DataManager.Instance.TV_Lock[myPlayerID] = false;
            DataManager.Instance.TV_Track[myPlayerID] = false;

            ThermalOnMesh.text = "□ Thermal";
            InverseMesh.text = " ╰□ Inverse";

            InverseText.SetActive(false);
            VelocityMesh.text = "0";
            VelocityText.SetActive(false);
            DistText.SetActive(false);
        }

        protected void Update()
        {
            Aim.SetActive(DataManager.Instance.TV_Lock[myPlayerID]);
            if (DataManager.Instance.TV_Lock[myPlayerID])
            {
                DistText.SetActive(true);
                DistMesh.text = Math.Round(DataManager.Instance.EO_Distance[myPlayerID] / 1000,1).ToString()+" km";
            }
            else
            {
                DistText.SetActive(false);
            }
            if (DataManager.Instance.TV_Track[myPlayerID])
            {
                Aim.transform.localRotation = Quaternion.Lerp(Aim.transform.localRotation, Quaternion.Euler(0, 0, 0), 0.1f);
                VelocityText.SetActive(true);
                VelocityMesh.text = Math.Round(DataManager.Instance.A2G_TargetData[myPlayerID].velocity.magnitude * 3.6f, 1).ToString()+" kph";
            }
            else
            {
                Aim.transform.localRotation = Quaternion.Lerp(Aim.transform.localRotation, Quaternion.Euler(0, 45, 0), 0.1f);
                VelocityMesh.text = "0";
                VelocityText.SetActive(false);
            }
            //update direction arrow
            Direction.transform.localRotation = Quaternion.Euler(0, -DataManager.Instance.A2G_Orientation[myPlayerID], 0);
            Direction.transform.localScale = new Vector3(0.33f,0.33f,0.33f * Mathf.Clamp((DataManager.Instance.A2G_Pitch[myPlayerID]/90f),0.1f,1f));
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

        public override void SimulateUpdateClient()
        {
            DataManager.Instance.TV_Track[myPlayerID] = EOMsgReceiver.Instance.Track[myPlayerID];
        }

        public override void SimulateUpdateHost()
        {
            if (Lock.IsPressed)
            {
                DataManager.Instance.TV_Lock[myPlayerID] = !DataManager.Instance.TV_Lock[myPlayerID];
            }
            if (Track.IsPressed && DataManager.Instance.TV_Lock[myPlayerID])
            {
                DataManager.Instance.TV_Track[myPlayerID] = !DataManager.Instance.TV_Track[myPlayerID];
                ModNetworking.SendToAll(ClientTrackMsg.CreateMessage(myPlayerID, DataManager.Instance.TV_Track[myPlayerID]));
            }
            if (!DataManager.Instance.TV_Lock[myPlayerID])
            {
                DataManager.Instance.TV_Track[myPlayerID] = false;
                ModNetworking.SendToAll(ClientTrackMsg.CreateMessage(myPlayerID, DataManager.Instance.TV_Track[myPlayerID]));
            }
        }

        public override void SimulateFixedUpdateHost()
        {
            try
            {
                if (YawLeft.IsHeld)
                {
                    DataManager.Instance.TV_LeftRight[myPlayerID] = -1;
                }
                else if (YawRight.IsHeld)
                {
                    DataManager.Instance.TV_LeftRight[myPlayerID] = 1;
                }
                else
                {
                    DataManager.Instance.TV_LeftRight[myPlayerID] = 0;
                }

                if (PitchUp.IsHeld)
                {
                    DataManager.Instance.TV_UpDown[myPlayerID] = 1;
                }
                else if (PitchDown.IsHeld)
                {
                    DataManager.Instance.TV_UpDown[myPlayerID] = -1;
                }
                else
                {
                    DataManager.Instance.TV_UpDown[myPlayerID] = 0;
                }

                if (ZoomIn.IsHeld)
                {
                    DataManager.Instance.TV_FOV[myPlayerID] *= 0.98f;
                }
                else if (ZoomOut.IsHeld)
                {
                    DataManager.Instance.TV_FOV[myPlayerID] /= 0.98f;
                }
                DataManager.Instance.TV_FOV[myPlayerID] = Mathf.Clamp(DataManager.Instance.TV_FOV[myPlayerID], 0.5f, 40);
            }
            catch { }
            
        }

        void OnGUI()
        {
            //GUI.Box(new Rect(100, 200, 200, 50), DataManager.Instance.EO_ThermalOn[myPlayerID].ToString());
            //GUI.Box(new Rect(100, 300, 200, 50), DataManager.Instance.EO_InverseThermal[myPlayerID].ToString());
            //GUI.Box(new Rect(100, 400, 200, 50), FOV.ToString());
            if (DataManager.Instance.TV_Lock[myPlayerID] && IsSimulating)
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
}
