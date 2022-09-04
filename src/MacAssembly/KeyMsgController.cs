using System;
using System.Collections.Generic;
using System.Text;

using Modding.Modules;
using Modding;
using Modding.Blocks;
using UnityEngine;


namespace ModernAirCombat
{
    class KeymsgController : SingleInstance<KeymsgController>
    {
        public override string Name { get; } = "Keymsg Controller";
        public Dictionary<int, bool>[] keyheld = new Dictionary<int, bool>[16];
        public Dictionary<int, bool>[] keypressed = new Dictionary<int, bool>[16];
        public Dictionary<int, bool>[,] keysheld = new Dictionary<int, bool>[16, 9];
        public static MessageType SendHeld = ModNetworking.CreateMessageType(DataType.Integer, DataType.Integer, DataType.Boolean);
        public static MessageType SendHelds = ModNetworking.CreateMessageType(DataType.Integer, DataType.Integer, DataType.Integer, DataType.Boolean);//playerid, keyindex, guid, heldOrNot
        public static MessageType SendPressed = ModNetworking.CreateMessageType(DataType.Integer, DataType.Integer, DataType.Boolean);

        public KeymsgController()
        {
            for (int i = 0; i < 16; i++)
            {
                keyheld[i] = new Dictionary<int, bool>();
                keypressed[i] = new Dictionary<int, bool>();
                for (int j = 0; j < 9; j++)
                {
                    keysheld[i, j] = new Dictionary<int, bool>();
                }
            }

        }
        public static void HeldRcved(Message message)
        {
            if (StatMaster.isClient)
            {
                Instance.keyheld[(int)message.GetData(0)][(int)message.GetData(1)] = (bool)message.GetData(2);
            }
        }
        public static void HeldsRcved(Message message)
        {
            if (StatMaster.isClient)
            {
                Instance.keysheld[(int)message.GetData(0),(int)message.GetData(1)][(int)message.GetData(2)] = (bool)message.GetData(3);
            }
        }
        public static void PressedRcved(Message message)
        {
            if (StatMaster.isClient)
            {
                Instance.keypressed[(int)message.GetData(0)][(int)message.GetData(1)] = (bool)message.GetData(2);
            }
        }
    }
}
