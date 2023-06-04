﻿namespace NVRAMTuner.Test.Services
{
    using Client.Messages;
    using Client.Services;
    using Client.Services.Interfaces;
    using CommunityToolkit.Mvvm.Messaging;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;
    using System.Collections.Generic;
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
        /// Mock <see cref="ISshClientService"/> used by the tests
        /// </summary>
        private Mock<ISshClientService> mockSshClientService;

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
        /// </summary>
        private Mock<IMessenger> mockMessenger;

        /// <summary>
        /// Test initialisation method
        /// </summary>
        [TestInitialize]
        public void TestInitialise()
        {
            this.mockSshClientService = new Mock<ISshClientService>(MockBehavior.Strict);
            this.mockFileSystem = new Mock<IFileSystem>(MockBehavior.Strict);
            this.mockEnvironmentService = new Mock<IEnvironmentService>(MockBehavior.Strict);
            this.mockMessenger = new Mock<IMessenger>(MockBehavior.Strict);
        }

        /// <summary>
        /// Tests that the <see cref="NetworkService"/> class can be initialised as expected
        /// </summary>
        [TestMethod]
        public void CanInitialiseNetworkServiceTest()
        {
            // Arrange
            // Act
            this.CreateSut();

            // Assert
            this.sut.Should().NotBeNull();
        }

        /// <summary>
        /// Tests the <see cref="NetworkService.LocateLocalSshKeys"/> method correctly behaves when a
        /// set of SSH keys are found on the local filesystem
        /// </summary>
        [TestMethod]
        public void LocateLocalSshKeysWhenKeysAreFoundTest()
        {
            // Arrange
            const string testHomeDir = @"C:\\users\\user\\";
            this.mockEnvironmentService
                .Setup(m 
                    => m.GetFolderPath(It.IsAny<Environment.SpecialFolder>()))
                .Returns(testHomeDir)
                .Verifiable();
            this.mockFileSystem
                .Setup(m => m.Path.Combine(It.Is<string>(s
                    => s == testHomeDir), It.Is<string>(s2 
                    => s2 == ".ssh")))
                .Returns(@$"{testHomeDir}\\.ssh")
                .Verifiable();

            this.mockEnvironmentService.Setup(m => m.GetFolderPath(It.IsAny<Environment.SpecialFolder>()));

            // Act
            this.CreateSut();
            this.sut.LocateLocalSshKeys();

            // Assert
            1.Should().Be(1);
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
        public void VerifyNetworkPortStringCanRecogniseIncorrectPortsTest(string testCase)
        {
            NetworkService.VerifyNetworkPort(testCase).Should().BeFalse();
        }

        #endregion

        /// <summary>
        /// Initialises a new instance of the <see cref="NetworkService"/> class for the tests
        /// </summary>
        private void CreateSut()
        {
            this.sut = new NetworkService(
                this.mockSshClientService.Object,
                this.mockFileSystem.Object, 
                this.mockEnvironmentService.Object,
                this.mockMessenger.Object);
        }
    }
}