using Helpers;
using System;
using UnityEngine;

namespace Components 
{ 
    [Serializable]
    public class PlaneTextureConfig
    {
        [SerializeField][IdentifierDropDown(nameof(PlanePaintIdentifier))] private int textureID;
        [SerializeField] private ItemsTypes itemType;
        [SerializeField] private Texture2D planeTexture;
        [SerializeField] private Sprite shopSprite;
        [SerializeField] private int currencyCost;
        [SerializeField] private string description;

        public ItemsTypes ItemType => itemType;
        public int TextureID => textureID;
        public Texture2D PlaneTexture => planeTexture;
        public Sprite ShopSprite => shopSprite;
        public int CurrencyCost => currencyCost;
        public string Description => description;
    }
}