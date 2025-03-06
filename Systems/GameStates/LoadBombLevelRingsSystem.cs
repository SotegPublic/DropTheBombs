using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Commands;

namespace Systems
{
	[Serializable][Documentation(Doc.State, "LoadBombLevelRingsSystem")]
    public sealed class LoadBombLevelRingsSystem : BaseGameStateSystem, IGlobalStart
    {
        [Required]
        private RingsHolderComponent ringsHolder;
        [Required]
        private RingsOffsetComponent ringsOffsetComponent;
        [Required]
        private BombLevelHolder levelHolder;
        [Required]
        private BombLevelVariablesComponent levelVariables;


        protected override int State => GameStateIdentifierMap.LoadBombLevelRings;

        private PlayerProgressComponent playerProgress;
        private LevelsHolderComponent levelsConfigHolder;
        private List<UniTask> taskList = new List<UniTask>(64);
        private int goodNodsCount;


        public void GlobalStart()
        {
            playerProgress = Owner.World.GetSingleComponent<PlayerProgressComponent>();
            levelsConfigHolder = Owner.World.GetSingleComponent<LevelsHolderComponent>();
        }

        public override void InitSystem()
        {
        }

        protected override async void ProcessState(int from, int to)
        {
            goodNodsCount = 0;
            taskList.Clear();

            var levelConfig = levelsConfigHolder.GetLevelConfigByIndex(playerProgress.BombLevelIndex - 1);
            var levelMonoComponent = levelHolder.BombLevel;
            var fallPoints = levelConfig.FallSpawnPoints;
            var currentPosition = new Vector3(
                levelMonoComponent.LastSpawnTransform.position.x,
                levelMonoComponent.LastSpawnTransform.position.y,
                levelMonoComponent.LastSpawnTransform.position.z
                );

            SpawnRings(fallPoints, ref currentPosition, levelConfig.IntervalBetweenRingsOrTrapsVertical, true);

            currentPosition.y += levelConfig.IntervalBetweenRingsOrTrapsVertical;
            levelVariables.DropBombPosition = currentPosition;

            currentPosition.z -= levelConfig.IntervalBetweenRingsOrTrapsHorizontal;
            var flyPoints = levelConfig.FlySpawnPoints;

            SpawnRings(flyPoints, ref currentPosition, levelConfig.IntervalBetweenRingsOrTrapsHorizontal, false);

            levelVariables.StartLevelPosition = currentPosition;

            await UniTask.WhenAll(taskList);

            Owner.World.GetSingleComponent<CurrentLevelProgressComponent>().RingsNodsCount = goodNodsCount;

            EndState();
        }

