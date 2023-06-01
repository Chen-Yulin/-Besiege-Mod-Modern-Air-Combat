using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Collections;

using Modding.Modules;
using Modding;
using Modding.Blocks;
using UnityEngine;

namespace ModernAirCombat
{
    public class PanelDragController : PropellerDragController
    {
        protected AxialDrag _axialDrag;

        public override void FixedUpdate()
        {
            if (_tf)
            {

                if (!_axialDrag)
                {
                    _axialDrag = GetComponent<AxialDrag>();
                    _originAxisDrag = _axialDrag.AxisDrag.y;
                }
                else
                {
                    _axialDrag.AxisDrag = new Vector3(0, Mathf.Clamp((1 - _height / MaxHeight), 0.2f, 1f) * _originAxisDrag, 0);
                }
            }

        }

    }
}
