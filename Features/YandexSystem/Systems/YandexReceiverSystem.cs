using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using Cysharp.Threading.Tasks;
using Components;
using Newtonsoft.Json;
using System.Threading;
using Commands;

namespace Systems
{
	[Serializable][Documentation(Doc.Yandex, "This system provides access to YandexSDK")]
    public sealed class YandexReceiverSystem : BaseSystem, IGlobalStart, IReactGlobalCommand<LoadExternCallbackCommand>
    {
        private YandexReceiver yandexReceiver;
        private SaveAndLoadDataProvider saveAndLoadDataProvider;
        private AuthenticateStatusComponent authStatusComponent;
        private CancellationTokenSource tokenSource;
        private string externJSON;

        public YandexReceiver YandexReceiver => yandexReceiver;

        public void GlobalStart()
        {
            authStatusComponent = Owner.World.GetSingleComponent<AuthenticateStatusComponent>();
        }

        public override void InitSystem()
        {
            yandexReceiver = Owner.AsActor().GetComponent<YandexReceiver>();
            saveAndLoadDataProvider = Owner.AsActor().GetComponent<SaveAndLoadDataProvider>();
        }

        public void SavePlayerData(string playerData)
        {
            if(authStatusComponent.IsAuthenticated)
            {
                saveAndLoadDataProvider.SaveGameExtern(playerData);
            }
            else
            {
                saveAndLoadDataProvider.SaveGameLocal(playerData);
            }
        }

        public async UniTask<string> LoadPlayerData()
        {
            if (authStatusComponent.IsAuthenticated)
            {
                tokenSource = new CancellationTokenSource();
                yandexReceiver.YandexDebug("We create tokenSource");

                saveAndLoadDataProvider.LoadFromCloud();

                await tokenSource.Token.WaitUntilCanceled();

                tokenSource.Dispose();
                tokenSource = null;

                yandexReceiver.YandexDebug("We load extern save");

                return externJSON;
            }
            else
            {
                if (saveAndLoadDataProvider.TryLoadLocal(out var save))
                {
                    return save;
                }

                return null;
            }
        }

        public void CommandGlobalReact(LoadExternCallbackCommand command)
        {
            externJSON = command.JsonData;
            yandexReceiver.YandexDebug("We get loaded command");

            if (tokenSource != null)
            {
                yandexReceiver.YandexDebug("We cancel token");
                tokenSource.Cancel();
            }
        }
    }
}