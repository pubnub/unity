using System;

namespace PubNubAPI
{
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