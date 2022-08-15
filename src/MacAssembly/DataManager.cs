using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernAirCombat
{
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

        public targetManager[] TargetData = new targetManager[10];
        public displayerData[] DisplayerData = new displayerData[10];
        
    }
}
