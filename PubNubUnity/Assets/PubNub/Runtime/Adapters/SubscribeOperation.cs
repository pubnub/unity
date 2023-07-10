using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PubnubApi;
using PubnubApi.EndPoint;

namespace PubnubApi.Unity {
	public static class SubscribeOperationExtensions {
		public static SubscribeOperation<T> Channels<T>(this SubscribeOperation<T> so, List<string> channels) =>
			so.Channels(channels.ToArray());

		public static SubscribeOperation<string> Channels(this SubscribeOperation<string> so, List<string> channels) =>
			so.Channels<string>(channels);

		public static SubscribeOperation<T> ChannelGroups<T>(this SubscribeOperation<T> so, List<string> channels) =>
			so.ChannelGroups(channels.ToArray());

		public static SubscribeOperation<string> ChannelGroups(this SubscribeOperation<string> so, List<string> channels) =>
			so.ChannelGroups<string>(channels);


		public static SubscribeOperation<string> QueryParam(this SubscribeOperation<string> so,
			Dictionary<string, string> customQueryParam) => so.QueryParam(customQueryParam.ToDictionary(kvp => kvp.Key, kvp => kvp.Value as object));

		public static UnsubscribeOperation<T> ChannelGroups<T>(this UnsubscribeOperation<T> op, List<string> channelGroups) {
			return op.ChannelGroups(channelGroups.ToArray());
		}

		public static UnsubscribeOperation<T> Channels<T>(this UnsubscribeOperation<T> op, List<string> channels) {
			return op.ChannelGroups(channels.ToArray());
		}
	}
}