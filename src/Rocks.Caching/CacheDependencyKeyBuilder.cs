using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Rocks.Caching
{
	public static class CacheDependencyKeyBuilder
	{
		/// <summary>
		/// A prefix that will be added before each dependency root key.
		/// </summary>
		public const string DependencyRootCacheKeyPrefix = "DependencyRoot:";


		/// <summary>
		/// Creates a dependency root string key.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="value">Value (can be null).</param>
		/// <exception cref="ArgumentNullException"><paramref name="key" /> is <c>null</c>.</exception>
		[DebuggerStepThrough]
		public static string Create (object key, object value)
		{
			if (key == null)
				throw new ArgumentNullException ("key");

			var key_str = DependencyRootCacheKeyPrefix;

			{
				var key_as_str = key as string;
				if (key_as_str == null)
				{
					var key_enum = key as IEnumerable;
					if (key_enum != null)
					{
						var sbev = new StringBuilder (1000);

						foreach (var ev in key_enum)
						{
							sbev.Append (':');

							var v_custom = ev as ICacheKeyProvider;
							if (v_custom != null)
								sbev.Append (v_custom.GetCacheKey ());
							else
								sbev.Append (Convert.ToString (ev, CultureInfo.InvariantCulture));
						}

						sbev.Append (':');
						key_str += sbev.ToString ();
					}
					else
						key_str += Convert.ToString (key, CultureInfo.InvariantCulture);
				}
				else
					key_str += key_as_str;
			}

			var sb = new StringBuilder (1000);

			var append = new Action<object> (v =>
			{
				sb.Append ('{');
				sb.Append (key_str);

				if (v != null)
				{
					sb.Append (':');

					var v_custom = v as ICacheKeyProvider;
					if (v_custom != null)
						sb.Append (v_custom.GetCacheKey ());
					else
						sb.Append (Convert.ToString (v, CultureInfo.InvariantCulture));
				}

				sb.Append ('}');
			});

			var value_enum = value as IEnumerable;
			if (!(value is string) && value_enum != null)
			{
				foreach (var ev in value_enum)
					append (ev);
			}
			else
			{
				append (value);
			}

			return sb.ToString ();
		}


		/// <summary>
		/// Creates a dependency root keys list. Enumeration values will be added as separate key for each item.
		/// </summary>
		/// <param name="args">A name-value list of arguments (null value pairs will be added too; enumerations will be added as separate key for each item).</param>
		[DebuggerStepThrough]
		public static string[] CreateMany (params object[] args)
		{
			var res = new List<string> ();

			if (args != null)
			{
				for (var k = 0; k < args.Length; k += 2)
				{
					var key = args[k];
					if (key == null)
						continue;

					var value = k + 1 < args.Length ? args[k + 1] : null;

					if (!(value is string))
					{
						var e = value as IEnumerable;
						if (e != null)
						{
							res.AddRange (from object ev in e select Create (key, ev));
							continue;
						}
					}

					res.Add (Create (key, value));
				}
			}

			return res.ToArray ();
		}
	}
}
