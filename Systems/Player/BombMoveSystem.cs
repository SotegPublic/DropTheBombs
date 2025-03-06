using System;
using HECSFramework.Core;
using UnityEngine;
using Components;
using UnityEngine.InputSystem;
using Commands;

namespace Systems
{
    [Serializable]
    [Documentation(Doc.Player, Doc.Movement, "move player by input")]
    public sealed class BombMoveSystem : BaseSystem, IReactCommand<InputCommand>, IReactCommand<InputStartedCommand>, IReactCommand<InputEndedCommand>, IUpdatable, IInitAfterView, IGlobalStart, IReactGlobalCommand<TransitionGameStateCommand>
    {
        [Required]
        public UnityTransformComponent UnityTransformComponent;

        [Required]
        public MoveSpeedComponent MoveSpeedComponent;

        [Required]
        public SideMoveSpeedComponent SideMoveSpeed;

        private EdgeComponent edge;
        private InputAction swipePositionUpdateAction;
        private InputAction mousePositionUpdateAction;
        private Vector2 startSwipePosition;
        private Vector2 startMousePosition;
        private Vector2 currentSwipePosition;
        private Vector2 currentMousePosition;
        private Vector3 bombStartPosition;
        private MainCameraComponent cameraComponent;
        private bool isMouseActive;

        private Transform transform;
        private Transform viewTransform;

        private bool isActive;


        public void GlobalStart()
        {
            edge = Owner.World.GetSingleComponent<EdgeComponent>();
        }

        public void CommandGlobalReact(TransitionGameStateCommand command)
        {
            if(command.To == GameStateIdentifierMap.BombVerticalGameState)
            {
                isActive = true;
            }
        }


        public void CommandReact(InputStartedCommand command)
        {
            if (command.Index == InputIdentifierMap.Fire)
            {
                startSwipePosition = swipePositionUpdateAction.ReadValue<Vector2>();
                startMousePosition = mousePositionUpdateAction.ReadValue<Vector2>();
                isMouseActive = true;
                bombStartPosition = viewTransform.localPosition;
            }
        }

        public void CommandReact(InputEndedCommand command)
        {
            if (command.Index == InputIdentifierMap.Fire)
            {
                isMouseActive = false;
            }
        }

        public void CommandReact(InputCommand command)
        {
            if (command.Index == InputIdentifierMap.Move && !isMouseActive)
            {
                ViewButtonsMove(command);
            }

            if (command.Index == InputIdentifierMap.MainClickPosition)
            {
                ViewSwipeMove(command);
            }

            if (command.Index == InputIdentifierMap.MouseClikPosition && isMouseActive)
            {
                ViewMouseMove(command);
            }
        }

        private void ViewSwipeMove(InputCommand command)
        {
            currentSwipePosition = swipePositionUpdateAction.ReadValue<Vector2>();

            if (startSwipePosition == Vector2.zero)
                startSwipePosition = currentSwipePosition;

            var startToSpace = cameraComponent.Camera.ScreenToWorldPoint(new Vector3(startSwipePosition.y, startSwipePosition.x, cameraComponent.Camera.nearClipPlane));
            var currentToSpace = cameraComponent.Camera.ScreenToWorldPoint(new Vector3(currentSwipePosition.y, currentSwipePosition.x, cameraComponent.Camera.nearClipPlane));

            MoveBomb(startToSpace, currentToSpace);
        }

        private void ViewMouseMove(InputCommand command)
        {
            currentMousePosition = mousePositionUpdateAction.ReadValue<Vector2>();

            if (startMousePosition == Vector2.zero)
                startMousePosition = currentMousePosition;

            var startToSpace = cameraComponent.Camera.ScreenToWorldPoint(new Vector3(startMousePosition.y, startMousePosition.x, cameraComponent.Camera.nearClipPlane));
            var currentToSpace = cameraComponent.Camera.ScreenToWorldPoint(new Vector3(currentMousePosition.y, currentMousePosition.x, cameraComponent.Camera.nearClipPlane));

            MoveBomb(startToSpace, currentToSpace);
        }

        private void MoveBomb(Vector3 startToSpace, Vector3 currentToSpace)
        {
            var inputDir = currentToSpace - startToSpace;

            var newPos = viewTransform.transform.localPosition;
            var delta = inputDir.x * SideMoveSpeed.SwipeSpeed;

            newPos.y = bombStartPosition.y - delta;
            newPos.y = Mathf.Clamp(newPos.y, -edge.Edge, edge.Edge);

            viewTransform.localPosition = newPos;
        }

        private void ViewButtonsMove(InputCommand command)
        {
            var inputDir = command.Context.ReadValue<Vector2>();

            var delta = inputDir.x * SideMoveSpeed.ButtonSpeed * Time.deltaTime;
            var currentPosX = viewTransform.localPosition.x;
            var currentPosY = viewTransform.localPosition.y;
            var currentPosZ = viewTransform.localPosition.z;

            viewTransform.localPosition = new Vector3(currentPosX, Math.Clamp(currentPosY - delta, -edge.Edge, edge.Edge), currentPosZ);
        }

        public void InitAfterView()
        {
            viewTransform = Owner.GetComponent<ViewReadyTagComponent>().View.transform;
        }

        public override void InitSystem()
        {
            transform = UnityTransformComponent.Transform;
            Owner.World.GetSingleComponent<InputActionsComponent>().TryGetInputAction(InputIdentifierMap.MainClickPosition, out swipePositionUpdateAction);
            Owner.World.GetSingleComponent<InputActionsComponent>().TryGetInputAction(InputIdentifierMap.MouseClikPosition, out mousePositionUpdateAction);
            cameraComponent = Owner.World.GetSingleComponent<MainCameraComponent>();
        }

        public void UpdateLocal()
        {
            if (!isActive)
                return;

            if (Owner.TryGetComponent<StopBombMoveTagComponent>(out var component))
                return;

            var currentPos = transform.position;
            var newPos = currentPos + transform.forward * MoveSpeedComponent.MoveSpeed * Time.deltaTime;
            transform.position = newPos;
        }



        public void Reset()
        {
        }
    }
}