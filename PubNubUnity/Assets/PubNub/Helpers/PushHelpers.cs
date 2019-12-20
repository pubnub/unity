using System;
using System.Text;


namespace PubNubAPI
{
    public static class PushHelpers
    {
        public static void SetTopic (string topic, ref StringBuilder parameterBuilder)
        {
            if(!string.IsNullOrEmpty(topic)){
                parameterBuilder.AppendFormat("&topic={0}", topic);
            }
        }

        public static void SetEnvironment (PNPushEnvironment env, ref StringBuilder parameterBuilder)
        {
            if(env.Equals(PNPushEnvironment.Production)){
                parameterBuilder.AppendFormat("&environment={0}", "production");
            } else {
                parameterBuilder.AppendFormat("&environment={0}", "development");
            }
        }
    }
}