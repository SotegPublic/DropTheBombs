using HECSFramework.Core;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Traps, Doc.Holder, "here we hold trap parameters")]
    public sealed class TrapParametersHolder : BaseComponent
    {
        [SerializeField] private int destroyBombsCount;
        [SerializeField] private bool isMoving;
        [SerializeField] private bool isHorizontal;

        public int DestroyBombsCount => destroyBombsCount;
        public bool IsMoving => isMoving;
        public bool IsHorizontal => isHorizontal;

        public void SetTrapParameters(int count, bool isRingMoving, bool isTrapHorizontal)
        {
            destroyBombsCount = count;
            isMoving = isRingMoving;
            isHorizontal = isTrapHorizontal;
        }
    }
}