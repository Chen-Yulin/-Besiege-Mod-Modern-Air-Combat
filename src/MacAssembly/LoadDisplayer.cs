using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Collections;
using System.Linq;

using Modding.Modules;
using Modding;
using Modding.Blocks;
using UnityEngine;

using ModIO;

namespace ModernAirCombat
{
    public class LoadDataManager : SingleInstance<LoadDataManager>
    {

        public override string Name { get; } = "LoadDataManager";
        public enum WeaponType { SRAAM, MRAAM, AGM, GBU }
        public class WeaponLoad
        {
            public WeaponType weapon;
            public Transform weaponTransform;
            public bool released;
            public WeaponLoad(WeaponType Weapon, Transform transform)
            {
                weapon = Weapon;
                weaponTransform = transform;
                released = false;
            }
        }

        public Dictionary<int,Dictionary<int,WeaponLoad>> Weapons = new Dictionary<int, Dictionary<int, WeaponLoad>>();
        public Dictionary<int, int> MachineGunBullets = new Dictionary<int, int>();
        public Dictionary<int, int> FlareNum = new Dictionary<int, int>();
        public Dictionary<int, int> ChaffNum = new Dictionary<int, int>();

        public void InitLoad(int playerID)
        {
            if (!Weapons.ContainsKey(playerID))
            {
                Weapons.Add(playerID, new Dictionary<int, WeaponLoad>());
            }
        }
        public void AddLoad(int playerID, int guid, WeaponType weapon, Transform transform)
        {
            if (!Weapons.ContainsKey(playerID))
            {
                Weapons.Add(playerID, new Dictionary<int, WeaponLoad>());
            }
            if (Weapons[playerID].ContainsKey(guid))
            {
                Weapons[playerID][guid] = new WeaponLoad(weapon, transform);
            }
            else
            {
                Weapons[playerID].Add(guid, new WeaponLoad(weapon, transform));
            }

        }

        public void ReleaseLoad(int playerID, int guid)
        {
            try {
                Weapons[playerID].Remove(guid);
            } catch { }

        }

        public void ClearPlayerLoad(int playerID)
        {
            Weapons[playerID].Clear();
        }

        public void AddMachineGunBullet(int playerID, int BulletNum)
        {
            if (!MachineGunBullets.ContainsKey(playerID))
            {
                MachineGunBullets.Add(playerID,BulletNum);
            }
            else
            {
                MachineGunBullets[playerID] += BulletNum;
            }
        }
        public void ClearMachineGunBullet(int playerID)
        {
            MachineGunBullets[playerID] = 0;
        }
        public void AddFlareNum(int playerID, int flareNum)
        {
            if (!FlareNum.ContainsKey(playerID))
            {
                FlareNum.Add(playerID, flareNum);
            }
            else
            {
                FlareNum[playerID] += flareNum;
            }
        }
        public void ClearFlareNum(int playerID)
        {
            FlareNum[playerID] = 0;
        }
        public void AddChaffNum(int playerID, int chaffNum)
        {
            if (!ChaffNum.ContainsKey(playerID))
            {
                ChaffNum.Add(playerID, chaffNum);
            }
            else
            {
                ChaffNum[playerID] += chaffNum;
            }
        }
        public void ClearChaffNum(int playerID)
        {
            ChaffNum[playerID] = 0;
        }

    }
    class LoadDisplayerBlock : BlockScript
    {
        public int myPlayerID;
        public bool initialized = false;

        public Plane selfPlane;

        public MKey LaunchSRAAM;
        public MKey LaunchMRAAM;
        public MKey LaunchAGM;
        public MKey LaunchGBU;
        public MSlider Region;
        public MSlider Offset;

        public GameObject WeaponIcons;
        public GameObject MachineGunText;
        public TextMesh MachineGunMesh;
        public GameObject FlareText;
        public TextMesh FlareMesh;
        public GameObject ChaffText;
        public TextMesh ChaffMesh;

        public List<LoadDataManager.WeaponLoad> leftWingLoad = new List<LoadDataManager.WeaponLoad>();
        public List<LoadDataManager.WeaponLoad> rightWingLoad = new List<LoadDataManager.WeaponLoad>();

