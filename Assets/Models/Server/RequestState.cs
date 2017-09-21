using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace PubNubAPI
{
    public class RequestState
    {
        //public Action<T> SuccessCallback;
        //public Action<PubnubClientError> ErrorCallback;
        /*public PubnubWebRequest Request;
        public PubnubWebResponse Response;*/
        public PNOperationType RespType;

        internal long StartRequestTicks;
        internal long EndRequestTicks;

        public long ResponseCode;
        public string URL;

        //public string WebRequestId;
        public string WebRequestId;
        public HTTPMethod httpMethod;

        public string POSTData = "";

        public int Timeout;
        public int Pause;
        public bool Reconnect;
        /*public List<ChannelEntity> ChannelEntities;
        public bool Timeout;
        public bool Reconnect;
        public long Timetoken;
        public Type TypeParameterType;
        public long ID;
        public string UUID;*/

        public RequestState ()
        {
            StartRequestTicks = 0;
            EndRequestTicks = 0;
            URL = "";
            httpMethod = HTTPMethod.Get;
            POSTData = "";
            Timeout = 0;
            Pause = 0;
            Reconnect = false;
            /*SuccessCallback = null;
            Request = null;
            Response = null;
            ChannelEntities = null;
            ID = 0;*/
        }

        /*public RequestState (RequestState<T> requestState)
        {
            //ErrorCallback = requestState.ErrorCallback;
            ChannelEntities = requestState.ChannelEntities;
            Reconnect = requestState.Reconnect;
            Request = requestState.Request;
            Response = requestState.Response;
            Timeout = requestState.Timeout;
            Timetoken = requestState.Timetoken;
            TypeParameterType = requestState.TypeParameterType;
            SuccessCallback = requestState.SuccessCallback as Action<T>;
            ID = requestState.ID;
            RespType = requestState.RespType;
        }*/
    }
}