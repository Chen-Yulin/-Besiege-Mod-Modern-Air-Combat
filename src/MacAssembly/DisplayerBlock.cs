using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections;
using System.Numerics;

using Modding.Modules;
using Modding;
using Modding.Blocks;
using UnityEngine;

namespace ModernAirCombat
{
    public class ScanLineController : MonoBehaviour
    {
        public float frequency = 1;
        public float angleLeft = -60;
        public float angleRight = 60;
        public float currAngle;
        public bool direction = false;

        private void Start()
        {
            currAngle = angleLeft;
        }
        
        private void FixedUpdate()
        {
            if (direction == false)
            {
                currAngle += 2.4f * frequency;
                if (currAngle>=angleRight)
                {
                    direction = true;
                }
            }
            else
            {
                currAngle -= 2.4f * frequency;
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

        public GameObject BaseGrid;
        public ScanLineController SLController;
        public GameObject ScanLine;
        public GameObject LeftAngleIndicator;
        public GameObject RightAngleIndicator;
        public GameObject Chooser;



        private MeshFilter BaseGridMeshFilter;
        private MeshRenderer BaseGridRenderer;
        private MeshFilter ScanLineMeshFilter;
        private MeshRenderer ScanLineRenderer;
        private MeshFilter LeftAngleIndicatorMeshFilter;
        private MeshRenderer LeftAngleIndicatorRenderer;
        private MeshFilter RightAngleIndicatorMeshFilter;
        private MeshRenderer RightAngleIndicatorRenderer;
        private MeshFilter ChooserMeshFilter;
        private MeshRenderer ChooserRenderer;
        private float leftScanAngle = -60f;
        private float rightScanAngle = 60f;
        private float middleScanAngle = 0f;
        private float deltaScanAngle = 60f;
        private Vector2 ChooserPosition = new Vector2(0, 0);
        private bool smallerAngle = false;
        private bool biggerAngle = false;
        private bool upChooser = false;
        private bool downChooser = false;
        private bool leftChooser = false;
        private bool rightChooser = false;


        public void InitGrid()
        {
            if (!transform.FindChild("Grid"))
            {
                BaseGrid = new GameObject("Grid");
                BaseGrid.transform.SetParent(transform);
                BaseGrid.transform.localPosition = new Vector3(0f, 0f, 0.095f);
                BaseGrid.transform.localRotation = Quaternion.Euler(270, 0, 180);
                BaseGrid.transform.localScale = new Vector3(0.105f,0.105f,0.105f);
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

        }

        public override void SafeAwake()
        {
            EnlargeScanAngle = AddKey("扩大搜索角", "EnlargeScanAngle", KeyCode.T);
            ReduceScanAngle = AddKey("缩小搜索角", "ReduceScanAngle", KeyCode.U);
            ChooserUp = AddKey("上移选取器", "ChooserUp", KeyCode.Y);
            ChooserDown = AddKey("下移选取器", "ChooserDown", KeyCode.H);
            ChooserLeft = AddKey("左移选取器", "ChooserLeft", KeyCode.G);
            ChooserRight = AddKey("右移选取器", "ChooserRight", KeyCode.J);
            InitGrid();
            InitPanel();
        }

        public void Start()
        {
            
            
        }

        public override void OnSimulateStart()
        {
            SLController = transform.gameObject.AddComponent<ScanLineController>();
            ScanLine.SetActive(true);
            LeftAngleIndicator.SetActive(true);
            RightAngleIndicator.SetActive(true);
            Chooser.SetActive(true);
        }

        private void Update()
        {
            //set the position of scanLine
            ScanLine.transform.localPosition = new Vector3(SLController.currAngle*0.00175f, 0, 0.095f);
            //set the position of chooser
            Chooser.transform.localPosition = new Vector3(ChooserPosition.x * 0.00175f, -ChooserPosition.y * 0.00175f, 0.095f);
            //set the position of left&right angleIndicator
            LeftAngleIndicator.transform.localPosition = new Vector3(leftScanAngle * 0.00175f, 0, 0.095f);
            RightAngleIndicator.transform.localPosition = new Vector3(rightScanAngle * 0.00175f, 0, 0.095f);
            //modify ScanLineController
            SLController.angleLeft = leftScanAngle;
            SLController.angleRight = rightScanAngle;

            //judge whether the key for adjusting Scan angle is pressed
            if (EnlargeScanAngle.IsPressed)
            {
                biggerAngle = true;
            }else if (EnlargeScanAngle.IsReleased)
            {
                biggerAngle = false;
            }
            if (ReduceScanAngle.IsPressed)
            {
                smallerAngle = true;
            }else if (ReduceScanAngle.IsReleased)
            {
                smallerAngle = false;
            }

            //judge wether the key for adjusting chooser is pressed
            if (ChooserUp.IsPressed)
            {
                upChooser = true;
            }else if (ChooserUp.IsReleased)
            {
                upChooser = false;
            }
            if (ChooserDown.IsPressed)
            {
                downChooser = true;
            }else if (ChooserDown.IsReleased)
            {
                downChooser = false;
            }
            if (ChooserLeft.IsPressed)
            {
                leftChooser = true;
            }else if (ChooserLeft.IsReleased)
            {
                leftChooser = false;
            }
            if (ChooserRight.IsPressed)
            {
                rightChooser = true;
            }else if (ChooserRight.IsReleased)
            {
                rightChooser = false;
            }
            

        }
        public override void SimulateFixedUpdateHost()
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
            middleScanAngle = ChooserPosition.x;
            //adjust the scan angle according to key
            if (biggerAngle && deltaScanAngle<=60)
            {
                deltaScanAngle += 0.4f;
            }
            else if (smallerAngle && deltaScanAngle>=0)
            {
                deltaScanAngle -= 0.4f;
            }
            leftScanAngle = Math.Max(-60f, middleScanAngle - deltaScanAngle);
            rightScanAngle = Math.Min(60f, middleScanAngle + deltaScanAngle);


        }

        void OnGUI()
        {
            if (BlockBehaviour.isSimulating)
            {
                //GUI.Box(new Rect(100, 100, 200, 50), ScanLine.transform.localPosition.ToString());

            }
        }

    }
}
