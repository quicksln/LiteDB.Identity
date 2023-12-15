using System;
using FluentAssertions;
using LiteDB.Identity.Validators.Implementations;
using Xunit;

namespace LiteDB.Identity.Tests.Validators.Implementations
{
    public class ValidatorTests
    {
        private readonly Validator _validator;

        public ValidatorTests()
        {
            _validator = new Validator();
        }

        [Theory]
        [InlineData("string")]
        [InlineData("")]
        [InlineData("string with space")]
        [InlineData(1)]
        [InlineData(1f)]
        [InlineData(1d)]
        public void Validator_ValidateForNull_Should_Not_Throw(object value)
        {

            Action act = () => _validator.ValidateForNull(value);
            act.Should().NotThrow<ArgumentNullException>();
        }

        [Theory]
        [InlineData("string")]
        [InlineData("kkkj")]
        [InlineData("string with space")]
        public void Validator_ValidateForNullOrEmptyString_Should_Not_Throw(string value)
        {
            Action act = () => _validator.ValidateForNullOrEmptyString(value);
            act.Should().NotThrow<ArgumentException>();
        }

        [Fact]
        public void Validator_ValidateForNullOrEmptyString_Should_Throw_Argument_Exception()
        {
            string value = null!;
            
            Action act = () => _validator.ValidateForNullOrEmptyString(value);
            
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Validator_ValidateForNullOrEmptyString_Should_Throw_Argument_Exception_With_Variable_Name()
        {
            string value = null!;

            Action act = () => _validator.ValidateForNullOrEmptyString(value, nameof(value));

            act.Should().Throw<ArgumentException>().WithMessage($"Value cannot be null or empty. Parameter name: {nameof(value)}");
        }

        [Fact]
        public void Validator_ValidateForNullOrEmptyString_Should_Throw_Argument_Exception_With_Custom_Message()
        {
            string value = null!;
            string message = "custom message";

            Action act = () => _validator.ValidateForNullOrEmptyString(value, null, message);

            act.Should().Throw<ArgumentException>().WithMessage(message);
        }

        [Fact]
        public void Validator_ValidateForNull_Should_Throw_Argument_Null_Exception()
        {
            object value = null!;

            Action act = () => _validator.ValidateForNull(value);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Validator_ValidateForNull_Should_Throw_Argument_Null_Exception_With_Variable_Name()
        {
            object value = null!;

            Action act = () => _validator.ValidateForNull(value, nameof(value));

            act.Should().Throw<ArgumentException>().WithMessage($"Value cannot be null. (Parameter '{nameof(value)}')");
        }

        [Fact]
        public void Validator_ValidateForNull_Should_Throw_Argument_Null_Exception_With_Custom_Message()
        {
            string value = null!;
            string message = "custom message";

            Action act = () => _validator.ValidateForNull(value, null, message);

            act.Should().Throw<ArgumentException>().WithMessage(message);
        }
    }
}
