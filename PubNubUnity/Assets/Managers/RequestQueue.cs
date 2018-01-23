using System;
using System.Collections;

namespace PubNubAPI
{
    public sealed class RequestQueue
    {        
        //TODO handle disconenction
        //TODO max size

        private RequestQueue ()
        {
        }
        private static volatile RequestQueue instance;
        private static object syncRoot = new System.Object();
        private readonly Queue q = new Queue();

        public int QueueCount {
            get;
            private set;
        }

        public bool HasItems {get; private set;}

        public static RequestQueue Instance
        {
            get 
            {
                if (instance == null) 
                {
                    lock (syncRoot) 
                    {
                        if (instance == null){
                            instance = new RequestQueue();
                        }
                    }
                }

                return instance;
            }
        }

        public void Enqueue(object callback, PNOperationType operationType, object operationParams, PubNubUnity pn){
            #if (ENABLE_PUBNUB_LOGGING)
            pn.PNLog.WriteToLog(string.Format("Queuing {0}", operationType), PNLoggingMethod.LevelInfo);
            #endif
            QueueStorage qs = new QueueStorage(callback, operationType, (object)operationParams, pn);
            q.Enqueue(qs);
            Reset ();
        }

        internal QueueStorage Dequeue(){
            object o = q.Dequeue ();
            QueueStorage qs = o as QueueStorage;
            Reset ();
            return qs;
        }

        public void Reset(){
            if (q.Count > 0) {
                HasItems = true;
            } else {
                HasItems = false;
            }
        }
    }
}

