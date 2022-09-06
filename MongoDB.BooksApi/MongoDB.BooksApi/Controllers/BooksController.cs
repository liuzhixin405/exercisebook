using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.BooksApi.Models;
using MongoDB.BooksApi.Services;
using MongoDB.Bson;

namespace MongoDB.BooksApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookService _bookService;
        private readonly BookServiceN _bookServiceN;
        public BooksController(BookService bookService, BookServiceN bookServiceN)
        {
            _bookService = bookService;
            _bookServiceN = bookServiceN;
        }
        [HttpGet]
        public ActionResult<List<Book>> Get() => _bookService.Get();

        [HttpGet("{id:length(24)}", Name = "GetBook")]
        public ActionResult<Book> Get(string id) 
        {

           var book = _bookService.Get(id);
            if(book == null)
            {
                return NotFound();
            }
            return book;
        }

        [HttpPost]
        public async Task<ActionResult<Book>> Create(Book book)
        {
            book._id = ObjectId.GenerateNewId().ToString();
            //_bookService.Create(book);
            await _bookServiceN.CreateBook(book); 
            return CreatedAtRoute("GetBook", new { id = book._id.ToString() }, book);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id,Book bookIn)
        {
            var book = _bookService.Get(id);
            if (book == null) {

                return NotFound();
            }
            _bookService.Update(id, bookIn);
            return NoContent();
            
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var book = _bookService.Get(id);
            if(book == null)
            {
                return NotFound();
            }
            _bookService.Remove(book._id);

            return NoContent();
        }


    }
}
