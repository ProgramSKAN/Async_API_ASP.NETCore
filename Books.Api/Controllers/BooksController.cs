//using Microsoft.AspNetCore.Mvc;// :Controller>> used for MVC apps not APIs
//after asp.net core 2.1 we use :ControllerBase with [ApiController] which enforces attribute routing >for APIS
using AutoMapper;
using Books.Api.Filters;
using Books.Api.Models;
using Books.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Books.Api.Controllers
{
    [Route("api/books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private IBooksRepository _booksRepository;
        private readonly IMapper _mapper;

        public BooksController(IBooksRepository booksRepository, IMapper mapper)
        {
            _booksRepository = booksRepository ?? throw new ArgumentNullException(nameof(booksRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        [HttpGet]
        [BooksResultFilter]
        public async Task<IActionResult> GetBooks()
        {
            var bookEntities = await _booksRepository.GetBooksAsync();
            return Ok(bookEntities);
        }

        [HttpGet]
        [BookResultFilter]
        [Route("{id}",Name ="GetBook")]
        public async Task<IActionResult> GetBook(Guid id)
        {
            var bookEntity = await _booksRepository.GetBookAsync(id);
            if (bookEntity == null)
            {
                return NotFound();
            }
            var bookCovers = await _booksRepository.GetBookCoverAsync(id);
            //var propertyBag = new Tuple<Entities.Book, IEnumerable<ExternalModels.BookCover>>(
            //    bookEntity, bookCovers);

            //(Entities.Book book, IEnumerable<ExternalModels.BookCover> bookCovers) propertyBag = 
            //    (bookEntity, bookCovers);

            return Ok((bookEntity, bookCovers));
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] BookForCreation book)//deserialize the request body to BookForCreation instance
        {//since the book to create is not the Models.Books nor Entities.Book. so create a class "BookForCreation"

            var bookEntity = _mapper.Map<Entities.Book>(book);
            _booksRepository.AddBook(bookEntity);
            await _booksRepository.SaveChangesAsync();

            //fetch (refetch) the book from data store, including the author
            await _booksRepository.GetBookAsync(bookEntity.Id);

            return CreatedAtRoute("GetBook", new { id = bookEntity.Id },bookEntity);
        }
    }
}
