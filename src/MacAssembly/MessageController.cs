using System;
using System.Collections.Generic;
using System.Text;

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
            ModNetworking.Callbacks[RadarBlock.ClientRadarHeadMsg] += RadarMsgReceiver.Instance.RadarHeadMsgReceiver;
            ModNetworking.Callbacks[MachineGunBlock.ClientFirePara] += MachineGunMsgReceiver.Instance.FireParaReceiver;
            ModNetworking.Callbacks[MachineGunBlock.ClientBulletExploMsg] += MachineGunMsgReceiver.Instance.BulletExploMsgReceiver;
            ModNetworking.Callbacks[RWRBlock.ClientRWRData] += RWRMsgReceiver.Instance.DataReceiver;
            ModNetworking.Callbacks[DisplayerBlock.ClientBlackoutMsg] += DisplayerMsgReceiver.Instance.BlackoutReceiver;
            ModNetworking.Callbacks[CentralController.ClientBlackoutMsg] += CCDataReceiver.Instance.BlackoutMsgReceiver;
            ModNetworking.Callbacks[HUDBlock.ClientHUDBVRMsg] += HUDMsgReceiver.Instance.DataReceiver;
            ModNetworking.Callbacks[ModController.ClientRestrictionMsg] += ModControllerMsgReceiver.Instance.RestrictionMsgReceiver;
            ModNetworking.Callbacks[ModController.ClientBoundaryMsg] += ModControllerMsgReceiver.Instance.BoundaryMsgReceiver;
            ModNetworking.Callbacks[StickBlock.ClientAxisMsg] += StickMsgReceiver.Instance.AxisMsgReceiver;
            ModNetworking.Callbacks[ElectroOpticalBlock.ClientLockPointPositionMsg] += EOMsgReceiver.Instance.PositionReceiver;
            ModNetworking.Callbacks[ElectroOpticalBlock.ClientLockPointVelocityMsg] += EOMsgReceiver.Instance.VelocityReceiver;
            ModNetworking.Callbacks[ElectroOpticalBlock.ClientLockMsg] += EOMsgReceiver.Instance.LockReceiver;
            ModNetworking.Callbacks[ElectroOpticalBlock.ClientFOVMsg] += EOMsgReceiver.Instance.FOVReceiver;
            ModNetworking.Callbacks[A2GScreenBlock.ClientTrackMsg] += EOMsgReceiver.Instance.TrackReveiver;
            ModNetworking.Callbacks[A2GDisplayerSimulator.ClientTrackMsg] += EOMsgReceiver.Instance.TrackReveiver;
            ModNetworking.Callbacks[ElectroOpticalBlock.ClientTrackMsg] += EOMsgReceiver.Instance.TrackReveiver;
            ModNetworking.Callbacks[ElectroOpticalBlock.ClientThermalOnMsg] += EOMsgReceiver.Instance.ThermalOnReceiver;
            ModNetworking.Callbacks[ElectroOpticalBlock.ClientInverseThermalMsg] += EOMsgReceiver.Instance.ThermalInverseReceiver;

            // for CC
            ModNetworking.Callbacks[RadarDisplayerSimulator.ClientNormalPanelMsg] += RadarDisplayerSimulator_MsgReceiver.Instance.NormalPanelReceiver;
            ModNetworking.Callbacks[RadarDisplayerSimulator.ClientLockPanelMsg] += RadarDisplayerSimulator_MsgReceiver.Instance.LockPanelReceiver;
            ModNetworking.Callbacks[RadarDisplayerSimulator.ClientLockStatusMsg] += RadarDisplayerSimulator_MsgReceiver.Instance.LockStatusReceiver;
            ModNetworking.Callbacks[RadarBlock.ClientTmpTargetData] += RadarDisplayerSimulator_MsgReceiver.Instance.tmpTargetDataReceiver;
            ModNetworking.Callbacks[MFD.clientMFDType] += MFDMsgReceiver.Instance.ScreenTypeMsgReceiver;
            ModNetworking.Callbacks[RadarDisplayerSimulator.ClientOnGuiTargetMsg] += CCDataReceiver.Instance.RadarOnGUIMsgReceiver;
        }
    }
}
