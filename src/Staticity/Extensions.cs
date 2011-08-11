namespace Staticity {
	using System;
	using System.Collections.Generic;

	public static class Extensions {
		 public static TValue TryGetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key) {
			 TValue val;
			 if(dict.TryGetValue(key, out val)) {
				 return val;
			 }
			 return default(TValue);
		 }

		public static TValue TryGetValueAndRemove<TValue>(this Dictionary<string, object> dict, string key) {
			object val;

			if(dict.TryGetValue(key, out val)) {
				dict.Remove(key);
				if(val is TValue) {
					return (TValue) val;
				}
			}

			return default(TValue);
		}

		public static void WhenItIs<T>(this object obj, Action<T> action) {
			if(obj is T) {
				action((T) obj);
			}
		}
	}
}