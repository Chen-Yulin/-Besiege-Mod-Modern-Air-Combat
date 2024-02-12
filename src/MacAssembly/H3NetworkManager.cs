using Modding;
using Modding.Common;
using skpCustomModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Net.NetworkInformation;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;

namespace Navalmod
{
    public struct byteAndBB
    {
        public byteAndBB(BlockBehaviour blockBehaviour, int b)
        {
            bb = blockBehaviour;
            offset = b;
        }
        public BlockBehaviour bb;
        public int offset;
    }

    public class ClusterSend
    {
        public List<H3NetCluster> clusterList;
    }
    public class DesScale
    {

    }

    public class H3NetCluster
    {
        public H3NetCluster(H3NetBlock Basebb)
        {
            Base = Basebb;
        }
        public List<H3NetBlock> blocks;
        public H3NetBlock Base;
        public ushort playerid;

        public static List<H3NetCluster> GetAllNetCluster()
        {
            List<H3NetCluster> h3NetClusters = new List<H3NetCluster>();
            foreach (PlayerData playerData in Playerlist.Players)
            {
                h3NetClusters.AddRange(GetPlayerNetCluster(playerData.networkId));

            }
            return h3NetClusters;
        }
        public static BlockBehaviour searchNewBase(Machine.SimCluster simCluster)
        {
            foreach (BlockBehaviour blockBehaviour in simCluster.Blocks)
            {
            }
            return simCluster.Blocks[0];
        }
        public static List<H3NetCluster> GetPlayerNetCluster(ushort playerid)
        {
            List<H3NetCluster> h3NetClusters = new List<H3NetCluster>();
            PlayerData playerData = Playerlist.GetPlayer(playerid);
            foreach (Machine.SimCluster simCluster in playerData.machine.simClusters)
            {
                try
                {
                    List<H3NetBlock> h3NetBlocks = H3NetBlock.BBstoH3NetBBs(simCluster.Blocks, simCluster.Base);
                    H3NetCluster h3NetCluster = new H3NetCluster(new H3NetBlock(simCluster.Base));
                    h3NetCluster.blocks = h3NetBlocks;
                    h3NetCluster.playerid = playerid;
                    h3NetClusters.Add(h3NetCluster);
                }
                catch
                {

                }
            }
            return h3NetClusters;
        }
        public void ApplyToMachine()
        {
            BlockBehaviour baseBB = Base.GetSetRealBB();
            GameObject gameObject = new GameObject(baseBB.transform.name + "Base");
            H3NetworkManager.Instance.clusterlist.Add(gameObject);
            gameObject.transform.position = baseBB.transform.position;
            gameObject.transform.rotation = baseBB.transform.rotation;

            H3NetworkBlock h3NetworkBlock = baseBB.GetComponent<H3NetworkBlock>();
            h3NetworkBlock.isClusterBase = true;
            h3NetworkBlock.islocal = true;
            h3NetworkBlock.lastpos = Vector3.zero;
            h3NetworkBlock.lastqua = new Quaternion(0f, 0f, 0f, 0f);
            h3NetworkBlock.nowpos = Vector3.zero;
            h3NetworkBlock.nowqua = new Quaternion(0f, 0f, 0f, 0f);
            baseBB.transform.SetParent(gameObject.transform, false);

            Base.GetSetRealBB();

            foreach (H3NetBlock h3NetBlock in blocks)
            {
                BlockBehaviour bb = h3NetBlock.GetSetRealBB();
                bb.transform.SetParent(gameObject.transform, false);
                h3NetBlock.GetSetRealBB();
            }
            gameObject.transform.parent = baseBB.ParentMachine.SimulationMachine;
        }
    }
    public class H3NetBlock
    {
        public H3NetBlock(BlockBehaviour blockBehaviour)
        {
            guid = blockBehaviour.BuildingBlock.Guid;
            playerid = blockBehaviour.ParentMachine.PlayerID;
            pos = blockBehaviour.transform.position;
            rot = blockBehaviour.transform.rotation;
            scale = blockBehaviour.transform.localScale;
        }
        public H3NetBlock()
        {
        }
        public Guid guid;
        public ushort playerid;
        public Vector3 pos;
        public Quaternion rot;
        public Vector3 scale;
        public static List<H3NetBlock> BBstoH3NetBBs(BlockBehaviour[] blockBehaviours, BlockBehaviour basebb)
        {

            List<H3NetBlock> h3NetBlocks = new List<H3NetBlock>();
            foreach (BlockBehaviour blockBehaviour in blockBehaviours)
            {
                try
                {
                    if (blockBehaviour != basebb)
                    {
                        h3NetBlocks.Add(new H3NetBlock(blockBehaviour));
                    }
                }
                catch
                {
                    ModConsole.Log("憋没事继承blockbehaviour,错误零件名:" + blockBehaviour.name);
                }
            }
            return h3NetBlocks;
        }
        public BlockBehaviour GetRealBB()
        {
            foreach (BlockBehaviour blockBehaviour in ReferenceMaster.GetAllSimulationBlocks())
            {
                if (blockBehaviour.BuildingBlock.Guid == guid && blockBehaviour.ParentMachine.PlayerID == playerid)
                {
                    return blockBehaviour;
                }
            }
            return null;
        }
        public void SetRealBB()
        {
            foreach (BlockBehaviour blockBehaviour in ReferenceMaster.GetAllSimulationBlocks())
            {
                if (blockBehaviour.BuildingBlock.Guid == guid && blockBehaviour.ParentMachine.PlayerID == playerid)
                {
                    blockBehaviour.transform.position = pos;
                    blockBehaviour.transform.rotation = rot;
                    blockBehaviour.transform.localScale = scale;
                }
            }
        }
        public BlockBehaviour GetSetRealBB()
        {
            foreach (BlockBehaviour blockBehaviour in ReferenceMaster.GetAllSimulationBlocks())
            {
                if (blockBehaviour.BuildingBlock.Guid == guid && blockBehaviour.ParentMachine.PlayerID == playerid)
                {
                    blockBehaviour.transform.position = pos;
                    blockBehaviour.transform.rotation = rot;
                    blockBehaviour.transform.localScale = scale;
                    return blockBehaviour;
                }
            }
            return null;
        }

    }