        public List<GameObject> leftWingLoadIcons = new List<GameObject>();
        public List<GameObject> rightWingLoadIcons = new List<GameObject>();

        public int BulletsLeft;
        public int FlareLeft;
        public int ChaffLeft;

        public Vector3 leftRoot = new Vector3(-0.03f,-0.32f,0f);
        public Vector3 leftTip = new Vector3(-1f, 0f, 0f);
        public Vector3 rightRoot = new Vector3(0.03f, -0.32f, 0f);
        public Vector3 rightTip = new Vector3(1f, 0f, 0f);

        public int leftRemain = 0;
        public int rightRemain = 0;

        public override bool EmulatesAnyKeys { get { return true; } }

        public void InitLoad() // call in simlulate update
        {
            LoadDataManager.Instance.InitLoad(myPlayerID);
            // filter the weapons out of region
            Dictionary <int,LoadDataManager.WeaponLoad> filtered = new Dictionary<int,LoadDataManager.WeaponLoad>();
            foreach (var weaponLoad in LoadDataManager.Instance.Weapons[myPlayerID])
            {
                if ((weaponLoad.Value.weaponTransform.position-transform.position).sqrMagnitude <= Mathf.Pow(Region.Value,2))
                {
                    filtered.Add(weaponLoad.Key, weaponLoad.Value);
                }
            }
            selfPlane = new Plane(transform.right, transform.position + transform.right*Offset.Value);
            var sorted = from pair in filtered orderby Mathf.Abs(selfPlane.GetDistanceToPoint(pair.Value.weaponTransform.position)) ascending select pair;
            foreach (var pair in sorted)
            {
                if (selfPlane.GetDistanceToPoint(pair.Value.weaponTransform.position)<=0)
                {
                    leftWingLoad.Add(pair.Value);

                }
                else
                {
                    rightWingLoad.Add(pair.Value);
                }
            }
            leftRemain = leftWingLoad.Count;
            rightRemain = rightWingLoad.Count;
            //Debug.Log("Left:");
            //foreach (var weapon in leftWingLoad)
            //{
            //    Debug.Log(weapon.weapon);
            //}
            //Debug.Log("right:");
            //foreach (var weapon in rightWingLoad)
            //{
            //    Debug.Log(weapon.weapon);
            //}
        }
        public void InitLoadIcon() //call in simlulate update after InitLoad()
        {
            for (int i = 0; i < leftWingLoad.Count; i++)
            {
                leftWingLoadIcons.Add(new GameObject("WeaponIcon"));
                leftWingLoadIcons[i].transform.SetParent(WeaponIcons.transform);
                Vector3 IconPosition = new Vector3();
                IconPosition = leftRoot + (leftTip - leftRoot) * (i+1) / (leftWingLoad.Count + 1);
                leftWingLoadIcons[i].transform.localPosition = IconPosition;
                leftWingLoadIcons[i].transform.localRotation = Quaternion.Euler(270, 0, 0);
                leftWingLoadIcons[i].transform.localScale = new Vector3(0.105f, 0.105f, 0.105f);
                MeshFilter leftWingLoadIconsMeshFilter = leftWingLoadIcons[i].AddComponent<MeshFilter>();
                leftWingLoadIconsMeshFilter.mesh = ModResource.GetMesh("Plane Mesh").Mesh;
                MeshRenderer leftWingLoadIconsRenderer = leftWingLoadIcons[i].AddComponent<MeshRenderer>();
                leftWingLoadIconsRenderer.material = new Material(Shader.Find("Particles/Alpha Blended"));
                switch (leftWingLoad[i].weapon)
                {
                    case LoadDataManager.WeaponType.SRAAM:
                        leftWingLoadIconsRenderer.material.SetTexture("_MainTex", ModResource.GetTexture("SRAAMLoad Texture"));
                        break;
                    case LoadDataManager.WeaponType.MRAAM:
                        leftWingLoadIconsRenderer.material.SetTexture("_MainTex", ModResource.GetTexture("MRAAMLoad Texture"));
                        break;
                    case LoadDataManager.WeaponType.AGM:
                        leftWingLoadIconsRenderer.material.SetTexture("_MainTex", ModResource.GetTexture("AGMLoad Texture"));
                        break;
                    case LoadDataManager.WeaponType.GBU:
                        leftWingLoadIconsRenderer.material.SetTexture("_MainTex", ModResource.GetTexture("GBULoad Texture"));
                        break;
                    default:
                        break;
                }
                leftWingLoadIconsRenderer.material.SetColor("_TintColor", Color.green);
                leftWingLoadIcons[i].SetActive(true);
            }
            for (int i = 0; i < rightWingLoad.Count; i++)
            {
                rightWingLoadIcons.Add(new GameObject("WeaponIcon"));
                rightWingLoadIcons[i].transform.SetParent(WeaponIcons.transform);
                Vector3 IconPosition = new Vector3();
                IconPosition = rightRoot + (rightTip - rightRoot) * (i + 1) / (rightWingLoad.Count + 1);
                rightWingLoadIcons[i].transform.localPosition = IconPosition;
                rightWingLoadIcons[i].transform.localRotation = Quaternion.Euler(270, 0, 0);
                rightWingLoadIcons[i].transform.localScale = new Vector3(0.105f, 0.105f, 0.105f);
                MeshFilter rightWingLoadIconsMeshFilter = rightWingLoadIcons[i].AddComponent<MeshFilter>();
                rightWingLoadIconsMeshFilter.mesh = ModResource.GetMesh("Plane Mesh").Mesh;
                MeshRenderer rightWingLoadIconsRenderer = rightWingLoadIcons[i].AddComponent<MeshRenderer>();
                rightWingLoadIconsRenderer.material = new Material(Shader.Find("Particles/Alpha Blended"));
                switch (rightWingLoad[i].weapon)
                {
                    case LoadDataManager.WeaponType.SRAAM:
                        rightWingLoadIconsRenderer.material.SetTexture("_MainTex", ModResource.GetTexture("SRAAMLoad Texture"));
                        break;
                    case LoadDataManager.WeaponType.MRAAM:
                        rightWingLoadIconsRenderer.material.SetTexture("_MainTex", ModResource.GetTexture("MRAAMLoad Texture"));
                        break;
                    case LoadDataManager.WeaponType.AGM:
                        rightWingLoadIconsRenderer.material.SetTexture("_MainTex", ModResource.GetTexture("AGMLoad Texture"));
                        break;
                    case LoadDataManager.WeaponType.GBU:
                        rightWingLoadIconsRenderer.material.SetTexture("_MainTex", ModResource.GetTexture("GBULoad Texture"));
                        break;
                    default:
                        break;
                }
                rightWingLoadIconsRenderer.material.SetColor("_TintColor", Color.green);
                rightWingLoadIcons[i].SetActive(true);
            }
        } 
        public void InitPanel()
        {
            if (!transform.FindChild("Wing"))
            {
                GameObject Wing = new GameObject("Wing");
                Wing.transform.SetParent(transform);
                Wing.transform.localPosition = new Vector3(0f, 0f, 0.095f);
                Wing.transform.localRotation = Quaternion.Euler(270, 0, 180);
                Wing.transform.localScale = new Vector3(0.105f, 0.105f, 0.105f);
                MeshFilter WingMeshFilter = Wing.AddComponent<MeshFilter>();
                WingMeshFilter.mesh = ModResource.GetMesh("Plane Mesh").Mesh;
                MeshRenderer WingRenderer = Wing.AddComponent<MeshRenderer>();
                WingRenderer.material = new Material(Shader.Find("Particles/Alpha Blended"));
                WingRenderer.material.SetTexture("_MainTex", ModResource.GetTexture("LoadWing Texture"));
                WingRenderer.material.SetColor("_TintColor", Color.green);
                Wing.SetActive(true);
            }
            if (!transform.FindChild("WeaponIcons"))
            {
                WeaponIcons = new GameObject("WeaponIcons");
                WeaponIcons.transform.SetParent(transform);
                WeaponIcons.transform.localPosition = new Vector3(0f, 0.01f, 0.095f);
                WeaponIcons.transform.localRotation = Quaternion.Euler(0, 0, 0);
                WeaponIcons.transform.localScale = new Vector3(0.11f, 0.11f, 0.11f);
            }
            if (!transform.FindChild("MachineGunText"))
            {
                MachineGunText = new GameObject("MachineGunText");
                MachineGunText.transform.SetParent(transform);
                MachineGunText.transform.localPosition = new Vector3(-0.06f, 0.05f, 0.095f);
                MachineGunText.transform.localRotation = Quaternion.Euler(180, 0, 0);
                MachineGunText.transform.localScale = new Vector3(0.003f, 0.003f, 0.003f);
                MachineGunMesh = MachineGunText.AddComponent<TextMesh>();
                MachineGunMesh.text = "Bullets: ";
                MachineGunMesh.color = Color.green;
                MachineGunMesh.characterSize = 0.7f;
                MachineGunMesh.fontSize = 64;
                MachineGunMesh.fontStyle = FontStyle.Normal;
                MachineGunMesh.anchor = TextAnchor.MiddleLeft;
                MachineGunText.SetActive(false);
            }
            if (!transform.FindChild("FlareText"))
            {
                FlareText = new GameObject("FlareText");
                FlareText.transform.SetParent(transform);
                FlareText.transform.localPosition = new Vector3(-0.06f, 0.07f, 0.095f);
                FlareText.transform.localRotation = Quaternion.Euler(180, 0, 0);
                FlareText.transform.localScale = new Vector3(0.003f, 0.003f, 0.003f);
                FlareMesh = FlareText.AddComponent<TextMesh>();
                FlareMesh.text = "Flares: ";
                FlareMesh.color = Color.green;
                FlareMesh.characterSize = 0.7f;
                FlareMesh.fontSize = 64;
                FlareMesh.fontStyle = FontStyle.Normal;
                FlareMesh.anchor = TextAnchor.MiddleLeft;
                FlareText.SetActive(false);
            }
            if (!transform.FindChild("ChaffText"))
            {
                ChaffText = new GameObject("ChaffText");
                ChaffText.transform.SetParent(transform);
                ChaffText.transform.localPosition = new Vector3(-0.06f, 0.09f, 0.095f);
                ChaffText.transform.localRotation = Quaternion.Euler(180, 0, 0);
                ChaffText.transform.localScale = new Vector3(0.003f, 0.003f, 0.003f);
                ChaffMesh = ChaffText.AddComponent<TextMesh>();
                ChaffMesh.text = "Chaff: ";
                ChaffMesh.color = Color.green;
                ChaffMesh.characterSize = 0.7f;
                ChaffMesh.fontSize = 64;
                ChaffMesh.fontStyle = FontStyle.Normal;
                ChaffMesh.anchor = TextAnchor.MiddleLeft;
                ChaffText.SetActive(false);
            }
        }

