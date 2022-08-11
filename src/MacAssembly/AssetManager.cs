using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Modding;

namespace ModernAirCombat
{

    public class Asset_Trail
    {
        public GameObject FlameTrail;
        public GameObject SmokeTrail;

        public Asset_Trail(ModAssetBundle modAssetBundle)
        {
            FlameTrail = modAssetBundle.LoadAsset<GameObject>("FrameTrail");
            SmokeTrail = modAssetBundle.LoadAsset<GameObject>("SmokeTrail");
            FlameTrail.AddComponent<DestroyIfEditMode>();
            SmokeTrail.AddComponent<DestroyIfEditMode>();
            FlameTrail.SetActive(false);
            SmokeTrail.SetActive(false);
        }
    }

    public class Asset_Explo
    {
        public GameObject ExploFireball;
        public GameObject ExploDust;
        public GameObject ExploSmokeBlack;
        public GameObject ExploShower;

        public Asset_Explo(ModAssetBundle modAssetBundle)
        {
            ExploFireball = modAssetBundle.LoadAsset<GameObject>("Fireball");
            ExploDust = modAssetBundle.LoadAsset<GameObject>("Dust");
            ExploSmokeBlack = modAssetBundle.LoadAsset<GameObject>("SmokeBlack");
            ExploShower = modAssetBundle.LoadAsset<GameObject>("Shower");
            ExploFireball.AddComponent<DestroyIfEditMode>();
            ExploDust.AddComponent<DestroyIfEditMode>();
            ExploSmokeBlack.AddComponent<DestroyIfEditMode>();
            ExploShower.AddComponent<DestroyIfEditMode>();
            ExploFireball.SetActive(false);
            ExploDust.SetActive(false);
            ExploSmokeBlack.SetActive(false);
            ExploShower.SetActive(false);
            
        }
    }

    public class AssetManager : SingleInstance<AssetManager>
    {
        public override string Name { get; } = "Asset Manager";

        public Asset_Trail Trail { get; private set; }
        public Asset_Explo Explo { get; private set; }

        private void Awake()
        {
            Trail = new Asset_Trail(ModResource.GetAssetBundle("Trail Effect"));
            Explo = new Asset_Explo(ModResource.GetAssetBundle("Explo Effect"));
        }
    }
}
