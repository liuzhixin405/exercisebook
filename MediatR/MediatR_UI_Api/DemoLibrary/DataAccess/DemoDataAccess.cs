using DemoLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoLibrary.DataAccess
{
    public class DemoDataAccess : IDataAccess
    {
        private List<PersonModel> people = new();
        public DemoDataAccess()
        {
            people.Add(new PersonModel() { Id = 1, FirstName = "Tim", LastName = "Corey" });
            people.Add(new PersonModel() { Id = 2, FirstName = "Sue", LastName = "Storm" });
        }
        public List<PersonModel> GetPeople()
        {
            return people;
        }

        public Task<PersonModel> InsertPeople(String firstName, string lastName)
        {
            PersonModel personModel = new PersonModel() { FirstName = firstName, LastName = lastName };
            personModel.Id = people.Max(x => x.Id) + 1;
            people.Add(personModel);
            return Task.FromResult(personModel);
        }

        public Task<PersonModel> GetPersonById(int id)
        {
            return Task.FromResult(people.FirstOrDefault(x => x.Id == id));
        }
    }
}
