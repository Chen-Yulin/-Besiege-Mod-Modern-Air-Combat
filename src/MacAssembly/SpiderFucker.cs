using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class SpiderFucker : SingleInstance<SpiderFucker>
{
    public override string Name => "SpiderFuckerr";
    public bool FloorDeactiveSwitch //去地板
    {
        get { return skpCustomModule.SkyBoxChanger.Instance.FloorDeactiveSwitch; }

        set { skpCustomModule.SkyBoxChanger.Instance.FloorDeactiveSwitch = value; }
    }
    public bool ExpandFloorSwitch //true=固定10倍空气墙
    {
        get { return skpCustomModule.SkyBoxChanger.Instance.ExpandFloorSwitch; }

        set { skpCustomModule.SkyBoxChanger.Instance.ExpandFloorSwitch = value; }
    }
    public bool ExExpandFloorSwitch  //true=自定义空气墙大小  与ExpandFloorSwitch不可同时启动
    {
        get { return skpCustomModule.SkyBoxChanger.Instance.ExExpandFloorSwitch; }

        set { skpCustomModule.SkyBoxChanger.Instance.ExExpandFloorSwitch = value; }
    }
    public float ExExpandScale  //自定义空气墙大小（单位m）
    {
        get { return skpCustomModule.SkyBoxChanger.Instance.scale; }

        set { skpCustomModule.SkyBoxChanger.Instance.scale = value; }
    }
    public void Apply()//应用设置
    {
        skpCustomModule.SkyBoxChanger.Instance.toggle_skyboxApply = true;
    }
}

