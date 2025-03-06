using System;
using Commands;
using Components;
using HECSFramework.Core;
using Newtonsoft.Json;
using UnityEngine;

namespace Systems
{
    [Serializable][Documentation(Doc.Player, "LoadPlayerSystem")]
    public sealed class LoadPlayerSystem : BaseGameStateSystem 
    {
        [Required]
        private PlayerProgressComponent playerProgress;

        [Required]
        private PlayerPlaneCustomisationComponent playerCastomisation;

        [Single]
        private YandexReceiverSystem yandexSystem;

        [Single]
        private CheckinCheckSystem checkinCheckSystem;

        protected override int State { get; } = GameStateIdentifierMap.LoadPlayer;

        public override void InitSystem()
        {
        }

        protected async override void ProcessState(int from, int to)
        {
            var playerData = await yandexSystem.LoadPlayerData();

            if(playerData != null )
            {
                LoadToPlayerData(playerData);
                yandexSystem.YandexReceiver.YandexDebug("We load player data");
                //LoadBaseData();
                //yandexSystem.YandexReceiver.YandexDebug("We load base data");
            }
            else
            {
                LoadBaseData();
                yandexSystem.YandexReceiver.YandexDebug("We load base data");
            }

            UpdateSound();


            yandexSystem.YandexReceiver.UpdateLeaderBoard("MaximumLevel", 3, true, 0);
            yandexSystem.YandexReceiver.UpdateLeaderBoard("MaximumLevel", 3, true, 5);


            EndState();
        }

        private void LoadToPlayerData(string save)
        {
            var player = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>();

            var container = JsonConvert.DeserializeObject<JSONEntityContainer>(save);
            container.DeserializeToEntity(player);

            if(!playerProgress.IsSaveInited)
            {
                yandexSystem.YandexReceiver.YandexDebug("We load first base data");
                LoadBaseData();
            }

            UpdateDailyRewards();
        }

        private void LoadBaseData()
        {
            InitDefaultLanguageSettings();

            playerProgress.IsSoundOff = false;
            playerProgress.IsHapticsOff = true;


            playerCastomisation.CheckPlanesCollection();

            playerCastomisation.SetPlaneUnlocked(PlaneIdentifierMap.Cucuruso);
            playerCastomisation.SetPlaneOwned(PlaneIdentifierMap.Cucuruso);
            playerCastomisation.SetPlaneInstalled(PlaneIdentifierMap.Cucuruso);

            //playerCastomisation.SetPlaneUnlocked(PlaneIdentifierMap.Suchoi);
            //playerCastomisation.SetPlaneOwned(PlaneIdentifierMap.Suchoi);
            //playerCastomisation.SetSkinUnlocked(PlaneIdentifierMap.Suchoi, PlanePaintIdentifierMap.Picaso);
            //playerCastomisation.SetSkinUnlocked(PlaneIdentifierMap.Suchoi, PlanePaintIdentifierMap.Haki);

            // Do not set skin unlocked in planes without skins pack
            //playerCastomisation.SetSkinUnlocked(PlaneIdentifierMap.Cucuruso, PlanePaintIdentifierMap.Base);
            playerCastomisation.SetSkinOwned(PlaneIdentifierMap.Cucuruso, PlanePaintIdentifierMap.Base);
            playerCastomisation.SetSkinInstalled(PlaneIdentifierMap.Cucuruso, PlanePaintIdentifierMap.Base);
            playerCastomisation.GetPlane(PlaneIdentifierMap.Cucuruso).SetPlaneIsNew(false);

            playerProgress.IsSaveInited = true;

            //playerProgress.Currency = 2200;

            UpdateDailyRewards();

            Owner.World.Command(new SaveCommand());

            yandexSystem.YandexReceiver.YandexDebug($"saved plane id = {playerCastomisation.CurrentPlaneID}");
        }

        private void UpdateDailyRewards()
        {
            if (playerProgress.UnlockedDailyRewards == 0)
            {
                playerProgress.UnlockedDailyRewards = 9;
            }
            else if (playerProgress.ReceivedDailyRewards == playerProgress.UnlockedDailyRewards)
            {
                if(checkinCheckSystem.IsNewDay())
                {
                    playerProgress.UnlockedDailyRewards += 9;
                    Owner.World.Command(new SaveCommand());
                }
            }
        }

        private void UpdateSound()
        {
            Owner.World.Command(new ChangeSoundSettingsCommand { IsSoundOn = !playerProgress.IsSoundOff });
        }

        private void InitDefaultLanguageSettings()
        {
            var lang = yandexSystem.YandexReceiver.GetLang();

            switch (lang)
            {
                case "ru":
                    playerProgress.CurrentLanguage = LanguageTypes.RUS;
                    break;
                case "tr":
                    playerProgress.CurrentLanguage = LanguageTypes.TRK;
                    break;
                case "en":
                default:
                    playerProgress.CurrentLanguage = LanguageTypes.ENG;
                    break;
            }
        }
    }
}