using SFML.Graphics;
using SFML.Window;
using SonicRemake.Inputs;
using Newtonsoft.Json;
using SonicRemake.Movement;
using Arch.Core;
using SonicRemake.Components;
using Sprite = SonicRemake.Components.Sprite;
using SFML.System;
using Transform = SonicRemake.Components.Transform;

var window = new RenderWindow(new VideoMode(200, 200), "Title");


var world = World.Create();

var inputs = new Inputs();
var movement = new Movement();

var clock = new Clock();
clock.Restart();

// Create Sonic
world.Create(
	new Transform(new Vector2f(0, 0), new Vector2f(1, 1), 0), 
	new Velocity(0, 0), 
	new Sprite("sonic.png"), 
	new Renderer());

while (window.IsOpen)
{
	window.DispatchEvents();

	window.Clear();

	OnTick();

	window.Display();

	// var thing = inputs.HandleInput();
	// movement.HandleMovement(thing);
	// Console.WriteLine(movement.GroundSpeed);
	// Console.WriteLine(JsonConvert.SerializeObject(thing));
}

void OnTick()
{
	// Draw all entities with a position and a sprite
	var spriteAndPositionQuery = new QueryDescription().WithAll<Renderer, Sprite, Transform>();
	world.Query(spriteAndPositionQuery, (Entity entity, ref Renderer renderer, ref Sprite sprite, ref Transform transform) =>
	{
		renderer.Texture ??= new Texture($"Assets/Sprites/{sprite.SpriteId}");
		
		window.Draw(new SFML.Graphics.Sprite
		{
			Texture = renderer.Texture,
			Position = transform.Position,
			Scale = transform.Scale,
			Rotation = transform.Rotation
		});
	});
	
	var positionQuery = new QueryDescription().WithAll<Transform>();
	world.Query(positionQuery, (Entity entity, ref Transform transform) =>
	{
		transform.Position = transform.Position with { Y = (float)Math.Sin(clock.ElapsedTime.AsSeconds() * 10) * 100 };
	});
}
