using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;

namespace Systems
{
	[Serializable][Documentation(Doc.Bombs, Doc.Counters, "this system control bombs counters via commands")]
    public sealed class BombsCountersControllSystem : BaseSystem, IReactCommand<UpdatePowerCommand>, IReactCommand<UpdateBombsCountCommand>
    {
        [Required]
        private BombPowerComponent bombPower;
        [Required]
        private BombCountComponent bombsCount;

        public void CommandReact(UpdateBombsCountCommand command)
        {
            var newCount = 0;

            switch (command.CalculationType)
            {
                case ModifierCalculationType.Add:
                    newCount = (int)(bombsCount.Value + command.Count);
                    break;
                case ModifierCalculationType.Subtract:
                    newCount = (int)(bombsCount.Value - command.Count);
                    break;
                case ModifierCalculationType.Multiply:
                    newCount = (int)(bombsCount.Value * command.Count);
                    break;
                case ModifierCalculationType.Divide:
                    newCount = (int)(bombsCount.Value/command.Count);
                    break;
            }

            newCount = Math.Clamp(newCount, 1, 81);

            bombsCount.SetValue(newCount);
        }

        public void CommandReact(UpdatePowerCommand command)
        {
            var newPower = 0f;

            switch (command.CalculationType)
            {
                case ModifierCalculationType.Add:
                    newPower = bombPower.Value + command.Value;
                    break;
                case ModifierCalculationType.Subtract:
                    newPower = bombPower.Value - command.Value;
                    break;
                case ModifierCalculationType.Multiply:
                    newPower = bombPower.Value * command.Value;
                    break;
                case ModifierCalculationType.Divide:
                    newPower = bombPower.Value / command.Value;
                    break;
            }

            newPower = newPower < 1 ? 1 : newPower;

            bombPower.SetValue(newPower);
        }

        public override void InitSystem()
        {
        }
    }
}