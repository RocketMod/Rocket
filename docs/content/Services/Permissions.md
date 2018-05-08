> **Important: The examples in here may be outdated, feel free to help us keeping them up to date, you can simply edit them if you are logged in on GitHub.**

***
## Basics
Rocket handles permissions in the file 
```
Unturned\<Instance>\Rocket\Permissions.config.xml
```

This file contains basic options to customize the permission features, groups with permissions and assignments and a whitelist for groups.

This is how a permission file could look like:

```xml
<?xml version="1.0" encoding="utf-8"?>
<RocketPermissions xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <DefaultGroup>default</DefaultGroup>
  <Groups>
    <Group>
      <Id>default</Id>
      <DisplayName>Guest</DisplayName>
      <Prefix />
      <Suffix />
      <Color>white</Color>
      <Members />
      <Permissions>
        <Permission Cooldown="0">p</Permission>
        <Permission Cooldown="0">compass</Permission>
        <Permission Cooldown="0">rocket</Permission>
      </Permissions>
    </Group>
    <Group>
      <Id>vip</Id>
      <DisplayName>VIP</DisplayName>
      <Prefix>[VIP]</Prefix>
      <Suffix>[VIP]</Suffix>
      <Color>FF9900</Color>
      <Members>
        <Member>76561198016438091</Member>
      </Members>
      <ParentGroup>default</ParentGroup>
      <Permissions>
        <Permission Cooldown="0">effect</Permission>
        <Permission Cooldown="120">heal</Permission>
        <Permission Cooldown="30">v</Permission>
      </Permissions>
    </Group>
  </Groups>
</RocketPermissions>
```

## Detailed explanations

### DefaultGroup
The Id of the group that guests are in.

### Groups
**Id**: The internal name of this group

**Displayname**: The display name of this group shown in the chat (Not implemented yet)

**Color**: A specified color in the chat (Formats: HTML color codes and Unity3D color names)

**Members**: List of CSteamIDs aka. STEAMID64

**Permissions**: List of permissions this group can execute

* **Permission Cooldown**: Cooldown in seconds between executing a specified command (Must be positive!)

**ParentGroup**: List of groups from which the permissions will be taken

**Prefix**: Text in front of a Player's name

**Suffix**: Text after a Player's name