    public class H3NetworkManager : SingleInstance<H3NetworkManager>
    {
        public float ping = 0f;
        public static MessageType H3NetBlock;
        public static MessageType H3ClusterNet;
        public static MessageType ClientRequest;
        public static MessageType apperarenceBlock;
        public static MessageType h3NetBlockSet;
        public float rateSend
        {
            get { return ratesend; }
            set { ratesend = value; }
        }
        private float ratesend = 0.05f;
        private float time;
        private float sendtoalltime;
        public H3NetworkManager()
        {

            OptionsMaster.maxSendRate = 10000000f;
            OptionsMaster.defaultSendRate = 10000000f;
            OptionsMaster.minSendRate = 10000000f;
            H3NetBlock = ModNetworking.CreateMessageType(new DataType[]
            {
                DataType.ByteArray
            });

            H3ClusterNet = ModNetworking.CreateMessageType(new DataType[]
            {
                DataType.ByteArray
            });
            ClientRequest = ModNetworking.CreateMessageType(new DataType[]
{
                DataType.Integer
});
            apperarenceBlock = ModNetworking.CreateMessageType(new DataType[]
{
                DataType.String,//baseblock
                DataType.Integer,//baseplayer
                DataType.String//targetblock
                
});
            h3NetBlockSet = ModNetworking.CreateMessageType(new DataType[]
{
                DataType.ByteArray
});
            ModNetworking.Callbacks[H3NetBlock] += delegate (Message msg)//plyaercount*4+(blockcount 4 + playerid 2 + {block...(blockid+blockpos+blockrot)(35*block*count)})*playercount
            {
                byte[] recbytes = (byte[])msg.GetData(0);
                //ModConsole.Log(recbytes.Length.ToString());

                PullAllPlayers(recbytes);
                ping = 0f;
            };
            ModNetworking.Callbacks[H3ClusterNet] += delegate (Message msg)//plyaercount*4+(blockcount 4 + playerid 2 + {block...(blockid+blockpos+blockrot)(35*block*count)})*playercount
            {
                byte[] recbytes = (byte[])msg.GetData(0);
                ClusterSend clusterSend = DeserializeClusterSend(recbytes);
                ApplyAllClusters(clusterSend.clusterList);

            };
        }
        public List<GameObject> clusterlist = new List<GameObject>();
        public List<GameObject> clusterlistLast = new List<GameObject>();
        public override string Name => "manager";
        public byte[] test;
        public void ApplyAllClusters(List<H3NetCluster> h3NetClusters)
        {
            foreach (H3NetworkBlock h3NetworkBlock in FindObjectsOfType<H3NetworkBlock>())
            {
                h3NetworkBlock.isClusterBase = false;
            }
            clusterlist = new List<GameObject>();
            foreach (H3NetCluster h3net in h3NetClusters)
            {
                h3net.ApplyToMachine();
            }
            foreach (GameObject gameObject in clusterlistLast)
            {
                Destroy(gameObject);
            }
            clusterlistLast = clusterlist;
        }

