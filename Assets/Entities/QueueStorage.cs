using System;

namespace PubNubAPI
{
    internal class QueueStorage
    {
        internal object Callback { get; set;}
        internal PNOperationType OperationType { get; set;} 
        //internal OperationParams OperationParams { get; set;}
        internal object OperationParams { get; set;}
        internal PubNub PubNubInstance { get; set;}

        //public QueueStorage (object callback, PNOperationType operationType, OperationParams operationParams)
        public QueueStorage (object callback, PNOperationType operationType, object operationParams, PubNub pn)
        //public QueueStorage (object callback, PNOperationType operationType, PubNubBuilder<T> operationParams)
        {
            this.Callback = callback;
            this.OperationType = operationType;
            this.OperationParams = operationParams;
            this.PubNubInstance = pn;

        }
    }
}

