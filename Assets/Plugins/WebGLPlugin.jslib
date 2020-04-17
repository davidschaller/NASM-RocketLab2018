var WebGLPlugin = { 
	Init: function()
	{
		if (!document.getElementById('unityInited')){
			var unityInit = document.createElement("P");
			unityInit.id = 'unityInited';
			unityInit.style.position = 'fixed';
			unityInit.style.top = '-500px';
			unityInit.style.left = '-500px';
			unityInit.style.height = '10px';
			unityInit.style.width = '10px';
			unityInit.style.display = 'none';
			document.body.appendChild(unityInit);
            if(UnityLoader.SystemInfo.mobile){
                gameInstance.SendMessage("OptionsData", "SetAsTouchDevice")
            } else {
                var btn = document.getElementById("fullscreenBtn");
                if(btn){
                    if(navigator.maxTouchPoints && navigator.maxTouchPoints > 2){ //new iPad workaround
                        gameInstance.SendMessage("OptionsData", "SetAsTouchDevice")
                        btn.style.display = 'none';
                    }else{
                        btn.style.display = 'block';
                    }
                }
            }
		}
	},
};
mergeInto(LibraryManager.library, WebGLPlugin);