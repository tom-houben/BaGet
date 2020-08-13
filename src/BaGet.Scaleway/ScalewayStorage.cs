using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Aws4RequestSigner;
using BaGet.Core;
using Microsoft.Extensions.Options;

namespace BaGet.Scaleway
{
    public class ScalewayStorage: IStorageService
    {
        private readonly ScalewayObjectStorageOptions _options;
        private readonly HttpClient _httpClient;

        public ScalewayStorage(IOptionsSnapshot<ScalewayObjectStorageOptions> options, HttpClient httpClient)
        {
            _options = options.Value;
            _httpClient = httpClient;
        }

        public async Task<Stream> GetAsync(string path, CancellationToken cancellationToken = default)
        {
            var signer = new AWS4RequestSigner(_options.AccessKey, _options.SecretKey);

            var uri = new Uri(_options.BucketEndpoint);
            uri = new Uri(uri, path);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = uri
            };

            request = await signer.Sign(request, "s3", "nl-ams");
            var result = await _httpClient.SendAsync(request, cancellationToken);

            if (result.IsSuccessStatusCode)
                return await result.Content.ReadAsStreamAsync();

            return null;
        }

        public Task<Uri> GetDownloadUriAsync(string path, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<Uri>(null);
        }

        public async Task<StoragePutResult> PutAsync(string path, Stream content, string contentType, CancellationToken cancellationToken = default)
        {
            var signer = new AWS4RequestSigner(_options.AccessKey, _options.SecretKey);

            var uri = new Uri(_options.BucketEndpoint);
            uri = new Uri(uri, path);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                Content = new StreamContent(content),
                RequestUri = uri
            };

            request = await signer.Sign(request, "s3", "nl-ams");
            var result = await _httpClient.SendAsync(request, cancellationToken);

            return result.IsSuccessStatusCode
                ? StoragePutResult.Success
                : StoragePutResult.Conflict;
        }

        public async Task DeleteAsync(string path, CancellationToken cancellationToken = default)
        {
            var signer = new AWS4RequestSigner(_options.AccessKey, _options.SecretKey);

            var uri = new Uri(_options.BucketEndpoint);
            uri = new Uri(uri, path);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = uri
            };

            request = await signer.Sign(request, "s3", "nl-ams");
            await _httpClient.SendAsync(request, cancellationToken);
        }
    }
}
