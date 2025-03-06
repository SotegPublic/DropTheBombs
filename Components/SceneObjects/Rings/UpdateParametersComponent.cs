using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Rings, "here we set update step parameters")]
    public sealed class UpdateParametersComponent : BaseComponent
    {
        [SerializeField] private float subAndAddStep = 1f;
        [SerializeField] private float divAndMultStep = 0.1f;
        [SerializeField] private int hitsCountForUpdate = 2;

        public float SubAndAddStep => subAndAddStep;
        public float DivAndMultStep => divAndMultStep;
        public int HitsCountForUpdate => hitsCountForUpdate;
    }
}