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

const float physicsTimeStep = 1.0f / 40.0f;

var inputs = new Inputs();
var movement = new Movement();

var window = new RenderWindow(new VideoMode(200, 200), "Sonic");

var world = World.Create();

var clock = new Clock();
clock.Restart();

ImmutableList<GameSystem> systems = [new RenderSystem(), new FpsDebugSystem()];


// Create Sonic
world.Create(
	new Transform(new Vector2f(0, 0), new Vector2f(1, 1), 0),
	new Velocity(0, 0),
	new Sprite("sonic.png"),
	new SpriteSheet(0, 192, 17),
	new Renderer()
);

// Run OnStart for all systems
systems.ForEach(system => system.OnStart(world));

float physicsTimeAccumulator = 0.0f;

while (window.IsOpen)
{
	float deltaTime = clock.Restart().AsSeconds();
	physicsTimeAccumulator += deltaTime;

	var context = new GameContext { DeltaTime = deltaTime, PhysicsDeltaTime = physicsTimeStep };

	// Handle window events
	window.DispatchEvents();

	// Clear window
	window.Clear();

	// Run OnRender for all systems
	systems.ForEach(system => system.OnRender(world, window, context));

	// Display frame
	window.Display();

	while (physicsTimeAccumulator >= physicsTimeStep)
	{
		// Run OnPhysics for all systems
		systems.ForEach(system => system.OnPhysics(world, context));
		physicsTimeAccumulator -= physicsTimeStep;
	}
}
