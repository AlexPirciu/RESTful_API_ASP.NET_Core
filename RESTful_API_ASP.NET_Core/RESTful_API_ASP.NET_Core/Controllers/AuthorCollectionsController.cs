using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RESTful_API_ASP.NET_Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

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

            var authorCollectionToReturn = Mapper.Map<IEnumerable<Models.Author>>(authorsCreated);
            var ids = string.Join(",", authorCollectionToReturn.Select(a => a.Id));

            return CreatedAtRoute("GetAuthorCollection", new { ids = ids }, authorCollectionToReturn);
        }

        [HttpGet("({ids})", Name = "GetAuthorCollection")]
        public IActionResult GetAuthorsCollection([ModelBinder(BinderType = typeof(Helpers.ArrayModelBinder))]IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                return BadRequest();
            }

            //intreaba-l pe Alin!!!!
            var authorsFromRepo = libraryRepository.GetAuthors(ids);

            if (ids.Count() != authorsFromRepo.Count())
            {
                return NotFound();
            }

            var authorsToReturn = Mapper.Map<IEnumerable<Models.Author>>(authorsFromRepo);
            return Ok(authorsToReturn);
        }
    }
}