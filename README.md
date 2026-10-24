yay ai readme
btw you need to put BepInEx files in the BepInEx folder to build
# LoginScreenReload

BepInEx (IL2CPP) plugin for Rec Room. Pressing **F1** while on the login screen reloads the login screen (`TitleScreen` scene).

## How it works
- Harmony-patches `TitleScreenManager.Update` (only runs on the login screen).
- On `F1` key-down, it calls `SceneManager.LoadScene("TitleScreen", Single)` with a 1-second cooldown to avoid re-triggering during scene load.

## Build
```bash
dotnet build
```
Copy `bin/Debug/net6.0/LoginScreenReload.dll` to `BepInEx/plugins/`.
