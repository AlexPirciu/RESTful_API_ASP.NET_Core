using Microsoft.AspNetCore.Mvc;

namespace RESTful_API_ASP.NET_Core.Helpers
{
    public class URICreator
    {
        public static object CreateURI(AuthorsResourceParameters authorsResourceParameters, PagedList<Entities.Author> authorsFromRepo, IUrlHelper urlHelper, string route)
        {

            var previousPageLink = authorsFromRepo.HasPrevious ? CreateAutorsResourceUri(authorsResourceParameters, ResourceUriType.PreviousPage, urlHelper, route) : null;
            var nextPageLink = authorsFromRepo.HasNext ? CreateAutorsResourceUri(authorsResourceParameters, ResourceUriType.NextPage, urlHelper, route) : null;
            var paginationMetadata = new
            {
                totalCount = authorsFromRepo.TotalCount,
                pageSize = authorsFromRepo.PageSize,
                currentPage = authorsFromRepo.CurrentPage,
                totalPages = authorsFromRepo.TotalPages,
                previousLink = previousPageLink,
                nextLink = nextPageLink
            };
            return paginationMetadata;
        }

        private static string CreateAutorsResourceUri(AuthorsResourceParameters authorsResourceParameters, ResourceUriType type, IUrlHelper urlHelper, string route)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return urlHelper.Link(route, new
                    {
                        fields = authorsResourceParameters.Fields,
                        orderBy = authorsResourceParameters.OrderBy,
                        searchQuery = authorsResourceParameters.SearchQuery,
                        genre = authorsResourceParameters.Genre,
                        pageNumber = authorsResourceParameters.PageNumber - 1,
                        pageSize = authorsResourceParameters.PageSize
                    });
                case ResourceUriType.NextPage:
                    return urlHelper.Link(route, new
                    {
                        fields = authorsResourceParameters.Fields,
                        orderBy = authorsResourceParameters.OrderBy,
                        searchQuery = authorsResourceParameters.SearchQuery,
                        genre = authorsResourceParameters.Genre,
                        pageNumber = authorsResourceParameters.PageNumber + 1,
                        pageSize = authorsResourceParameters.PageSize
                    });
                default:
                    return urlHelper.Link(route, new
                    {
                        fields = authorsResourceParameters.Fields,
                        orderBy = authorsResourceParameters.OrderBy,
                        searchQuery = authorsResourceParameters.SearchQuery,
                        genre = authorsResourceParameters.Genre,
                        pageNumber = authorsResourceParameters.PageNumber,
                        pageSize = authorsResourceParameters.PageSize
                    });
            }
        }
    }
}
