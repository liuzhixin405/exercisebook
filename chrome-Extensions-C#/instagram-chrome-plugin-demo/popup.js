var crawler = {};
var token;
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
<!--获取token-->
crawler.getToken=function(){
//post请求
var request = crawler.createCORSRequest("post","http://localhost:8088/token");
if(request){
var reqpars = JSON.stringify({"Account":"Administrator","Password":"12345678"});
request.setRequestHeader("Content-Type","application/json");
request.onload=function(){
token =  JSON.parse(request.responseText).data.token
console.log(token)
};
request.send(reqpars);
}
}
<!--获取ins的post profile-->
crawler.getInsPost = function(){
    if (!chrome.cookies) {
        chrome.cookies = chrome.experimental.cookies;
      }
    chrome.cookies.getAll({domain: '.instagram.com' }, function(cookies) {
	    for (var i in cookies) {
            const XHR= new XMLHttpRequest()
            XHR.open('get',`https://www.instagram.com/p/CDUvvB5ghgL/?__a=1&${i}=${cookies[i]}`)
            XHR.send()
            XHR.onreadystatechange = ()=>{
                if (XHR.readyState==4 && XHR.status==200){
                    console.log(JSON.parse(XHR.responseText).graphql)
                }
            }
            // axios.get(`https://www.instagram.com/p/CDUvvB5ghgL/?__a=1&${i}=${cookies[i]}`).then(res=>{
            //     console.log(res)
            // })
        }
    })
}
<!--post profile写入到数据库-->
crawler.putDatabase=function(){
	//post请求
var request = crawler.createCORSRequest("post","https://unibone.uat.heywind.cn/InstagramPosts/UpdateInstagramPost");
if(request){
var reqpars = JSON.stringify({"Shortcode":"","OringinalJson":"","IsCrawlerRequest":"true"});
request.setRequestHeader("Content-Type","application/json");
request.setRequestHeader("Authorization",`Bearer ${token}`);
request.onload=function(){
token =  JSON.parse(request.responseText).data.token
console.log(token)
};
request.send(reqpars);
}
}

<!--获取指令队列数据-->
crawler.getCommandQueue=function(){
	//post请求
var request = crawler.createCORSRequest("post","https://unibone.uat.heywind.cn/Tarpa/CommandQueues/GetCommandQueueList");
if(request){
var reqpars = JSON.stringify({"Shortcode":"","OringinalJson":"","IsCrawlerRequest":"true"});
request.setRequestHeader("Content-Type","application/json");
request.setRequestHeader("Authorization",`Bearer ${token}`);
request.onload=function(){
token =  JSON.parse(request.responseText).data.token
console.log(token)
};
request.send(reqpars);
}
}
window.onload = function(){
    //crawler.getInsPost();
	crawler.getToken();
	crawler.
}


