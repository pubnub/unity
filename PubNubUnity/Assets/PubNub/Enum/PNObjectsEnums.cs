using System;

namespace PubNubAPI
{
    public enum PNObjectsEvent
    {
        // PNObjectsEventCreate is the enum when the event `create` occurs
        PNObjectsEventCreate,
        // PNObjectsEventUpdate is the enum when the event `update` occurs
        PNObjectsEventUpdate,
        // PNObjectsEventDelete is the enum when the event `delete` occurs
        PNObjectsEventDelete,

        PNObjectsNoneEvent,
    }

    public static class PNObjectsEventExtensions
    {
        public static string GetDescription(this PNObjectsEvent @this)
        {
            if (@this.Equals(PNObjectsEvent.PNObjectsEventCreate)){
                return "create";
            } else if (@this.Equals(PNObjectsEvent.PNObjectsEventUpdate)){
                return "update";
            } else if (@this.Equals(PNObjectsEvent.PNObjectsEventDelete)){
                return "delete";
            } else {
                return "";  
            }
        }
    }

    public enum PNObjectsEventType
    {
        // PNObjectsUserEvent is the enum when the event of type `user` occurs
        PNObjectsUserEvent,
        // PNObjectsSpaceEvent is the enum when the event of type `space` occurs
        PNObjectsSpaceEvent,
        // PNObjectsMembershipEvent is the enum when the event of type `membership` occurs
        PNObjectsMembershipEvent,
        // PNObjectsNoneEvent is used for error handling
        PNObjectsNoneEvent

    }

    public static class PNObjectsEventTypeExtensions
    {
        public static string GetDescription(this PNObjectsEventType @this)
        {
            if (@this.Equals(PNObjectsEventType.PNObjectsUserEvent)){
                return "user";
            } else if (@this.Equals(PNObjectsEventType.PNObjectsSpaceEvent)){
                return "space";
            } else if (@this.Equals(PNObjectsEventType.PNObjectsMembershipEvent)){
                return "membership";
            } else {
                return "none";  
            }
        }
    }

    // PNUserSpaceInclude is used as an enum to catgorize the available User and Space include types
    public enum PNUserSpaceInclude 
    {
        // PNUserSpaceCustom is the enum equivalent to the value `custom` available User and Space include types
        PNUserSpaceCustom
    }

    public static class PNUserSpaceIncludeExtensions
    {
        public static string GetDescription(this PNUserSpaceInclude @this)
        {
            if (@this.Equals(PNUserSpaceInclude.PNUserSpaceCustom)){
                return "custom";
            } else {
                return "custom";  
            }
        }
    }

    // PNMembershipsInclude is used as an enum to catgorize the available Memberships include types
    public enum PNMembershipsInclude
    {
        // PNMembershipsCustom is the enum equivalent to the value `custom` available Memberships include types
        PNMembershipsCustom,
        // PNMembershipsSpace is the enum equivalent to the value `space` available Memberships include types
        PNMembershipsSpace,
        // PNMembershipsSpaceCustom is the enum equivalent to the value `space.custom` available Memberships include types
        PNMembershipsSpaceCustom

    }

    public static class PNMembershipsIncludeExtensions
    {
        public static string GetDescription(this PNMembershipsInclude @this)
        {
            if (@this.Equals(PNMembershipsInclude.PNMembershipsCustom)){
                return "custom";
            } else if (@this.Equals(PNMembershipsInclude.PNMembershipsSpace)){
                return "space";
            } else if (@this.Equals(PNMembershipsInclude.PNMembershipsSpaceCustom)){
                return "space.custom";
            } else {
                return "custom";  
            }
        }
    }


    // PNMembersInclude is used as an enum to catgorize the available Members include types
    public enum PNMembersInclude
    {
        // PNMembersCustom is the enum equivalent to the value `custom` available Members include types
        PNMembersCustom,
        // PNMembersUser is the enum equivalent to the value `user` available Members include types
        PNMembersUser,
        // PNMembersUserCustom is the enum equivalent to the value `user.custom` available Members include types
        PNMembersUserCustom,

    }

    public static class PNMembersIncludeExtensions
    {
        public static string GetDescription(this PNMembersInclude @this)
        {
            if (@this.Equals(PNMembersInclude.PNMembersCustom)){
                return "custom";
            } else if (@this.Equals(PNMembersInclude.PNMembersUser)){
                return "user";
            } else if (@this.Equals(PNMembersInclude.PNMembersUserCustom)){
                return "user.custom";
            } else {
                return "custom";  
            }
        }
    }    
}