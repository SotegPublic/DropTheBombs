using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Pilons, Doc.Holder, "here we hold rise order for pilons")]
    public sealed class RiseSystemParametersComponent : BaseComponent
    {
        [SerializeField] private PilonIdentifier[] order;
        [SerializeField] private float upDistance;
        [SerializeField] private float riseTime = 0.8f;
        [SerializeField] private float riseTimeChangeStep = 0.1f;

        public PilonIdentifier[] Order => order;
        public float UpDistance => upDistance;
        public float RiseTime => riseTime;
        public float RiseTimeChangeStep => riseTimeChangeStep;
    }
}