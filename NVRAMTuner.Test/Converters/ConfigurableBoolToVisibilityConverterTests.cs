namespace NVRAMTuner.Test.Converters
{
    using Client.Converters;
    using Client.Converters.Parameters;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Windows;

    /// <summary>
    /// Tests targeting <see cref="ConfigurableBoolToVisibilityConverter"/>
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ConfigurableBoolToVisibilityConverterTests
    {
        /// <summary>
        /// System under test
        /// </summary>
        private ConfigurableBoolToVisibilityConverter sut;

        /// <summary>
        /// Tests that the <see cref="ConfigurableBoolToVisibilityConverter.ConvertBack"/> throws
        /// an exception
        /// </summary>
        [TestMethod]
        public void ConvertBackThrowsNotImplementedExceptionTest()
        {
            // Arrange
            this.CreateSut();

            // Act
            // Assert
            this.sut.Invoking(c => c.ConvertBack(null, typeof(Visibility), 5, CultureInfo.CurrentCulture))
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
            this.sut.Invoking(c => c.Convert(null, typeof(Visibility), 5, CultureInfo.CurrentCulture))
                .Should()
                .Throw<ArgumentNullException>();
        }

        /// <summary>
        /// Tests that when given a null parameter, the converter throws an error
        /// </summary>
        [TestMethod]
        public void ConvertWithNullParameterThrowsErrorTest()
        {
            // Arrange
            this.CreateSut();

            // Act
            // Assert
            this.sut.Invoking(c => c.Convert(5, typeof(Visibility), null, CultureInfo.CurrentCulture))
                .Should()
                .Throw<ArgumentNullException>();
        }

        /// <summary>
        /// Tests that when converting with a converter parameter of <see cref="ConfigurableVisConverterParams.Normal"/>,
        /// the value is converted as expected for a true boolean
        /// </summary>
        [TestMethod]
        public void CanConvertTrueWithNormalParamTest()
        {
            // Arrange
            this.CreateSut();

            // Act
            // Assert
            this.sut.Convert(true, typeof(Visibility), "Normal", CultureInfo.CurrentCulture)
                .Should()
                .Be(Visibility.Visible);
        }

        /// <summary>
        /// Tests that when converting with a converter parameter of <see cref="ConfigurableVisConverterParams.Normal"/>,
        /// the value is converted as expected for a false boolean
        /// </summary>
        [TestMethod]
        public void CanConvertFalseWithNormalParamTest()
        {
            // Arrange
            this.CreateSut();

            // Act
            // Assert
            this.sut.Convert(false, typeof(Visibility), "Normal", CultureInfo.CurrentCulture)
                .Should()
                .Be(Visibility.Collapsed);
        }

        /// <summary>
        /// Tests that when converting with a converter parameter of <see cref="ConfigurableVisConverterParams.Reverse"/>,
        /// the value is converted as expected for a true boolean
        /// </summary>
        [TestMethod]
        public void CanConvertTrueWithReverseParamTest()
        {
            // Arrange
            this.CreateSut();

            // Act
            // Assert
            this.sut.Convert(true, typeof(Visibility), "Reverse", CultureInfo.CurrentCulture)
                .Should()
                .Be(Visibility.Collapsed);
        }

        /// <summary>
        /// Tests that when converting with a converter parameter of <see cref="ConfigurableVisConverterParams.Reverse"/>,
        /// the value is converted as expected for a false boolean
        /// </summary>
        [TestMethod]
        public void CanConvertFalseWithReverseParamTest()
        {
            // Arrange
            this.CreateSut();

            // Act
            // Assert
            this.sut.Convert(false, typeof(Visibility), "Reverse", CultureInfo.CurrentCulture)
                .Should()
                .Be(Visibility.Visible);
        }

        /// <summary>
        /// Creates a new <see cref="ConfigurableBoolToVisibilityConverter"/> for testing
        /// </summary>
        private void CreateSut()
        {
            this.sut = new ConfigurableBoolToVisibilityConverter();
        }
    }
}