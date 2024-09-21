using SFML.Graphics;
using SFML.Window;

var window = new RenderWindow(new VideoMode(200, 200), "Title");
var circle = new CircleShape(100);
circle.FillColor = Color.Red;

var idleTexture = new Texture("idle_1.png");
var sonic = new Sprite();
sonic.Texture = idleTexture;

while (window.IsOpen)
{
	window.DispatchEvents();
	
	window.Clear();
	window.Draw(sonic);
	window.Display();
}