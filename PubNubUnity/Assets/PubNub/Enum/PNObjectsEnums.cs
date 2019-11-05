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
            switch (@this)
            {
                case PNObjectsEvent.PNObjectsEventCreate:
                    return "create";
                case PNObjectsEvent.PNObjectsEventUpdate:
                    return "update";
                case PNObjectsEvent.PNObjectsEventDelete:
                    return "delete";
                default:
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
            switch (@this)
            {
                case PNObjectsEventType.PNObjectsUserEvent:
                    return "user";
                case PNObjectsEventType.PNObjectsSpaceEvent:
                    return "space";
                case PNObjectsEventType.PNObjectsMembershipEvent:
                    return "membership";
                default:
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
            switch (@this)
            {
                case PNUserSpaceInclude.PNUserSpaceCustom:
                    return "custom";
                default:
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
            switch (@this)
            {
                case PNMembershipsInclude.PNMembershipsCustom:
                    return "custom";
                case PNMembershipsInclude.PNMembershipsSpace:
                    return "space";
                case PNMembershipsInclude.PNMembershipsSpaceCustom:
                    return "space.custom";
                default:
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
            switch (@this)
            {
                case PNMembersInclude.PNMembersCustom:
                    return "custom";
                case PNMembersInclude.PNMembersUser:
                    return "user";
                case PNMembersInclude.PNMembersUserCustom:
                    return "user.custom";
                default:
                    return "custom";    
            }
        }
    }    
}