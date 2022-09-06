
	var crawler = {};
crawler.createCORSRequest=function (method,url){
var xhr = new XMLHttpRequest();
if("withCredentials" in xhr){
xhr.open(method,url,true);
}else if(typeof XDomainRequest !="undifined"){
xhr = new XDomainRequest();
xhr.open(method,url);
}else
{
	xhr=null;
}
return xhr;
}

crawler.load=function(){
	
    if (!chrome.cookies) {
        chrome.cookies = chrome.experimental.cookies;
      }
	  
   chrome.cookies.getAll({domain: '.instagram.com' }, (cookies)=> {
		var cookieJson = {}
	    for (var i in cookies) {
			var key = cookies[i].name;
			var value = cookies[i].value;
			cookieJson[key]=value;
			
			}

			var request = crawler.createCORSRequest("post","http://localhost:8088/");
			if(request){
			var reqpars = JSON.stringify(cookieJson);
			request.onload=function(){

			};
			request.send(reqpars);
			alert("start working");
			}
    });
}

window.onload = function(){
    crawler.load();
	
}