<template>
  <div class="detail">
    <div>
      <a class="goback" @click="goBack()">返回</a>
      <div>
        <img v-for="(v,i) in imgData" :key="i" :src="v.image_url">
      </div>
      <div>
        <p>{{infoData.product_name}}</p>
        <p>{{infoData.product_detail}}</p>
      </div>
    </div>
  </div>
</template>

<script>
import { reactive, onMounted, onUnmounted ,toRefs} from "@vue/composition-api"

export default {
  setup(props,{root}) {
    const state = reactive({
      id:1,
      imgData:[],
      infoData:{}
    });
    onMounted(()=>{  //挂载完成  生命周期
      //state.id = root.$route.params.id;
      //console.log(state.id );
      detailData(root.$route.params.id);
      root.$store.dispatch('HIDENAV');  //vuex中的state值隐藏
    })
    onUnmounted(()=>{  //销毁 生命周期
      root.$store.dispatch('SHOWNAV');  //vuex中的state值显示
    })
    const detailData=(id)=>{
      root.$http.get('/detail',{
        params:{
          mId:id
        }
      }).then(res=>{
        console.log(res);
        let {data} = res;
        state.imgData = data[0];
        state.infoData = data[1][0];
      })
    }
    const goBack=()=>{
      root.$router.push('/home')
    }
    return {
      ...toRefs(state),
      goBack
    };
  }
}
</script>

<style scoped>

</style>