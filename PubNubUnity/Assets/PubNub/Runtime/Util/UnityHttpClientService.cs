using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace PubnubApi.Unity {

	public class UnityHttpClientService : IHttpClientService {
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
				Debug.LogWarning($"TIMEOUT: {(int)transportRequest.Timeout.Value.TotalSeconds}");
				request.timeout = (int)transportRequest.Timeout.Value.TotalSeconds;
			}

			if (transportRequest.Headers.Keys.Count > 0) {
				foreach (var kvp in transportRequest.Headers) {
					request.SetRequestHeader(kvp.Key, kvp.Value);
				}
			}
		}

		public async Task<TransportResponse> DeleteRequest(TransportRequest transportRequest) {
			Debug.LogWarning("DELETE");
			TransportResponse response;
			try {
				var deleteRequest = UnityWebRequest.Delete(transportRequest.RequestUrl);
				PrepareUnityRequest(deleteRequest, transportRequest);
				var taskCompletionSource = new TaskCompletionSource<TransportResponse>();
				transportRequest.CancellationToken.Register(() => {
					deleteRequest.Abort();
					taskCompletionSource.TrySetCanceled();
				});
				deleteRequest.SendWebRequest().completed += _ => {
					taskCompletionSource.TrySetResult(UnityRequestToResponse(deleteRequest));
				};
				response = await taskCompletionSource.Task.ConfigureAwait(false);
			} catch (Exception ex) {
				Debug.LogError($"DELETE Error: {ex}");
				response = new TransportResponse() {
					RequestUrl = transportRequest.RequestUrl,
					Error = ex
				};
			}
			return response;
		}

		public async Task<TransportResponse> GetRequest(TransportRequest transportRequest) {
			Debug.LogWarning($"GET: {transportRequest.RequestUrl}");
			TransportResponse response;
			try {
				var getRequest = UnityWebRequest.Get(transportRequest.RequestUrl);
				PrepareUnityRequest(getRequest, transportRequest);
				Debug.LogWarning($"GET PREPARED: {Time.frameCount}");
				var taskCompletionSource = new TaskCompletionSource<TransportResponse>();
				transportRequest.CancellationToken.Register(() => {
					getRequest.Abort();
					taskCompletionSource.TrySetCanceled();
				});
				getRequest.SendWebRequest().completed += _ => {
					Debug.LogWarning($"GET COMPLETED: {Time.frameCount}");
					taskCompletionSource.TrySetResult(UnityRequestToResponse(getRequest));
					Debug.LogWarning("GET RESULT SET");
					Debug.LogError($"GET ERROR: {getRequest.error}");
				};
				response = await taskCompletionSource.Task.ConfigureAwait(false);
			} catch (Exception ex) {
				Debug.LogError($"GET Error: {ex}");
				response = new TransportResponse() {
					RequestUrl = transportRequest.RequestUrl,
					Error = ex
				};
			}
			return response;
		}

		public async Task<TransportResponse> PostRequest(TransportRequest transportRequest) {
			Debug.LogWarning("POST");
			TransportResponse response;
			try {
				var formData = new List<IMultipartFormSection>();
				if (!string.IsNullOrEmpty(transportRequest.BodyContentString)) {
					formData.Add(new MultipartFormDataSection(transportRequest.BodyContentString));
				} else if (transportRequest.BodyContentBytes != null) {
					formData.Add(new MultipartFormDataSection(transportRequest.BodyContentBytes));
				}

				var postRequest = UnityWebRequest.Post(transportRequest.RequestUrl, formData);
				PrepareUnityRequest(postRequest, transportRequest);
				var taskCompletionSource = new TaskCompletionSource<TransportResponse>();
				transportRequest.CancellationToken.Register(() => {
					postRequest.Abort();
					taskCompletionSource.TrySetCanceled();
				});
				postRequest.SendWebRequest().completed += _ => {
					taskCompletionSource.TrySetResult(UnityRequestToResponse(postRequest));
				};
				response = await taskCompletionSource.Task.ConfigureAwait(false);
			} catch (Exception ex) {
				Debug.LogError($"POST Error: {ex}");
				response = new TransportResponse() {
					RequestUrl = transportRequest.RequestUrl,
					Error = ex
				};
			}
			return response;
		}

		public async Task<TransportResponse> PatchRequest(TransportRequest transportRequest) {
			Debug.LogWarning("PATCH");
			TransportResponse response;
			try {
				UnityWebRequest patchRequest;
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
				transportRequest.CancellationToken.Register(() => {
					patchRequest.Abort();
					taskCompletionSource.TrySetCanceled();
				});
				patchRequest.SendWebRequest().completed += _ => {
					taskCompletionSource.TrySetResult(UnityRequestToResponse(patchRequest));
				};
				response = await taskCompletionSource.Task.ConfigureAwait(false);
			} catch (Exception ex) {
				Debug.LogError($"PATCH Error: {ex}");
				response = new TransportResponse() {
					RequestUrl = transportRequest.RequestUrl,
					Error = ex
				};
			}

			return response;
		}

		public async Task<TransportResponse> PutRequest(TransportRequest transportRequest) {
			Debug.LogWarning("PUT");
			TransportResponse response;
			try {
				UnityWebRequest putRequest;
				if (!string.IsNullOrEmpty(transportRequest.BodyContentString)) {
					putRequest = UnityWebRequest.Put(transportRequest.RequestUrl, transportRequest.BodyContentString);
				} else if (transportRequest.BodyContentBytes != null) {
					putRequest = UnityWebRequest.Put(transportRequest.RequestUrl, transportRequest.BodyContentBytes);
				} else {
					throw new ArgumentException("PUT Transport Request has no body!");
				}

				PrepareUnityRequest(putRequest, transportRequest);
				var taskCompletionSource = new TaskCompletionSource<TransportResponse>();
				transportRequest.CancellationToken.Register(() => {
					putRequest.Abort();
					taskCompletionSource.TrySetCanceled();
				});
				putRequest.SendWebRequest().completed += _ => {
					taskCompletionSource.TrySetResult(UnityRequestToResponse(putRequest));
				};
				response = await taskCompletionSource.Task.ConfigureAwait(false);
			} catch (Exception ex) {
				Debug.LogError($"PUT Error: {ex}");
				response = new TransportResponse() {
					RequestUrl = transportRequest.RequestUrl,
					Error = ex
				};
			}

			return response;
		}
	}
}