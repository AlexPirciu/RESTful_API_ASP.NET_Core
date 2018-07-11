using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RESTful_API_ASP.NET_Core.Services;
using System;
using System.Collections.Generic;

namespace RESTful_API_ASP.NET_Core.Controllers
{
    [Route("api/authors/{authorId}/books")]

    public class BooksController : Controller
    {
        private ILibraryRepository libraryRepository;

        public BooksController(ILibraryRepository libraryRepository)
        {
            this.libraryRepository = libraryRepository;
        }

        [HttpGet()]
        public IActionResult GetBookForAuthor(Guid authorId)
        {
            if (!libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var booksForAuthorFromRepo = libraryRepository.GetBooksForAuthor(authorId);

            var booksForAuthor = Mapper.Map<IEnumerable<Models.Book>>(booksForAuthorFromRepo);
            return Ok(booksForAuthor);
        }

        [HttpGet("{bookId}", Name = "GetBookForAuthor")]
        public IActionResult GetBookForAuthor(Guid authorId, Guid bookId)
        {
            if (!libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookForAuthorFromRepo = libraryRepository.GetBookForAuthor(authorId, bookId);

            if (bookForAuthorFromRepo == null)
            {
                return NotFound();
            }

            var bookForAuthor = Mapper.Map<Models.Book>(bookForAuthorFromRepo);
            return Ok(bookForAuthor);
        }

        [HttpPost()]
        public IActionResult CreateBookForAuthor(Guid authorId, [FromBody] Models.BookForCreation book)
        {
            if (book == null)
            {
                return BadRequest();
            }

            if (!libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookCreated = Mapper.Map<Entities.Book>(book);

            libraryRepository.AddBookForAuthor(authorId, bookCreated);
            if (!libraryRepository.Save())
            {
                throw new Exception("Creating book failed on save");
            }

            var bookToReturn = Mapper.Map<Models.Book>(bookCreated);
            return CreatedAtRoute("GetBookForAuthor", new { authorId = authorId, bookId = bookToReturn.Id }, bookToReturn);
        }
    }
}