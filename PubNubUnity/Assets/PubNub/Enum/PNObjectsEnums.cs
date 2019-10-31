using System;

namespace PubNubAPI
{
    // PNUserSpaceInclude is used as an enum to catgorize the available User and Space include types
    public enum PNUserSpaceInclude 
    {
        // PNUserSpaceCustom is the enum equivalent to the value `custom` available User and Space include types
        PNUserSpaceCustom
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
}