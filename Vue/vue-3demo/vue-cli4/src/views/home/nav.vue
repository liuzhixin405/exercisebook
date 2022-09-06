<template>
  <div class="index-category">
      <div class="category" v-for="(list,index) in navLists" :key="index">
          <i class="iconfont" :class="list.icon" :style="{background:list.color}"></i>
          <label>{{list.title}}</label>
      </div>
  </div>
</template>

<script>

import {reactive,computed,toRefs,onMounted,inject} from '@vue/composition-api'
export default {
  setup(props,{root}){
    //创建响应式数据
    const state = reactive({   //data
      navLists:inject('navLists')
    })
    //return state;  //setup函数将响应式数据对象return出去供template使用
    //toRefs函数可以将reactive创建出来的数据转化为响应式的数据
    return {
      ...toRefs(state)
    }
  }
}
</script>