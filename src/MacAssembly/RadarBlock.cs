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
    public class Target
    {
        public bool hasObject;
        public bool enemy;
        public Vector3 position;
        public Vector3 velocity;
        public float closingRate;
        //public float displayAngle;

        public Target()
        {
            hasObject = false;
        }

        public Target(Collider col, BlockBehaviour radar)
        {
            hasObject = true;
            position = col.gameObject.transform.position;
            velocity = col.attachedRigidbody.velocity;

            //calculating the closingRate
            Vector3 myVelocity = radar.GetComponent<Rigidbody>().velocity;
            Vector3 relativeVelocity = velocity - myVelocity;
            Vector3 relativePositionNormalized = (radar.transform.position - position).normalized;
            closingRate = relativeVelocity.x * relativePositionNormalized.x + relativeVelocity.y * relativePositionNormalized.y + relativeVelocity.z * relativePositionNormalized.z;
            //calculate the angle displayed on displayer
            //displayAngle = vector2angle(new Vector2(myVelocity.x, myVelocity.z))-vector2angle(new Vector2(-relativePositionNormalized.x,-relativePositionNormalized.z));
            
            if (col.attachedRigidbody.gameObject.GetComponent<BlockBehaviour>().Team == radar.Team)
            {
                enemy = false;
            }
            else
            {
                enemy = true;
            }
        }
        private float vector2angle(Vector2 vec)
        {
            if (vec.y>0)
            {
                return Vector2.Angle(vec, Vector2.right);
            }
            else
            {
                return 360 - Vector2.Angle(vec, Vector2.right);
            }
        }
    }

    public class targetManager : MonoBehaviour
    {
        public Target[] targets = new Target[51];

        public targetManager()
        {
            for (int i = 0; i < 51; i++)
            {
                
                targets[i] = new Target();
                //Debug.Log(targets[i].hasObject);
            }
        }
        
        public void AddTarget(int currRegion, Collider col, BlockBehaviour radar)
        {
            targets[currRegion] = new Target(col, radar);
            Debug.Log(currRegion.ToString() + " add a target");
        }

        public void RemoveTarget(int currRegion)
        {
            //Debug.Log(1);
            if (targets[currRegion].hasObject)
            {
                targets[currRegion].hasObject = false;
                Debug.Log(currRegion.ToString() + " remove a target");
            }
        }
    }

    
    public class RadarBlock : BlockScript
    {


        public targetManager targetManagerRadar;
        public float scanAngle = 0;
        public float scanPitch = 0;
        public GameObject ScanCollider;
        public MeshCollider radarScan;
        public MeshFilter radarMF;
        public MeshRenderer radarMR;
        public GameObject RadarScanDisplayer;
        public GameObject buildAdvice;
        public GameObject RadarBase;
        public ScanCollisonHit radarHit;



        private Mesh scannerMesh;
        
        private Color displayColor;
        private TextMesh adviceTextMesh;

        public void InitAdvice()
        {
            if (!transform.FindChild("Advice"))
            {
                buildAdvice = new GameObject("Advice");
                buildAdvice.transform.SetParent(transform);
                buildAdvice.transform.localPosition = new Vector3(0f, 0.5f, 0.4f);
                buildAdvice.transform.localRotation = Quaternion.Euler(0, 180, 0);
                buildAdvice.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                adviceTextMesh = buildAdvice.AddComponent<TextMesh>();
                adviceTextMesh.text = "This side up";
                adviceTextMesh.characterSize = 0.25f;
                adviceTextMesh.fontSize = 64;
                adviceTextMesh.anchor = TextAnchor.MiddleCenter;
                adviceTextMesh.color = Color.green;
                buildAdvice.SetActive(true);
            }

        }
        private void InitScan()
        {
            if (transform.parent.transform.FindChild("Radar Base") == null)
            {
                scannerMesh = ModResource.GetMesh("RadarScan Mesh").Mesh;

                RadarBase = new GameObject("Radar Base");
                RadarBase.transform.SetParent(transform.parent.transform);
                RadarBase.transform.position = transform.position;
                RadarBase.transform.rotation = transform.rotation;
                RadarBase.transform.localScale = new Vector3(1, 1, 1);

                ScanCollider = new GameObject("RadarScanCol");
                ScanCollider.transform.SetParent(RadarBase.transform);
                ScanCollider.transform.localPosition = new Vector3(0f, 0f, 0f);
                ScanCollider.transform.localRotation = Quaternion.Euler(270f, 0f, 0f);
                ScanCollider.transform.localScale = new Vector3(5000, 5000, 5000);

                radarScan = ScanCollider.AddComponent<MeshCollider>();
                radarScan.sharedMesh = scannerMesh;
                radarScan.convex = true;
                radarScan.isTrigger = true;
                radarHit = ScanCollider.AddComponent<ScanCollisonHit>();

                if (ScanCollider.transform.FindChild("RadarScanDisplay") == null)
                {
                    RadarScanDisplayer = new GameObject("RadarScanDisplay");
                    RadarScanDisplayer.transform.SetParent(ScanCollider.transform);
                    RadarScanDisplayer.transform.localPosition = new Vector3(0f, 0f, 0f);
                    RadarScanDisplayer.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    RadarScanDisplayer.transform.localScale = Vector3.one;
                    radarMF = RadarScanDisplayer.AddComponent<MeshFilter>();
                    radarMF.sharedMesh = scannerMesh;
                    radarMR = RadarScanDisplayer.AddComponent<MeshRenderer>();
                    radarMR.material = new Material(Shader.Find("Particles/Alpha Blended"));
                    displayColor = Color.green;
                    displayColor.a = 0.05f;
                    radarMR.material.SetColor("_TintColor", displayColor);

                    RadarScanDisplayer.SetActive(true);
                }

                ScanCollider.SetActive(false);
                radarHit.Reset();
            }
        }

        private void GetTWSAim()
        {
            //Debug.Log(Math.Round((scanAngle + 60) / 2.4f));
            int currRegion = (int)Math.Floor((scanAngle+60)/2.4f+0.5f);
            //Debug.Log(currRegion);
            //Debug.Log(radarHit.targetCols.Count);
            if (radarHit.targetCols.Count == 0)
            {
                targetManagerRadar.RemoveTarget(currRegion);
            }
            else
            {
                Collider targetCol = radarHit.targetCols.Peek();
                //Debug.Log(targetCol.name);
                targetManagerRadar.AddTarget(currRegion, targetCol, BlockBehaviour);
            }
            
            
        }

        public override void SafeAwake()
        {

            InitScan();
            InitAdvice();
            targetManagerRadar = new targetManager();

        }

        public override void BuildingUpdate()
        {
            if (!buildAdvice.activeSelf)
            {
                buildAdvice.SetActive(true);
            }
        }

        public override void OnSimulateStart()
        {
            buildAdvice.SetActive(false);
            ScanCollider.SetActive(true);
        }
        public override void SimulateFixedUpdateHost()
        {
            GetTWSAim();
            RadarBase.transform.position = transform.position+4*transform.forward;
            RadarBase.transform.rotation = Quaternion.LookRotation((transform.rotation * Vector3.back).normalized);
            try
            {
                DisplayerBlock tmp;
                tmp = transform.parent.FindChild("Displayer").GetComponent<DisplayerBlock>();
                scanAngle = tmp.SLController.currAngle;
                scanPitch = tmp.radarPitch;
                ScanCollider.transform.localRotation = Quaternion.Euler(270f+scanPitch, scanAngle, 0);
                radarHit.Reset();

            }
            catch
            {}
            //Debug.Log(scanAngle);

 

        }



            void OnGUI()
        {
            if (BlockBehaviour.isSimulating)
            {

                //GUI.Box(new Rect(100, 100, 200, 50), displayerScript.SLController.currAngle.ToString());

            }
        }

    }
}
