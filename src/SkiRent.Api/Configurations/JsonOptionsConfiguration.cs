using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Mvc;

namespace SkiRent.Api.Configurations;

public static class JsonOptionsConfiguration
{
    public static void Configure(JsonOptions options)
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.AllowInputFormatterExceptionMessages = false;
        options.JsonSerializerOptions.AllowTrailingCommas = true;
    }
}
