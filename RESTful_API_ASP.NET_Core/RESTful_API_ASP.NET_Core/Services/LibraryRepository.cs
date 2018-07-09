using RESTful_API_ASP.NET_Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RESTful_API_ASP.NET_Core.Services
{
    public class LibraryRepository : ILibraryRepository
    {
        private LibraryContext context;

        public LibraryRepository(LibraryContext context)
        {
            this.context = context;
        }

        public bool AuthorExists(Guid authorId)
        {
            return context.Authors.Any(a => a.Id == authorId);
        }

        public IEnumerable<Author> GetAuthors()
        {
            return context.Authors.OrderBy(a => a.FirstName).ThenBy(a => a.LastName);
        }

        public IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorsId)
        {
            return context.Authors.Where(a => authorsId.Contains(a.Id))
                .OrderBy(a => a.FirstName)
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

        //public void UpdateAuthor(Author author)
        //{

        //}

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
