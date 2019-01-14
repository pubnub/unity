using System;

namespace PubNubAPI
{
    public class PubNubException : System.Exception
    {
        public PubNubException() { }
        public PubNubException(string message) : base(message) { }
        public PubNubException(string message, System.Exception inner) : base(message, inner) { }
    }
    public class PubNubUserException : System.Exception
    {
        public PubNubUserException() { }
        public PubNubUserException(string message) : base(message) { }
        public PubNubUserException(string message, System.Exception inner) : base(message, inner) { }
    }
}   