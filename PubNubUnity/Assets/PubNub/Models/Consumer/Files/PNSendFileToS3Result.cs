using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class PNSendFileToS3Result
    {
        public bool IsNetworkError {get; set;}
        public bool IsHttpError {get; set;}
        public string Text {get; set;}
    }
}