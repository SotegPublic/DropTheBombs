using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Holder, Doc.Plane, "here we hold plane views")]
    public sealed class PlanesViewsHolderComponent : BaseComponent
    {
        [SerializeField] private PlaneViewConfig[] planes;

        public PlaneViewConfig[] GetAllPlanesConfigs()
        {
            return planes;
        }

        public PlaneViewConfig GetPlaneViewConfigByID(int id)
        {
            for(int i = 0 ; i < planes.Length; i++)
            {
                if (planes[i].PlaneID == id)
                {
                    return planes[i];
                }
            }

            throw new Exception($"unknown plane ID: {id}");
        }
    }
}