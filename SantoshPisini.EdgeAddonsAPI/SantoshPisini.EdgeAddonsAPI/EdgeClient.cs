using SantoshPisini.EdgeAddonsAPI.Exceptions;
using SantoshPisini.EdgeAddonsAPI.Models;
using System.Net;
using System.Text.Json;

namespace SantoshPisini.EdgeAddonsAPI
{
    /// <summary>
    /// EdgeClient provides a more control over the steps while publishing the extension.
    /// </summary>
    public class EdgeClient
    {
        private readonly string BaseAddress = "https://api.addons.microsoftedge.microsoft.com";


        /// <summary>
        /// Initialize Edge Client, used to generate access token for subsequent requests.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="EdgeClientException"></exception>
        public async Task<TokenResponseModel> GetAccessToken(string clientId, string clientSecret, string accessTokenURL)
        {
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(accessTokenURL))
            {
                throw new EdgeClientException("ClientId, ClientSecret or AccessTokenURL is empty.");
            }
            using var client = new HttpClient();
            var formData = new List<KeyValuePair<string, string>>
                {
                    new("client_id", clientId),
                    new("scope", "https://api.addons.microsoftedge.microsoft.com/.default"),
                    new("client_secret", clientSecret),
                    new("grant_type", "client_credentials")
                };
            var request = new HttpRequestMessage(HttpMethod.Post, accessTokenURL) { Content = new FormUrlEncodedContent(formData) };
            var response = await client.SendAsync(request);
            return response.StatusCode switch
            {
                HttpStatusCode.OK => JsonSerializer.Deserialize<TokenResponseModel>(await response.Content.ReadAsStringAsync())!,
                HttpStatusCode.BadRequest => throw new EdgeClientException("ClientId or ClientSecret is invalid."),
                HttpStatusCode.NotFound => throw new EdgeClientException("AccessTokenURL is invalid."),
                _ => throw new EdgeClientException($"Something went wrong! Status Code: {response.StatusCode}"),
            };
        }

        /// <summary>
        /// Uploads a package to update an existing draft submission of an extension.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Operation Id</returns>
        /// <exception cref="EdgeClientException"></exception>
        public async Task<string?> UploadPackage(string accessToken, string productId, string packageUrl)
        {
            if (string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(packageUrl))
            {
                throw new EdgeClientException("ProductId, AccessToken or PackageUrl is empty.");
            }
            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseAddress}/v1/products/{productId}/submissions/draft/package");
            request.Headers.Add("Authorization", $"Bearer {accessToken}");
            try
            {
                request.Content = new StreamContent(await client.GetStreamAsync(packageUrl));
            }
            catch (Exception ex)
            {
                throw new EdgeClientException($"Failed to get package from url. Error: {ex.Message}", ex);
            }
            request.Content!.Headers.Add("Content-Type", "application/zip");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return response.Headers.GetValues("Location")?.First();
        }

        /// <summary>
        /// Get the status of package upload.
        /// </summary>
        /// <param name="model"></param>
        /// <exception cref="EdgeClientException"></exception>
        public async Task<StatusResponseModel> GetPackageStatus(string accessToken, string productId, string operationID)
        {
            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(operationID))
            {
                throw new EdgeClientException("ProductId, AccessToken or OperationID is empty.");
            }
            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseAddress}/v1/products/{productId}/submissions/draft/package/operations/{operationID}");
            request.Headers.Add("Authorization", $"Bearer {accessToken}");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return JsonSerializer.Deserialize<StatusResponseModel>(await response.Content.ReadAsStringAsync())!;
        }

        /// <summary>
        /// Publish the current draft of the pacakge.
        /// </summary>
        /// <param name="model"></param>
        /// <exception cref="EdgeClientException"></exception>
        public async Task<string?> PublishPackage(string accessToken, string productId, string notes = "")
        {
            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(productId))
            {
                throw new EdgeClientException("ProductId or AccessToken is empty.");
            }
            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseAddress}/v1/products/{productId}/submissions");
            request.Headers.Add("Authorization", $"Bearer {accessToken}");
            request.Content = new StringContent(JsonSerializer.Serialize(new
            {
                notes
            }), null, "application/json");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return response.Headers.GetValues("Location")?.First();
        }

        /// <summary>
        /// Get the status of published package.
        /// </summary>
        /// <param name="model"></param>
        /// <exception cref="EdgeClientException"></exception>
        public async Task<StatusResponseModel> GetPublishStatus(string accessToken, string productId, string operationID)
        {
            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(operationID))
            {
                throw new EdgeClientException("ProductId, AccessToken or OperationID is empty.");
            }
            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseAddress}/v1/products/{productId}/submissions/operations/{operationID}");
            request.Headers.Add("Authorization", $"Bearer {accessToken}");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return JsonSerializer.Deserialize<StatusResponseModel>(await response.Content.ReadAsStringAsync())!;
        }
    }
}
