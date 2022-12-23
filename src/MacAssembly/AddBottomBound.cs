using System;
using System.Collections.Generic;
using System.Text;

using Modding;

using Modding.Common;
using UnityEngine;

namespace ModernAirCombat
{
    class AddBottomBound:SingleInstance<AddBottomBound>
    {
        public override string Name { get; } = "AddBottomBound";

        public void Update()
        {
            if (StatMaster.isMP)
            {
                if (GameObject.Find("WORLD BOUNDARIES"))
                {
                    GameObject WorldBoundary = GameObject.Find("WORLD BOUNDARIES").gameObject;
                    if (!WorldBoundary.transform.FindChild("WorldBoundaryBottom"))
                    {
                        GameObject BoundaryBottom = Instantiate(WorldBoundary.transform.Find("WorldBoundaryTop").gameObject);
                        BoundaryBottom.transform.SetParent(WorldBoundary.transform);
                        BoundaryBottom.name = "WorldBoundaryBottom";
                        BoundaryBottom.transform.position = new Vector3(0f, -250f, 0f);
                    }
                }
            }
        }
    }
}
