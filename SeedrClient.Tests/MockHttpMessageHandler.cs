using System.Net;

namespace SeedrClient.Tests;
public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly string _responseContent;
    private readonly HttpStatusCode _statusCode;
    private readonly HttpContent? _httpContent; // For stream responses

    public HttpRequestMessage? LastRequest { get; private set; }

    public MockHttpMessageHandler(string responseContent, HttpStatusCode statusCode)
    {
        _responseContent = responseContent;
        _statusCode = statusCode;
        _httpContent = null;
    }

    public MockHttpMessageHandler(HttpContent httpContent, HttpStatusCode statusCode)
    {
        _responseContent = string.Empty; // Not used if HttpContent is provided
        _statusCode = statusCode;
        _httpContent = httpContent;
    }


    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        LastRequest = request;
        HttpResponseMessage response;

        if (_httpContent != null)
        {
            response = new HttpResponseMessage(_statusCode)
            {
                Content = _httpContent
            };
        }
        else
        {
            response = new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_responseContent, System.Text.Encoding.UTF8, "application/json")
            };
        }
        return Task.FromResult(response);
    }
}