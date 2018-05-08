The default C# events used with old RocketMod 4 were very limited. It was not possible to have prioritized events. The new API allows controlling lifespan and execution order of events.

# Listening for events

## Listening with IEventListener<>
To listen to events, you will have to create a class which implements `IEventListener<YourTargetEvent>`. 
Don't forget to add `[EventHandler]` attribute to your method to set various options like priority.

You have to register your event listener with `EventManager.AddEventListener(YourPlugin, YourIListener)` on plugin load to register your events. 

### Example listener class

```cs
public class MyEventListener : IEventListener<PlayerConnectedEvent>, IEventListener<PlayerChatEvent>
{
    private IChatManager chatManager;
    public MyEventListener(IChatManager chatManager)
    {
        this.chatManager = chatManager;
    }

    [EventHandler]
    public void HandleEvent(IEventEmitter emitter, PlayerConnectedEvent @event)
    {
        chatManager.Broadcast(@event.Player.Name + " joined");
    }

    [EventHandler]
    public void HandleEvent(IEventEmitter emitter, PlayerChatEvent @event)
    { 
        if(ContainsBadWord(@event.Message))
        {
            @event.IsCancelled = true;
        }
    }
    
    public bool ContainsBadWord(string message)
            => return message.Contains("Trojaner");
}

public class MyPlugin : Plugin
{
    private readonly IChatManager chatManager;
    private readonly IEventManager eventManager;

    public HelloWorldPluginMain(IDependencyContainer container, IChatManager chatManager, IEventManager eventManager) : base("HelloWorldPlugin", container)
    {
        this.chatManager = chatManager;
        this.eventManager = eventManager;
    }

    protected override void OnLoad()
    {
        eventManager.AddEventListener(this, new MyEventListener(chatManager));
    }
}
```

**Note:** Execution order of events is like this:
`Lowest -> Low -> Normal -> High -> Highest -> Monitor`
You should only use monitor when it does not impact anything ingame, for example you can use it for logging purposes.

## Listening with Callbacks

## Event priorities and cancellation
EventHandlers can have priorities by using `[EventHandler(Priority = EventPriority.X)]`
The method with `Lowest` will be executed first, and `Monitor` will be executed last. `Monitor` should be only be used by listener which do not do any changes.
If any event listener decides to cancel the event before your event listener is called, your listener will not be called.
You can use `[EventHandler(IgnoreCancelled = true)]` to receive cancelled events. This also allows you to un-cancel events by setting `Event.IsCancelled` to false.

# Creating custom events (e.g. for other plugins)
Create a new class which extends `Rocket.API.Eventing.Event`. If you want to have it cancellable, add `ICancellableEvent` to it. 

```cs
public class MyEvent: Event, ICancellableEvent
{
        public string SomeData { get; set; }

        public MyEvent() : this(true)
        {

        }

        public MyEvent(bool global = true) : base(global)
        {
        }

        public MyEvent(EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(executionTarget, global)
        {
        }

        public MyEvent(string name = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(name, executionTarget, global)
        {
        }

        public bool IsCancelled { get; set; }
}
```

You can trigger your event like this:

```cs
MyEvent @event = new MyEvent();
@event.SomeData = data; 
eventManager.Emit(plugin, @event, 
    (sender, e) => {
       //event finished and all callbacks were called
       if(@event.IsCancelled) //if your event extends ICancellableEvent
          return;
       // do something with @event.SomeData        
    }); // plugins listening to events can change "SomeData"
```