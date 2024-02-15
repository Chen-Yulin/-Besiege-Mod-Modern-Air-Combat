using Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Navalmod
{
    public class H3NetworkBlock : MonoBehaviour
    {
        public BlockBehaviour blockBehaviour;
        public Vector3 lastpos;
        public Quaternion lastqua;
        public Vector3 nowpos;
        public Quaternion nowqua;
        public Vector3 deltavec;
        public Rigidbody rb;
        public float pingtime;
        public float time;
        public float maxtime;
        public bool haschange = false;
        public bool islocal = false;
        public bool localchangeenter;
        public bool isClusterBase;
        public H3NetworkBlock()
        {
            lastpos = Vector3.zero;
            lastqua = Quaternion.identity;

        }
        public void PushObject(ref int offset, byte[] buffer)// byte[19]
        {
            H3NetCompression.CompressPosition(base.transform.position, buffer, offset);//12
            NetworkCompression.CompressRotation(base.transform.rotation, buffer, offset + 12);//7
            offset += 19;
        }
        public void PushObjectLocal(ref int offset, byte[] buffer)// byte[19]
        {
            BlockBehaviour bb = base.GetComponent<H3ClustersTest>().ClusterBaseBlock;
            H3NetCompression.CompressPosition(bb.transform.InverseTransformPoint(base.transform.position), buffer, offset);//12
            ModConsole.Log(bb.transform.InverseTransformPoint(base.transform.position).ToString() + "localsend");
            NetworkCompression.CompressRotation(Quaternion.Inverse(bb.transform.rotation) * base.transform.rotation, buffer, offset + 12);//7
            offset += 19;
        }
        public void PullObject(ref int offset, byte[] buffer)// byte[19]
        {
            try
            {
                Vector3 vector3;
                Quaternion quat;
                H3NetCompression.DecompressPosition(buffer, offset, out vector3);//12
                NetworkCompression.DecompressRotation(buffer, offset + 12, out quat);//7


                lastpos = base.transform.position;
                lastqua = base.transform.rotation;


                if (islocal)
                {
                    lastpos = base.transform.localPosition;
                    lastqua = base.transform.localRotation;
                }
                nowpos = vector3;
                nowqua = quat;
                base.transform.position = nowpos;
                base.transform.rotation = nowqua;
                if (islocal)
                {
                    nowpos = base.transform.localPosition;
                    nowqua = base.transform.localRotation;
                    base.transform.localPosition = lastpos;
                    base.transform.localRotation = lastqua;
                }
                if (pingtime >= 10f)
                {
                    pingtime = 0f;
                }
                haschange = true;
                pingtime = 0.25f;
                if (pingtime == 0)
                {
                    SmoothToPoint(SingleInstance<H3NetworkManager>.Instance.rateSend, nowpos, base.transform.position);

                }
                else
                {

                    if (islocal)
                    {
                        // pingtime = GetComponentInParent<H3NetworkBlock>().pingtime;
                    }

                    time = pingtime;
                    maxtime = time;
                }
                pingtime = 0;


                offset += 19;
            }
            catch
            {

            }
        }
        public void Update()
        {
            if (StatMaster.isClient)
            {
                time -= Time.deltaTime;

                if (blockBehaviour.isSimulating)
                {
                    if (haschange)
                    {
                        if (islocal)
                        {
                            pingtime += Time.deltaTime;
                            base.transform.localPosition = Vector3.Lerp(lastpos, nowpos, (maxtime - Math.Max(time, 0)) / maxtime);
                            base.transform.localRotation = Quaternion.Lerp(lastqua, nowqua, (maxtime - Math.Max(time, 0)) / maxtime);
                        }
                        else
                        {

                            pingtime += Time.deltaTime;
                            base.transform.position = Vector3.Lerp(lastpos, nowpos, (maxtime - Math.Max(time, 0)) / maxtime);
                            base.transform.rotation = Quaternion.Lerp(lastqua, nowqua, (maxtime - Math.Max(time, 0)) / maxtime);
                        }
                    }
                }
            }
        }
        public void LerpPos()
        {
            base.transform.position = Vector3.Lerp(lastpos, nowpos, (maxtime - Math.Max(time, 0)) / maxtime);
            base.transform.rotation = Quaternion.Lerp(lastqua, nowqua, (maxtime - Math.Max(time, 0)) / maxtime);
        }
        public void SmoothToPoint(float time, Vector3 nowpos, Vector3 lastpos)
        {
            this.time = time;
            maxtime = time;
            deltavec = (nowpos - lastpos).normalized * time;
        }
    }
}
