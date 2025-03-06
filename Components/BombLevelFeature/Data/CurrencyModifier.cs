using System;
using UnityEngine;

namespace Components
{
    [Serializable]
    public class CurrencyModifier
    {
        [SerializeField] private int levelEdge;
        [SerializeField] private float modifier;

        public int LevelEdge => levelEdge;
        public float Modifier => modifier;
    }
}