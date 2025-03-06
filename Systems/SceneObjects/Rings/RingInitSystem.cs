using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using Components;
using Commands;

namespace Systems
{
	[Serializable][Documentation(Doc.Rings, "RingInitSystem")]
    public sealed class RingInitSystem : BaseSystem, IReactCommand<RingInitCommand>, IInitAfterView
    {
        [Required]
        private RingParametersHolderComponent ringParameters;
        [Required]
        private RingTagComponent ringTag;
        [Required]
        private TargetTagHolderComponent targetTag;
        [Required]
        private RingMonobehHolderComponent monobehHolder;

        public void CommandReact(RingInitCommand command)
        {
            ringParameters.SetRingParameters(command.RingOperationValue, command.RingOperatoinType, command.IsMoving, command.isHorizontal);
            targetTag.TargetTag = command.TargetTag;
            UpdateRingCanvas();
        }

        private void UpdateRingCanvas()
        {
            var valueStr = "";

            switch (ringParameters.RightRingOperatoinType)
            {
                case ModifierCalculationType.Add:
                    valueStr = "+" + ringParameters.RingOperationValue;
                    break;
                case ModifierCalculationType.Subtract:
                    valueStr = "-" + ringParameters.RingOperationValue;
                    break;
                case ModifierCalculationType.Multiply:
                    valueStr = "x" + ringParameters.RingOperationValue;
                    break;
                case ModifierCalculationType.Divide:
                    valueStr = "/" + ringParameters.RingOperationValue;
                    break;
            }

            monobehHolder.Monocomponent.ValueText.text = valueStr;

            //monobehHolder.Monocomponent.TypeText.text = (ringTag.RingId == RingIdentifierMap.Count || ringTag.RingId == RingIdentifierMap.BadCount) ?
            //    "BOMB" : "POWER";

            Owner.World.Command(new RegisterTextFieldCommand
            {
                TextField = monobehHolder.Monocomponent.TypeText,
                TextID = (ringTag.RingId == RingIdentifierMap.Count || ringTag.RingId == RingIdentifierMap.BadCount) ?
                         LocalizedTextIdentifierMap.BombLocalizedText : LocalizedTextIdentifierMap.PowerLocalizedText
            });
        }

        public void InitAfterView()
        {
            monobehHolder.Monocomponent = Owner.AsActor().GetComponentInChildren<RingMonoComponent>();
        }

        public override void InitSystem()
        {
        }

        public void Reset()
        {
            
        }
    }
}