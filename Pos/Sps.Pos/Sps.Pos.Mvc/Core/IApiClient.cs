using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Sps.Pos.Dto.Response.Common;
using Sps.Pos.Infrastructure.Extension;
using System.Net.Http.Headers;

namespace Sps.Pos.Mvc.Core
{
	public interface IApiClient
	{
		AppSettingConfiguration AppSettingConfiguration { get; }

		Task<TResponse> GetAsync<TResponse>(string apiAddress, CancellationToken cancellationToken);

		Task<TResponse> GetAsync<TResponse>(string apiAddress, dynamic dynamicRequest, CancellationToken cancellationToken);

		Task<T> PostAsync<T>(string apiAddress, dynamic dynamicRequest, CancellationToken cancellationToken);

		Task<T> PutAsync<T>(string apiAddress, dynamic dynamicRequest, CancellationToken cancellationToken);

		Task<TResponse> DeleteAsync<TResponse>(string apiAddress, CancellationToken cancellationToken);
	}

	public class ApiClient : IApiClient
	{
		private readonly HttpClient _client;
		public readonly AppSettingConfiguration _appSettings;

		public AppSettingConfiguration AppSettingConfiguration => _appSettings;

		public ApiClient(HttpClient client, IOptions<AppSettingConfiguration> settings, IHttpContextAccessor context)
		{
			_appSettings = settings.Value;
			_client = client;
			_client.BaseAddress = new Uri(_appSettings.ApiBaseAddress);
			_client.Timeout = new TimeSpan(0, 0, _appSettings.TimeoutSeconds);
			_client.DefaultRequestHeaders.Clear();
			var tkn = context?.HttpContext?.User?.GetAuthenticationToken();
			if (!string.IsNullOrWhiteSpace(tkn))
			{
				_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tkn);
			}
		}

		public async Task<TResponse> GetAsync<TResponse>(string apiAddress, CancellationToken cancellationToken)
		{
			var request = new HttpRequestMessage(HttpMethod.Get, apiAddress);
			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

			using (var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
			{
				var stream = await response.Content.ReadAsStreamAsync();
				if (response.IsSuccessStatusCode)
				{
					return stream.ReadAndDeserializeFromJson<TResponse>();
				}
				else
				{
					var content = await stream.StreamToStringAsync();
					var errorObject = JsonConvert.DeserializeObject<ErrorResponse>(content);
					throw new ApiException
					{
						Content = content,
						StatusCode = (int)response.StatusCode,
						ErrorCode = errorObject?.ErrorCode,
						ErrorMessage = errorObject?.ErrorMessage,
					};
				}
			}
		}

		public async Task<TResponse> GetAsync<TResponse>(string apiAddress, dynamic dynamicRequest, CancellationToken cancellationToken)
		{
			var request = new HttpRequestMessage(HttpMethod.Get, apiAddress);
			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			request.Content = new StringContent(JsonConvert.SerializeObject(dynamicRequest));
			request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

			using (var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
			{
				var stream = await response.Content.ReadAsStreamAsync();
				if (response.IsSuccessStatusCode)
				{
					return stream.ReadAndDeserializeFromJson<TResponse>();
				}
				else
				{
					var content = await stream.StreamToStringAsync();
					var errorObject = JsonConvert.DeserializeObject<ErrorResponse>(content);
					throw new ApiException
					{
						Content = content,
						StatusCode = (int)response.StatusCode,
						ErrorCode = errorObject?.ErrorCode,
						ErrorMessage = errorObject?.ErrorMessage,
					};
				}
			}
		}

		public async Task<TResponse> PostAsync<TResponse>(string apiAddress, dynamic dynamicRequest, CancellationToken cancellationToken)
		{
			var request = new HttpRequestMessage(HttpMethod.Post, apiAddress);
			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			request.Content = new StringContent(JsonConvert.SerializeObject(dynamicRequest));
			request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
			var stream = await response.Content.ReadAsStreamAsync();
			if (response.IsSuccessStatusCode)
			{
				return stream.ReadAndDeserializeFromJson<TResponse>();
			}
			else
			{
				var content = await stream.StreamToStringAsync();
				var errorObject = JsonConvert.DeserializeObject<ErrorResponse>(content);
				throw new ApiException
				{
					Content = content,
					StatusCode = (int)response.StatusCode,
					ErrorCode = errorObject?.ErrorCode,
					ErrorMessage = errorObject?.ErrorMessage,
				};
			}
		}

		public async Task<TResponse> PutAsync<TResponse>(string apiAddress, dynamic dynamicRequest, CancellationToken cancellationToken)
		{
			var request = new HttpRequestMessage(HttpMethod.Put, apiAddress);
			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			request.Content = new StringContent(JsonConvert.SerializeObject(dynamicRequest));
			request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
			var stream = await response.Content.ReadAsStreamAsync();
			if (response.IsSuccessStatusCode)
			{
				return stream.ReadAndDeserializeFromJson<TResponse>();
			}
			else
			{
				var content = await stream.StreamToStringAsync();
				var errorObject = JsonConvert.DeserializeObject<ErrorResponse>(content);
				throw new ApiException
				{
					Content = content,
					StatusCode = (int)response.StatusCode,
					ErrorCode = errorObject?.ErrorCode,
					ErrorMessage = errorObject?.ErrorMessage,
				};
			}
		}

		public async Task<TResponse> DeleteAsync<TResponse>(string apiAddress, CancellationToken cancellationToken)
		{
			var request = new HttpRequestMessage(HttpMethod.Delete, apiAddress);
			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

			using (var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
			{
				var stream = await response.Content.ReadAsStreamAsync();
				if (response.IsSuccessStatusCode)
				{
					return stream.ReadAndDeserializeFromJson<TResponse>();
				}
				else
				{
					var content = await stream.StreamToStringAsync();
					var errorObject = JsonConvert.DeserializeObject<ErrorResponse>(content);
					throw new ApiException
					{
						Content = content,
						StatusCode = (int)response.StatusCode,
						ErrorCode = errorObject?.ErrorCode,
						ErrorMessage = errorObject?.ErrorMessage,
					};
				}
			}
		}
	}

	public class ApiException : Exception
	{
		public int StatusCode { get; set; }

		public string ErrorCode { get; set; }

		public string ErrorMessage { get; set; }

		public string Content { get; set; }
	}
}
