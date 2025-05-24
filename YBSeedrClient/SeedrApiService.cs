


namespace YBSeedrClient;
/// <summary>
/// The seedr api service.
/// </summary>
public partial class SeedrApiService : ISeedrApiService
{
    /// <summary>
    /// The http client.
    /// </summary>
    private readonly HttpClient _httpClient;
    /// <summary>
    /// The base url.
    /// </summary>
    private const string BaseUrl = "https://www.seedr.cc/rest";

    /// <summary>
    /// The browser launcher.
    /// </summary>
    private readonly IBrowserLauncher _browserLauncher;
    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<SeedrApiService> _logger;
    /// <summary>
    /// The cached json serializer options.
    /// </summary>
    private static readonly JsonSerializerOptions CachedJsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="SeedrApiService"/> class.
    /// </summary>
    /// <param name="httpClient">The http client.</param>
    /// <param name="browserLauncher">The browser launcher.</param>
    /// <param name="logger">The logger.</param>
    public SeedrApiService(HttpClient httpClient, IBrowserLauncher browserLauncher, ILogger<SeedrApiService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _browserLauncher = browserLauncher ?? throw new ArgumentNullException(nameof(browserLauncher));
        _logger=logger ?? NullLogger<SeedrApiService>.Instance;

    }


    /// <summary>
    /// Initializes a new instance of the <see cref="SeedrApiService"/> class.
    /// </summary>
    /// <param name="email">The email.</param>
    /// <param name="password">The password.</param>
    /// <param name="browserLauncher">The browser launcher.</param>
    /// <param name="messageHandler">The message handler.</param>
    /// <param name="logger">The logger.</param>
    public SeedrApiService(string email, string password, IBrowserLauncher browserLauncher,
        HttpMessageHandler? messageHandler = null, ILogger<SeedrApiService>? logger = null)
    {
        var handler = messageHandler ?? new HttpClientHandler { AllowAutoRedirect = true };
        _httpClient = new HttpClient(handler);
        var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{email}:{password}"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _browserLauncher = browserLauncher ?? throw new ArgumentNullException(nameof(browserLauncher));
        _logger = logger ?? NullLogger<SeedrApiService>.Instance;
    }


    /// <summary>
    /// Get full url.
    /// </summary>
    /// <param name="endpoint">The endpoint.</param>
    /// <returns>A string</returns>
    private static string GetFullUrl(string endpoint) => $"{BaseUrl}/{endpoint}";

    /// <summary>
    /// Get and return a task of type t? asynchronously.
    /// </summary>
    /// <typeparam name="T"/>
    /// <param name="endpoint">The endpoint.</param>
    /// <exception cref="InvalidOperationException"></exception>
    /// <returns><![CDATA[Task<T?>]]></returns>
    private async Task<T?> GetAsync<T>(string endpoint)
    {
        HttpResponseMessage response;
        try
        {
            string fullUrl = GetFullUrl(endpoint); // Assuming GetFullUrl is correct
            _logger.LogDebug($"GET request to: {fullUrl}");
            response = await _httpClient.GetAsync(fullUrl); // Removed GetFullUrl from here assuming it's done above
            _logger.LogDebug($"GET response Status Code: {response.StatusCode}");
            response.EnsureSuccessStatusCode(); // This will throw an HttpRequestException if not a success code (2xx)
        }
        catch (HttpRequestException ex)
        {
            _logger.LogDebug($"HttpRequestException in GetAsync for {endpoint}: {ex.Message}");
            _logger.LogDebug($"Status Code from exception: {ex.StatusCode}"); // Log status code if available
                                                                              // Handle error appropriately, maybe return default(T) or throw custom exception
            return default;
        }
        catch (Exception ex) // Catch other potential exceptions like TaskCanceledException (timeout)
        {
            _logger.LogDebug($"Generic Exception in GetAsync for {endpoint}: {ex.ToString()}");
            return default;
        }

        _logger.LogDebug($"Is response.Content null AFTER GetAsync? {response.Content == null}");
        if (response.Content == null)
        {
            _logger.LogDebug("response.Content IS NULL. This is unexpected for a successful GET request.");
            // What are the response headers?
            _logger.LogDebug("Response Headers:");
            foreach (var header in response.Headers)
            {
                _logger.LogDebug($"  {header.Key}: {string.Join(", ", header.Value)}");
            }
            // If response.Content is null, HandleResponse will likely fail or behave unexpectedly.
            // You might return default(T) or throw here if this state is invalid.
            return default; // Or throw new InvalidOperationException("Response content was null after successful GET.");
        }

        return await HandleResponse<T>(response);
    }
    /// <summary>
    /// Post and return a task of type t? asynchronously.
    /// </summary>
    /// <typeparam name="T"/>
    /// <param name="endpoint">The endpoint.</param>
    /// <param name="content">The content.</param>
    /// <returns><![CDATA[Task<T?>]]></returns>
    private async Task<T?> PostAsync<T>(string endpoint, HttpContent? content)
    {
        var response = await _httpClient.PostAsync(GetFullUrl(endpoint), content);
        return await HandleResponse<T>(response);
    }

