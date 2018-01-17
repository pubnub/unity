using System;

namespace PubNubAPI
{
    public class ChannelEntity
    {
        public ChannelIdentity ChannelID {get; set;}
        public ChannelParameters ChannelParams {get; set;}
        public ChannelEntity(ChannelIdentity channelID, ChannelParameters channelParams){
            this.ChannelID = channelID;
            this.ChannelParams = channelParams;
        }
    }
}

