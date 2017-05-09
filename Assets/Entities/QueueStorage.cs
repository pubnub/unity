using System;

namespace PubNubAPI
{
    internal class QueueStorage
    {
        internal object Callback { get; set;}
        internal PNOperationType OperationType { get; set;} 
        internal OperationParams OperationParams { get; set;}

        public QueueStorage (object callback, PNOperationType operationType, OperationParams operationParams)
        {
            this.Callback = callback;
            this.OperationType = operationType;
            this.OperationParams = operationParams;
        }
    }
}

