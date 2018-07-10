using AutoMapper;
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

        [HttpGet("{authorId}")]
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
    }
}
