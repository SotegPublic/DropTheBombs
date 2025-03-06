using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.SocialPlatforms.Impl;

namespace Systems
{
	[Serializable][Documentation(Doc.Save, Doc.Player, "this system save player data")]
    public sealed class SavePlayerSystem : BaseSystem, IReactGlobalCommand<SaveCommand> 
    {
        [Single]
        private YandexReceiverSystem yandexSystem;

        public void CommandGlobalReact(SaveCommand command)
        {
            var saveContainer = new JSONEntityContainer();
            saveContainer.SerializeEntitySavebleOnly(Owner);
            var data = JsonConvert.SerializeObject(saveContainer);
            
            yandexSystem.SavePlayerData(data);
        }

        public override void InitSystem()
        {
        }
    }
}
