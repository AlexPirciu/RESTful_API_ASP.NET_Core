using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RESTful_API_ASP.NET_Core.Services;
using System;
using System.Collections.Generic;

namespace RESTful_API_ASP.NET_Core.Controllers
{
    [Route("api/authors/{authorId}/books")]

    public class BooksController : Controller
    {
        private ILogger<BooksController> logger;
        private ILibraryRepository libraryRepository;

        public BooksController(ILibraryRepository libraryRepository, ILogger<BooksController> logger)
        {
            this.logger = logger;
            this.libraryRepository = libraryRepository;
        }

        [HttpGet()]
        public IActionResult GetBooksForAuthor(Guid authorId)
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

            if (book.Title == book.Description)
            {
                ModelState.AddModelError(nameof(Models.BookForCreation), "The title and the description must be different");
            }

            if (!ModelState.IsValid)
            {
                return new Helpers.UnprocessableEntityObjectResult(ModelState);
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

        [HttpDelete("{bookId}")]
        public IActionResult DeleteBookForAuthor(Guid authorId, Guid bookId)
        {
            if (!libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookFromRepo = libraryRepository.GetBookForAuthor(authorId, bookId);
            if (bookFromRepo == null)
            {
                return NotFound();
            }

            libraryRepository.DeleteBookForAuthor(bookFromRepo);
            if (!libraryRepository.Save())
            {
                throw new Exception($"Deleting book {bookId} for the author {authorId} failed.");
            }

            logger.LogInformation(100, $"Book {bookId} for author {authorId} was deleted.");

            return NoContent();
        }

        //persistance?
        //mock persistance?
        //de ce trebuie apelata functia pt update?
        [HttpPut("{bookId}")]
        public IActionResult UpdateBookForAuthor(Guid authorId, Guid bookId, [FromBody] Models.BookForUpdate book)
        {
            if (book == null)
            {
                return BadRequest();
            }

            if (book.Title == book.Description)
            {
                ModelState.AddModelError(nameof(Models.BookForUpdate), "The title and the description must be different");
            }

            if (!ModelState.IsValid)
            {
                return new Helpers.UnprocessableEntityObjectResult(ModelState);
            }

            if (!libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookFromRepo = libraryRepository.GetBookForAuthor(authorId, bookId);
            if (bookFromRepo == null)
            {
                var bookToAdd = Mapper.Map<Entities.Book>(book);
                bookToAdd.Id = bookId;
                libraryRepository.AddBookForAuthor(authorId, bookToAdd);

                if (!libraryRepository.Save())
                {
                    throw new Exception($"Upserting book {bookId} for the author {authorId} failed on save.");
                }

                var bookToReturn = Mapper.Map<Models.Book>(bookToAdd);
                return CreatedAtRoute("GetBookForAuthor", new { authorId = authorId, bookId = bookId }, bookToReturn);
            }

            Mapper.Map(book, bookFromRepo);

            libraryRepository.UpdateBookForAuthor(bookFromRepo);
            if (!libraryRepository.Save())
            {
                throw new Exception($"Updating book {bookId} for the author {authorId} failed on save.");
            }

            return NoContent();
        }

        [HttpPatch("{bookId}")]
        public IActionResult PartiallyUpdateBookForAuthor(Guid authorId, Guid bookId, [FromBody] JsonPatchDocument<Models.BookForUpdate> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            if (!libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookFromRepo = libraryRepository.GetBookForAuthor(authorId, bookId);
            if (bookFromRepo == null)
            {
                var book = new Models.BookForUpdate();
                patchDoc.ApplyTo(book, ModelState);

                if (book.Title == book.Description)
                {
                    ModelState.AddModelError(nameof(Models.BookForUpdate), "The title and the description must be different");
                }

                TryValidateModel(book);

                if (!ModelState.IsValid)
                {
                    return new Helpers.UnprocessableEntityObjectResult(ModelState);
                }

                var bookToAdd = Mapper.Map<Entities.Book>(book);
                bookToAdd.Id = bookId;

                libraryRepository.AddBookForAuthor(authorId, bookToAdd);
                if (!libraryRepository.Save())
                {
                    throw new Exception($"Upsert book {bookId} for the author {authorId} failed on save.");
                }

                var bookToReturn = Mapper.Map<Models.Book>(bookToAdd);
                return CreatedAtRoute("GetBookForAuthor", new { bookId = bookId, authorId = authorId }, bookToReturn);
            }

            var bookToPatch = Mapper.Map<Models.BookForUpdate>(bookFromRepo);

            //patchDoc.ApplyTo(bookToPatch, ModelState);

            patchDoc.ApplyTo(bookToPatch);

            if (bookToPatch.Title == bookToPatch.Description)
            {
                ModelState.AddModelError(nameof(Models.BookForUpdate), "The title and the description must be different");
            }

            TryValidateModel(bookToPatch);

            if (!ModelState.IsValid)
            {
                return new Helpers.UnprocessableEntityObjectResult(ModelState);
            }

            Mapper.Map(bookToPatch, bookFromRepo);
            libraryRepository.UpdateBookForAuthor(bookFromRepo);
            if (!libraryRepository.Save())
            {
                throw new Exception($"Patching book {bookId} for the author {authorId} failed on save.");
            }

            return NoContent();
        }
    }
}