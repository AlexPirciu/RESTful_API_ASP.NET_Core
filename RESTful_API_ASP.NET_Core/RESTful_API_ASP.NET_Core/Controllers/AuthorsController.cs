using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using RESTful_API_ASP.NET_Core.Services;
using System;
using System.Collections.Generic;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RESTful_API_ASP.NET_Core.Controllers
{
    [Route("api/authors")]

    public class AuthorsController : Controller
    {
        private ILibraryRepository libraryRepository;

        public AuthorsController(ILibraryRepository libraryRepository)
        {
            this.libraryRepository = libraryRepository;
        }

        [HttpGet()]
        public IActionResult GetAuthors()
        {
            var authorsFromRepo = libraryRepository.GetAuthors();
            var authors = Mapper.Map<IEnumerable<Models.Author>>(authorsFromRepo);

            return Ok(authors);
        }

        [HttpGet("{authorId}", Name = "GetAuthor")]
        public IActionResult GetAuthor(Guid authorId)
        {
            var authorFromRepo = libraryRepository.GetAuthor(authorId);

            if (authorFromRepo == null)
            {
                return NotFound();
            }

            var author = Mapper.Map<Models.Author>(authorFromRepo);
            return Ok(author);
        }

        [HttpPost()]
        public IActionResult CreateAuthor([FromBody] Models.AuthorForCreation author)
        {
            if (author == null)
            {
                return BadRequest();
            }

            var authorCreated = Mapper.Map<Entities.Author>(author);

            libraryRepository.AddAuthor(authorCreated);
            if (!libraryRepository.Save())
            {
                throw new Exception("Creating an author failed on save.");
                //return StatusCode(500, "A problem ocurred when handling the request.");
            }

            var authorToReturn = Mapper.Map<Models.Author>(authorCreated);
            return CreatedAtRoute("GetAuthor", new { authorId = authorToReturn.Id }, authorToReturn);
        }

        [HttpPost("{authorId}")]
        public IActionResult BlockingAuthorCreation(Guid authorId)
        {
            if (libraryRepository.AuthorExists(authorId))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }
            return NotFound();
        }

        [HttpDelete("{authorId}")]
        public IActionResult DeleteAuthor(Guid authorId)
        {
            var authorFromRepo = libraryRepository.GetAuthor(authorId);

            if (authorFromRepo == null)
            {
                return NotFound();
            }

            libraryRepository.DeleteAuthor(authorFromRepo);
            if (!libraryRepository.Save())
            {
                throw new Exception($"Deleting author {authorId} failed.");
            }

            return NoContent();
        }

        [HttpPut("{authorId}")]
        public IActionResult UpdateAuthor(Guid authorId, [FromBody] Models.AuthorForUpdate author)
        {
            if (author == null)
            {
                return BadRequest();
            }

            var authorFromRepo = libraryRepository.GetAuthor(authorId);
            if (authorFromRepo == null)
            {
                var authorToAdd = Mapper.Map<Entities.Author>(author);
                libraryRepository.AddAuthor(authorToAdd);
                authorToAdd.Id = authorId;
                if (!libraryRepository.Save())
                {
                    throw new Exception($"Creating author {authorId} failed on save.");
                }

                var authorToReturn = Mapper.Map<Models.Author>(authorToAdd);
                return CreatedAtRoute("GetAuthor", new { authorId = authorId }, authorToReturn);
            }

            Mapper.Map(author, authorFromRepo);
            libraryRepository.UpdateAuthor(authorFromRepo);
            if (!libraryRepository.Save())
            {
                throw new Exception($"Updating author {authorId} failed on save.");
            }

            return NoContent();
        }

        [HttpPatch("{authorId}")]
        public IActionResult PartialUpdateAuthor(Guid authorId, [FromBody] JsonPatchDocument<Models.AuthorForUpdate> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var authorFromRepo = libraryRepository.GetAuthor(authorId);
            if (authorFromRepo == null)
            {
                var author = new Models.AuthorForUpdate();
                patchDoc.ApplyTo(author);

                var authorToAdd = Mapper.Map<Entities.Author>(author);
                libraryRepository.AddAuthor(authorToAdd);
                authorToAdd.Id = authorId;
                if (!libraryRepository.Save())
                {
                    throw new Exception($"Creatine author {authorId} failed on save.");
                }

                var authorToReturn = Mapper.Map<Models.Author>(authorToAdd);
                return CreatedAtRoute("GetAuthor", new { authorId = authorId }, authorToReturn);
            }

            var authorToPatch = Mapper.Map<Models.AuthorForUpdate>(authorFromRepo);
            patchDoc.ApplyTo(authorToPatch);

            Mapper.Map(authorToPatch, authorFromRepo);
            libraryRepository.UpdateAuthor(authorFromRepo);
            authorFromRepo.Id = authorId;
            if (!libraryRepository.Save())
            {
                throw new Exception($"Updating author {authorId} failed on save.");
            }

            return NoContent();
        }
    }
}
