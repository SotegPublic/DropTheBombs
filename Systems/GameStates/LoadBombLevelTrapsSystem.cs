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
	[Serializable][Documentation(Doc.State, "LoadBombLevelTrapsSystem")]
    public sealed class LoadBombLevelTrapsSystem : BaseGameStateSystem, IGlobalStart
    {
        [Required]
        private TrapsContainersHolderComponent trapsHolder;
        [Required]
        private TrapsOffsetComponent trapsOffsetComponent;
        [Required]
        private BombLevelHolder levelHolder;


        protected override int State => GameStateIdentifierMap.LoadBombLevelTraps;

        private PlayerProgressComponent playerProgress;
        private LevelsHolderComponent levelsConfigHolder;
        private List<UniTask> taskList = new List<UniTask>(64);

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
            taskList.Clear();

            var levelConfig = levelsConfigHolder.GetLevelConfigByIndex(playerProgress.BombLevelIndex - 1);
            var levelMonoComponent = levelHolder.BombLevel;
            var fallPoints = levelConfig.FallSpawnPoints;
            var currentPosition = new Vector3(
                levelMonoComponent.LastSpawnTransform.position.x,
                levelMonoComponent.LastSpawnTransform.position.y,
                levelMonoComponent.LastSpawnTransform.position.z
                );

            SpawnTraps(fallPoints, ref currentPosition, levelConfig.IntervalBetweenRingsOrTrapsVertical, true);

            currentPosition.y += levelConfig.IntervalBetweenRingsOrTrapsVertical;
            currentPosition.z -= levelConfig.IntervalBetweenRingsOrTrapsHorizontal;
            var flyPoints = levelConfig.FlySpawnPoints;

            SpawnTraps(flyPoints, ref currentPosition, levelConfig.IntervalBetweenRingsOrTrapsHorizontal, false);

            await UniTask.WhenAll(taskList);

            EndState();
        }

        private void SpawnTraps(List<SpawnPointConfig> points, ref Vector3 currentPosition, float interval, bool isVertical)
        {
            for (int i = points.Count - 1; i >= 0; i--)
            {
                var singleLeft = points[i].IsSingle && points[i].IsSingleLeft && !points[i].IsTrapLeft;
                var singleRight = points[i].IsSingle && !points[i].IsSingleLeft && !points[i].IsTrapRight;
                var allNotTraps = !points[i].IsSingle && (!points[i].IsTrapLeft && !points[i].IsTrapRight);

                if (singleLeft || singleRight || allNotTraps)
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
                    if (!points[i].IsSingleLeft)
                    {
                        var trapContainer = trapsHolder.GetTrapByID(points[i].RightTrapID);
                        var offset = points[i].IsSingleLeft ? trapsOffsetComponent.TrapOffset : -trapsOffsetComponent.TrapOffset;
                        var command = new TrapInitCommand
                        {
                            BombsDestroyCount = points[i].RightTrapDestroyBombsCount,
                            IsMoving = points[i].IsMoving,
                            TargetTag = targetTag,
                            IsHorizontal = !isVertical
                        };

                        taskList.Add(SpawnTrap(trapContainer, command, offset, currentPosition, isVertical));
                    }
                    else
                    {
                        var trapContainer = trapsHolder.GetTrapByID(points[i].LeftTrapID);
                        var offset = points[i].IsSingleLeft ? trapsOffsetComponent.TrapOffset : -trapsOffsetComponent.TrapOffset;
                        var command = new TrapInitCommand
                        {
                            BombsDestroyCount = points[i].LeftTrapDestroyBombsCount,
                            IsMoving = points[i].IsMoving,
                            TargetTag = targetTag,
                            IsHorizontal = !isVertical
                        };

                        taskList.Add(SpawnTrap(trapContainer, command, offset, currentPosition, isVertical));
                    }
                }
                else
                {
                    if (points[i].IsTrapRight)
                    {
                        var ringContainer = trapsHolder.GetTrapByID(points[i].RightTrapID);
                        var offset = -trapsOffsetComponent.TrapOffset;
                        var command = new TrapInitCommand
                        {
                            BombsDestroyCount = points[i].RightTrapDestroyBombsCount,
                            IsMoving = false,
                            TargetTag = targetTag,
                            IsHorizontal = !isVertical
                        };


                        taskList.Add(SpawnTrap(ringContainer, command, offset, currentPosition, isVertical));
                    }

                    if (points[i].IsTrapLeft)
                    {
                        var trapContainer = trapsHolder.GetTrapByID(points[i].LeftTrapID);
                        var offset = trapsOffsetComponent.TrapOffset;
                        var command = new TrapInitCommand
                        {
                            BombsDestroyCount = points[i].LeftTrapDestroyBombsCount,
                            IsMoving = false,
                            TargetTag = targetTag,
                            IsHorizontal = !isVertical
                        };

                        taskList.Add(SpawnTrap(trapContainer, command, offset, currentPosition, isVertical));
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

        private async UniTask SpawnTrap(EntityContainer trapContainer, TrapInitCommand command, float Offset, Vector3 spawnPosition, bool isVertical)
        {
            var trapActor = await trapContainer.GetActor();

            trapActor.Init();

            var job = new WaitFor<ViewReadyTagComponent>(trapActor.Entity);
            var awaiter = job.RunJob();

            await awaiter;

            trapActor.Entity.Command(command);
            var trapTransform = trapActor.Entity.GetTransform();



            if (isVertical)
            {
                trapTransform.position = new Vector3(spawnPosition.x, spawnPosition.y, spawnPosition.z + Offset);
            }
            else
            {
                trapTransform.position = new Vector3(spawnPosition.x + Offset, spawnPosition.y, spawnPosition.z);
                if (trapActor.Entity.GetComponent<TrapTagComponent>().TrapID != TrapIdentifierMap.TNTBaloon)
                {
                    trapTransform.Rotate(-90, 0, 0);
                }
            }
        }
    }
}