        //-----------------------
        public void FixedUpdate()
        {



            if (StatMaster.isHosting)
            {
                time += Time.fixedDeltaTime;
                sendtoalltime += Time.fixedDeltaTime;

                if (time >= ratesend)
                {
                    time = 0;

                    //ModConsole.Log(PushAllPlayers().Length.ToString());
                    for (int i = 0; i < Playerlist.Players.Count; i++)
                    {
                        ModNetworking.SendTo(Player.From(Playerlist.Players[i].networkId), H3NetBlock.CreateMessage(new object[]
                    {
                    PushOnePlayers((ushort)i)
                    }));
                    }



                }
                if (sendtoalltime >= ratesend * 5f)
                {
                    sendtoalltime = 0f;
                    ModNetworking.SendToAll(H3NetBlock.CreateMessage(new object[]
{
                    PushAllPlayers()
}));
                }

            }
            else
            {
                ping += Time.fixedDeltaTime;
            }
        }
        public byte[] SerializePartCluster(ClusterSend cluster)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, cluster);
                return ms.ToArray();
            }
        }
        public ClusterSend DeserializePartCluster(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (ClusterSend)formatter.Deserialize(ms);
            }
        }
        public void SendAllBB()
        {
            foreach (H3ClustersTest h3ClustersTest in Transform.FindObjectsOfType<H3ClustersTest>())
            {
                h3ClustersTest.send = true;
            }
            ModNetworking.SendToAll(H3NetBlock.CreateMessage(new object[]
{
                    PushAllPlayers()
}));
        }
        public void FixedCluster()
        {
            if (StatMaster.isHosting)
            {
                PushAllCluster();
                SendAllBB();
            }
            else
            {
                ModNetworking.SendToAll(ClientRequest.CreateMessage(new object[]
{
                    0
}));
            }
        }
        public void PushAllCluster()
        {

            List<H3NetCluster> h3NetClusters = H3NetCluster.GetAllNetCluster();
            ClusterSend clusterSend = new ClusterSend();
            clusterSend.clusterList = h3NetClusters;
            byte[] byteArray = SerializeClusterSend(clusterSend);
            ModNetworking.SendToAll(H3ClusterNet.CreateMessage(new object[]
{
                    byteArray
}));
            /*
            foreach (blockset blockset in FindObjectsOfType<blockset>()) {
                if (blockset.appearancePart.IsActive) {

                    ModNetworking.SendToAll(apperarenceBlock.CreateMessage(new object[]
        {
                    blockset.blockBehaviour.BuildingBlock.Guid.ToString(),
                    (int)blockset.blockBehaviour._parentMachine.PlayerID,
                    blockset.sonTo.GetComponent<BlockBehaviour>().BuildingBlock.Guid.ToString(),
        })); ; }
            }*/

        }
        public void PullAllPlayers(byte[] bytes)
        {
            int offset = 0;
            int playernum = BitConverter.ToInt32(bytes, offset);
            offset += 4;
            for (int i = 0; i < playernum; i++)
            {
                int playerlenght = BitConverter.ToInt32(bytes, offset) * 35 + 6;
                byte[] playerbytes = new byte[playerlenght];
                Buffer.BlockCopy(bytes, offset, playerbytes, 0, playerlenght);
                offset += playerlenght;
                try
                {
                    PullPlayer(playerbytes);
                }
                catch
                {

                }
            }


        }
        public void PullPlayer(byte[] bytes)
        {
            List<byteAndBB> byteandbb = new List<byteAndBB>();
            List<H3NetworkBlock> rePos = new List<H3NetworkBlock>();
            int offset = 0;
            int blocknum = BitConverter.ToInt32(bytes, offset);
            offset += 4;
            ushort playid = BitConverter.ToUInt16(bytes, offset);
            try
            {
                PlayerData playerData = Playerlist.GetPlayer(playid);
                //List<BlockBehaviour> blockBehaviours = FindObjectsOfType<BlockBehaviour>().ToList();
                List<BlockBehaviour> blockBehaviours = ReferenceMaster.GetAllSimulationBlocks();
                try
                {
                    playerData.machine.networkBlocks = new NetworkBlock[0];
                    foreach (Machine.SimCluster simCluster in playerData.machine.simClusters)
                    {
                        simCluster.BaseTransform.GetComponent<H3NetworkBlock>().isClusterBase = true;
                    }
                }
                catch
                {

                }
                offset += 2;
                try
                {
                    for (int i = 0; i < blocknum; i++)
                    {
                        Guid guid = Int2Guid(BitConverter.ToInt32(bytes, offset), BitConverter.ToInt32(bytes, offset + 4), BitConverter.ToInt32(bytes, offset + 8), BitConverter.ToInt32(bytes, offset + 12));
                        offset += 16;

                        for (int n = 0; n < blockBehaviours.Count; n++)
                        {
                            try
                            {
                                if (blockBehaviours[n].BuildingBlock.Guid == guid && blockBehaviours[n].ParentMachine.PlayerID == playid)
                                {
                                    BlockBehaviour blockBehaviour = blockBehaviours[n];
                                    try
                                    {
                                        if (blockBehaviour.transform.parent.name == (blockBehaviour.transform.name + "Base") && blockBehaviour.transform.GetComponent<H3NetworkBlock>().isClusterBase == true)
                                        {
                                            try
                                            {
                                                if (blockBehaviour.transform.parent.GetComponent<H3NetworkBlock>() == null)
                                                {
                                                    blockBehaviour.transform.parent.gameObject.AddComponent<H3NetworkBlock>().blockBehaviour = blockBehaviour;

                                                }
                                                H3NetworkBlock h3NetworkBlock = blockBehaviour.transform.parent.GetComponent<H3NetworkBlock>();
                                                h3NetworkBlock.islocal = false;
                                                h3NetworkBlock.PullObject(ref offset, bytes);
                                                rePos.Add(h3NetworkBlock);
                                            }
                                            catch
                                            {

                                            }
                                        }
                                        else if (blockBehaviour.transform.parent.GetComponent<H3NetworkBlock>() == null && blockBehaviour.transform.GetComponent<H3NetworkBlock>().isClusterBase == true)
                                        {
                                            try
                                            {
                                                H3NetworkBlock h3NetworkBlock = blockBehaviour.transform.GetComponent<H3NetworkBlock>();
                                                h3NetworkBlock.islocal = false;
                                                h3NetworkBlock.PullObject(ref offset, bytes);
                                                rePos.Add(h3NetworkBlock);
                                            }
                                            catch
                                            {

                                            }
                                        }
                                        else
                                        {
                                            try
                                            {

                                                blockBehaviours.Remove(blockBehaviour);
                                                byteandbb.Add(new byteAndBB(blockBehaviour, offset));
                                                offset += 19;//35



                                            }
                                            catch
                                            {

                                            }
                                        }
                                    }
                                    catch
                                    {

                                    }
                                    break;
                                }
                            }
                            catch
                            {

                            }
                        }

                    }
                }
                catch
                {

                }
                foreach (byteAndBB byteAndBB in byteandbb)
                {
                    H3NetworkBlock h3NetworkBlock = byteAndBB.bb.GetComponent<H3NetworkBlock>();
                    h3NetworkBlock.islocal = true;
                    int offsetE = 0;
                    offsetE = byteAndBB.offset;
                    h3NetworkBlock.PullObject(ref offsetE, bytes);
                }
                foreach (H3NetworkBlock h3NetworkBlock1 in rePos)
                {
                    h3NetworkBlock1.LerpPos();
                }
            }
            catch
            {

            }



        }
        public byte[] PushAllPlayers()
        {
            byte[][] send = new byte[Playerlist.Players.Count][];
            for (int i = 0; i < Playerlist.Players.Count; i++)
            {
                send[i] = PushPlayer((ushort)i);
            }
            int number = 0;
            foreach (byte[] bytes in send)
            {
                number += bytes.Length;
            }
            byte[] ret = new byte[number + 4];
            number = 0;
            byte[] bytes2 = BitConverter.GetBytes(Playerlist.Players.Count);
            ret[number] = bytes2[0];
            ret[number + 1] = bytes2[1];
            ret[number + 2] = bytes2[2];
            ret[number + 3] = bytes2[3];
            number += 4;
            NetworkCompression.WriteArray(send, ret, number);
            return ret;
        }
        public byte[] PushOnePlayers(ushort playerid)
        {
            byte[][] send = new byte[1][];
            for (int i = 0; i < 1; i++)
            {
                send[i] = PushPlayer(playerid);
            }
            int number = 0;
            foreach (byte[] bytes in send)
            {
                number += bytes.Length;
            }
            byte[] ret = new byte[number + 4];
            number = 0;
            byte[] bytes2 = BitConverter.GetBytes(1);
            ret[number] = bytes2[0];
            ret[number + 1] = bytes2[1];
            ret[number + 2] = bytes2[2];
            ret[number + 3] = bytes2[3];
            number += 4;
            NetworkCompression.WriteArray(send, ret, number);
            return ret;
        }
        public byte[] PushPlayer(ushort player)//blockcount 4 + playerid 2 + {block...(blockid+blockpos+blockrot+blockhealth)(39*block*count)}   blockcount*39+6
        {
            PlayerData playerData = Playerlist.Players[player];

            List<BlockBehaviour> blockBehaviours = new List<BlockBehaviour>();
            try
            {
                foreach (Machine.SimCluster block in playerData.machine.simClusters)
                {
                    try
                    {
                        foreach (BlockBehaviour blockBehaviour in block.Blocks)
                        {
                            try
                            {
                                if (blockBehaviour.GetComponent<H3NetworkBlock>() != null)
                                {
                                    if (blockBehaviour.transform.GetComponent<H3ClustersTest>() == null && blockBehaviour != block.Base)
                                    {
                                        blockBehaviour.transform.gameObject.AddComponent<H3ClustersTest>().ClusterBaseBlock = block.Base;

                                    }

                                    H3ClustersTest h3ClustersTest = blockBehaviour.transform.GetComponent<H3ClustersTest>();
                                    if (h3ClustersTest.send == true)
                                    {
                                        h3ClustersTest.send = false;
                                        if (blockBehaviour.GetComponent<H3NetworkBlock>() == true)
                                        {
                                            blockBehaviours.Add(blockBehaviour);
                                        }

                                    }
                                }
                            }
                            catch
                            {

                            }
                        }
                        if (block.Base.GetComponent<H3NetworkBlock>() == true)
                        {
                            blockBehaviours.Add(block.Base);
                        }
                    }
                    catch
                    {

                    }
                }
            }
            catch
            {

            }
            byte[][] send = new byte[blockBehaviours.Count][];
            for (int i = 0; i < blockBehaviours.Count; i++)
            {

                BlockBehaviour blockBehaviour = blockBehaviours[i];
                try
                {
                    H3NetworkBlock h3NetworkBlock = blockBehaviour.GetComponent<H3NetworkBlock>();
                    int[] ints = Guid2Int(blockBehaviour.BuildingBlock.Guid);
                    byte[] bytes = new byte[16];
                    BitConverter.GetBytes(ints[0]).CopyTo(bytes, 0);
                    BitConverter.GetBytes(ints[1]).CopyTo(bytes, 4);
                    BitConverter.GetBytes(ints[2]).CopyTo(bytes, 8);
                    BitConverter.GetBytes(ints[3]).CopyTo(bytes, 12);
                    byte[] buffer = new byte[bytes.Length + 19];//19+4
                    int offset = 0;
                    Buffer.BlockCopy(bytes, 0, buffer, 0, bytes.Length);
                    offset += bytes.Length;
                    h3NetworkBlock.PushObject(ref offset, buffer);//35
                    send[i] = buffer;//39
                }
                catch
                {

                }
            }

            int number = 0;
            int bytecount = 0;
            foreach (byte[] bytes1 in send)
            {
                bytecount += bytes1.Length;
            }
            byte[] sendreturn = new byte[bytecount + 6];
            byte[] bytes2 = BitConverter.GetBytes(blockBehaviours.Count);
            sendreturn[number] = bytes2[0];
            sendreturn[number + 1] = bytes2[1];
            sendreturn[number + 2] = bytes2[2];
            sendreturn[number + 3] = bytes2[3];
            number += 4;

            sendreturn[number] = BitConverter.GetBytes(playerData.networkId)[0];
            sendreturn[number + 1] = BitConverter.GetBytes(playerData.networkId)[1];
            number += 2;

            NetworkCompression.WriteArray(send, sendreturn, number);

            return sendreturn;
        }

        public static int[] Guid2Int(Guid value)
        {
            byte[] b = value.ToByteArray();
            int bint = BitConverter.ToInt32(b, 0);
            var bint1 = BitConverter.ToInt32(b, 4);
            var bint2 = BitConverter.ToInt32(b, 8);
            var bint3 = BitConverter.ToInt32(b, 12);
            return new[] { bint, bint1, bint2, bint3 };
        }

        public static Guid Int2Guid(int value, int value1, int value2, int value3)
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            BitConverter.GetBytes(value1).CopyTo(bytes, 4);
            BitConverter.GetBytes(value2).CopyTo(bytes, 8);
            BitConverter.GetBytes(value3).CopyTo(bytes, 12);
            return new Guid(bytes);
        }

        public static byte[] SerializeClusterSend(ClusterSend send)
        {
            List<byte> result = new List<byte>();

            // Serialize count of clusters
            result.AddRange(BitConverter.GetBytes(send.clusterList.Count));

            // Serialize each cluster
            foreach (H3NetCluster cluster in send.clusterList)
            {
                // Serialize Base
                int[] guidInts = Guid2Int(cluster.Base.guid);
                foreach (int intValue in guidInts)
                {
                    result.AddRange(BitConverter.GetBytes(intValue));
                }
                result.AddRange(BitConverter.GetBytes(cluster.Base.playerid));
                result.AddRange(BitConverter.GetBytes(cluster.Base.pos.x));
                result.AddRange(BitConverter.GetBytes(cluster.Base.pos.y));
                result.AddRange(BitConverter.GetBytes(cluster.Base.pos.z));
                result.AddRange(BitConverter.GetBytes(cluster.Base.rot.x));
                result.AddRange(BitConverter.GetBytes(cluster.Base.rot.y));
                result.AddRange(BitConverter.GetBytes(cluster.Base.rot.z));
                result.AddRange(BitConverter.GetBytes(cluster.Base.rot.w));
                result.AddRange(BitConverter.GetBytes(cluster.Base.scale.x));
                result.AddRange(BitConverter.GetBytes(cluster.Base.scale.y));
                result.AddRange(BitConverter.GetBytes(cluster.Base.scale.z));

                // Serialize blocks count
                result.AddRange(BitConverter.GetBytes(cluster.blocks.Count));

                // Serialize each block
                foreach (H3NetBlock block in cluster.blocks)
                {
                    guidInts = Guid2Int(block.guid);
                    foreach (int intValue in guidInts)
                    {
                        result.AddRange(BitConverter.GetBytes(intValue));
                    }
                    result.AddRange(BitConverter.GetBytes(block.playerid));
                    result.AddRange(BitConverter.GetBytes(block.pos.x));
                    result.AddRange(BitConverter.GetBytes(block.pos.y));
                    result.AddRange(BitConverter.GetBytes(block.pos.z));
                    result.AddRange(BitConverter.GetBytes(block.rot.x));
                    result.AddRange(BitConverter.GetBytes(block.rot.y));
                    result.AddRange(BitConverter.GetBytes(block.rot.z));
                    result.AddRange(BitConverter.GetBytes(block.rot.w));
                    result.AddRange(BitConverter.GetBytes(block.scale.x));
                    result.AddRange(BitConverter.GetBytes(block.scale.y));
                    result.AddRange(BitConverter.GetBytes(block.scale.z));
                }

                // Serialize playerid of cluster
                result.AddRange(BitConverter.GetBytes(cluster.playerid));
            }

            return result.ToArray();
        }

        public static ClusterSend DeserializeClusterSend(byte[] data)
        {
            ClusterSend result = new ClusterSend();
            result.clusterList = new List<H3NetCluster>();
            int offset = 0;

            // Deserialize count of clusters
            int clusterCount = BitConverter.ToInt32(data, offset);
            offset += 4;

            for (int i = 0; i < clusterCount; i++)
            {
                // Deserialize Base
                int value = BitConverter.ToInt32(data, offset);
                offset += 4;

                int value1 = BitConverter.ToInt32(data, offset);
                offset += 4;

                int value2 = BitConverter.ToInt32(data, offset);
                offset += 4;

                int value3 = BitConverter.ToInt32(data, offset);
                offset += 4;

                Guid guid = Int2Guid(value, value1, value2, value3);

                ushort playerId = BitConverter.ToUInt16(data, offset);
                offset += 2;

                Vector3 pos = new Vector3(BitConverter.ToSingle(data, offset),
                                          BitConverter.ToSingle(data, offset + 4),
                                          BitConverter.ToSingle(data, offset + 8));
                offset += 12;

                Quaternion rot = new Quaternion(BitConverter.ToSingle(data, offset),
                                                BitConverter.ToSingle(data, offset + 4),
                                                BitConverter.ToSingle(data, offset + 8),
                                                BitConverter.ToSingle(data, offset + 12));
                offset += 16;

                Vector3 scale = new Vector3(BitConverter.ToSingle(data, offset),
                                            BitConverter.ToSingle(data, offset + 4),
                                            BitConverter.ToSingle(data, offset + 8));
                offset += 12;

                H3NetBlock baseBlock = new H3NetBlock()
                {
                    guid = guid,
                    playerid = playerId,
                    pos = pos,
                    rot = rot,
                    scale = scale
                };

                H3NetCluster cluster = new H3NetCluster(baseBlock);
                cluster.blocks = new List<H3NetBlock>();

                // Deserialize blocks count
                int blockCount = BitConverter.ToInt32(data, offset);
                offset += 4;

                for (int j = 0; j < blockCount; j++)
                {
                    // Deserialize each block, similar to how Base was deserialized
                    value = BitConverter.ToInt32(data, offset);
                    offset += 4;

                    value1 = BitConverter.ToInt32(data, offset);
                    offset += 4;

                    value2 = BitConverter.ToInt32(data, offset);
                    offset += 4;

                    value3 = BitConverter.ToInt32(data, offset);
                    offset += 4;

                    guid = Int2Guid(value, value1, value2, value3);

                    playerId = BitConverter.ToUInt16(data, offset);
                    offset += 2;

                    pos = new Vector3(BitConverter.ToSingle(data, offset),
                                      BitConverter.ToSingle(data, offset + 4),
                                      BitConverter.ToSingle(data, offset + 8));
                    offset += 12;

                    rot = new Quaternion(BitConverter.ToSingle(data, offset),
                                         BitConverter.ToSingle(data, offset + 4),
                                         BitConverter.ToSingle(data, offset + 8),
                                         BitConverter.ToSingle(data, offset + 12));
                    offset += 16;

                    scale = new Vector3(BitConverter.ToSingle(data, offset),
                                        BitConverter.ToSingle(data, offset + 4),
                                        BitConverter.ToSingle(data, offset + 8));
                    offset += 12;

                    H3NetBlock block = new H3NetBlock()
                    {
                        guid = guid,
                        playerid = playerId,
                        pos = pos,
                        rot = rot,
                        scale = scale
                    };
                    cluster.blocks.Add(block);
                }

                // Deserialize playerid of cluster
                cluster.playerid = BitConverter.ToUInt16(data, offset);
                offset += 2;

                result.clusterList.Add(cluster);
            }

            return result;
        }




    }
}
