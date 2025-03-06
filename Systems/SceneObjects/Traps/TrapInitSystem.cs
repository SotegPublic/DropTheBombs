using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;

namespace Systems
{
	[Serializable][Documentation(Doc.Traps, "this system init trap")]
    public sealed class TrapInitSystem : BaseSystem, IReactCommand<TrapInitCommand>
    {
        [Required]
        private TrapParametersHolder trapParameters;
        [Required]
        private TargetTagHolderComponent targetTag;

        public void CommandReact(TrapInitCommand command)
        {
            trapParameters.SetTrapParameters(command.BombsDestroyCount, command.IsMoving, command.IsHorizontal);
            targetTag.TargetTag = command.TargetTag;
        }

        public override void InitSystem()
        {
        }
    }
}