using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Modding.Modules;
using Modding;
using Modding.Blocks;
using UnityEngine;

namespace ModernAirCombat
{
    public class BVRTargetData
    {
        public Vector3 position;
        public Vector3 velocity;
        public BVRTargetData()
        {
            position = new Vector3(0, 0, 0);
            velocity = new Vector3(0, 0, 0);
        }
    }
    public class displayerData
    {
        public float radarPitch;
        public float radarAngle;
        public displayerData(float radarPitch, float radarAngle)
        {
            this.radarPitch = radarPitch;
            this.radarAngle = radarAngle;
        }   
    }

    public class DataManager : SingleInstance<DataManager>
    {
        public override string Name { get; } = "Data Manager";
        public Vector3[] RadarTransformForward = new Vector3[16];
        public targetManager[] TargetData = new targetManager[16];
        public displayerData[] DisplayerData = new displayerData[16];
        public BVRTargetData[] BVRData = new BVRTargetData[16];
        public float[,] RWRData = new float[16,8];

        public DataManager()
        {
            for (int i = 0; i < 16; i++)
            {
                RadarTransformForward[i] = Vector3.zero;
                TargetData[i] = new targetManager();
                DisplayerData[i] = new displayerData(0,0);
                BVRData[i] = new BVRTargetData();
            }
        }
        
        
    }
}
