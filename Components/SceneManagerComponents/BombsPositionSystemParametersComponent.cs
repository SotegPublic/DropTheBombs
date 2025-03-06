using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Variables, Doc.Movement, Doc.Bombs, "here we set parameters for Bombs Position System")]
    public sealed class BombsPositionSystemParametersComponent : BaseComponent
    {
        [SerializeField] private float positionStep = 0.5f;
        [SerializeField] private int ringsCount = 4;
        [SerializeField] private float bombsoffset = 0.1f;

        public float PositionStep => positionStep;
        public int RingsCount => ringsCount;
        public float Bombsoffset => bombsoffset;
    }
}