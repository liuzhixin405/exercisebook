using ObserverSeven;

Teacher teacher = new Teacher();
//teacher.Subscribe(new StudentLi("李逵"));
//teacher.Subscribe(new StudentZhang("张麻子"));
var student1 = new StudentLi("李逵");
var student2 = new StudentZhang("张麻子");
student1.Subscribe(teacher);
student2.Subscribe(teacher);

teacher.SendMessage("明天放假");
teacher.OnCompleted();

//这里学生是多个，也定义可以多个老师，实现多对多关系