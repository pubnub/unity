using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace PubnubApi.Unity {

	/// <summary>
	/// This is an implementation of the PubNub Transport Layer created for Web GL builds compatibility
	/// </summary>
	public class UnityWebGLHttpClientService : IHttpClientService {

		private PubnubLogModule logger;

		public void SetLogger(PubnubLogModule logger) {
			this.logger = logger;
		}

		private TransportResponse UnityRequestToResponse(UnityWebRequest request) {
			return new TransportResponse() {
				StatusCode = (int)request.responseCode,
				Content = request.downloadHandler?.data,
				RequestUrl = request.url,
				Headers = request.GetResponseHeaders()
					.ToDictionary(
						x => x.Key,
						y => (IEnumerable<string>)new[] { y.Value })
			};
		}

		private void PrepareUnityRequest(UnityWebRequest request, TransportRequest transportRequest) {
			if (transportRequest.Timeout.HasValue) {
				request.timeout = (int)transportRequest.Timeout.Value.TotalSeconds;
			}

			if (transportRequest.Headers.Keys.Count > 0) {
				foreach (var kvp in transportRequest.Headers) {
					request.SetRequestHeader(kvp.Key, kvp.Value);
				}
			}
		}

		private TransportResponse GetTransportResponseForRequestException(Exception exception, TransportRequest transportRequest,
			UnityWebRequest requestWithTimeout)
		{
			if (requestWithTimeout == null) {
				logger?.Error($"HttpClient Service: UnityWebRequest for url {transportRequest.RequestUrl} was null!");
				return new TransportResponse()
				{
					RequestUrl = transportRequest.RequestUrl,
					Error = exception,
				};
			}

			TransportResponse transportResponse;
			if (exception is TaskCanceledException taskCanceledException) {
				logger?.Error($"HttpClient Service: TaskCanceledException for url {transportRequest.RequestUrl}");
				transportResponse = new TransportResponse()
				{
					RequestUrl = transportRequest.RequestUrl,
					Error = taskCanceledException,
				};
				logger?.Debug("HttpClient Service: Task cancelled due to cancellation request");
				transportResponse.IsCancelled = true;
			} else {
				logger?.Error(
					$"HttpClient Service: Exception for http call url {transportRequest.RequestUrl}, exception message: {exception.Message}, stacktrace: {exception.StackTrace}");
				transportResponse = new TransportResponse() {
					RequestUrl = transportRequest.RequestUrl,
					Error = exception
				};
				//Apparently error.Contains("Request timeout") is the only way to determine if request timed out
				if (!string.IsNullOrEmpty(requestWithTimeout.error) && requestWithTimeout.error.Contains("Request timeout") &&
				    !transportRequest.CancellationTokenSource.IsCancellationRequested)
				{
					logger?.Debug("HttpClient Service: Request cancelled due to timeout");
					transportResponse.IsTimeOut = true;
				}
			}

			return transportResponse;
		}

		public async Task<TransportResponse> DeleteRequest(TransportRequest transportRequest) {
			var deleteRequest = UnityWebRequest.Delete(transportRequest.RequestUrl);
			PrepareUnityRequest(deleteRequest, transportRequest);
			TransportResponse response;
			try {
				var taskCompletionSource = new TaskCompletionSource<TransportResponse>();
				transportRequest.CancellationTokenSource.Token.Register(() => {
					deleteRequest.Abort();
					taskCompletionSource.TrySetCanceled();
				});
				deleteRequest.SendWebRequest().completed += _ => {
					taskCompletionSource.TrySetResult(UnityRequestToResponse(deleteRequest));
				};
				response = await taskCompletionSource.Task.ConfigureAwait(false);
			} catch (Exception ex) {
				response = GetTransportResponseForRequestException(ex, transportRequest, deleteRequest);
			} finally {
				transportRequest.CancellationTokenSource?.Dispose();
			}
			return response;
		}

		public async Task<TransportResponse> GetRequest(TransportRequest transportRequest) {
			var getRequest = UnityWebRequest.Get(transportRequest.RequestUrl);
			PrepareUnityRequest(getRequest, transportRequest);
			TransportResponse response;
			try {
				var taskCompletionSource = new TaskCompletionSource<TransportResponse>();
				transportRequest.CancellationTokenSource.Token.Register(() => {
					getRequest.Abort();
					taskCompletionSource.TrySetCanceled();
				});
				getRequest.SendWebRequest().completed += _ => {
					taskCompletionSource.TrySetResult(UnityRequestToResponse(getRequest));
				};
				response = await taskCompletionSource.Task.ConfigureAwait(false);
			} catch (Exception ex) {
				response = GetTransportResponseForRequestException(ex, transportRequest, getRequest);
			} finally {
				transportRequest.CancellationTokenSource?.Dispose();
			}
			return response;
		}

		public async Task<TransportResponse> PostRequest(TransportRequest transportRequest) {
			TransportResponse response;
			UnityWebRequest postRequest = null;
			try {
				var formData = new List<IMultipartFormSection>();
				if (!string.IsNullOrEmpty(transportRequest.BodyContentString)) {
					formData.Add(new MultipartFormDataSection(transportRequest.BodyContentString));
				} else if (transportRequest.BodyContentBytes != null) {
					formData.Add(new MultipartFormDataSection(transportRequest.BodyContentBytes));
				}

				postRequest = UnityWebRequest.Post(transportRequest.RequestUrl, formData);
				PrepareUnityRequest(postRequest, transportRequest);
				var taskCompletionSource = new TaskCompletionSource<TransportResponse>();
				transportRequest.CancellationTokenSource.Token.Register(() => {
					postRequest.Abort();
					taskCompletionSource.TrySetCanceled();
				});
				postRequest.SendWebRequest().completed += _ => {
					taskCompletionSource.TrySetResult(UnityRequestToResponse(postRequest));
				};
				response = await taskCompletionSource.Task.ConfigureAwait(false);
			} catch (Exception ex) {
				response = GetTransportResponseForRequestException(ex, transportRequest, postRequest);
			} finally {
				transportRequest.CancellationTokenSource?.Dispose();
			}
			return response;
		}

		public async Task<TransportResponse> PatchRequest(TransportRequest transportRequest) {
			TransportResponse response;
			UnityWebRequest patchRequest = null;
			try {

				if (!string.IsNullOrEmpty(transportRequest.BodyContentString)) {
					patchRequest = UnityWebRequest.Put(transportRequest.RequestUrl, transportRequest.BodyContentString);
				} else if (transportRequest.BodyContentBytes != null) {
					patchRequest = UnityWebRequest.Put(transportRequest.RequestUrl, transportRequest.BodyContentBytes);
				} else {
					throw new ArgumentException("PATCH Transport Request has no body!");
				}
				patchRequest.method = "PATCH";

				PrepareUnityRequest(patchRequest, transportRequest);
				var taskCompletionSource = new TaskCompletionSource<TransportResponse>();
				transportRequest.CancellationTokenSource.Token.Register(() => {
					patchRequest.Abort();
					taskCompletionSource.TrySetCanceled();
				});
				patchRequest.SendWebRequest().completed += _ => {
					taskCompletionSource.TrySetResult(UnityRequestToResponse(patchRequest));
				};
				response = await taskCompletionSource.Task.ConfigureAwait(false);
			} catch (Exception ex) {
				response = GetTransportResponseForRequestException(ex, transportRequest, patchRequest);
			} finally {
				transportRequest.CancellationTokenSource?.Dispose();
			}

			return response;
		}

		public async Task<TransportResponse> PutRequest(TransportRequest transportRequest) {
			UnityWebRequest putRequest = null;
			TransportResponse response;
			try {
				if (!string.IsNullOrEmpty(transportRequest.BodyContentString)) {
					putRequest = UnityWebRequest.Put(transportRequest.RequestUrl, transportRequest.BodyContentString);
				} else if (transportRequest.BodyContentBytes != null) {
					putRequest = UnityWebRequest.Put(transportRequest.RequestUrl, transportRequest.BodyContentBytes);
				} else {
					throw new ArgumentException("PUT Transport Request has no body!");
				}

				PrepareUnityRequest(putRequest, transportRequest);
				var taskCompletionSource = new TaskCompletionSource<TransportResponse>();
				transportRequest.CancellationTokenSource.Token.Register(() => {
					putRequest.Abort();
					taskCompletionSource.TrySetCanceled();
				});
				putRequest.SendWebRequest().completed += _ => {
					taskCompletionSource.TrySetResult(UnityRequestToResponse(putRequest));
				};
				response = await taskCompletionSource.Task.ConfigureAwait(false);
			} catch (Exception ex) {
				response = GetTransportResponseForRequestException(ex, transportRequest, putRequest);
			} finally {
				transportRequest.CancellationTokenSource?.Dispose();
			}

			return response;
		}
	}
}