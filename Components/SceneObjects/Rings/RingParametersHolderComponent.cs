using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Rings, Doc.Holder, "here we hol ring parameters")]
    public sealed class RingParametersHolderComponent : BaseComponent
    {
        [SerializeField] private float ringOperationValue;
        [SerializeField] private ModifierCalculationType rightRingOperatoinType;
        [SerializeField] private bool isMoving;
        [SerializeField] private bool isHorizontal;

        public float RingOperationValue => ringOperationValue;
        public ModifierCalculationType RightRingOperatoinType => rightRingOperatoinType;
        public bool IsMoving => isMoving;
        public bool IsHorizontal => isHorizontal;

        public void SetRingParameters (float value, ModifierCalculationType operatoinType, bool isRingMoving, bool isHorizontalRing)
        {
            rightRingOperatoinType = operatoinType;
            ringOperationValue = value;
            isMoving = isRingMoving;
            isHorizontal = isHorizontalRing;
        }

        public void ChangeValue(float newValue)
        {
            ringOperationValue = newValue;
        }

        public void ChangeOperationType(ModifierCalculationType operatoinType)
        {
            rightRingOperatoinType = operatoinType;
        }
    }
}