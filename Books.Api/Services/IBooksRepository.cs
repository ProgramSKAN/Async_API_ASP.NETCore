using Books.Api.Entities;
using Books.Api.ExternalModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Books.Api.Services
{
    public interface IBooksRepository
    {
        //Asynchronous
        Task<IEnumerable<Book>> GetBooksAsync();
        Task<IEnumerable<Entities.Book>> GetBooksAsync(IEnumerable<Guid> bookIds);
        Task<BookCover> GetBookCoverAsync(Guid coverId);
        Task<Book> GetBookAsync(Guid id);
        void AddBook(Entities.Book bookToAdd);//not async task since we r not using DB.
        Task<bool> SaveChangesAsync();

        //Sunchronous
        IEnumerable<Book> GetBooks();
        Book GetBook(Guid id);
    }
}
