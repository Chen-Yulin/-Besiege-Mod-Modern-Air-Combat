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
        public MKey Reset;

        public GameObject Screen;
        public MeshFilter ScreenMF;
        public MeshRenderer ScreenMR;

        private int myPlayerID;
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
                ScreenMR.material.shader = AssetManager.Instance.Shader.GrayShader;
                ScreenMR.material.mainTexture = DataManager.Instance.output[myPlayerID];
                ScreenMR.sortingOrder = 50;

                Screen.SetActive(false);
            }

        }

        public void ScreenOn()
        {
            DataManager.Instance.TV_FOV[myPlayerID] = 40;
            Screen.SetActive(true);
        }

        public override void SafeAwake()
        {
            myPlayerID = BlockBehaviour.ParentMachine.PlayerID;

            Lock = AddKey("Lock", "Lock", KeyCode.X);

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

        }
        public override void OnSimulateStart()
        {
            ScreenOn();
        }

        protected void Update()
        {



        }

        public override void SimulateUpdateHost()
        {
            if (Lock.IsPressed)
            {
                DataManager.Instance.TV_Lock[myPlayerID] = !DataManager.Instance.TV_Lock[myPlayerID];
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

    }
}
