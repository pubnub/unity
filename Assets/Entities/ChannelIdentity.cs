using System;

namespace PubNubAPI
{
    public class ChannelIdentity
    {
        public string ChannelOrChannelGroupName {get; set;}
        public bool IsChannelGroup {get; set;}
        public bool IsPresenceChannel {get; set;}

        public ChannelIdentity(string channelOrChannelGroupName, bool isChannelGroup, bool isPresenceChannel){
            ChannelOrChannelGroupName = channelOrChannelGroupName;
            IsChannelGroup = isChannelGroup;
            IsPresenceChannel = isPresenceChannel;
        }
    }
}

