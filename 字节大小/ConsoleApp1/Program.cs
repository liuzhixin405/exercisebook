namespace ConsoleApp1
{
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    record struct Str(string Name);

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public class Person
    {
        public string Name { get; set; }
    }
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public class Chinese
    {
        public int Age { get; set; }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public class Teacher
    {
        public int Age { get; set; }
        public int Id { get; set; }
    }
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public class English
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public class Ukrainian
    {
        public string FirstName { get; set; }
        public int Age { get; set; }
    }
    class Program
    {
        unsafe static void Main(string[] args)
        {
            Console.WriteLine($"Student   (2 int 1 string) byte: {System.Runtime.InteropServices.Marshal.SizeOf(typeof(Student))}");
            Console.WriteLine($"Teacher   (2 int) byte: {System.Runtime.InteropServices.Marshal.SizeOf(typeof(Teacher))}");
            Console.WriteLine($"Chinese   (1 int) byte: {System.Runtime.InteropServices.Marshal.SizeOf(typeof(Chinese))}");
            Console.WriteLine($"Person    (1 string) byte: {System.Runtime.InteropServices.Marshal.SizeOf(typeof(Person))}");
            Console.WriteLine($"English   (2 string) byte: {System.Runtime.InteropServices.Marshal.SizeOf(typeof(English))}");
            Console.WriteLine($"Ukrainian (1 int 1 string) byte: {System.Runtime.InteropServices.Marshal.SizeOf(typeof(Ukrainian))}");
            Console.WriteLine($"int32 byte: {sizeof(int)}");
        }
    }
    /*
     Student   (2 int 1 string) byte: 24
Teacher   (2 int) byte: 8
Chinese   (1 int) byte: 4
Person    (1 string) byte: 8
English   (2 string) byte: 16
Ukrainian (1 int 1 string) byte: 16
int32 byte: 4
     */

}