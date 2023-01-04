using System;
using System.Collections.Generic;
using System.Text;

using Modding;

using Modding.Common;
using UnityEngine;


namespace ModernAirCombat
{
    class ModControllerMsgReceiver : SingleInstance<ModControllerMsgReceiver>
    {
        public override string Name { get; } = "ModControllerMsgReceiver";

        public bool RestrictionOn = false;
        public bool BoundaryOff = false;
        
        public void RestrictionMsgReceiver(Message msg)
        {
            RestrictionOn = (bool)msg.GetData(0);
        }
        public void BoundaryMsgReceiver(Message msg)
        {
            BoundaryOff = (bool)msg.GetData(0);
        }

    }

    class ModController : SingleInstance<ModController>
    {
        public override string Name { get; } = "MacModController";

        private Rect windowRect = new Rect(15f, 100f, 280f, 50f);
        private readonly int windowID = ModUtility.GetWindowId();
        public bool windowHidden = false;
        public bool BoundaryOff = false;
        public bool BoundaryOffGUI = false;
        public float BoundarySize = 20000f;
        public bool RestrictionGUI = false;
        public bool Restriction = false;
        public bool showWayPoints;
        public string path;

        public static MessageType ClientRestrictionMsg = ModNetworking.CreateMessageType(DataType.Boolean);
        public static MessageType ClientBoundaryMsg = ModNetworking.CreateMessageType(DataType.Boolean);


