using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static SurfaceInfo;

namespace Navalmod
{
    public class H3ClustersTest :MonoBehaviour
    {
        public BlockBehaviour ClusterBaseBlock;
        Quaternion rotLast;
        public bool send;
        public float time;
        public GameObject targetobj;
        public float disLast;

        public void FixedUpdate()
        {
            time -= Time.deltaTime;
            if (time <= 0)
            {
                time = SingleInstance<H3NetworkManager>.Instance.rateSend;
                float i = (ClusterBaseBlock.transform.position - base.transform.position).magnitude;
                Quaternion n = GetLocalRotation();
                if (Quaternion.Angle(n, rotLast) > 1f || Mathf.Abs(disLast-i)>0.3f)
                {

                    send = true;
                    rotLast = n;
                    disLast = i;
                }
            }
            
        }
        public Quaternion GetLocalRotation()
        {
            //Destroy(targetobj);
            return Quaternion.Inverse(ClusterBaseBlock.transform.rotation) * base.transform.rotation;
        }
    }
}
