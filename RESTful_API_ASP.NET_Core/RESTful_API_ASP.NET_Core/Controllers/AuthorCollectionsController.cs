using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RESTful_API_ASP.NET_Core.Services;
using System;
using System.Collections.Generic;

namespace RESTful_API_ASP.NET_Core.Controllers
{
    [Route("api/authorcollection")]
    public class AuthorCollectionsController : Controller
    {
        private ILibraryRepository libraryRepository;

        public AuthorCollectionsController(ILibraryRepository libraryRepository)
        {
            this.libraryRepository = libraryRepository;
        }

        [HttpPost()]
        public IActionResult CreateAuthorCollection([FromBody] IEnumerable<Models.AuthorForCreation> authorCollection)
        {
            if (authorCollection == null)
            {
                return BadRequest();
            }

            var authorsCreated = Mapper.Map<IEnumerable<Entities.Author>>(authorCollection);

            foreach (var author in authorsCreated)
            {
                libraryRepository.AddAuthor(author);
            }

            if (!libraryRepository.Save())
            {
                throw new Exception("Creating an author collection failed on save.");
            }

            return Ok();
        }
    }
}