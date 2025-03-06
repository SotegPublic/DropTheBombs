using System;
using UnityEngine;

namespace Components
{
    [Serializable]
    public class PilonHealthModifierConfig
    {
        [SerializeField] private int bossIndex;
        [SerializeField] private float modifier;

        public int BossIndex => bossIndex;
        public float Modifier => modifier;
    }
}