using WebApi.Models;

namespace WebApi.Extensions
{
    public static class DictionaryExtensions
    {
        public static T ToObject<T>(this IHeaderDictionary headers) where T : ApiRequestHeaders, new()
        {
            var result = new T();
            foreach (var header in headers)
            {
                switch (header.Key)
                {
                    case "x-webapi-key":
                        result.ApiKey = header.Value;
                        break;

                    case "x-github-token":
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        (result as GitHubApiRequestHeaders).GitHubToken = header.Value;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                        break;
                }
            }

            return result;
        }
    }
}