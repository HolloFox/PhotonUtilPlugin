# Photon Util Plugin

This is a plugin for TaleSpire using BepInEx.

## Install

Go to the releases folder and download the latest and extract to the contents of your TaleSpire game folder.

## Usage

This util is designed to be the networking backbone of all talespire related mods.

## How to Compile / Modify

Open ```PhotonUtilPlugin.sln``` in Visual Studio.

You will need to add references to:

```
* BepInEx.dll  (Download from the BepInEx project.)
* Bouncyrock.TaleSpire.Runtime (found in Steam\steamapps\common\TaleSpire\TaleSpire_Data\Managed)
* UnityEngine.dll
* UnityEngine.CoreModule.dll
* UnityEngine.InputLegacyModule.dll 
* UnityEngine.UI
* Unity.TextMeshPro
```

Build the project.

Browse to the newly created ```bin/Debug``` or ```bin/Release``` folders and copy the ```PhotonUtilPlugin.dll``` to ```Steam\steamapps\common\TaleSpire\BepInEx\plugins```
