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
    public class MissleExploMessageReciver : SingleInstance<MissleExploMessageReciver>
    {
        public override string Name { get; } = "MissleExploMessageReciver";

        public Dictionary<int, Dictionary<int, bool>> MissleExploMsg = new Dictionary<int, Dictionary<int, bool>>();

        public void ReceiveMsg(Message msg)
        {
            int guid_msg = (int)msg.GetData(0);
            int playerid_msg = (int)msg.GetData(1);
            bool explo_msg = (bool)msg.GetData(2);

            if (MissleExploMsg.ContainsKey(guid_msg))
            {
                if (MissleExploMsg[guid_msg].ContainsKey(playerid_msg))
                {
                    MissleExploMsg[guid_msg][playerid_msg] = explo_msg;
                }
                else
                {
                    MissleExploMsg[guid_msg].Add(playerid_msg, explo_msg);
                }
            }
            else
            {
                Dictionary<int, bool> tmpDic = new Dictionary<int, bool>
                {
                    { playerid_msg, explo_msg }
                };
                MissleExploMsg.Add(guid_msg, tmpDic);

            }

        }

        public bool GetExploMsg(int guid, int playerID)
        {
            if (MissleExploMsg.ContainsKey(guid))
            {
                if (MissleExploMsg[guid].ContainsKey(playerID))
                {
                    return MissleExploMsg[guid][playerID];
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }

        }


    }

    public class ScanCollisonHit : MonoBehaviour
    {
        public MPTeam myTeam = MPTeam.None;
        public bool IFF = false;
        //public ushort PlayerID = 0;
        public Stack<Collider> targetCols = new Stack<Collider>();
        public bool isRadarScan = false;


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
                if (targetCols.Count > 5 || col.name == "flare")
                    return;

                //chaff has no rigidbody so put it here
                if (isRadarScan && col.name == "chaff")
                {
                    if (UnityEngine.Random.value > 0.4)
                    {
                        targetCols.Push(col);
                        return;
                    }

                }



                if (col.isTrigger || col.transform.parent.GetInstanceID() == col.GetInstanceID())
                    return;

                

                if (col.attachedRigidbody.gameObject.name == "missle")
                    return;
                BlockBehaviour hitedBlock = col.attachedRigidbody.gameObject.GetComponent<BlockBehaviour>();
                if (!hitedBlock.isSimulating)
                    return;
                if (IFF)
                {
                    hitedTeam = hitedBlock.Team;
                    //Debug.Log(hitedTeam.ToString()+" "+myTeam.ToString());
                    if (myTeam == hitedTeam)
                    {
                        return;
                    }
                }
                targetCols.Push(col);
            }
            catch 
            {
                return;
            }
            
        }


    }

    public class ScanCollisonHitFlare : MonoBehaviour
    {
        //public MPTeam team = MPTeam.None;
        //public bool IFF = true;
        //public ushort PlayerID = 0;
        public Stack<Collider> FlareCols = new Stack<Collider>();

        public void Reset()
        {
            FlareCols.Clear();
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
                if (FlareCols.Count > 1)
                {
                    return;
                }
                if (col.gameObject.name == "flare(Clone)")
                {
                    Debug.Log("find");
                    if (UnityEngine.Random.value > 0.5f)
                    {
                        FlareCols.Push(col);
                    }
                }
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
                if (explo == true)
                    return;
                try
                {
                    if (col.isTrigger || col.transform.parent.GetInstanceID() == col.GetInstanceID() || col.attachedRigidbody.gameObject.name == "missle")
                        return;
                }
                catch { }
                
                if (col.name == "flareCol")
                {
                    if (UnityEngine.Random.value > 0.5f)
                    {
                        explo = true;
                    }
                    else
                    {
                        return;
                    }
                }
                explo = true;
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
        public MMenu modelType;


        public enum status { stored, launched, active, missed, exploded };
        public status myStatus;
        public GameObject ScanCollider;
        public SphereCollider missleScan;
        public ScanCollisonHit coneHit;
        public GameObject ScanFlare;
        public ScanCollisonHitFlare flareHit;
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

        public GameObject LaunchSound;
        public AudioClip LaunchClip;
        public AudioSource LaunchAS;

        public GameObject ExploSound;
        public AudioClip ExploClip;
        public AudioSource ExploAS;


        public float ExploPower = 100000f;
        public float ExploRadius = 15f;

        public static MessageType MissleExplo = ModNetworking.CreateMessageType(DataType.Integer, DataType.Integer, DataType.Boolean);

        protected int myGuid;

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

        public ushort myPlayerID;
        public bool launchMsgInit = false;
        public int currModelType = 0;

        public bool currSkinStatus = false;


        public virtual void InitModelType()
        {
            modelType = AddMenu("Missile Type", 0, new List<string>
            {
                "R-73",
                "Aim-9"
            }, false);
        }


        public void AxisLookAt(Transform tr_self, Vector3 lookPos, Vector3 directionAxis, float speed)
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
            tr_self.rotation = Quaternion.Lerp(rotation, Quaternion.AngleAxis(angle, axis) * rotation,speed);

        }//from CSDN


        public void InitSoundEffect()
        {
            if (!transform.FindChild("Launch sound"))
            {
                LaunchClip = ModResource.GetAudioClip("MissileLaunch Audio");
                LaunchSound = new GameObject("Launch sound");
                LaunchSound.transform.SetParent(transform);
                LaunchAS = LaunchSound.GetComponent<AudioSource>() ?? LaunchSound.AddComponent<AudioSource>();
                LaunchSound.AddComponent<MakeAudioSourceFixedPitch>();
                LaunchAS.clip = LaunchClip;
                LaunchAS.spatialBlend = 1.0f;
                LaunchAS.volume = 60f;

                LaunchAS.SetSpatializerFloat(1, 1f);
                LaunchAS.SetSpatializerFloat(2, 0);
                LaunchAS.SetSpatializerFloat(3, 12);
                LaunchAS.SetSpatializerFloat(4, 1000f);
                LaunchAS.SetSpatializerFloat(5, 1f);
                LaunchSound.SetActive(false);

                ExploClip = ModResource.GetAudioClip("MissileExplo Audio");
                ExploSound = new GameObject("Explo sound");
                ExploSound.transform.SetParent(transform);
                ExploAS = ExploSound.GetComponent<AudioSource>() ?? ExploSound.AddComponent<AudioSource>();
                ExploSound.AddComponent<MakeAudioSourceFixedPitch>();
                ExploAS.clip = ExploClip;
                ExploAS.spatialBlend = 1.0f;
                ExploAS.volume = 0.1f;
                ExploAS.rolloffMode = AudioRolloffMode.Linear;
                ExploAS.maxDistance = 1000;
                ExploAS.SetSpatializerFloat(1, 1f);
                ExploAS.SetSpatializerFloat(2, 0);
                ExploAS.SetSpatializerFloat(3, 12);
                ExploAS.SetSpatializerFloat(4, 1000f);
                ExploAS.SetSpatializerFloat(5, 1f);
                ExploSound.SetActive(false);
            }
            
        }

        public void initScan()
        {
            if (BlockBehaviour.transform.FindChild("ScanCol") == null)
            {
                ScanCollider = new GameObject("ScanCol");
                ScanCollider.transform.SetParent(BlockBehaviour.transform);
                ScanCollider.transform.localPosition = new Vector3(0f, 650f, 0.3f);
                ScanCollider.transform.localRotation = Quaternion.Euler(90, 0, 0);
                ScanCollider.transform.localScale = Vector3.one;
                missleScan = ScanCollider.AddComponent<SphereCollider>();
                missleScan.radius = 1;
                missleScan.isTrigger = true;
                coneHit = ScanCollider.AddComponent<ScanCollisonHit>();

                ScanCollider.SetActive(false);
                coneHit.Reset();
            }
            if (BlockBehaviour.transform.FindChild("ScanFlare") == null)
            {
                ScanFlare = new GameObject("ScanFlare");
                ScanFlare.transform.SetParent(BlockBehaviour.transform);
                ScanFlare.transform.localPosition = new Vector3(0f, 25f, 0.3f);
                ScanFlare.transform.localRotation = Quaternion.Euler(90, 0, 0);
                ScanFlare.transform.localScale = new Vector3(35,35,35);
                SphereCollider ScanFlareCol = ScanFlare.AddComponent<SphereCollider>();
                ScanFlareCol.radius = 1;
                ScanFlareCol.isTrigger = true;
                flareHit = ScanFlare.AddComponent<ScanCollisonHitFlare>();

                ScanFlare.SetActive(false);
                flareHit.Reset();


            }
        }
        public void initPF()
        {
            if (BlockBehaviour.transform.FindChild("PFCol") == null)
            {
                PFCollider = new GameObject("PFCol");
                PFCollider.transform.SetParent(BlockBehaviour.transform);
                PFCollider.transform.localPosition = new Vector3(0f, 6f, 0.3f);
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


        protected void initParticleSystem()
        {
            TrailSmokeParticle = TrailSmoke.GetComponent<ParticleSystem>();
            TrailFlameParticle = TrailFlame.GetComponent<ParticleSystem>();
        }

        protected void playExploEffect()
        {
            try
            {
                TrailSmokeParticle.Stop();
                TrailFlameParticle.Stop();
            }
            catch { }

            GameObject ExploSoundEffect = (GameObject)Instantiate(ExploSound, transform, false);
            ExploSoundEffect.SetActive(true);
            ExploSoundEffect.GetComponent<AudioSource>().Play();
            Destroy(ExploSoundEffect, 3.5f);


            GameObject ExploParticleEffect = (GameObject)Instantiate(AssetManager.Instance.Explo.Explo, transform.position, transform.rotation);
            ExploParticleEffect.SetActive(true);
            Destroy(ExploParticleEffect, 3);
          

            BlockBehaviour.MeshRenderer.enabled = false;
            transform.FindChild("Colliders").gameObject.SetActive(false);
            myStatus = status.exploded;
        }

        protected void playExplo()
        {

            if (StatMaster.isMP)
            {
                Message missleExplo = MissleExplo.CreateMessage(myGuid, (int)myPlayerID, true);
                ModNetworking.SendToAll(missleExplo);
                //Debug.Log(myGuid.ToString());
            }

            playExploEffect();
            myRigidbody.constraints = RigidbodyConstraints.FreezePosition;

            Collider[] ExploCol = Physics.OverlapSphere(transform.position, ExploRadius);
            foreach (Collider hits in ExploCol)
            {
                if (hits.GetComponent<Rigidbody>())
                {
                    hits.GetComponent<Rigidbody>().AddExplosionForce(ExploPower, transform.position, 2*ExploRadius);
                    try
                    {
                        hits.attachedRigidbody.gameObject.GetComponent<FireTag>().Ignite();
                    }
                    catch { }
                    
                }
            }
            ScanCollider.SetActive(false);
            ScanFlare.SetActive(false);
            PFCollider.SetActive(false);
            
        }



        protected void GetAim()
        {
            try
            {
                ScanCollider.SetActive(true);
                ScanFlare.SetActive(true);
                

                detectFreqTime += Time.fixedDeltaTime;


                if (detectFreqTime >= 0.02)
                {
                    //judge whether there is a target
                    if (coneHit.targetCols.Count == 0 && flareHit.FlareCols.Count < 2)
                    {
                        targetDetected = false;
                        return;
                    }
                    else
                    {
                        targetDetected = true;
                    }
                    detectFreqTime = 0;


                    //judge whether use flare's collider or real target's collider
                    Collider targetCol;
                    if (UnityEngine.Random.value < 0.5f && flareHit.FlareCols.Count == 2)
                    {
                        targetCol = flareHit.FlareCols.Peek();
                        Debug.Log("distracted");
                    }
                    else
                    {
                        targetCol = coneHit.targetCols.Peek();
                    }
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
                    ScanFlare.SetActive(false);
                    coneHit.Reset();
                    flareHit.Reset();
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
            InitModelType();
            gameObject.name = "missle";
            Launch = AddKey("Launch", "launch", KeyCode.X);
            //IFF = AddToggle("开启友伤", "IFF", true);
            //showScanner = AddToggle("显示探测范围", "showScanner", false);
            //detectAngleSlider = AddSlider("探测角度", "detection angle", 90.0f, 60.0f, 120.0f);
            detectDelay = AddSlider("Safety delay", "detection delay", 0.2f, 0.0f, 1f);
            launchDelay = AddSlider("Launch delay", "launch delay", 0.1f, 0.0f, 0.3f);
            PFRang = AddSlider("Proximity fuse range", "PF range", 5f, 1f, 10f);

            initScan();//挂载上导弹前方的圆锥触发器
            initTrail();
            //initExplo();
            initPF();
            InitSoundEffect();

            AimIcon = ModResource.GetTexture("Aim Icon").Texture;
            myPlayerID = BlockBehaviour.ParentMachine.PlayerID;
        }

        public override void BuildingUpdate()
        {
            if (currModelType != modelType.Value || currSkinStatus != OptionsMaster.skinsEnabled)
            {
                BlockBehaviour.transform.FindChild("Vis").GetComponent<MeshFilter>().sharedMesh = ModResource.GetMesh(modelType.Selection + " Mesh").Mesh;
                BlockBehaviour.transform.FindChild("Vis").GetComponent<MeshRenderer>().material.SetTexture("_MainTex", ModResource.GetTexture(modelType.Selection + " Texture").Texture);
                currModelType = modelType.Value;
                currSkinStatus = OptionsMaster.skinsEnabled;
            }
        }

        public void Start()
        {
            BlockBehaviour.blockJoint.breakForce = -1;
            
            myTransform = gameObject.GetComponent<Transform>();
            myRigidbody = gameObject.GetComponent<Rigidbody>();

            myRigidbody.drag = 0f;
            myRigidbody.angularDrag = 0f;


            Vector3 ScanColScale = new Vector3(550,550,550);
            ScanCollider.transform.localScale = ScanColScale;
            
            initParticleSystem();

            misslePF.radius = PFRang.Value;


        }
        protected void Update()
        {
            if (currSkinStatus != OptionsMaster.skinsEnabled)
            {
                BlockBehaviour.transform.FindChild("Vis").GetComponent<MeshFilter>().sharedMesh = ModResource.GetMesh(modelType.Selection + " Mesh").Mesh;
                BlockBehaviour.transform.FindChild("Vis").GetComponent<MeshRenderer>().material.SetTexture("_MainTex", ModResource.GetTexture(modelType.Selection + " Texture").Texture);
                currModelType = modelType.Value;
                currSkinStatus = OptionsMaster.skinsEnabled;
            }
        }

        public override void OnSimulateStart()
        {
            myGuid = BlockBehaviour.BuildingBlock.Guid.GetHashCode();
            if (!StatMaster.isClient)
            {
                KeymsgController.Instance.keyheld[myPlayerID].Add(myGuid, false);
            }
            


            if (StatMaster.isMP)
            {
                Message missleExplo = MissleExplo.CreateMessage(BlockBehaviour.BuildingBlock.Guid.GetHashCode(), (int)BlockBehaviour.ParentMachine.PlayerID, false);
                ModNetworking.SendToAll(missleExplo);
            }
        }

        public override void OnSimulateStop()
        {
            KeymsgController.Instance.keyheld[myPlayerID].Remove(myGuid);
        }

        public override void SimulateUpdateHost()
        {
            
            try
            {
                if (IsSimulating)
                {
                    if (BlockBehaviour.BuildingBlock.Guid.GetHashCode() != 0 && BlockBehaviour.BuildingBlock.Guid.GetHashCode() != myGuid)
                        myGuid = BlockBehaviour.BuildingBlock.Guid.GetHashCode();

                    if (!launchMsgInit)
                    {
                        launchMsgInit = !launchMsgInit;
                        ModNetworking.SendToAll(KeymsgController.SendHeld.CreateMessage((int)myPlayerID, (int)myGuid, false));
                    }

                    if (Launch.IsHeld && myStatus == status.stored && !StatMaster.isClient)
                    {

                        if (!StatMaster.isClient)
                        {
                            ModNetworking.SendToAll(KeymsgController.SendHeld.CreateMessage((int)myPlayerID, (int)myGuid, true));
                        }

                        myStatus = status.launched;
                    }
                }
            }
            catch { }
            
            
        }

        public override void SimulateFixedUpdateClient()
        {
            try
            {
                if (KeymsgController.Instance.keyheld[myPlayerID][myGuid] && myStatus == status.stored)
                {
                    myStatus = status.launched;
                }
            }
            catch { }
            if (myStatus == status.launched)
            {

                if (time < 3.5f + launchDelay.Value)
                {
                    if (time > launchDelay.Value)
                    {
                        if (activeTrail == false)
                        {
                            TrailSmokeParticle.Play();
                            TrailFlameParticle.Play();
                            activeTrail = true;
                            GameObject LaunchSoundEffect = (GameObject)Instantiate(LaunchSound, transform, false);
                            LaunchSoundEffect.SetActive(true);
                            LaunchSoundEffect.GetComponent<AudioSource>().Play();
                            Destroy(LaunchSoundEffect, 3.5f);
                        }

                    }

                    if (time < detectDelay.Value + launchDelay.Value)
                    {
                    }
                    else
                    {
                        if (MissleExploMessageReciver.Instance.GetExploMsg(myGuid,myPlayerID))
                        {
                            //Debug.Log("ClientExplo");
                            playExploEffect();
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

                }
            }
            if (myStatus == status.missed)
            {
                targetDetected = false;

            }
            if (myStatus == status.missed || myStatus == status.exploded)
            {
                if (!effectDestroyed)
                {
                    Destroy(TrailSmoke, 3);
                    Destroy(TrailFlame, 3);
                    effectDestroyed = true;
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
                    myRigidbody.drag = 0.1f;
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

                            GameObject LaunchSoundEffect = (GameObject)Instantiate(LaunchSound, transform, false);
                            LaunchSoundEffect.SetActive(true);
                            LaunchSoundEffect.GetComponent<AudioSource>().Play();
                            Destroy(LaunchSoundEffect, 3.5f);

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
                            AxisLookAt(myTransform, predictPositionModified, Vector3.up, 0.08f);
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
                if (PFCollider.activeSelf)
                {
                    ScanCollider.SetActive(false);
                    ScanFlare.SetActive(false);
                    PFCollider.SetActive(false);
                }
            
                if (myRigidbody.position.y > 20)
                {
                    myTransform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(-180, 0, 0), 0.001f);
                }
            }
            if(myStatus == status.missed || myStatus == status.exploded)
            {
                if (!effectDestroyed)
                {
                    Destroy(TrailSmoke, 3);
                    Destroy(TrailFlame, 3);
                    effectDestroyed = true;
                }
            }
        }

        void OnGUI()
        {

        }
        

    }
}
