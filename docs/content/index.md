With RocketMod 5 we redesigned the core to have all its functionalities completely modular. Interface behaviors - we are calling them "Providers" or "Services" are defined and our own implementations are created first. There will be a loading mechanism to allow customizing and extending every bit of it. RocketMod uses the Unity IoC framework to achieve this. For more, visit [[Services]].

RocketMod 5 introduces universal plugins. RocketMod 5 will support multiple games, including Unturned 3 being done and Eco being in development. We also consider supporting Unturned 4 in the future. Universal plugins only reference Rocket.API and Rocket.Core and can be run on any game that supports RocketMod. Our abstraction layer allows anyone to easily do this.

Classical plugins still exist, so you can still make plugins which use and reference Unturned features, but these plugins will likely not run on other platforms / games.
