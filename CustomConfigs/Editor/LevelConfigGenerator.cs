using Cysharp.Threading.Tasks;
using HECSFramework.Core;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using System;
using System.Runtime.InteropServices.ComTypes;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelConfigGenerator : OdinEditorWindow
{
    [SerializeField] private int startPower;
    [SerializeField] private int startCount;
    [SerializeField] private int targetPower;
    [SerializeField] private int hLenth;
    [SerializeField] private int vLenth;
    [SerializeField] private int levelNumber;

    private const int MAX_COUNT = 81;
    private const float MAX_DEVIATION = 0.15f;
    private const float MIN_DEVIATION = 0.15f;

    private bool isSingle = false;
    private bool isSingleLeft = false;
    private bool isMoving = false;

    private bool isTrapRight = false;
    private int rightBonusType = 0;
    private ModifierCalculationType rightRingOperatoinType = ModifierCalculationType.Add;
    private float rightRingOperationValue = 0;
    private int rightTrapID = 0;
    private int rightTrapDestroyBombsCount = 0;

    private bool isTrapLeft = false;
    private int leftBonusType = 0;
    private ModifierCalculationType leftRingOperatoinType = ModifierCalculationType.Add;
    private float leftRingOperationValue = 0;
    private int leftTrapID = 0;
    private int leftTrapDestroyBombsCount = 0;

    [MenuItem("NotBombs/LevelConfigGenerator")]
    public static void OpenGeneratorWindow()
    {
        GetWindow<LevelConfigGenerator>();
    }

    [Button]
    private void Generate()
    {
        var config = CreateInstance<LevelConfig>();

        var totalSteps = vLenth + hLenth;
        var totalStartPower = startCount * startPower;

        float currentPowerBalance = targetPower - totalStartPower;
        float currentStepsCount = totalSteps;
        float currentBombPower = startPower;
        float currentBombsCount = startCount;
        bool isFlySteps = true;


        for (int i = 0; i < totalSteps; i++)
        {
            var stepValue = currentPowerBalance / currentStepsCount;
            var configValue = Random.Range((stepValue * (1 - MAX_DEVIATION)), (stepValue * (1 + MIN_DEVIATION)));

            isSingle = Random.Range(0, 100) > 50 ? true : false;

            if (isSingle)
            {
                var isGood = (Random.Range(0, 20) > 5) || currentStepsCount == 1;
                isSingleLeft = Random.Range(0, 100) > 50;
                isMoving = Random.Range(0, 100) > 50;

                if (isGood)
                {
                    if (isSingleLeft)
                    {
                        CalcGoodNodeParameters(ref configValue, ref currentBombPower, ref currentBombsCount, ref leftBonusType, ref leftRingOperatoinType,
                            ref leftRingOperationValue);
                    }
                    else
                    {
                        CalcGoodNodeParameters(ref configValue, ref currentBombPower, ref currentBombsCount, ref rightBonusType, ref rightRingOperatoinType,
                            ref rightRingOperationValue);
                    }

                    currentPowerBalance -= configValue;
                }
                else
                {
                    var isTrap = Random.Range(0, 10) > 5;

                    if(isTrap)
                    {
                        if(isSingleLeft)
                        {
                            isTrapLeft = true;
                            GetTrap(ref leftTrapID, ref leftTrapDestroyBombsCount);
                        }
                        else
                        {
                            isTrapRight = true;
                            GetTrap(ref rightTrapID, ref rightTrapDestroyBombsCount);
                        }
                    }
                    else
                    {
                        if(isSingleLeft)
                        {
                            GetBadRing(ref leftBonusType, ref leftRingOperationValue, ref leftRingOperatoinType);
                        }
                        else
                        {
                            GetBadRing(ref rightBonusType, ref rightRingOperationValue, ref rightRingOperatoinType);
                        }

                    }
                }
            }
            else
            {
                var isLeftGood = Random.Range(0, 100) > 50;

                if(isLeftGood)
                {
                    CalcGoodNodeParameters(ref configValue, ref currentBombPower, ref currentBombsCount, ref leftBonusType, ref leftRingOperatoinType,
                        ref leftRingOperationValue);

                    var isTrap = Random.Range(0, 10) > 5;

                    if (isTrap)
                    {
                        isTrapRight = true;
                        GetTrap(ref rightTrapID, ref rightTrapDestroyBombsCount);
                    }
                    else
                    {
                        GetBadRing(ref rightBonusType, ref rightRingOperationValue, ref rightRingOperatoinType);
                    }

                }
                else
                {
                    CalcGoodNodeParameters(ref configValue, ref currentBombPower, ref currentBombsCount, ref rightBonusType, ref rightRingOperatoinType,
                        ref rightRingOperationValue);

                    var isTrap = Random.Range(0, 10) > 5;

                    if (isTrap)
                    {
                        isTrapLeft = true;
                        GetTrap(ref leftTrapID, ref leftTrapDestroyBombsCount);
                    }
                    else
                    {
                        GetBadRing(ref leftBonusType, ref leftRingOperationValue, ref leftRingOperatoinType);
                    }
                }

                currentPowerBalance -= configValue;
            }


            isFlySteps = currentStepsCount <= vLenth ? false : true;

            if (isFlySteps)
            {
                config.FlySpawnPoints.Add(GetConfig());
            }
            else
            {
                config.FallSpawnPoints.Add(GetConfig());
            }

            currentStepsCount--;
            ResetValues();
        }

        AssetDatabase.CreateAsset(config, $"Assets/BluePrints/CustomConfigs/LevelConfigs/{levelNumber}_LevelConfig.asset");
    }

    private void GetBadRing(ref int bonusType, ref float ringOperationValue, ref ModifierCalculationType ringOperatoinType)
    {
        bonusType = Random.Range(0, 10) > 5 ? RingIdentifierMap.BadPower : RingIdentifierMap.BadCount;
        ringOperatoinType = Random.Range(0, 10) > 5 ? ModifierCalculationType.Subtract : ModifierCalculationType.Divide;

        if (ringOperatoinType == ModifierCalculationType.Subtract)
        {
            ringOperationValue = Random.Range(1, 100);
        }
        else if (ringOperatoinType == ModifierCalculationType.Divide)
        {
            ringOperationValue = (float)Math.Round(Random.Range(1f, 8f), 1);
        }
    }

    private void GetTrap(ref int trapID, ref int trapDestroyCount)
    {
        var rnd = Random.Range(0, 30);

        if (rnd >= 0 && rnd <= 10)
        {
            trapID = TrapIdentifierMap.TNTBaloon;

        }
        else if (rnd > 10 && rnd <= 20)
        {
            trapID = TrapIdentifierMap.SawBlade;
        }
        else
        {
            trapID = TrapIdentifierMap.Propeller;
        }

        trapDestroyCount = Random.Range(1, 10);
    }

    private void CalcGoodNodeParameters(ref float configValue, ref float currentBombPower, ref float currentBombsCount, ref int bonusType, ref ModifierCalculationType calcType,
        ref float operationValue)
    {
        bonusType = Random.Range(0, 100) > 30 || currentBombsCount >= MAX_COUNT ? RingIdentifierMap.Power : RingIdentifierMap.Count;
        calcType = Random.Range(0, 100) > 30 ? ModifierCalculationType.Add : ModifierCalculationType.Multiply;

        if (bonusType == RingIdentifierMap.Power)
        {
            operationValue = (float)Math.Round(CalcPower(ref currentBombPower, ref currentBombsCount, calcType, configValue),1);
        }
        else if (bonusType == RingIdentifierMap.Count)
        {
            operationValue = (float)Math.Round(CalcCount(ref currentBombPower, ref currentBombsCount, calcType, ref configValue),1);
        }
    }

    private float CalcCount(ref float currentBombPower, ref float currentBombsCount, ModifierCalculationType operatoinType, ref float configValue)
    {
        var operationValue = 1f;
        float newCount;

        switch (operatoinType)
        {
            case ModifierCalculationType.Add:
                operationValue = (int)(configValue / currentBombPower);
                operationValue = operationValue == 0 ? 1 : operationValue;
                newCount = currentBombsCount + operationValue;

                if (newCount > MAX_COUNT)
                {
                    operationValue = (int)(MAX_COUNT - currentBombsCount);
                    
                }

                currentBombsCount += operationValue;
                break;
            case ModifierCalculationType.Multiply:
                operationValue = ((configValue / currentBombPower) + currentBombsCount) / currentBombsCount;
                operationValue = operationValue <= 1 ? 1.2f : operationValue;
                newCount = operationValue * currentBombsCount;

                if(newCount > MAX_COUNT)
                {
                    operationValue = ((int)(MAX_COUNT - currentBombsCount) / currentBombsCount);
                    configValue = (MAX_COUNT - currentBombsCount) * currentBombPower;
                }

                currentBombsCount = operationValue * currentBombsCount;
                break;
        }

        return operationValue;
    }

    private float CalcPower(ref float currentBombPower, ref float currentBombsCount, ModifierCalculationType leftRingOperatoinType, float configValue)
    {
        var operationValue = 1f;

        switch (leftRingOperatoinType)
        {
            case ModifierCalculationType.Add:
                
                operationValue = (int)(configValue / currentBombsCount);
                operationValue = operationValue == 0 ? 1 : operationValue;
                currentBombPower += operationValue;
                return operationValue;

            case ModifierCalculationType.Multiply:

                operationValue = ((configValue / currentBombsCount) + currentBombPower) / currentBombPower;
                operationValue = operationValue <= 1 ? 1.1f : operationValue;

                currentBombPower = operationValue * currentBombPower;
                return operationValue;
        }

        return 1;
    }

    private SpawnPointConfig GetConfig()
    {
        return new SpawnPointConfig(
            isSingle,
            isSingleLeft,
            isMoving,
            isTrapRight,
            rightBonusType,
            rightRingOperatoinType,
            rightRingOperationValue,
            rightTrapID,
            rightTrapDestroyBombsCount,
            isTrapLeft,
            leftBonusType,
            leftRingOperatoinType,
            leftRingOperationValue,
            leftTrapID,
            leftTrapDestroyBombsCount
            );
    }

    private void ResetValues()
    {
        isSingle = false;
        isSingleLeft = false;
        isMoving = false;

        isTrapRight = false;
        rightBonusType = 0;
        rightRingOperatoinType = ModifierCalculationType.Add;
        rightRingOperationValue = 0;
        rightTrapID = 0;
        rightTrapDestroyBombsCount = 0;

        isTrapLeft = false;
        leftBonusType = 0;
        leftRingOperatoinType = ModifierCalculationType.Add;
        leftRingOperationValue = 0;
        leftTrapID = 0;
        leftTrapDestroyBombsCount = 0;
    }
}
