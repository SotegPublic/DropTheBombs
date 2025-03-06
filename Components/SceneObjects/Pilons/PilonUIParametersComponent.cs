using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Variables, "here we set parameters for pilon")]
    public sealed class PilonUIParametersComponent : BaseComponent
    {
        [SerializeField] private float riseTime = 0.4f;
        [SerializeField] private float riseSpeed = 10;

        public float RiseTime => riseTime;
        public float RiseSpeed => riseSpeed;
    }
}