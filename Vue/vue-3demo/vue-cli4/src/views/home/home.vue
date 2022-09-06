<template>
  <div>
    <!-- 轮播图 -->
    <Banner />
    <!-- 类别导航 -->
    <Nav />
    <!-- 列表展示 -->
    <Lists />
  </div>
</template>

<script>
import Banner from './banner.vue'
import Nav from './nav.vue'
import Lists from './lists.vue'
import {reactive,computed,toRefs,provide} from '@vue/composition-api'
export default {
  setup(){
    //创建响应式数据
    const state = reactive({   //data
    items:[
        require('@/assets/img/1.jpg'),
        require('@/assets/img/2.jpg'),
        require('@/assets/img/3.jpg'),
        require('@/assets/img/4.jpg')
      ],
    navLists:[
        {title:'美食',icon:'iconfenlei',color:'#19d5d6'},
        {title:'电影',icon:'iconfaxian1',color:'#ff1054'},
        {title:'住宿',icon:'iconcompass',color:'#02a76a'},
        {title:'娱乐',icon:'iconcruise-ship',color:'#dd0dd1'},
        {title:'外卖',icon:'iconcruise-ship',color:'#0294fa'},
        {title:'打车',icon:'iconcruise-ship',color:'#f54704'},
        {title:'景点',icon:'iconcruise-ship',color:'#ec9be0'},
        {title:'旅游',icon:'iconcruise-ship',color:'#c7da0e'}
    ]
    })
    const active = ()=>{   //方法
     
    }
    //return state;  //setup函数将响应式数据对象return出去供template使用
    //toRefs函数可以将reactive创建出来的数据转化为响应式的数据
    provide('items',state.items), //组件传参
    provide('navLists',state.navLists) //组件传参
    return {
      ...toRefs(state)
    }
  },
  components:{
    Banner,
    Nav,
    Lists
  }
}
</script>