        public void KillBoundary()
        {
            BoundarySize = 10000f;

            StatMaster.Bounding.worldExtents = new Vector3(this.BoundarySize / 2f, this.BoundarySize, this.BoundarySize / 2f);

            GameObject WorldBoundaries = GameObject.Find("WORLD BOUNDARIES").gameObject;

            Transform Backtransform = WorldBoundaries.transform.Find("WorldBoundaryBack");
            Backtransform.position = new Vector3(0f, -250f, -this.BoundarySize / 2f);
            BoxCollider component3 = Backtransform.GetComponent<BoxCollider>();
            component3.size = new Vector3(this.BoundarySize, this.BoundarySize, 20f);
            component3.center = new Vector3(0f, this.BoundarySize / 2f, 0f);
            Transform transform2 = WorldBoundaries.transform.Find("WorldBoundaryFront");
            transform2.position = new Vector3(0f, -250f, this.BoundarySize / 2f);
            BoxCollider component4 = transform2.GetComponent<BoxCollider>();
            component4.size = new Vector3(this.BoundarySize, this.BoundarySize, 20f);
            component4.center = new Vector3(0f, this.BoundarySize / 2f, 0f);
            Transform transform3 = WorldBoundaries.transform.Find("WorldBoundaryLeft");
            transform3.position = new Vector3(-this.BoundarySize / 2f, -250f, 0f);
            BoxCollider component5 = transform3.GetComponent<BoxCollider>();
            component5.size = new Vector3(20f, this.BoundarySize, this.BoundarySize);
            component5.center = new Vector3(0f, this.BoundarySize / 2f, 0f);
            Transform transform4 = WorldBoundaries.transform.Find("WorldBoundaryRight");
            transform4.position = new Vector3(this.BoundarySize / 2f, -250f, 0f);
            BoxCollider component6 = transform4.GetComponent<BoxCollider>();
            component6.size = new Vector3(20f, this.BoundarySize, this.BoundarySize);
            component6.center = new Vector3(0f, this.BoundarySize / 2f, 0f);
            Transform transform5 = WorldBoundaries.transform.Find("WorldBoundaryTop");
            transform5.position = new Vector3(0f, this.BoundarySize - 250f, 0f);
            BoxCollider component7 = transform5.GetComponent<BoxCollider>();
            component7.size = new Vector3(this.BoundarySize, 20f, this.BoundarySize);
            component7.center = new Vector3(0f, 0f, 10f);
            Transform transform6 = WorldBoundaries.transform.Find("WorldBoundaryBottom");
            transform6.position = new Vector3(0f, -250f, 0f);
            BoxCollider component8 = transform6.GetComponent<BoxCollider>();
            component8.size = new Vector3(this.BoundarySize, 20f, this.BoundarySize);
            component8.center = new Vector3(0f, 0f, 10f);

            Collider[] componentsInChildren = WorldBoundaries.GetComponentsInChildren<Collider>();
            Bounds worldBounds = default(Bounds);
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                Bounds bounds = componentsInChildren[i].bounds;
                if (i==0)
                {
                    worldBounds = bounds;
                }
                else
                {
                    worldBounds.Encapsulate(bounds);
                }
            }
            NetworkCompression.SetWorldBounds(worldBounds);
        }
        public void RecoverBoundary()
        {

            this.BoundarySize = 2000f;
            GameObject gameObject5 = GameObject.Find("WORLD BOUNDARIES").gameObject;
            Transform transform7 = gameObject5.transform.Find("WorldBoundaryBack");
            transform7.position = new Vector3(0f, -250f, -1000f);
            BoxCollider component9 = transform7.GetComponent<BoxCollider>();
            component9.size = new Vector3(2000f, 2000f, 20f);
            component9.center = new Vector3(0f, 1000f, 0f);
            Transform transform8 = gameObject5.transform.Find("WorldBoundaryFront");
            transform8.position = new Vector3(0f, -250f, 1000f);
            BoxCollider component10 = transform8.GetComponent<BoxCollider>();
            component10.size = new Vector3(2000f, 2000f, 20f);
            component10.center = new Vector3(0f, 1000f, 0f);
            Transform transform9 = gameObject5.transform.Find("WorldBoundaryLeft");
            transform9.position = new Vector3(-1000f, -250f, 0f);
            BoxCollider component11 = transform9.GetComponent<BoxCollider>();
            component11.size = new Vector3(20f, 2000f, 2000f);
            component11.center = new Vector3(0f, 1000f, 0f);
            Transform transform10 = gameObject5.transform.Find("WorldBoundaryRight");
            transform10.position = new Vector3(1000f, -250f, 0f);
            BoxCollider component12 = transform10.GetComponent<BoxCollider>();
            component12.size = new Vector3(20f, 2000f, 2000f);
            component12.center = new Vector3(0f, 1000f, 0f);
            Transform transform11 = gameObject5.transform.Find("WorldBoundaryTop");
            transform11.position = new Vector3(0f, 1750f, 0f);
            BoxCollider component13 = transform11.GetComponent<BoxCollider>();
            component13.size = new Vector3(2000f, 20f, 2000f);
            component13.center = new Vector3(0f, 0f, 10f);
            Transform transform12 = gameObject5.transform.Find("WorldBoundaryBottom");
            transform12.position = new Vector3(0f, -250f, 0f);
            BoxCollider component14 = transform12.GetComponent<BoxCollider>();
            component14.size = new Vector3(2000f, 20f, 2000f);
            component14.center = new Vector3(0f, 0f, 10f);
            Collider[] componentsInChildren2 = gameObject5.GetComponentsInChildren<Collider>();
            Bounds worldBounds2 = default(Bounds);
            for (int j = 0; j < componentsInChildren2.Length; j++)
            {
                Bounds bounds2 = componentsInChildren2[j].bounds;
                if (j == 0)
                {
                    worldBounds2 = bounds2;
                }
                else
                {
                    worldBounds2.Encapsulate(bounds2);
                }
            }
            NetworkCompression.SetWorldBounds(worldBounds2);
        }

        private void Awake()
        {

        }

        public void Start()
        {
            
        }

