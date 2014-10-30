using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using FastMember;

namespace Rocks.Caching
{
	public static class CacheKeyBuilder
	{
		#region Static methods

		/// <summary>
		///     Creates a cache key in "{args1}{args2}{args3} ... ect." format.
		/// </summary>
		/// <param name="args1">An argument (IEnumerable objects will be represented as concatinated string of it's items).</param>
		[DebuggerStepThrough, MethodImpl (MethodImplOptions.AggressiveInlining)]
		public static string Create (object args1)
		{
			if (args1 == null)
				return string.Empty;

			var sb = new StringBuilder ();

			sb.Append ('{');
			Append (sb, args1);
			sb.Append ('}');

			return sb.ToString ();
		}


		/// <summary>
		///     Creates a cache key in "{args1}{args2}{args3} ... ect." format.
		/// </summary>
		/// <param name="args1">An argument (IEnumerable objects will be represented as concatinated string of it's items).</param>
		/// <param name="args2">An argument (IEnumerable objects will be represented as concatinated string of it's items).</param>
		[DebuggerStepThrough, MethodImpl (MethodImplOptions.AggressiveInlining)]
		public static string Create (object args1, object args2)
		{
			var sb = new StringBuilder ();

			sb.Append ('{');
			Append (sb, args1);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args2);
			sb.Append ('}');

			return sb.ToString ();
		}


