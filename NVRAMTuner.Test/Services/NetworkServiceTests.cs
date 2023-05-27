namespace NVRAMTuner.Test.Services
{
    using Client.Services;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests targeting <see cref="NetworkService"/>
    /// </summary>
    [TestClass]
    public class NetworkServiceTests
    {
        /// <summary>
        /// Tests that the <see cref="NetworkService.VerifyIpv4Address"/> correctly returns
        /// false for incorrectly formatted IPv4 addresses
        /// </summary>
        /// <param name="testCase"></param>
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

        /// <summary>
        /// Tests that the <see cref="NetworkService.VerifyIpv4Address"/> correctly returns
        /// true for correctly formatted IPv4 addresses
        /// </summary>
        /// <param name="testCase"></param>
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
    }
}