# Rocket.Unturned
## UnturnedPlayer

An extension for most used player info and methods.

```csharp
// Run the command.
public void Execute(IRocketPlayer caller, string[] command) {
	if (caller is ConsolePlayer) {
		Logger.Log("This command cannot be called from the console.");
		return;
	}
	if (command.Length > 0) {
		UnturnedChat.Say(caller, "This command is only for yourself.");
		return;
	}
	UnturnedPlayer pCaller = (UnturnedPlayer)caller;
	// Check if Admin
	if (pCaller.IsAdmin && pCaller.Equals(UnturnedPlayer.FromName(command[0]))) {
		pCaller.Features.GodMode = true;
		pCaller.Features.VanishMode = true;
		pCaller.Infection = 0;
		pCaller.Hunger = 0;
		pCaller.Thirst = 0;
		pCaller.Heal(100, true, true);
		// In UnturnedChat, both caller and pCaller will work.
		UnturnedChat.Say(caller, String.Format("You are an Admin {0} [{1}], and now a vanished god at full health.", pCaller.CharacterName, pCaller.SteamName));
	} else {
		UnturnedChat.Say(caller, String.Format("For trying to call this admin-only command, you get more zombies {0} [{1}]!", pCaller.CharacterName, pCaller.SteamName));
		pCaller.Kick("Bad dog.");
	}
}
```

####Properties

Name | Type | Details | get or set
---------- | ---------- | ---------- | ----------
Bleeding | bool | Player is bleeding | get and set
Broken | bool | Player's broken bones | get and set
CharacterName | string | Player's Character name | get only
CSteamID | CSteamID | Player's Steam ID | get only
Dead | bool | Player is dead | get only
Events | UnturnedPlayerEvents | A getter for "player" events | get only
Experience | uint | Player's Experience | get and set
Features | RocketPlayerFeatures | Player's Features component | get only
Freezing | bool | Player is freezing | get only
Health | byte | Player's Health | get only
Hunger | byte | Player's Hunger | get and set
Infection | byte | Player's Infection level | get and set
Inventory | PlayerInventory | Player's Inventory | get only
IsAdmin | bool | true if admin, false otherwise | get only
IsPro | bool | true if the player has gold account | get only
Ping | float | the ping of the player | get only
Player | Player | SDG.Unturned.Player Instance | get only
Position | Vector3 | Player's position | get only
Rotation | float | Direction player is looking | get only
Stamina | byte | Player's current stamina | get only
Stance | EPlayerStance | Player's current stance | get only
SteamGroupID | CSteamID | Player's Group ID | get only
SteamName | string | Player's Steam name | get only
Thirst | byte | Player's Thirst | get and set

####Methods

Name | Parameters | Output | Description
---------- | ---------- | ---------- | ----------
Admin | bool, UnturnedPlayer=null | none | Admins player if true, unadmins if false [caller (UnturnedPlayer)]
Ban | string, uint | none | Bans player for reason (string) and seconds (uint)
Damage | byte, Vector3, EDeathCause, ELimb, CSteamID | EPlayerKill  | Sends to the server a damage of the player
Equals | UnturnedPlayer | bool | Check if two UnturnedPlayers are equal
FromCSteamID | CSteamID | UnturnedPlayer | Gets UnturnedPlayer instance from CSteamID, null if not found
FromName | string | UnturnedPlayer | Get UnturnedPlayer instance from given CSteamID or player name (in a string), null if not found
FromPlayer | SDG.Unturned.Player | UnturnedPlayer | Get UnturnedPlayer instance, null if not found
FromSteamPlayer | SteamPlayer | UnturnedPlayer | Get RocketPlayer instance, null if not found
GetComponent<T> | Component | Component | Finds the instance of component type T
GiveItem | SDG.Unturned.Item | bool | Gives player an item, success returns true
GiveItem | ushort, byte | bool | Gives player an item of itemid (ushort) and its amount (byte), success returns true
GiveVehicle | ushort | bool | Gives player a vehicle of id (ushort), success returns true
Heal | byte, bool?=null, bool?=null | none | Heals player by amount (byte), bleeding, and broken (bool? x2)
Kick | string | none | Kicks player for reason (string)
Suicide | none | none | Causes player to suicide
Teleport | UnturnedPlayer | none | Teleports player to another player (UnturnedPlayer)
Teleport | Vector3, float | none | Teleports player to position (Vector3) facing (float)
Teleport | string | bool | Teleports player to map node (string), success returns true
TriggerEffect | ushort | none | Triggers an effect on the player

