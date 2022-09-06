<template>

<div class="login">   
  <h4>后台管理</h4>
  <el-form label-width="80px" :model="loginData">
    <el-form-item label="账号">
      <el-input v-model="loginData.name" placeholder="请输入用户名" />
    </el-form-item>
    <el-form-item label="密码">
     <el-input v-model="loginData.password" placeholder="请输入密码" show-password />
   </el-form-item>
      <el-form-item>
    <el-button class="sub-btn" @click="subFun" type="primary">登录</el-button>
     </el-form-item>
  </el-form>123012301230
</div>
</template> 
<script>
 import { reactive } from "vue"
 import { ElMessage } from 'element-plus'
 import {login} from '../http/api' 
 import router from '../router/index.js'
export default {
  name: "Login",
  setup() {
    let loginData = reactive({
      name: "",
      password: "",
    });
    let subFun=()=>{
        if(!loginData.name||!loginData.password)
        {
          ElMessage.error('请填写账号密码重试');
          return;
        }
            login(loginData).then(res=>{
              console.log(res)
              router.push('/Home')
            })
        
    };
    return {
      loginData,
      subFun
    };
  },
};
</script>

<style scoped>
.login{
  width:500px;
  margin: 150px auto;
  border: 1px solid #efefef;
  border-radius: 10px;
  padding: 20px;
}
h4{
  text-align: center;
}
.sub-btn{
  width: 100%;
}
</style>