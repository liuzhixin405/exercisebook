using EF.GraphQL.Two.Models;

namespace EF.GraphQL.Two.Interfaces
{
    public interface IPersonRepository
    {
        Task<List<Person>> GetPersons();
        Task<Person> GetById(Guid id);
        Task<Person> CreatePerson(Person person);
        Task<Person> UpdatePerson(Person person,Person updatedPerson);
        Task DeletePerson(Person person);
    }
}
