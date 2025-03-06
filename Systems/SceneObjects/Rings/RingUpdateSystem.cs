using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;
using DG.Tweening;

namespace Systems
{
	[Serializable][Documentation(Doc.Rings, "this system update ring after bullet hit them")]
    public sealed class RingUpdateSystem : BaseSystem, IReactCommand<UpdateRingCommand>, IReactCommand<BumpRingCommand>, IReactCommand<ResetEntityCommand>, IInitAfterView
    {
        [Required]
        private UpdateParametersComponent parameters;
        [Required]
        private RingParametersHolderComponent ringParameters;
        [Required]
        private RingMonobehHolderComponent monobehHolder;
        [Required]
        private RingTagComponent tagComponent;

        private int hitsCount; 
        private int oldRingType;
        private Vector3 baseScale;
        private Canvas RingCanvas => monobehHolder.Monocomponent.GetComponentInChildren<Canvas>();

        public void CommandReact(UpdateRingCommand command)
        {
            switch(ringParameters.RightRingOperatoinType)
            {
                case ModifierCalculationType.Add:

                    ringParameters.ChangeValue(ringParameters.RingOperationValue + parameters.SubAndAddStep);

                    break;
                case ModifierCalculationType.Subtract:

                    ringParameters.ChangeValue(ringParameters.RingOperationValue - parameters.SubAndAddStep);

                    if (ringParameters.RingOperationValue == 0)
                    {
                        ChangeRing(tagComponent.RingId, ModifierCalculationType.Add, ringParameters.RingOperationValue + parameters.SubAndAddStep);
                    }
                    
                    break;
                case ModifierCalculationType.Multiply:

                    hitsCount++;
                    if(hitsCount == parameters.HitsCountForUpdate)
                    {
                        ringParameters.ChangeValue((float)Math.Round(ringParameters.RingOperationValue + parameters.DivAndMultStep, 1));
                        hitsCount = 0;
                    }

                    break;
                case ModifierCalculationType.Divide:

                    hitsCount++;

                    if (hitsCount == parameters.HitsCountForUpdate)
                    {
                        ringParameters.ChangeValue((float)Math.Round(ringParameters.RingOperationValue - parameters.DivAndMultStep,1));

                        if (ringParameters.RingOperationValue <= 1)
                        {
                            ChangeRing(tagComponent.RingId, ModifierCalculationType.Multiply, (float)Math.Round(ringParameters.RingOperationValue + parameters.DivAndMultStep, 1));
                            hitsCount = 0;
                        }
                    }

                    break;
            }

            UpdateRingTexts();
            BumpRing(true);
        }

        private void BumpRing(bool isHit)
        {
            if (isHit)
            {
                Owner.World.Command(new SpawnFXToCoordCommand
                {
                    Coord = monobehHolder.Monocomponent.transform.position,
                    FXId = FXIdentifierMap.RingHitEffect
                });

                AnimateTransform(RingCanvas.transform);
            }
            else
            {
                Owner.World.Command(new SpawnFXToCoordCommand
                {
                    Coord = monobehHolder.Monocomponent.transform.position,
                    FXId = FXIdentifierMap.RingPassEffect
                });

                AnimateTransform(RingCanvas.transform);
            }
        }

        private void AnimateTransform(Transform transform)
        {
            DOTween.Kill(transform.GetHashCode());
            var scale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);

            var sequence = DOTween.Sequence();
            sequence.SetId(transform.GetHashCode());
            sequence.Append(transform.DOScale(scale * 1.3f, 0.12f));
            sequence.Append(transform.DOScale(scale, 0.12f));

        }


        private void ChangeRing(int ringID, ModifierCalculationType newCalcType, float newValue)
        {
            oldRingType = ringID;
            
            if(ringID == RingIdentifierMap.BadCount)
            {
                ChangeRingColor(true);
                tagComponent.ChangeRingID(RingIdentifierMap.Count);
            }
            else if(ringID == RingIdentifierMap.BadPower)
            {
                ChangeRingColor(false);
                tagComponent.ChangeRingID(RingIdentifierMap.Power);
            }

            ringParameters.ChangeValue(newValue);
            ringParameters.ChangeOperationType(newCalcType);
        }

        private void UpdateRingTexts()
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
        }

        private void ChangeRingColor(bool isCount)
        {
            monobehHolder.Monocomponent.CurrentGO.SetActive(false);
            if(isCount)
            {
                monobehHolder.Monocomponent.CountGO.SetActive(true);
            }
            else
            {
                monobehHolder.Monocomponent.PowerGO.SetActive(true);
            }
        }

        private void ResetRingColor()
        {
            if(monobehHolder.Monocomponent.IsBad)
            {
                monobehHolder.Monocomponent.CurrentGO.SetActive(true);
                monobehHolder.Monocomponent.CountGO.SetActive(false);
                monobehHolder.Monocomponent.PowerGO.SetActive(false);
            }
        }

        public override void InitSystem()
        {
            oldRingType = tagComponent.RingId;
        }

        public void CommandReact(BumpRingCommand command)
        {
            BumpRing(false);
        }

        public void CommandReact(ResetEntityCommand command)
        {
            ResetRingColor();
            tagComponent.ChangeRingID(oldRingType);
            RingCanvas.transform.localScale = baseScale;

            Owner.World.Command(new UnregisterTextFieldCommand
            {
                TextField = monobehHolder.Monocomponent.TypeText,
                TextID = (tagComponent.RingId == RingIdentifierMap.Count || tagComponent.RingId == RingIdentifierMap.BadCount) ?
             LocalizedTextIdentifierMap.BombLocalizedText : LocalizedTextIdentifierMap.PowerLocalizedText
            });
        }

        public void InitAfterView()
        {
            baseScale = RingCanvas.transform.localScale;
        }

        public void Reset()
        {
        }
    }
}