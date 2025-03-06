using System;
using UnityEngine;
using Helpers;

namespace Components
{
    [Serializable]
    public class PilonBaseConfig
    {
        [SerializeField][IdentifierDropDown(nameof(PilonIdentifier))] private int pilonID;
        [SerializeField] private float pilonHealth;
        [SerializeField] private float pilonXModifier;

        public int PilonID => pilonID;
        public float PilonHealth => pilonHealth;
        public float PilonXModifier => pilonXModifier;
    }
}