        private void SpawnRings(List<SpawnPointConfig> points, ref Vector3 currentPosition, float interval, bool isVertical)
        {
            for (int i = points.Count - 1; i >= 0; i--)
            {
                if ((points[i].IsSingle && points[i].IsSingleLeft && points[i].IsTrapLeft) ||
                    (points[i].IsSingle && !points[i].IsSingleLeft && points[i].IsTrapRight) ||
                    (!points[i].IsSingle && (points[i].IsTrapLeft && points[i].IsTrapRight)))
                {
                    if (isVertical)
                    {
                        currentPosition.y += interval;
                    }
                    else
                    {
                        currentPosition.z -= interval;
                    }

                    continue;
                }

                var targetTag = isVertical ? "Bombs" : "Plane";

                if (points[i].IsSingle)
                {
                    if(!points[i].IsSingleLeft)
                    {
                        var ringContainer = ringsHolder.GetRingByID(points[i].RightBonusType, points[i].RightRingOperatoinType);
                        var offset = points[i].IsSingleLeft ? ringsOffsetComponent.RingsOffset : -ringsOffsetComponent.RingsOffset;
                        var command = new RingInitCommand
                        {
                            RingOperationValue = points[i].RightRingOperationValue,
                            RingOperatoinType = points[i].RightRingOperatoinType,
                            IsMoving = points[i].IsMoving,
                            TargetTag = targetTag,
                            isHorizontal = !isVertical
                        };

                        taskList.Add(SpawnRing(ringContainer, command, offset, currentPosition, isVertical));

                        if(points[i].RightRingOperatoinType != ModifierCalculationType.Divide && points[i].RightRingOperatoinType != ModifierCalculationType.Subtract)
                        {
                            goodNodsCount++;
                        }
                    }
                    else
                    {
                        var ringContainer = ringsHolder.GetRingByID(points[i].LeftBonusType, points[i].LeftRingOperatoinType);
                        var offset = points[i].IsSingleLeft ? ringsOffsetComponent.RingsOffset : -ringsOffsetComponent.RingsOffset;
                        var command = new RingInitCommand
                        {
                            RingOperationValue = points[i].LeftRingOperationValue,
                            RingOperatoinType = points[i].LeftRingOperatoinType,
                            IsMoving = points[i].IsMoving,
                            TargetTag = targetTag,
                            isHorizontal = !isVertical
                        };

                        taskList.Add(SpawnRing(ringContainer, command, offset, currentPosition, isVertical));

                        if (points[i].LeftRingOperatoinType != ModifierCalculationType.Divide && points[i].LeftRingOperatoinType != ModifierCalculationType.Subtract)
                        {
                            goodNodsCount++;
                        }
                    }
                }
                else
                {
                    var goodRingsCount = 0;

                    if (!points[i].IsTrapRight)
                    {
                        var ringContainer = ringsHolder.GetRingByID(points[i].RightBonusType, points[i].RightRingOperatoinType);
                        var offset = -ringsOffsetComponent.RingsOffset;
                        var command = new RingInitCommand
                        {
                            RingOperationValue = points[i].RightRingOperationValue,
                            RingOperatoinType = points[i].RightRingOperatoinType,
                            IsMoving = false,
                            TargetTag = targetTag,
                            isHorizontal = !isVertical
                        };

                        taskList.Add(SpawnRing(ringContainer, command, offset, currentPosition, isVertical));

                        if (points[i].RightRingOperatoinType != ModifierCalculationType.Divide && points[i].RightRingOperatoinType != ModifierCalculationType.Subtract)
                        {
                            goodRingsCount++;
                        }
                    }

                    if (!points[i].IsTrapLeft)
                    {
                        var ringContainer = ringsHolder.GetRingByID(points[i].LeftBonusType, points[i].LeftRingOperatoinType);
                        var offset = ringsOffsetComponent.RingsOffset;
                        var command = new RingInitCommand
                        {
                            RingOperationValue = points[i].LeftRingOperationValue,
                            RingOperatoinType = points[i].LeftRingOperatoinType,
                            IsMoving = false,
                            TargetTag = targetTag,
                            isHorizontal = !isVertical
                        };

                        taskList.Add(SpawnRing(ringContainer, command, offset, currentPosition, isVertical));

                        if (points[i].LeftRingOperatoinType != ModifierCalculationType.Divide && points[i].LeftRingOperatoinType != ModifierCalculationType.Subtract)
                        {
                            goodRingsCount++;
                        }
                    }

                    if(goodRingsCount != 0)
                    {
                        goodNodsCount++;
                    }
                }

                if (isVertical)
                {
                    currentPosition.y += interval;
                }
                else
                {
                    currentPosition.z -= interval;
                }
            }
        }

        private async UniTask SpawnRing(EntityContainer ringContainer, RingInitCommand command, float offset, Vector3 spawnPosition, bool isVertical)
        {
            var ringActor = await ringContainer.GetActor();

            ringActor.Init();

            var job = new WaitFor<ViewReadyTagComponent>(ringActor.Entity);
            var awaiter = job.RunJob();

            await awaiter;

            ringActor.Entity.Command(command);
            var ringTransform = ringActor.Entity.GetTransform();

            

            if (isVertical)
            {
                ringTransform.position = new Vector3(spawnPosition.x, spawnPosition.y, spawnPosition.z + offset);
                ringTransform.Rotate(0, 90, 0);
            }
            else
            {
                ringTransform.position = new Vector3(spawnPosition.x + offset, spawnPosition.y, spawnPosition.z);
                ringTransform.Rotate(-90, 0, 0);
            }
        }
    }
}