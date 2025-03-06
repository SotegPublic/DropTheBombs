using System;
using HECSFramework.Core;
using Components;
using Commands;
using Cysharp.Threading.Tasks;

namespace Systems
{
	[Serializable][Documentation(Doc.GameState, "BombLevelHubSystem")]
    public sealed class BombLevelHubSystem : BaseGameStateSystem, IReactGlobalCommand<OnClickStartButtonCommand>, IGlobalStart        
    {
        [Required]
        private PlanesSoundsHolderComponent planesSounds;


        private MoveSpeedComponent moveSpeed;

        protected override int State => GameStateIdentifierMap.BombLevelHub;

        public void CommandGlobalReact(OnClickStartButtonCommand command)
        {
            if(command.ButtonActorID == EntityContainersMap._StartBombButtonPanelContainer)
            {
                var planeEntity = Owner.World.GetEntityBySingleComponent<PlayerPlaneTagComponent>();

                planeEntity.RemoveComponent<PlaneSwayingSystemNeededTagComponent>();
                planeEntity.AddComponent<PlaneTiltSystemNeededTagComponent>();

                EndState();
            }
        }

        public void GlobalStart()
        {
            moveSpeed = Owner.World.GetEntityBySingleComponent<BombLevelFeatureTagComponent>().GetComponent<MoveSpeedComponent>(); 
        }

        public override void InitSystem()
        {
        }

        protected override void ProcessState(int from, int to)
        {        
            Owner.World.GetEntityBySingleComponent<PlayerPlaneTagComponent>().GetComponent<MoveSpeedComponent>().MoveSpeed = moveSpeed.MoveSpeed;
            Owner.World.GetEntityBySingleComponent<PlayerBombTagComponent>().GetComponent<MoveSpeedComponent>().MoveSpeed = moveSpeed.MoveSpeed;

            Owner.World.Command(new SetBombHubStateCommand());
            Owner.World.Command(new HideLoadScreenCommand());

            Owner.World.Command(new ShowUICommand { UIViewType = UIIdentifierMap.MoneyScreenUI_UIIdentifier });
            Owner.World.Command(new ShowUICommand { UIViewType = UIIdentifierMap.SettingsButton_UIIdentifier });
            Owner.World.Command(new ShowUICommand { UIViewType = UIIdentifierMap.LevelProgressPanel_UIIdentifier });
            Owner.World.Command(new ShowUICommand { UIViewType = UIIdentifierMap.LevelsCounterPanel_UIIdentifier });
            Owner.World.Command(new ShowUICommand { UIViewType = UIIdentifierMap.HubElementsPanel_UIIdentifier });
            Owner.World.Command(new ShowUICommand { UIViewType = UIIdentifierMap.UpgradesPanel_UIIdentifier });
            Owner.World.Command(new ShowUICommand { UIViewType = UIIdentifierMap.StartBombButtonScreen_UIIdentifier });

            var plane = Owner.World.GetEntityBySingleComponent<PlayerPlaneTagComponent>();
            var planeID = Owner.World.GetSingleComponent<PlayerPlaneCustomisationComponent>().CurrentPlaneID;
            
            if (planesSounds.TryGetSoundByID(planeID, out var clip))
            {
                Owner.World.Command(new PlaySoundCommand { AudioType = SoundType.FX, IsRepeatable = true, Owner = plane.GUID, Clip = clip });
            }

            var playerEntity = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>();
            var planeEntity = Owner.World.GetEntityBySingleComponent<PlayerPlaneTagComponent>();
            var levelProgress = playerEntity.GetComponent<CurrentLevelProgressComponent>();

            levelProgress.GameType = GameTypes.Bomb;
            planeEntity.AddComponent<PlaneSwayingSystemNeededTagComponent>();

            //Owner.World.GetSingleComponent<CurrentLevelProgressComponent>().IsBombLevelBossDie = true;
        }
    }
}