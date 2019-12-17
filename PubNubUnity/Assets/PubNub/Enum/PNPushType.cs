using System;
using System.Runtime.Serialization;

namespace PubNubAPI
{
    public enum PNPushType
    {
        None = 0,
        GCM,
        APNS,
        MPNS,
        APNS2
    }

    public enum PNPushEnvironment
    {
        //PNPushEnvironmentDevelopment for production
        [EnumMember(Value = "production")]
        Production,
        //PNPushEnvironmentDevelopment for development
        [EnumMember(Value = "development")]
        Development,
        //PNPushEnvironmentNone is for backward compatibility
        None
    }

    public static class PNPushEnvironmentExtensions
    {
        public static string GetDescription(this PNPushEnvironment @this)
        {
            if (@this.Equals(PNPushEnvironment.Production)){
                return "production";
            } else if (@this.Equals(PNPushEnvironment.Development)){
                return "development";  
            } else {
                return "none";                 
            }
        }
    }
}