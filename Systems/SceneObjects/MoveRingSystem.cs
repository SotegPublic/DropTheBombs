using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;

namespace Systems
{
	[Serializable][Documentation(Doc.Rings, Doc.Movement, "this system moving rings")]
    public sealed class MoveRingSystem : BaseSystem, IUpdatable, IGlobalStart
    {
        [Required]
        private RingParametersHolderComponent ringParameters;
        [Required]
        private MoveSpeedComponent moveSpeed;

        private EdgeComponent edgeComponent;
        private bool isRightMoving;
        private float dropZ;

        public void GlobalStart()
        {
            edgeComponent = Owner.World.GetSingleComponent<EdgeComponent>();
            dropZ = Owner.World.GetSingleComponent<BombLevelVariablesComponent>().DropBombPosition.z;
        }

        public override void InitSystem()
        {
        }

        public void UpdateLocal()
        {
            if(ringParameters.IsMoving)
            {
                var currentPosition = Owner.GetPosition();
                var offset = 0f;

                if(ringParameters.IsHorizontal)
                {
                    var dirMod = isRightMoving ? 1 : -1;

                    offset = Math.Clamp(currentPosition.x + (moveSpeed.MoveSpeed * Time.deltaTime * dirMod), -edgeComponent.Edge, edgeComponent.Edge);
                    Owner.GetTransform().position = new Vector3(offset, currentPosition.y, currentPosition.z);

                    if(Owner.GetTransform().position.x == edgeComponent.Edge || Owner.GetTransform().position.x == -edgeComponent.Edge)
                    {
                        isRightMoving = !isRightMoving;
                    }
                }
                else
                {
                    var dirMod = isRightMoving ? 1 : -1;

                    offset = Math.Clamp(currentPosition.z + (moveSpeed.MoveSpeed * Time.deltaTime * dirMod), dropZ - edgeComponent.Edge, dropZ + edgeComponent.Edge);
                    Owner.GetTransform().position = new Vector3(currentPosition.x, currentPosition.y, offset);

                    if (Owner.GetTransform().position.z == dropZ + edgeComponent.Edge || Owner.GetTransform().position.z == dropZ - edgeComponent.Edge)
                    {
                        isRightMoving = !isRightMoving;
                    }
                }
            }
        }
    }
}