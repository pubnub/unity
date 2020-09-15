using System;

namespace PubNubAPI
{
    public enum PNObjectsEvent
    {
        // PNObjectsEventSet is the enum when the event `set` occurs
        PNObjectsEventSet,
        // PNObjectsEventDelete is the enum when the event `remove` occurs
        PNObjectsEventDelete,

        PNObjectsNoneEvent,
    }

    public static class PNObjectsEventExtensions
    {
        public static string GetDescription(this PNObjectsEvent @this)
        {
            if (@this.Equals(PNObjectsEvent.PNObjectsEventSet)){
                return "set";
            } else if (@this.Equals(PNObjectsEvent.PNObjectsEventDelete)){
                return "delete";
            } else {
                return "";  
            }
        }
    }

    public enum PNObjectsEventType
    {
        // PNObjectsUUIDEvent is the enum when the event of type `uuid` occurs
        PNObjectsUUIDEvent,
        // PNObjectsChannelEvent is the enum when the event of type `channel` occurs
        PNObjectsChannelEvent,
        // PNObjectsMembershipEvent is the enum when the event of type `membership` occurs
        PNObjectsMembershipEvent,
        // PNObjectsNoneEvent is used for error handling
        PNObjectsNoneEvent

    }

    public static class PNObjectsEventTypeExtensions
    {
        public static string GetDescription(this PNObjectsEventType @this)
        {
            if (@this.Equals(PNObjectsEventType.PNObjectsUUIDEvent)){
                return "uuid";
            } else if (@this.Equals(PNObjectsEventType.PNObjectsChannelEvent)){
                return "channel";
            } else if (@this.Equals(PNObjectsEventType.PNObjectsMembershipEvent)){
                return "membership";
            } else {
                return "none";  
            }
        }
    }

    // PNChannelMetadataInclude is used as an enum to catgorize the available Channel Metadata include types
    public enum PNChannelMetadataInclude
    {
        // PNChannelMetadataIncludeCustom is the enum equivalent to the value `custom` available Channel Metadata include types
        PNChannelMetadataIncludeCustom
    }

    // PNUUIDMetadataInclude is used as an enum to catgorize the available UUID include types
    public enum PNUUIDMetadataInclude
    {
        // PNUUIDMetadataIncludeCustom is the enum equivalent to the value `custom` available UUID include types
        PNUUIDMetadataIncludeCustom
    }

    public static class PNChannelMetadataIncludeExtensions
    {
        public static string GetDescription(this PNChannelMetadataInclude @this)
        {
            if (@this.Equals(PNChannelMetadataInclude.PNChannelMetadataIncludeCustom)){
                return "custom";
            } else {
                return "custom";  
            }
        }
    }

    public static class PNUUIDMetadataIncludeExtensions
    {
        public static string GetDescription(this PNUUIDMetadataInclude @this)
        {
            if (@this.Equals(PNUUIDMetadataInclude.PNUUIDMetadataIncludeCustom)){
                return "custom";
            } else {
                return "custom";  
            }
        }
    }  

    // PNMembershipsInclude is used as an enum to catgorize the available Memberships include types
    public enum PNMembershipsInclude
    {
        // PNMembershipsIncludeCustom is the enum equivalent to the value `custom` available Memberships include types
        PNMembershipsIncludeCustom,
        // PNMembershipsIncludeChannel is the enum equivalent to the value `channel` available Memberships include types
        PNMembershipsIncludeChannel,
        // PNMembershipsIncludeChannelCustom is the enum equivalent to the value `channel.custom` available Memberships include types
        PNMembershipsIncludeChannelCustom

    }

    public static class PNMembershipsIncludeExtensions
    {
        public static string GetDescription(this PNMembershipsInclude @this)
        {
            if (@this.Equals(PNMembershipsInclude.PNMembershipsIncludeCustom)){
                return "custom";
            } else if (@this.Equals(PNMembershipsInclude.PNMembershipsIncludeChannel)){
                return "channel";
            } else if (@this.Equals(PNMembershipsInclude.PNMembershipsIncludeChannelCustom)){
                return "channel.custom";
            } else {
                return "custom";  
            }
        }
    }


    // PNChannelMembersInclude is used as an enum to catgorize the available Members include types
    public enum PNChannelMembersInclude
    {
        // PNChannelMembersIncludeCustom is the enum equivalent to the value `custom` available Members include types
        PNChannelMembersIncludeCustom,
        // PNChannelMembersIncludeUUID is the enum equivalent to the value `uuid` available Members include types
        PNChannelMembersIncludeUUID,
        // PNChannelMembersIncludeUUIDCustom is the enum equivalent to the value `uuid.custom` available Members include types
        PNChannelMembersIncludeUUIDCustom,

    }

    public static class PNChannelMembersIncludeExtensions
    {
        public static string GetDescription(this PNChannelMembersInclude @this)
        {
            if (@this.Equals(PNChannelMembersInclude.PNChannelMembersIncludeCustom)){
                return "custom";
            } else if (@this.Equals(PNChannelMembersInclude.PNChannelMembersIncludeUUID)){
                return "uuid";
            } else if (@this.Equals(PNChannelMembersInclude.PNChannelMembersIncludeUUIDCustom)){
                return "uuid.custom";
            } else {
                return "custom";  
            }
        }
    }    
}