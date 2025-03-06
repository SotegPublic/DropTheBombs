mergeInto(LibraryManager.library, {
	
	SaveExtern: function (date) 
	{
		var dateString = UTF8ToString(date);
		var myObj = JSON.parse(dateString);
		player.setData(myObj);
		
		console.log('data saved');
	},
	
	LoadExtern: function () 
	{
		player.getData().then(_date => {
			const myJSON = JSON.stringify(_date);
			console.log('myJSON');
			myGameInstance.SendMessage('YandexReceiver', 'LoadExternCallback', myJSON);
		});
	},
	
	SaveToLocalStorage : function(key, value) {
		console.log('saveToLocal');
		
		try {
			localStorage.setItem(UTF8ToString(key), UTF8ToString(value));
		}
		catch (e) {
			console.error('Save to Local Storage error: ', e.message);
		}
	},

	LoadFromLocalStorage : function(key) {
		console.log('loadFromLocal');
		var returnStr = localStorage.getItem(UTF8ToString(key));
		var bufferSize = lengthBytesUTF8(returnStr) + 1;
		var buffer = _malloc(bufferSize);
		stringToUTF8(returnStr, buffer, bufferSize);
		return buffer;
	},
			
	HasKey : function(key) {
		try 
		{
			if (localStorage.getItem(UTF8ToString(key))) {
			  return 1;
			}
			else {
			  return 0;
			}
		}
		catch (e) {
			console.error('Has key in Local Storage error: ', e.message);
			return 0;
		}
	},
});