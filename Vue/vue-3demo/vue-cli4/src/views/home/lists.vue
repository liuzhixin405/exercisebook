<template>
  <div class="index-main">
      <ul>
          <li class="lists" v-for="v in items" :key="v.product_id">
              <router-link :to="'/detail/'+v.product_id">
                <img :src="v.product_img_url" >
              </router-link>
              <label>
                  <b class="discount">折扣价:{{v.product_uprice}}</b>
                  <span class="price-text">原价:{{v.product_price}}</span> 
              </label>            
          </li> 
      </ul>  
  </div>
</template>

<script>
import {reactive,computed,toRefs,onMounted} from '@vue/composition-api'
export default {
  setup(props,{root}){
    //创建响应式数据
    const state = reactive({   //data
      items:[]
    })
    const getData = ()=>{   //方法
      root.$http.get('/home/page/1/20').then(res=>{
        console.log(res);
        if(res.status == '200'){
          let {data} = res.data;   //对象解构
          state.items = data;  //数据渲染
        }
        
      })
    }
    onMounted(()=>{   //生命周期   挂载完成
      getData();
    })
    //return state;  //setup函数将响应式数据对象return出去供template使用
    //toRefs函数可以将reactive创建出来的数据转化为响应式的数据
    return {
      ...toRefs(state)
    }
  }
}
</script>