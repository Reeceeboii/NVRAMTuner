#pragma warning disable CS8618

namespace NVRAMTuner.Test.ViewModels.Variables
{
    using Client.Messages.Variables;
    using Client.Models.Nvram;
    using Client.Models.Nvram.Concrete;
    using Client.Services.Wrappers;
    using Client.Services.Wrappers.Interfaces;
    using Client.ViewModels.Variables;
    using CommunityToolkit.Mvvm.Messaging;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System.Diagnostics.CodeAnalysis;
    using TestUtils;

    /// <summary>
    /// Tests targeting <see cref="EditsViewModel"/>
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class EditsViewModelTests
    {
        /// <summary>
        /// System under test
        /// </summary>
        private EditsViewModel sut;

        /// <summary>
        /// Mock <see cref="IMessengerService"/> used by the tests
        /// </summary>
        private Mock<IMessengerService> mockMessengerService;

        /// <summary>
        /// An instance of <see cref="IMessengerService"/> with an internal reference to
        /// a real <see cref="IMessenger"/> instance. This can actually be used to send messages
        /// to test the private receiver methods of sut(s)
        /// </summary>
        private IMessengerService messengerServiceWithRealMessenger;

        /// <summary>
        /// Test initialisation method
        /// </summary>
        [TestInitialize]
        public void TestInitialise()
        {
            this.mockMessengerService = new Mock<IMessengerService>(MockBehavior.Strict);
            this.messengerServiceWithRealMessenger = new MessengerService(WeakReferenceMessenger.Default);
        }

        /// <summary>
        /// Tests that <see cref="EditsViewModel"/> can be initialised and that
        /// expected properties are not null
        /// </summary>
        [TestMethod]
        public void CanInitialiseEditsViewModelTest()
        {
            // Arrange
            TestUtils.SetupMockMessageRegistration<VariableSelectedMessage, EditsViewModel>(this.mockMessengerService);

            // Act
            this.CreateSut(this.mockMessengerService.Object);

            // Assert
            this.sut.RollbackChangesCommand.Should().NotBeNull();
            this.sut.StageChangesCommand.Should().NotBeNull();
            this.sut.RollbackChangesCommand.CanExecute(null).Should().BeFalse();
            this.sut.StageChangesCommand.CanExecute(null).Should().BeFalse();

            TestUtils.VerifyMessageRegistrationTimes<VariableSelectedMessage, EditsViewModel>(this.mockMessengerService, Times.Once());
        }

        /// <summary>
        /// Tests that the <see cref="EditsViewModel"/> correctly responds to receiving an instance
        /// of <see cref="VariableSelectedMessage"/> from an <see cref="IMessenger"/> that has no
        /// changes applied
        /// </summary>
        [TestMethod]
        public void ReceiveVariableSelectedNoChangesMessageTest()
        {
            // Arrange
            IVariable testVariable = new NvramVariable { Name = "testVar" };

            // Act
            this.CreateSut(this.messengerServiceWithRealMessenger);
            this.messengerServiceWithRealMessenger.Send(new VariableSelectedMessage(testVariable));

            // Assert
            this.sut.SelectedVariable.Should().Be(testVariable);

            // commands should still be unable to execute as no changes have been made to the variable yet
            this.sut.RollbackChangesCommand.CanExecute(null).Should().BeFalse();
            this.sut.StageChangesCommand.CanExecute(null).Should().BeFalse();
        }

        /// <summary>
        /// Tests that the <see cref="EditsViewModel"/> correctly responds to receiving an instance
        /// of <see cref="VariableSelectedMessage"/> from an <see cref="IMessenger"/> that does have changes applied
        /// </summary>
        [TestMethod]
        public void ReceiveVariableSelectedWithChangesMessageTest()
        {
            // Arrange
            IVariable testVariable = new NvramVariable
            {
                Name = "testVar",
                OriginalValue = "original",
                DefaultValue = "original with changes"
            };

            // Act
            this.CreateSut(this.messengerServiceWithRealMessenger);
            this.messengerServiceWithRealMessenger.Send(new VariableSelectedMessage(testVariable));

            // Assert
            this.sut.SelectedVariable.Name.Should().NotBeNull();
            this.sut.RollbackChangesCommand.CanExecute(null).Should().BeTrue();
            this.sut.StageChangesCommand.CanExecute(null).Should().BeTrue();
        }

        /// <summary>
        /// Tests that the <see cref="EditsViewModel.RollbackChangesCommand"/> functions as expected
        /// </summary>
        [TestMethod]
        public void RollbackChangesCommandTest()
        {
            // Arrange
            IVariable testVariable = new NvramVariable
            {
                Name = "testVar",
                OriginalValue = "original",
                ValueDelta = "original with changes"
            };

            // Act
            this.CreateSut(this.messengerServiceWithRealMessenger);
            this.messengerServiceWithRealMessenger.Send(new VariableSelectedMessage(testVariable));
            this.sut.RollbackChangesCommand.Execute(null);

            // Assert
            this.sut.SelectedVariable.Should().NotBeNull();
            testVariable.ValueDelta.Should().Be(testVariable.OriginalValue);
            this.sut.RollbackChangesCommand.CanExecute(null).Should().BeFalse();
            this.sut.StageChangesCommand.CanExecute(null).Should().BeFalse();
        }

        /// <summary>
        /// Initialises a new instance of <see crefs="EditsViewModel"/> for testing
        /// </summary>
        /// <param name="mockMessengerServiceForSut">An instance of <see cref="IMessengerService"/></param>
        private void CreateSut(IMessengerService mockMessengerServiceForSut)
        {
            this.sut = new EditsViewModel(mockMessengerServiceForSut);
        }
    }
}