using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using DG.Tweening;

namespace Systems
{
	[Serializable][Documentation(Doc.Plane, Doc.Movement, "this system control plane tilt when it approaching the edge")]
    public sealed class PlaneTiltSystem : BaseSystem, IUpdatable, IInitAfterView
    {
        [Required]
        private PlaneTiltParametersComponent tiltParameters;

        private GameObject view;
        private bool isViewActive;

        public void InitAfterView()
        {
            view = Owner.GetComponent<ViewReadyTagComponent>().View;
            isViewActive = true;
        }

        public override void InitSystem()
        {
        }

        public void Reset()
        {
            isViewActive = false;
        }

        public void UpdateLocal()
        {
            if (!Owner.TryGetComponent<PlaneTiltSystemNeededTagComponent>(out var component))
                return;

            if (!isViewActive)
                return;

            var newY = tiltParameters.GetYPosition(view.transform.localPosition.x);
            var newRotationZ = tiltParameters.GetRotation(view.transform.localPosition.x);

            view.transform.localPosition = new Vector3(view.transform.localPosition.x, newY, view.transform.localPosition.z);
            view.transform.localRotation = Quaternion.Euler(view.transform.localRotation.z, view.transform.localRotation.y, newRotationZ);
        }
    }
}