using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Collections;

using Modding.Modules;
using Modding;
using Modding.Blocks;
using UnityEngine;
using UnityEngine.Networking;
using InternalModding.Blocks;

namespace ModernAirCombat
{

    class CustomBlockController : SingleInstance<CustomBlockController>
    {

        public override string Name { get; } = "Custom Block Controller";

        internal PlayerMachineInfo PMI;

        private void Awake()
        {

            //加载配置
            //Events.OnMachineLoaded += LoadConfiguration;
            Events.OnMachineLoaded += (pmi) => { PMI = pmi; };
            ////储存配置
            //Events.OnMachineSave += SaveConfiguration;
            //添加零件初始化事件委托
            Events.OnBlockInit += AddSliders;

        }
        private void AddSliders(Block block)
        {

            BlockBehaviour blockbehaviour = block.BuildingBlock.InternalObject;
            AddSliders(blockbehaviour);
        }
        private void AddSliders(BlockBehaviour block)
        {
            //if (StatMaster.isMP == StatMaster.IsLevelEditorOnly)
            switch (block.BlockID)
            {
                case (int)BlockType.Propeller:
                    {
                        if (!block.gameObject.GetComponent<PropellerDragController>())
                        {

                            block.gameObject.AddComponent<PropellerDragController>();
                        }
                        break;
                    }
                case (int)BlockType.SmallPropeller:
                    {
                        if (!block.gameObject.GetComponent<PropellerDragController>())
                        {
                            block.gameObject.AddComponent<PropellerDragController>();
                        }
                        break;
                    }
                case (int)BlockType.WingPanel:
                    {
                        if (!block.gameObject.GetComponent<PanelDragController>())
                        {
                            block.gameObject.AddComponent<PanelDragController>();
                        }
                        break;
                    }
                case (int)BlockType.Wing:
                    {
                        if (!block.gameObject.GetComponent<PanelDragController>())
                        {
                            block.gameObject.AddComponent<PanelDragController>();
                        }
                        break;
                    }
                default:
                    {
                        if (block.gameObject.GetComponent<SRAAMBlock>() || 
                            block.gameObject.GetComponent<MRAAMBlock>() || 
                            block.gameObject.GetComponent<AGMBlock>() || 
                            block.gameObject.GetComponent<GuidedBombBlock>())
                        {
                            break;
                        }
                        if (!block.gameObject.GetComponent<AirDragController>())
                        {
                            block.gameObject.AddComponent<AirDragController>();
                        }
                        
                        break;
                    }
            }
        }
    }
}
