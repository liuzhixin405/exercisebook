import { createRouter, createWebHashHistory } from 'vue-router'
import Home from '../views/Home.vue'
import Login from '../views/Login.vue'

const routes = [
  {
    path: '/Home',
    name: 'Home',
    component: Home,
    meta:{
      isShow:false,
    },
    children:[
      {
        path: '/courseList',
        name: 'CourseListin',
        meta:{
          isShow:true,
          title:'课程列表'
        },
        component: () => import(/* webpackChunkName: "login" */ '../views/courseList.vue')
      },
      {
        path: '/teacherList',
        name: 'TeacherList',
        meta:{
          isShow:true,
          title:'讲师列表'
        },
        component: () => import(/* webpackChunkName: "login" */ '../views/teacherList.vue')
      },
      {
        path: '/personal',
        name: 'Personal',
        meta:{
          isShow:true,
          title:'个人中心'
        },
        component: () => import(/* webpackChunkName: "login" */ '../views/personal.vue')
      }
    ]
  },
  {
    path: '/',
    name: 'Login',
    component: Login
  }
]

const router = createRouter({
  history: createWebHashHistory(),
  routes
})

export default router
