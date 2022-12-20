using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Collections;

using Modding.Modules;
using Modding;
using Modding.Blocks;
using UnityEngine;
using UnityEngine.Networking;

namespace ModernAirCombat
{
    class MachineGunMsgReceiver : SingleInstance<MachineGunMsgReceiver>
    {
        public override string Name { get; } = "MachineGunMsgReceiver";

        public Dictionary<int, Vector3>[] GunVelocity = new Dictionary<int, Vector3>[16];
        public Dictionary<int, Vector3>[] BulletForward = new Dictionary<int, Vector3>[16];

        public Stack<Vector3>[] BulletExplo = new Stack<Vector3>[16];
        
        public MachineGunMsgReceiver()
        {
            for (int i = 0; i < 16; i++)
            {
                GunVelocity[i] = new Dictionary<int, Vector3>();
                BulletForward[i] = new Dictionary<int, Vector3>();
                BulletExplo[i] = new Stack<Vector3>();
            }
        }
        public void FireParaReceiver(Message msg)
        {
            GunVelocity[(int)msg.GetData(0)][(int)msg.GetData(1)] = (Vector3)msg.GetData(2);
            BulletForward[(int)msg.GetData(0)][(int)msg.GetData(1)] = (Vector3)msg.GetData(3);
        }

        public void BulletExploMsgReceiver(Message msg)
        {
            for (int i = 0; i < 16; i++)
            {
                BulletExplo[i].Push((Vector3)msg.GetData(0));
            }
        }
        
        

    }


    public class MachineGunBlock : BlockScript
    {
        public MMenu modelType;
        public MKey FireKey;
        public MToggle EnableSmoke;
        public MSlider Caliber;
        public MSlider InitialSpeed;
        public MSlider AmountOfBullet;
        public MSlider FiringRate;
        public MColourSlider bulletColor;

        public GameObject Bullet;
        public Queue<GameObject> bulletAssembly = new Queue<GameObject> ();

        public GameObject ShotSound;
        public AudioClip ShotClip;
        public AudioSource ShotAS;

        public GameObject FlyingSound;
        public AudioClip FlyingClip;
        public AudioSource FlyingAS;

        public GameObject HitSound;
        public AudioClip HitClip;
        public AudioSource HitAS;

        public int bulletLimit = 100;

        public static MessageType ClientFirePara = ModNetworking.CreateMessageType(DataType.Integer, DataType.Integer, DataType.Vector3, DataType.Vector3);//playerID.guid,velocity,rotation
        public static MessageType ClientBulletExploMsg = ModNetworking.CreateMessageType(DataType.Vector3);

        public GameObject GunFireEffect;

        public IEnumerator FireIE;

        public int myPlayerID;
        public int myGuid;
        public Rigidbody myRigidbody;

        public bool ClientKey = false;

        public int BulletsLeft = -11;

        public int BulletToBeFired = 0;

        public float TimeSinceStartUp = 0;

        public int currModelType = 0;
        public bool currSkinStatus = false;

        public void InitHitSound()
        {
            if (!transform.FindChild("Hit sound"))
            {
                HitClip = ModResource.GetAudioClip("BulletHit Audio");
                HitSound = new GameObject("Hit sound");
                HitSound.transform.SetParent(transform);
                HitAS = HitSound.GetComponent<AudioSource>() ?? HitSound.AddComponent<AudioSource>();
                HitSound.AddComponent<MakeAudioSourceFixedPitch>();
                HitAS.clip = HitClip;
                HitAS.spatialBlend = 1.0f;
                HitAS.volume = 0.02f * Caliber.Value;
                HitAS.SetSpatializerFloat(1, 1f);
                HitAS.SetSpatializerFloat(2, 0);
                HitAS.SetSpatializerFloat(3, 12);
                HitAS.SetSpatializerFloat(4, 1000f);
                HitAS.SetSpatializerFloat(5, 1f);
                HitSound.SetActive(false);
            }

        }
        public void InitShotSound()
        {
            if (!transform.FindChild("shot sound"))
            {
                ShotClip = ModResource.GetAudioClip("MachineGun Audio");
                ShotSound = new GameObject("shot sound");
                ShotSound.transform.SetParent(transform);
                ShotAS = ShotSound.GetComponent<AudioSource>() ?? ShotSound.AddComponent<AudioSource>();
                ShotSound.AddComponent<MakeAudioSourceFixedPitch>();
                ShotAS.clip = ShotClip;
                ShotAS.spatialBlend = 1.0f;
                ShotAS.volume = 0.02f * Caliber.Value;
                ShotAS.SetSpatializerFloat(1, 1f);
                ShotAS.SetSpatializerFloat(2, 0);
                ShotAS.SetSpatializerFloat(3, 12);
                ShotAS.SetSpatializerFloat(4, 1000f);
                ShotAS.SetSpatializerFloat(5, 1f);
                ShotSound.SetActive(false);
            }
        }

