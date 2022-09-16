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
    class StickMsgReceiver : SingleInstance<StickMsgReceiver>
    {
        public override string Name { get; } = "StickMsgReceiver";

        public float[] XAxis = new float[16];
        public float[] YAxis = new float[16];
        public float[] referSize = new float[16];
        
        public void AxisMsgReceiver(Message msg)
        {
            int playerID = (int)msg.GetData(0);
            XAxis[playerID] = (float)msg.GetData(1);
            YAxis[playerID] = (float)msg.GetData(2);
            referSize[playerID] = (float)msg.GetData(3);
        }
       
    }

    class StickBlock : BlockScript
    {
        public MKey startKey;
        public MKey leftKey;
        public MKey rightKey;
        public MKey upKey;
        public MKey downKey;
        public MSlider sensitivity;
        public MSlider returnSpeed;
        public MSlider deadZone;
        public MToggle DisplayAxis;
        public MToggle ToggleMode;

        public List<SteeringWheel> leftBlock = new List<SteeringWheel>();
        public List<SteeringWheel> rightBlock = new List<SteeringWheel>();
        public List<SteeringWheel> upBlock = new List<SteeringWheel>();
        public List<SteeringWheel> downBlock = new List<SteeringWheel>();

        public int myPlayerID;

        public static MessageType ClientAxisMsg = ModNetworking.CreateMessageType(DataType.Integer, DataType.Single, DataType.Single, DataType.Single); //playerID, XAxis, YAxis, referSize

        public bool StickOn = false;
        public Vector2 initialPostion = Vector2.zero;
        public int crossIconSize = 250;
        public float Sensitivity = 1;
        public float XAxis;
        public float YAxis;

        private Texture crossTexture;
        private Texture deadZoneTexture;

        public void SetAxis()
        {
            if (Input.mousePosition.x > initialPostion.x + deadZone.Value)
            {
                XAxis = Input.mousePosition.x - initialPostion.x - deadZone.Value;
            }
            else if (Input.mousePosition.x < initialPostion.x - deadZone.Value)
            {
                XAxis = Input.mousePosition.x - initialPostion.x + deadZone.Value;
            }
            else
            {
                XAxis = 0;
            }

            if (Input.mousePosition.y > initialPostion.y + deadZone.Value)
            {
                YAxis = Input.mousePosition.y - initialPostion.y - deadZone.Value;
            }
            else if (Input.mousePosition.y < initialPostion.y - deadZone.Value)
            {
                YAxis = Input.mousePosition.y - initialPostion.y + deadZone.Value;
            }
            else
            {
                YAxis = 0;
            }
        }

        public void SetHinge(float referSize)
        {
            float XAngle = XAxis * sensitivity.Value / referSize;
            float YAngle = YAxis * sensitivity.Value / referSize;


            foreach (SteeringWheel block in leftBlock)
            {
                if (leftKey.GetKey(0) == block.KeyList[0].GetKey(0))
                {
                    block.AngleToBe = (!block.Flipped) ? -XAngle : XAngle;
                }
                else
                {
                    block.AngleToBe = (block.Flipped) ? -XAngle : XAngle;
                }

            }


            foreach (SteeringWheel block in upBlock)
            {
                if (upKey.GetKey(0) == block.KeyList[0].GetKey(0))
                {
                    block.AngleToBe = (block.Flipped) ? -YAngle : YAngle;
                }
                else
                {
                    block.AngleToBe = (!block.Flipped) ? -YAngle : YAngle;
                }
            }




        }

        public override void SafeAwake()
        {
            myPlayerID = BlockBehaviour.ParentMachine.PlayerID;
            crossTexture = ModResource.GetTexture("Cross Texture").Texture;
            deadZoneTexture = ModResource.GetTexture("DeadZone Texture").Texture;

            startKey = AddKey("Use Stick", "Use Stick", KeyCode.V);
            leftKey = AddKey("Left Binding Key", "Left Binding Key", KeyCode.A);
            rightKey = AddKey("Right Binding Key", "Right Binding Key", KeyCode.D);
            upKey = AddKey("Up Binding Key", "Up Binding Key", KeyCode.W);
            downKey = AddKey("Down Binding Key", "Down Binding Key", KeyCode.S);
            ToggleMode = AddToggle("Toggle Mode", "Toggle Mode", false);
            DisplayAxis = AddToggle("Display Axises", "Display Axises", true);
            sensitivity = AddSlider("Senitivity", "Sensitivity", 1f, 0.1f, 10f);
            returnSpeed = AddSlider("Return Speed", "Return Speed", 1f, 0f, 3f);
            deadZone = AddSlider("DeadZone Size", "DeadZone Size", 25, 0, 75);
        }

        public void Start()
        {

        }
        public override void OnSimulateStart()
        {
            foreach(BlockBehaviour block in this.GetComponent<BlockBehaviour>().ParentMachine.SimulationBlocks)
            {
                if (block.gameObject.name == "SteeringHinge")
                {
                    foreach (MKey key in block.KeyList)
                    {
                        for (int i = 0; i < key.KeysCount; i++)
                        {
                            bool hasAdd = false;
                            if (key.GetKey(i) == leftKey.GetKey(i))
                            {
                                leftBlock.Add(block.GetComponent<SteeringWheel>());
                                hasAdd = true;
                            }
                            if (key.GetKey(i) == rightKey.GetKey(i))
                            {
                                rightBlock.Add(block.GetComponent<SteeringWheel>());
                                hasAdd = true;
                            }
                            if (key.GetKey(i) == upKey.GetKey(i))
                            {
                                upBlock.Add(block.GetComponent<SteeringWheel>());
                                hasAdd = true;
                            }
                            if (key.GetKey(i) == downKey.GetKey(i))
                            {
                                downBlock.Add(block.GetComponent<SteeringWheel>());
                                hasAdd = true;
                            }
                            if (hasAdd)
                            {
                                break;
                            }
                        }
                    }
                }
            }

        }

        protected void Update()
        {
            if (IsSimulating)
            {


                if (ToggleMode.IsActive)
                {
                    if (startKey.IsPressed)
                    {
                        initialPostion = Input.mousePosition;
                        StickOn = !StickOn;
                    }
                }
                else
                {
                    if (startKey.IsPressed)
                    {
                        initialPostion = Input.mousePosition;
                    }
                    StickOn = startKey.IsHeld;
                }
                


                if (StickOn)
                {
                    if (StatMaster.isMP && myPlayerID != 0)
                    {
                        if (PlayerData.localPlayer.networkId == myPlayerID)
                        {
                            SetAxis();
                            ModNetworking.SendToAll(ClientAxisMsg.CreateMessage(myPlayerID, XAxis, YAxis, (float)(Camera.main.pixelHeight + Camera.main.pixelWidth) / 64f));
                        }

                        if (!StatMaster.isClient)
                        {
                            XAxis = StickMsgReceiver.Instance.XAxis[myPlayerID];
                            YAxis = StickMsgReceiver.Instance.YAxis[myPlayerID];
                            SetHinge(StickMsgReceiver.Instance.referSize[myPlayerID]);
                        }
                    }
                    else
                    {
                        SetAxis();
                        SetHinge((Camera.main.pixelHeight + Camera.main.pixelWidth) / 64);
                    }


                }
                else
                {
                    XAxis = 0;
                    YAxis = 0;
                }
            }

        }

        public void FixedUpdate()
        {
            initialPostion = Vector2.Lerp(initialPostion, Input.mousePosition, 0.01f*returnSpeed.Value);
        }

        void OnGUI()
        {
            //GUI.Box(new Rect(100, 100, 200, 50), XAxis.ToString());
            //GUI.Box(new Rect(100, 100, 200, 50), YAxis.ToString());
            if (!DisplayAxis.IsActive)
            {
                return;
            }
            if (StatMaster.isMP)
            {
                if (PlayerData.localPlayer.networkId != myPlayerID)
                {
                    return;
                }
            }
            if (StickOn)
            {
                GUI.DrawTexture(new Rect(initialPostion.x - crossIconSize / 2, Camera.main.pixelHeight - initialPostion.y - crossIconSize / 2, crossIconSize, crossIconSize), crossTexture);
                GUI.DrawTexture(new Rect(initialPostion.x - deadZone.Value / 2, Camera.main.pixelHeight - initialPostion.y - deadZone.Value / 2, deadZone.Value, deadZone.Value), deadZoneTexture);
            }
        }
    }
}
