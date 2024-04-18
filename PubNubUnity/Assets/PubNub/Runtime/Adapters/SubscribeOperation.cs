using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PubnubApi;
using PubnubApi.EndPoint;
using PubnubApi.Interface;

namespace PubnubApi.Unity {
	public static class SubscribeOperationExtensions {
		public static ISubscribeOperation<T> Channels<T>(this SubscribeOperation<T> so, List<string> channels) =>
			so.Channels(channels.ToArray());

		public static ISubscribeOperation<string> Channels(this SubscribeOperation<string> so, List<string> channels) =>
			so.Channels<string>(channels);

		public static ISubscribeOperation<T> ChannelGroups<T>(this SubscribeOperation<T> so, List<string> channels) =>
			so.ChannelGroups(channels.ToArray());

		public static ISubscribeOperation<string> ChannelGroups(this SubscribeOperation<string> so, List<string> channels) =>
			so.ChannelGroups<string>(channels);


		public static ISubscribeOperation<string> QueryParam(this SubscribeOperation<string> so,
			Dictionary<string, string> customQueryParam) => so.QueryParam(customQueryParam.ToDictionary(kvp => kvp.Key, kvp => kvp.Value as object));

		public static IUnsubscribeOperation<T> ChannelGroups<T>(this UnsubscribeOperation<T> op, List<string> channelGroups) =>
			op.ChannelGroups(channelGroups.ToArray());
		

		public static IUnsubscribeOperation<T> Channels<T>(this UnsubscribeOperation<T> op, List<string> channels) =>
			op.Channels(channels.ToArray());
	}
}