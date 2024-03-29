using System;
using Modding;
using UnityEngine;
using Navalmod;
using skpCustomModule;

namespace ModernAirCombat
{
    public enum BlockList
    {
        SRAAM = 1,
		MRAAM = 2,
		LRAAM = 3
    }
    public class ModernAirCombat : ModEntryPoint
	{
		public static GameObject Mod;
		public override void OnLoad()
		{
			Mod = new GameObject("Morden Air Combat Mod");
			UnityEngine.Object.DontDestroyOnLoad(Mod);

			Mod.AddComponent<FlareMessageReciver>();
			Mod.AddComponent<MissleExploMessageReciver>();
			Mod.AddComponent<DataManager>();
			Mod.AddComponent<KeymsgController>();
			Mod.AddComponent<DisplayerMsgReceiver>();
			Mod.AddComponent<MachineGunMsgReceiver>();
			Mod.AddComponent<RWRMsgReceiver>();
			Mod.AddComponent<RadarMsgReceiver>();
			Mod.AddComponent<ModController>();
			Mod.AddComponent<StickMsgReceiver>();
			Mod.AddComponent<HUDMsgReceiver>();
			Mod.AddComponent<EOMsgReceiver>();
			Mod.AddComponent<ModControllerMsgReceiver>();
			Mod.AddComponent<LoadDataManager>();
			Mod.AddComponent<AddBottomBound>();
			//new 
			Mod.AddComponent<CC2RadarDisplayerData>();
			Mod.AddComponent<RadarDisplayerSimulator_MsgReceiver>();
			Mod.AddComponent<MFDMsgReceiver>();
			Mod.AddComponent<CCData>();
			Mod.AddComponent<CCDataReceiver>();
			Mod.AddComponent<CC2LoadDisplayerData>();
			Mod.AddComponent<CC2NavDisplayerData>();
			Mod.AddComponent<CustomBlockController>();
            Mod.AddComponent<AdCustomModuleMod>();
			Mod.AddComponent<H3NetworkManager>();


            AssetManager.Instance.transform.SetParent(Mod.transform);
			MessageController.Instance.transform.SetParent(Mod.transform);

			Debug.Log("Hello, this is Modern Air Combat��");
		}
        public override void OnEntityPrefabCreation(int entityId, GameObject prefab)
        {
            if (entityId == 1)
            {
                prefab.AddComponent<skpCustomModule.AdLevelBlockBehaviour>();
            }
        }
    }
}

