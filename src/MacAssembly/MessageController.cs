using System;
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
            ModNetworking.Callbacks[SRAAMBlock.MissleExplo] += MissleExploMessageReciver.Instance.ReceiveMsg;
            ModNetworking.Callbacks[MRAAMBlock.MissleExplo] += MissleExploMessageReciver.Instance.ReceiveMsg;

            ModNetworking.Callbacks[KeymsgController.SendHeld] += KeymsgController.HeldRcved;
            ModNetworking.Callbacks[KeymsgController.SendPressed] += KeymsgController.PressedRcved;
            ModNetworking.Callbacks[KeymsgController.SendHelds] += KeymsgController.HeldsRcved;
            ModNetworking.Callbacks[DisplayerBlock.ClientTargetDistanceMsg] += DisplayerMsgReceiver.Instance.DistanceReceiver;
            ModNetworking.Callbacks[DisplayerBlock.ClientTargetPositionMsg] += DisplayerMsgReceiver.Instance.PositionReceiver;
            ModNetworking.Callbacks[DisplayerBlock.ClientPanelMsg] += DisplayerMsgReceiver.Instance.PanelReceiver;
            ModNetworking.Callbacks[DisplayerBlock.ClientLockingMsg] += DisplayerMsgReceiver.Instance.LockingReceiver;
            ModNetworking.Callbacks[DisplayerBlock.ClientChooserMsg] += DisplayerMsgReceiver.Instance.ChooserPositionReceiver;
            ModNetworking.Callbacks[DisplayerBlock.ClientLockedTargetMsg] += DisplayerMsgReceiver.Instance.LockedTargetReceiver;
            ModNetworking.Callbacks[DisplayerBlock.ClientOnGuiTargetMsg] += DisplayerMsgReceiver.Instance.OnGuiTargetPositionReceiver;
        }
    }
}
