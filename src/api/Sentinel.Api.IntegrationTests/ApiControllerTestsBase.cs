using Microsoft.AspNetCore.Mvc;
using Sentinel.Common;
using System.Net.Http.Json;
using System.Text.Json;

namespace Sentinel.Api.IntegrationTests;

public abstract class ApiControllerTestsBase(ApiWebApplicationFactory factory)
    : IClassFixture<ApiWebApplicationFactory>, IDisposable
{
    protected HttpClient Client { get; } = factory.CreateClient();

    protected ApiWebApplicationFactory Factory { get; } = factory;

    protected static CancellationToken CancellationToken => TestContext.Current.CancellationToken;

    protected static HttpRequestMessage CreateRequest(
        HttpMethod method,
        string requestUri,
        string? scopes = null,
        string? apiVersion = null,
        HttpContent? content = null)
    {
        var request = new HttpRequestMessage(method, requestUri) { Content = content };

        if (scopes is not null)
            request.Headers.Add(Constants.ScopeHeaderKey, scopes);

        if (apiVersion is not null)
            request.Headers.Add(Constants.ApiVersionHeaderKey, apiVersion);

        return request;
    }

    protected static HttpRequestMessage CreateJsonRequest<T>(
        HttpMethod method,
        string requestUri,
        T body,
        string? scopes = null,
        string? apiVersion = null) =>
        CreateRequest(method, requestUri, scopes, apiVersion, JsonContent.Create(body));

    protected static HttpRequestMessage CreateRawJsonRequest(HttpMethod method, string requestUri, string json) =>
        CreateRequest(method, requestUri, content: new StringContent(json, System.Text.Encoding.UTF8, "application/json"));

    protected static Task<T?> ReadContentAsync<T>(HttpResponseMessage response) =>
        response.Content.ReadFromJsonAsync<T>(CancellationToken);

    protected static Task<ProblemDetails?> ReadProblemDetailsAsync(HttpResponseMessage response) =>
        ReadContentAsync<ProblemDetails>(response);

    /// <summary>Extracts the validation error messages for a given field from the problem details "errors" extension.</summary>
    protected static IReadOnlyList<string> GetValidationErrors(ProblemDetails problemDetails, string propertyName) =>
        ((JsonElement)problemDetails.Extensions["errors"]!)
            .GetProperty(propertyName)
            .EnumerateArray()
            .Select(error => error.GetString()!)
            .ToList();

    /// <summary>Attaches the actor headers required by any endpoint that resolves <c>IActorAccessor.Current</c>.</summary>
    protected static HttpRequestMessage WithActorHeaders(HttpRequestMessage request, string id, string displayName)
    {
        request.Headers.Add(Constants.UserIdHeaderKey, id);
        request.Headers.Add(Constants.UserNameHeaderKey, displayName);
        return request;
    }

    public void Dispose()
    {
        Client.Dispose();
        GC.SuppressFinalize(this);
    }
}