using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;

namespace Systems
{
	[Serializable][Documentation(Doc.Bombs, Doc.Movement, DocFeature.MoveByCurveFeature, "this system controls the rotation of the bomb during its curved flight")]
    public sealed class BombSteeringSystem : BaseSystem, IPriorityUpdatable
    {
        [Required]
        private SteeringParametersComponent parameters;

        private EntitiesFilter moveAlongInProgress;

        public int Priority => -1;

        public override void InitSystem()
        {
            moveAlongInProgress = Owner.World.GetFilter(Filter.Get<MoveByCurveToV3TagComponent, MoveByCurveToV3InProgressComponent>());
        }

        public void PriorityUpdateLocal()
        {
            foreach (var e in moveAlongInProgress)
            {
                ref var progressData = ref e.GetOrAddComponent<MoveByCurveToV3InProgressComponent>().MoveData;

                var futureProgress = progressData.Progress + parameters.FutureOffset;
                var influenceX = progressData.XAnimationCurve.Evaluate(futureProgress) * progressData.XRandom;
                var influenceY = progressData.YAnimationCurve.Evaluate(futureProgress) * progressData.YRandom;
                var influenceZ = progressData.ZAnimationCurve.Evaluate(futureProgress) * progressData.ZRandom;

                var fãturePosition = Vector3.Lerp(progressData.From, progressData.To, futureProgress);
                fãturePosition += new Vector3(influenceX, influenceY, influenceZ);

                var transform = e.GetComponent<UnityTransformComponent>().Transform;
                transform.LookAt(fãturePosition);
            }
        }
    }
}