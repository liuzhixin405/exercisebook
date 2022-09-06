<template>
  <div class="banner">
    <img  v-for="(v,i) in items" :key="i" :src="v" v-show="n==i"/>
    <div class="banner-circle">
      <ul>
        <li v-for="(v,i) in items" :key="i" :class="n==i?'selected':''"></li>
      </ul>
    </div>
  </div>
</template>

<script>
import {reactive,computed,toRefs,onMounted,onUnmounted,inject} from '@vue/composition-api'
export default {
  setup(){
    //创建响应式数据
    let timer = null; //定时器
    const state = reactive({   //data
      n:2,
      items:inject('items')

    })
    const autoPlay = ()=>{   //方法 
      timer = setInterval(play,2000)
    }
    const play = ()=>{   //方法
      console.log(12345)
      state.n++;
      if(state.n>=state.items.length){
        state.n = 0;
      }
    }
    onMounted(()=>{   //生命周期   挂载完成
      autoPlay();
    })
    onUnmounted(()=>{   //生命周期   销毁
      clearInterval(timer);
    })
    //return state;  //setup函数将响应式数据对象return出去供template使用
    //toRefs函数可以将reactive创建出来的数据转化为响应式的数据
    return {
      ...toRefs(state)
    }
  }
}
</script>

