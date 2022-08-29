using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections;
using System.Numerics;

using Modding.Modules;
using Modding;
using Modding.Blocks;
using UnityEngine;

namespace ModernAirCombat
{
    class RWRMsgReceiver : SingleInstance<RWRMsgReceiver>
    {
        public override string Name { get; } = "RWRMsgReceiver";

        public float[,] RWRData = new float[16, 8];

        public void DataReceiver(Message msg)
        {
            int playerID = (int)msg.GetData(0);
            for (int i = 0; i < 8; i++)
            {
                RWRData[playerID, i] = (float)msg.GetData(i + 1);
            }
        }

    }

    public class MakeAudioSourceFixedPitch : MonoBehaviour
    {
        protected AudioSource FixedAS;
        protected void Start()
        {
            FixedAS = base.GetComponent<AudioSource>();
        }
        protected void Update()
        {
            FixedAS.pitch = Time.timeScale;
        }
    }

    class RWRBlock : BlockScript
    {
        public MSlider Volume;

        public AudioClip BeepClip;
        public AudioSource BeepAS;
        public GameObject[] Icon;

        IEnumerator sendData;

        public int myPlayerID;
        //protected bool hasRadiation;

        public static MessageType ClientRWRData = ModNetworking.CreateMessageType( DataType.Integer,
                                                                            DataType.Single, DataType.Single, DataType.Single, DataType.Single,
                                                                            DataType.Single, DataType.Single, DataType.Single, DataType.Single);


