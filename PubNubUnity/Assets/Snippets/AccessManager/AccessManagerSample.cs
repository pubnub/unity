// snippet.using
using PubnubApi;
using PubnubApi.Unity;

// snippet.end
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;

class AccessManagerSample
{
    private static Pubnub pubnub;

    static void PubnubInit()
    {
        // snippet.pubnub_init
        //Create configuration
        PNConfiguration pnConfiguration = new PNConfiguration(new UserId("myUniqueUserId"))
        {
	        SubscribeKey = "YOUR_SUBSCRIBE_KEY",
	        PublishKey = "YOUR_PUBLISH_KEY",
	        SecretKey = "YOUR_SECRET_KEY",
        };
        //Create a new PubNub instance
        Pubnub pubnub = PubnubUnityUtils.NewUnityPubnub(pnConfiguration);

        // If you're using Unity Editor setup you can get the Pubnub instance from PNManagerBehaviour
        // For more details, see https://www.pubnub.com/docs/sdks/unity#configure-pubnub
        /*
        [SerializeField] private PNManagerBehaviour pubnubManager;
        Pubnub pubnub = pubnubManager.pubnub;
        */

        // snippet.end
    }

    static async Task GrantTokenComplex()
    {
        // snippet.grant_token_complex
        PNResult<PNAccessManagerTokenResult> grantTokenResponse = await pubnub.GrantToken()
            .TTL(15)
            .AuthorizedUuid("my-authorized-uuid")
            .Resources(new PNTokenResources()
            {
                Channels = new Dictionary<string, PNTokenAuthValues>() {
                    { "channel-a", new PNTokenAuthValues() { Read = true } },
                    { "channel-b", new PNTokenAuthValues() { Read = true, Write = true } },
                    { "channel-c", new PNTokenAuthValues() { Read = true, Write = true } },
                    { "channel-d", new PNTokenAuthValues() { Read = true, Write = true } }},
                ChannelGroups = new Dictionary<string, PNTokenAuthValues>() {
                    { "channel-group-b", new PNTokenAuthValues() { Read = true } } },
                Uuids = new Dictionary<string, PNTokenAuthValues>() {
                    { "uuid-c", new PNTokenAuthValues() { Get = true } },
                    { "uuid-d", new PNTokenAuthValues() { Get = true, Update = true } }}
            })
            .ExecuteAsync();
        PNAccessManagerTokenResult grantTokenResult = grantTokenResponse.Result;
        PNStatus grantTokenStatus = grantTokenResponse.Status;
        if (!grantTokenStatus.Error && grantTokenResult != null)
        {
            Debug.Log(pubnub.JsonPluggableLibrary.SerializeToJsonString(grantTokenResult));
        }
        else
        {
            Debug.Log(pubnub.JsonPluggableLibrary.SerializeToJsonString(grantTokenStatus));
        }
        // snippet.end
    }

    static async Task GrantTokenWithRegex()
    {
        // snippet.grant_token_regex
        PNResult<PNAccessManagerTokenResult> grantTokenResponse = await pubnub.GrantToken()
            .TTL(15)
            .AuthorizedUuid("my-authorized-uuid")
            .Patterns(new PNTokenPatterns()
            {
                Channels = new Dictionary<string, PNTokenAuthValues>() {
                    { "channel-[A-Za-z0-9]", new PNTokenAuthValues() { Read = true } }}
            })
            .ExecuteAsync();
        PNAccessManagerTokenResult grantTokenResult = grantTokenResponse.Result;
        PNStatus grantTokenStatus = grantTokenResponse.Status;
        if (!grantTokenStatus.Error && grantTokenResult != null)
        {
            Debug.Log(pubnub.JsonPluggableLibrary.SerializeToJsonString(grantTokenResult));
        }
        else
        {
            Debug.Log(pubnub.JsonPluggableLibrary.SerializeToJsonString(grantTokenStatus));
        }
        // snippet.end
    }

    static async Task GrantTokenComplexWithRegex()
    {
        // snippet.grant_token_complex_with_regex
        PNResult<PNAccessManagerTokenResult> grantTokenResponse = await pubnub.GrantToken()
            .TTL(15)
            .AuthorizedUuid("my-authorized-uuid")
            .Resources(new PNTokenResources()
            {
                Channels = new Dictionary<string, PNTokenAuthValues>() {
                    { "channel-a", new PNTokenAuthValues() { Read = true } },
                    { "channel-b", new PNTokenAuthValues() { Read = true, Write = true } },
                    { "channel-c", new PNTokenAuthValues() { Read = true, Write = true } },
                    { "channel-d", new PNTokenAuthValues() { Read = true, Write = true } }},
                ChannelGroups = new Dictionary<string, PNTokenAuthValues>() {
                    { "channel-group-b", new PNTokenAuthValues() { Read = true } } },
                Uuids = new Dictionary<string, PNTokenAuthValues>() {
                    { "uuid-c", new PNTokenAuthValues() { Get = true } },
                    { "uuid-d", new PNTokenAuthValues() { Get = true, Update = true } }}
            })
            .Patterns(new PNTokenPatterns()
            {
                Channels = new Dictionary<string, PNTokenAuthValues>() {
                    { "channel-[A-Za-z0-9]", new PNTokenAuthValues() { Read = true } }}
            })
            .ExecuteAsync();
        PNAccessManagerTokenResult grantTokenResult = grantTokenResponse.Result;
        PNStatus grantTokenStatus = grantTokenResponse.Status;
        if (!grantTokenStatus.Error && grantTokenResult != null)
        {
            Debug.Log(pubnub.JsonPluggableLibrary.SerializeToJsonString(grantTokenResult));
        }
        else
        {
            Debug.Log(pubnub.JsonPluggableLibrary.SerializeToJsonString(grantTokenStatus));
        }
        // snippet.end
    }