        public void InitFlyingSound()
        {
            if (!transform.FindChild("Flying sound"))
            {
                FlyingClip = ModResource.GetAudioClip("Flying Audio");
                FlyingSound = new GameObject("Flying sound");
                FlyingSound.transform.SetParent(transform);
                FlyingAS = FlyingSound.GetComponent<AudioSource>() ?? FlyingSound.AddComponent<AudioSource>();
                FlyingSound.AddComponent<MakeAudioSourceFixedPitch>();
                FlyingAS.clip = FlyingClip;
                FlyingAS.loop = true;
                FlyingAS.spatialBlend = 1.0f;
                FlyingAS.volume = 5f * Caliber.Value;
                FlyingAS.SetSpatializerFloat(1, 1f);
                FlyingAS.SetSpatializerFloat(2, 0);
                FlyingAS.SetSpatializerFloat(3, 12);
                FlyingAS.SetSpatializerFloat(4, 1000f);
                FlyingAS.SetSpatializerFloat(5, 1f);
                FlyingSound.SetActive(false);
            }
        }

        public void BulletExplo()
        {
            foreach (GameObject bullet in bulletAssembly)
            {
                if (bullet.activeSelf)
                {
                    Ray bulletRay = new Ray(bullet.transform.position + bullet.transform.forward * 15, bullet.transform.forward * 13);
                    RaycastHit hit;
                    if (Physics.Raycast(bulletRay, out hit, 10))
                    {
                        try
                        {
                            hit.collider.gameObject.transform.parent.parent.gameObject.GetComponent<BreakOnForce>().Break();
                        }
                        catch
                        {
                        }
                        if (hit.collider.isTrigger){
                            return;
                        }
                        //send explo position
                        ModNetworking.SendToAll(ClientBulletExploMsg.CreateMessage(hit.point));

                        GameObject explo = (GameObject)Instantiate(AssetManager.Instance.BulletExplo.BulletExplo, hit.point, Quaternion.identity);
                        explo.transform.localScale = new Vector3(Caliber.Value/20,Caliber.Value/20,Caliber.Value/20);
                        explo.SetActive(true);
                        Destroy(explo, 1);
                        bullet.SetActive(false);
                        try
                        {
                            hit.collider.attachedRigidbody.AddForce(bullet.transform.forward * 2000f * Caliber.Value, ForceMode.Force);
                            hit.collider.attachedRigidbody.AddTorque(bullet.transform.up * 500000f * Caliber.Value, ForceMode.Force);
                            //if (UnityEngine.Random.value>0.95)
                            //{
                            //    hit.collider.attachedRigidbody.gameObject.GetComponent<BlockBehaviour>().fireTag.Ignite();
                            //}
                            
                        }
                        catch { }
                        if (Caliber.Value > 20)
                        {
                            Collider[] ExploCol = Physics.OverlapBox(hit.point, new Vector3(3,3,3));
                            foreach (Collider hits in ExploCol)
                            {
                                if (hits.GetComponent<Rigidbody>())
                                {
                                    hits.GetComponent<Rigidbody>().AddExplosionForce(7000f*(Caliber.Value-20), hit.point, 20f);
                                }
                            }
                        }

                        //play sound
                        GameObject HitSoundEffect = (GameObject)Instantiate(HitSound, hit.point,Quaternion.identity);
                        HitSoundEffect.SetActive(true);
                        HitSoundEffect.GetComponent<AudioSource>().Play();
                        Destroy(HitSoundEffect, 1);

                    }
                }
                
            }
        }

