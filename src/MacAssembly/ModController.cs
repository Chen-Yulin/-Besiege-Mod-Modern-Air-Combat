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
        public bool RestrictionGUI = false;
        public bool Restriction = false;

        public int state = 0;

        // for customize
        public float ElectrOpticalCameraDistance = 20000f;

        public static MessageType ClientRestrictionMsg = ModNetworking.CreateMessageType(DataType.Boolean);
        public static MessageType ClientBoundaryMsg = ModNetworking.CreateMessageType(DataType.Boolean);


        
        

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

        public void FixedUpdate()
        {
            if (state == 40)
            {
                ModNetworking.SendToAll(ClientRestrictionMsg.CreateMessage(Restriction));
                state = 0;
            }
            else
            {
                state++;
            }
        }
        private void MACWindow(int windoID)
        {
            RestrictionGUI = GUILayout.Toggle(RestrictionGUI, "Turn On Missile Restriction (Host Only)");

            // for free boundary
            if (GUILayout.Button("Apply", new GUILayoutOption[0]))
            {
                SingleInstance<SpiderFucker>.Instance.Apply();
            }
            SingleInstance<SpiderFucker>.Instance.FloorDeactiveSwitch = GUILayout.Toggle(SingleInstance<SpiderFucker>.Instance.FloorDeactiveSwitch, "Floor Deactive", new GUILayoutOption[0]);
            if (SingleInstance<SpiderFucker>.Instance.ExpandFloorSwitch && SingleInstance<SpiderFucker>.Instance.ExExpandFloorSwitch)
            {
                SingleInstance<SpiderFucker>.Instance.ExExpandFloorSwitch = false;
            }
            SingleInstance<SpiderFucker>.Instance.ExExpandFloorSwitch = GUILayout.Toggle(SingleInstance<SpiderFucker>.Instance.ExExpandFloorSwitch, "Customized Boundary(m)", new GUILayoutOption[0]);
            if (SingleInstance<SpiderFucker>.Instance.ExpandFloorSwitch && SingleInstance<SpiderFucker>.Instance.ExExpandFloorSwitch)
            {
                SingleInstance<SpiderFucker>.Instance.ExpandFloorSwitch = false;
            }
            SingleInstance<SpiderFucker>.Instance.ExExpandScale = Convert.ToSingle(GUILayout.TextArea(SingleInstance<SpiderFucker>.Instance.ExExpandScale.ToString(), new GUILayoutOption[0]));

            GUILayout.Label("Press Ctrl+M to hide");
            
            GUI.DragWindow();

        }

        private void OnGUI()
        {
            //GUI.Box(new Rect(100, 200, 200, 50), BoundaryOff.ToString());
            if (!windowHidden && !StatMaster.hudHidden)
            {
                windowRect = GUILayout.Window(windowID, windowRect, new GUI.WindowFunction(MACWindow), "Modern Air Combat Mod Setting");
            }

        }
    }
}
