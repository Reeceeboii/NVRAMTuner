## Services/Wrappers

This directory holds services that are aimed to provide wrappers around previously non-testable (or difficult to test) APIs.
This includes things like `System.Process` and `CommunityToolkit.Mvvm.Messaging`.

Creating interfaces around these namespaces and any of their exposed functionality that NVRAMTuner relies upon means that they can be registered in the IoC container (`ServiceContainer.cs`)
and injected into client classes. Subsequently, they can be mocked with `moq` within unit tests.
