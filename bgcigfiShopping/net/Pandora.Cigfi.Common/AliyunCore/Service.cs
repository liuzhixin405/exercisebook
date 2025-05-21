using AliYunCore;

namespace Service{
    class AliYunClient{
        private DefaultProfile profile;

        public AliYunClient(DefaultProfile profile){
            this.profile = profile;
        }

        public string getResponse(Request req){
            if (string.Equals("cn-shanghai",profile.regionId)){
                req.domain = "green.cn-shanghai.aliyuncs.com";
            }
            req.accessKeyId = this.profile.accessKeyId;
            req.accessKeySecret = this.profile.accessKeySecret;
            return HttpClient.getResponse(req);
        }


    }

}