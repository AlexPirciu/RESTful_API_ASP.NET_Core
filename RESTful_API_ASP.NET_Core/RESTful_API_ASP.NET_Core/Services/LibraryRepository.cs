using RESTful_API_ASP.NET_Core.Entities;
using RESTful_API_ASP.NET_Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RESTful_API_ASP.NET_Core.Services
{
    public class LibraryRepository : ILibraryRepository
    {
        private LibraryContext context;
        private IPropertyMappingService propertyMappingService;

        public LibraryRepository(LibraryContext context, IPropertyMappingService propertyMappingService)
        {
            this.context = context;
            this.propertyMappingService = propertyMappingService;
        }

        public bool AuthorExists(Guid authorId)
        {
            return context.Authors.Any(a => a.Id == authorId);
        }

        public Helpers.PagedList<Author> GetAuthors(Helpers.AuthorsResourceParameters authorsResourceParameters)
        {
            //var collectionBeforePaging = context.Authors
            //    .OrderBy(a => a.FirstName)
            //    .ThenBy(a => a.LastName).AsQueryable();

            var collectionBeforePaging =
                context.Authors.ApplySort(authorsResourceParameters.OrderBy,
                propertyMappingService.GetPropertyMapping<Models.Author,Entities.Author>());

            if (!string.IsNullOrEmpty(authorsResourceParameters.Genre))
            {
                var genreForWhereClause = authorsResourceParameters.Genre.Trim().ToLowerInvariant();
                collectionBeforePaging = collectionBeforePaging.Where(a => a.Genre.ToLowerInvariant() == genreForWhereClause);
            }

            if (!string.IsNullOrEmpty(authorsResourceParameters.SearchQuery))
            {
                var searchQueryForWhereClause = authorsResourceParameters.SearchQuery.Trim().ToLowerInvariant();
                collectionBeforePaging = collectionBeforePaging.Where(a => a.Genre.ToLowerInvariant().Contains(searchQueryForWhereClause)
                || a.FirstName.ToLowerInvariant().Contains(searchQueryForWhereClause)
                || a.LastName.ToLowerInvariant().Contains(searchQueryForWhereClause));
            }

            return Helpers.PagedList<Author>.Create(collectionBeforePaging, authorsResourceParameters.PageNumber, authorsResourceParameters.PageSize);
        }

        public IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorsId)
        {
            return context.Authors.Where(a => authorsId.Contains(a.Id))
                //.OrderBy(a => a.FirstName)
                .OrderBy(a => a.LastName)
                .ToList();
        }

        public Author GetAuthor(Guid authorId)
        {
            return context.Authors.FirstOrDefault(a => a.Id == authorId);
        }

        public void AddAuthor(Author author)
        {
            author.Id = Guid.NewGuid();
            context.Authors.Add(author);

            if (author.Books.Any())
            {
                foreach (var book in author.Books)
                {
                    book.Id = Guid.NewGuid();
                }
            }
        }

        public void UpdateAuthor(Author author)
        {

        }

        public void DeleteAuthor(Author author)
        {
            context.Authors.Remove(author);
        }

        public IEnumerable<Book> GetBooksForAuthor(Guid authorId)
        {
            return context.Books.Where(a => a.AuthorId == authorId)
                .OrderBy(a => a.Title)
                .ToList();
        }

        public Book GetBookForAuthor(Guid authorId, Guid bookId)
        {
            return context.Books.FirstOrDefault(b => b.AuthorId == authorId && b.Id == bookId);
        }

        public void AddBookForAuthor(Guid authorId, Book book)
        {
            var author = GetAuthor(authorId);

            if (author != null)
            {
                if (book.Id == Guid.Empty)
                {
                    book.Id = Guid.NewGuid();
                }
                author.Books.Add(book);
            }
        }

        public void UpdateBookForAuthor(Book book)
        {

        }

        public void DeleteBookForAuthor(Book book)
        {
            context.Books.Remove(book);
        }

        public bool Save()
        {
            return (context.SaveChanges() >= 0);
        }
    }
}
