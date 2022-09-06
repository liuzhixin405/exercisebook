<template>
  <div id="app">
	  <!--上部分-->
  <router-view></router-view>
  <!--底部导航-->
  <NavBttom></NavBttom>
  </div>
</template>
<script>
	import NavBttom from './views/navBottom.vue'
	import {reactive,computed,onMounted,toRefs} from '@vue/composition-api'
	export default
	{
		setup()
		{
			const state = reactive({
				name:'abc',
				id:1,
				n:computed(()=>state.id-1)
			});
			const active=()=>{
				return state.id +=1;
				//console.log(123)
			}
			return {
				...toRefs(state),
				active
			}
		}, //3.0新属性
		components:
		{
			NavBttom
		}
	}
</script>
<style>
@import './assets/css/index.css';
#app .nav-bottom .active {
	color:yellow;
}
</style>

///扩展运算符 ...
<!-- var a1=[12,3] // ...a1 ==[1,2,3] === 1,2,3
 var a2 = [4,5];
 var a3=[...a1,...a2];
 var o1 = {id:1};
 var o2 = {name:'abc'};
 var o3={...o1,...o2}; // {id:1,o2:{name:'abc'}}

 let f=v=>v;
 
 lef f=function(v){
	 return v;
 }
 <template>
   <div id="app">
     测试{{name}}---{{id}}--{{n}}
 	<button @click="active()">click</button>
   </div>
 </template>
 <script>
 	import {reactive,computed,onMounted,toRefs} from '@vue/composition-api'
 	export default
 	{
 		setup()
 		{
 			const state = reactive({
 				name:'abc',
 				id:1,
 				n:computed(()=>state.id-1)
 			});
 			const active=()=>{
 				return state.id +=1;
 				//console.log(123)
 			}
 			return {
 				...toRefs(state),
 				active
 			}
 		} //3.0新属性
 		
 	}
 </script>
 <style>
 
 </style>
  -->
 