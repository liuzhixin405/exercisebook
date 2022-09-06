<template>
  <div class="home">
    <el-container>
      <el-header class="top-box">
          <el-row :gutter="24">
          <el-col :span="18">
              <img class="logo" src="../assets/logo.png" alt="">
          </el-col>
          <el-col :span="6">
            <div class="info-box">
              <span>Tina</span>
              <el-button class="out-btn" type="text">退出</el-button>
            </div>
          </el-col>
        </el-row>
        </el-header>
      <el-container>
        <el-aside width="200px">
            <el-menu
             router='true'
              class="el-menu-vertical-demo">
              <el-menu-item :route="i.path" v-for="i in tabList" :key="i.name" :index="i.name">
                <!-- <i class="el-icon-document"></i> -->
                <template #title>{{i.meta.title}}</template>
              </el-menu-item>
              
            </el-menu>
        </el-aside>
        <el-main>
          <div class="content-box">
            <router-view/>
          </div>
        </el-main>
      </el-container>
    </el-container>
  </div>
</template>

<script>
// @ is an alias to /src
// import {reactive} from "vue";
import router from "../router/index";
export default {
  name: "Home",
  setup() {
    let tabList = router.options.routes[0].children;
    return {
      tabList,
    };
  },
};
</script>
<style lang="scss" scoped>
.top-box {
  background: #efefef;
  padding: 5px;
  .logo {
    width: 50px;
  }
  .info-box {
    text-align: right;
  }
  .out-btn {
    margin-left: 10px;
  }
}
.content-box{
  padding:20px;
}
</style>