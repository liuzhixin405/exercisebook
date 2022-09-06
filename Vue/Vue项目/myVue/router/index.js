router=new VueRouter({
routes:[
    {path:"/",redirect:"/main"},
    {path:"/jobList",component:jobList},
    {path:"/main",component:main},
    ]
})