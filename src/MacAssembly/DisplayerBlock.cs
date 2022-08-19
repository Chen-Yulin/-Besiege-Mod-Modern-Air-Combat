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
        public GameObject PitchIndicatorSelf;
        public GameObject PitchIndicatorTarget;
        public Rigidbody myRigid;
        
        



        protected MeshFilter BaseGridMeshFilter;
        protected MeshRenderer BaseGridRenderer;
        protected MeshFilter ScanLineMeshFilter;
        protected MeshRenderer ScanLineRenderer;
        protected MeshFilter LeftAngleIndicatorMeshFilter;
        protected MeshRenderer LeftAngleIndicatorRenderer;
        protected MeshFilter RightAngleIndicatorMeshFilter;
        protected MeshRenderer RightAngleIndicatorRenderer;
        protected MeshFilter ChooserMeshFilter;
        protected MeshRenderer ChooserRenderer;
        protected TextMesh ModeTextMesh;
        protected TextMesh InfoText;
        protected Texture LockIconOnScreen;
        protected float leftScanAngle = -60f;
        protected float rightScanAngle = 60f;
        protected float middleScanAngle = 0f;
        protected float realMiddleScanAngle = 0f;
        protected float deltaScanAngle = 60f;
        protected Vector2 ChooserPosition = new Vector2(0, 0);
        protected bool smallerAngle = false;
        protected bool biggerAngle = false;
        protected bool upChooser = false;
        protected bool downChooser = false;
        protected bool leftChooser = false;
        protected bool rightChooser = false;
        protected bool downScan = false;
        protected bool upScan = false;
        protected string mode = "TWS";
        public displayerData DisplayerData = new displayerData(0, 0);
        public BVRTargetData sendBVRData = new BVRTargetData();
        protected int playerID;
        protected int currRegion;
        protected Target[] RadarTarget;
        protected bool locking; //whether the radar keeps tracking an object
        protected int lockRegion = 0;
        protected int iconSize = 28;
        protected float deltaPitch = 0;




        public void InitEnemyIcons()
        {
            if (!transform.FindChild("EnemyDisplayTWS"))
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

        public void InitPitchIndicator()
        {
            if (!transform.FindChild("PitchIndicatorSelf"))
            {
                PitchIndicatorSelf = new GameObject("PitchIndicatorSelf");
                PitchIndicatorSelf.transform.SetParent(transform);
                PitchIndicatorSelf.transform.localPosition = new Vector3(-0.1f, 0f, 0.095f);
                PitchIndicatorSelf.transform.localRotation = Quaternion.Euler(180, 0, 0);
                PitchIndicatorSelf.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                TextMesh textMesh;
                textMesh = PitchIndicatorSelf.AddComponent<TextMesh>();
                textMesh.text = "]";
                textMesh.color = Color.green;
                textMesh.characterSize = 1.2f;
                textMesh.fontSize = 32;
                textMesh.fontStyle = FontStyle.Normal;
                textMesh.anchor = TextAnchor.MiddleCenter;

                PitchIndicatorSelf.SetActive(false);
            }

            if (!transform.FindChild("PitchIndicatorTarget"))
            {
                PitchIndicatorTarget = new GameObject("PitchIndicatorTarget");
                PitchIndicatorTarget.transform.SetParent(transform);
                PitchIndicatorTarget.transform.localPosition = new Vector3(-0.1f, 0f, 0.095f);
                PitchIndicatorTarget.transform.localRotation = Quaternion.Euler(0, 0, 0);
                PitchIndicatorTarget.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                TextMesh textMesh;
                textMesh = PitchIndicatorTarget.AddComponent<TextMesh>();
                textMesh.text = "O";
                textMesh.color = Color.yellow;
                textMesh.characterSize = 0.6f;
                textMesh.fontSize = 32;
                textMesh.fontStyle = FontStyle.Normal;
                textMesh.anchor = TextAnchor.MiddleCenter;

                PitchIndicatorTarget.SetActive(false);
            }
        }
        public void DisplayEnemy()
        {
            
            if (RadarTarget[currRegion].hasObject)
            {
                EnemyIconsTWS[currRegion].transform.localPosition = new Vector3(0.002f * (currRegion - 50), -0.1f + RadarTarget[currRegion].distance * 0.000033f, 0f);
                EnemyIconsTWS[currRegion].SetActive(true);
            }
            else
            {
                EnemyIconsTWS[currRegion].SetActive(false);
            }
            ClearBlank();
        }

        public void DisplayPitchIndicator()
        {
            PitchIndicatorSelf.transform.localPosition = new Vector3(-0.1f, -radarPitch / (350+150/2), 0.095f);
            if (locking)
            {
                PitchIndicatorTarget.SetActive(true);
                deltaPitch = Vector3.Angle(Vector3.up, DataManager.Instance.RadarTransformForward[playerID]) - Vector3.Angle(Vector3.up, RadarTarget[lockRegion].position - transform.position);
                deltaPitch = Math.Max(deltaPitch, -35);
                deltaPitch = Math.Min(deltaPitch, 35);
                PitchIndicatorTarget.transform.localPosition = new Vector3(-0.1f, -deltaPitch / (350+150/2), 0.095f);
                radarPitch = deltaPitch;
            }
            
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

        protected bool FindLockedTarget()
        {
            bool res = false;
            
            for (int i = 0; i < 15; i++)
            {
                if (lockRegion-i>=0 && lockRegion+i<=100)
                {
                    if (RadarTarget[lockRegion + i].hasObject && !RadarTarget[lockRegion + i].isMissle)
                    {
                        
                        lockRegion = lockRegion+i;
                        res = true;
                    }
                    else if (RadarTarget[lockRegion - i].hasObject && !RadarTarget[lockRegion - i].isMissle)
                    {

                        lockRegion = lockRegion - i;
                        res = true;
                    }

                    if (res)
                    {

                        ChooserPosition.x = (lockRegion - 50) * 1.2f;
                        Chooser.transform.localPosition = new Vector3(ChooserPosition.x * 0.00175f, 0.1f - RadarTarget[lockRegion].distance * 0.0000333f, 0.095f);
                        LockIcon.transform.localPosition = new Vector3(ChooserPosition.x * 0.00175f, 0.1f - RadarTarget[lockRegion].distance * 0.0000333f, 0.095f);

                        LockIcon.SetActive(true);
                        try
                        {
                            DestroyImmediate(LockIcon.transform.FindChild("LockInfo").gameObject);
                        }
                        catch { }

                        LockInfo = new GameObject("LockInfo");
                        LockInfo.transform.SetParent(LockIcon.transform);
                        LockInfo.transform.localPosition = new Vector3(2f, 1f, 0f);
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

        protected void ClearBlank()
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
            myRigid = BlockBehaviour.GetComponent<Rigidbody>();
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
            InitPitchIndicator();
            LockIconOnScreen = ModResource.GetTexture("LockIconScreen Texture").Texture;
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
            PitchIndicatorSelf.SetActive(true);
        }

        protected void Update()
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
                    if (!locking)//what to do when mode is switched manuelly to unlock
                    {
                        radarPitch = 0f;
                        LockIcon.SetActive(false);
                        PitchIndicatorTarget.SetActive(false);
                    }
                }

            }
            catch { }
            
            

        }
        public override void SimulateFixedUpdateHost()
        {
            try
            {
                currRegion = (int)Math.Floor((SLController.currAngle + 60) / 1.2f + 0.5f);
                lockRegion = (int)Math.Floor((ChooserPosition.x + 60) / 1.2f + 0.5f);


                //start track if lock mode on
                if (locking)
                {
                    if (!FindLockedTarget())//what to do when mode is switched passively to unlock
                    {
                        radarPitch = 0f;
                        LockIcon.SetActive(false);
                        PitchIndicatorTarget.SetActive(false);
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
                if (biggerAngle && deltaScanAngle < 60)
                {
                    deltaScanAngle += 0.4f;
                }
                else if (smallerAngle && deltaScanAngle > 0)
                {
                    deltaScanAngle -= 0.4f;
                }

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


                //leftScanAngle = Math.Max(-60f, middleScanAngle - deltaScanAngle);
                //rightScanAngle = Math.Min(60f, middleScanAngle + deltaScanAngle);
                leftScanAngle = realMiddleScanAngle - deltaScanAngle;
                rightScanAngle = realMiddleScanAngle + deltaScanAngle;

                //adjust the pitch of radar according to key
                if (upScan && radarPitch < 35)
                {
                    radarPitch += 0.4f;
                }
                else if (downScan && radarPitch > -35)
                {
                    radarPitch -= 0.4f;
                }

                DisplayerData.radarPitch = radarPitch;
                DisplayerData.radarAngle = SLController.currAngle;
                DataManager.Instance.DisplayerData[playerID] = DisplayerData;


                RadarTarget = DataManager.Instance.TargetData[playerID].targets;
                DisplayEnemy();
                DisplayPitchIndicator();//always after displayEnemy


                if (locking)
                {
                    sendBVRData.position = RadarTarget[lockRegion].position;
                    sendBVRData.velocity = RadarTarget[lockRegion].velocity;
                    DataManager.Instance.BVRData[playerID] = sendBVRData;
                }
            }
            catch { }
             

        }

        void OnGUI()
        {
            //if (BlockBehaviour.isSimulating)
            //{
            //    GUI.Box(new Rect(100, 100, 200, 50), leftScanAngle.ToString());
            //    GUI.Box(new Rect(100, 150, 200, 50), leftScanAngle.ToString());

            //}
            
            if (locking)
            {
                GUI.color = Color.green;
                Vector3 onScreenPosition = Camera.main.WorldToScreenPoint(RadarTarget[lockRegion].position);
                if (onScreenPosition.z >= 0)
                    GUI.DrawTexture(new Rect(onScreenPosition.x - iconSize / 2, Camera.main.pixelHeight - onScreenPosition.y - iconSize / 2, iconSize, iconSize), LockIconOnScreen);
                GUI.Box(new Rect(100, 150, 200, 50), sendBVRData.velocity.ToString());
                GUI.Box(new Rect(100, 200, 200, 50), sendBVRData.position.ToString());
            }
            
        }

    }
}
