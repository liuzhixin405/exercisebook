(()=>{

    interface IPerson{
        firstName:string
        lastName:string
    }
    class Person{
        firstName:string
        lastName:string
        fullName:string
        constructor(firstName:string,lastName:string){
            this.firstName=firstName
            this.lastName=lastName
            this.fullName = this.firstName+'_'+this.lastName
        }
    }
   
    const person =new Person('诸葛','孔明')
    console.log(person.fullName)
    })()