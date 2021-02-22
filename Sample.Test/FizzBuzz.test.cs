using Xunit;
using static Sample.Core.FizzBuzz;

namespace Sample.Test
{
    public class FizzBuzzTest
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(-1)]
        public void ToFizzBuzzFormat_Returns_NumberString(int i)
            => Assert.Equal(i.ToString(), i.ToFizzBuzzFormat());

        [Theory]
        [InlineData(3)]
        [InlineData(-3)]
        [InlineData(123456789)]
        public void ToFizzBuzzFormat_Returns_Fizz(int i)
            => Assert.Equal("Fizz", i.ToFizzBuzzFormat());

        [Theory]
        [InlineData(5)]
        [InlineData(-10)]
        [InlineData(2000)]
        public void ToFizzBuzzFormat_Returns_Buzz(int i)
            => Assert.Equal("Buzz", i.ToFizzBuzzFormat());

        [Theory]
        [InlineData(0)]
        [InlineData(15)]
        [InlineData(-30)]
        public void ToFizzBuzzFormat_Returns_FizzBuzz(int i)
            => Assert.Equal("Fizz Buzz", i.ToFizzBuzzFormat());
    }
}