        public void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.M))
                {
                    windowHidden = !windowHidden;
                }

            {
                //if (BoundaryOff && !BoundaryDestroyed && StatMaster.isMP && !StatMaster.isClient)
                //{
                //    if (BoundaryBottom == null)
                //    {
                //        GameObject worldBoundary = GameObject.Find("WORLD BOUNDARIES").gameObject;
                //        this.BoundaryBottom = Instantiate<GameObject>(worldBoundary.transform.Find("WorldBoundaryTop").gameObject);
                //        this.BoundaryBottom.transform.SetParent(worldBoundary.transform);
                //        this.BoundaryBottom.name = "WorldBoundaryBottom";
                //        this.BoundaryBottom.transform.position = new Vector3(0f, -250f, 0f);
                //    }

                //    if (GameObject.Find("ICE FREEZE") != null)
                //    {
                //        GameObject gameObject3 = GameObject.Find("ICE FREEZE").gameObject;
                //        gameObject3.SetActive(false);
                //    }

                //    StatMaster.Bounding.worldExtents = new Vector3(this.BoundarySize / 2f, this.BoundarySize, this.BoundarySize / 2f);
                //    GameObject WorldBoundary = GameObject.Find("WORLD BOUNDARIES").gameObject;

                //    Transform backTransform = WorldBoundary.transform.Find("WorldBoundaryBack");
                //    backTransform.position = new Vector3(0f, -250f, -this.BoundarySize / 2f);
                //    BoxCollider BackCollider = backTransform.GetComponent<BoxCollider>();
                //    BackCollider.size = new Vector3(this.BoundarySize, this.BoundarySize, 20f);
                //    BackCollider.center = new Vector3(0f, this.BoundarySize / 2f, 0f);

                //    Transform frontTransform = WorldBoundary.transform.Find("WorldBoundaryFront");
                //    frontTransform.position = new Vector3(0f, -250f, this.BoundarySize / 2f);
                //    BoxCollider FrontCollider = frontTransform.GetComponent<BoxCollider>();
                //    FrontCollider.size = new Vector3(this.BoundarySize, this.BoundarySize, 20f);
                //    FrontCollider.center = new Vector3(0f, this.BoundarySize / 2f, 0f);

                //    Transform leftTransform = WorldBoundary.transform.Find("WorldBoundaryLeft");
                //    leftTransform.position = new Vector3(-this.BoundarySize / 2f, -250f, 0f);
                //    BoxCollider LeftCollider = leftTransform.GetComponent<BoxCollider>();
                //    LeftCollider.size = new Vector3(20f, this.BoundarySize, this.BoundarySize);
                //    LeftCollider.center = new Vector3(0f, this.BoundarySize / 2f, 0f);

                //    Transform rightTransform = WorldBoundary.transform.Find("WorldBoundaryRight");
                //    rightTransform.position = new Vector3(this.BoundarySize / 2f, -250f, 0f);
                //    BoxCollider RightCollider = rightTransform.GetComponent<BoxCollider>();
                //    RightCollider.size = new Vector3(20f, this.BoundarySize, this.BoundarySize);
                //    RightCollider.center = new Vector3(0f, this.BoundarySize / 2f, 0f);

                //    Transform topTransform = WorldBoundary.transform.Find("WorldBoundaryTop");
                //    topTransform.position = new Vector3(0f, this.BoundarySize - 250f, 0f);
                //    BoxCollider TopCollider = topTransform.GetComponent<BoxCollider>();
                //    TopCollider.size = new Vector3(this.BoundarySize, 20f, this.BoundarySize);
                //    TopCollider.center = new Vector3(0f, 0f, 10f);

                //    Transform bottomTransform = WorldBoundary.transform.Find("WorldBoundaryBottom");
                //    bottomTransform.position = new Vector3(0f, -250f, 0f);
                //    BoxCollider bottomCollider = bottomTransform.GetComponent<BoxCollider>();
                //    bottomCollider.size = new Vector3(this.BoundarySize, 20f, this.BoundarySize);
                //    bottomCollider.center = new Vector3(0f, 0f, 10f);

                //    Collider[] componentsInChildren = WorldBoundary.GetComponentsInChildren<Collider>();
                //    Bounds worldBounds = default;
                //    for (int i = 0; i < componentsInChildren.Length; i++)
                //    {
                //        Bounds bounds = componentsInChildren[i].bounds;
                //        if (i == 0)
                //        {
                //            worldBounds = bounds;
                //        }
                //        else
                //        {
                //            worldBounds.Encapsulate(bounds);
                //        }
                //    }
                //    NetworkCompression.SetWorldBounds(worldBounds);
                //    BoundaryDestroyed = true;
                //}
                //else if (!BoundaryOff && BoundaryDestroyed && StatMaster.isMP)
                //{
                //    GameObject WorldBoundary = GameObject.Find("WORLD BOUNDARIES").gameObject;
                //    for (int i = 0; i < 5; i++)
                //    {
                //        WorldBoundary.transform.GetChild(i).gameObject.GetComponent<BoxCollider>().enabled = true;
                //    }
                //}
            }
            
            }

            if (!StatMaster.isClient)
            {
                if (Restriction != RestrictionGUI)
                {
                    Restriction = RestrictionGUI;
                    ModNetworking.SendToAll(ClientRestrictionMsg.CreateMessage(Restriction));
                }
            }
            else
            {
                Restriction = ModControllerMsgReceiver.Instance.RestrictionOn;
            }

            if (!StatMaster.isClient)
            {
                if (BoundaryOff != BoundaryOffGUI)
                {
                    BoundaryOff = BoundaryOffGUI;
                    if (BoundaryOff)
                    {
                        KillBoundary();
                    }
                    else
                    {
                        RecoverBoundary();
                    }
                    ModNetworking.SendToAll(ClientBoundaryMsg.CreateMessage(BoundaryOff));
                }
            }
            else
            {
                if (BoundaryOff != ModControllerMsgReceiver.Instance.BoundaryOff)
                {
                    BoundaryOff = ModControllerMsgReceiver.Instance.BoundaryOff;
                    if (BoundaryOff)
                    {
                        KillBoundary();
                    }
                    else
                    {
                        RecoverBoundary();
                    }
                }
            }



        }

        public void FixedUpdate()
        {
            ModNetworking.SendToAll(ClientRestrictionMsg.CreateMessage(Restriction));
            ModNetworking.SendToAll(ClientBoundaryMsg.CreateMessage(BoundaryOff));
        }
        private void MACWindow(int windoID)
        {
            BoundaryOffGUI = GUILayout.Toggle(BoundaryOff, "Turn Off Boundary (Provided by AdCustomModule)");
            RestrictionGUI = GUILayout.Toggle(RestrictionGUI, "Turn On Missile Restriction");
            GUILayout.Label("## Important Notification ##");
            GUILayout.Label("New block Central Controller and Multiple Function Displayer are added!");
            GUILayout.Label("It's highly recommended to replace your displayers and cameras with these two blocks.");
            GUILayout.Label("Radar Displayer, A2G Displayer, Load Displayer are now deprecated, which means that they will not be maintained from now on.");

            GUILayout.Label("Press Ctrl+M to hide");
            
            GUI.DragWindow();

        }

        private void OnGUI()
        {
            //GUI.Box(new Rect(100, 200, 200, 50), BoundaryOff.ToString());

            if (StatMaster.isMP && !StatMaster.isClient && !windowHidden && !StatMaster.hudHidden)
            {
                windowRect = GUILayout.Window(windowID, windowRect, new GUI.WindowFunction(MACWindow), "Modern Air Combat Mod Setting");
            }
            else if (!StatMaster.isMP && !windowHidden && !StatMaster.hudHidden)
            {
                windowRect = GUILayout.Window(windowID, windowRect, new GUI.WindowFunction(MACWindow), "Modern Air Combat Mod Setting");
            }
        }
    }
}
