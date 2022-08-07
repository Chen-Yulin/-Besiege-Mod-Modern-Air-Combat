using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections;

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
        void Start()
        {
        }
        void Update()
        {
            //Debug.Log("-1");
        }

        void OnTriggerEnter(Collider col)
        {
            Debug.Log(col.gameObject.name);
        }


    }

    class SRAAMBlock : BlockScript
    {
        public MKey Launch;
        public MToggle IFF;
        public MSlider detectAngleSlider;
        public enum status {stored,launched,missed,exploded};
        public status myStatus;
        public GameObject ScanCollider;
        public MeshCollider missleScan;
        public ConeCollisonHit coneHit;

        private Transform myTransform;      //实例化Transform对象
        private Rigidbody myRigidbody;
        private float time = 0;
        private float detectRange;
        private float detectWidth;
        private bool LogScanCol = false;


        public void initScan()
        {
            if (BlockBehaviour.transform.FindChild("ScanCol") == null)
            {
                ScanCollider = new GameObject("ScanCol");
                ScanCollider.transform.SetParent(BlockBehaviour.transform);
                ScanCollider.transform.localPosition = new Vector3(0f, 5f, 0f);
                ScanCollider.transform.localRotation = Quaternion.identity;
                ScanCollider.transform.localScale = Vector3.one;
                missleScan = ScanCollider.AddComponent<MeshCollider>();
                missleScan.sharedMesh = ModResource.GetMesh("Cone Scan").Mesh;
                missleScan.convex = true;
                missleScan.isTrigger = true;

                coneHit = ScanCollider.AddComponent<ConeCollisonHit>();
                ScanCollider.SetActive(false);
            }
        }


        public override void SafeAwake()
        {
            Launch = AddKey("发射", "launch", KeyCode.X);
            IFF = AddToggle("开启友伤", "IFF", true);
            detectAngleSlider = AddSlider("探测角度", "detection angle", 90.0f, 60.0f, 120.0f);

            initScan();//挂载上导弹前方的圆锥触发器
        }
        public void Start()
        {
            myTransform = gameObject.GetComponent<Transform>();        //获取相应对象的引用
            myRigidbody = gameObject.GetComponent<Rigidbody>();

            detectRange = (float)(800.0f *System.Math.Cos(detectAngleSlider.Value / 2 * 3.1415f / 180));
            detectWidth = 2*(float)(System.Math.Tan(detectAngleSlider.Value / 2 * 3.1415f / 180) * detectRange);
            Vector3 ScanColScale = new Vector3(detectWidth, detectRange, detectWidth);
            ScanCollider.transform.localScale = ScanColScale;
            Debug.Log(ScanColScale);
            coneHit.IFF = IFF.IsActive;
            coneHit.team = BlockBehaviour.Team;
            coneHit.PlayerID = BlockBehaviour.ParentMachine.PlayerID;
            ScanCollider.SetActive(true);


        }

        public override void OnSimulateStart()
        {
            base.OnSimulateStart();
        }

        public override void OnSimulateStop()
        {
            base.OnSimulateStop();
        }
        
        

         

        private void Update() {
            if(LogScanCol == false)
            {
                Debug.Log(ScanCollider.transform.localScale);
                Debug.Log(ScanCollider.transform.position);
                Debug.Log(ScanCollider.transform.localRotation);
                Debug.Log(missleScan.transform.localScale);
                Debug.Log(missleScan.transform.position);
                
                LogScanCol=true;
            }
        
            if (Launch.IsHeld && myStatus == status.stored)
            {
                myStatus = status.launched;
                Debug.Log("missle launched");
                Debug.Log(detectRange);
                myRigidbody.drag = 2.0f;
                myRigidbody.angularDrag = 4.0f;
                
            }
        }

       
        private IEnumerator GetAim()
        {
            yield break;
        }

        public override void SimulateFixedUpdateHost()
        {
            if (Launch.EmulationHeld() && myStatus == status.stored)
            {
                myStatus = status.launched;
                Debug.Log("missle launched");
                Debug.Log(detectRange);
                myRigidbody.drag = 2.0f;
                myRigidbody.angularDrag = 4.0f;

            }

            if (myStatus == status.launched)
            {
                if (time < 3.0f)
                {
                    myRigidbody.AddRelativeForce(new Vector3(0, 1000, 0), ForceMode.Force);
                    time += Time.deltaTime;
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
    }
}
