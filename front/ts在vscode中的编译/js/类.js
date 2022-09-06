(() => {
    class Person {
        constructor(firstName, lastName) {
            this.firstName = firstName;
            this.lastName = lastName;
            this.fullName = this.firstName + '_' + this.lastName;
        }
    }
    const person = new Person('诸葛', '孔明');
    console.log(person.fullName);
})();
