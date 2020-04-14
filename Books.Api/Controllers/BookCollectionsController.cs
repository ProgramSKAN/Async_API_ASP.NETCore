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
    [Route("api/bookcollections")]
    [ApiController]
    public class BookCollectionsController : ControllerBase
    {
        private IBooksRepository _booksRepository;
        private IMapper _mapper;

        public BookCollectionsController(IBooksRepository booksRepository,IMapper mapper)
        {
            _booksRepository = booksRepository ?? throw new ArgumentNullException(nameof(booksRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        

        //Bulk Insert
        [HttpPost]
        [BooksResultFilter]
        public async Task<IActionResult> CreateBookCollection([FromBody] IEnumerable<BookForCreation> bookCollection)
        {
            var bookEntities = _mapper.Map<IEnumerable<Entities.Book>>(bookCollection);
            foreach(var booEntity in bookEntities)
            {
                _booksRepository.AddBook(booEntity);
            }
            await _booksRepository.SaveChangesAsync();
            var booksToReturn = await _booksRepository.GetBooksAsync(bookEntities.Select(b => b.Id).ToList());
            var bookIds = string.Join(",", booksToReturn.Select(a => a.Id));

            return CreatedAtRoute("GetBookCollection", new { bookIds = bookIds }, booksToReturn);
        }

        //supporting location header for bulk insert
        //api/bookcollections/(id1,id2)
        [HttpGet("({bookIds})",Name ="GetBookCollection")]
        [BooksResultFilter]

        //to bind ids from uri to ienumerable of Guids, use modelbinder attribute and pass the type arraymodelbinder(custom binder)
        public async Task<IActionResult> GetBookCollection([ModelBinder(BinderType =typeof(ArrayModelBinder))] IEnumerable<Guid> bookIds)
        {
            var bookEntites = await _booksRepository.GetBooksAsync(bookIds);
            if (bookIds.Count() != bookEntites.Count())
            {
                return NotFound();
            }
            
            return Ok(bookEntites);
        }
    }
}
