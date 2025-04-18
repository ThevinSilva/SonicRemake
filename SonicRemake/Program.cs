﻿using SFML.Graphics;
using SFML.Window;
using SonicRemake.Inputs;
using SonicRemake.Components;
using Sprite = SonicRemake.Components.Sprite;
using SFML.System;
using Transform = SonicRemake.Components.Transform;
using SonicRemake.Systems;
using SonicRemake.Animations;
using SonicRemake.Systems.Rendering;
using SonicRemake.Systems.Rendering.Textures;
using SonicRemake.Systems.Rendering.Animations;
using SonicRemake.Systems.Rendering.Characters;
using SonicRemake.Systems.Rendering.Camera;
using SonicRemake.Levels;
using SonicRemake.Systems.Sensor;
using SonicRemake.Common;
using SonicRemake.Systems.Maps;
using SonicRemake.Systems.Rendering.Debugging;

AnimationHelper.LoadAnimationsFromYaml("Assets/Animations/sonic_mania.yaml");

var gameLevel = new Level("Game");
gameLevel.AddSystems(new TileManagementSystem(),
    new TextureLoaderSystem(),
    new RectangleLoaderSystem(),
    new SensorSystem(),
    new MovementSystem(),
    new AnimationSystem(),
    new AnimationLoadSystem(),
    new SonicAnimationSystem(),
    new AnimationSequenceSystem(),
    new CameraSystem(),
    new LineLoaderSystem(),
    new SensorDebug(),
    new SolidTilesDebugSystem(),
    // new GridDebugSystem(),
    new RenderSystem(),
    //new LogDebugSystem(),
    new FpsDebugSystem(),
    new SpeedometerDebugSystem(),
    new UiRenderSystem()
//	new MenuSystem()
);

gameLevel.Entities.Create(
        new Transform(new Vector2f(0, 0), new Vector2f(1, 1)),
        new Velocity(),
        new Sprite(TextureHelper.CreateHandle("sonic_mania.png", new Color(0, 240, 0), new Color(0, 170, 0), new Color(0, 138, 0), new Color(0, 111, 0))),
        new SpriteSheet(1, 13, 48),
        new SpriteAnimator(),
        new SpriteAnimation(),
        new AnimationSequence([]),
        new Renderer(Layer.Characters),
        new Sonic(),
        new Sensors(),
        new SolidTiles() // TODO:  Empty component for the Map System to work
    );

gameLevel.Entities.Create(
    new Transform(new Vector2f(0, 0), new Vector2f(1, 1)),
    new Camera(4f)
);


const float tickTimeStep = 1.0f / 60.0f;
float tickTimeStepAccumulator = 0.0f;

var window = new RenderWindow(new VideoMode(800, 400), "Sonic Remake");
window.SetFramerateLimit(120);

var hudHandle = TextureHelper.CreateHandle("hud.png", new Color(147, 187, 236));
var iconHandle = TextureHelper.CreateHandle(hudHandle, 1, 243, 18, 18);
var icon = TextureHelper.FromHandle(iconHandle).CopyToImage();

window.SetIcon(icon.Size.X, icon.Size.Y, icon.Pixels);

var clock = new Clock();
clock.Restart();

LevelManager.LoadLevel(gameLevel);

// sandbox.Entities.Create(
// 	new Transform(new Vector2f(0, 28 * 16), new Vector2f(1, 1)),
// 	new Renderer(),
// 	new Sprite("Green Hill Zone/Scene1-BG Outside.png")
// );

while (window.IsOpen)
{
    float deltaTime = clock.Restart().AsSeconds();
    tickTimeStepAccumulator += deltaTime;

    // Build the context that will be passed to all systems
    var context = new GameContext
    {
        DeltaTime = deltaTime,
        TickDeltaTime = tickTimeStep,
    };

    // Handle window events
    window.DispatchEvents();

    // Close the window if the window wants to be closed
    window.Closed += (sender, e) => window.Close();

    window.Resized += (sender, e) => window.SetView(new View(new FloatRect(0, 0, e.Width, e.Height)));

    // Clear window
    window.Clear();

    // Check if a new level has been loaded
    if (LevelManager.HasLevelChanged())
        foreach (var system in LevelManager.Active.Systems)
            system.Start(LevelManager.Active.Entities);

    // Run OnRender for all systems
    foreach (var system in LevelManager.Active.Systems)
        system.Render(LevelManager.Active.Entities, window, context);

    // Display frame
    window.Display();

    // Run OnPhysics for all systems if enough time has passed
    while (tickTimeStepAccumulator >= tickTimeStep)
    {
        if (window.HasFocus())
            Input.UpdateInputState();

        foreach (var system in LevelManager.Active.Systems)
            system.Tick(LevelManager.Active.Entities, context);
        tickTimeStepAccumulator -= tickTimeStep;
    }
}
