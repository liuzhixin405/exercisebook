Vue.createApp({
template:"#my-app",
data(){
    return {
        books:[
            {
            id:1,
            name:'厨艺大全',
            date:'2022-01',
            price:65.00,
            count:1
        },
        {
            id:2,
            name:'超级大理财',
            date:'2021-06',
            price:15.00,
            count:4
        },
        {
            id:3,
            name:'旅游去哪儿',
            date:'2022-01',
            price:65.00,
            count:1
        },
        {
            id:4,
            name:'c#入门',
            date:'2022-01',
            price:65.00,
            count:1
        },
        {
            id:5,
            name:'编程珠玑',
            date:'1990-03',
            price:125.00,
            count:10
        }
    ]
    
}
},
computed:{
    totalPrice:function(){
        let finalPrice=0;
        for(let book of this.books){
           
            finalPrice +=book.count *book.price;
        }
        return "¥"+finalPrice;
    },
    filterBooks(){
        return this.books.map(item=>{
            const newItem = Object.assign({},item);
            newItem.price = "¥"+item.price;
            return newItem;
        })
    }
},
methods:{
    increment(index){
        
        this.books[index].count++;
    },
    decrement(index){
        var count = this.books[index].count;
        if(count >1)
        this.books[index].count--;
    },
    removeBook(index){
        this.books.splice(index,1)
    },
    //第二种方法
    formatPrice(price){

        var obj = JSON.parse(JSON.stringify(this.books)) //深拷贝
        return "¥"+price;
    }
}
}).mount("#app")