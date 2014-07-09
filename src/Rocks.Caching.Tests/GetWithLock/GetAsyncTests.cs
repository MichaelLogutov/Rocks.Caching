using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocks.Caching.Tests.Stubs;

// ReSharper disable ImplicitlyCapturedClosure
// ReSharper disable UnusedMember.Global

namespace Rocks.Caching.Tests.GetWithLock
{
	[TestClass]
	public class GetAsyncTests
	{
		[TestMethod]
		public void SingleThread_InvokesGetValueFunctionOnce ()
		{
			// arrange
			var cache = new CacheProviderStub ();
			var exec_count = 0;


			// act
			var actual = cache.GetAsync ("Test",
			                             () => Task.Run (() =>
			                             {
				                             exec_count++;
				                             return new CachableResult<string> ("aaa", new CachingParameters (TimeSpan.FromMinutes (1)));
			                             }))
			                  .Result;


			// assert
			actual.Should ().Be ("aaa");

			cache.Values
			     .Should ().HaveCount (1)
			     .And.ContainKey ("Test")
			     .WhichValue.Value.Should ().Be ("aaa");

			exec_count.Should ().Be (1);
		}


		[TestMethod]
		public void MultiThreadWithConcurency_InvokesGetValueFunctionOnce ()
		{
			// arrange
			var cache = new CacheProviderStub ();

			string result1 = null;
			string result2 = null;
			var exec_count = 0;
			var start_event = new ManualResetEvent (false);

			Func<Task<CachableResult<string>>> create_result = () => Task.Run (async () =>
			{
				Interlocked.Increment (ref exec_count);

				await Task.Delay (TimeSpan.FromMilliseconds (300));

				var result = new CachableResult<string> ("aaa", new CachingParameters (TimeSpan.FromMinutes (1)));

				return result;
			});

			var t1 = new Thread (() =>
			{
				start_event.WaitOne ();
				result1 = cache.GetAsync ("Test", create_result).Result;
			});

			var t2 = new Thread (() =>
			{
				start_event.WaitOne ();
				result2 = cache.GetAsync ("Test", create_result).Result;
			});


			// act
			t1.Start ();
			t2.Start ();

			start_event.Set ();

			t1.Join ();
			t2.Join ();


			// assert
			cache.Values
			     .Should ().HaveCount (1)
			     .And.ContainKey ("Test")
			     .WhichValue.Value.Should ().Be ("aaa");

			result1.Should ().Be ("aaa");
			result2.Should ().Be ("aaa");

			exec_count.Should ().Be (1);

			CachedResults.Locks.Should ().BeEmpty ();
		}


		[TestMethod]
		public void MultiThreadWithConcurency_GetResultThrows_BothThrows ()
		{
			// arrange
			var cache = new CacheProviderStub ();

			var exec_count = 0;
			var exception_count = 0;
			var start_event = new ManualResetEvent (false);

			Func<Task<CachableResult<string>>> create_result = () => Task.Run<CachableResult<string>> (async () =>
			{
				Interlocked.Increment (ref exec_count);

				await Task.Delay (TimeSpan.FromMilliseconds (300));

				throw new InvalidOperationException ();

				return null;
			});


			var t1 = new Thread (() =>
			{
				start_event.WaitOne ();
				try
				{
					cache.GetAsync ("Test", create_result).Wait ();
				}
				catch (AggregateException)
				{
					Interlocked.Increment (ref exception_count);
				}
			});

			var t2 = new Thread (() =>
			{
				start_event.WaitOne ();
				try
				{
					cache.GetAsync ("Test", create_result).Wait ();
				}
				catch (AggregateException)
				{
					Interlocked.Increment (ref exception_count);
				}
			});


			// act
			t1.Start ();
			t2.Start ();

			start_event.Set ();

			t1.Join ();
			t2.Join ();


			// assert
			exec_count.Should ().Be (1);
			exception_count.Should ().Be (2);
			CachedResults.Locks.Should ().BeEmpty ();
		}


		[TestMethod]
		public void ResultDataIsNull_CachesIt ()
		{
			// arrange
			var cache = new CacheProviderStub ();
			var exec_count = 0;

			var create_result = new Func<Task<CachableResult<string>>> (() => Task.Run (() =>
			{
				exec_count++;
				return new CachableResult<string> (null, new CachingParameters (TimeSpan.FromMinutes (1)));
			}));


			// act
			var result = cache.GetAsync ("Test", create_result).Result;
			var result2 = cache.GetAsync ("Test", create_result).Result;


			// assert
			cache.Values.Should ().HaveCount (1);
			result.Should ().BeNull ();
			result2.Should ().BeNull ();
			exec_count.Should ().Be (1);
		}


		[TestMethod]
		public void ResultDataIsNullAndDependencyKeysIncludeResult_DoesNotCache ()
		{
			// arrange
			var cache = new CacheProviderStub ();
			var exec_count = 0;

			var create_result = new Func<Task<CachableResult<string>>> (() => Task.Run (() =>
			{
				exec_count++;

				return new CachableResult<string> (null,
				                                   new CachingParameters (TimeSpan.FromMinutes (1), dependencyKeys: new[] { "dependency key" }),
				                                   dependencyKeysIncludeResult: true);
			}));


			// act
			var result = cache.GetAsync ("Test", create_result).Result;
			var result2 = cache.GetAsync ("Test", create_result).Result;


			// assert
			cache.Values.Should ().NotContainKey ("Test");
			result.Should ().BeNull ();
			result2.Should ().BeNull ();
			exec_count.Should ().Be (2);
		}


		[TestMethod]
		public void ResultIsNull_DoesNotCache ()
		{
			// arrange
			var cache = new CacheProviderStub ();
			var exec_count = 0;

			var create_result = new Func<Task<CachableResult<string>>> (() => Task.Run (() =>
			{
				exec_count++;
				return (CachableResult<string>) null;
			}));


			// act
			var result = cache.GetAsync ("Test", create_result).Result;
			var result2 = cache.GetAsync ("Test", create_result).Result;


			// assert
			cache.Values.Should ().BeEmpty ();
			result.Should ().BeNull ();
			result2.Should ().BeNull ();
			exec_count.Should ().Be (2);
		}


		[TestMethod]
		public void NestedCalls_ReturnsItem ()
		{
			// arrange
			var cache = new CacheProviderStub ();
			var exec_count_a = 0;
			var exec_count_b = 0;


			// act
			var result = cache.GetAsync ("TestA",
			                             () => Task.Run (async () =>
			                             {
				                             exec_count_a++;

				                             var res = await cache.GetAsync ("TestB",
				                                                             () => Task.Run (() =>
				                                                             {
					                                                             exec_count_b++;
					                                                             return new CachableResult<string> ("bbb",
					                                                                                                new CachingParameters (TimeSpan.FromDays (1)));
				                                                             }));

				                             return new CachableResult<string> (res, new CachingParameters (TimeSpan.FromDays (1)));
			                             }))
			                  .Result;


			// assert
			result.Should ().Be ("bbb");
			cache.Values.Should ().HaveCount (2);
			exec_count_a.Should ().Be (1);
			exec_count_b.Should ().Be (1);
			CachedResults.Locks.Should ().BeEmpty ();
		}
	}
}