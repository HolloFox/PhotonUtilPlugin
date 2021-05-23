# Photon Util Plugin (PUP)

Provides messaging between TaleSpire clients without a need of a private server for mods.

## Usage

The Photon Util Plugin is intended to be used as a dependency for other plugins. To use it as part of your own plugins, add a reference to the DLL into your Visual Studio project and then add the following to your plugin header:

```C#
    [BepInPlugin(Guid, "My Plugin That Uses The Photon Util Plugin", Version)]
    [BepInDependency(PhotonUtilPlugin.Guid)]
    public class MyNetworkedPlugin : BaseUnityPlugin
    {
        public const string Guid = "org.demo.plugins.mycooldemo";
        private const string Version = "1.1.0.0";
        
        void Awake()
        {
		// Run once on awake
        }
        
        void Update()
        {
		// Run every frame
        }
    }
```

This will give access to the PhotonUtilPlugin namespace at runtime and ensure that only one PhotonUtilPlugin instance handles all plugins that need it.

To be able to get or send messages, you register your mod. Typically this can be done in the Awake() function but specific situations may dictated otherwise. This can be done using:

```C#
    void Awake()
    {
        PhotonUtilPlugin.AddMod(Guid);
    }
```

To create a message you can isntantiate a new Photon Message that you'll use to send your content.

```C#
	var message = new PhotonMessage	
		{
        		PackageId = Guid,
                	Version = Version,
                	SerializedMessage = "your message here"
		};
```

There's currently 2 ways to send a message. 1 is using your ledger.
To add to your ledger you call the add message function after your mod has been added to the plugin.
```C#
	PhotonUtilPlugin.AddMessage(message);
```
This will be broadcasted for every other player to view.
To view everyone's ledgers you call GetMessages
```C#
	var AllMessage = PhotonUtilPlugin.GetMessages(Guid);
```
This will return a `Dictionary<PhotonPlayer, List<PhotonMessage>>` allowing you to go through the history of messages per person within the room.
If you're ledger is getting abit long you may call the clear function to clean it up.
```C#
	PhotonUtilPlugin.ClearNonPersistent(Guid);
```
This will clearup all messages that you deem to not persist.

The 2nd way is to Bind it to an instance, (This will usually be for a singleton you want to keep sync like a character sheet for a session) 

```C#
	// Create and Update, 
	PhotonUtilPlugin.CreateInstance("My Instance",message);
	PhotonUtilPlugin.UpdateInstance("My Instance",message);
	
	// Delete
	PhotonUtilPlugin.DeleteInstance(Guid,"My Instance");

	// Read
	var result = ReadInstance(Guid,"My Instance", PhotonPlayer player);
```
Creating, updating and deleting is relatively easy as these objects are relatively straight forwards.

Reading involves obtaining a player first and this can be done by:
```C#
	var players = PhotonNetwork.playerList;
```
This will return all players in the current "room" you are in ready to read their stored values for your mod's session.
