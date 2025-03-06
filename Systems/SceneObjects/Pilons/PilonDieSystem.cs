using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;
using Cysharp.Threading.Tasks;

namespace Systems
{
	[Serializable][Documentation(Doc.Pilons, "this system control pilon's death")]
    public sealed class PilonDieSystem : BaseSystem, IReactCommand<IsDeadCommand>
    {
        [Required]
        private PilonTagComponent tag;
        [Required]
        private PilonMonoComponentHolderComponent monobehHolderComponent;

        private Collider[] destroedColliders = new Collider[256];
        
        public async void CommandReact(IsDeadCommand command)
        {
            var progressComponent = Owner.World.GetSingleComponent<CurrentLevelProgressComponent>();
            progressComponent.LastKilledPilonID = tag.PilonID;
            var bossIndex = Owner.World.GetSingleComponent<PlayerProgressComponent>().BossIndex;

            if(tag.PilonID == PilonIdentifierMap.BossPilon)
            {
                var bossReward = Owner.World.GetSingleComponent<CurrencyCalculationsConfigComponent>().GetBossReward(bossIndex);
                progressComponent.IsBombLevelBossDie = true;
                progressComponent.CurrentBossKillReward += bossReward;
            }
            else
            {
                var currentPower = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>().GetComponent<BombPowerComponent>().Value;
                var destroyFXID = Owner.World.GetSingleComponent<BombsConfigsHolder>().GetConfigByPower(currentPower).BombDestroyFXID;
                
                Owner.World.Command(new SpawnFXToCoordCommand { Coord = monobehHolderComponent.Monocomponent.Building.transform.position, FXId = destroyFXID });
                Owner.World.Command(new SetCameraShakeCommand { DurationValue = 0.3f, StrengthValue = 1f, Vibrato = 100 });

                monobehHolderComponent.Monocomponent.Building.SetActive(false);
                monobehHolderComponent.Monocomponent.DestroedBuilding.SetActive(true);

                var count = Physics.OverlapSphereNonAlloc(monobehHolderComponent.Monocomponent.DestroedBuilding.transform.position, 20f, destroedColliders);

                for(int i = 0; i < count; i++)
                {
                    if(destroedColliders[i].tag == "Bombs")
                    {
                        continue;
                    }

                    Rigidbody rb = destroedColliders[i].gameObject.GetComponent<Rigidbody>();

                    if (destroedColliders[i].gameObject.activeSelf && rb != null)
                    {
                        rb.isKinematic = false;
                        rb.AddExplosionForce(500, monobehHolderComponent.Monocomponent.DestroedBuilding.transform.position, 20f);
                    }
                }

                var reward = Owner.World.GetSingleComponent<CurrencyCalculationsConfigComponent>().GetPilonReward(bossIndex);
                progressComponent.GainedCurrency += reward;
            }

            monobehHolderComponent.Monocomponent.HealthPanel.SetActive(false);

            if (tag.PilonID != PilonIdentifierMap.BossPilon)
            {
                await UniTask.Delay(2000);
                monobehHolderComponent.Monocomponent.DestroedBuilding.SetActive(false);
            }
        }

        public override void InitSystem()
        {
        }
    }
}