using Components;
using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Systems
{
    public class BombPositionModel: IEquatable<BombPositionModel>
    {
        private Vector3 fallPosition;
        private Vector3 underPlanePosition;
        private int ringIndex;
        private bool isBusy;
        private Actor bombActor;

        public Vector3 FallPosition => fallPosition;
        public Vector3 UnderPlanePosition => underPlanePosition;
        public int RingIndex => ringIndex;
        public bool IsBusy => isBusy;
        public Actor BombActor => bombActor;

        public void InitModel(Vector3 newfallPosition, Vector3 newUnderPlanePosition)
        {
            fallPosition = newfallPosition;
            underPlanePosition = newUnderPlanePosition;
        }

        public void SetRingIndex (int index)
        {
            ringIndex = index;
        }

        public void TakePosition(Actor bomb, bool isPlaneState)
        {
            bombActor = bomb;
            isBusy = true;

            if (isPlaneState)
            {
                bomb.transform.localPosition = underPlanePosition;
            }
            else
            {
                bomb.transform.localPosition = fallPosition;
            }

            bomb.Entity.GetComponent<BombEngagePositionComponent>().SetPosition(fallPosition, underPlanePosition);

        }

        public void FreePosition()
        {
            bombActor.Entity.GetComponent<BombEngagePositionComponent>().ClearPosition();

            bombActor = null;
            isBusy = false;
        }

        public bool Equals(BombPositionModel other)
        {
            return other.BombActor == bombActor;
        }
    }
}