using UnityEngine;
using Helpers;
using System;
using Sirenix.OdinInspector;
using HECSFramework.Core;

[Serializable]
public class SpawnPointConfig
{
    [SerializeField] private bool isSingle;
    [SerializeField][ShowIf(nameof(isSingle))] private bool isSingleLeft;
    [SerializeField][ShowIf(nameof(isSingle))] private bool isMoving;
    

    [BoxGroup("Right group")]
    [SerializeField][HideIfGroup("Right group/@isSingle && isSingleLeft")] private bool isTrapRight;
    [SerializeField, IdentifierDropDown(nameof(RingIdentifier))][HideIfGroup("Right group/@isSingle && isSingleLeft")][HideIf("isTrapRight")] private int rightBonusType;
    [SerializeField][HideIfGroup("Right group/@isSingle && isSingleLeft")][HideIf("isTrapRight")] private ModifierCalculationType rightRingOperatoinType;
    [SerializeField][HideIfGroup("Right group/@isSingle && isSingleLeft")][HideIf("isTrapRight")] private float rightRingOperationValue;

    [SerializeField, IdentifierDropDown(nameof(TrapIdentifier))][HideIfGroup("Right group/@isSingle && isSingleLeft")][ShowIf("isTrapRight")] private int rightTrapID;
    [SerializeField][HideIfGroup("Right group/@isSingle && isSingleLeft")][ShowIf("isTrapRight")] private int rightTrapDestroyBombsCount;


    [BoxGroup("Left group")]
    [SerializeField][HideIfGroup("Left group/@isSingle && !isSingleLeft")] private bool isTrapLeft;
    [SerializeField, IdentifierDropDown(nameof(RingIdentifier))][HideIfGroup("Left group/@isSingle && !isSingleLeft")][HideIf("isTrapLeft")] private int leftBonusType;
    [SerializeField][HideIfGroup("Left group/@isSingle && !isSingleLeft")][HideIf("isTrapLeft")] private ModifierCalculationType leftRingOperatoinType;
    [SerializeField][HideIfGroup("Left group/@isSingle && !isSingleLeft")][HideIf("isTrapLeft")] private float leftRingOperationValue;

    [SerializeField, IdentifierDropDown(nameof(TrapIdentifier))][HideIfGroup("Left group/@isSingle && !isSingleLeft")][ShowIf("isTrapLeft")] private int leftTrapID;
    [SerializeField][HideIfGroup("Left group/@isSingle && !isSingleLeft")][ShowIf("isTrapLeft")] private int leftTrapDestroyBombsCount;

    public bool IsSingle => isSingle;
    public bool IsSingleLeft => isSingleLeft;
    public bool IsMoving => isMoving;
    public bool IsTrapRight => isTrapRight;
    public bool IsTrapLeft => isTrapLeft;
    public int RightBonusType => rightBonusType;
    public ModifierCalculationType RightRingOperatoinType => rightRingOperatoinType;
    public float RightRingOperationValue => rightRingOperationValue;
    public int LeftBonusType => leftBonusType;
    public ModifierCalculationType LeftRingOperatoinType => leftRingOperatoinType;
    public float LeftRingOperationValue => leftRingOperationValue;
    public int RightTrapID => rightTrapID;
    public int RightTrapDestroyBombsCount => rightTrapDestroyBombsCount;
    public int LeftTrapID => leftTrapID;
    public int LeftTrapDestroyBombsCount => leftTrapDestroyBombsCount;

    public SpawnPointConfig(bool isSingle, bool isSingleLeft, bool isMoving, bool isTrapRight,
        int rightBonusType, ModifierCalculationType rightRingOperatoinType, float rightRingOperationValue,
        int rightTrapID, int rightTrapDestroyBombsCount, bool isTrapLeft, int leftBonusType,
        ModifierCalculationType leftRingOperatoinType, float leftRingOperationValue, int leftTrapID, int leftTrapDestroyBombsCount)
    {
        this.isSingle = isSingle;
        this.isSingleLeft = isSingleLeft;
        this.isMoving = isMoving;
        this.isTrapRight = isTrapRight;
        this.rightBonusType = rightBonusType;
        this.rightRingOperatoinType = rightRingOperatoinType;
        this.rightRingOperationValue = rightRingOperationValue;
        this.rightTrapID = rightTrapID;
        this.rightTrapDestroyBombsCount = rightTrapDestroyBombsCount;
        this.isTrapLeft = isTrapLeft;
        this.leftBonusType = leftBonusType;
        this.leftRingOperatoinType = leftRingOperatoinType;
        this.leftRingOperationValue = leftRingOperationValue;
        this.leftTrapID = leftTrapID;
        this.leftTrapDestroyBombsCount = leftTrapDestroyBombsCount;
    }
}


