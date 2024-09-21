using SFML.Graphics;
using SFML.Window;
using SonicRemake.Inputs;
using Newtonsoft.Json;
using SonicRemake.Movement;

var window = new RenderWindow(new VideoMode(200, 200), "Title");
var circle = new CircleShape(100);
circle.FillColor = Color.Red;

var idleTexture = new Texture("idle_1.png");
var sonic = new Sprite();
var inputs = new Inputs();
var movement = new Movement();
sonic.Texture = idleTexture;

while (window.IsOpen)
{
	window.DispatchEvents();
	
	window.Clear();
	window.Draw(sonic);
	window.Display();
	var thing = inputs.HandleInput();
	movement.HandleMovement(thing);
	Console.WriteLine(movement.GroundSpeed);
    Console.WriteLine(JsonConvert.SerializeObject(thing));
}