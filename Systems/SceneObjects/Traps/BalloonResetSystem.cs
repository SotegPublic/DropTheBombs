using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;

namespace Systems
{
	[Serializable][Documentation(Doc.Traps, Doc.Reset, "this system reset balloon")]
    public sealed class BalloonResetSystem : BaseSystem, IReactCommand<ResetEntityCommand> 
    {
        public void CommandReact(ResetEntityCommand command)
        {
            Owner.AsActor().GetComponentInChildren<BalloondMonoComponent>().ResetBaloon();
        }

        public override void InitSystem()
        {
        }
    }
}