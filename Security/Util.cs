using IdentityJwt.UseCases.AccessManagement;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Text.Json;

namespace IdentityJwt.Security
{
    public static class Util
    {
        public static T JsonParse<T>(string text) where T : class, new()
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            return JsonSerializer.Deserialize<T>(text);
        }
    }
}