        public void BulletExploClient()
        {
            while (MachineGunMsgReceiver.Instance.BulletExplo[myPlayerID].Count>0)
            {
                Vector3 point = MachineGunMsgReceiver.Instance.BulletExplo[myPlayerID].Pop();
                GameObject explo = (GameObject)Instantiate(AssetManager.Instance.BulletExplo.BulletExplo, point, Quaternion.identity);
                explo.SetActive(true);
                Destroy(explo, 1);
                GameObject HitSoundEffect = (GameObject)Instantiate(HitSound, point, Quaternion.identity);
                HitSoundEffect.SetActive(true);
                HitSoundEffect.GetComponent<AudioSource>().Play();
                Destroy(HitSoundEffect, 1);
            }
        }

        public void InitGunFire()
        {
            if (!transform.FindChild("GunFire"))
            {
                GunFireEffect = (GameObject)Instantiate(AssetManager.Instance.GunFire.GunFire,transform);
                GunFireEffect.transform.localPosition = new Vector3(0, 0, 1);
                GunFireEffect.transform.localRotation = Quaternion.Euler(0, 0, 0);
                GunFireEffect.transform.localScale = new Vector3(1, 1, 1);
                GunFireEffect.SetActive(true);
            }
        }

        public void InitBullet()
        {
            Bullet = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(Bullet.GetComponent<MeshFilter>());
            Destroy(Bullet.GetComponent<BoxCollider>());
            TrailRenderer trailRenderer = Bullet.GetComponent<TrailRenderer>() ?? Bullet.AddComponent<TrailRenderer>();
            trailRenderer.autodestruct = false;
            trailRenderer.receiveShadows = false;
            trailRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            trailRenderer.startWidth = 0.01f*Caliber.Value;
            trailRenderer.endWidth = 0.0f;

            trailRenderer.material = new Material(Shader.Find("Particles/Additive"));
            trailRenderer.material.SetColor("_TintColor", bulletColor.Value);

            trailRenderer.enabled = true;
            trailRenderer.time = 0.07f;

            Rigidbody rig = Bullet.GetComponent<Rigidbody>() ?? Bullet.AddComponent<Rigidbody>();
            rig.mass = 0.01f;
            rig.drag = 0.02f;

            Bullet.SetActive(false);

        }

        IEnumerator Fire(float delTime)
        {
            
            while (true)
            {
                yield return new WaitForSeconds(delTime);
                if (TimeSinceStartUp == 0)
                {
                    TimeSinceStartUp = Time.realtimeSinceStartup;
                    BulletToBeFired++;
                }
                else
                {
                    float CoroutineDeltaTime = Time.realtimeSinceStartup - TimeSinceStartUp;
                    
                    CoroutineDeltaTime = CoroutineDeltaTime * Time.timeScale;
                    BulletToBeFired += (int)Math.Floor(CoroutineDeltaTime / delTime);

                    TimeSinceStartUp += (BulletToBeFired * delTime) / Time.timeScale;
                }
                
                
                
            }
        }

        public void Release()
        {
            BulletsLeft--;
            if (bulletAssembly.Count >= 150)
            {
                Destroy(bulletAssembly.Dequeue());
            }
            else
            {
                GameObject bullet = (GameObject)Instantiate(Bullet, transform.position - 3 * transform.forward, transform.rotation);
                bullet.SetActive(true);
                if (!StatMaster.isClient)
                {
                    bullet.GetComponent<Rigidbody>().velocity = InitialSpeed.Value * transform.forward + myRigidbody.velocity
                                                            + 4 * UnityEngine.Random.value * transform.right - 4 * transform.right
                                                            + 4 * UnityEngine.Random.value * transform.up - 4 * transform.up;
                }
                else
                {
                    bullet.GetComponent<Rigidbody>().velocity = InitialSpeed.Value * MachineGunMsgReceiver.Instance.BulletForward[myPlayerID][myGuid]
                                                            + MachineGunMsgReceiver.Instance.GunVelocity[myPlayerID][myGuid]
                                                            + 4 * UnityEngine.Random.value * transform.right - 4 * transform.right
                                                            + 4 * UnityEngine.Random.value * transform.up - 4 * transform.up;
                }

                GameObject FlyingSoundEffect = (GameObject)Instantiate(FlyingSound, bullet.transform, false);
                FlyingSoundEffect.SetActive(true);
                FlyingSoundEffect.GetComponent<AudioSource>().Play();
                Destroy(FlyingSoundEffect, 3);
                
                bulletAssembly.Enqueue(bullet);
            }
        }

