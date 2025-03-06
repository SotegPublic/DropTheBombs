using System;
using HECSFramework.Core;
using Components;
using Commands;
using Cysharp.Threading.Tasks;
using Helpers;
using System.Buffers;

namespace Systems
{
	[Serializable][Documentation(Doc.Bombs, "this system launching bombs into targets")]
    public sealed class LaunchBombsSystem : BaseSystem, IReactCommand<StartBombingCommand>, IGlobalStart
    {
        private ActiveBombsHolderComponent bombsHolder;
        private BombPowerComponent bombsPower;
        private bool isLastLaunch;
        
        public void  CommandReact(StartBombingCommand command)
        {
            if(isLastLaunch)
                return;

            int bombsCount = 0;

            if (command.Target.GetComponent<PilonTagComponent>().PilonID == PilonIdentifierMap.BossPilon)
            {
                bombsCount = bombsHolder.ActivebombsPositionsList.Count;
            }
            else
            {
                bombsCount = (int)Math.Truncate(command.TargetHealth / bombsPower.Value);
            }

            if(command.TargetHealth % bombsPower.Value != 0)
            {
                bombsCount++;
            }

            var activeBombsCount = bombsHolder.ActivebombsPositionsList.Count;

            if (bombsCount >= activeBombsCount)
            {
                bombsCount = activeBombsCount;
                isLastLaunch = true;
            }

            var bombsArray = HECSPooledArray<BombPositionModel>.GetArray(bombsCount).Items;

            for (int i = 0; i < bombsCount; i++)
            {
                var positionModel = bombsHolder.ActivebombsPositionsList[activeBombsCount - 1];
                bombsArray[i] = positionModel;
                bombsHolder.ActivebombsPositionsList.RemoveAt(activeBombsCount - 1);
                activeBombsCount--;
            }

            LaunchBombs(bombsArray, bombsCount, command);
        }

        private async void LaunchBombs(BombPositionModel[] bombsArray, int bombsCount, StartBombingCommand command)
        {
            var targetPositionsCount = command.BombsTargets.Length;
            var targetIndex = 0;

            for (int i = 0; i < bombsCount; i++)
            {
                var positionModel = bombsArray[i];
                var bombActor = positionModel.BombActor;
                bombActor.gameObject.transform.SetParent(null);
                var bombEntity = bombActor.Entity;

                var targetHolder = bombEntity.GetOrAddComponent<BombTargetComponent>();
                targetHolder.Target = command.Target;
                var tag = bombEntity.GetOrAddComponent<MoveByCurveToV3TagComponent>();
                tag.From = bombActor.transform.position;
                tag.To = command.BombsTargets[targetIndex].position;
                tag.DrawRule = MoveByCurveDrawRuleIdentifierMap.DefaultMoveByCurveIdentifier;

                targetIndex++;
                if (targetIndex == targetPositionsCount)
                {
                    targetIndex = 0;
                }

                bombActor.Entity.Command(new PlayLocalVFXCommand { Enable = true, ID = FXIdentifierMap.FireTrail });

                positionModel.FreePosition();
                Owner.World.GetEntityBySingleComponent<PlayerTagComponent>().GetComponent<BombCountComponent>().ChangeValue(-1);

                Owner.GetOrAddComponent<VisualLocalLockComponent>().AddLock();

                if (isLastLaunch && (i + 1 >= bombsCount * 0.5 || bombsCount == 1))
                {
                    Owner.Command(new AllBombsLaunchedCommand());
                    isLastLaunch = false;
                }

                await UniTask.Delay(60);
            }

            ArrayPool<BombPositionModel>.Shared.Return(bombsArray);
        }

        public void GlobalStart()
        {
            bombsHolder = Owner.World.GetSingleComponent<ActiveBombsHolderComponent>();
            bombsPower = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>().GetComponent<BombPowerComponent>();
        }

        public override void InitSystem()
        {

        }
    }
}