##UnturnedPlayerFeatures

A set of features for players.  This is a sealed class and should be accessed through UnturnedPlayer.

#### Properties
Name | Type | Default | get/set
---------- | ---------- | ---------- | ----------
VanishMode | bool | false | get and set
GodMode | bool | false | get and set

## UnturnedPlayerComponent
> To use, your class should inherit from this component

```csharp
public class MyFancyPlayerComponent : UnturnedPlayerComponent {

	public bool HasKitten;
	
	private void Start() {
		this.HasKitten = true;
	}
	
	private void FixedUpdate() {
		if (this.HasKitten) {
			UnturnedChat.Say(this.Player, MyFancyPlugin.Instance.Translate("myfancyplugin_message", 12));
		}
	}
}
```

Applied to all players when they connect and removed when they disconnect (or plugin unloaded).

#### Fields
Name | Type | Default
---------- | ---------- | ----------
Player | UnturnedPlayer | Current UnturnedPlayer instance

<aside class="notice">
Include Rocket.Core.Player in your references.
</aside>

## UnturnedChat

```csharp
UnturnedChat.Say(caller, "Message about how fancy I am here.");
UnturnedChat.Say(caller.CSteamID, "Message about how fancy I am here.");
UnturnedChat.Say("Broadcast about how fancy Rocket is.");
```

Used to send or broadcast messages in chat.

#### Methods
Name | Parameters | Output
---------- | ---------- | ----------
GetColorFromHex | string | Returns a new instance of UnityEngine.Color with the given hexadecimal color
GetColorFromName | string, UnityEngine.Color | Returns an instance of UnityEngine.Color with the given color name. On fail, the given UnityEngine.Color will be returned.
GetColorFromRGB | byte, byte, byte, byte=255 | Returns a new instance of UnityEngine.Color with the given RGBA (Red, Green, Blue and Alpha) color
Say | string [, UnityEngine.Color] | Broadcasts message to the server with a given color (optional)
Say | CSteamID, string [, UnityEngine.Color] | Sends a private message to the given CSteamID with a given color (optional)
Say | IRocketPlayer, string [, UnityEngine.Color] | Sends a private message to the given IRocketPlayer with a given color (optional)
wrapMessage | string | Wraps the given string and returns List&lt;string&gt; (wraps after 90 characters)

## UnturnedEvents

```csharp
protected override Load()
{
	U.Events.OnPlayerConnected += (UnturnedPlayer player) =>
	{
		UnturnedChat.Say(player, "Welcome to my server!");
		UnturnedChat.Say(player.CharacterName + " has joined the server!");
	}
	U.Events.OnPlayerDisconnected += (UnturnedPlayer player) =>
	{
		UnturnedChat.Say(player.CharacterName + " has disconnected from the server!", Color.Red);
	}
	U.Events.OnShutdown += () =>
	{
		Logger.Log("rip");
	}
}
```

Unturned events.

#### Events
Name | Parameters | Occurrence
---------- | ---------- | ----------
OnBeforePlayerConnected | UnturnedPlayer | When the player starts the connection with the server
OnPlayerConnected | UnturnedPlayer | When the player's game has completely loaded
OnPlayerDisconnected | UnturnedPlayer | When the player disconnects from the server
OnShutdown | none | When the server shuts down

