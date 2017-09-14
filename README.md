# UnityPrjStructure
A standard unity project directory structure.

## Directory Description
* 3rdParty--------All 3rd party packages
* ArtAssets-------All artists-made assets
    + Audio
        - Music
        - Sfx
    + Effects-----Particle effects
    + Gui
    + Models------All models/animations/textures here
* GameModules-----Program code for game modules
* Materials-------Common use(not artist-made) mat
    + Shaders
* Plugins
* Prefabs---------Prefabs should be made by designers.
                  Try to prefab everything you put on scenes
* Scenes
* StreamingAssets
* TestingGround---Put all your test assets(code/scene/other) here
    + Editor

## Coding Convention
| IDENTIFIER | CASE | EXAMPLE 
|------------|------|-----------
| Namespace | Pascal | System.Drawing
| Interface | Pascal with prefix I | IDisposal
| Class | Pascal | AppDomain
| Exception class | Pascal | WebException
| Enumeration types | Pascal | ErrorLevel
| Enumeration values | Pascal | FileError
| Event | Pascal | ValueChange
| Method | Pascal | ToString
| Readonly static field/Const | Pascal | RedValue
| Private (static) field | Camel with prefix _ | _myVal
| Public (static) field/Property | Pascal | BackColor
| Parameter/Local variables | Camel | localVar

## 3rd Frameworks/Packages
* [Zenject](https://github.com/modesttree/Zenject)----------a IOC/DependencyInjection framework
* [UniRx](https://github.com/neuecc/UniRx)------------Reactive Extention for Unity3d
* [Entitas CSharp](https://github.com/sschmid/Entitas-CSharp) - entity component system framework, but I think it is a little overdesigned.
* [Node Editor](https://github.com/Baste-RainGames/Node_Editor) - (calculation-)node editor
* [Fungus](https://github.com/snozbot/fungus) - 2d interactive storytelling game framework

and more packages [here](https://github.com/wcwsoft/Unity-Script-Collection).