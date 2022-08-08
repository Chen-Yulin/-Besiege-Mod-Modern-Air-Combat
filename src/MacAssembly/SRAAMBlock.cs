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

        private Transform myTransform;      //实例化Transform对象
        private Rigidbody myRigidbody;
        private float time = 0;
        private float detectFreqTime = 0;
        private float detectRange;
        private float detectWidth;
        private MeshFilter scannerMeshFilter;
        private Mesh scannerMesh;
        private MeshRenderer scannerRenderer;
        private Color scannerColor;
        private Quaternion launchRotation;
        private bool getlaunchRotation = false;

        public void initScan()
        {
            if (BlockBehaviour.transform.FindChild("ScanCol") == null)
            {
                scannerMesh = ModResource.GetMesh("Cone Scan").Mesh;

                ScanCollider = new GameObject("ScanCol");
                ScanCollider.transform.SetParent(BlockBehaviour.transform);
                ScanCollider.transform.localPosition = new Vector3(0f, 4f, 0.3f);
                ScanCollider.transform.localRotation = Quaternion.Euler(90, 0, 0);
                ScanCollider.transform.localScale = Vector3.one;
                missleScan = ScanCollider.AddComponent<MeshCollider>();
                missleScan.sharedMesh = scannerMesh;
                missleScan.convex = true;
                missleScan.isTrigger = true;
                coneHit = ScanCollider.AddComponent<ConeCollisonHit>();

                ScanCollider.SetActive(false);
                coneHit.Reset();



                // render the mesh of scanner
                ScannerDisplay = new GameObject("Scanner Display");
                ScannerDisplay.transform.SetParent(BlockBehaviour.transform);
                ScannerDisplay.transform.localPosition = new Vector3(0f, 4f, 0.3f);
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


        private void GetAim()
        {
            ScanCollider.SetActive(true);
            if (coneHit.targetCols.Count == 0)
            {
                targetDetected = false;
                return;
            }
            else
            {
                targetDetected = true;
            }
            detectFreqTime += Time.fixedDeltaTime;
            
            if (detectFreqTime > 0.05)
            {
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
                float estimatedTime;

                //calculate three times to ensure precision
                float myVelocity = Rigidbody.velocity.magnitude;
                //Debug.Log(myVelocity);
                estimatedTime = (targetPosition - transform.position).magnitude/myVelocity;
                predictPosition = targetPosition + targetVelocity * estimatedTime;
                estimatedTime = (predictPosition - transform.position).magnitude / myVelocity;
                predictPosition = targetPosition + targetVelocity * estimatedTime;
                estimatedTime = (predictPosition - transform.position).magnitude / myVelocity;
                predictPosition = targetPosition + targetVelocity * estimatedTime;
                Debug.Log(predictPosition);

                launchRotation = transform.rotation;
                ScanCollider.SetActive(false);
                coneHit.Reset();
            }
            


        }


        public override void SafeAwake()
        {
            Launch = AddKey("发射", "launch", KeyCode.X);
            IFF = AddToggle("开启友伤", "IFF", true);
            showScanner = AddToggle("显示探测范围", "showScanner", false);
            detectAngleSlider = AddSlider("探测角度", "detection angle", 90.0f, 60.0f, 120.0f);
            detectDelay = AddSlider("延时保险", "detection delay", 0.2f, 0.0f, 1f);
            launchDelay = AddSlider("延时点火", "launch delay", 0.1f, 0.0f, 0.3f);
            initScan();//挂载上导弹前方的圆锥触发器
        }

        public void Start()
        {
            myTransform = gameObject.GetComponent<Transform>();        //获取相应对象的引用
            myRigidbody = gameObject.GetComponent<Rigidbody>();

            detectRange = (float)(400.0f * System.Math.Cos(detectAngleSlider.Value / 2 * 3.1415f / 180));
            detectWidth = 2 * (float)(System.Math.Tan(detectAngleSlider.Value / 2 * 3.1415f / 180) * detectRange);
            Vector3 ScanColScale = new Vector3(detectWidth, detectWidth, detectRange);
            ScanCollider.transform.localScale = ScanColScale;
            ScannerDisplay.transform.localScale = ScanColScale;
            Debug.Log(ScanColScale);
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
            if (Launch.IsHeld && myStatus == status.stored)
            {
                myStatus = status.launched;
                Debug.Log("missle launched");
                Debug.Log(detectRange);
            }
        }

        public override void SimulateFixedUpdateHost()
        {

            if (Launch.EmulationHeld() && myStatus == status.stored)
            {
                myStatus = status.launched;
                Debug.Log("missle launched");
                Debug.Log(detectRange);
                myRigidbody.drag = 0.1f;
                myRigidbody.angularDrag = 4.0f;

            }

            if (myStatus == status.launched)
            {
                if (!getlaunchRotation)
                {
                    launchRotation = transform.rotation;
                    getlaunchRotation = true;
                    Debug.Log(launchRotation);
                }

                if (time < 3.0f + launchDelay.Value)
                {
                    if (time > launchDelay.Value)
                    {
                        myRigidbody.AddRelativeForce(new Vector3(0, 1500, 0), ForceMode.Force);
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
                    }
                    time += Time.fixedDeltaTime;
                }
                else
                {
                    myStatus = status.missed;
                    myRigidbody.drag = 0.1f;
                    myRigidbody.angularDrag = 1.0f;
                }
            }
            if (myStatus == status.missed)
            {
                if (myRigidbody.position.y > 20)
                {
                    myTransform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(-180, 0, 0), 0.001f);
                }
            }
        }

        void OnGUI()
        {


        }


    }
}
