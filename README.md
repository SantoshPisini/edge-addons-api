
# Microsoft Edge Addons API in .NET

This repository is a library for the [Microsoft Edge Add-ons API ](https://learn.microsoft.com/en-us/microsoft-edge/extensions-chromium/publish/api/using-addons-api) that is used for uploading and publishing new versions of your add-ons to the [Microsoft Edge Add-ons store](https://microsoftedge.microsoft.com/addons/Microsoft-Edge-Extensions-Home).

## Usage

### Minimal Edge Client
Uploads the given zip package & then publishes it.

    var model = new PublishModel
    {
	    ClientId = clientId,
	    ClientSecret = clientSecret,
	    AccessTokenURL = accessTokenURL,
	    ProductId = productId,
	    PackageUrl = packageUrl,
	    Notes = "Publishing notes."
    };

    await MinimalEdgeClient.UploadThenPublish(model);

### Edge Client
 Edge Client provides a more control over the steps while publishing the extension.

    var client = new EdgeClient();
	var tokenResult = await client.GetAccessToken(clientId, clientSecret, accessTokenURL);
	var accessToken = tokenResult.AccessToken!;
	var operationId = await client.UploadPackage(accessToken, productId, packageUrl);
	var uploadStatus = await client.GetPackageStatus(accessToken, productId, operationId!);
	var publishOperationId = await client.PublishPackage(accessToken, productId, "Some Notes");
	var publishUploadStatus = await client.GetPublishStatus(accessToken, productId, publishOperationId!);

> Exception: EdgeClientException is thrown when there is a failure.

### References

 - [Using the Microsoft Edge Add-ons API](https://learn.microsoft.com/en-us/microsoft-edge/extensions-chromium/publish/api/using-addons-api)
 - [Microsoft Edge Add-ons API Reference](https://learn.microsoft.com/en-us/microsoft-edge/extensions-chromium/publish/api/addons-api-reference)
