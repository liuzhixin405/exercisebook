using MongoDB.BooksApi.Extensions;
using MongoDB.BooksApi.Models;
using MongoDB.BooksApi.Repository;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MongoDB.BooksApi.Services
{

    public class BookServiceN
    {
        private readonly BookRepository<Book> _bookRepository;

        public BookServiceN(BookRepository<Book> BookRepository)
        {
            this._bookRepository = BookRepository;
        }

        public async Task<Book> CreateBook(Book book, IClientSessionHandle session = null)
        {
            if (book == null) throw new ArgumentNullException("book is null");
            return await _bookRepository.InsertOneAsync(book, session);
        }
        public async Task<bool> UpdateBook(Book book)
        {
            if (book == null) throw new ArgumentNullException("book is null");
            return await _bookRepository.UpdateOneAsync(book);
        }

        public async Task<bool> DeleteBookById(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException("book is null");
            return await _bookRepository.RemoveOneAsync(id);
        }
        public async Task<Tuple<long,List<Book>>> GetBooks(BookSearchBox box)
        {
            Expression<Func<Book, bool>> lambda =i=> true;
            if (!string.IsNullOrWhiteSpace(box.Name))
            {
                lambda = lambda.AndOne(x => x.BookName.Contains(box.Name));
            }
            if (!string.IsNullOrWhiteSpace(box.Category))
            {
                lambda = lambda.AndOne(x => x.Category.Contains(box.Category));
            }
            if (!string.IsNullOrWhiteSpace(box.Author))
            {
                lambda = lambda.AndOne(x => x.Author.Contains(box.Author));
            }

            return await _bookRepository.GetListAsync(lambda,box.PageIndex,box.PageSize);
        }
    }
    public class BookService
    {
        private readonly IMongoCollection<Book> _books;
        public BookService(IBookstoreDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);   //数据库链接 
            var databse = client.GetDatabase(settings.DatabaseName);    //表名
            _books = databse.GetCollection<Book>(settings.BooksCollectionName);   //数据库名
        }

        public List<Book> Get() => _books.Find(book => true).ToList();
        public Book Get(string id) => _books.Find<Book>(book => book._id== id).FirstOrDefault();

        public Book Create(Book book)
        {
            _books.InsertOne(book);
            return book; 
        }
        public void Update(string id, Book book) => _books.ReplaceOne(book => book._id == id, book);

        public void Remove(Book bookDel) => _books.DeleteOne(book => book._id == bookDel._id);

        public void Remove(string id) => _books.DeleteOne(book => book._id == id);
    }
}
