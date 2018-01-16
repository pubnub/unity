using System;
using UnityEngine;
using UnityEngine.Networking;

namespace PubNubAPI
{
    internal class UnityWebRequestWrapper
    {  
        internal UnityWebRequest CurrentUnityWebRequest;
        internal string URL;
        internal int Timeout;
        internal int Pause;
        internal PNCurrentRequestType CurrentRequestType;
        internal bool IsComplete;
        internal RequestState CurrentRequestState;
        internal bool RunTimer = false;
        internal float Timer = 0;
        internal bool RunPauseTimer = false;
        internal float PauseTimer = 0;
        internal UnityWebRequestWrapper(PNCurrentRequestType crt, RequestState requestState){
            this.CurrentUnityWebRequest = null;
            this.URL = requestState.URL;
            this.Timeout = requestState.Timeout;
            this.Pause = requestState.Pause;
            this.CurrentRequestType = crt;
            this.CurrentRequestState = requestState;
            this.IsComplete = false;
            this.Timer = requestState.Timeout;
        }
    }
}