        IEnumerator SendData()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.05f);
                try
                {
                    ModNetworking.SendToAll(ClientRWRData.CreateMessage(myPlayerID, DataManager.Instance.RWRData[myPlayerID, 0],
                                                                                    DataManager.Instance.RWRData[myPlayerID, 1],
                                                                                    DataManager.Instance.RWRData[myPlayerID, 2],
                                                                                    DataManager.Instance.RWRData[myPlayerID, 3],
                                                                                    DataManager.Instance.RWRData[myPlayerID, 4],
                                                                                    DataManager.Instance.RWRData[myPlayerID, 5],
                                                                                    DataManager.Instance.RWRData[myPlayerID, 6],
                                                                                    DataManager.Instance.RWRData[myPlayerID, 7]
                                                                                    ));
                }
                catch { }
                
            }
        }

        public void InitBeep()
        {
            BeepClip = ModResource.GetAudioClip("RWRBeep Audio").AudioClip;
            BeepAS = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
            gameObject.AddComponent<MakeAudioSourceFixedPitch>();
            BeepAS.clip = BeepClip;
            BeepAS.spatialBlend = 1.0f;
            BeepAS.volume = Volume.Value;

            BeepAS.SetSpatializerFloat(1, 1f);
            BeepAS.SetSpatializerFloat(2, 0);
            BeepAS.SetSpatializerFloat(3, 12);
            BeepAS.SetSpatializerFloat(4, 1000f);
            BeepAS.SetSpatializerFloat(5, 1f);

        }

        public void InitIcon()
        {
            if (!transform.FindChild("RWR Icon"))
            {
                TextMesh text;

                Icon = new GameObject[8];
                Icon[0] = new GameObject("RWR Icon");
                Icon[0].transform.SetParent(transform);
                Icon[0].transform.localPosition = new Vector3(0.00f, 0.06f, 0.04f);
                Icon[0].transform.localRotation = Quaternion.Euler(0, 0, 270);
                Icon[0].transform.localScale = 0.03f*Vector3.one;
                text = Icon[0].AddComponent<TextMesh>();
                text.text = "((";
                text.color = Color.green;
                text.characterSize = 0.25f;
                text.fontSize = 64;
                text.fontStyle = FontStyle.Bold;
                text.anchor = TextAnchor.MiddleCenter;
                Icon[0].SetActive(false);

                Icon[1] = new GameObject("RWR Icon");
                Icon[1].transform.SetParent(transform);
                Icon[1].transform.localPosition = new Vector3(-0.042f, 0.042f, 0.04f);
                Icon[1].transform.localRotation = Quaternion.Euler(0, 0, 322);
                Icon[1].transform.localScale = 0.03f * Vector3.one;
                text = Icon[1].AddComponent<TextMesh>();
                text.text = "((";
                text.color = Color.green;
                text.characterSize = 0.25f;
                text.fontSize = 64;
                text.fontStyle = FontStyle.Bold;
                text.anchor = TextAnchor.MiddleCenter;
                Icon[1].SetActive(false);

                Icon[2] = new GameObject("RWR Icon");
                Icon[2].transform.SetParent(transform);
                Icon[2].transform.localPosition = new Vector3(-0.06f, 0.0f, 0.04f);
                Icon[2].transform.localRotation = Quaternion.Euler(0, 0, 0);
                Icon[2].transform.localScale = 0.03f * Vector3.one;
                text = Icon[2].AddComponent<TextMesh>();
                text.text = "((";
                text.color = Color.green;
                text.characterSize = 0.25f;
                text.fontSize = 64;
                text.fontStyle = FontStyle.Bold;
                text.anchor = TextAnchor.MiddleCenter;
                Icon[2].SetActive(false);

                Icon[3] = new GameObject("RWR Icon");
                Icon[3].transform.SetParent(transform);
                Icon[3].transform.localPosition = new Vector3(-0.042f, -0.042f, 0.04f);
                Icon[3].transform.localRotation = Quaternion.Euler(0, 0, 52);
                Icon[3].transform.localScale = 0.03f * Vector3.one;
                text = Icon[3].AddComponent<TextMesh>();
                text.text = "((";
                text.color = Color.green;
                text.characterSize = 0.25f;
                text.fontSize = 64;
                text.fontStyle = FontStyle.Bold;
                text.anchor = TextAnchor.MiddleCenter;
                Icon[3].SetActive(false);

                Icon[4] = new GameObject("RWR Icon");
                Icon[4].transform.SetParent(transform);
                Icon[4].transform.localPosition = new Vector3(0.0f, -0.06f, 0.04f);
                Icon[4].transform.localRotation = Quaternion.Euler(0, 0, 90);
                Icon[4].transform.localScale = 0.03f * Vector3.one;
                text = Icon[4].AddComponent<TextMesh>();
                text.text = "((";
                text.color = Color.green;
                text.characterSize = 0.25f;
                text.fontSize = 64;
                text.fontStyle = FontStyle.Bold;
                text.anchor = TextAnchor.MiddleCenter;
                Icon[4].SetActive(false);

                Icon[5] = new GameObject("RWR Icon");
                Icon[5].transform.SetParent(transform);
                Icon[5].transform.localPosition = new Vector3(0.042f, -0.042f, 0.04f);
                Icon[5].transform.localRotation = Quaternion.Euler(0, 0, 142);
                Icon[5].transform.localScale = 0.03f * Vector3.one;
                text = Icon[5].AddComponent<TextMesh>();
                text.text = "((";
                text.color = Color.green;
                text.characterSize = 0.25f;
                text.fontSize = 64;
                text.fontStyle = FontStyle.Bold;
                text.anchor = TextAnchor.MiddleCenter;
                Icon[5].SetActive(false);

                Icon[6] = new GameObject("RWR Icon");
                Icon[6].transform.SetParent(transform);
                Icon[6].transform.localPosition = new Vector3(0.06f, 0.0f, 0.04f);
                Icon[6].transform.localRotation = Quaternion.Euler(0, 0, 180);
                Icon[6].transform.localScale = 0.03f * Vector3.one;
                text = Icon[6].AddComponent<TextMesh>();
                text.text = "((";
                text.color = Color.green;
                text.characterSize = 0.25f;
                text.fontSize = 64;
                text.fontStyle = FontStyle.Bold;
                text.anchor = TextAnchor.MiddleCenter;
                Icon[6].SetActive(false);

                Icon[7] = new GameObject("RWR Icon");
                Icon[7].transform.SetParent(transform);
                Icon[7].transform.localPosition = new Vector3(0.042f, 0.042f, 0.04f);
                Icon[7].transform.localRotation = Quaternion.Euler(0, 0, 232);
                Icon[7].transform.localScale = 0.03f * Vector3.one;
                text = Icon[7].AddComponent<TextMesh>();
                text.text = "((";
                text.color = Color.green;
                text.characterSize = 0.25f;
                text.fontSize = 64;
                text.fontStyle = FontStyle.Bold;
                text.anchor = TextAnchor.MiddleCenter;
                Icon[7].SetActive(false);

            }
        }
        
        public override void SafeAwake()
        {
            myPlayerID = BlockBehaviour.ParentMachine.PlayerID;

            Volume = AddSlider("Volume", "volume", 0.4f, 0.1f, 1f);

            
        }
        public void Start()
        { }
        public override void OnSimulateStart()
        {
            InitBeep();
            InitIcon();

            sendData = SendData();
            StartCoroutine(sendData);
        }
        public override void OnSimulateStop()
        {
            StopCoroutine(sendData);
        }

        public override void SimulateUpdateHost()
        {
            //hasRadiation = false;
            try
            {
                for (int i = 0; i < 8; i++)
                {
                    if (DataManager.Instance.RWRData[myPlayerID, i] > 0)
                    {
                        if (DataManager.Instance.RWRData[myPlayerID, i] == 1)
                        {
                            BeepAS.Play();
                        }
                        if (!Icon[i].activeSelf)
                        {
                            Icon[i].SetActive(true);
                        }
                        DataManager.Instance.RWRData[myPlayerID, i] -= Time.deltaTime;
                    }
                    else
                    {
                        if (Icon[i].activeSelf)
                        {
                            Icon[i].SetActive(false);
                        }
                        DataManager.Instance.RWRData[myPlayerID, i] = 0;
                    }
                }
            }
            catch { }
        }

        public override void SimulateUpdateClient()
        {
            try
            {
                for (int i = 0; i < 8; i++)
                {
                    if (RWRMsgReceiver.Instance.RWRData[myPlayerID, i] > 0)
                    {
                        if (RWRMsgReceiver.Instance.RWRData[myPlayerID, i] >=0.9 && !BeepAS.isPlaying)
                        {
                            BeepAS.Play();
                        }
                        if (!Icon[i].activeSelf)
                        {
                            Icon[i].SetActive(true);
                        }
                        RWRMsgReceiver.Instance.RWRData[myPlayerID, i] -= Time.deltaTime;
                    }
                    else
                    {
                        if (Icon[i].activeSelf)
                        {
                            Icon[i].SetActive(false);
                        }
                        RWRMsgReceiver.Instance.RWRData[myPlayerID, i] = 0;
                    }
                }
            }
            catch { }
        }


        void OnGUI()
        {
            //GUI.Box(new Rect(100, 100, 200, 50), DataManager.Instance.RWRData[playerID, 0].ToString());
            //GUI.Box(new Rect(100, 150, 200, 50), DataManager.Instance.RWRData[playerID, 1].ToString());
            //GUI.Box(new Rect(100, 200, 200, 50), DataManager.Instance.RWRData[playerID, 2].ToString());
            //GUI.Box(new Rect(100, 250, 200, 50), DataManager.Instance.RWRData[playerID, 3].ToString());
            //GUI.Box(new Rect(100, 300, 200, 50), DataManager.Instance.RWRData[playerID, 4].ToString());
            //GUI.Box(new Rect(100, 350, 200, 50), DataManager.Instance.RWRData[playerID, 5].ToString());
            //GUI.Box(new Rect(100, 400, 200, 50), DataManager.Instance.RWRData[playerID, 6].ToString());
            //GUI.Box(new Rect(100, 450, 200, 50), DataManager.Instance.RWRData[playerID, 7].ToString());
        }

    }
}
