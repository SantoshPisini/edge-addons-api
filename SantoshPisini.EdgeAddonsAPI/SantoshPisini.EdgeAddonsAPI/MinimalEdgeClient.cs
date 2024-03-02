using SantoshPisini.EdgeAddonsAPI.Exceptions;
using SantoshPisini.EdgeAddonsAPI.Models;

namespace SantoshPisini.EdgeAddonsAPI
{
    public static class MinimalEdgeClient
    {
        private const int MAX_RETRY_COUNT = 5;
        private const int INITIAL_TIME_DELAY_IN_SECONDS = 3;

        /// <summary>
        /// Uploads the given zip package & then publishes it.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task UploadThenPublish(PublishModel model)
        {
            var client = new EdgeClient();
            var tokenResult = await client.GetAccessToken(model.ClientId, model.ClientSecret, model.AccessTokenURL);
            var accessToken = tokenResult.AccessToken!;
            var operationId = await client.UploadPackage(accessToken, model.ProductId, model.PackageUrl);
            var uploadStatus = await ExecuteWithRetry(client.GetPackageStatus, accessToken, model.ProductId, operationId!);
            var publishOperationId = await client.PublishPackage(accessToken, model.ProductId, model.Notes);
            await ExecuteWithRetry(client.GetPublishStatus, accessToken, model.ProductId, publishOperationId!);
        }

        private static async Task<StatusResponseModel> ExecuteWithRetry(Func<string, string, string, Task<StatusResponseModel>> action, params string[] args)
        {
            var retries = 0;
            TimeSpan delay = TimeSpan.FromSeconds(INITIAL_TIME_DELAY_IN_SECONDS);

            while (retries < MAX_RETRY_COUNT)
            {
                var result = await action(args[0], args[1], args[2]);
                switch (result.Status)
                {
                    case "Succeeded":
                        return result;
                    case "Failed":
                        throw new EdgeClientException($"Failed with error code: {result.ErrorCode}, message: {result.Message}, errors: {string.Join(", ", result.Errors?.Select(x => x.Message))}");
                    default:
                        break;
                }
                await Task.Delay(delay);
                retries++;
                delay *= 2;
            }

            throw new EdgeClientException("Retry limit exceeded.");
        }
    }
}

