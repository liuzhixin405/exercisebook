public class Parent { }

public class Child:Parent { }

public interface IContravariant<in T>
{
    void GetInput(T input);
    //T GetT(); //不允许
}

public class Class1<T>: IContravariant<T>
{
    public void GetInput(T input)
    {
        
    }
}

public class Program_OutIn
{
    static void OutIn_Main(string[] args)
    {
        IContravariant<Child> child = new Class1<Parent>();
        //IContravariant<Parent> parent = new Class1<Child>(); //不允许

        //IConvariant<Child> child2 = new Class2<Parent>();  //不允许
        IConvariant<Parent> parent2 = new Class2<Child>();
    }
}

public interface IConvariant<out T>
{
    T GetOut();
}

public class Class2<T>: IConvariant<T> where T : new()
{
    public T GetOut() { return new T(); }
}