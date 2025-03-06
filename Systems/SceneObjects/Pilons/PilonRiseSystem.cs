using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;

namespace Systems
{
	[Serializable][Documentation(Doc.Pilons, "this system rise pilon")]
    public sealed class PilonRiseSystem : BaseSystem, IReactCommand<PilonRiseCommand>, IUpdatable
    {
        private bool isActive;
        private Vector3 startPosition;
        private Vector3 endPosition;
        private Transform pilonTransform;
        private float riseTime;
        private float currentTime;


        public void CommandReact(PilonRiseCommand command)
        {
            startPosition = Owner.GetPosition();
            riseTime= command.RiseTime;
            endPosition = new Vector3(startPosition.x, startPosition.y + command.RiseDistance, startPosition.z);
            pilonTransform = Owner.GetTransform();
            isActive = true;
        }

        public override void InitSystem()
        {
        }

        public void UpdateLocal()
        {
            if (!isActive)
                return;

            currentTime += Time.deltaTime;
            var newPosition = Vector3.Lerp(startPosition, endPosition, currentTime / riseTime);
            pilonTransform.position = newPosition;

            if(currentTime > riseTime)
            {
                isActive = false;
                currentTime = 0;
                Owner.Command(new PilonRiseEndCommand());
            }
        }
    }
}