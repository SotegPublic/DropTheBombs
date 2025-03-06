using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;

namespace Systems
{
	[Serializable][Documentation(Doc.Pilons, Doc.Level, "this system init pilon")]
    public sealed class InitPilonSystem : BaseSystem, IGlobalStart, IInitAfterView
    {
        [Required]
        private PilonTagComponent pilonTag;
        [Required]
        private PilonMonoComponentHolderComponent monobehHolder;
        [Required]
        private HealthComponent healthComponent;

        public void GlobalStart()
        {
            var bossIndex = Owner.World.GetSingleComponent<PlayerProgressComponent>().BossIndex;
            var pilonHealth = (int)Math.Round(Owner.World.GetSingleComponent<PilonHealthCalculationsConfigComponent>().GetPilonHealth(pilonTag.PilonID, bossIndex), 0);
            var pilonMonobeh = Owner.AsActor().GetComponent<PilonMonocomponent>();

            pilonMonobeh.HealthText.text = pilonHealth.ToString() + "/" + pilonHealth.ToString();
            healthComponent.Setup(pilonHealth);

            pilonMonobeh.xText.text = "x" + Owner.World.GetSingleComponent<PilonHealthCalculationsConfigComponent>().GetPilonXModifier(pilonTag.PilonID).ToString();

            pilonMonobeh.CanvasGroup.alpha = 0;
            pilonMonobeh.CanvasGroup.blocksRaycasts = false;

            monobehHolder.SetMonocomponent(pilonMonobeh);
        }

        public void InitAfterView()
        {
            if(pilonTag.PilonID == PilonIdentifierMap.BossPilon)
            {
                var bossType = Owner.GetComponent<BossTypeComponent>().Type;

                switch (bossType)
                {
                    case BossesTypes.KongBoss:
                        var kongView = Owner.GetComponent<ViewReadyTagComponent>().View;
                        kongView.transform.Rotate(0, 183, 0);
                        kongView.transform.localScale = new Vector3(100f, 100f, 100f);
                        kongView.transform.localPosition = new Vector3(0, -1.6f, 10);
                        break;
                    case BossesTypes.GodzillaBoss:
                        var godzillaView = Owner.GetComponent<ViewReadyTagComponent>().View;
                        godzillaView.transform.Rotate(-20, 180, 0);
                        godzillaView.transform.localScale = new Vector3(1.6f, 1.6f, 1.6f);
                        godzillaView.transform.localPosition = new Vector3(0, -1.6f, 7);
                        break;
                    case BossesTypes.ItMonsterBoss:
                        var itView = Owner.GetComponent<ViewReadyTagComponent>().View;
                        itView.transform.Rotate(0, 180, 0);
                        itView.transform.localScale = new Vector3(80f, 80f, 80f);
                        itView.transform.localPosition = new Vector3(0, -1.6f, 7);
                        break;
                    case BossesTypes.RobotBoss:
                        var roboView = Owner.GetComponent<ViewReadyTagComponent>().View;
                        roboView.transform.Rotate(0, 0, 0);
                        roboView.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                        roboView.transform.localPosition = new Vector3(0, -1.6f, 13);
                        break;
                    case BossesTypes.SquidDollBoss:
                        var dollView = Owner.GetComponent<ViewReadyTagComponent>().View;
                        dollView.transform.localScale = new Vector3(45f, 45f, 45f);
                        dollView.transform.localPosition = new Vector3(0, -1.6f, 5.7f);
                        break;
                    case BossesTypes.ThanosBoss:
                        var thanosView = Owner.GetComponent<ViewReadyTagComponent>().View;
                        thanosView.transform.Rotate(0, 180, 0);
                        thanosView.transform.localScale = new Vector3(80f, 80f, 80f);
                        thanosView.transform.localPosition = new Vector3(0, -1.6f, 7);
                        break;
                    case BossesTypes.VenomBoss:
                        var venomView = Owner.GetComponent<ViewReadyTagComponent>().View;
                        venomView.transform.Rotate(0, 160, 0);
                        venomView.transform.localScale = new Vector3(210f, 210f, 210f);
                        venomView.transform.localPosition = new Vector3(0, -1f, 10);
                        break;
                }
            }
        }

        public override void InitSystem()
        {
        }

        public void Reset()
        {
        }
    }
}