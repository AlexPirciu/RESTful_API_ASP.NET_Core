using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RESTful_API_ASP.NET_Core.Helpers;
using RESTful_API_ASP.NET_Core.Services;
using System;
using System.Collections.Generic;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RESTful_API_ASP.NET_Core.Controllers
{
    [Route("api/authors")]

    public class AuthorsController : Controller
    {
        private ILogger<AuthorsController> logger;
        private ILibraryRepository libraryRepository;
        private IUrlHelper urlHelper;
        private IPropertyMappingService propertyMappingService;
        private ITypeHelperServices typeHelperServices;

        public AuthorsController(ILogger<AuthorsController> logger, ILibraryRepository libraryRepository,
            IUrlHelper urlHelper, IPropertyMappingService propertyMappingService, ITypeHelperServices typeHelperServices)
        {
            this.logger = logger;
            this.libraryRepository = libraryRepository;
            this.urlHelper = urlHelper;
            this.propertyMappingService = propertyMappingService;
            this.typeHelperServices = typeHelperServices;
        }

        [HttpGet(Name = "GetAuthors")]
        public IActionResult GetAuthors(Helpers.AuthorsResourceParameters authorsResourceParameters)
        {
            if (!propertyMappingService.ValidMappingExistsFor<Models.Author, Entities.Author>(authorsResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!typeHelperServices.TypeHasProperties<Models.Author>(authorsResourceParameters.Fields))
            {
                return BadRequest();
            }

            var authorsFromRepo = libraryRepository.GetAuthors(authorsResourceParameters);

            var paginationMetadata = Helpers.URICreator.CreateURI(authorsResourceParameters, authorsFromRepo, urlHelper, "GetAuthors");

            Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

            var authors = Mapper.Map<IEnumerable<Models.Author>>(authorsFromRepo);

            return Ok(authors.ShapeData(authorsResourceParameters.Fields));
        }

        [HttpGet("{authorId}", Name = "GetAuthor")]
        public IActionResult GetAuthor(Guid authorId, [FromQuery]string fields)
        {
            if (!typeHelperServices.TypeHasProperties<Models.Author>(fields))
            {
                return BadRequest();
            }
            var authorFromRepo = libraryRepository.GetAuthor(authorId);

            if (authorFromRepo == null)
            {
                return NotFound();
            }

            var author = Mapper.Map<Models.Author>(authorFromRepo);
            return Ok(author.ShapeData(fields));
        }

        [HttpPost()]
        public IActionResult CreateAuthor([FromBody] Models.AuthorForCreation author)
        {
            if (author == null)
            {
                return BadRequest();
            }

            if (author.FirstName == author.LastName)
            {
                ModelState.AddModelError(nameof(Models.AuthorForCreation), "The first name and the last name of the author must be different");
            }

            if (!ModelState.IsValid)
            {
                return new Helpers.UnprocessableEntityObjectResult(ModelState);
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

            logger.LogInformation(100, $"Auhtor with id {authorId} was deleted.");
            return NoContent();
        }

        [HttpPut("{authorId}")]
        public IActionResult UpdateAuthor(Guid authorId, [FromBody] Models.AuthorForUpdate author)
        {
            if (author == null)
            {
                return BadRequest();
            }

            if (author.FirstName == author.LastName)
            {
                ModelState.AddModelError(nameof(Models.AuthorForCreation), "The first name and the last name of the author must be different");
            }

            if (!ModelState.IsValid)
            {
                return new Helpers.UnprocessableEntityObjectResult(ModelState);
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
                patchDoc.ApplyTo(author, ModelState);

                if (author.FirstName == author.LastName)
                {
                    ModelState.AddModelError(nameof(Models.AuthorForUpdate), "The first name and the last name of the author must be different");
                }

                TryValidateModel(author);

                if (!ModelState.IsValid)
                {
                    return new Helpers.UnprocessableEntityObjectResult(ModelState);
                }

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

            var authorToPatch = Mapper.Map<Models.AuthorForUpdate>(authorFromRepo);
            patchDoc.ApplyTo(authorToPatch, ModelState);

            if (authorToPatch.FirstName == authorToPatch.LastName)
            {
                ModelState.AddModelError(nameof(Models.AuthorForUpdate), "The first name and the last name of the author must be different");
            }

            TryValidateModel(authorToPatch);

            if (!ModelState.IsValid)
            {
                return new Helpers.UnprocessableEntityObjectResult(ModelState);
            }

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
