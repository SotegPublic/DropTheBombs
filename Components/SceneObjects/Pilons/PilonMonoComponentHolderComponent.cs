using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Holder, Doc.Pilons, "here we hold pilon monocomponent")]
    public sealed class PilonMonoComponentHolderComponent : BaseComponent
    {
        [SerializeField] private PilonMonocomponent monocomponent;

        public PilonMonocomponent Monocomponent => monocomponent;

        public void SetMonocomponent(PilonMonocomponent component)
        {
            monocomponent = component;
        }
    }
}