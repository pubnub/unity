using System;

namespace PubNubAPI
{
    public enum PNReconnectionPolicy
    {
        NONE, // Set Max reconnects to 0
        LINEAR,
        EXPONENTIAL
    }
}