        public void LaunchWeapon() // before updateLoadAndIcon
        {
            //sraam
            if (LaunchSRAAM.IsPressed)
            {
                if (leftRemain>=rightRemain)
                {
                    if (leftRemain == 0)
                    {
                        return;
                    }
                    for (int i = leftWingLoad.Count-1; i >= 0; i--)
                    {
                        if (leftWingLoad[i].weapon == LoadDataManager.WeaponType.SRAAM && leftWingLoad[i].released == false)
                        {
                            base.EmulateKeys(new MKey[0], leftWingLoad[i].weaponTransform.gameObject.GetComponent<SRAAMBlock>().Launch, true);
                            return;
                        }
                    }
                    if (rightRemain == 0)
                    {
                        return;
                    }
                    for (int i = rightWingLoad.Count - 1; i >= 0; i--)
                    {
                        if (rightWingLoad[i].weapon == LoadDataManager.WeaponType.SRAAM && rightWingLoad[i].released == false)
                        {
                            base.EmulateKeys(new MKey[0], rightWingLoad[i].weaponTransform.gameObject.GetComponent<SRAAMBlock>().Launch, true);
                            return;
                        }
                    }
                }
                else
                {
                    if (rightRemain == 0)
                    {
                        return;
                    }
                    for (int i = rightWingLoad.Count-1; i >= 0; i--)
                    {
                        if (rightWingLoad[i].weapon == LoadDataManager.WeaponType.SRAAM && rightWingLoad[i].released == false)
                        {
                            base.EmulateKeys(new MKey[0], rightWingLoad[i].weaponTransform.gameObject.GetComponent<SRAAMBlock>().Launch, true);
                            return;
                        }
                    }
                    if (leftRemain == 0)
                    {
                        return;
                    }
                    for (int i = leftWingLoad.Count - 1; i >= 0; i--)
                    {
                        if (leftWingLoad[i].weapon == LoadDataManager.WeaponType.SRAAM && leftWingLoad[i].released == false)
                        {
                            base.EmulateKeys(new MKey[0], leftWingLoad[i].weaponTransform.gameObject.GetComponent<SRAAMBlock>().Launch, true);
                            return;
                        }
                    }
                }
            }
            //mraam
            if (LaunchMRAAM.IsPressed)
            {
                if (leftRemain >= rightRemain)
                {
                    if (leftRemain == 0)
                    {
                        return;
                    }
                    for (int i = leftWingLoad.Count-1; i >= 0; i--)
                    {
                        if (leftWingLoad[i].weapon == LoadDataManager.WeaponType.MRAAM && leftWingLoad[i].released == false)
                        {
                            base.EmulateKeys(new MKey[0], leftWingLoad[i].weaponTransform.gameObject.GetComponent<MRAAMBlock>().Launch, true);
                            return;
                        }
                    }
                    if (rightRemain == 0)
                    {
                        return;
                    }
                    for (int i = rightWingLoad.Count - 1; i >= 0; i--)
                    {
                        if (rightWingLoad[i].weapon == LoadDataManager.WeaponType.MRAAM && rightWingLoad[i].released == false)
                        {
                            base.EmulateKeys(new MKey[0], rightWingLoad[i].weaponTransform.gameObject.GetComponent<MRAAMBlock>().Launch, true);
                            return;
                        }
                    }
                }
                else
                {
                    if (rightRemain == 0)
                    {
                        return;
                    }
                    for (int i = rightWingLoad.Count-1; i >= 0; i--)
                    {
                        if (rightWingLoad[i].weapon == LoadDataManager.WeaponType.MRAAM && rightWingLoad[i].released == false)
                        {
                            base.EmulateKeys(new MKey[0], rightWingLoad[i].weaponTransform.gameObject.GetComponent<MRAAMBlock>().Launch, true);
                            return;
                        }
                    }
                    if (leftRemain == 0)
                    {
                        return;
                    }
                    for (int i = leftWingLoad.Count - 1; i >= 0; i--)
                    {
                        if (leftWingLoad[i].weapon == LoadDataManager.WeaponType.MRAAM && leftWingLoad[i].released == false)
                        {
                            base.EmulateKeys(new MKey[0], leftWingLoad[i].weaponTransform.gameObject.GetComponent<MRAAMBlock>().Launch, true);
                            return;
                        }
                    }
                }
            }
            //agm
            if (LaunchAGM.IsPressed)
            {
                if (leftRemain >= rightRemain)
                {
                    if (leftRemain == 0)
                    {
                        return;
                    }
                    for (int i = leftWingLoad.Count-1; i >= 0; i--)
                    {
                        if (leftWingLoad[i].weapon == LoadDataManager.WeaponType.AGM && leftWingLoad[i].released == false)
                        {
                            base.EmulateKeys(new MKey[0], leftWingLoad[i].weaponTransform.gameObject.GetComponent<AGMBlock>().Launch, true);
                            return;
                        }
                    }
                    if (rightRemain == 0)
                    {
                        return;
                    }
                    for (int i = rightWingLoad.Count - 1; i >= 0; i--)
                    {
                        if (rightWingLoad[i].weapon == LoadDataManager.WeaponType.AGM && rightWingLoad[i].released == false)
                        {
                            base.EmulateKeys(new MKey[0], rightWingLoad[i].weaponTransform.gameObject.GetComponent<AGMBlock>().Launch, true);
                            return;
                        }
                    }
                }
                else
                {
                    if (rightRemain == 0)
                    {
                        return;
                    }
                    for (int i = rightWingLoad.Count-1; i >= 0; i--)
                    {
                        if (rightWingLoad[i].weapon == LoadDataManager.WeaponType.AGM && rightWingLoad[i].released == false)
                        {
                            base.EmulateKeys(new MKey[0], rightWingLoad[i].weaponTransform.gameObject.GetComponent<AGMBlock>().Launch, true);
                            return;
                        }
                    }
                    if (leftRemain == 0)
                    {
                        return;
                    }
                    for (int i = leftWingLoad.Count - 1; i >= 0; i--)
                    {
                        if (leftWingLoad[i].weapon == LoadDataManager.WeaponType.AGM && leftWingLoad[i].released == false)
                        {
                            base.EmulateKeys(new MKey[0], leftWingLoad[i].weaponTransform.gameObject.GetComponent<AGMBlock>().Launch, true);
                            return;
                        }
                    }
                }
            }
            //gbu
            if (LaunchGBU.IsPressed)
            {
                if (leftRemain >= rightRemain)
                {
                    if (leftRemain == 0)
                    {
                        return;
                    }
                    for (int i = leftWingLoad.Count-1; i >= 0; i--)
                    {
                        if (leftWingLoad[i].weapon == LoadDataManager.WeaponType.GBU && leftWingLoad[i].released == false)
                        {
                            base.EmulateKeys(new MKey[0], leftWingLoad[i].weaponTransform.gameObject.GetComponent<GuidedBombBlock>().Launch, true);
                            return;
                        }
                    }
                    if (rightRemain == 0)
                    {
                        return;
                    }
                    for (int i = rightWingLoad.Count - 1; i >= 0; i--)
                    {
                        if (rightWingLoad[i].weapon == LoadDataManager.WeaponType.GBU && rightWingLoad[i].released == false)
                        {
                            base.EmulateKeys(new MKey[0], rightWingLoad[i].weaponTransform.gameObject.GetComponent<GuidedBombBlock>().Launch, true);
                            return;
                        }
                    }
                }
                else
                {
                    if (rightRemain == 0)
                    {
                        return;
                    }
                    for (int i = rightWingLoad.Count-1; i >= 0; i--)
                    {
                        if (rightWingLoad[i].weapon == LoadDataManager.WeaponType.GBU && rightWingLoad[i].released == false)
                        {
                            base.EmulateKeys(new MKey[0], rightWingLoad[i].weaponTransform.gameObject.GetComponent<GuidedBombBlock>().Launch, true);
                            return;
                        }
                    }
                    if (leftRemain == 0)
                    {
                        return;
                    }
                    for (int i = leftWingLoad.Count - 1; i >= 0; i--)
                    {
                        if (leftWingLoad[i].weapon == LoadDataManager.WeaponType.GBU && leftWingLoad[i].released == false)
                        {
                            base.EmulateKeys(new MKey[0], leftWingLoad[i].weaponTransform.gameObject.GetComponent<GuidedBombBlock>().Launch, true);
                            return;
                        }
                    }

                }

            }
        }
        public void updateLoadAndIcon()
        {
            for (int i = 0; i < leftWingLoad.Count; i++)
            {
                SRAAMBlock.status currStatus = SRAAMBlock.status.stored;
                switch (leftWingLoad[i].weapon)
                {
                    case LoadDataManager.WeaponType.SRAAM:
                        currStatus = leftWingLoad[i].weaponTransform.GetComponent<SRAAMBlock>().myStatus;
                        break;
                    case LoadDataManager.WeaponType.MRAAM:
                        currStatus = leftWingLoad[i].weaponTransform.GetComponent<MRAAMBlock>().myStatus;
                        break;
                    case LoadDataManager.WeaponType.AGM:
                        currStatus = leftWingLoad[i].weaponTransform.GetComponent<AGMBlock>().myStatus;
                        break;
                    case LoadDataManager.WeaponType.GBU:
                        currStatus = leftWingLoad[i].weaponTransform.GetComponent<GuidedBombBlock>().myStatus;
                        break;
                    default:
                        break;
                }
                if (currStatus != SRAAMBlock.status.stored)
                {
                    leftWingLoadIcons[i].SetActive(false);
                    if (leftWingLoad[i].released == false)
                    {
                        leftWingLoad[i].released = true;
                        leftRemain--;
                    }
                }

            }
            for (int i = 0; i < rightWingLoad.Count; i++)
            {
                SRAAMBlock.status currStatus = SRAAMBlock.status.stored;
                switch (rightWingLoad[i].weapon)
                {
                    case LoadDataManager.WeaponType.SRAAM:
                        currStatus = rightWingLoad[i].weaponTransform.GetComponent<SRAAMBlock>().myStatus;
                        break;
                    case LoadDataManager.WeaponType.MRAAM:
                        currStatus = rightWingLoad[i].weaponTransform.GetComponent<MRAAMBlock>().myStatus;
                        break;
                    case LoadDataManager.WeaponType.AGM:
                        currStatus = rightWingLoad[i].weaponTransform.GetComponent<AGMBlock>().myStatus;
                        break;
                    case LoadDataManager.WeaponType.GBU:
                        currStatus = rightWingLoad[i].weaponTransform.GetComponent<GuidedBombBlock>().myStatus;
                        break;
                    default:
                        break;
                }
                if (currStatus != SRAAMBlock.status.stored)
                {
                    rightWingLoadIcons[i].SetActive(false);
                    if (rightWingLoad[i].released == false)
                    {
                        rightWingLoad[i].released = true;
                        rightRemain--;
                    }
                }

            }
        }
        public void updateMachineGunBullets()// call in LateUpdate
        {
            BulletsLeft = Mathf.Clamp(LoadDataManager.Instance.MachineGunBullets[myPlayerID],0,99999);
            LoadDataManager.Instance.ClearMachineGunBullet(myPlayerID);
        }
        public void updateFlareNum()// call in LateUpdate
        {
            FlareLeft = Mathf.Clamp(LoadDataManager.Instance.FlareNum[myPlayerID], 0, 99999);
            LoadDataManager.Instance.ClearFlareNum(myPlayerID);
        }
        public void updateChaffNum()// call in LateUpdate
        {
            ChaffLeft = Mathf.Clamp(LoadDataManager.Instance.ChaffNum[myPlayerID], 0, 99999);
            LoadDataManager.Instance.ClearChaffNum(myPlayerID);
        }
        public override void SafeAwake()
        {
            LaunchSRAAM = AddKey("Launch SRAAM", "LaunchSRAAM", KeyCode.Alpha1);
            LaunchMRAAM = AddKey("Launch MRAAM", "LaunchMRAAM", KeyCode.Alpha2);
            LaunchAGM = AddKey("Launch AGM", "LaunchAGM", KeyCode.Alpha3);
            LaunchGBU = AddKey("Launch GBU", "LaunchGBU", KeyCode.Alpha4);
            Region = AddSlider("Region", "Region", 20f, 0f, 40f);
            Offset = AddSlider("Offset", "Offset", 0f, -2f,2f);
            myPlayerID = BlockBehaviour.ParentMachine.PlayerID;
            InitPanel();
        }
        public void Start() { }

