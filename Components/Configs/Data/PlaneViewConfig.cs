using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Helpers;

namespace Components
{
    [Serializable]
    public class PlaneViewConfig
    {
        [SerializeField][IdentifierDropDown(nameof(PlaneIdentifier))] private int planeID;
        [SerializeField] private ItemsTypes itemType;
        [SerializeField] private AssetReferenceGameObject shopViewReferense;
        [SerializeField] private AssetReferenceGameObject gameViewReferense;
        [SerializeField] private AssetReferenceGameObject lockedPrefVarRef;
        [SerializeField] private int maxBombsCount;
        [SerializeField] private Sprite shopSprite;
        [SerializeField] private int currencyCost;
        [SerializeField] private string description;

        public AssetReferenceGameObject ShopViewReferense => shopViewReferense;
        public AssetReferenceGameObject GameViewReferense => gameViewReferense;
        public AssetReferenceGameObject LockedPrefVarRef => lockedPrefVarRef;
        public int PlaneID => planeID;
        public ItemsTypes ItemType => itemType;
        public int MaxBombsCount => maxBombsCount;
        public Sprite ShopSprite => shopSprite;
        public int CurrencyCost => currencyCost;
        public string Description => description;
    }
}