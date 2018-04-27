using FluentAssertions;
using AutoFixture;
using Xunit;

namespace Rocks.Caching.Tests
{
    public class CachingParametersTests
    {
        [Fact]
        public void Clone_ReturnsDeepClone()
        {
            // arrange
            var fixture = new Fixture();

            var source = fixture.Create<CachingParameters>();


            // act
            var result = source.Clone();


            // assert
            result.Should().BeEquivalentTo(source);
        }
    }
}