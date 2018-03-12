using System;

namespace PubNubAPI
{
    public class PubNubException : System.Exception
    {
        public PubNubException() { }
        public PubNubException(string message) : base(message) { }
        public PubNubException(string message, System.Exception inner) : base(message, inner) { }
    }
}   