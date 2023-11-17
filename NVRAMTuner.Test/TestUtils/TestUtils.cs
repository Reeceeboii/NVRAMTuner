namespace NVRAMTuner.Test.TestUtils
{
    using Client.Services.Wrappers.Interfaces;
    using CommunityToolkit.Mvvm.Messaging;
    using Moq;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Test utilities
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class TestUtils
    {
        /// <summary>
        /// Sets up a <see cref="Mock{T}"/> of <see cref="IMessengerService"/> to expected an invocation of
        /// <see cref="IMessengerService.Register{TRecipient,TMessage}"/> with a given message and recipient type
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to expect to be registered</typeparam>
        /// <typeparam name="TRecipient">The type of the expected recipient</typeparam>
        /// <param name="mock">The given instance of <see cref="Mock{T}"/></param>
        public static void SetupMockMessageRegistration<TMessage, TRecipient>(Mock<IMessengerService> mock)
            where TRecipient : class
            where TMessage : class
        {
            mock.Setup(m =>
                m.Register(It.IsAny<TRecipient>(), It.IsAny<MessageHandler<TRecipient, TMessage>>()))
                .Verifiable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <typeparam name="TRecipient"></typeparam>
        /// <param name="mock"></param>
        /// <param name="times"></param>
        public static void VerifyMessageRegistrationTimes<TMessage, TRecipient>(Mock<IMessengerService> mock, Times times)
            where TRecipient : class
            where TMessage : class
        {
            mock.Verify(m => m.Register(
                It.IsAny<TRecipient>(), It.IsAny<MessageHandler<TRecipient, TMessage>>()), times);
        }
    }
}