using System;
using System.Linq;
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
        #region Public methods

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
                                                             return new CachableResult<string> ("aaa",
                                                                                                new CachingParameters (TimeSpan.FromMinutes (1)));
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


        [TestMethod, Timeout (1000)]
        public void TwoThreads_WithConcurency_InvokesGetValueFunctionOnce ()
        {
            MultiThreadWithConcurency_InvokesGetValueFunctionOnce_Test (2, sta: false);
        }


        [TestMethod, Timeout (1000)]
        public void TenThreads_WithConcurency_InvokesGetValueFunctionOnce ()
        {
            MultiThreadWithConcurency_InvokesGetValueFunctionOnce_Test (10, sta: false);
        }


        [TestMethod, Timeout (1000)]
        public void MaxThreads_WithConcurency_InvokesGetValueFunctionOnce ()
        {
            MultiThreadWithConcurency_InvokesGetValueFunctionOnce_Test (Environment.ProcessorCount, sta: false);
        }


        [TestMethod, Timeout (1000)]
        public void TwoThreads_WithConcurency_STA_InvokesGetValueFunctionOnce ()
        {
            MultiThreadWithConcurency_InvokesGetValueFunctionOnce_Test (2, sta: true);
        }


        [TestMethod, Timeout (1000)]
        public void TenThreads_WithConcurency_STA_InvokesGetValueFunctionOnce ()
        {
            MultiThreadWithConcurency_InvokesGetValueFunctionOnce_Test (10, sta: true);
        }


        [TestMethod, Timeout (1000)]
        public void MaxThreads_WithConcurency_STA_InvokesGetValueFunctionOnce ()
        {
            MultiThreadWithConcurency_InvokesGetValueFunctionOnce_Test (Environment.ProcessorCount, sta: true);
        }


        [TestMethod, Timeout (1000)]
        public void TwoThreads_WithConcurency_GetResultThrows_BothThrows ()
        {
            MultiThreadWithConcurency_GetResultThrows_BothThrows_Test (2, sta: false);
        }


        [TestMethod, Timeout (1000)]
        public void TenThreads_WithConcurency_GetResultThrows_BothThrows ()
        {
            MultiThreadWithConcurency_GetResultThrows_BothThrows_Test (10, sta: false);
        }


        [TestMethod, Timeout (1000)]
        public void MaxThreads_WithConcurency_GetResultThrows_BothThrows ()
        {
            MultiThreadWithConcurency_GetResultThrows_BothThrows_Test (Environment.ProcessorCount, sta: false);
        }


        [TestMethod, Timeout (1000)]
        public void TwoThreads_WithConcurency_STA_GetResultThrows_BothThrows ()
        {
            MultiThreadWithConcurency_GetResultThrows_BothThrows_Test (2, sta: true);
        }


        [TestMethod, Timeout (1000)]
        public void TenThreads_WithConcurency_STA_GetResultThrows_BothThrows ()
        {
            MultiThreadWithConcurency_GetResultThrows_BothThrows_Test (10, sta: true);
        }


        [TestMethod, Timeout (1000)]
        public void MaxThreads_WithConcurency_STA_GetResultThrows_BothThrows ()
        {
            MultiThreadWithConcurency_GetResultThrows_BothThrows_Test (Environment.ProcessorCount, sta: true);
        }


        [TestMethod]
        public void ResultDataIsNull_CachesIt ()
        {
            // arrange
            var cache = new CacheProviderStub ();
            var exec_count = 0;

            var create_result = new Func<Task<CachableResult<string>>>
                (() => Task.Run
                           (() =>
                            {
                                exec_count++;
                                return new CachableResult<string> (null, CachingParameters.FromMinutes (1));
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

            var create_result = new Func<Task<CachableResult<string>>>
                (() => Task.Run (() =>
                                 {
                                     exec_count++;

                                     return new CachableResult<string>
                                         (null,
                                          new CachingParameters (TimeSpan.FromMinutes (1),
                                                                 dependencyKeys: new[] { "dependency key" }),
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

            var create_result = new Func<Task<CachableResult<string>>>
                (() => Task.Run (() =>
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
        public void ResultIsNull_ValueTypeResult_DoesNotCache ()
        {
            // arrange
            var cache = new CacheProviderStub ();
            var exec_count = 0;

            var create_result = new Func<Task<CachableResult<int>>>
                (() => Task.Run (() =>
                                 {
                                     exec_count++;
                                     return (CachableResult<int>) null;
                                 }));


            // act
            var result = cache.GetAsync ("Test", create_result).Result;
            var result2 = cache.GetAsync ("Test", create_result).Result;


            // assert
            cache.Values.Should ().BeEmpty ();
            result.Should ().Be (0);
            result2.Should ().Be (0);
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
            var result = cache.GetAsync
                ("TestA",
                 () => Task.Run
                           (async () =>
                                  {
                                      exec_count_a++;

                                      var res = await cache.GetAsync
                                                          ("TestB",
                                                           () => Task.Run (() =>
                                                                           {
                                                                               exec_count_b++;
                                                                               return new CachableResult<string>
                                                                                   ("bbb",
                                                                                    CachingParameters.FromDays (1));
                                                                           }));

                                      return new CachableResult<string> (res,
                                                                         new CachingParameters (TimeSpan.FromDays (1)));
                                  }))
                              .Result;


            // assert
            result.Should ().Be ("bbb");
            cache.Values.Should ().HaveCount (2);
            exec_count_a.Should ().Be (1);
            exec_count_b.Should ().Be (1);
            GetWithLockExtensions.Locks.Should ().BeEmpty ();
        }

        #endregion

        #region Private methods

        private static void MultiThreadWithConcurency_InvokesGetValueFunctionOnce_Test (int maxThreads, bool sta)
        {
            // arrange
            var cache = new CacheProviderStub ();

            var exec_count = 0;
            var start_event = new ManualResetEvent (false);

            Func<Task<CachableResult<string>>> create_result =
                () => Task.Run (async () =>
                                      {
                                          Interlocked.Increment (ref exec_count);

                                          await Task.Delay (TimeSpan.FromMilliseconds (500));

                                          var result = new CachableResult<string> ("aaa", CachingParameters.FromMinutes (1));

                                          return result;
                                      });

            var results = new string[maxThreads];

            var threads = Enumerable
                .Range (0, maxThreads)
                .Select (x =>
                         {
                             var thread = new Thread (() =>
                                                      {
                                                          start_event.WaitOne ();
                                                          results[x] = cache.GetAsync ("Test", create_result).Result;
                                                      });

                             thread.IsBackground = true;

                             if (sta)
                                 thread.SetApartmentState (ApartmentState.STA);

                             return thread;
                         })
                .ToList ();


            // act
            threads.ForEach (t => t.Start ());
            start_event.Set ();
            threads.ForEach (t => t.Join ());


            // assert
            cache.Values
                 .Should ().HaveCount (1)
                 .And.ContainKey ("Test")
                 .WhichValue.Value.Should ().Be ("aaa");

            results.Should ().OnlyContain (x => x == "aaa");
            exec_count.Should ().Be (1);

            GetWithLockExtensions.Locks.Should ().BeEmpty ();
        }


        private static void MultiThreadWithConcurency_GetResultThrows_BothThrows_Test (int maxThreads, bool sta)
        {
            // arrange
            var cache = new CacheProviderStub ();

            var exec_count = 0;
            var exception_count = 0;
            var start_event = new ManualResetEvent (false);

            Func<Task<CachableResult<string>>> create_result =
                () => Task.Run<CachableResult<string>> (async () =>
                                                              {
                                                                  Interlocked.Increment (ref exec_count);

                                                                  await
                                                                      Task.Delay (
                                                                          TimeSpan.FromMilliseconds (
                                                                              500));

                                                                  throw new InvalidOperationException ();
                                                              });

            var threads = Enumerable
                .Range (0, maxThreads)
                .Select (x =>
                         {
                             var thread = new Thread (() =>
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

                             thread.IsBackground = true;

                             if (sta)
                                 thread.SetApartmentState (ApartmentState.STA);

                             return thread;
                         })
                .ToList ();


            // act
            threads.ForEach (t => t.Start ());
            start_event.Set ();
            threads.ForEach (t => t.Join ());


            // assert
            exec_count.Should ().Be (1);
            exception_count.Should ().Be (maxThreads);
            GetWithLockExtensions.Locks.Should ().BeEmpty ();
        }

        #endregion
    }
}