    static async Task GrantTokenOldBasicUsage()
    {
        // snippet.basic_usage_old
        PNResult<PNAccessManagerTokenResult> grantTokenResponse = await pubnub.GrantToken()
            .TTL(15)
            .AuthorizedUserId("my-authorized-userId")
            .Resources(new PNTokenResources()
            {
                Spaces = new Dictionary<string, PNTokenAuthValues>() {
                    { "my-space", new PNTokenAuthValues() { Read = true } } } // False to disallow
            })
            .ExecuteAsync();
        PNAccessManagerTokenResult grantTokenResult = grantTokenResponse.Result;
        PNStatus grantTokenStatus = grantTokenResponse.Status;
        //PNAccessManagerTokenResult is a parsed and abstracted response from the server
        if (!grantTokenStatus.Error && grantTokenResult != null)
        {
            Debug.Log(pubnub.JsonPluggableLibrary.SerializeToJsonString(grantTokenResult));
        }
        else
        {
            Debug.Log(pubnub.JsonPluggableLibrary.SerializeToJsonString(grantTokenStatus));
        }
        // snippet.end
    }

    static async Task GrantTokenOldComplex()
    {
        // snippet.grant_token_complex_old
        PNResult<PNAccessManagerTokenResult> grantTokenResponse = await pubnub.GrantToken()
            .TTL(15)
            .AuthorizedUserId("my-authorized-userId")
            .Resources(new PNTokenResources()
            {
                Spaces = new Dictionary<string, PNTokenAuthValues>() {
                    { "space-a", new PNTokenAuthValues() { Read = true } },
                    { "space-b", new PNTokenAuthValues() { Read = true, Write = true } },
                    { "space-c", new PNTokenAuthValues() { Read = true, Write = true } },
                    { "space-d", new PNTokenAuthValues() { Read = true, Write = true } }},
                Users = new Dictionary<string, PNTokenAuthValues>() {
                    { "user-c", new PNTokenAuthValues() { Get = true } },
                    { "user-d", new PNTokenAuthValues() { Get = true, Update = true } }}
            })
            .ExecuteAsync();
        PNAccessManagerTokenResult grantTokenResult = grantTokenResponse.Result;
        PNStatus grantTokenStatus = grantTokenResponse.Status;
        if (!grantTokenStatus.Error && grantTokenResult != null)
        {
            Debug.Log(pubnub.JsonPluggableLibrary.SerializeToJsonString(grantTokenResult));
        }
        else
        {
            Debug.Log(pubnub.JsonPluggableLibrary.SerializeToJsonString(grantTokenStatus));
        }
        // snippet.end
    }

    static async Task GrantTokenOldWithRegex()
    {
        // snippet.grant_token_old_regex
        PNResult<PNAccessManagerTokenResult> grantTokenResponse = await pubnub.GrantToken()
            .TTL(15)
            .AuthorizedUserId("my-authorized-userId")
            .Patterns(new PNTokenPatterns()
            {
                Spaces = new Dictionary<string, PNTokenAuthValues>() {
                    { "space-[A-Za-z0-9]", new PNTokenAuthValues() { Read = true } }}
            })
            .ExecuteAsync();
        PNAccessManagerTokenResult grantTokenResult = grantTokenResponse.Result;
        PNStatus grantTokenStatus = grantTokenResponse.Status;
        if (!grantTokenStatus.Error && grantTokenResult != null)
        {
            Debug.Log(pubnub.JsonPluggableLibrary.SerializeToJsonString(grantTokenResult));
        }
        else
        {
            Debug.Log(pubnub.JsonPluggableLibrary.SerializeToJsonString(grantTokenStatus));
        }
        // snippet.end
    }

