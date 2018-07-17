namespace RESTful_API_ASP.NET_Core.Services
{
    public interface ITypeHelperServices
    {
        bool TypeHasProperties<T>(string fields);
    }
}
