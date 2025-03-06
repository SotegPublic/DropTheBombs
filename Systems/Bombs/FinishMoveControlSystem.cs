using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;

namespace Systems
{
	[Serializable][Documentation(Doc.Bombs, "this system controls moving on finish state")]
    public sealed class FinishMoveControlSystem : BaseSystem, IReactCommand<AllBombsLaunchedCommand>
    {
        public override void InitSystem()
        {
        }

        public void CommandReact(AllBombsLaunchedCommand command)
        {
            StopMoving();
        }

        private async void StopMoving()
        {
            Owner.AddComponent<StopBombMoveTagComponent>();

            var job = new WaitForLocalDrawQueue(Owner);

            await job.RunJob();

            Owner.World.Command(new GoToEndStateCommand());
        }
    }
}