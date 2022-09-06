var port = null; 
chrome.runtime.onMessage.addListener(
  function(request, sender, sendResponse) {
	  
     if (request.type == "launch"){
       	connectToNativeHost(request.message);
    }
	return true;
});
 
 
//onNativeDisconnect
function onDisconnected()
{
	console.log(chrome.runtime.lastError);
	console.log('disconnected from native app.');
	port = null;
}
 
function onNativeMessage(message) {
	console.log('recieved message from native app: ' + JSON.stringify(message));
}
 
//connect to native host and get the communicatetion port
function connectToNativeHost(msg)
{
	
	var nativeHostName = "com.my_company.my_application";
	console.log(nativeHostName);
 	port = chrome.runtime.connectNative(nativeHostName);
	port.onMessage.addListener(onNativeMessage);
	port.onDisconnect.addListener(onDisconnected);
	port.postMessage({message: msg});	
 }