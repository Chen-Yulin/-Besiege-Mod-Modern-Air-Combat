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
    class ConeCollisonHit : MonoBehaviour
    {
        public MPTeam team = MPTeam.None;
        public bool IFF = true;
        public ushort PlayerID = 0;
        public Stack<Collider> targetCols = new Stack<Collider>();


        public void Reset()
        {
            targetCols.Clear();
        }

        void Start()
        {
        }
        void Update()
        {
        }

        void OnTriggerEnter(Collider col)
        {
            MPTeam hitedTeam; 
            if (targetCols.Count > 5)
                return;
            if (col.isTrigger || col.transform.parent.GetInstanceID() == col.GetInstanceID())
                return;
            try
            {
                BlockBehaviour hitedBlock = col.attachedRigidbody.gameObject.GetComponent<BlockBehaviour>();
                hitedTeam = hitedBlock.Team;
            }
            catch 
            {
                return;
            }
            targetCols.Push(col);
        }


    }

    class SRAAMBlock : BlockScript
    {
        public MKey Launch;
        public MToggle IFF;
        public MSlider detectAngleSlider;
        public MToggle showScanner;
        public MKey showTrail;
        public MSlider detectDelay;
        public MSlider launchDelay;
        public enum status { stored, launched, missed, exploded };
        public status myStatus;
        public GameObject ScanCollider;
        public MeshCollider missleScan;
        public ConeCollisonHit coneHit;
        public GameObject ScannerDisplay;
        public bool targetDetected = false;
        public Vector3 predictPosition;
        public Vector3 predictPositionModified;
        //public GameObject Prediction;
        //public Rigidbody PredictionRigid;
        public GameObject TrailFlame;
        public GameObject TrailSmoke;
        public ParticleSystem TrailFlameParticle;
        public ParticleSystem TrailSmokeParticle;
        public GameObject ExploFireball;
        public GameObject ExploTrailsBlack;
        public GameObject ExploDust;
        public GameObject ExploShockwave;
        public GameObject ExploSmokeBlack;
        public GameObject ExploShower;
        public ParticleSystem ExploFireballParticle;
        public ParticleSystem ExploTrailsBlackParticle;
        public ParticleSystem ExploDustParticle;
        public ParticleSystem ExploShockwaveParticle;
        public ParticleSystem ExploSmokeBlackParticle;
        public ParticleSystem ExploShowerParticle;



        private float estimatedTime;
        private Transform myTransform;      //实例化Transform对象
        private Rigidbody myRigidbody;
        private float time = 0;
        private float detectFreqTime = 0;
        private float detectRange;
        private float detectWidth;
        private MeshFilter scannerMeshFilter;
        private Mesh scannerMesh;
        private MeshRenderer scannerRenderer;
        private Texture AimIcon;

        private int iconSize;
        private Color scannerColor;
        private Quaternion launchRotation;
        private bool getlaunchRotation = false;
        private bool activeTrail = false;


        public void AxisLookAt(Transform tr_self, Vector3 lookPos, Vector3 directionAxis)
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
            tr_self.rotation = Quaternion.Lerp(rotation, Quaternion.AngleAxis(angle, axis) * rotation,0.1f);

        }//from CSDN


        public void initScan()
        {
            if (BlockBehaviour.transform.FindChild("ScanCol") == null)
            {
                scannerMesh = ModResource.GetMesh("Cone Scan").Mesh;

                ScanCollider = new GameObject("ScanCol");
                ScanCollider.transform.SetParent(BlockBehaviour.transform);
                ScanCollider.transform.localPosition = new Vector3(0f, 15f, 0.3f);
                ScanCollider.transform.localRotation = Quaternion.Euler(90, 0, 0);
                ScanCollider.transform.localScale = Vector3.one;
                missleScan = ScanCollider.AddComponent<MeshCollider>();
                missleScan.sharedMesh = scannerMesh;
                missleScan.convex = true;
                missleScan.isTrigger = true;
                coneHit = ScanCollider.AddComponent<ConeCollisonHit>();

                ScanCollider.SetActive(false);
                coneHit.Reset();
            }


            if (BlockBehaviour.transform.FindChild("Scanner Display") == null)
            {
                // render the mesh of scanner
                ScannerDisplay = new GameObject("Scanner Display");
                ScannerDisplay.transform.SetParent(BlockBehaviour.transform);
                ScannerDisplay.transform.localPosition = new Vector3(0f, 15f, 0.3f);
                ScannerDisplay.transform.localRotation = Quaternion.Euler(90, 0, 0);
                ScannerDisplay.transform.localScale = Vector3.one;
                scannerMeshFilter = ScannerDisplay.AddComponent<MeshFilter>();
                scannerMeshFilter.mesh = scannerMesh;
                scannerRenderer = ScannerDisplay.AddComponent<MeshRenderer>();
                scannerRenderer.material = new Material(Shader.Find("Particles/Alpha Blended"));
                scannerColor = Color.green;
                scannerColor.a = 0.05f;
                scannerRenderer.material.SetColor("_TintColor", scannerColor);
                ScannerDisplay.SetActive(false);
            }
        }

        private void initTrail()
        {
            TrailSmoke = Instantiate(AssetManager.Instance.Trail.SmokeTrail);
            TrailFlame = Instantiate(AssetManager.Instance.Trail.FlameTrail);
            TrailSmoke.transform.SetParent(BlockBehaviour.transform);
            TrailFlame.transform.SetParent(BlockBehaviour.transform);
            TrailSmoke.transform.localPosition = new Vector3(0, -4f, 0.3f);
            TrailSmoke.transform.localRotation = Quaternion.Euler(90, 0, 0);
            TrailSmoke.transform.localScale = Vector3.one;
            TrailFlame.transform.localPosition = new Vector3(0, -1.2f, 0.3f);
            TrailFlame.transform.localRotation = Quaternion.Euler(90, 0, 0);
            TrailFlame.transform.localScale = Vector3.one;

            TrailSmoke.SetActive(true);
            TrailFlame.SetActive(true);
        }

        private void initExplo()
        {
            ExploFireball = Instantiate(AssetManager.Instance.Explo.ExploFireball);
            ExploTrailsBlack = Instantiate(AssetManager.Instance.Explo.ExploTrailsBlack);
            ExploDust = Instantiate(AssetManager.Instance.Explo.ExploDust);
            ExploShockwave = Instantiate(AssetManager.Instance.Explo.ExploShockwave);
            ExploSmokeBlack = Instantiate(AssetManager.Instance.Explo.ExploSmokeBlack);
            ExploShower = Instantiate(AssetManager.Instance.Explo.ExploShower);

            ExploFireball.transform.SetParent(BlockBehaviour.transform);
            ExploTrailsBlack.transform.SetParent(BlockBehaviour.transform);
            ExploDust.transform.SetParent(BlockBehaviour.transform);
            ExploShockwave.transform.SetParent(BlockBehaviour.transform);
            ExploSmokeBlack.transform.SetParent(BlockBehaviour.transform);
            ExploShower.transform.SetParent(BlockBehaviour.transform);

            ExploFireball.transform.localPosition = new Vector3(0, -4f, 0.3f);
            ExploFireball.transform.localRotation = Quaternion.Euler(90, 0, 0);
            ExploFireball.transform.localScale = Vector3.one;

            ExploTrailsBlack.transform.localPosition = new Vector3(0, -4f, 0.3f);
            ExploTrailsBlack.transform.localRotation = Quaternion.Euler(90, 0, 0);
            ExploTrailsBlack.transform.localScale = Vector3.one;

            ExploDust.transform.localPosition = new Vector3(0, -4f, 0.3f);
            ExploDust.transform.localRotation = Quaternion.Euler(90, 0, 0);
            ExploDust.transform.localScale = Vector3.one;

            ExploShockwave.transform.localPosition = new Vector3(0, -4f, 0.3f);
            ExploShockwave.transform.localRotation = Quaternion.Euler(90, 0, 0);
            ExploShockwave.transform.localScale = Vector3.one;

            ExploSmokeBlack.transform.localPosition = new Vector3(0, -4f, 0.3f);
            ExploSmokeBlack.transform.localRotation = Quaternion.Euler(90, 0, 0);
            ExploSmokeBlack.transform.localScale = Vector3.one;

            ExploShower.transform.localPosition = new Vector3(0, -4f, 0.3f);
            ExploShower.transform.localRotation = Quaternion.Euler(90, 0, 0);
            ExploShower.transform.localScale = Vector3.one;

            ExploFireball.SetActive(false);
            ExploTrailsBlack.SetActive(false);
            ExploDust.SetActive(false);
            ExploShockwave.SetActive(false);
            ExploSmokeBlack.SetActive(false);
            ExploShower.SetActive(false);
        }

        private void initParticleSystem()
        {
            TrailSmokeParticle = TrailSmoke.GetComponent<ParticleSystem>();
            TrailFlameParticle = TrailFlame.GetComponent<ParticleSystem>();
            ExploFireballParticle = ExploFireball.GetComponent<ParticleSystem>();
            ExploTrailsBlackParticle = ExploTrailsBlack.GetComponent<ParticleSystem>();
            ExploDustParticle = ExploDust.GetComponent<ParticleSystem>();
            ExploShockwaveParticle = ExploShockwave.GetComponent<ParticleSystem>();
            ExploSmokeBlackParticle = ExploSmokeBlack.GetComponent<ParticleSystem>();
            ExploShowerParticle = ExploShower.GetComponent<ParticleSystem>();
        }

        private void playExplo()
        {
            ExploFireball.SetActive(true);
            ExploTrailsBlack.SetActive(true);
            ExploDust.SetActive(true);
            ExploShockwave.SetActive(true);
            ExploSmokeBlack.SetActive(true);
            ExploShower.SetActive(true);
            ExploFireballParticle.Play();
            ExploTrailsBlackParticle.Play();
            ExploDustParticle.Play();
            ExploShockwaveParticle.Play();
            ExploSmokeBlackParticle.Play();
            ExploShowerParticle.Play();
        }


        private void GetAim()
        {
            ScanCollider.SetActive(true);
            
            detectFreqTime += Time.fixedDeltaTime;
            

            if (detectFreqTime >= 0.02)
            {
                if (coneHit.targetCols.Count == 0)
                {
                    targetDetected = false;
                    return;
                }
                else
                {
                    targetDetected = true;
                }
                detectFreqTime = 0;

                //Debug.Log("Targets:");
                //foreach (Collider col in coneHit.targetCols)
                //{
                //    Debug.Log(col.transform.parent.gameObject.name);
                //}

                Collider targetCol = coneHit.targetCols.Peek();
                Vector3 targetPosition = targetCol.transform.position;
                //Debug.Log(targetPosition);
                Vector3 targetVelocity = targetCol.attachedRigidbody.velocity;
                //Debug.Log(targetVelocity);
                

                //calculate three times to ensure precision
                float myVelocity = Rigidbody.velocity.magnitude;
                //Debug.Log(myVelocity);
                estimatedTime = (targetPosition - transform.position).magnitude/myVelocity;
                predictPosition = targetPosition + targetVelocity * estimatedTime;
                estimatedTime = (predictPosition - transform.position).magnitude / myVelocity;
                predictPosition = targetPosition + targetVelocity * estimatedTime;
                estimatedTime = (predictPosition - transform.position).magnitude / myVelocity;
                predictPosition = targetPosition + targetVelocity * estimatedTime;
                launchRotation = transform.rotation;
                ScanCollider.SetActive(false);
                coneHit.Reset();
            }
            Vector3 positionDiff = predictPosition - (transform.position + Rigidbody.velocity * estimatedTime);
            //Debug.Log(positionDiff);
            Vector3 modifiedDiff;
            if (positionDiff.magnitude < 200)
            {
                modifiedDiff.x = (float)(0.6f * positionDiff.x);
                modifiedDiff.y = (float)(0.6f * positionDiff.y);
                modifiedDiff.z = (float)(0.6f * positionDiff.z);
            }
            else
            {
                modifiedDiff.x = (float)(0.2f * positionDiff.x);
                modifiedDiff.y = (float)(0.2f * positionDiff.y);
                modifiedDiff.z = (float)(0.2f * positionDiff.z);
            }
            
            predictPositionModified = predictPosition + modifiedDiff ;

            

                //PredictionRigid.transform.position = predictPosition;
                //Debug.Log(predictPosition);


            


        }


        public override void SafeAwake()
        {
            Launch = AddKey("发射", "launch", KeyCode.X);
            showTrail = AddKey("显示尾迹", "showTrail", KeyCode.T);
            IFF = AddToggle("开启友伤", "IFF", true);
            showScanner = AddToggle("显示探测范围", "showScanner", false);
            detectAngleSlider = AddSlider("探测角度", "detection angle", 90.0f, 60.0f, 120.0f);
            detectDelay = AddSlider("延时保险", "detection delay", 0.2f, 0.0f, 1f);
            launchDelay = AddSlider("延时点火", "launch delay", 0.1f, 0.0f, 0.3f);

            initScan();//挂载上导弹前方的圆锥触发器
            initTrail();
            initExplo();

            AimIcon = ModResource.GetTexture("Aim Icon").Texture;
        }

        public void Start()
        {
            myTransform = gameObject.GetComponent<Transform>();        //获取相应对象的引用
            myRigidbody = gameObject.GetComponent<Rigidbody>();

            myRigidbody.drag = 0f;
            myRigidbody.angularDrag = 0f;

            detectRange = (float)(400.0f * System.Math.Cos(detectAngleSlider.Value / 2 * 3.1415f / 180));
            detectWidth = 2 * (float)(System.Math.Tan(detectAngleSlider.Value / 2 * 3.1415f / 180) * detectRange);
            Vector3 ScanColScale = new Vector3(detectWidth, detectWidth, detectRange);
            ScanCollider.transform.localScale = ScanColScale;
            ScannerDisplay.transform.localScale = ScanColScale;
            //Debug.Log(ScanColScale);
            coneHit.IFF = IFF.IsActive;
            coneHit.team = BlockBehaviour.Team;
            coneHit.PlayerID = BlockBehaviour.ParentMachine.PlayerID;

            if (showScanner.IsActive)
            {
                ScannerDisplay.SetActive(true);
            }
            else
            {
                ScannerDisplay.SetActive(false);
            }
            
            initParticleSystem();


        }

        public override void OnSimulateStart()
        {
            base.OnSimulateStart();
        }

        public override void OnSimulateStop()
        {
            base.OnSimulateStop();
        }

        private void Update()
        {
            if (IsSimulating)
            {
                if (Launch.IsHeld && myStatus == status.stored)
                {
                    myStatus = status.launched;
                    myRigidbody.drag = 0.1f;
                    myRigidbody.angularDrag = 4.0f;
                    //Debug.Log("missle launched");
                    //Debug.Log(detectRange);
                }


            }
            
        }


        public override void SimulateFixedUpdateHost()
        {

            if (Launch.EmulationHeld() && myStatus == status.stored)
            {
                myStatus = status.launched;
                //Debug.Log("missle launched");
                //Debug.Log(detectRange);
                myRigidbody.drag = 0.1f;
                myRigidbody.angularDrag = 4.0f;

            }

            if (myStatus == status.launched)
            {
                if (!getlaunchRotation)
                {
                    myRigidbody.drag = 3f;
                    myRigidbody.angularDrag = 4.0f;
                    launchRotation = transform.rotation;
                    getlaunchRotation = true;
                    //Debug.Log(launchRotation);
                }

                if (time < 3.0f + launchDelay.Value)
                {
                    if (time > launchDelay.Value)
                    {
                        if(activeTrail == false)
                        {
                            TrailSmokeParticle.Play();
                            TrailFlameParticle.Play();
                            activeTrail = true;
                        }
                        myRigidbody.AddRelativeForce(new Vector3(0, 3000, 0), ForceMode.Force);
                    }

                    if (time < detectDelay.Value + launchDelay.Value)
                    {
                        myTransform.rotation = Quaternion.Lerp(transform.rotation, launchRotation, 0.1f);
                    }
                    else
                    {
                        GetAim();
                        if (!targetDetected)
                        {
                            myTransform.rotation = Quaternion.Lerp(transform.rotation, launchRotation, 0.1f);
                        }
                        else
                        {
                            //myTransform.up = Vector3.Lerp(myTransform.up, predictPositionModified - myTransform.position,0.01f);
                            AxisLookAt(myTransform, predictPositionModified, Vector3.up);
                        }
                    }
                    time += Time.fixedDeltaTime;
                }
                else
                {
                    if (activeTrail == true)
                    {
                        TrailSmokeParticle.Stop();
                        TrailFlameParticle.Stop();
                        activeTrail = false;
                        playExplo();
                    }
                    myStatus = status.missed;
                    myRigidbody.drag = 0.1f;
                    myRigidbody.angularDrag = 1.0f;
                }
            }
            if (myStatus == status.missed)
            { 
                targetDetected = false;
            
                if (myRigidbody.position.y > 20)
                {
                    myTransform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(-180, 0, 0), 0.005f);
                }
            }
        }

        void OnGUI()
        {
            if (BlockBehaviour.isSimulating)
            {
                //GUI.Box(new Rect(100, 100, 200, 50), myRigidbody.velocity.ToString());
                if (targetDetected)
                {
                    //GUI.Box(new Rect(100, 100, 100, 50), predictPosition.ToString());
                    
                    //GUI.Box(new Rect(100, 150, 200, 50), myRigidbody.transform.up.ToString());
                    //GUI.Box(new Rect(100, 200, 200, 50), (predictPositionModified - myTransform.position).normalized.ToString());

                    iconSize = 32;
                    GUI.color = Color.green;
                    Vector3 onScreenPosition = Camera.main.WorldToScreenPoint(predictPosition);
                    if (onScreenPosition.z >= 0)
                        GUI.DrawTexture(new Rect(onScreenPosition.x - iconSize / 2, Camera.main.pixelHeight - onScreenPosition.y - iconSize / 2, iconSize, iconSize), AimIcon);
                }
            }
        }
        

    }
}
