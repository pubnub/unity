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
				request.timeout = (int)transportRequest.Timeout.Value.TotalSeconds;
			}

			if (transportRequest.Headers.Keys.Count > 0) {
				foreach (var kvp in transportRequest.Headers) {
					request.SetRequestHeader(kvp.Key, kvp.Value);
				}
			}
		}

		public async Task<TransportResponse> DeleteRequest(TransportRequest transportRequest) {
			Debug.LogError("DELETE");
			TransportResponse response;
			try {
				var deleteRequest = UnityWebRequest.Delete(transportRequest.RequestUrl);
				PrepareUnityRequest(deleteRequest, transportRequest);
				var taskCompletionSource = new TaskCompletionSource<TransportResponse>();
				deleteRequest.SendWebRequest().completed += _ => {
					//will raise an exception if cancellation token is triggered
					//but shouldn't because we also Abort (probably?)
					taskCompletionSource.SetResult(UnityRequestToResponse(deleteRequest));
				};
				await using (transportRequest.CancellationToken.Register(() => {
					             deleteRequest.Abort();
					             taskCompletionSource.TrySetCanceled();
				             })) {
					response = await taskCompletionSource.Task;
				}
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
			Debug.LogError("GET");
			TransportResponse response;
			try {
				var getRequest = UnityWebRequest.Get(transportRequest.RequestUrl);
				PrepareUnityRequest(getRequest, transportRequest);
				Debug.LogError("PREPARED");
				var taskCompletionSource = new TaskCompletionSource<TransportResponse>();
				getRequest.SendWebRequest().completed += _ => {
					//will raise an exception if cancellation token is triggered
					//but shouldn't because we also Abort (probably?)
					Debug.LogError("COMPLETED");
					taskCompletionSource.SetResult(UnityRequestToResponse(getRequest));
					Debug.LogError("RESULT SET");
				};
				await using (transportRequest.CancellationToken.Register(() => {
					             getRequest.Abort();
					             taskCompletionSource.TrySetCanceled();
				             })) {
					Debug.LogError("GET PRE AWAIT");
					response = await taskCompletionSource.Task;
					Debug.LogError("GET POST AWAIT");
				}
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
			Debug.LogError("POST");
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
				postRequest.SendWebRequest().completed += _ => {
					//will raise an exception if cancellation token is triggered
					//but shouldn't because we also Abort (probably?)
					taskCompletionSource.SetResult(UnityRequestToResponse(postRequest));
				};
				await using (transportRequest.CancellationToken.Register(() => {
					             postRequest.Abort();
					             taskCompletionSource.TrySetCanceled();
				             })) {
					response = await taskCompletionSource.Task;
				}
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
			Debug.LogError("PATCH");
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
				patchRequest.SendWebRequest().completed += _ => {
					//will raise an exception if cancellation token is triggered
					//but shouldn't because we also Abort (probably?)
					taskCompletionSource.SetResult(UnityRequestToResponse(patchRequest));
				};
				await using (transportRequest.CancellationToken.Register(() => {
					             patchRequest.Abort();
					             taskCompletionSource.TrySetCanceled();
				             })) {
					response = await taskCompletionSource.Task;
				}
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
			Debug.LogError("PUT");
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
				putRequest.SendWebRequest().completed += _ => {
					//will raise an exception if cancellation token is triggered
					//but shouldn't because we also Abort (probably?)
					taskCompletionSource.SetResult(UnityRequestToResponse(putRequest));
				};
				await using (transportRequest.CancellationToken.Register(() => {
					             putRequest.Abort();
					             taskCompletionSource.TrySetCanceled();
				             })) {
					response = await taskCompletionSource.Task;
				}
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