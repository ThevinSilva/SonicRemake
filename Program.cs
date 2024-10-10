using SFML.Graphics;
using SFML.Window;
using SonicRemake.Inputs;
using SonicRemake.Movement;
using Arch.Core;
using SonicRemake.Components;
using Sprite = SonicRemake.Components.Sprite;
using SFML.System;
using Transform = SonicRemake.Components.Transform;
using SonicRemake.Systems;
using System.Collections.Immutable;
using SonicRemake.Animations;
using SonicRemake.Systems.Rendering;
using SonicRemake.Systems.Rendering.Textures;
using SonicRemake.Systems.Rendering.Debugging;
using SonicRemake.Systems.Rendering.Animations;

AnimationHelper.LoadAnimationsFromYaml("Assets/Animations/sonic_mania.yaml");

// Limit the delta time to 120fps
const float deltaTimeLimit = 1.0f / 120.0f;

const float physicsTimeStep = 1.0f / 40.0f;
float physicsTimeAccumulator = 0.0f;

const float animationTimeStep = 1.0f / 60.0f;
float animationTimeAccumulator = 0.0f;

// var inputs = new Inputs();

var window = new RenderWindow(new VideoMode(1920, 1080), "Sonic");

var world = World.Create();

var clock = new Clock();
clock.Restart();

ImmutableList<GameSystem> systems = [
	new RenderSystem(),
	new TextureLoaderSystem(),
	new FpsDebugSystem(),
	new AnimationSystem(),
	new AnimationLoadSystem(),
 	new Movement()
];

// Create Sonic
world.Create(
		new Transform(new Vector2f(1920 / 2, 1080 / 2), new Vector2f(1, 1)),
		new Velocity(),
		new Sprite("sonic_mania.png", new Color(0, 240, 0), new Color(0, 170, 0), new Color(0, 138, 0), new Color(0, 111, 0)),
		new SpriteSheet(1, 13, 48),
		new Animator(),
		new AnimationQueue(new Queue<string>(["peelout"])),
		new Renderer(),
		new Sonic()
	);

// Run OnStart for all systems
systems.ForEach(system => system.OnStart(world));

while (window.IsOpen)
{
	float deltaTime = clock.Restart().AsSeconds();
	physicsTimeAccumulator += deltaTime;
	animationTimeAccumulator += deltaTime;

	// Build the context that will be passed to all systems
	var context = new GameContext
	{
		DeltaTime = deltaTime,
		PhysicsDeltaTime = physicsTimeAccumulator,
		AnimationDeltaTime = animationTimeAccumulator
	};

	// Handle window events
	window.DispatchEvents();

	// Close the window if the window wants to be closed
	window.Closed += (sender, e) => window.Close();

	// Clear window
	window.Clear();

	// Run OnRender for all systems
	systems.ForEach(system => system.OnRender(world, window, context));

	// Display frame
	window.Display();

	// Run OnAnimation for all systems if enough time has passed
	while (animationTimeAccumulator >= animationTimeStep)
	{
		systems.ForEach(system => system.OnAnimation(world, window, context));
		animationTimeAccumulator -= animationTimeStep;
	}

	// Run OnPhysics for all systems if enough time has passed
	while (physicsTimeAccumulator >= physicsTimeStep)
	{
		systems.ForEach(system => system.OnPhysics(world, context));
		physicsTimeAccumulator -= physicsTimeStep;
	}

	// Limit the delta time to 120fps
	if (deltaTime < deltaTimeLimit)
	{
		Thread.Sleep((int)((deltaTimeLimit - deltaTime) * 1000));
	}
}
