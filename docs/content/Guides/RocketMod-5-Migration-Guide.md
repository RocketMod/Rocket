When migrating to RocketMod 5, you should first reference the NuGet packages: Rocket.API, Rocket.Core and (if your plugin depends on Rocket.Unturned features or the Unturned dlls) Rocket.Unturned.
You will have to reference UnityEngine.dll yourself as we can not provide it because of licensing issues.

Please read [[Services]] before continuing.

After that, change `<MyPlugin> : RocketPlugin<X>` part of your main plugin file to  `<MyPlugin> : Plugin`.

Now update your API usage, below you can see what has changed.

# Common API Changes

| **Old API**                                     | **New API**                             |
|-------------------------------------------------|-----------------------------------------|
| UnturnedChat                                    | [[ChatManager]]                         |
| RocketPlugin<>                                  | Plugin                                  |
| IRocketCommand.Execute(IRocketPlayer, string[]) | IRocketCommand.Execute(ICommandContext) |
| R.Translation                                   | [[Translations]]                        |
| Logger.Debug / Logger.Info ...                  | [[Logging]]                             |
| Any event (OnXXXX) (e.g. OnPlayerConnected)     | [[Eventing]]                            |
| IRocketPermissionsProvider                      | [[Permissions]]                         |
| R and U                                         | [[Services]]                            |
| Plugin.Configuration                            | [[Configurations]]                      |
| Update(), FixedUpdate(), etc...                 | [[Scheduling]]                          |

# MonoBehaviours
If you want to make true universal plugins, you should avoid using MonoBehaviour under all circumstances. You can use the [[TaskScheduler | Scheduling]] instead. Only use MonoBehaviours if you need to add components which need access to other component data and hooks (e.g. OnCollision, Rigidbodies, etc).

# Permission changes
For better provider integrations `bool IRocketPermissionsProvider.HasPermission(IRocketPlayer, List<string>)` has been changed to `PermissionResult IRocketPermissionDataProvider.HasPermission(IRocketPlayer, string)`. PermissionResult can return "Grant" (explicit allow), "Deny" (explicit disallow) or "Default" (not set; should be handled the same as "Deny" but is up to the plugin using it).

Permission plugins will have to implement a provider extending `IPermissionManager` and register it.