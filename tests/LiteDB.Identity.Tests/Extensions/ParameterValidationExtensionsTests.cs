namespace LiteDB.Identity.Tests.Extensions
{
    using System;

    using FluentAssertions;

    using Xunit;

    using static LiteDB.Identity.Extensions.ParameterValidationExtensions;

    public class ParameterValidationExtensionsTests
    {
        [Theory]
        [InlineData("string")]
        [InlineData("")]
        [InlineData("string with space")]
        [InlineData(1)]
        [InlineData(1f)]
        [InlineData(1d)]
        public void ThrowArgumentNullExceptionIfNull_Should_Not_Throw_Exception(object value)
        {
            value.Invoking(y => y.ThrowArgumentNullExceptionIfNull(nameof(y)))
                .Should().NotThrow();
        }

        [Fact]
        public void ThrowArgumentNullExceptionIfNull_Should_Throw_Exception()
        {
            string variable = null;
            var parameterName = "Name";

            variable.Invoking(y => y.ThrowArgumentNullExceptionIfNull(parameterName))
                .Should().Throw<ArgumentNullException>()
                .WithMessage($"Value cannot be null. (Parameter '{parameterName}')");
        }
    }
}