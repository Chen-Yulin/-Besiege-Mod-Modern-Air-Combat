﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Modding;

namespace ModernAirCombat
{
    public class Asset_Flare
    {
        public GameObject FlameFlare;
        public GameObject SmokeFlare;
        public Asset_Flare (ModAssetBundle modAssetBundle)
        {
            FlameFlare = modAssetBundle.LoadAsset<GameObject>("FlareFrame");
            SmokeFlare = modAssetBundle.LoadAsset<GameObject>("FlareSmoke");
            FlameFlare.AddComponent<DestroyIfEditMode>();
            SmokeFlare.AddComponent<DestroyIfEditMode>();
            FlameFlare.SetActive(false);
            SmokeFlare.SetActive(false);
            
        }
    }

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
        public GameObject Explo;

        public Asset_Explo(ModAssetBundle modAssetBundle)
        {
            Explo = modAssetBundle.LoadAsset<GameObject>("explosion");

            Explo.AddComponent<DestroyIfEditMode>();

            Explo.SetActive(false);

            
        }
    }

    public class Asset_GunFire
    {
        public GameObject GunFire;
        public Asset_GunFire(ModAssetBundle modAssetBundle)
        {
            GunFire = modAssetBundle.LoadAsset<GameObject>("GunFire");
            GunFire.AddComponent<DestroyIfEditMode>();
            GunFire.SetActive(false);
        }
    }

    public class Asset_BulletExplo
    {
        public GameObject BulletExplo;
        public Asset_BulletExplo(ModAssetBundle modAssetBundle)
        {
            BulletExplo = modAssetBundle.LoadAsset<GameObject>("bulletExplo");
            BulletExplo.AddComponent<DestroyIfEditMode>();
            BulletExplo.SetActive(false);
        }
    }

    public class AssetManager : SingleInstance<AssetManager>
    {
        public override string Name { get; } = "Asset Manager";

        public Asset_Trail Trail { get; protected set; }
        public Asset_Explo Explo { get; protected set; }
        public Asset_Flare Flare { get; protected set; }
        public Asset_GunFire GunFire { get; protected set; }
        public Asset_BulletExplo BulletExplo { get; protected set; }



        protected void Awake()
        {
            Trail = new Asset_Trail(ModResource.GetAssetBundle("Trail Effect"));
            Explo = new Asset_Explo(ModResource.GetAssetBundle("Explo Effect"));
            Flare = new Asset_Flare(ModResource.GetAssetBundle("Flare Effect"));
            GunFire = new Asset_GunFire(ModResource.GetAssetBundle("Gun Effect"));
            BulletExplo = new Asset_BulletExplo(ModResource.GetAssetBundle("Gun Effect"));

        }
    }
}
