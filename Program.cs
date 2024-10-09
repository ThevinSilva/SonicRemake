﻿using SFML.Graphics;
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
using SonicRemake.Systems.Characters;

const float physicsTimeStep = 1.0f / 40.0f;
float physicsTimeAccumulator = 0.0f;

const float animationTimeStep = 1.0f / 60.0f;
float animationTimeAccumulator = 0.0f;

var inputs = new Inputs();
var movement = new Movement();

var window = new RenderWindow(new VideoMode(200, 200), "Sonic");

var world = World.Create();

var clock = new Clock();
clock.Restart();

ImmutableList<GameSystem> systems = [new RenderSystem(), new FpsDebugSystem(), new SonicAnimationSystem()];


// Create Sonic
world.Create(
	new Transform(new Vector2f(0, 0), new Vector2f(1, 1), 0),
	new Velocity(0, 0),
	new Sprite("sonic.png"),
	new SpriteSheet(0, 192, 17),
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
}
