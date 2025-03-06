using System;
using Commands;
using Components;
using HECSFramework.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Systems
{
    [Serializable][Documentation(Doc.Bombs,Doc.Movement, "this system calculate positions for bombs")]
    public sealed class BombsPositionSystem : BaseSystem, IReactGlobalCommand<ResetLevelCommand>
    {
        [Required]
        private ActiveBombsHolderComponent activeBombsHolderComponent;
        [Required]
        private BombsPositionSystemParametersComponent parameters;

        private BombPositionModel[] tmpSortingArray;
        private int currentRingIndex;

        public override void InitSystem()
        {
            activeBombsHolderComponent.InitArray(parameters.RingsCount);

            var maxPos = parameters.PositionStep * parameters.RingsCount;
            var matrixSize = (parameters.RingsCount * 2) + 1;

            tmpSortingArray = new BombPositionModel[8 * parameters.RingsCount];

            var currentX = maxPos;
            var currentY = maxPos;

            for (int ik = 0; ik < matrixSize; ik++)
            {
                for (int jk = 0; jk < matrixSize; jk++)
                {
                    var bombPosModel = new BombPositionModel();
                    bombPosModel.InitModel(new Vector3(currentX + Random.Range(-parameters.Bombsoffset, parameters.Bombsoffset), currentY + Random.Range(-parameters.Bombsoffset, parameters.Bombsoffset), 0),
                        new Vector3(currentX / 2, currentY / 2, 0));

                    var ringIndexFromCenter =(int)(Math.Abs(currentX / parameters.PositionStep) <= Math.Abs(currentY / parameters.PositionStep) ?
                        Math.Abs(currentY / parameters.PositionStep) : Math.Abs(currentX / parameters.PositionStep));
                    bombPosModel.SetRingIndex(ringIndexFromCenter);

                    var i = ik + 1;
                    var j = jk + 1;

                    var reversRingIndex = parameters.RingsCount - ringIndexFromCenter;
                    var ringSideLenth = 1 + (ringIndexFromCenter * 2);

                    //var lastRingLastIndex = 4 * reversRingIndex(matrixSize - 2 * reversRingIndex) - to fill the matrix in a spiral 
                    var arrayIndexesSummOnRing = (i - reversRingIndex) + (j - reversRingIndex) - 1;
                    var reversArrayIndexesSummOnRing = (4 * ringSideLenth) - arrayIndexesSummOnRing - 2;

                    var index = 0;

                    if (i <= j)
                    {
                        index = arrayIndexesSummOnRing;
                    }
                    else
                    {
                        index = reversArrayIndexesSummOnRing;
                    }


                    if (ringIndexFromCenter == 0)
                    {
                        activeBombsHolderComponent.SetStartPositionModel(bombPosModel);
                    }
                    else
                    {
                        activeBombsHolderComponent.BombsPositionsByRing[ringIndexFromCenter - 1][index - 1] = bombPosModel;
                    }

                    currentY -= parameters.PositionStep;
                }
                currentX -= parameters.PositionStep;
                currentY = maxPos;
            }

            currentRingIndex = 0;
        }

        public void CommandGlobalReact(ResetLevelCommand command)
        {
            currentRingIndex = 0;
        }

        public BombPositionModel GetZeroPosition()
        {
            return activeBombsHolderComponent.StartPositionModel;
        }

        public BombPositionModel GetPositionModelForPlacing()
        {
            var currentRing = activeBombsHolderComponent.BombsPositionsByRing[currentRingIndex];
            var freePositionsCount = 0;

            for(int i = 0; i < currentRing.Length; i++)
            {
                if (!currentRing[i].IsBusy)
                {
                    tmpSortingArray[freePositionsCount] = currentRing[i];
                    freePositionsCount++;
                }
            }

            if(freePositionsCount == 0)
            {
                currentRingIndex++;
                var nextRing = activeBombsHolderComponent.BombsPositionsByRing[currentRingIndex];
                return nextRing[Random.Range(0, nextRing.Length)];
            }
            else
            {
                return tmpSortingArray[Random.Range(0, freePositionsCount)];
            }
        }

        public BombPositionModel GetPositionModelForDestroy()
        {
            var currentRing = activeBombsHolderComponent.BombsPositionsByRing[currentRingIndex];
            var busyPositionsCount = 0;

            for (int i = 0; i < currentRing.Length; i++)
            {
                if (currentRing[i].IsBusy)
                {
                    tmpSortingArray[busyPositionsCount] = currentRing[i];
                    busyPositionsCount++;
                }
            }

            if (busyPositionsCount == 0)
            {
                currentRingIndex--;
                var nextRing = activeBombsHolderComponent.BombsPositionsByRing[currentRingIndex];
                return nextRing[Random.Range(0, nextRing.Length)];
            }
            else
            {
                return tmpSortingArray[Random.Range(0, busyPositionsCount)];
            }
        }
    }
}