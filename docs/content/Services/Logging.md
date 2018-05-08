RocketMod 5 uses ILogger service for logging.
Add it to your plugin like this:

```cs
public class MyPlugin : Plugin
{
    public MyPlugin(IDependencyContainer container, ILogger logger) : base(container)
    {
        this.logger = logger;
        logger.LogInformation("Hello world!");
    }
}
```

To register your own logger, see [[Services]].