        public override void SafeAwake()
        {
            myPlayerID = BlockBehaviour.ParentMachine.PlayerID;
            

            FireKey = AddKey("Fire", "Fire", KeyCode.C);
            EnableSmoke = AddToggle("Smoke Toggle", "Smoke Toggle", true);
            Caliber = AddSlider("Caliber (mm)", "Caliber", 20f, 7.7f, 37f);
            InitialSpeed = AddSlider("Initial Speed (m/s)", "Initial Speed", 800f, 400f, 1000f);
            FiringRate = AddSlider("Firing Rate (/s)", "Firing Rate", 80f, 1f, 100f);
            AmountOfBullet = AddSlider("Amount of Bullets", "Amount of Bullets", 300f, 0f, 10000);
            bulletColor = AddColourSlider("Bullet tracer color", "Bullet tracer color", Color.yellow, false);
            modelType = AddMenu("Machine Gun Type", 0, new List<string>
            {
                "M61",
                "MK103",
                "GSH301",
                "ADEN 30mm"
            }, false);




        }

        public void Start()
        {
            myRigidbody = GetComponent<Rigidbody>();
            BulletsLeft = (int)Math.Floor(AmountOfBullet.Value);
            InitBullet();
            InitGunFire();
            InitShotSound();
            InitFlyingSound();
            InitHitSound();
        }

        public override void BuildingUpdate()
        {
            BlockBehaviour.transform.FindChild("Vis").GetComponent<MeshFilter>().sharedMesh = ModResource.GetMesh(modelType.Selection + " Mesh").Mesh;
            BlockBehaviour.transform.FindChild("Vis").GetComponent<MeshRenderer>().material.SetTexture("_MainTex", ModResource.GetTexture(modelType.Selection + " Texture").Texture);
            currModelType = modelType.Value;
            currSkinStatus = OptionsMaster.skinsEnabled;
        }

        public override void OnSimulateStart()
        {
            myGuid = BlockBehaviour.BuildingBlock.Guid.GetHashCode();
            FireIE = Fire(1f / FiringRate.Value);
            try
            {
                if (StatMaster.isMP)
                {
                    KeymsgController.Instance.keyheld[myPlayerID].Add(myGuid, false);
                    MachineGunMsgReceiver.Instance.GunVelocity[myPlayerID].Add(myGuid, Vector3.zero);
                    MachineGunMsgReceiver.Instance.BulletForward[myPlayerID].Add(myGuid, Vector3.up);
                }
            }
            catch { }
        }

