using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace PubNubUnity.Internal {
	public class PNDispatcher : MonoBehaviour {
    	static PNDispatcher instance;
    	static object lockObject;
    
    	static volatile Queue<System.Action> dispatchQueue = new Queue<System.Action>();
    
    	void FixedUpdate() {
    		HandleDispatch();
    	}
    
    	static void HandleDispatch() {
    		lock (lockObject) {
    			var c = dispatchQueue.Count;
    			for (int i = 0; i < c; i++) {
    				try {
    					dispatchQueue.Dequeue()();
    				} catch (System.Exception e) {
	                    // TODO investigate if we need more error handling mechanisms
    					Debug.LogError($"{e.Message} ::\n{e.StackTrace}");
    				}
    			}
    		}
    	}
    
        /// <summary>
        /// Dispatches the callback to Unity's main thread. Facilitates working on Unity's objects within the callback.
        /// </summary>
        /// <param name="action">Action to dispatch to main thread</param>
    	public static void Dispatch(System.Action action) {
    		if (action is null) {
	            return;
    		}
    
    		lock (lockObject) {
    			dispatchQueue.Enqueue(action);
    		}
    	}
    
        /// <summary>
        /// Dispatch an async operation's result to the main thread. Facilitates working on Unity's objects within the callback.
        /// </summary>
        /// <param name="task">Async task to dispatch</param>
        /// <param name="callback">Callback function which accepts task result as the argument</param>
        /// <typeparam name="T">Task return type</typeparam>
    	public static async void DispatchTask<T>(Task<T> task, System.Action<T> callback) {
    		if (callback is null) {
    			return;
    		}
    
    		T res;
    		if (task.IsCompleted) {
    			res = task.Result;
    		} else { 
    			res = await task;
    		}
    			
    		Dispatch(() => callback(res));
    	}
    
    	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    	static void Initialize() {
    		if (!instance) {
    			instance = new GameObject("[PubNub Dispatcher]").AddComponent<PNDispatcher>();
    		}
    		instance.gameObject.hideFlags = HideFlags.NotEditable | HideFlags.DontSave;
    		instance.transform.hideFlags = HideFlags.HideInInspector;
            
            // For future potential edit-mode dispatching 
    		if (Application.isPlaying) {
    			DontDestroyOnLoad(instance.gameObject);
    		}
            
    		lockObject ??= new object();
        }
    }
}
