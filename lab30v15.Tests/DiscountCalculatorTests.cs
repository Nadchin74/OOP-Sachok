using System;
using Xunit;
using lab30v15;

namespace lab30v15.Tests
{
    public class DiscountCalculatorTests
    {
        private readonly DiscountCalculator _calculator;

        public DiscountCalculatorTests()
        {
            _calculator = new DiscountCalculator();
        }

        // --- Тести для методу CalculateDiscount ---

        [Fact]
        public void CalculateDiscount_ZeroDiscount_ReturnsOriginalPrice()
        {
            Assert.Equal(100m, _calculator.CalculateDiscount(100m, 0m));
        }

        [Fact]
        public void CalculateDiscount_MaxDiscount_ReturnsZero()
        {
            Assert.Equal(0m, _calculator.CalculateDiscount(150m, 100m));
        }

        [Fact]
        public void CalculateDiscount_ZeroPrice_ReturnsZero()
        {
            Assert.Equal(0m, _calculator.CalculateDiscount(0m, 25m));
        }

        [Theory]
        [InlineData(100, 15, 85)]
        [InlineData(200, 50, 100)]
        [InlineData(50, 10, 45)]
        public void CalculateDiscount_ValidInputs_CalculatesCorrectly(decimal price, decimal discount, decimal expected)
        {
            Assert.Equal(expected, _calculator.CalculateDiscount(price, discount));
        }

        [Theory]
        [InlineData(-10, 10)]
        [InlineData(-100, 50)]
        public void CalculateDiscount_NegativePrice_ThrowsArgumentException(decimal price, decimal discount)
        {
            var ex = Assert.Throws<ArgumentException>(() => _calculator.CalculateDiscount(price, discount));
            Assert.Equal("Ціна не може бути від'ємною.", ex.Message);
        }

        [Theory]
        [InlineData(100, -5)]
        [InlineData(100, 105)]
        public void CalculateDiscount_InvalidDiscountRange_ThrowsArgumentException(decimal price, decimal discount)
        {
            var ex = Assert.Throws<ArgumentException>(() => _calculator.CalculateDiscount(price, discount));
            Assert.Equal("Відсоток знижки має бути в межах від 0 до 100.", ex.Message);
        }

        // --- Тести для методу ApplyCoupon ---

        [Theory]
        [InlineData(100, "SAVE10", 90)]
        [InlineData(200, "SAVE20", 160)]
        [InlineData(100, "HALFOFF", 50)]
        public void ApplyCoupon_ValidCoupons_AppliesDiscountCorrectly(decimal price, string coupon, decimal expected)
        {
            Assert.Equal(expected, _calculator.ApplyCoupon(price, coupon));
        }

        [Fact]
        public void ApplyCoupon_CaseInsensitiveCoupon_AppliesCorrectly()
        {
            // Перевіряємо, що регістр літер не має значення (save10 == SAVE10)
            Assert.Equal(90m, _calculator.ApplyCoupon(100m, "save10"));
        }

        [Theory]
        [InlineData(100, null)]
        [InlineData(100, "")]
        [InlineData(100, "   ")]
        public void ApplyCoupon_NullOrEmptyCoupon_ThrowsArgumentException(decimal price, string invalidCoupon)
        {
            var ex = Assert.Throws<ArgumentException>(() => _calculator.ApplyCoupon(price, invalidCoupon));
            Assert.Equal("Купон не може бути порожнім.", ex.Message);
        }

        [Fact]
        public void ApplyCoupon_UnknownCoupon_ThrowsArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() => _calculator.ApplyCoupon(100m, "FAKECOUPON"));
            Assert.Equal("Недійсний код купона.", ex.Message);
        }

        [Fact]
        public void ApplyCoupon_NegativePrice_ThrowsArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() => _calculator.ApplyCoupon(-50m, "SAVE10"));
            Assert.Equal("Ціна не може бути від'ємною.", ex.Message);
        }
    }
}