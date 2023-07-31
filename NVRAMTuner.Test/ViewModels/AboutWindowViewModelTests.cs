#pragma warning disable CS8618

namespace NVRAMTuner.Test.ViewModels
{
    using Client.Services.Wrappers.Interfaces;
    using Client.ViewModels;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Tests targeting <see cref="AboutWindowViewModel"/>
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class AboutWindowViewModelTests
    {
        /// <summary>
        /// System under test.
        /// </summary>
        private AboutWindowViewModel sut;

        /// <summary>
        /// Mock <see cref="IProcessService"/> used by the tests
        /// </summary>
        private Mock<IProcessService> mockProcessService;

        /// <summary>
        /// Test initialisation method
        /// </summary>
        [TestInitialize]
        public void TestInitialise()
        {
            this.mockProcessService = new Mock<IProcessService>();
        }

        /// <summary>
        /// Tests that <see cref="AboutWindowViewModel"/> can be initialised and that
        /// expected properties are not null
        /// </summary>
        [TestMethod]
        public void CanInitialiseAboutWindowViewModelTest()
        {
            // Arrange
            // Act
            this.CreateSut();

            // Assert
            this.sut.Should().NotBeNull();
            this.sut.OpenLicenseInBrowserCommand.Should().NotBeNull();
        }

        /// <summary>
        /// Tests that the <see cref="AboutWindowViewModel.License"/> property returns
        /// the expected value
        /// </summary>
        [TestMethod]
        public void CanGetLicenseTest()
        {
            // Arrange
            string expectedValue = Client.Properties.Resources.GPLv3;

            // Act
            this.CreateSut();

            // Assert
            this.sut.License.Should().Be(expectedValue);
        }

        /// <summary>
        /// Tests that the <see cref="AboutWindowViewModel.Copyright"/> property returns
        /// the expected value
        /// </summary>
        [TestMethod]
        public void CanGetCopyrightTest()
        {
            // Arrange
            string expectedValue = $"Copyright © Reece Mercer {DateTime.Now.Year}";

            // Act
            this.CreateSut();

            // Assert
            this.sut.Copyright.Should().Be(expectedValue);
        }

        /// <summary>
        /// Tests that the <see cref="AboutWindowViewModel.OpenLicenseInBrowserCommand"/> behaves
        /// as expected and opens the correct URl via <see cref="IProcessService.Start(string)"/>
        /// </summary>
        [TestMethod]
        public void OpenLicenseInBrowserCommandTest()
        {
            // Arrange
            // Act
            this.CreateSut();
            this.sut.OpenLicenseInBrowserCommand.Execute(null);

            // Assert
            this.mockProcessService.Verify(m => 
                m.Start(It.Is<string>(s 
                    => s == Client.Properties.Resources.LicenseURL)), Times.Once);
        }

        /// <summary>
        /// Initialises a new instance of <see cref="AboutWindowViewModel"/> for testing
        /// </summary>
        private void CreateSut()
        {
            this.sut = new AboutWindowViewModel(this.mockProcessService.Object);
        }
    }
}
