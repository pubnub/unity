using System;

namespace PubNubAPI
{
    public class ChannelEntity
    {
        public ChannelIdentity ChannelID;
        public ChannelParameters ChannelParams;
        public ChannelEntity(ChannelIdentity channelID, ChannelParameters channelParams){
            this.ChannelID = channelID;
            this.ChannelParams = channelParams;
        }
    }
}

