using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using InputPaths = Nautilus.Handlers.GameInputHandler.Paths;

namespace Ungeziefi.Camera_Zoom
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency("com.snmodding.nautilus")]
    public class Main : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "Ungeziefi.Camera_Zoom";
        public const string PLUGIN_NAME = "Camera Zoom";
        public const string PLUGIN_VERSION = "3.0.0";

        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
        internal static new ManualLogSource Logger { get; private set; }
        internal static new Config Config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();
        public static GameInput.Button PCZoomButton;
        public static GameInput.Button VCZoomButton;
        public static GameInput.Button CCZoomInButton;
        public static GameInput.Button CCZoomOutButton;
        public static GameInput.Button CDZoomInButton;
        public static GameInput.Button CDZoomOutButton;

        public void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");

            PCZoomButton = EnumHandler.AddEntry<GameInput.Button>("PCZoomButton")
                .CreateInput("Toggle player zoom")
                .WithKeyboardBinding(InputPaths.Mouse.MiddleButton)
                .WithControllerBinding(InputPaths.Gamepad.LeftStick)
                .WithCategory("Camera Zoom");

            VCZoomButton = EnumHandler.AddEntry<GameInput.Button>("VCZoomButton")
                .CreateInput("Toggle vehicle zoom")
                .WithKeyboardBinding(InputPaths.Mouse.MiddleButton)
                .WithControllerBinding(InputPaths.Gamepad.LeftStick)
                .WithCategory("Camera Zoom");

            CCZoomInButton = EnumHandler.AddEntry<GameInput.Button>("CCZoomInButton")
                .CreateInput("Cyclops cameras zoom in")
                .WithKeyboardBinding(InputPaths.Keyboard.Shift)
                .WithControllerBinding(InputPaths.Gamepad.ButtonNorth)
                .WithCategory("Camera Zoom");

            CCZoomOutButton = EnumHandler.AddEntry<GameInput.Button>("CCZoomOutButton")
                .CreateInput("Cyclops cameras zoom out")
                .WithKeyboardBinding(InputPaths.Keyboard.Ctrl)
                .WithControllerBinding(InputPaths.Gamepad.ButtonWest)
                .WithCategory("Camera Zoom");

            CDZoomInButton = EnumHandler.AddEntry<GameInput.Button>("CDZoomInButton")
                .CreateInput("Camera Drones zoom in")
                .WithKeyboardBinding(InputPaths.Keyboard.Shift)
                .WithControllerBinding(InputPaths.Gamepad.ButtonNorth)
                .WithCategory("Camera Zoom");

            CDZoomOutButton = EnumHandler.AddEntry<GameInput.Button>("CDZoomOutButton")
                .CreateInput("Camera Drones zoom out")
                .WithKeyboardBinding(InputPaths.Keyboard.Ctrl)
                .WithControllerBinding(InputPaths.Gamepad.ButtonWest)
                .WithCategory("Camera Zoom");

            Harmony.CreateAndPatchAll(Assembly, $"{PLUGIN_GUID}");
        }
    }
}