    /// <summary>
    /// Deletes and return a task of type t? asynchronously.
    /// </summary>
    /// <typeparam name="T"/>
    /// <param name="endpoint">The endpoint.</param>
    /// <returns><![CDATA[Task<T?>]]></returns>
    private async Task<T?> DeleteAsync<T>(string endpoint)
    {
        var response = await _httpClient.DeleteAsync(GetFullUrl(endpoint));
        return await HandleResponse<T>(response);
    }

    // Update the HandleResponse method to use the source-generated context
    /// <summary>
    /// Handle the response.
    /// </summary>
    /// <typeparam name="T"/>
    /// <param name="response">The response.</param>
    /// <returns><![CDATA[Task<T?>]]></returns>
    private async Task<T?> HandleResponse<T>(HttpResponseMessage response)
    {
        var jsonResponse = await response.Content.ReadAsStringAsync();
        _logger.LogDebug($"DEBUG: Status Code: {response.StatusCode}, Response: {jsonResponse.Substring(0, Math.Min(500, jsonResponse.Length))}");

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogDebug($"API Error: {response.StatusCode}");
            try
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogDebug(errorContent);
                var errorApiResult = new ApiResult { Result = false, Error = $"API Error: {response.StatusCode} (Content: {errorContent})" };
                return (T)(object)errorApiResult;
            }