<aside class="notice">
You need to use the instance of UnturnedEvents class which can be found in Rocket.Unturned.U.Events.
</aside>

## UnturnedPlayerEvents

```csharp
protected override void Load()
{
	UnturnedPlayerEvents.OnPlayerChatted += (UnturnedPlayer player, ref Color color, string message, EChatMode chatMode) =>
	{
		File.AppendAllText("mylogs.txt", String.Format("[CHAT] [{0}] {1}[{2}]: {3}", chatMode.ToString(), player.CharacterName, player.SteamName, message));
	};
}
```

Unturned Player events.

#### Non-Static Events
Name | Parameters | Occurrence
---------- | ---------- | ----------
OnDead | UnturnedPlayer, Vector3 | When a player is dead with the given position (Vector3)
OnDeath | UnturnedPlayer, EDeathCause, ELimb, CSteamID | When a player dies with the given dead player (UnturnedPlayer), the cause of the death (EDeathCause), Which limb caused the death (ELimb) and the murderer's CSteamID
OnInventoryAdded | UnturnedPlayer, InventoryGroup, byte, ItemJar | When an item is added to the player's inventory
OnIntentoryRemoved | UnturnedPlayer, InventoryGroup, byte, ItemJar | When an item is removed from the player's inventory
OnInventoryResized | UnturnedPlayer, InventoryGroup, byte, byte | When the player's inventory is resized
OnInventoryUpdated | UnturnedPlayer, InventoryGroup, byte, ItemJar | When an item is updated in the player's inventory
OnRevive | UnturnedPlayer, Vector3, byte | When the player revives at the given position (Vector3)
OnUpdateBleeding | UnturnedPlayer, bool | When the player's bleeding status is updated
OnUpdateBroken | UnturnedPlayer, bool | When the player's broken status is updated
OnUpdateExperience | UnturnedPlayer, uint | When the player's experience value is updated
OnUpdateFood | UnturnedPlayer, byte | When the player's food value is updated
OnUpdateGesture | UnturnedPlayer, PlayerGesture | When the player's gesture is updated
OnUpdateHealth | UnturnedPlayer, byte | When the player's health value is updated
OnUpdateLife | UnturnedPlayer, byte | When the player's life value is updated
OnUpdatePosition | UnturnedPlayer, Vector3 | When the player's position is updated
OnUpdateStamina | UnturnedPlayer, byte | When the player's stamina value is updated
OnUpdateStance | UnturnedPlayer, byte | When the player's stance value is updated
OnUpdateStat | UnturnedPlayer, EPlayerStat | When the player updates his stats
OnUpdateVirus | UnturnedPlayer, byte | When the player's virus value is updated
OnUpdateWater | UnturnedPlayer, byte | When the player's water value is updated

<aside class="notice">
To use the non-static events, you need to access them through a player's instance of UnturnedPlayer.Events
</aside>

