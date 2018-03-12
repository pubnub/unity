using System;

namespace PubNubAPI
{
    internal class QueueStorage
    {
        internal object Callback { get; set;}
        internal PNOperationType OperationType { get; set;} 
        internal object OperationParams { get; set;}
        internal PubNubUnity PubNubInstance { get; set;}

        public QueueStorage (object callback, PNOperationType operationType, object operationParams, PubNubUnity pn)
        {
            this.Callback = callback;
            this.OperationType = operationType;
            this.OperationParams = operationParams;
            this.PubNubInstance = pn;

        }
    }
}