    static async Task GrantTokenOldComplexWithRegex()
    {
        // snippet.grant_token_complex_old_with_regex
        PNResult<PNAccessManagerTokenResult> grantTokenResponse = await pubnub.GrantToken()
            .TTL(15)
            .AuthorizedUserId("my-authorized-userId")
            .Resources(new PNTokenResources()
            {
                Spaces = new Dictionary<string, PNTokenAuthValues>() {
                    { "space-a", new PNTokenAuthValues() { Read = true } },
                    { "space-b", new PNTokenAuthValues() { Read = true, Write = true } },
                    { "space-c", new PNTokenAuthValues() { Read = true, Write = true } },
                    { "space-d", new PNTokenAuthValues() { Read = true, Write = true } }},
                Users = new Dictionary<string, PNTokenAuthValues>() {
                    { "user-c", new PNTokenAuthValues() { Get = true } },
                    { "user-d", new PNTokenAuthValues() { Get = true, Update = true } }}
            })
            .Patterns(new PNTokenPatterns()
            {
                Spaces = new Dictionary<string, PNTokenAuthValues>() {
                    { "space-[A-Za-z0-9]", new PNTokenAuthValues() { Read = true } }}
            })
            .ExecuteAsync();
        PNAccessManagerTokenResult grantTokenResult = grantTokenResponse.Result;
        PNStatus grantTokenStatus = grantTokenResponse.Status;
        if (!grantTokenStatus.Error && grantTokenResult != null)
        {
            Debug.Log(pubnub.JsonPluggableLibrary.SerializeToJsonString(grantTokenResult));
        }
        else
        {
            Debug.Log(pubnub.JsonPluggableLibrary.SerializeToJsonString(grantTokenStatus));
        }
        // snippet.end
    }

    static async Task RevokeTokenBasicUsage()
    {
        // snippet.revoke_token
        PNResult<PNAccessManagerRevokeTokenResult> revokeTokenResponse = await pubnub
            .RevokeToken()
            .Token("p0thisAkFl043rhDdHRsCkNDcGF0pERjaGFuoENnctokenVzcqBDc3BjoERtZXRhoENzaWdYIGOAeTyWGJI")
            .ExecuteAsync();
        PNAccessManagerRevokeTokenResult revokeTokenResult = revokeTokenResponse.Result;
        PNStatus revokeTokenStatus = revokeTokenResponse.Status;
        if (!revokeTokenStatus.Error && revokeTokenResult != null)
        {
            Debug.Log("Revoke token success");
        }
        else
        {
            Debug.Log(pubnub.JsonPluggableLibrary.SerializeToJsonString(revokeTokenStatus));
        }
        // snippet.end
    }

    static async Task SetAuthTokenSnippet()
    {
        // snippet.set_token
        pubnub.SetAuthToken(
            "p0thisAkFl043rhDdHRsCkNyZXisRGNoYW6hanNlY3JldAFDZ3Jwsample3KgQ3NwY6BDcGF0pERjaGFuoENnctokenVzcqBDc3BjoERtZXRhoENzaWdYIGOAeTyWGJI");

        // snippet.end
    }

    static async Task ParseTokenBasicUsage()
    {
        // snippet.parse_token_usage
        var parsedTokenContent =
            pubnub.ParseToken(
                "p0thisAkFl043rhDdHRsCkNyZXisRGNoYW6hanNlY3JldAFDZ3Jwsample3KgQ3NwY6BDcGF0pERjaGFuoENnctokenVzcqBDc3BjoERtZXRhoENzaWdYIGOAeTyWGJI");
        var parsedTokenJson = pubnub.JsonPluggableLibrary.SerializeToJsonString(parsedTokenContent);
        // snippet.end

        /*
        // snippet.parse_token_result
        {
           "Version":2,
           "Timestamp":1619718521,
           "TTL":15,
           "AuthorizedUuid":"my_uuid",
           "Resources":{
              "Uuids":{
                "uuid-id":{
                    "Read":true,
                    "Write":true,
                    "Manage":true,
                    "Delete":true,
                    "Get":true,
                    "Update":true,
                    "Join":true
                }
              },
              "Channels":{
                "channel-id":{
                    "Read":true,
                    "Write":true,
                    "Manage":true,
                    "Delete":true,
                    "Get":true,
                    "Update":true,
                    "Join":true
                }
              },
              "ChannelGroups":{
                "group-id":{
                    "Read":true,
                    "Write":true,
                    "Manage":true,
                    "Delete":true,
                    "Get":true,
                    "Update":true,
                    "Join":true
                }
              }
           },
           "Patterns":{
              "Uuids":{
                "uuid-pattern":{
                    "Read":true,
                    "Write":true,
                    "Manage":true,
                    "Delete":true,
                    "Get":true,
                    "Update":true,
                    "Join":true
                }
              },
              "Channels":{
                "channel-pattern":{
                    "Read":true,
                    "Write":true,
                    "Manage":true,
                    "Delete":true,
                    "Get":true,
                    "Update":true,
                    "Join":true
                }
              },
              "ChannelGroups":{
                "group-pattern":{
                    "Read":true,
                    "Write":true,
                    "Manage":true,
                    "Delete":true,
                    "Get":true,
                    "Update":true,
                    "Join":true
                }
              }
           }
        }
        // snippet.end
        */
    }
}