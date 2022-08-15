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
                currAngle += 1.2f * frequency;
                if (currAngle>=angleRight)
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

    class DisplayerBlock : BlockScript
    {
        public MKey EnlargeScanAngle;
        public MKey ReduceScanAngle;
        public MKey ChooserUp;
        public MKey ChooserDown;
        public MKey ChooserLeft;
        public MKey ChooserRight;
        public MKey scanUp;
        public MKey scanDown;
        public MKey Lock;

        public GameObject BaseGrid;
        public ScanLineController SLController;
        public GameObject ScanLine;
        public GameObject LeftAngleIndicator;
        public GameObject RightAngleIndicator;
        public GameObject Chooser;
        public GameObject LockIcon;
        public GameObject LockInfo;
        public GameObject Mode;
        public float radarPitch = 0;
        public GameObject EnemyDisplayTWS; // gameobject for managing enemy icons
        public GameObject[] EnemyIconsTWS;
        
        



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
        private TextMesh ModeTextMesh;
        private TextMesh InfoText;
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
        private bool downScan = false;
        private bool upScan = false;
        private string mode = "TWS";
        public displayerData DisplayerData = new displayerData(0, 0);
        private int playerID;
        private int currRegion;
        private Target[] RadarTarget;
        private bool locking; //whether the radar keeps tracking an object
        private int lockRegion = 0;



        public void InitEnemyIcons()
        {
            if (!transform.parent.FindChild("EnemyDisplayTWS"))
            {
                EnemyDisplayTWS = new GameObject("EnemyDisplayTWS");
                EnemyDisplayTWS.transform.SetParent(transform);
                EnemyDisplayTWS.transform.localPosition = new Vector3(0f, 0f, 0.095f);
                EnemyDisplayTWS.transform.localRotation = Quaternion.Euler(0, 180, 180);
                EnemyDisplayTWS.transform.localScale = new Vector3(1f, 1f, 1f);
                EnemyIconsTWS = new GameObject[101];
                for (int i = 0; i < 101; i++)
                {
                    EnemyIconsTWS[i] = new GameObject("enemy" + i.ToString());
                    EnemyIconsTWS[i].transform.SetParent(EnemyDisplayTWS.transform);
                    EnemyIconsTWS[i].transform.localPosition = new Vector3(0f, 0f, 0f);
                    EnemyIconsTWS[i].transform.localRotation = Quaternion.Euler(0, 180, 180);
                    EnemyIconsTWS[i].transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                    TextMesh textMesh;
                    textMesh = EnemyIconsTWS[i].AddComponent<TextMesh>();
                    textMesh.text = "-";
                    textMesh.color = Color.green;
                    textMesh.characterSize = 0.5f;
                    textMesh.fontSize = 64;
                    textMesh.fontStyle = FontStyle.Bold;
                    textMesh.anchor = TextAnchor.MiddleCenter;
                    EnemyIconsTWS[i].SetActive(false);
                    
                }
            }
        }

        public void DisplayEnemy()
        {
            
            if (RadarTarget[currRegion].hasObject)
            {
                EnemyIconsTWS[currRegion].transform.localPosition = new Vector3(0.002f * (currRegion - 50), -0.094f + RadarTarget[currRegion].distance * 0.00004f, 0f);
                EnemyIconsTWS[currRegion].SetActive(true);
            }
            else
            {
                EnemyIconsTWS[currRegion].SetActive(false);
            }
            ClearBlank();
        }

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
            if (!transform.FindChild("LockIcon"))
            {
                LockIcon = new GameObject("LockIcon");
                LockIcon.transform.SetParent(transform);
                LockIcon.transform.localPosition = new Vector3(0f, 0f, 0.095f);
                LockIcon.transform.localRotation = Quaternion.Euler(0, 0, 0);
                LockIcon.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                TextMesh textMesh;
                textMesh = LockIcon.AddComponent<TextMesh>();
                textMesh.text = "O";
                textMesh.color = Color.yellow;
                textMesh.characterSize = 0.6f;
                textMesh.fontSize = 32;
                textMesh.fontStyle = FontStyle.Normal;
                textMesh.anchor = TextAnchor.MiddleCenter;


                LockIcon.SetActive(false);
            }



        }

        public void InitMode(string mode)
        {
            if (!transform.FindChild("Mode"))
            {
                Mode = new GameObject("Mode");
                Mode.transform.SetParent(transform);
                Mode.transform.localPosition = new Vector3(-0.08f, 0.09f, 0.095f);
                Mode.transform.localRotation = Quaternion.Euler(0, 180, 180);
                Mode.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                ModeTextMesh = Mode.AddComponent<TextMesh>();
                ModeTextMesh.text = mode;
                ModeTextMesh.characterSize = 0.25f;
                ModeTextMesh.fontSize = 64;
                ModeTextMesh.anchor = TextAnchor.MiddleCenter;
                ModeTextMesh.color = Color.green;
                Mode.SetActive(false);
            }
            
        }

        private bool FindLockedTarget()
        {
            
            
            for (int i = 0; i < 21; i++)
            {
                if (lockRegion-10+i>=0 && lockRegion - 10+i<=100)
                {
                    if (RadarTarget[lockRegion - 10 + i].hasObject)
                    {
                        LockIcon.SetActive(true);
                        lockRegion = lockRegion - 10+i;
                        ChooserPosition.x = (lockRegion - 50) * 1.2f;
                        Chooser.transform.localPosition = new Vector3(ChooserPosition.x * 0.00175f, 0.094f - RadarTarget[lockRegion].distance * 0.00004f, 0.095f);
                        LockIcon.transform.localPosition = new Vector3(ChooserPosition.x * 0.00175f, 0.094f - RadarTarget[lockRegion].distance * 0.00004f, 0.095f);

                        try
                        {
                            DestroyImmediate(LockIcon.transform.FindChild("LockInfo").gameObject);
                        }
                        catch { }

                        LockInfo = new GameObject("LockInfo");
                        LockInfo.transform.SetParent(LockIcon.transform);
                        LockInfo.transform.localPosition = new Vector3(2f, 0f, 0f);
                        LockInfo.transform.localRotation = Quaternion.Euler(180, 0, 0);
                        LockInfo.transform.localScale = new Vector3(1f, 1f, 1f);
                        InfoText = LockInfo.AddComponent<TextMesh>();
                        InfoText.text = Math.Floor(RadarTarget[lockRegion].closingRate).ToString();
                        InfoText.fontSize = 32;
                        InfoText.fontStyle = FontStyle.Normal;
                        InfoText.anchor = TextAnchor.LowerLeft;
                        InfoText.characterSize = 0.6f;
                        InfoText.color = Color.yellow;
                        LockInfo.SetActive(true);





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

        private void ClearBlank()
        {
            float leftRegion = (leftScanAngle + 60) / 1.2f;
            float rightRegion = (rightScanAngle + 60) / 1.2f;
            for (int i = 0; i <= 100; i++)
            {
                if (i < leftRegion)
                {
                    EnemyIconsTWS[i].SetActive(false);
                }
                else
                {
                    break;
                }
            }
            for (int i = 100; i >= 0; i--)
            {
                if (i > rightRegion)
                {
                    EnemyIconsTWS[i].SetActive(false);
                }
                else
                {
                    break;
                }
            }
        }
        

        public override void SafeAwake()
        {
            playerID = BlockBehaviour.ParentMachine.PlayerID;
            Lock = AddKey("锁定目标", "Lock Target", KeyCode.X);
            EnlargeScanAngle = AddKey("扩大搜索角", "EnlargeScanAngle", KeyCode.T);
            ReduceScanAngle = AddKey("缩小搜索角", "ReduceScanAngle", KeyCode.U);
            ChooserUp = AddKey("上移光标", "ChooserUp", KeyCode.Y);
            ChooserDown = AddKey("下移光标", "ChooserDown", KeyCode.H);
            ChooserLeft = AddKey("左移光标", "ChooserLeft", KeyCode.G);
            ChooserRight = AddKey("右移光标", "ChooserRight", KeyCode.J);
            scanUp = AddKey("雷达向上", "scan up", KeyCode.I);
            scanDown = AddKey("雷达向下", "scan down", KeyCode.K);
            
            InitGrid();
            InitPanel();
            InitMode(mode);
            InitEnemyIcons();
        }

        public void Start()
        {
            if (IsSimulating)
            {
                if (!transform.parent.FindChild("Displayer"))
                {
                    name = "Displayer";
                }
            }
            
        }

        public override void OnSimulateStart()
        {
            SLController = transform.gameObject.AddComponent<ScanLineController>();
            ScanLine.SetActive(true);
            LeftAngleIndicator.SetActive(true);
            RightAngleIndicator.SetActive(true);
            Chooser.SetActive(true);
            Mode.SetActive(true);
        }

        private void Update()
        {
            try
            {
                //set the position of scanLine
                ScanLine.transform.localPosition = new Vector3(SLController.currAngle * 0.00175f, 0, 0.095f);
                //set the position of chooser
                if (!locking)
                {
                    Chooser.transform.localPosition = new Vector3(ChooserPosition.x * 0.00175f, -ChooserPosition.y * 0.00175f, 0.095f);
                }
                
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
                }
                else if (EnlargeScanAngle.IsReleased)
                {
                    biggerAngle = false;
                }
                if (ReduceScanAngle.IsPressed)
                {
                    smallerAngle = true;
                }
                else if (ReduceScanAngle.IsReleased)
                {
                    smallerAngle = false;
                }

                //judge wether the key for adjusting chooser is pressed
                if (ChooserUp.IsPressed)
                {
                    upChooser = true;
                }
                else if (ChooserUp.IsReleased)
                {
                    upChooser = false;
                }
                if (ChooserDown.IsPressed)
                {
                    downChooser = true;
                }
                else if (ChooserDown.IsReleased)
                {
                    downChooser = false;
                }
                if (ChooserLeft.IsPressed)
                {
                    leftChooser = true;
                }
                else if (ChooserLeft.IsReleased)
                {
                    leftChooser = false;
                }
                if (ChooserRight.IsPressed)
                {
                    rightChooser = true;
                }
                else if (ChooserRight.IsReleased)
                {
                    rightChooser = false;
                }

                //judage wether the key for adjusting pitch of radar is pressed
                if (scanUp.IsPressed)
                {
                    upScan = true;
                }else if (scanUp.IsReleased)
                {
                    upScan = false;
                }
                if (scanDown.IsPressed)
                {
                    downScan = true;
                }else if (scanDown.IsReleased)
                {
                    downScan = false;
                }

                //judage wether the key for toggle lock mode is pressed
                if (Lock.IsPressed)
                {
                    locking = !locking;
                    if (!locking)
                    {
                        LockIcon.SetActive(false);
                    }
                }

            }
            catch { }
            
            

        }
        public override void SimulateFixedUpdateHost()
        {
             currRegion = (int)Math.Floor((SLController.currAngle + 60) / 1.2f + 0.5f);
             lockRegion = (int)Math.Floor((ChooserPosition.x + 60) / 1.2f + 0.5f);


            //start track if lock mode on
            if (locking)
            {
                if (!FindLockedTarget())
                {
                    LockIcon.SetActive(false);
                    locking = false;
                }
            }
            else
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
            //adjust the pitch of radar according to key
            if (upScan && radarPitch < 35)
            {
                radarPitch += 0.4f;
            }else if(downScan && radarPitch > -35)
            {
                radarPitch -= 0.4f;
            }

            

            leftScanAngle = Math.Max(-60f, middleScanAngle - deltaScanAngle);
            rightScanAngle = Math.Min(60f, middleScanAngle + deltaScanAngle);
            DisplayerData.radarPitch = radarPitch;
            DisplayerData.radarAngle = SLController.currAngle;
            DataManager.Instance.DisplayerData[playerID] = DisplayerData;
            
            RadarTarget = DataManager.Instance.TargetData[playerID].targets;
            DisplayEnemy();

            

        }

        void OnGUI()
        {
            if (BlockBehaviour.isSimulating)
            {
                GUI.Box(new Rect(100, 100, 200, 50), leftScanAngle.ToString());
                GUI.Box(new Rect(100, 150, 200, 50), leftScanAngle.ToString());

            }
        }

    }
}
