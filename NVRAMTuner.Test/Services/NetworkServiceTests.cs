#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS8618

namespace NVRAMTuner.Test.Services
#pragma warning restore IDE0079 // Remove unnecessary suppression
{
    using Client.Services;
    using Client.Services.Interfaces;
    using Client.Services.Wrappers;
    using Client.Services.Wrappers.Interfaces;
    using CommunityToolkit.Mvvm.Messaging;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System.Diagnostics.CodeAnalysis;
    using System.IO.Abstractions;

    /// <summary>
    /// Tests targeting <see cref="NetworkService"/>
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class NetworkServiceTests
    {
        /// <summary>
        /// System under test
        /// </summary>
        private INetworkService sut;

        /// <summary>
        /// Mock <see cref="IFileSystem"/> used by the tests
        /// </summary>
        private Mock<IFileSystem> mockFileSystem;

        /// <summary>
        /// Mock <see cref="IEnvironmentService"/> used by the tests
        /// </summary>
        private Mock<IEnvironmentService> mockEnvironmentService;

        /// <summary>
        /// Mock <see cref="IMessenger"/> used by the tests
        /// TODO - convert to wrapper: https://github.com/Reeceeboii/NVRAMTuner/issues/25
        /// </summary>
        private Mock<IMessenger> mockMessenger;

        /// <summary>
        /// Mock <see cref="ISettingsService"/> used by the tests
        /// </summary>
        private Mock<ISettingsService> mockSettingsService;

        /// <summary>
        /// Test initialisation method
        /// </summary>
        [TestInitialize]
        public void TestInitialise()
        {
            this.mockFileSystem = new Mock<IFileSystem>(MockBehavior.Strict);
            this.mockEnvironmentService = new Mock<IEnvironmentService>(MockBehavior.Strict);
            this.mockMessenger = new Mock<IMessenger>(MockBehavior.Strict);
            this.mockSettingsService = new Mock<ISettingsService>(MockBehavior.Strict);
        }

        /// <summary>
        /// Tests that the <see cref="NetworkService"/> class can be initialised as expected
        /// </summary>
        [TestMethod]
        public void CanInitialiseNetworkServiceTest()
        {
            // Arrange
            IMessengerService testMessengerService = new MessengerService(this.mockMessenger.Object);

            // Act
            this.CreateSut(testMessengerService);

            // Assert
            this.sut.Should().NotBeNull();
            NetworkService.DefaultSshPort.Should().Be("22");
        }

        /// <summary>
        /// Tests the <see cref="NetworkService.FolderContainsSshKeys"/> method behaves correctly when
        /// a directory is provided and it does contain a public and private SSH key pair
        /// </summary>
        [TestMethod]
        public void FolderContainsSshKeysKeysReturnsTrueWhenKeysPresentTest()
        {
            // Arrange
            IMessengerService testMessengerService = new MessengerService(this.mockMessenger.Object);
            const string testFolder = @"Some\Path\To\.ssh\Keys";

            this.mockFileSystem.Setup(m
                    => m.Path.Combine(
                        It.Is<string>(s => s.Equals(testFolder)),
                        It.Is<string>(s => s.Equals("id_rsa.pub") || s.Equals("id_rsa"))))
                .Returns("something")
                .Verifiable();

            this.mockFileSystem.Setup(m => m.File.Exists(It.IsAny<string>()))
                .Returns(true)
                .Verifiable();

            // Act
            this.CreateSut(testMessengerService);
            bool existResult = this.sut.FolderContainsSshKeys(testFolder);

            // Assert
            existResult.Should().BeTrue();
        }

        /// <summary>
        /// Tests the <see cref="NetworkService.FolderContainsSshKeys"/> method behaves correctly when
        /// a directory is provided and it does NOT contain a public and private SSH key pair
        /// </summary>
        [TestMethod]
        public void FolderContainsSshKeysKeysReturnsFalseWhenKeysNotPresentTest()
        {
            // Arrange
            IMessengerService testMessengerService = new MessengerService(this.mockMessenger.Object);
            const string testFolder = @"Some\Path\To\.ssh\Keys";

            this.mockFileSystem.Setup(m
                    => m.Path.Combine(
                        It.Is<string>(s => s.Equals(testFolder)),
                        It.Is<string>(s => s.Equals("id_rsa.pub") || s.Equals("id_rsa"))))
                .Returns("something")
                .Verifiable();

            this.mockFileSystem.Setup(m => m.File.Exists(It.IsAny<string>()))
                .Returns(false)
                .Verifiable();

            // Act
            this.CreateSut(testMessengerService);
            bool existResult = this.sut.FolderContainsSshKeys(testFolder);

            // Assert
            existResult.Should().BeFalse();
        }

        #region VerifyIpv4Address tests

        /// <summary>
        /// Tests that the <see cref="NetworkService.VerifyIpv4Address"/> correctly returns
        /// true for correctly formatted IPv4 addresses
        /// </summary>
        /// <param name="testCase">A test case expected to pass</param>
        [DataTestMethod]
        [DataRow("1.1.1.1")]
        [DataRow("0.0.0.0")]
        [DataRow("   128.34.54.14          ")]
        [DataRow("             1.1.1.1")]
        [DataRow("1.1.1.1           ")]
        [DataRow("255.255.255.255")]
        [DataRow("192.168.1.5")]
        public void VerifyIpv4AddressCanRecogniseCorrectInputsTest(string testCase)
        {
            NetworkService.VerifyIpv4Address(testCase).Should().BeTrue();
        }

        /// <summary>
        /// Tests that the <see cref="NetworkService.VerifyIpv4Address"/> method correctly
        /// returns false for incorrectly formatted IPv4 addresses
        /// </summary>
        /// <param name="testCase">A test case expected to fail</param>
        [DataTestMethod]
        [DataRow("1")]
        [DataRow("2")]
        [DataRow("")]
        [DataRow("....")]
        [DataRow(" 1.1.1. ")]
        [DataRow("     ")]
        [DataRow("-1.-1.-1.-1")]
        [DataRow("I am not even an ipv4 address")]
        [DataRow("😀 never trust user input")]
        public void VerifyIpv4AddressCanRecogniseIncorrectInputsTest(string testCase)
        {
            NetworkService.VerifyIpv4Address(testCase).Should().BeFalse();
        }

        #endregion

        #region VerifyNetworkPort Tests

        /// <summary>
        /// Verifies that the <see cref="NetworkService.VerifyNetworkPort(int)"/> method returns
        /// true for valid port numbers
        /// </summary>
        [DataTestMethod]
        [DataRow(1)]
        [DataRow(1000)]
        [DataRow(25000)]
        [DataRow(65535)]
        public void VerifyNetworkPortIntCanRecogniseCorrectPortsTest(int testCase)
        {
            NetworkService.VerifyNetworkPort(testCase).Should().BeTrue();
        }

        /// <summary>
        /// Verifies that the <see cref="NetworkService.VerifyNetworkPort(int)"/> method returns
        /// false for invalid port numbers
        /// </summary>
        [DataTestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(65536)]
        [DataRow(100000)]
        public void VerifyNetworkPortIntCanRecogniseIncorrectPortsTest(int testCase)
        {
            NetworkService.VerifyNetworkPort(testCase).Should().BeFalse();
        }

        /// <summary>
        /// Verifies that the <see cref="NetworkService.VerifyNetworkPort(string)"/> method returns
        /// true for valid port numbers that are passed in as strings. Implicitly also tests the
        /// <see cref="NetworkService.VerifyNetworkPort(int)"/> overload
        /// </summary>
        [DataTestMethod]
        [DataRow("1")]
        [DataRow("1000")]
        [DataRow("25000")]
        [DataRow("65535")]
        [DataRow("   55   ")]
        [DataRow("       25000")]
        [DataRow("22            ")]
        public void VerifyNetworkPortStringCanRecogniseCorrectPortsTest(string testCase)
        {
            NetworkService.VerifyNetworkPort(testCase).Should().BeTrue();
        }

        /// <summary>
        /// Verifies that the <see cref="NetworkService.VerifyNetworkPort(string)"/> method returns
        /// false for invalid port numbers that are passed in as strings. Implicitly also tests the
        /// <see cref="NetworkService.VerifyNetworkPort(int)"/> overload
        /// </summary>
        [DataTestMethod]
        [DataRow("0")]
        [DataRow("")]
        [DataRow("     ")]
        [DataRow("65536")]
        [DataRow("   799234   ")]
        [DataRow("       250000")]
        [DataRow("2444442            ")]
        [DataRow("")]
        public void VerifyNetworkPortStringCanRecogniseIncorrectPortsTest(string testCase)
        {
            NetworkService.VerifyNetworkPort(testCase).Should().BeFalse();
        }

        #endregion

        /// <summary>
        /// Initialises a new instance of the <see cref="NetworkService"/> class for the tests
        /// </summary>
        /// <param name="testMessengerService">An instance of <see cref="IMessengerService"/></param>
        private void CreateSut(IMessengerService testMessengerService)
        {
            this.sut = new NetworkService(
                this.mockFileSystem.Object, 
                this.mockEnvironmentService.Object,
                testMessengerService,
                this.mockSettingsService.Object);
        }
    }
}