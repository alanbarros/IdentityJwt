using System.Text.Json;

namespace IdentityJwt.Infra.Security
{
    public static class Util
    {
        public static T JsonParse<T>(string text) where T : class, new()
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            return JsonSerializer.Deserialize<T>(text);
        }

        public static T ConvertTo<T>(this string text) where T : class, new()
            => JsonParse<T>(text);
    }
}