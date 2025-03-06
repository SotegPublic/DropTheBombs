using System;
using HECSFramework.Core;
using Components;
using Commands;
using Cysharp.Threading.Tasks;
using System.Security.Cryptography;

namespace Systems
{
	[Serializable][Documentation(Doc.Pilons, "this system controll pilons uprising")]
    public sealed class RisePilonsSystem : BaseSystem, IReactGlobalCommand<TransitionGameStateCommand>
    {
        [Required]
        private RiseSystemParametersComponent parameters;

        private EntitiesFilter pilons;
        private float currentRiseTime;

        public async void CommandGlobalReact(TransitionGameStateCommand command)
        {
            currentRiseTime = parameters.RiseTime;

            if (command.To == GameStateIdentifierMap.BombHorizontalGameState)
            {
                for (int i = 0; i < parameters.Order.Length; i++)
                {
                    for (int j = 0; j < pilons.Count; j++)
                    {
                        if (pilons[j].GetComponent<PilonTagComponent>().PilonID == parameters.Order[i])
                        {
                            pilons[j].Command(new PilonRiseCommand { RiseDistance = parameters.UpDistance, RiseTime = currentRiseTime });
                        }
                    }

                    currentRiseTime += parameters.RiseTimeChangeStep;
                    await UniTask.Delay(80);
                }
            }
        }

        public override void InitSystem()
        {
            pilons = Owner.World.GetFilter(Filter.Get<PilonTagComponent>());
        }
    }
}