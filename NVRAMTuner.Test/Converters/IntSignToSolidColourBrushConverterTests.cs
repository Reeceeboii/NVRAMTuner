namespace NVRAMTuner.Test.Converters
{
    using Client.Converters;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Windows.Media;

    /// <summary>
    /// Tests targeting <see cref="IntSignToSolidColourBrushConverter"/>
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class IntSignToSolidColourBrushConverterTests
    {
        /// <summary>
        /// System under test
        /// </summary>
        private IntSignToSolidColourBrushConverter sut;

        /// <summary>
        /// Tests that the <see cref="IntSignToSolidColourBrushConverter.ConvertBack"/> throws
        /// an exception
        /// </summary>
        [TestMethod]
        public void ConvertBackThrowsNotImplementedExceptionTest()
        {
            // Arrange
            this.CreateSut();

            // Act
            // Assert
            this.sut.Invoking(c => c.ConvertBack(null, typeof(SolidColorBrush), 5, CultureInfo.CurrentCulture))
                .Should()
                .Throw<NotImplementedException>();
        }

        /// <summary>
        /// Tests that when given a null value, the converter throws an error
        /// </summary>
        [TestMethod]
        public void ConvertWithNullValueThrowsErrorTest()
        {
            // Arrange
            this.CreateSut();

            // Act
            // Assert
            this.sut.Invoking(c => c.Convert(null, typeof(Colors), 5, CultureInfo.CurrentCulture))
                .Should()
                .Throw<ArgumentNullException>();
        }

        /// <summary>
        /// Tests that the <see cref="IntSignToSolidColourBrushConverter.Convert"/> method correctly returns the
        /// right colour when given a positive input
        /// </summary>
        /// <param name="testCase">A test value expected to result in <see cref="Colors.Green"/></param>
        [DataTestMethod]
        [DataRow(1)]
        [DataRow(int.MaxValue)]
        public void CanConvertWithPositiveValuesTest(int testCase)
        {
            // Arrange
            this.CreateSut();

            // Act
            object? test = this.sut.Convert(testCase, typeof(Colors), null, CultureInfo.CurrentCulture);
            
            // Assert
            test.Should().NotBeNull();
            test.Should().Be(Colors.Orange);
        }

        /// <summary>
        /// Tests that the <see cref="IntSignToSolidColourBrushConverter.Convert"/> method correctly returns the
        /// right colour when given a negative input
        /// </summary>
        /// <param name="testCase">A test value expected to result in <see cref="Colors.Red"/></param>
        [DataTestMethod]
        [DataRow(-1)]
        [DataRow(int.MinValue)]
        public void CanConvertWithNegativeValuesTest(int testCase)
        {
            // Arrange
            this.CreateSut();

            // Act
            object? test = this.sut.Convert(testCase, typeof(Colors), null, CultureInfo.CurrentCulture);

            // Assert
            test.Should().NotBeNull();
            test.Should().Be(Colors.Green);
        }

        /// <summary>
        /// Tests that the <see cref="IntSignToSolidColourBrushConverter.Convert"/> method correctly returns
        /// transparency when zero is given as an input
        /// </summary>
        [TestMethod]
        public void CanConvertWithZeroValueTest()
        {
            // Arrange
            this.CreateSut();

            // Act
            object? test = this.sut.Convert(0, typeof(Colors), null, CultureInfo.CurrentCulture);

            // Assert
            test.Should().NotBeNull();
            test.Should().Be(Colors.Transparent);
        }

        /// <summary>
        /// Initialises a new instance of <see cref="IntSignToSolidColourBrushConverter"/> for testing
        /// </summary>
        private void CreateSut()
        {
            this.sut = new IntSignToSolidColourBrushConverter();
        }
    }
}