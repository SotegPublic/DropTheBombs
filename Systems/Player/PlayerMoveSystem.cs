using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using UnityEngine.InputSystem;
using Commands;

namespace Systems
{
    [Serializable]
    [Documentation(Doc.Player, Doc.Movement, "move player by input")]
    public sealed class PlayerMoveSystem : BaseSystem, IReactCommand<InputCommand>, IReactCommand<InputStartedCommand>, IReactCommand<InputEndedCommand>, IUpdatable, IInitAfterView, IGlobalStart
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
        private Vector3 planeStartPosition;
        private Vector2 currentSwipePosition;
        private Vector2 currentMousePosition;
        private MainCameraComponent cameraComponent;
        private bool isMouseActive;
        

        private Transform transform;
        private Transform viewTransform;

        private bool isViewActive;

        public void GlobalStart()
        {
            edge = Owner.World.GetSingleComponent<EdgeComponent>();
        }

        public void CommandReact(InputStartedCommand command)
        {
            if(command.Index == InputIdentifierMap.Fire)
            {
                startSwipePosition = swipePositionUpdateAction.ReadValue<Vector2>();
                startMousePosition = mousePositionUpdateAction.ReadValue<Vector2>();
                isMouseActive = true;
                planeStartPosition = viewTransform.localPosition;
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

            if(command.Index == InputIdentifierMap.MainClickPosition) 
            {
                ViewSwipeMove(command);
            }

            if(command.Index == InputIdentifierMap.MouseClikPosition && isMouseActive)
            {
                ViewMouseMove(command);
            }
        }

        private void ViewSwipeMove(InputCommand command)
        {
            currentSwipePosition = swipePositionUpdateAction.ReadValue<Vector2>();

            if (startSwipePosition == Vector2.zero)
                startSwipePosition = currentSwipePosition;

            var startToSpace = cameraComponent.Camera.ScreenToWorldPoint(new Vector3(startSwipePosition.x, startSwipePosition.y, cameraComponent.Camera.nearClipPlane));
            var currentToSpace = cameraComponent.Camera.ScreenToWorldPoint(new Vector3(currentSwipePosition.x, currentSwipePosition.y, cameraComponent.Camera.nearClipPlane));

            MovePlane(startToSpace, currentToSpace);
        }

        private void ViewMouseMove(InputCommand command)
        {
            currentMousePosition = mousePositionUpdateAction.ReadValue<Vector2>();

            if (startMousePosition == Vector2.zero)
                startMousePosition = currentMousePosition;

            var startToSpace = cameraComponent.Camera.ScreenToWorldPoint(new Vector3(startMousePosition.x, startMousePosition.y, cameraComponent.Camera.nearClipPlane));
            var currentToSpace = cameraComponent.Camera.ScreenToWorldPoint(new Vector3(currentMousePosition.x, currentMousePosition.y, cameraComponent.Camera.nearClipPlane));

            MovePlane(startToSpace, currentToSpace);
        }

        private void MovePlane(Vector3 startToSpace, Vector3 currentToSpace)
        {
            var inputDir = currentToSpace - startToSpace;

            var newPos = viewTransform.transform.localPosition;
            var delta = inputDir.x * SideMoveSpeed.SwipeSpeed;

            newPos.x = planeStartPosition.x + delta;
            newPos.x = Mathf.Clamp(newPos.x, -edge.Edge, edge.Edge);

            viewTransform.localPosition = newPos;
        }

        private void ViewButtonsMove(InputCommand command)
        {
            var WSADInputDir = command.Context.ReadValue<Vector2>();

            var delta = WSADInputDir.x * SideMoveSpeed.ButtonSpeed * Time.deltaTime;
            var currentPosX = viewTransform.localPosition.x;
            var currentPosY = viewTransform.localPosition.y;
            var currentPosZ = viewTransform.localPosition.z;

            viewTransform.localPosition = new Vector3(Math.Clamp(currentPosX + delta, -edge.Edge, edge.Edge), currentPosY, currentPosZ);
        }

        public void InitAfterView()
        {
            viewTransform = Owner.GetComponent<ViewReadyTagComponent>().View.transform;
            isViewActive = true;
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
            if (!isViewActive) 
                return;
            if (!Owner.TryGetComponent<InputListenerTagComponent>(out var component))
                return;

            var currentPos = transform.position;
            var newPos = currentPos + transform.forward * MoveSpeedComponent.MoveSpeed * Time.deltaTime;
            transform.position = newPos;
        }

        public void Reset()
        {
            isViewActive = false;
        }
    }
}