            catch (JsonException)
            {
                /* Could not parse error as ApiResult */
            }
            return default;
        }

        try
        {
            _logger.LogDebug($"Attempting to deserialize to type: {typeof(T).FullName}");
            T? s = JsonSerializer.Deserialize<T>(jsonResponse, CachedJsonSerializerOptions);
            _logger.LogDebug($"Deserialization result 's' is null: {s == null}");

            if (s != null)
            {
                if (s is ApiResult apiResult && typeof(T) == typeof(ApiResult))
                {
                    _logger.LogDebug($"  ApiResult.Code: {apiResult.Code}");
                    _logger.LogDebug($"  ApiResult.Result: {apiResult.Result}");
                    _logger.LogDebug($"  ApiResult.Account is null: {apiResult.Account == null}");
                    if (apiResult.Account != null)
                    {
                        _logger.LogDebug($"    ApiResult.Account.Username: {apiResult.Account.Username}");
                        _logger.LogDebug($"    ApiResult.Account.Id: {apiResult.Account.Id}");
                        _logger.LogDebug($"    ApiResult.Account.StorageTotal: {apiResult.Account.StorageTotal}");
                    }
                }
            }
            return s;
        }
        catch (JsonException ex)
        {
            _logger.LogDebug($"JSON Deserialization Error for type {typeof(T).FullName}: {ex.Message}");
            _logger.LogDebug($"JSON that failed: {jsonResponse.Substring(0, Math.Min(1000, jsonResponse.Length))}");
            return default;
        }
    }

    /// <summary>
    /// List root folder asynchronously.
    /// </summary>
    /// <returns><![CDATA[Task<FolderContent?>]]></returns>
    public Task<FolderContent?> ListRootFolderAsync() => GetAsync<FolderContent>("folder");
    /// <summary>
    /// List the folder asynchronously.
    /// </summary>
    /// <param name="folderId">The folder id.</param>
    /// <returns><![CDATA[Task<FolderContent?>]]></returns>
    public Task<FolderContent?> ListFolderAsync(long folderId) => GetAsync<FolderContent>($"folder/{folderId}");
    /// <summary>
    /// Get file download link asynchronously.
    /// </summary>
    /// <param name="fileId">The file id.</param>
    /// <returns><![CDATA[Task<DownloadLinkResult>]]></returns>
    public async Task<DownloadLinkResult> GetFileDownloadLinkAsync(long fileId)
    {
        var requestUrl = GetFullUrl($"file/{fileId}");
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                if (response.IsSuccessStatusCode)
                {
                    return new DownloadLinkResult { Url = response.RequestMessage?.RequestUri?.ToString(), Success = true, StatusCode = response.StatusCode };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogDebug($"Error getting file download link: {response.StatusCode} - {errorContent}");
                    return new DownloadLinkResult { Success = false, ErrorMessage = $"API Error: {response.StatusCode}", StatusCode = response.StatusCode };
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogDebug($"HttpRequestException getting file download link: {ex.Message}");
                return new DownloadLinkResult { Success = false, ErrorMessage = ex.Message, StatusCode = ex.StatusCode };
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"Generic exception getting file download link: {ex.ToString()}");
                return new DownloadLinkResult { Success = false, ErrorMessage = ex.Message };
            }
        }
    }

    /// <summary>
    /// Get folder as zip link asynchronously.
    /// </summary>
    /// <param name="folderId">The folder id.</param>
    /// <returns><![CDATA[Task<DownloadLinkResult>]]></returns>
    public async Task<DownloadLinkResult> GetFolderAsZipLinkAsync(long folderId)
    {
        var requestUrl = GetFullUrl($"folder/{folderId}/download");
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if (response.IsSuccessStatusCode)
            {
                return new DownloadLinkResult { Url = response.RequestMessage?.RequestUri?.ToString(), Success = true, StatusCode = response.StatusCode };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogDebug($"Error getting folder download link: {response.StatusCode} - {errorContent}");
                return new DownloadLinkResult { Success = false, ErrorMessage = $"API Error: {response.StatusCode}", StatusCode = response.StatusCode };
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogDebug($"HttpRequestException getting folder download link: {ex.Message}");
            return new DownloadLinkResult { Success = false, ErrorMessage = ex.Message, StatusCode = ex.StatusCode };
        }
        catch (Exception ex)
        {
            _logger.LogDebug($"Generic exception getting folder download link: {ex.ToString()}");
            return new DownloadLinkResult { Success = false, ErrorMessage = ex.Message };
        }

    }

    /// <summary>
    /// Creates the folder asynchronously.
    /// </summary>
    /// <param name="fullPath">The full path.</param>
    /// <returns><![CDATA[Task<ApiResult?>]]></returns>
    public Task<ApiResult?> CreateFolderAsync(string fullPath)
    {
        var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("path", fullPath) });
        return PostAsync<ApiResult>("folder", content);
    }

    /// <summary>
    /// Renames the folder asynchronously.
    /// </summary>
    /// <param name="folderId">The folder id.</param>
    /// <param name="newName">The new name.</param>
    /// <returns><![CDATA[Task<ApiResult?>]]></returns>
    public Task<ApiResult?> RenameFolderAsync(long folderId, string newName)
    {
        var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("rename_to", newName) });
        return PostAsync<ApiResult>($"folder/{folderId}/rename", content);
    }

    /// <summary>
    /// Deletes the folder asynchronously.
    /// </summary>
    /// <param name="folderId">The folder id.</param>
    /// <returns><![CDATA[Task<ApiResult?>]]></returns>
    public Task<ApiResult?> DeleteFolderAsync(long folderId) => DeleteAsync<ApiResult>($"folder/{folderId}");

    /// <summary>
    /// Download file on preferred browser asynchronously.
    /// </summary>
    /// <param name="fileId">The file id.</param>
    /// <returns><![CDATA[Task<bool>]]></returns>
    public async Task<bool> DownloadFileOnPreferredBrowserAsync(long fileId)
    {
        DownloadLinkResult linkResult = await GetFileDownloadLinkAsync(fileId);
        if (linkResult.Success && !string.IsNullOrEmpty(linkResult.Url))
        {
            return await _browserLauncher.OpenAsync(linkResult.Url, SeedrBrowserLaunchMode.SystemPreferred);
        }
        else
        {
            _logger.LogDebug($"Failed to get download link for file {fileId}: {linkResult.ErrorMessage}");
            return false;
        }
    }
    /// <summary>
    /// Downloads folder on preferred browser asynchronously.
    /// </summary>
    /// <param name="folderId">The folder id.</param>
    /// <returns><![CDATA[Task<bool>]]></returns>
    public async Task<bool> DownloadFolderOnPreferredBrowserAsync(long folderId)
    {
        var requestUrl = GetFullUrl($"folder/{folderId}/download");
        return await _browserLauncher.OpenAsync(requestUrl, SeedrBrowserLaunchMode.SystemPreferred);
    }


    /// <summary>
    /// Get file hls url asynchronously.
    /// </summary>
    /// <param name="fileId">The file id.</param>
    /// <returns><![CDATA[Task<ApiResult?>]]></returns>
    public Task<ApiResult?> GetFileHlsUrlAsync(long fileId) => GetAsync<ApiResult>($"file/{fileId}/hls");

    /// <summary>
    /// Rename the file asynchronously.
    /// </summary>
    /// <param name="fileId">The file id.</param>
    /// <param name="newName">The new name.</param>
    /// <returns><![CDATA[Task<ApiResult?>]]></returns>
    public Task<ApiResult?> RenameFileAsync(long fileId, string newName)
    {
        var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("rename_to", newName) });
        return PostAsync<ApiResult>($"file/{fileId}/rename", content);
    }

    /// <summary>
    /// Delete the file asynchronously.
    /// </summary>
    /// <param name="fileId">The file id.</param>
    /// <returns><![CDATA[Task<ApiResult?>]]></returns>
    public Task<ApiResult?> DeleteFileAsync(long fileId) => DeleteAsync<ApiResult>($"file/{fileId}");

    /// <summary>
    /// Add the magnet asynchronously.
    /// </summary>
    /// <param name="magnetLink">The magnet link.</param>
    /// <returns><![CDATA[Task<ApiResult?>]]></returns>
    public Task<ApiResult?> AddMagnetAsync(string magnetLink)
    {
        var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("magnet", magnetLink) });
        return PostAsync<ApiResult>("transfer/magnet", content);
    }

    /// <summary>
    /// Add the url asynchronously.
    /// </summary>
    /// <param name="url">The url.</param>
    /// <returns><![CDATA[Task<ApiResult?>]]></returns>
    public Task<ApiResult?> AddUrlAsync(string url)
    {
        var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("url", url) });
        return PostAsync<ApiResult>("transfer/url", content);
    }

    // Modified AddFileAsync to accept an optional stream for testing
    /// <summary>
    /// Add the file asynchronously.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <param name="testStream">The test stream.</param>
    /// <returns><![CDATA[Task<ApiResult?>]]></returns>
    public async Task<ApiResult?> AddFileAsync(string filePath, Stream? testStream = null)
    {
        Stream streamToUpload;
        string fileName = Path.GetFileName(filePath);

        if (testStream != null) // For unit testing
        {
            streamToUpload = testStream;
        }
        else // For actual operation
        {
            if (!System.IO.File.Exists(filePath))
            {
                var errorMessage = $"File not found: {filePath}";
                _logger.LogDebug(errorMessage);
                return new ApiResult { Result = false, Error = "Local file not found." };
            }
            streamToUpload = System.IO.File.OpenRead(filePath);
        }

        using (streamToUpload) // Ensure stream is disposed if created here
        using (var content = new MultipartFormDataContent())
        {
            var streamContent = new StreamContent(streamToUpload);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            content.Add(streamContent, "file", fileName);
            return await PostAsync<ApiResult>("transfer/file", content);
        }
    }

    /// <summary>
    /// Get transfer data asynchronously.
    /// </summary>
    /// <param name="transferId">The transfer id.</param>
    /// <returns><![CDATA[Task<SeedrTransfer?>]]></returns>
    public Task<SeedrTransfer?> GetTransferDataAsync(long transferId) => GetAsync<SeedrTransfer>($"transfer/{transferId}");
    /// <summary>
    /// Deletes the transfer asynchronously.
    /// </summary>
    /// <param name="transferId">The transfer id.</param>
    /// <returns><![CDATA[Task<ApiResult?>]]></returns>
    public Task<ApiResult?> DeleteTransferAsync(long transferId) => DeleteAsync<ApiResult>($"transfer/{transferId}");
    /// <summary>
    /// Get user data asynchronously.
    /// </summary>
    /// <returns><![CDATA[Task<SeedrUser?>]]></returns>
    public async Task<SeedrUser?> GetUserDataAsync()
    {
        var apiResponse = await GetAsync<ApiResult>("user");
        return apiResponse?.Account;
    }

    /// <summary>
    /// Get folder link.
    /// </summary>
    /// <param name="folderId">The folder id.</param>
    /// <returns>A string</returns>
    public string GetFolderLink(long folderId)
    {
        return GetFullUrl($"folder/{folderId}/download");
    }

    /// <summary>
    /// Get file link.
    /// </summary>
    /// <param name="fileId">The file id.</param>
    /// <returns>A string</returns>
    public string GetFileLink(long fileId)
    {
        return GetFullUrl($"file/{fileId}");
    }
}