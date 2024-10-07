using SFML.Graphics;
using SFML.Window;
using SonicRemake.Inputs;
using Newtonsoft.Json;
using SonicRemake.Movement;
using Arch.Core;
using SonicRemake.Components;
using Sprite = SonicRemake.Components.Sprite;
using SFML.System;

var window = new RenderWindow(new VideoMode(200, 200), "Title");


var world = World.Create();

var inputs = new Inputs();
var movement = new Movement();

var clock = new Clock();
clock.Restart();

var sonicEntity = world.Create(new Position(0, 0), new Velocity(0, 0), new Sprite("sonic.png"));

while (window.IsOpen)
{
	window.DispatchEvents();

	window.Clear();

	OnTick();

	window.Display();

	var thing = inputs.HandleInput();
	movement.HandleMovement(thing);
	Console.WriteLine(movement.GroundSpeed);
	Console.WriteLine(JsonConvert.SerializeObject(thing));
}


void OnTick()
{
	// Draw all entities with a position and a sprite
	var spriteAndPositionQuery = new QueryDescription().WithAll<Sprite, Position>();
	world.Query(spriteAndPositionQuery, (Entity entity, ref Sprite sprite, ref Position position) =>
	{
		var drawable = new SFML.Graphics.Sprite()
		{
			Texture = new Texture($"Assets/Sprites/{sprite.SpriteId}"),
			Position = new Vector2f(position.X, position.Y),
			Scale = new Vector2f(0.5f, 0.5f)
		};

		window.Draw(drawable);
	});

	// Move all entities with a position slowly up and down
	var positionQuery = new QueryDescription().WithAll<Position>();
	world.Query(positionQuery, (Entity entity, ref Position position) =>
	{
		position.Y = (float)Math.Sin(clock.ElapsedTime.AsSeconds() * 10) * 10;
	});
}
