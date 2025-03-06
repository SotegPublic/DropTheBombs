using Helpers;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Components
{
    [Serializable]
    public class BombConfig
    {
        [SerializeField][IdentifierDropDown(nameof(BombIdentifier))] private int bombID;
        [SerializeField][IdentifierDropDown(nameof(FXIdentifier))] private int bombFXID;
        [SerializeField][IdentifierDropDown(nameof(FXIdentifier))] private int bombDestroyFXID;
        [SerializeField] private Mesh bombMesh;
        [SerializeField] private float bombEvolvePower;

        public int BombID => bombID;
        public int BombFXID => bombFXID;
        public int BombDestroyFXID => bombDestroyFXID;
        public Mesh BombMesh => bombMesh;
        public float BombEvolvePower => bombEvolvePower;
    }
}