mergeInto(LibraryManager.library, {

	CheckYandexSDK: function()
	{
		if (ysdk !== null) 
		{
			console.log('yasdk loaded');
			return 1;
		}
		else
		{
			console.log('yasdk not loaded');
			return 0;
		}
	},
	
	CheckAuth: function()
	{
		initPlayer().then(_player => {
			if (_player.getMode() === 'lite') 
			{
				console.log('auth not');
				myGameInstance.SendMessage('YandexReceiver', 'AfterAuthCheckCallback', 0);
			}
			else 
			{
				console.log('auth done');
				myGameInstance.SendMessage('YandexReceiver', 'AfterAuthCheckCallback', 1);
			}
		}).catch(err => {
			console.log('error when player init');
		});
	},
	
	Debug: function(text)
	{
		var dText = UTF8ToString(text);
		console.log(dText);
	},
	
	
	YaAuth: function()
	{
		if (player.getMode() === 'lite') 
		{
			ysdk.auth.openAuthDialog().then(() => 
			{
				console.log('auth done');
				initPlayer()
				.then(_player =>
				{
					console.log('auth player init');
					myGameInstance.SendMessage('YandexReceiver', 'AuthCallback', 1);
				})
				.catch(err => {
					console.log('error when player init');
				});
			}).catch(() => {
				console.log('auth not');
				myGameInstance.SendMessage('YandexReceiver', 'AuthCallback', 0);
			});
		}
	},
	
	RateGame: function () 
	{
		ysdk.feedback.canReview()
		.then(({ value, reason }) => 
		{
			if (value) 
			{
				ysdk.feedback.requestReview()
					.then(({ feedbackSent }) => 
					{
						console.log(feedbackSent);
					})
			} 
			else 
			{
				console.log(reason)
			}
		})
	},
	
	GetDeviceType: function () 
	{
		var data = ysdk.deviceInfo;
		var divType = data.type;
		var bufferSize = lengthBytesUTF8(divType) + 1;
		var buffer = _malloc(bufferSize);
		stringToUTF8(divType, buffer, bufferSize);
		console.log(buffer);
		return buffer;
	},
	
	GetLanguage: function () 
	{
		var lang = ysdk.environment.i18n.lang;
		var bufferSize = lengthBytesUTF8(lang) + 1;
		var buffer = _malloc(bufferSize);
		stringToUTF8(lang, buffer, bufferSize);
		console.log('load language');
		console.log(buffer);
		return buffer;
	},
	
	SendReady: function()
	{
		if (ysdk !== null && ysdk.features.LoadingAPI !== undefined && ysdk.features.LoadingAPI !== null && initGame !== true) {
			ysdk.features.LoadingAPI.ready();
			initGame = true;
			console.log('Game Ready');
			FocusGame();
		}
	},
	
	GetServerTime: function() {
        if (ysdk !== null) {
            var serverTime = ysdk.serverTime().toString();
            var lengthBytes = lengthBytesUTF8(serverTime) + 1;
            var stringOnWasmHeap = _malloc(lengthBytes);
            stringToUTF8(serverTime, stringOnWasmHeap, lengthBytes);
            return stringOnWasmHeap;
        }
        return 0;
    },
	
	ShowAdv: function () 
	{
		ysdk.adv.showFullscreenAdv({
			callbacks: {
				onClose: function(wasShown) {
					myGameInstance.SendMessage('YandexReceiver', 'ShowAdvCallback');
				},
				onError: function(error) {
					myGameInstance.SendMessage('YandexReceiver', 'ShowAdvCallback');
				}
			}
		})
	},
	
	ShowRewardedAdv: function (rewardId) 
	{
		ysdk.adv.showRewardedVideo({
			callbacks: {
				onOpen: () => {
				  console.log('Video ad open.');
				},
				onRewarded: () => {
					myGameInstance.SendMessage('YandexReceiver', 'GetAdvRewardCallback', rewardId);
				  console.log('Rewarded!');
				},
				onClose: () => {
					myGameInstance.SendMessage('YandexReceiver', 'ShowRewardedAdvCallback', rewardId);
				  console.log('Video ad closed.');
				},
				onError: (e) => {
					myGameInstance.SendMessage('YandexReceiver', 'ShowRewardedAdvCallback', rewardId);
				  console.log('Error while open video ad:', e);
				}
			}
		})
	},
	
	SetLeaderboardInfo: function (leaderBbardID, info)
	{
		if (player.getMode() !== 'lite')
		{
			var boardID = UTF8ToString(leaderBbardID);
			console.log(boardID);
		
			ysdk.getLeaderboards()
				.then(lb => {
					lb.setLeaderboardScore(boardID, info);
				});
			
			console.log('set winstreack');
		}
	},
	
	GetLeaderBoardInfo: function(leaderBbardID, topQuantity, isUserIncluded, aroundQuantity)
	{
		var boardID = UTF8ToString(leaderBbardID);
		console.log(boardID);
		
		ysdk.getLeaderboards().
		then(lb => {
			
			lb.getLeaderboardEntries(boardID, { quantityTop: topQuantity, includeUser: isUserIncluded, quantityAround: aroundQuantity })
			.then(res => {
				console.log(res);
				myGameInstance.SendMessage('YandexReceiver', 'GetLBInfoCallback', JSON.stringify(res));			
			}).catch(err => {
				console.log('error');
				console.log(err);
			});
		});
	},
	
	SetFocus: function()
	{
		container.focus();
		window.focus();
		canvas.focus();
		console.log('back focus');
	},	
});