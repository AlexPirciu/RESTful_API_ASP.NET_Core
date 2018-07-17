using RESTful_API_ASP.NET_Core.Entities;
using System;
using System.Collections.Generic;

namespace RESTful_API_ASP.NET_Core.Services
{
    public interface ILibraryRepository
    {
        bool AuthorExists(Guid authorId);

        Helpers.PagedList<Author> GetAuthors(Helpers.AuthorsResourceParameters authorsResourceParameters);

        Author GetAuthor(Guid authorId);

        IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorsId);

        void AddAuthor(Author author);

        void UpdateAuthor(Author author);

        void DeleteAuthor(Author author);

        IEnumerable<Book> GetBooksForAuthor(Guid authorId);

        Book GetBookForAuthor(Guid authorId, Guid bookId);

        void AddBookForAuthor(Guid author, Book book);

        void UpdateBookForAuthor(Book book);

        void DeleteBookForAuthor(Book book);

        bool Save();
    }
}
