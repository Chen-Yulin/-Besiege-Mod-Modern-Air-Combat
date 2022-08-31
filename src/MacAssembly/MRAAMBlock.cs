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
    public class MRAAMBlock : SRAAMBlock
    {

        
        protected float thrustTime = 3f;
        protected void AddAerodynamics()
        {
            Vector3 tmp = Vector3.Cross(Vector3.Cross(myRigidbody.velocity, myTransform.up), myTransform.up);
            myRigidbody.AddForce(new Vector3(tmp.x,tmp.y,tmp.z)*17,ForceMode.Force);
        }
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

                predictPositionModified = predictPosition + modifiedDiff + Vector3.up * (targetPosition - transform.position).magnitude * 0.05f;
                return true;
            }
            catch { return false; }
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
                if (time < thrustTime * 4 + launchDelay.Value)
                {
                    if (time > launchDelay.Value && time < thrustTime + launchDelay.Value)//play trail partical and add trust after launch delay 
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
                    if (time > thrustTime + launchDelay.Value)//deactive trail effect and destroy it after sometime
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


                
                if (time < thrustTime*4 + launchDelay.Value)
                {
                    if (time > launchDelay.Value && time < thrustTime + launchDelay.Value)//play trail partical and add trust after launch delay 
                    {
                        if (activeTrail == false)
                        {
                            myRigidbody.drag = 1f;
                            TrailSmokeParticle.Play();
                            TrailFlameParticle.Play();
                            activeTrail = true;
                            GameObject LaunchSoundEffect = (GameObject)Instantiate(LaunchSound, transform, false);
                            LaunchSoundEffect.SetActive(true);
                            LaunchSoundEffect.GetComponent<AudioSource>().Play();
                            Destroy(LaunchSoundEffect, 3.5f);
                        }
                        myRigidbody.AddRelativeForce(new Vector3(0, 4200, 0), ForceMode.Force);
                        AddAerodynamics();
                    }
                    if(time > thrustTime+launchDelay.Value)//deactive trail effect and destroy it after sometime
                    {
                        if (activeTrail == true)
                        {
                            myRigidbody.drag = 0.05f;
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
                        AddAerodynamics();
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
                                AxisLookAt(myTransform, predictPositionModified, Vector3.up, 0.02f);
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
                                    AxisLookAt(myTransform, predictPositionModified, Vector3.up, 0.04f);
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
            //if (myStatus == status.launched)
            //{
            //GUI.Box(new Rect(100, 300, 200, 50), time.ToString());
            //GUI.Box(new Rect(100, 200, 400, 50), myPlayerID.ToString() + MissleExploMessageReciver.Instance.GetExploMsg(myGuid, myPlayerID).ToString());
            //}
        }
    }
}