		/// <summary>
		///     Creates a cache key in "{args1}{args2}{args3} ... ect." format.
		/// </summary>
		/// <param name="args1">An argument (IEnumerable objects will be represented as concatinated string of it's items).</param>
		/// <param name="args2">An argument (IEnumerable objects will be represented as concatinated string of it's items).</param>
		/// <param name="args3">An argument (IEnumerable objects will be represented as concatinated string of it's items).</param>
		[DebuggerStepThrough, MethodImpl (MethodImplOptions.AggressiveInlining)]
		public static string Create (object args1, object args2, object args3)
		{
			var sb = new StringBuilder ();

			sb.Append ('{');
			Append (sb, args1);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args2);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args3);
			sb.Append ('}');

			return sb.ToString ();
		}


		/// <summary>
		///     Creates a cache key in "{args1}{args2}{args3} ... ect." format.
		/// </summary>
		[DebuggerStepThrough, MethodImpl (MethodImplOptions.AggressiveInlining)]
		public static string Create (object args1, object args2, object args3, object args4)
		{
			var sb = new StringBuilder ();

			sb.Append ('{');
			Append (sb, args1);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args2);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args3);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args4);
			sb.Append ('}');

			return sb.ToString ();
		}


		/// <summary>
		///     Creates a cache key in "{args1}{args2}{args3} ... ect." format.
		/// </summary>
		[DebuggerStepThrough, MethodImpl (MethodImplOptions.AggressiveInlining)]
		public static string Create (object args1, object args2, object args3, object args4, object args5)
		{
			var sb = new StringBuilder ();

			sb.Append ('{');
			Append (sb, args1);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args2);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args3);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args4);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args5);
			sb.Append ('}');

			return sb.ToString ();
		}



		/// <summary>
		///     Creates a cache key in "{args1}{args2}{args3} ... ect." format.
		/// </summary>
		[DebuggerStepThrough, MethodImpl (MethodImplOptions.AggressiveInlining)]
		public static string Create (object args1, object args2, object args3, object args4, object args5, object args6)
		{
			var sb = new StringBuilder ();

			sb.Append ('{');
			Append (sb, args1);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args2);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args3);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args4);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args5);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args6);
			sb.Append ('}');

			return sb.ToString ();
		}



		/// <summary>
		///     Creates a cache key in "{args1}{args2}{args3} ... ect." format.
		/// </summary>
		[DebuggerStepThrough, MethodImpl (MethodImplOptions.AggressiveInlining)]
		public static string Create (object args1, object args2, object args3, object args4, object args5, object args6, object args7)
		{
			var sb = new StringBuilder ();

			sb.Append ('{');
			Append (sb, args1);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args2);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args3);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args4);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args5);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args6);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args7);
			sb.Append ('}');

			return sb.ToString ();
		}



		/// <summary>
		///     Creates a cache key in "{args1}{args2}{args3} ... ect." format.
		/// </summary>
		[DebuggerStepThrough, MethodImpl (MethodImplOptions.AggressiveInlining)]
		public static string Create (object args1, object args2, object args3, object args4, object args5, object args6, object args7, object args8)
		{
			var sb = new StringBuilder ();

			sb.Append ('{');
			Append (sb, args1);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args2);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args3);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args4);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args5);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args6);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args7);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args8);
			sb.Append ('}');

			return sb.ToString ();
		}



		/// <summary>
		///     Creates a cache key in "{args1}{args2}{args3} ... ect." format.
		/// </summary>
		[DebuggerStepThrough, MethodImpl (MethodImplOptions.AggressiveInlining)]
		public static string Create (object args1,
		                             object args2,
		                             object args3,
		                             object args4,
		                             object args5,
		                             object args6,
		                             object args7,
		                             object args8,
		                             object args9)
		{
			var sb = new StringBuilder ();

			sb.Append ('{');
			Append (sb, args1);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args2);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args3);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args4);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args5);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args6);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args7);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args8);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args9);
			sb.Append ('}');

			return sb.ToString ();
		}



		/// <summary>
		///     Creates a cache key in "{args1}{args2}{args3} ... ect." format.
		/// </summary>
		[DebuggerStepThrough, MethodImpl (MethodImplOptions.AggressiveInlining)]
		public static string Create (object args1,
		                             object args2,
		                             object args3,
		                             object args4,
		                             object args5,
		                             object args6,
		                             object args7,
		                             object args8,
		                             object args9,
		                             object args10)
		{
			var sb = new StringBuilder ();

			sb.Append ('{');
			Append (sb, args1);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args2);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args3);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args4);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args5);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args6);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args7);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args8);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args9);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args10);
			sb.Append ('}');

			return sb.ToString ();
		}


		/// <summary>
		///     Creates a cache key in "{args1}{args2}{args3} ... ect." format.
		/// </summary>
		[DebuggerStepThrough, MethodImpl (MethodImplOptions.AggressiveInlining)]
		public static string Create (object args1,
		                             object args2,
		                             object args3,
		                             object args4,
		                             object args5,
		                             object args6,
		                             object args7,
		                             object args8,
		                             object args9,
		                             object args10,
		                             object args11,
		                             params object[] args)
		{
			var sb = new StringBuilder ();

			sb.Append ('{');
			Append (sb, args1);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args2);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args3);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args4);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args5);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args6);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args7);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args8);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args9);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args10);
			sb.Append ('}');
			sb.Append ('{');
			Append (sb, args11);
			sb.Append ('}');

			if (args != null)
			{
				foreach (var v in args)
				{
					sb.Append ('{');
					Append (sb, v);
					sb.Append ('}');
				}
			}

			return sb.ToString ();
		}


		/// <summary>
		///     Creates a cache key in "{property1}{property2}{property3} ... ect." format.
		/// </summary>
		[DebuggerStepThrough, MethodImpl (MethodImplOptions.AggressiveInlining)]
		public static string CreateFromObjectProperties (object obj, string prefix = null)
		{
			if (obj == null)
				return string.Empty;

			var accessor = TypeAccessor.Create (obj.GetType ());
			var sb = new StringBuilder ();

			if (!string.IsNullOrEmpty (prefix))
				sb.Append (prefix);

			foreach (var m in accessor.GetMembers ())
			{
				sb.Append ('{');
				Append (sb, accessor[obj, m.Name]);
				sb.Append ('}');
			}

			return sb.ToString ();
		}

		#endregion

		#region Private methods

		[DebuggerStepThrough]
		private static void Append (StringBuilder sb, object v)
		{
			if (v == null)
				return;

			var v_custom = v as ICacheKeyProvider;
			if (v_custom != null)
			{
				sb.Append (v.GetType ().Name);
				sb.Append (':');
				sb.Append (v_custom.GetCacheKey ());
				return;
			}

			var v_str = v as string;
			if (v_str != null)
			{
				sb.Append (v_str);
				return;
			}

			var e = v as IEnumerable;
			if (e != null)
			{
				Append (sb, e);
				return;
			}

			sb.Append (Convert.ToString (v, CultureInfo.InvariantCulture));
		}


		[DebuggerStepThrough]
		private static void Append (StringBuilder sb, IEnumerable e)
		{
			foreach (var ev in e)
			{
				sb.Append (':');
				Append (sb, ev);
			}

			sb.Append (':');
		}

		#endregion
	}
}