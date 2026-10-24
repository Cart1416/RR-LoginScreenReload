using System;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LoginScreenReload
{
    [BepInPlugin("com.recroom.loginscreenreload", "LoginScreenReload", "1.0.0")]
    public class LoginScreenReloadPlugin : BasePlugin
    {
        internal static new ManualLogSource Log;
        private Harmony _harmony;

        // The login / title screen scene name (from the game's scene list).
        internal const string LoginSceneName = "reccenter";

        public override void Load()
        {
            Log = base.Log;
            Log.LogInfo("LoginScreenReload v1.0.0 loading...");

            _harmony = new Harmony("com.recroom.loginscreenreload");
            _harmony.PatchAll();

            Log.LogInfo("LoginScreenReload patched. Press F1 on the login screen to reload it.");
        }

        public override bool Unload()
        {
            _harmony.UnpatchSelf();
            Log.LogInfo("LoginScreenReload unpatched.");
            return true;
        }

        internal static bool IsOnLoginScreen()
        {
            try
            {
                var scene = SceneManager.GetActiveScene();
                return scene.name == "TitleScreen";
            }
            catch (Exception ex)
            {
                Log?.LogWarning($"Failed to read active scene: {ex.Message}");
                return false;
            }
        }

        internal static void ReloadLoginScreen()
        {
            Log.LogInfo("Reloading login screen...");
            SceneManager.LoadScene(LoginSceneName, LoadSceneMode.Single);
        }

        // TitleScreenManager.Update only runs while the login/title screen scene is active,
        // so hooking it gives us a per-frame callback that exists exactly on the login screen.
        [HarmonyPatch(typeof(TitleScreenManager), "Update")]
        private static class TitleScreenUpdatePatch
        {
            // Prevents the reload from immediately re-triggering while the scene is still coming up.
            private static float _nextAllowedReloadTime;

            private static void Postfix()
            {
                try
                {
                    if (!Input.GetKeyDown(KeyCode.F1))
                        return;

                    if (!IsOnLoginScreen())
                        return;

                    if (Time.time < _nextAllowedReloadTime)
                        return;

                    _nextAllowedReloadTime = Time.time + 1f;
                    ReloadLoginScreen();
                }
                catch (Exception ex)
                {
                    Log?.LogError($"LoginScreenReload error: {ex}");
                }
            }
        }
    }
}
