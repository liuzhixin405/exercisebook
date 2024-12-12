using EF.GraphQL.Two.Interfaces;
using EF.GraphQL.Two.Models;
using EF.GraphQL.Two.Models.Context;

namespace EF.GraphQL.Two.Repository
{
    public class PersonRepository : IPersonRepository
    {
        private readonly SampleContext _context;
        public PersonRepository(SampleContext context)
        {
            _context = context;
        }

        public Task<Person> CreatePerson(Person person)
        {
            person.Id = Guid.NewGuid();
            _context.Persons.Add(person);
            return Task.FromResult(person);
        }

        public async Task DeletePerson(Person person)
        {
             _context.Persons.Remove(person);
        }

        public async Task<Person> GetById(Guid id)
        {
            await Task.CompletedTask;
           var result =  _context.Persons.FirstOrDefault(x=>x.Id == id);
            if (result == null)
            {
                throw new ArgumentException("Person not found");
            }
            result.Accounts = _context.Accounts.Where(x => x.PersonId == id).ToList();
            return result;
        }

        public async Task<List<Person>> GetPersons()
        {
            return  _context.Persons.ToList();
        }

        public Task<Person> UpdatePerson(Person person, Person updatedPerson)
        {
            _context.Persons.Remove(person);
            _context.Persons.Add(updatedPerson);
            return Task.FromResult(updatedPerson);  
        }
    }
}
