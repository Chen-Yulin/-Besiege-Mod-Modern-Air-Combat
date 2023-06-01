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
    public class PropellerDragController : MonoBehaviour
    {
        public float MaxHeight = 10000f;
        protected Transform _tf;
        protected PropellorController _controller;
        protected float _originAxisDrag;
        protected Rigidbody _rigid;
        protected float _originDrag;

        protected float _height
        {
            get { return _tf.position.y; }
        }
        
        public void Awake()
        {
        }
        public virtual void Start()
        {
            if (GetComponent<BlockBehaviour>().isSimulating && !StatMaster.isClient)
            {
                _tf = transform;
            }
        }
        public virtual void FixedUpdate()
        {
            if (_tf)
            {

                if (!_controller)
                {
                    _controller = GetComponent<PropellorController>();
                    _originAxisDrag = _controller.AxisDrag.y;
                }
                else
                {
                    _controller.AxisDrag = new Vector3(0, Mathf.Clamp((1 - _height / MaxHeight), 0.2f, 1f) * _originAxisDrag, 0);
                }
            }
            
        }

    }
}
