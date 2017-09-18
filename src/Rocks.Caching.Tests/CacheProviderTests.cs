using System;
using System.Threading;
using FluentAssertions;
using Xunit;

namespace Rocks.Caching.Tests
{
    public abstract class CacheProviderTestBase
    {
        protected abstract ICacheProvider CreateSut();


        [Fact]
        public void Add_Item_WasNotPresentBefore_AddsIt()
        {
            // arrange
            var cache = this.CreateSut();

            var key = "key";
            var item = "item";
            var parameters = new CachingParameters(TimeSpan.FromDays(1));


            // act
            cache.Clear();
            cache.Add(key, item, parameters);
            var result = cache.Get(key);


            // assert
            result.Should().Be(item);
        }


        [Fact]
        public void Add_Item_WasNotPresentBefore_NoCache_DoesNotAddsIt()
        {
            // arrange
            var cache = this.CreateSut();

            var key = "key";
            var item = "item";
            var parameters = CachingParameters.NoCache;


            // act
            cache.Clear();
            cache.Add(key, item, parameters);
            var result = cache.Get(key);


            // assert
            result.Should().BeNull();
        }


        [Fact]
        public void Add_Null_WasNotPresentBefore_DoesNotAddsIt()
        {
            // arrange
            var cache = this.CreateSut();

            var key = "key";
            var parameters = new CachingParameters(TimeSpan.FromDays(1));


            // act
            cache.Clear();
            cache.Add(key, null, parameters);
            var result = cache.Get(key);


            // assert
            result.Should().BeNull();
        }


        [Fact]
        public void Add_Null_WasPresentBefore_RemovesIt()
        {
            // arrange
            var cache = this.CreateSut();

            var key = "key";
            var parameters = new CachingParameters(TimeSpan.FromDays(1));


            // act
            cache.Clear();
            cache.Add(key, new object(), parameters);
            cache.Add(key, null, parameters);
            var result = cache.Get(key);


            // assert
            result.Should().BeNull();
        }


        [Fact]
        public void Remove_Exists_RemovesIt()
        {
            // arrange
            var cache = this.CreateSut();

            var key = "key";
            var item = "item";
            var parameters = new CachingParameters(TimeSpan.FromDays(1));


            // act
            cache.Clear();
            cache.Add(key, item, parameters);
            cache.Remove(key);
            var result = cache.Get(key);


            // assert
            result.Should().BeNull();
        }


        [Fact]
        public void Remove_NotExists_DoesNothing()
        {
            // arrange
            var cache = this.CreateSut();

            var key = "key";


            // act
            cache.Clear();
            cache.Remove(key);
            var result = cache.Get(key);


            // assert
            result.Should().BeNull();
        }


        [Fact]
        public void Clear_HasItem_RemovesAll()
        {
            // arrange
            var cache = this.CreateSut();

            var key = "key";
            var item = "item";
            var parameters = new CachingParameters(TimeSpan.FromDays(1));


            // act
            cache.Clear();
            cache.Add(key, item, parameters);
            cache.Clear();
            var result = cache.Get(key);


            // assert
            result.Should().BeNull();
        }


        [Fact]
        public void Add_AbsoluteExpiration_ReturnsNullAfterTimePassed()
        {
            // arrange
            var cache = this.CreateSut();

            var key = "key";
            var item = "item";
            var parameters = new CachingParameters(TimeSpan.FromMilliseconds(100));


            // act
            cache.Clear();
            cache.Add(key, item, parameters);
            Thread.Sleep(TimeSpan.FromMilliseconds(200));
            var result = cache.Get(key);


            // assert
            result.Should().BeNull();
        }


        [Fact]
        public void Add_SlidingExpiration_ReturnsNullAfterTimePassed()
        {
            // arrange
            var cache = this.CreateSut();

            var key = "key";
            var item = "item";
            var parameters = new CachingParameters(TimeSpan.FromMilliseconds(100), sliding: true);


            // act
            cache.Clear();
            cache.Add(key, item, parameters);
            Thread.Sleep(TimeSpan.FromMilliseconds(200));
            var result = cache.Get(key);


            // assert
            result.Should().BeNull();
        }
    }
}