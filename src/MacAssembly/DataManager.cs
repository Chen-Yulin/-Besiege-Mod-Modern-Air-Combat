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
    public class RWRTargetData
    {
        //public bool hasRadiation;
        
        
        public RWRTargetData()
        {
            //hasRadiation = false;
            
        }
    }
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
        public Vector3[] RadarTransformForward = new Vector3[10];
        public targetManager[] TargetData = new targetManager[10];
        public displayerData[] DisplayerData = new displayerData[10];
        public BVRTargetData[] BVRData = new BVRTargetData[10];
        public float[,] RWRData = new float[10,8];
        
    }
}
