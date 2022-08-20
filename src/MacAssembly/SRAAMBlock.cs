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
    public class ScanCollisonHit : MonoBehaviour
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
            try
            {
                MPTeam hitedTeam;
                if (col.name == "flareCol")
                    if (UnityEngine.Random.value > 0.5f)
                        targetCols.Push(col);
                if (targetCols.Count > 5)
                    return;
                if (col.isTrigger || col.transform.parent.GetInstanceID() == col.GetInstanceID())
                    return;

                if (col.attachedRigidbody.gameObject.name == "missle")
                    return;
            
                BlockBehaviour hitedBlock = col.attachedRigidbody.gameObject.GetComponent<BlockBehaviour>();
                hitedTeam = hitedBlock.Team;
                targetCols.Push(col);
            }
            catch 
            {
                return;
            }
            
        }


    }

    public class PFCollisionHit : MonoBehaviour
    {
        public MPTeam team = MPTeam.None;
        public bool IFF = true;
        public ushort playerID = 0;
        public bool explo = false;
        
        void Start()
        {
        }

        void Update()
        {
        }

        void OnTriggerEnter(Collider col)
        {
            try
            {
                MPTeam hitedTeam;
                if (explo == true)
                    return;
                if (col.isTrigger || col.transform.parent.GetInstanceID() == col.GetInstanceID())
                    return;
                if (col.attachedRigidbody.gameObject.name == "missle")
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
                explo = true;
                //Debug.Log("Explo!!!");
            }
            catch { }
            
        }

    }



    public class SRAAMBlock : BlockScript
    {
        public MKey Launch;
        public MToggle IFF;
        //public MSlider detectAngleSlider;
        //public MToggle showScanner;
        public MSlider detectDelay;
        public MSlider launchDelay;
        public MSlider PFRang;
        public enum status { stored, launched, active, missed, exploded };
        public status myStatus;
        public GameObject ScanCollider;
        public SphereCollider missleScan;
        public ScanCollisonHit coneHit;
        //public GameObject ScannerDisplay;
        public GameObject PFCollider;
        public PFCollisionHit PFHit;
        public SphereCollider misslePF;
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
        public GameObject ExploDust;
        public GameObject ExploSmokeBlack;
        public GameObject ExploShower;
        public ParticleSystem ExploFireballParticle;
        public ParticleSystem ExploDustParticle;
        public ParticleSystem ExploSmokeBlackParticle;
        public ParticleSystem ExploShowerParticle;

        public float ExploPower = 100000f;
        public float ExploRadius = 20f;



        protected float estimatedTime;
        protected Transform myTransform;      //实例化Transform对象
        protected Rigidbody myRigidbody;
        protected float time = 0;
        protected float detectFreqTime = 0;
        //protected float detectRange;
        //protected float detectWidth;
        //protected MeshFilter scannerMeshFilter;
        //protected Mesh scannerMesh;
        //protected MeshRenderer scannerRenderer;
        protected Texture AimIcon;

        protected int iconSize;
        protected Quaternion launchRotation;
        protected bool getlaunchRotation = false;
        protected bool activeTrail = false;
        protected bool effectDestroyed = false;
        protected bool gameObjectDestroyed = false;

        protected ushort myPlayerID;


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
                //scannerMesh = ModResource.GetMesh("Cone Scan").Mesh;

                ScanCollider = new GameObject("ScanCol");
                ScanCollider.transform.SetParent(BlockBehaviour.transform);
                ScanCollider.transform.localPosition = new Vector3(0f, 600f, 0.3f);
                ScanCollider.transform.localRotation = Quaternion.Euler(90, 0, 0);
                ScanCollider.transform.localScale = Vector3.one;
                missleScan = ScanCollider.AddComponent<SphereCollider>();
                missleScan.radius = 1;
                missleScan.isTrigger = true;
                coneHit = ScanCollider.AddComponent<ScanCollisonHit>();

                ScanCollider.SetActive(false);
                coneHit.Reset();
            }


            //if (BlockBehaviour.transform.FindChild("Scanner Display") == null)
            //{
            //    // render the mesh of scanner
            //    ScannerDisplay = new GameObject("Scanner Display");
            //    ScannerDisplay.transform.SetParent(BlockBehaviour.transform);
            //    ScannerDisplay.transform.localPosition = new Vector3(0f, 600f, 0.3f);
            //    ScannerDisplay.transform.localRotation = Quaternion.Euler(90, 0, 0);
            //    ScannerDisplay.transform.localScale = Vector3.one;
            //    scannerMeshFilter = ScannerDisplay.AddComponent<MeshFilter>();
            //    scannerRenderer = ScannerDisplay.AddComponent<MeshRenderer>();
            //    scannerRenderer.material = new Material(Shader.Find("Particles/Alpha Blended"));
            //    scannerColor = Color.green;
            //    scannerColor.a = 0.05f;
            //    scannerRenderer.material.SetColor("_TintColor", scannerColor);
            //    ScannerDisplay.SetActive(false);
            //}
        }

        public void initPF()
        {
            if (BlockBehaviour.transform.FindChild("PFCol") == null)
            {
                PFCollider = new GameObject("PFCol");
                PFCollider.transform.SetParent(BlockBehaviour.transform);
                PFCollider.transform.localPosition = new Vector3(0f, 1f, 0.3f);
                PFCollider.transform.localRotation = Quaternion.Euler(0, 0, 0);
                PFCollider.transform.localScale = Vector3.one;
                misslePF = PFCollider.AddComponent<SphereCollider>();
                misslePF.isTrigger = true;
                PFCollider.SetActive(false);
                PFHit = PFCollider.AddComponent<PFCollisionHit>();
            }
        }

        protected void initTrail()
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

        protected void initExplo()
        {
            ExploFireball = Instantiate(AssetManager.Instance.Explo.ExploFireball);
            ExploDust = Instantiate(AssetManager.Instance.Explo.ExploDust);
            ExploSmokeBlack = Instantiate(AssetManager.Instance.Explo.ExploSmokeBlack);
            ExploShower = Instantiate(AssetManager.Instance.Explo.ExploShower);

            ExploFireball.transform.SetParent(BlockBehaviour.transform);
            ExploDust.transform.SetParent(BlockBehaviour.transform);
            ExploSmokeBlack.transform.SetParent(BlockBehaviour.transform);
            ExploShower.transform.SetParent(BlockBehaviour.transform);

            ExploFireball.transform.localPosition = new Vector3(0, -4f, 0.3f);
            ExploFireball.transform.localRotation = Quaternion.Euler(90, 0, 0);
            ExploFireball.transform.localScale = Vector3.one;

            ExploDust.transform.localPosition = new Vector3(0, -4f, 0.3f);
            ExploDust.transform.localRotation = Quaternion.Euler(90, 0, 0);
            ExploDust.transform.localScale = Vector3.one;

            ExploSmokeBlack.transform.localPosition = new Vector3(0, -4f, 0.3f);
            ExploSmokeBlack.transform.localRotation = Quaternion.Euler(90, 0, 0);
            ExploSmokeBlack.transform.localScale = Vector3.one;

            ExploShower.transform.localPosition = new Vector3(0, -4f, 0.3f);
            ExploShower.transform.localRotation = Quaternion.Euler(90, 0, 0);
            ExploShower.transform.localScale = Vector3.one;

            ExploFireball.SetActive(false);
            ExploDust.SetActive(false);
            ExploSmokeBlack.SetActive(false);
            ExploShower.SetActive(false);
        }

        protected void initParticleSystem()
        {
            TrailSmokeParticle = TrailSmoke.GetComponent<ParticleSystem>();
            TrailFlameParticle = TrailFlame.GetComponent<ParticleSystem>();
            ExploFireballParticle = ExploFireball.GetComponent<ParticleSystem>();
            ExploDustParticle = ExploDust.GetComponent<ParticleSystem>();
            ExploSmokeBlackParticle = ExploSmokeBlack.GetComponent<ParticleSystem>();
            ExploShowerParticle = ExploShower.GetComponent<ParticleSystem>();
        }

        protected void playExplo()
        {

            myRigidbody.constraints = RigidbodyConstraints.FreezePosition;
            TrailSmokeParticle.Stop();
            TrailFlameParticle.Stop();
            ExploFireball.SetActive(true);
            ExploDust.SetActive(true);
            ExploSmokeBlack.SetActive(true);
            ExploShower.SetActive(true);
            BlockBehaviour.MeshRenderer.enabled = false;
            myStatus = status.exploded;

            Collider[] ExploCol = Physics.OverlapSphere(transform.position, ExploRadius);
            foreach (Collider hits in ExploCol)
            {
                if (hits.GetComponent<Rigidbody>())
                {
                    hits.GetComponent<Rigidbody>().AddExplosionForce(ExploPower, transform.position, ExploRadius);
                }
            }
            
        }


        protected void GetAim()
        {
            try
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
                    estimatedTime = (targetPosition - transform.position).magnitude / myVelocity;
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
                    modifiedDiff.x = (0.6f * positionDiff.x);
                    modifiedDiff.y = (0.6f * positionDiff.y);
                    modifiedDiff.z = (0.6f * positionDiff.z);
                }
                else
                {
                    modifiedDiff.x = (0.2f * positionDiff.x);
                    modifiedDiff.y = (0.2f * positionDiff.y);
                    modifiedDiff.z = (0.2f * positionDiff.z);
                }

                predictPositionModified = predictPosition + modifiedDiff;



                //PredictionRigid.transform.position = predictPosition;
                //Debug.Log(predictPosition);
            }
            catch { }







        }


        public override void SafeAwake()
        {
            gameObject.name = "missle";
            Launch = AddKey("Launch", "launch", KeyCode.X);
            //IFF = AddToggle("开启友伤", "IFF", true);
            //showScanner = AddToggle("显示探测范围", "showScanner", false);
            //detectAngleSlider = AddSlider("探测角度", "detection angle", 90.0f, 60.0f, 120.0f);
            detectDelay = AddSlider("Safty delay", "detection delay", 0.2f, 0.0f, 1f);
            launchDelay = AddSlider("Launch delay", "launch delay", 0.1f, 0.0f, 0.3f);
            PFRang = AddSlider("Proximity fuse range", "PF range", 5f, 1f, 10f);

            initScan();//挂载上导弹前方的圆锥触发器
            initTrail();
            initExplo();
            initPF();

            AimIcon = ModResource.GetTexture("Aim Icon").Texture;
            myPlayerID = BlockBehaviour.ParentMachine.PlayerID;
        }

        public void Start()
        {
            myTransform = gameObject.GetComponent<Transform>();        //获取相应对象的引用
            myRigidbody = gameObject.GetComponent<Rigidbody>();

            myRigidbody.drag = 0f;
            myRigidbody.angularDrag = 0f;


            Vector3 ScanColScale = new Vector3(550,550,550);
            ScanCollider.transform.localScale = ScanColScale;
            //ScannerDisplay.transform.localScale = ScanColScale;
            //Debug.Log(ScanColScale);
            //coneHit.IFF = IFF.IsActive;
            coneHit.team = BlockBehaviour.Team;
            coneHit.PlayerID = myPlayerID;

            //if (showScanner.IsActive)
            //{
            //    ScannerDisplay.SetActive(true);
            //}
            //else
            //{
            //    ScannerDisplay.SetActive(false);
            //}
            
            initParticleSystem();

            misslePF.radius = PFRang.Value;


        }

        public override void OnSimulateStart()
        {
            base.OnSimulateStart();
        }

        public override void OnSimulateStop()
        {
            base.OnSimulateStop();
        }

        protected void Update()
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

        //public override void SimulateFixedUpdateClient()
        //{
        //    if (Launch.EmulationHeld() && myStatus == status.stored)
        //    {
        //        myStatus = status.launched;
        //    }

        //    if (myStatus == status.launched)
        //    {

        //        if (time < 3.5f + launchDelay.Value)
        //        {
        //            if (time > launchDelay.Value)
        //            {
        //                if (activeTrail == false)
        //                {
        //                    TrailSmokeParticle.Play();
        //                    TrailFlameParticle.Play();
        //                    activeTrail = true;
        //                }

        //            }

        //            if (time < detectDelay.Value + launchDelay.Value)
        //            {
        //            }
        //            else
        //            {
        //                if (PFHit.explo == true)
        //                {
        //                    playExplo();
        //                    BlockBehaviour.gameObject.SetActive(false);
        //                }
        //            }
        //            time += Time.fixedDeltaTime;
        //        }
        //        else
        //        {
        //            if (activeTrail == true)
        //            {
        //                TrailSmokeParticle.Stop();
        //                TrailFlameParticle.Stop();
        //                activeTrail = false;
        //            }
        //            myStatus = status.missed;

        //        }
        //    }
        //    if (myStatus == status.missed)
        //    {
        //        targetDetected = false;

        //    }
        //    if (myStatus == status.missed || myStatus == status.exploded)
        //    {
        //        if (!effectDestroyed)
        //        {
        //            Destroy(TrailSmoke, 3);
        //            Destroy(TrailFlame, 3);
        //            Destroy(ExploFireball, 3);
        //            Destroy(ExploDust, 3);
        //            Destroy(ExploShower, 3);
        //            Destroy(ExploSmokeBlack, 3);
        //            effectDestroyed = true;
        //        }

        //        if (myStatus == status.exploded && !gameObjectDestroyed)
        //        {
        //            Destroy(BlockBehaviour.gameObject, 3.2f);
        //            gameObjectDestroyed = true;
        //        }
        //    }
        //}


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
                    
                    myRigidbody.angularDrag = 4.0f;
                    launchRotation = transform.rotation;
                    getlaunchRotation = true;
                    //Debug.Log(launchRotation);
                }

                if (time < 3.5f + launchDelay.Value)
                {
                    if (time > launchDelay.Value)
                    {
                        if(activeTrail == false)
                        {
                            myRigidbody.drag = 3f;
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
                        if (!PFCollider.activeSelf)
                        {
                            PFCollider.SetActive(true);
                        }
                        if (PFHit.explo == true)
                        {
                            playExplo();
                        }
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
            if(myStatus == status.missed || myStatus == status.exploded)
            {
                if (!effectDestroyed)
                {
                    Destroy(TrailSmoke, 3);
                    Destroy(TrailFlame, 3);
                    Destroy(ExploFireball, 3);
                    Destroy(ExploDust, 3);
                    Destroy(ExploShower, 3);
                    Destroy(ExploSmokeBlack, 3);
                    effectDestroyed = true;
                }
                
                if (myStatus == status.exploded && !gameObjectDestroyed)
                {
                    Destroy(BlockBehaviour.gameObject, 3.2f);
                    gameObjectDestroyed = true;
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
