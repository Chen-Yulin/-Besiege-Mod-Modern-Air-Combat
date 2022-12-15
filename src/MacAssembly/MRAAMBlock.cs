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
    public class MRAAMBlock : SRAAMBlock
    {

        public new float ExploPower = 14000f;
        public new float ExploRadius = 25f;

        protected bool PassiveGetAim()
        {
            
            try
            {
                Vector3 targetVelocity = DataManager.Instance.BVRData[myPlayerID].velocity;
                Vector3 targetPosition = DataManager.Instance.BVRData[myPlayerID].position;

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
                    modifiedDiff.x = (overshootMultiplier*0.3f * positionDiff.x);
                    modifiedDiff.y = (overshootMultiplier*0.3f * positionDiff.y);
                    modifiedDiff.z = (overshootMultiplier*0.3f * positionDiff.z);
                }

                predictPositionModified = predictPosition + modifiedDiff + Vector3.up * (targetPosition - transform.position).magnitude * 0.05f;
                return true;
            }
            catch { return false; }
        }

        public override void InitModelType()
        {
            modelType = AddMenu("Missile Type", 0, new List<string>()
            {
                "R-27",
                "R-77",
                "Aim-54",
                "Aim-120",
                "R-33"
            }, false);
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
            GValue = AddSlider("Maximum G-value", "Maximum G-value", 30f, 10f, 70f);
            thrust = AddSlider("Thrust", "Thrust", 2000, 1500, 2500);
            thrustTime = AddSlider("Thrust Duration","Thrust Duration", defaultValue: 5f, min: 3f, max: 20f);

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
                thrustModified = 2000f;
                thrustTimeModified = 5f;
                GModified = 30f;
            }
            else
            {
                thrustModified = thrust.Value;
                thrustTimeModified = thrustTime.Value;
                GModified = GValue.Value;
            }
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
            
            if (myStatus == status.launched || myStatus == status.active)
            {
                if (time < thrustTimeModified * 4 + launchDelay.Value)
                {
                    if (time > launchDelay.Value && time < thrustTimeModified + launchDelay.Value)//play trail partical and add trust after launch delay 
                    {
                        if (activeTrail == false)
                        {
                            TrailSmokeParticle.Play();
                            TrailFlameParticle.Play();
                            activeTrail = true;
                            GameObject LaunchSoundEffect = (GameObject)Instantiate(LaunchSound, transform, false);
                            LaunchSoundEffect.SetActive(true);
                            LaunchSoundEffect.GetComponent<AudioSource>().Play();
                            Destroy(LaunchSoundEffect, thrustTimeModified);
                        }
                    }
                    if (time > thrustTimeModified + launchDelay.Value)//deactive trail effect and destroy it after sometime
                    {
                        if (activeTrail == true)
                        {
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
                    }
                    if (MissleExploMessageReciver.Instance.GetExploMsg(myGuid, myPlayerID))
                    {
                        playExploEffect();
                    }
                    time += Time.fixedDeltaTime;
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
                myRigidbody.drag = 0.05f;
                myRigidbody.angularDrag = 4.0f;
            }

            if (myStatus == status.launched || myStatus == status.active)
            {
                if (DataManager.Instance.BVRData[myPlayerID].position == Vector3.zero)
                {
                    myStatus = status.active;
                }
                //get the launch rotation at the begining of launch
                if (!getlaunchRotation)
                {
                    myRigidbody.angularDrag = 4.0f;
                    launchRotation = transform.rotation;
                    getlaunchRotation = true;
                    //Debug.Log(launchRotation);
                }


                
                if (time < thrustTimeModified*4 + launchDelay.Value)
                {
                    if (time > launchDelay.Value && time < thrustTimeModified + launchDelay.Value)//play trail partical and add trust after launch delay 
                    {
                        if (activeTrail == false)
                        {
                            myRigidbody.drag = 0.5f;
                            TrailSmokeParticle.Play();
                            TrailFlameParticle.Play();
                            activeTrail = true;
                            GameObject LaunchSoundEffect = (GameObject)Instantiate(LaunchSound, transform, false);
                            LaunchSoundEffect.SetActive(true);
                            LaunchSoundEffect.GetComponent<AudioSource>().Play();
                            Destroy(LaunchSoundEffect, thrustTimeModified);
                        }
                        myRigidbody.AddRelativeForce(new Vector3(0, thrustModified, 0), ForceMode.Force);
                        AddAerodynamics(17,GModified);
                    }
                    if(time > thrustTimeModified+launchDelay.Value)//deactive trail effect and destroy it after sometime
                    {
                        if (activeTrail == true)
                        {
                            myRigidbody.drag = 0.03f;
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
                        AddAerodynamics(17,GModified);
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

                            if (PassiveGetAim())
                            {
                                AxisLookAt(myTransform, predictPositionModified, Vector3.up, 0.01f);
                            }
                            

                            if (Vector3.Distance(predictPositionModified,myTransform.position)<= 1200)
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
                            else
                            {
                                GetAim();
                                if (targetDetected)
                                {
                                    AxisLookAt(myTransform, predictPositionModified, Vector3.up, 0.03f);
                                }
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
        
        }
    }
}
