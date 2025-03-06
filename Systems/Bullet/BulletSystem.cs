using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;
using TMPro;

namespace Systems
{
	[Serializable][Documentation(Doc.Abilities, "this system controlled bullet fly")]
    public sealed class BulletSystem : BaseSystem, IUpdatable, IReactCommand<DestroyBulletCommand>, IInitAfterView
    {
        [Required]
        private ProgressComponent progressComponent;
        [Required]
        private StartPositionHolderComponent startPositionHolderComponent;
        [Required]
        private AnimationCurveComponent animationCurveComponent;
        [Required]
        private AttackDistanceComponent attackDistance;

        private Transform transform;
        public bool isActive;

        public void CommandReact(DestroyBulletCommand command)
        {
            isActive = false;
            Owner.World.Command(new DestroyEntityWorldCommand { Entity = Owner });
        }

        public override void InitSystem()
        {
            transform = Owner.GetComponent<UnityTransformComponent>().Transform;
        }

        public void UpdateLocal()
        {
            if (!isActive)
                return;

            var speed = animationCurveComponent.AnimationCurve.Evaluate(progressComponent.Value);
            progressComponent.ChangeValue(Time.deltaTime * speed);

            var endPosition = new Vector3(startPositionHolderComponent.StartPosition.x, startPositionHolderComponent.StartPosition.y, startPositionHolderComponent.StartPosition.z + attackDistance.Distance);

            var direction = Vector3.Lerp(startPositionHolderComponent.StartPosition, endPosition, progressComponent.Value);

            transform.position = direction;

            if (progressComponent.Value >= 1)
            {
                progressComponent.SetValue(0);
                Owner.World.Command(new DestroyEntityWorldCommand { Entity = Owner });
            }
        }

        public void InitAfterView()
        {
            isActive = true;
        }

        public void Reset()
        {
        }
    }
}