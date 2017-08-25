## RocketPlugin
> If you want to use a configuration, choose RocketPlugin&lt;IRocketConfiguration&gt;

```csharp
public class MyFancyPlugin : RocketPlugin {

	public static MyFancyPlugin Instance;
	private bool messageSent = false;
	
	protected override void Load() {
		Instance = this;
	}
	
	protected override void Unload(){
		//Clean up your mess before the plugin is unloaded
	}
	
	private void FixedUpdate() {
		if (this.State == PluginState.Loaded && !this.messageSent) {
			UnturnedChat.Say(Translate("myfancyplugin_message"));
			messageSent = true;
		}
	}
	
	private override TranslationList DefaultTranslations{
		get
		{
			return new TranslationList(){
				{"myfancyplugin_message","Hi Dave."}
			};
		}
	}
}
```

#### Properties
Name | Type | Details
---------- | ---------- | ----------
State | Rocket.API.PluginState | This getter will return the state of plugin, possible values are (Cancelled, Failure, Loaded and Unloaded)
DefaultTranslations | Rocket.API.Collections.TranslationList | A getter for default translations

## RocketPlugin&lt;IRocketConfiguration&gt;
> Specify a implementation of the interface IRocketConfiguration as type parameter

```csharp
public class MyFancyPlugin : RocketPlugin<MyFancyConfiguration> {

	public static MyFancyPlugin Instance;
	private bool messageSent = false;
	
	protected override void Load() {
		Instance = this;
	}
	
	protected override void Unload(){
		//Clean up your mess before the plugin is unloaded
	}
	
	private void FixedUpdate() {
		if (this.State == PluginState.Loaded && !this.messageSent && Configuration.ShowConfiguration) {
			UnturnedChat.Say(Translate("myfancyplugin_message",42));
			messageSent = true;
		}
	}
	
	private override TranslationList DefaultTranslations{
		get
		{
			return new TranslationList(){
				{"myfancyplugin_message","The number is: {0}"}
			};
		}
	}
}
```

<aside class="notice">
Check <a href="#irocketconfiguration">IRocketConfiguration Interface example</a> for details on how to create a configuration class 
</aside>

#### Properties
Name | Type | Details
---------- | ---------- | ----------
Configuration | &lt;IRocketConfiguration&gt; | Contains the values from your configuration
