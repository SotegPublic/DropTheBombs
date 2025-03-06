using HECSFramework.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Plane, Doc.Holder, "here we hold plane textures")]
    public sealed class PlaneTexturesHolderComponent : BaseComponent, IWorldSingleComponent
    {
        [SerializeField] private PlaneTextureConfig[] planeTextures;

        public List<PlaneTextureConfig> GetAllTexturesConfigsOfType(ItemsTypes type)
        {
            List<PlaneTextureConfig> tectures = new List<PlaneTextureConfig>(20);

            for (int i = 0; i < planeTextures.Length; i++)
            {
                if (planeTextures[i].ItemType == type)
                {
                    tectures.Add(planeTextures[i]);
                }
            }
            return tectures;

        }

        public PlaneTextureConfig GetPlaneTextureByID(int id)
        {
            for (int i = 0; i < planeTextures.Length; i++)
            {
                if (planeTextures[i].TextureID == id)
                {
                    return planeTextures[i];
                }
            }

            throw new Exception("unknown texture ID");
        }

        public PlaneTextureConfig[] GetAllTextures()
        {
            return planeTextures;
        }
    }
}
