With RocketMod it is very easy to implement your own game:
First import Rocket.API, Rocket.Core, Rocket.Runtime and (if you use .NET 3.5) Rocket.Compability from NuGet.
If you use UnityEngine or UnrealEngine, make sure you also import Rocket.UnityEngine or Rocket.UnrealEngine.

Note: if you use Rocket.UnityEngine, you have to download and reference UnityEngine.dll yourself. Due licensing issues, we can not provide that file.

The following services are expected to be implemented (see [[Services]] for registering services using IDependencyRegistrator):
* ITaskScheduler //This is not needed if you use UnityEngine or UnrealEngine packages
* IChatManager
* IPlayerManager
* IImplementation

The following events **must** be implemented:
* ImplementationReadyEvent

When your assembly loads, call `Runtime.Bootstrap()`. If everything works, your `Implementation.Load()` will be called.
You need to load plugins from each `IPluginManager` at this point using `IPluginManager.Init()`.

You can visist [Rocket.Unturned](https://github.com/RocketMod/Rocket.Unturned) for an example implementation.