#### Static Events
Name | Parameters | Occurrence
---------- | ---------- | ----------
OnPlayerChatted | UnturnedPlayer, ref Color, string, EChatMode | When the player sends a chat message to the server with the given message (string) and the chat mode (EChatMode)
OnPlayerDead | UnturnedPlayer, Vector3 | When a player is dead with the given position (Vector3)
OnPlayerDeath | UnturnedPlayer, EDeathCause, ELimb, CSteamID | When a player dies with the given dead player (UnturnedPlayer), the cause of the death (EDeathCause), Which limb caused the death (ELimb) and the murderer's CSteamID
OnPlayerInventoryAdded | UnturnedPlayer, InventoryGroup, byte, ItemJar | When an item is added to the player's inventory
OnPlayerIntentoryRemoved | UnturnedPlayer, InventoryGroup, byte, ItemJar | When an item is removed from the player's inventory
OnPlayerInventoryResized | UnturnedPlayer, InventoryGroup, byte, byte | When the player's inventory is resized
OnPlayerInventoryUpdated | UnturnedPlayer, InventoryGroup, byte, ItemJar | When an item is updated in the player's inventory
OnPlayerRevive | UnturnedPlayer, Vector3, byte | When the player revives at the given position (Vector3)
OnPlayerUpdateBleeding | UnturnedPlayer, bool | When the player's bleeding status is updated
OnPlayerUpdateBroken | UnturnedPlayer, bool | When the player's broken status is updated
OnPlayerUpdateExperience | UnturnedPlayer, uint | When the player's experience value is updated
OnPlayerUpdateFood | UnturnedPlayer, byte | When the player's food value is updated
OnPlayerUpdateGesture | UnturnedPlayer, PlayerGesture | When the player's gesture is updated
OnPlayerUpdateHealth | UnturnedPlayer, byte | When the player's health value is updated
OnPlayerUpdateLife | UnturnedPlayer, byte | When the player's life value is updated
OnPlayerUpdatePosition | UnturnedPlayer, Vector3 | When the player's position is updated
OnPlayerUpdateStamina | UnturnedPlayer, byte | When the player's stamina value is updated
OnPlayerUpdateStance | UnturnedPlayer, byte | When the player's stance value is updated
OnPlayerUpdateStat | UnturnedPlayer, EPlayerStat | When the player updates his stats
OnPlayerUpdateVirus | UnturnedPlayer, byte | When the player's virus value is updated
OnPlayerUpdateWater | UnturnedPlayer, byte | When the player's water value is updated
OnPlayerWear | UnturnedPlayer, UnturnedPlayerEvents.Wearables, ushort, byte? | When the player wears an item

## UnturnedPermissions

```csharp
// For example, we have a list of banned SteamIDs in a List<ulong>.
List<ulong> bannedids = new List<ulong>() {  };
UnturnedPermissions.OnJoinRequested += (Steamworks.CSteamID player, ref ESteamRejection? rejectionReason) =>
{
	if (bannedids.Contains(player.m_SteamID)){
		Logger.Log("Banned SteamID (" + player.m_SteamID.ToString() + ") has attempted to connect.");
		rejectionReason = ESteamRejection.FAILED_AUTHENTICATION;
	}
};
```

#### Events
Name | Parameters | Occurrence
---------- | ---------- | ----------
OnJoinRequested | CSteamID, ref ESteamRejection? | When the player requests a join request from the server, connection can be rejected by assigning a reason to ESteamRejection, or null if allowed
OnPermissionRequested | UnturnedPlayer, string, ref bool | When the player requests a permission, with the given permission name (string). Permission can be granted by setting the permissionGranted (bool) to true, false for the opposite

## UnturnedItems

```csharp
using Rocket.Unturned.Items;
using SDG.Unturned;
public static bool GiveMahItem(UnturnedPlayer p)
{
	if(p==null) return false;
	return p.GiveItem(UnturnedItems.AssembleItem(363,
		30, // clipsize
		new Attachment(22, 100), // sight
		new Attachment(151, 100), // tactical
		new Attachment(8, 100), // grip
		new Attachment(7, 100), // barrel
		new Attachment(17, 100), // magazine
		EFiremode.AUTO, // firemode
		1, 100 // amount, durability
	));
}
```

#### Methods
Name | Parameters | Output
---------- | ---------- | ----------
AssembleItem | ushort, byte, byte, byte[] | Returns a new instance of an Item with the given itemid (ushort), amount, durability (byte x2) and the metadata (byte[])
AssembleItem | ushort, byte, Items.Attachment, Items.Attachment, Items.Attachment, Items.Attachment, Items.Attachment, EFiremode, byte, byte | Returns a new instance of an Item with the given itemid (ushort)<br/>Same as the overload above, but extending metadata into parameters clipsize, sight, tactical, grip, barrel, magazine and firemode (byte, Items.Attachment x5, EFiremode), with amount and durability (byte x2)
GetItemAssetById | ushort | Returns the ItemAsset of the given ItemID
GetItemAssetByName | string | Returns the ItemAsset of the given Item name
