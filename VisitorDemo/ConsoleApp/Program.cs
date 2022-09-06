using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AppDomain appDomain= AppDomain.CreateDomain("New Domain");
            appDomain.SetData("Message", "guess what ...");
            AppDomain.Unload(appDomain);

            var cust = new Customer
            {
                FirstName = "Joe",
                LastName = "Bloggs",
                CreditLimit = 123
            };
            cust.Fnends.Add(new Employee { FirstName = "Sue", LastName = "Brown", Salary = 50000 });
            Console.WriteLine(new ToXElementPersonVisitor().DynamicVisit(cust));
            Console.Read();
        }
        static void SayMessage()
        {
            Console.WriteLine(AppDomain.CurrentDomain.GetData("Message"));
        }
    }

    class ToXElementPersonVisitor
    {
        public XElement DynamicVisit(Person p) => Visit((dynamic)p);

        private XElement Visit(Person p)
        {
            return new XElement("Person", new XAttribute("Type", p.GetType().Name),
                new XElement("FirstName",p.FirstName),
                 new XElement("FirstName", p.LastName),
                 p.Fnends.Select(f=> DynamicVisit(f))
                );
        }
        XElement Visit(Customer c)
        {
            XElement xe = Visit((Person)c);
            xe.Add(new XElement("CreditLimit", c.CreditLimit));
            return xe;
        }
        XElement Visit(Employee e)
        {
            XElement xe = Visit((Person)e);
            xe.Add(new XElement("Salary", e.Salary));
            return xe;
        }
    }
    internal class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public readonly IList<Person> Fnends = new List<Person>();

    }

    class Customer : Person
    {
        public decimal CreditLimit { get; set; }

             
    }
    class Employee : Person
    {
        public decimal Salary { get; set; }
    }


}
