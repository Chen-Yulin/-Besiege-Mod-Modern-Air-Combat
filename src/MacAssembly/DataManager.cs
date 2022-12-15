using System;
using System.Collections.Generic;
using System.Text;

using Modding.Modules;
using Modding;
using Modding.Blocks;
using UnityEngine;

namespace ModernAirCombat
{
    public class RadarTargetData
    {
        public Vector3 position;
        public Vector3 velocity;
        public RadarTargetData()
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
        public RadarTargetData[] BVRData = new RadarTargetData[16];
        public float[,] RWRData = new float[16,8];
        public RenderTexture[] highlight = new RenderTexture[16];
        public RenderTexture[] output = new RenderTexture[16];
        public float[] TV_FOV = new float[16];
        public bool[] TV_Lock = new bool[16];
        public bool[] TV_Trace = new bool[16];
        public Vector3[] TV_TargetPosition = new Vector3[16];
        public Vector3[] TV_TargetVelocity = new Vector3[16];
        public Vector3[] TV_LockPosition = new Vector3[16];
        public int[] TV_UpDown = new int[16];
        public int[] TV_LeftRight = new int[16];

        public DataManager()
        {
            for (int i = 0; i < 16; i++)
            {
                RadarTransformForward[i] = Vector3.zero;
                TargetData[i] = new targetManager();
                DisplayerData[i] = new displayerData(0,0);
                BVRData[i] = new RadarTargetData();
                highlight[i] = new RenderTexture(512, 512, 0);
                output[i] = new RenderTexture(512, 512, 0);
                TV_TargetPosition[i] = Vector3.zero;
                TV_TargetVelocity[i] = Vector3.zero;
                TV_LockPosition[i] = Vector3.zero;
                TV_FOV[i] = 40f;
            }
        }
        
        
    }
}
