﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using Modding;

namespace ModernAirCombat
{
    public class MessageController : SingleInstance<MessageController>
    {
        public override string Name { get; } = "Message Controller";

        public MessageController()
        {
            ModNetworking.Callbacks[FlareBlock.LaunchPara] += FlareMessageReciver.Instance.ReceiveMsg;
        }
    }
}