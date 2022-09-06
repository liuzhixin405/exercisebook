using DemoLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoLibrary.DataAccess
{
    public interface IDataAccess
    {
        List<PersonModel> GetPeople();
        Task<PersonModel> InsertPeople(string firstName, string lastName);
        Task<PersonModel> GetPersonById(int id);
    }
}