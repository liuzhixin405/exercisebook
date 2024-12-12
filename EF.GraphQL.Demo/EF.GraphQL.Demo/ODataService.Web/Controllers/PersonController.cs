using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using ODataService.Web.Models;

namespace ODataService.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : ODataController
    {
        private static readonly IList<Person> Persons = new
        List<Person>
        {
            new Person { Id = 1, Name = "John", Age = 30 },
            new Person { Id = 2, Name = "Mary", Age = 25 },
            new Person { Id = 3, Name = "Tom", Age = 40 },
            new Person { Id = 4, Name = "Jane", Age = 35 },
        };

        private readonly ILogger<PersonController> _logger;

        public PersonController(ILogger<PersonController> logger)
        {
            _logger = logger;
        }

        [HttpGet,EnableQuery]
        public ActionResult Get()
        {
           return Ok(Persons);
        }
        /// <summary>
        /// https://localhost:7139/odata/person?$select=name,age&orderby=age desc&top=1&skip=2
        /// https://localhost:7139/odata/person?$filter=name eq 'John'&select=name,age&orderby=age desc
        /// age add 1 eq 3
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get([FromODataUri] int key)
        {
            return Ok(Persons.FirstOrDefault(p => p.Id == key));
        }

        [HttpPost]
        public IActionResult Post(Person person)
        {
            Persons.Add(person);
            return Created(person);
        }
        public IActionResult Patch([FromODataUri] int key, Delta<Person> delta)
        {
            var updatePerson = Persons.FirstOrDefault(p => p.Id == key);
            if (updatePerson == null)
            {
                return NotFound();
            }

            delta.Patch(updatePerson);
            return Updated(updatePerson);
        }

        public IActionResult Delete([FromODataUri] int key)
        {
            var deletePerson = Persons.FirstOrDefault(p => p.Id == key);
            if (deletePerson == null)
            {
                return NotFound();
            }

            Persons.Remove(deletePerson);
            return NoContent(); 
        }
    }
}
