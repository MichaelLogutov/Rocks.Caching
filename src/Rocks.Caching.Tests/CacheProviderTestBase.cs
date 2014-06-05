using System;
using System.Threading;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rocks.Caching.Tests
{
	public abstract class CacheProviderTestBase
	{
		protected abstract ICacheProvider CreateSut ();


		[TestMethod]
		public void Add_Item_WasNotPresentBefore_AddsIt ()
		{
			// arrange
			var cache = this.CreateSut ();

			var key = "key";
			var item = "item";
			var parameters = new CachingParameters (TimeSpan.FromDays (1));


			// act
			cache.Clear ();
			cache.Add (key, item, parameters);
			var result = cache.Get (key);


			// assert
			result.Should ().Be (item);
		}


		[TestMethod]
		public void Add_Null_WasNotPresentBefore_DoesNotAddsIt ()
		{
			// arrange
			var cache = this.CreateSut ();

			var key = "key";
			var parameters = new CachingParameters (TimeSpan.FromDays (1));


			// act
			cache.Clear ();
			cache.Add (key, null, parameters);
			var result = cache.Get (key);


			// assert
			result.Should ().BeNull ();
		}


		[TestMethod]
		public void Add_Null_WasPresentBefore_RemovesIt ()
		{
			// arrange
			var cache = this.CreateSut ();

			var key = "key";
			var parameters = new CachingParameters (TimeSpan.FromDays (1));


			// act
			cache.Clear ();
			cache.Add (key, new object (), parameters);
			cache.Add (key, null, parameters);
			var result = cache.Get (key);


			// assert
			result.Should ().BeNull ();
		}


		[TestMethod]
		public void Remove_Exists_RemovesIt ()
		{
			// arrange
			var cache = this.CreateSut ();

			var key = "key";
			var item = "item";
			var parameters = new CachingParameters (TimeSpan.FromDays (1));


			// act
			cache.Clear ();
			cache.Add (key, item, parameters);
			cache.Remove (key);
			var result = cache.Get (key);


			// assert
			result.Should ().BeNull ();
		}


		[TestMethod]
		public void Remove_NotExists_DoesNothing ()
		{
			// arrange
			var cache = this.CreateSut ();

			var key = "key";


			// act
			cache.Clear ();
			cache.Remove (key);
			var result = cache.Get (key);


			// assert
			result.Should ().BeNull ();
		}


		[TestMethod]
		public void Clear_HasItem_RemovesAll ()
		{
			// arrange
			var cache = this.CreateSut ();

			var key = "key";
			var item = "item";
			var parameters = new CachingParameters (TimeSpan.FromDays (1));


			// act
			cache.Clear ();
			cache.Add (key, item, parameters);
			cache.Clear ();
			var result = cache.Get (key);


			// assert
			result.Should ().BeNull ();
		}


		[TestMethod]
		public void Add_AbsoluteExpiration_ReturnsNullAfterTimePassed ()
		{
			// arrange
			var cache = this.CreateSut ();

			var key = "key";
			var item = "item";
			var parameters = new CachingParameters (TimeSpan.FromMilliseconds (100));


			// act
			cache.Clear ();
			cache.Add (key, item, parameters);
			Thread.Sleep (TimeSpan.FromMilliseconds (200));
			var result = cache.Get (key);


			// assert
			result.Should ().BeNull ();
		}


		[TestMethod]
		public void Add_SlidingExpiration_ReturnsNullAfterTimePassed ()
		{
			// arrange
			var cache = this.CreateSut ();

			var key = "key";
			var item = "item";
			var parameters = new CachingParameters (TimeSpan.FromMilliseconds (100), sliding: true);


			// act
			cache.Clear ();
			cache.Add (key, item, parameters);
			Thread.Sleep (TimeSpan.FromMilliseconds (200));
			var result = cache.Get (key);


			// assert
			result.Should ().BeNull ();
		}


		[TestMethod]
		public void Add_WithDependency_DependentItemAlreadyExists_AddsItem ()
		{
			// arrange
			var cache = this.CreateSut ();

			var dependent_key = "dependent key";
			var key = "key";
			var item = "item";
			var parameters = new CachingParameters (TimeSpan.FromDays (1), dependencyKeys: new[] { dependent_key, null });


			// act
			cache.Clear ();
			cache.Add (dependent_key, "dependent item", new CachingParameters (TimeSpan.FromDays (1)));
			cache.Add (key, item, parameters);

			var result = cache.Get (key);


			// assert
			result.Should ().Be (item);
		}


		[TestMethod]
		public void Add_WithDependency_DependentItemAlreadyExistsAndHasExpiration_ReturnsNullAfterTimePassed ()
		{
			// arrange
			var cache = this.CreateSut ();

			var dependent_key = "dependent key";
			var dependent_parameters = new CachingParameters (TimeSpan.FromMilliseconds (100));

			var key = "key";
			var item = "item";
			var parameters = new CachingParameters (TimeSpan.FromDays (1), dependencyKeys: new[] { dependent_key });


			// act
			cache.Clear ();
			cache.Add (dependent_key, "dependent item", dependent_parameters);
			cache.Add (key, item, parameters);

			Thread.Sleep (TimeSpan.FromMilliseconds (200));
			cache.Get (dependent_key);

			var result = cache.Get (key);


			// assert
			result.Should ().BeNull ();
		}


		[TestMethod]
		public void Add_WithDependency_DependentWasNotPresent_AddsItem ()
		{
			// arrange
			var cache = this.CreateSut ();

			var dependent_key = "dependent key";
			var key = "key";
			var item = "item";
			var parameters = new CachingParameters (TimeSpan.FromDays (1), dependencyKeys: new[] { dependent_key });


			// act
			cache.Clear ();
			cache.Add (key, item, parameters);

			var result = cache.Get (key);


			// assert
			result.Should ().Be (item);
		}


		[TestMethod]
		public void Add_WithDependency_DependentItemAlreadyExists_ReturnsNullAfterDependentItemRemoved ()
		{
			// arrange
			var cache = this.CreateSut ();

			var dependent_key = "dependent key";
			var key = "key";
			var item = "item";
			var parameters = new CachingParameters (TimeSpan.FromDays (1), dependencyKeys: new[] { dependent_key });


			// act
			cache.Clear ();
			cache.Add (dependent_key, new object (), new CachingParameters (TimeSpan.FromDays (1)));
			cache.Add (key, item, parameters);
			cache.Remove (dependent_key);

			var result = cache.Get (key);


			// assert
			result.Should ().BeNull ();
		}


		[TestMethod]
		public void Add_WithDependency_DependentItemWasNotPresent_ReturnsNullAfterDependentItemRemoved ()
		{
			// arrange
			var cache = this.CreateSut ();

			var dependent_key = "dependent key";
			var key = "key";
			var item = "item";
			var parameters = new CachingParameters (TimeSpan.FromDays (1), dependencyKeys: new[] { dependent_key });


			// act
			cache.Clear ();
			cache.Add (key, item, parameters);
			cache.Remove (dependent_key);

			var result = cache.Get (key);


			// assert
			result.Should ().BeNull ();
		}
	}
}