        protected void Update()
        {

        }
        public override void OnSimulateStart()
        {
            MachineGunText.SetActive(true);
            FlareText.SetActive(true);
            ChaffText.SetActive(true);
        }

        public override void OnSimulateStop()
        {
            initialized = false;
            leftWingLoad.Clear();
            rightWingLoad.Clear();
        }

        public override void SimulateUpdateClient()
        {
            
        }
        public override void SimulateUpdateHost()
        {
            LaunchWeapon();
        }
        public override void SimulateFixedUpdateAlways()
        {
            if (!initialized)// call one time after OnsimulateStart
            {
                initialized = true;
                InitLoad();
                InitLoadIcon();
            }
            updateLoadAndIcon();
        }
        public override void SimulateLateUpdateAlways()
        {
            try
            {
                updateMachineGunBullets();
                MachineGunMesh.text = "Bullets:     " + BulletsLeft.ToString();
            }
            catch {
                MachineGunMesh.text = "Bullets:     x";
            }
            try
            {
                updateFlareNum();
                FlareMesh.text = "Flares:      " + FlareLeft.ToString();
            }
            catch
            {
                FlareMesh.text = "Flares:      x";
            }
            try
            {
                updateChaffNum();
                ChaffMesh.text = "Chaff:       " + ChaffLeft.ToString();
            }
            catch
            {
                ChaffMesh.text = "Chaff:       x";
            }

        }

        void OnGUI()
        {
            //GUI.Box(new Rect(100, 200, 200, 50), leftRemain.ToString());
            //GUI.Box(new Rect(100, 300, 200, 50), rightRemain.ToString());
        }
    }

}
