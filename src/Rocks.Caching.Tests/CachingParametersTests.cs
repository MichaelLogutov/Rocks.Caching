using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ploeh.AutoFixture;

namespace Rocks.Caching.Tests
{
	[TestClass]
	public class CachingParametersTests
	{
		[TestMethod]
		public void Clone_ReturnsDeepClone ()
		{
			// arrange
			var fixture = new Fixture ();

			var source = fixture.Create<CachingParameters> ();


			// act
			var result = source.Clone ();


			// assert
			result.ShouldBeEquivalentTo (source);
		}
	}
}