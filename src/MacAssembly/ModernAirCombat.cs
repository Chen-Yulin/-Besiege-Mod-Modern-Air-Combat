using System;
using Modding;
using UnityEngine;

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
			Mod = new GameObject("Morden Firearm Kit Mod");
			UnityEngine.Object.DontDestroyOnLoad(Mod);

			Mod.AddComponent<FlareMessageReciver>();
			Mod.AddComponent<MissleExploMessageReciver>();
			Mod.AddComponent<KeymsgController>();
			Mod.AddComponent<DisplayerMsgReceiver>();

			AssetManager.Instance.transform.SetParent(Mod.transform);
			MessageController.Instance.transform.SetParent(Mod.transform);

			Debug.Log("Hello, this is Modern Air Combat£¡");

			
			// Called when the mod is loaded.
		}
	}
}
