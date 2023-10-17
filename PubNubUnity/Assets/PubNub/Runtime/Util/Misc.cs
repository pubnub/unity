using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace PubnubApi.Unity {
	public static class Misc {
		public static T Hydrate<T>(this Dictionary<string, object> dict, T targetObject = null) where T : class {
			return (T)dict.Hydrate(typeof(T), targetObject);
		}

		public static object Hydrate(this Dictionary<string, object> dict, Type type, object targetObject = null) {
			var result = targetObject ?? FormatterServices.GetUninitializedObject(type);

			foreach (string key in dict.Keys) {
				MemberInfo propInfo = type.GetField(key) as MemberInfo ?? type.GetProperty(key) as MemberInfo;

				if (propInfo == null) continue;
				if (dict[key] is Dictionary<string, object> nestedDict) {
					propInfo.SetValue(result, nestedDict.Hydrate(type: propInfo.GetMemberType()));
				} else {
					propInfo.SetValue(result, dict[key]);
				}
			}

			return result;
		}

		static void SetValue(this MemberInfo mi, object target, object value) {
			switch (mi.MemberType) {
				case MemberTypes.Field:
					((FieldInfo)mi).SetValue(target, Convert.ChangeType(value, mi.GetMemberType()));
					break;
				case MemberTypes.Property:
					((PropertyInfo)mi).SetValue(target, Convert.ChangeType(value, mi.GetMemberType()), null);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		static System.Type GetMemberType(this MemberInfo member) {
			return member.MemberType switch {
				MemberTypes.Field => ((FieldInfo)member).FieldType,
				MemberTypes.Property => ((PropertyInfo)member).PropertyType,
				_ => throw new ArgumentOutOfRangeException()
			};
		}
	}
}