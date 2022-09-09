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
        
        public void RestrictionMsgReceiver(Message msg)
        {
            RestrictionOn = (bool)msg.GetData(0);
        }

    }

    class ModController : SingleInstance<ModController>
    {
        public override string Name { get; } = "MacModController";

        private Rect windowRect = new Rect(15f, 100f, 280f, 50f);
        private readonly int windowID = ModUtility.GetWindowId();
        public bool windowHidden = false;
        public bool BoundaryOff = true;
        public bool BoundaryDestroyed = false;
        //public float BoundarySize = 20000000f;
        public bool RestrictionGUI = false;
        public bool Restriction = false;

        public static MessageType ClientRestrictionMsg = ModNetworking.CreateMessageType(DataType.Boolean);

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

            

        }

        private void MACWindow(int windoID)
        {
            //BoundaryOff = GUILayout.Toggle(BoundaryOff, "Turn Off Boundary");
            RestrictionGUI = GUILayout.Toggle(RestrictionGUI, "Turn On Missile Restriction");
            GUILayout.Label("Press Ctrl+M to hide");
            GUI.DragWindow();

        }

        private void OnGUI()
        {
            //GUI.Box(new Rect(100, 200, 200, 50), Restriction.ToString());

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
