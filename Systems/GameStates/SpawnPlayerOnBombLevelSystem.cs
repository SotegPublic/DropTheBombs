using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Cysharp.Threading.Tasks;
using Commands;

namespace Systems
{
	[Serializable][Documentation(Doc.Player, Doc.Spawn, "this state spawn player on bomb level")]
    public sealed class SpawnPlayerOnBombLevelSystem : BaseGameStateSystem 
    {
        protected override int State => GameStateIdentifierMap.SpawnPlayerOnBombLevel;

        public override void InitSystem()
        {
        }

        protected async override void ProcessState(int from, int to)
        {
            var plane = (await Owner.World.Request<UniTask<PlaneSpawned>>()).Plane;
            await Owner.World.Request<UniTask<BombSpawned>, ConnectBombCommand>(new ConnectBombCommand { Plane = plane });

            EndState();
        }
    }
}