        public override void OnSimulateStop()
        {
            try
            {
                if (GunFireEffect.transform.GetChild(1).GetComponent<ParticleSystem>().isPlaying)
                {
                    if (EnableSmoke.isDefaultValue)
                    {
                        GunFireEffect.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
                    }
                    GunFireEffect.transform.GetChild(1).GetComponent<ParticleSystem>().Stop();
                    GunFireEffect.transform.GetChild(2).GetComponent<ParticleSystem>().Stop();
                }
            
                StopCoroutine(FireIE);
            }
            catch { }
            if (!StatMaster.isClient && StatMaster.isMP)
            {
                KeymsgController.Instance.keyheld[myPlayerID].Remove(myGuid);
                MachineGunMsgReceiver.Instance.GunVelocity[myPlayerID].Remove(myGuid);
                MachineGunMsgReceiver.Instance.BulletForward[myPlayerID].Remove(myGuid);
            }
            if (StatMaster.isMP)
            {
                ModNetworking.SendToAll(KeymsgController.SendHeld.CreateMessage((int)myPlayerID, (int)myGuid, false));
            }

            while (bulletAssembly.Count > 0)
            {
                Destroy(bulletAssembly.Dequeue());
            }
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
        public override void SimulateUpdateHost()
        {
            if (BulletsLeft<=0)
            {
                if (GunFireEffect.transform.GetChild(1).GetComponent<ParticleSystem>().isPlaying)
                {
                    if (EnableSmoke.isDefaultValue)
                    {
                        GunFireEffect.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
                    }
                    GunFireEffect.transform.GetChild(1).GetComponent<ParticleSystem>().Stop();
                    GunFireEffect.transform.GetChild(2).GetComponent<ParticleSystem>().Stop();
                }
                try
                {
                    StopCoroutine(FireIE);
                }
                catch { }
                
                return;
            }
            //networking 
            ModNetworking.SendToAll(ClientFirePara.CreateMessage(myPlayerID, myGuid, myRigidbody.velocity, transform.forward));

            if (FireKey.IsPressed)
            {
                TimeSinceStartUp = 0;
                BulletToBeFired = 0;
                StartCoroutine(FireIE);
                if (EnableSmoke.isDefaultValue)
                {
                    GunFireEffect.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                }
                
                GunFireEffect.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
                GunFireEffect.transform.GetChild(2).GetComponent<ParticleSystem>().Play();
                if (StatMaster.isMP)
                {
                    ModNetworking.SendToAll(KeymsgController.SendHeld.CreateMessage((int)myPlayerID, (int)myGuid, true));
                }

            }
            if (FireKey.IsReleased)
            {
                StopCoroutine(FireIE);
                if (EnableSmoke.isDefaultValue)
                {
                    GunFireEffect.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
                }
                GunFireEffect.transform.GetChild(1).GetComponent<ParticleSystem>().Stop();
                GunFireEffect.transform.GetChild(2).GetComponent<ParticleSystem>().Stop();
                if (StatMaster.isMP)
                {
                    ModNetworking.SendToAll(KeymsgController.SendHeld.CreateMessage((int)myPlayerID, (int)myGuid, false));
                }
            }
        }

        public override void SimulateUpdateClient()
        {
            if (BulletsLeft == -11)
            {
                BulletsLeft = (int)Math.Floor(AmountOfBullet.Value);
            }
            if (BulletsLeft <= 0)
            {
                
                if (GunFireEffect.transform.GetChild(1).GetComponent<ParticleSystem>().isPlaying)
                {
                    if (EnableSmoke.isDefaultValue)
                    {
                        GunFireEffect.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
                    }
                    GunFireEffect.transform.GetChild(1).GetComponent<ParticleSystem>().Stop();
                    GunFireEffect.transform.GetChild(2).GetComponent<ParticleSystem>().Stop();
                }
                try
                {
                    StopCoroutine(FireIE);
                }
                catch { }
                return;

            }
            if (!ClientKey && KeymsgController.Instance.keyheld[myPlayerID][myGuid])
            {
                TimeSinceStartUp = 0;
                ClientKey = true;
                StartCoroutine(FireIE);
                if (EnableSmoke.isDefaultValue)
                {
                    GunFireEffect.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                }
                GunFireEffect.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
                GunFireEffect.transform.GetChild(2).GetComponent<ParticleSystem>().Play();
            }
            if (ClientKey && !KeymsgController.Instance.keyheld[myPlayerID][myGuid])
            {
                ClientKey = false;
                StopCoroutine(FireIE);
                if (EnableSmoke.isDefaultValue)
                {
                    GunFireEffect.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
                }
                GunFireEffect.transform.GetChild(1).GetComponent<ParticleSystem>().Stop();
                GunFireEffect.transform.GetChild(2).GetComponent<ParticleSystem>().Stop();
            }
        }

        public override void SimulateFixedUpdateHost()
        {
            if (BulletToBeFired>0)
            {
                for (int i = 0; i < BulletToBeFired; i++)
                {
                    Release();
                }

                if (!StatMaster.isClient)
                {
                    Rigidbody.AddForce(-Caliber.Value * InitialSpeed.Value * transform.forward * 0.01f, ForceMode.Force);
                }

                GameObject ShotSoundEffect = (GameObject)Instantiate(ShotSound, transform, false);
                ShotSoundEffect.SetActive(true);
                ShotSoundEffect.GetComponent<AudioSource>().Play();
                Destroy(ShotSoundEffect, 1);
            }
            
            BulletToBeFired = 0;

            BulletExplo();
        }

        public override void SimulateFixedUpdateClient()
        {
            for (int i = 0; i < BulletToBeFired; i++)
            {
                Release();

                GameObject ShotSoundEffect = (GameObject)Instantiate(ShotSound, transform, false);
                ShotSoundEffect.SetActive(true);
                ShotSoundEffect.GetComponent<AudioSource>().Play();
                Destroy(ShotSoundEffect, 1);
            }
            BulletToBeFired = 0;
            BulletExploClient();
        }
    }
}
