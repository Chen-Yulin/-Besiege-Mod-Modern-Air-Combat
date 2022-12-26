using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Modding;

namespace ModernAirCombat
{
    public class Asset_HUD
    {
        public GameObject HUD;
        public Asset_HUD(ModAssetBundle modAssetBundle)
        {
            HUD = modAssetBundle.LoadAsset<GameObject>("HUD");
        }
    }
    public class Asset_Shader
    {
        public Shader GreenShader;
        public Shader GrayShader;
        public Shader Thermal1Shader;
        public Shader Thermal2Shader;
        public Asset_Shader(ModAssetBundle modAssetBundle)
        {
            GreenShader = modAssetBundle.LoadAsset<Shader>("Green");
            GrayShader = modAssetBundle.LoadAsset<Shader>("Gray");
            Thermal1Shader = modAssetBundle.LoadAsset<Shader>("Thermal1");
            Thermal2Shader = modAssetBundle.LoadAsset<Shader>("Thermal2");
        }
    }

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

    public class Asset_Chaff
    {
        public GameObject Chaff;
        public Asset_Chaff(ModAssetBundle modAssetBundle)
        {
            Chaff = modAssetBundle.LoadAsset<GameObject>("Chaff");
            Chaff.AddComponent<DestroyIfEditMode>();
            Chaff.SetActive(false);
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
    public class Asset_AGMExplo
    {
        public GameObject AGMExplo;

        public Asset_AGMExplo(ModAssetBundle modAssetBundle)
        {
            AGMExplo = modAssetBundle.LoadAsset<GameObject>("agmexplo");

            AGMExplo.AddComponent<DestroyIfEditMode>();

            AGMExplo.SetActive(false);


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

    public class Asset_PerformSmoke
    {
        public GameObject PerformSmoke;
        
        public Asset_PerformSmoke(ModAssetBundle modAssetBundle)
        {
            PerformSmoke = modAssetBundle.LoadAsset<GameObject>("PerformSmoke");
            PerformSmoke.AddComponent<DestroyIfEditMode>();
            PerformSmoke.SetActive(false);
        }
    }

    public class AssetManager : SingleInstance<AssetManager>
    {
        public override string Name { get; } = "Asset Manager";

        public Asset_Trail Trail { get; protected set; }
        public Asset_Explo Explo { get; protected set; }
        public Asset_AGMExplo AGMExplo { get; protected set; }
        public Asset_Flare Flare { get; protected set; }
        public Asset_Chaff Chaff { get; protected set; }
        public Asset_GunFire GunFire { get; protected set; }
        public Asset_BulletExplo BulletExplo { get; protected set; }
        public Asset_PerformSmoke PerformSmoke { get; protected set; }
        public Asset_Shader Shader { get; protected set; }

        public Asset_HUD HUD { get; protected set; }



        protected void Awake()
        {
            Trail = new Asset_Trail(ModResource.GetAssetBundle("Trail Effect"));
            Explo = new Asset_Explo(ModResource.GetAssetBundle("Explo Effect"));
            AGMExplo = new Asset_AGMExplo(ModResource.GetAssetBundle("AGMExplo Effect"));
            Flare = new Asset_Flare(ModResource.GetAssetBundle("Flare Effect"));
            Chaff = new Asset_Chaff(ModResource.GetAssetBundle("Chaff Effect"));
            GunFire = new Asset_GunFire(ModResource.GetAssetBundle("Gun Effect"));
            BulletExplo = new Asset_BulletExplo(ModResource.GetAssetBundle("Gun Effect"));
            PerformSmoke = new Asset_PerformSmoke(ModResource.GetAssetBundle("Perform Effect"));
            Shader = new Asset_Shader(ModResource.GetAssetBundle("Shader"));
            HUD = new Asset_HUD(ModResource.GetAssetBundle("HUD AB"));
        }
    }
}
