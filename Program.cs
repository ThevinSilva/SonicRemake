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

var idleTexture = new Texture("idle_1.png");
var sonic = new SFML.Graphics.Sprite();
var inputs = new Inputs();
var movement = new Movement();
sonic.Texture = idleTexture;

var clock = new Clock();
clock.Restart();

var sonicEntity = world.Create(new Position(0, 0), new Velocity(0, 0), new Sprite("idle_1.png"));

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
			Texture = new Texture(sprite.SpriteId),
			Position = new Vector2f(position.X, position.Y)
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
