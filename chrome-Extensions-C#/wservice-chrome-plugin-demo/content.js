var launch_message="launch_message";
document.addEventListener('myCustomEvent', function(evt) {
chrome.runtime.sendMessage({type:"launch", message: evt.detail}, function(response) {
  console.log(response)
});
}, false);