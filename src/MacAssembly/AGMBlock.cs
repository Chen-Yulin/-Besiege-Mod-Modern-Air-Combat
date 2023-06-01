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
    public class AGMBlock : MRAAMBlock
    {
        public new float ExploPower = 14000f;

        public new float ExploRadius = 35f;
        public new float ActiveDistance = 10f;
        public override void InitModelType()
        {
            modelType = AddMenu("Missile Type", 0, new List<string>
            {
                "AGM"
            }, false);
        }
        public override void UpdateLoadInfo()// call in SimulateStart
        {
            LoadDataManager.Instance.AddLoad(myPlayerID, myGuid, LoadDataManager.WeaponType.AGM, transform);
        }
        public bool A2GGetAim()
        {
            //Debug.Log(DataManager.Instance.A2G_TargetData[myPlayerID].position);
            try
            {
                Vector3 targetVelocity = DataManager.Instance.A2G_TargetData[myPlayerID].velocity;
                Vector3 targetPosition = DataManager.Instance.A2G_TargetData[myPlayerID].position;

                if (targetPosition == Vector3.zero)
                {
                    return false;
                }

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

                Vector3 positionDiff = predictPosition - (transform.position + Rigidbody.velocity * estimatedTime);
                //Debug.Log(positionDiff);
                Vector3 modifiedDiff;
                float overshootMultiplier = 50 / GValue.Value;
                if (positionDiff.magnitude < 200)
                {
                    modifiedDiff.x = (overshootMultiplier * positionDiff.x);
                    modifiedDiff.y = (overshootMultiplier * positionDiff.y);
                    modifiedDiff.z = (overshootMultiplier * positionDiff.z);
                }
                else
                {
                    modifiedDiff.x = (overshootMultiplier * 0.3f * positionDiff.x);
                    modifiedDiff.y = (overshootMultiplier * 0.3f * positionDiff.y);
                    modifiedDiff.z = (overshootMultiplier * 0.3f * positionDiff.z);
                }

                predictPositionModified = predictPosition + modifiedDiff + Vector3.up * (targetPosition - transform.position).magnitude * 0.05f;
                return true;
            }
            catch { return false; }
        }
        protected override void playExploEffect()
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
            Destroy(ExploSoundEffect, thrustTimeModified);


            GameObject ExploParticleEffect = (GameObject)Instantiate(AssetManager.Instance.AGMExplo.AGMExplo, transform.position, Quaternion.identity);
            ExploParticleEffect.SetActive(true);
            Destroy(ExploParticleEffect, 3);


            BlockBehaviour.MeshRenderer.enabled = false;
            transform.FindChild("Colliders").gameObject.SetActive(false);
            myStatus = status.exploded;
        }
        public override void SafeAwake()
        {
            InitModelType();
            gameObject.name = "AGM";
            Launch = AddKey("Launch", "launch", KeyCode.X);
            //IFF = AddToggle("开启友伤", "IFF", true);
            //showScanner = AddToggle("显示探测范围", "showScanner", false);
            //detectAngleSlider = AddSlider("探测角度", "detection angle", 90.0f, 60.0f, 120.0f);
            detectDelay = AddSlider("Safety delay", "detection delay", 0.4f, 0.1f, 1.5f);
            launchDelay = AddSlider("Launch delay", "launch delay", 0.3f, 0.1f, 0.5f);
            PFRang = AddSlider("Proximity fuse range", "PF range", 2f, 0.1f, 10f);
            GValue = AddSlider("Maximum G-value", "Maximum G-value", 10f, 5f, 30f);
            thrust = AddSlider("Thrust", "Thrust", 400, 300, 600);
            thrustTime = AddSlider("Thrust Duration", "Thrust Duration", defaultValue: 8f, min: 5f, max: 20f);
            BreakThrust = AddSlider("BreakThrust", "BreakThrust", 0f, 0f, 10000f);

            initScan();//挂载上导弹前方的圆锥触发器
            initTrail();
            //initExplo();
            initPF();
            InitSoundEffect();

            AimIcon = ModResource.GetTexture("Aim Icon").Texture;
            myPlayerID = BlockBehaviour.ParentMachine.PlayerID;
        }
        protected new void Update()
        {
            if (currSkinStatus != OptionsMaster.skinsEnabled)
            {
                BlockBehaviour.transform.FindChild("Vis").GetComponent<MeshFilter>().sharedMesh = ModResource.GetMesh(modelType.Selection + " Mesh").Mesh;
                BlockBehaviour.transform.FindChild("Vis").GetComponent<MeshRenderer>().material.SetTexture("_MainTex", ModResource.GetTexture(modelType.Selection + " Texture").Texture);
                currModelType = modelType.Value;
                currSkinStatus = OptionsMaster.skinsEnabled;
            }
            if (ModController.Instance.Restriction)
            {
                thrustModified = 400f;
                thrustTimeModified = 8f;
                GModified = 10f;
            }
            else
            {
                thrustModified = thrust.Value;
                thrustTimeModified = thrustTime.Value;
                GModified = GValue.Value;
            }
        }

        public override void SimulateFixedUpdateHost()
        {

            if (Launch.EmulationHeld() && myStatus == status.stored)
            {
                if (StatMaster.isMP)
                {
                    ModNetworking.SendToAll(KeymsgController.SendHeld.CreateMessage((int)myPlayerID, (int)myGuid, true));
                }
                myStatus = status.launched;
                //Debug.Log("AGM launched");
                //Debug.Log(detectRange);
                myRigidbody.drag = 0.05f * dragPercent;
                myRigidbody.angularDrag = 4.0f;
            }

            if (myStatus == status.launched || myStatus == status.active)
            {
                if (DataManager.Instance.A2G_TargetData[myPlayerID].position == Vector3.zero)
                {
                    myStatus = status.active;
                }
                if (_offRack)
                {
                    _offRack = false;
                    myRigidbody.AddForce(BreakThrust.Value * transform.forward, ForceMode.Force);

                }
                //get the launch rotation at the begining of launch
                if (!getlaunchRotation)
                {
                    myRigidbody.angularDrag = 4.0f;
                    launchRotation = transform.rotation;
                    getlaunchRotation = true;
                    //Debug.Log(launchRotation);

                    // pop missile up on released
                    foreach (ConfigurableJoint joint in GetComponent<BlockBehaviour>().jointsToMe)
                    {
                        joint.breakForce = 0f;
                    }
                    _offRack = true;
                }



                if (time < thrustTimeModified * 4 + launchDelay.Value)
                {
                    if (time > launchDelay.Value && time < thrustTimeModified + launchDelay.Value)//play trail partical and add trust after launch delay 
                    {
                        if (activeTrail == false)
                        {
                            myRigidbody.drag = 0.05f * dragPercent;
                            TrailSmokeParticle.Play();
                            TrailFlameParticle.Play();
                            activeTrail = true;
                            GameObject LaunchSoundEffect = (GameObject)Instantiate(LaunchSound, transform, false);
                            LaunchSoundEffect.SetActive(true);
                            LaunchSoundEffect.GetComponent<AudioSource>().Play();
                            Destroy(LaunchSoundEffect, thrustTimeModified);
                        }
                        myRigidbody.AddRelativeForce(new Vector3(0, thrustModified/2f, 0), ForceMode.Force);
                        AddAerodynamics(17 * dragPercent, GModified);
                    }
                    if (time > thrustTimeModified + launchDelay.Value)//deactive trail effect and destroy it after sometime
                    {
                        if (activeTrail == true)
                        {
                            myRigidbody.drag = 0.05f * dragPercent;
                            TrailSmokeParticle.Stop();
                            TrailFlameParticle.Stop();
                            activeTrail = false;

                        }
                        if (!effectDestroyed)
                        {
                            Destroy(TrailSmoke, 3);
                            Destroy(TrailFlame, 3);
                            effectDestroyed = true;
                        }
                        AddAerodynamics(17 * dragPercent, GModified);
                    }

                    //judge whether the missle start to track enemy (passive or active) and active PF
                    if (time < detectDelay.Value + launchDelay.Value) //whether is frozen
                    {
                        myTransform.rotation = Quaternion.Lerp(transform.rotation, launchRotation, 0.1f);
                    }
                    else //start tracking target
                    {
                        if (myStatus == status.launched)
                        {

                            // judge whether explode
                            if (!PFCollider.activeSelf)
                            {
                                PFCollider.SetActive(true);
                            }
                            if (PFHit.explo == true)
                            {
                                playExplo();
                            }

                            //start passvie track

                            if (A2GGetAim())
                            {
                                AxisLookAt(myTransform, predictPositionModified, Vector3.up, CalculateTurningRate());
                            }


                            if (Vector3.Distance(predictPositionModified, myTransform.position) <= ActiveDistance)
                            {
                                myStatus = status.active;
                            }


                        }
                        else    //start active track
                        {
                            if (!PFCollider.activeSelf)
                            {
                                PFCollider.SetActive(true);
                            }
                            if (PFHit.explo == true)
                            {
                                playExplo();
                            }

                        }
                    }
                    time += Time.fixedDeltaTime;
                }
                else
                {
                    if (PFCollider.activeSelf)
                    {
                        gameObject.SetActive(false);
                        PFCollider.SetActive(false);
                    }

                }
            }
        }
        void OnGUI()
        {
            //GUI.Box(new Rect(100, 200, 200, 50), DataManager.Instance.A2G_TargetData[myPlayerID].position.ToString());
            //GUI.Box(new Rect(100, 300, 200, 50), myStatus.ToString());
        }

    }
}
