using System;
using System.IO;
using System.Net;

namespace WebApp
{
    class Program
    {
        private static HttpListener _listener;
        static void Main(string[] args)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://localhost:8088/");
            _listener.Start();
            _listener.BeginGetContext(new AsyncCallback(Program.ProcessRequest), null);
            
            Console.ReadLine();
        }

        static void ProcessRequest(IAsyncResult result)
        {
            HttpListenerContext context = _listener.EndGetContext(result);
            HttpListenerRequest request = context.Request;

            //Answer getCommand/get post data/do whatever
            string postData;
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                postData = reader.ReadToEnd();
                //use your favourite json parser here
                Console.WriteLine(postData);
            }
            _listener.BeginGetContext(new AsyncCallback(Program.ProcessRequest), null);
        }

        static void SendMsg(HttpListenerContext context) 
        {
            string responseString = "This could be json to be parsed by the extension";

            HttpListenerResponse response = context.Response;
            response.ContentType = "text/html";

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            Stream output = response.OutputStream;

            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

        /*
         <!DOCTYPE HTML>
<html>
<head>
<script>
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


crawler.putDatabase=function(){
	//post请求
var request = crawler.createCORSRequest("post","http://localhost:8088/");
if(request){
var reqpars = JSON.stringify({"Shortcode":"","OringinalJson":"","IsCrawlerRequest":"true"});
request.setRequestHeader("Content-Type","application/x-www-form-urlencoded");
request.onload=function(){

};
request.send(reqpars);
}
}
function startApp() {

crawler.putDatabase();

	var evt = document.createEvent("CustomEvent");
	evt.initCustomEvent('myCustomEvent', true, false, "im information");
	// fire the event
	document.dispatchEvent(evt);
}
 
</script>
</head>
<body>
 
<button type="button" onClick="startApp()" id="startApp">startApp</button>
</body>
</html>
         
         */
    }
}
