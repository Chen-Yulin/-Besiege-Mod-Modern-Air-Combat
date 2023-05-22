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
    public class AirDragController : PropellerDragController
    {
        private bool _noRigid = false;
        private int _failCount = 0;
        public override void Start()
        {
            if (GetComponent<BlockBehaviour>().isSimulating)
            {
                _tf = transform;
                if (!GetComponent<Rigidbody>())
                {
                    _noRigid = true;
                }
            }
            
        }
        public override void FixedUpdate()
        {
            if (_tf && !_noRigid && _failCount == 0)
            {
                if (!_rigid)
                {
                    _rigid = GetComponent<Rigidbody>();
                    try
                    {
                        _originDrag = _rigid.drag;
                    }
                    catch { _failCount++;}
                }
                else
                {
                    _rigid.drag =Mathf.Clamp((1 - _height / MaxHeight), 0.2f, 1) * _originDrag;
                }
            }
        }
    }
}
