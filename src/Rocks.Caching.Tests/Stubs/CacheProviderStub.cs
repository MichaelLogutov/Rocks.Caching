using System.Collections.Generic;

// ReSharper disable MemberCanBePrivate.Global
namespace Rocks.Caching.Tests.Stubs
{
	internal class CacheProviderStub : ICacheProvider
	{
		public class CacheItem
		{
			public object Value { get; set; }
			public CachingParameters Parameters { get; set; }
		}


		public readonly Dictionary<string, CacheItem> Values = new Dictionary<string, CacheItem> ();


		public object Get (string key)
		{
			CacheItem res;
			if (!this.Values.TryGetValue (key, out res))
				return null;

			return res.Value;
		}


		public void Add (string key, object value, CachingParameters parameters)
		{
			if (value == null)
			{
				this.Remove (key);
				return;
			}

			this.Values[key] = new CacheItem
			{
				Value = value,
				Parameters = parameters
			};
		}


		public void Clear ()
		{
			this.Values.Clear ();
		}


		public void Remove (string key)
		{
			this.Values.Remove (key);
		}
	}
}
