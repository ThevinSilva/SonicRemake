using Arch.Core;
using SFML.Graphics;

namespace SonicRemake.Systems
{
	public abstract class GameSystem
	{
		/// <summary>
		/// Called when the system is started. Runs only once, before the main loop.
		/// </summary>
		public virtual void OnStart(World world) { }

		/// <summary>
		/// Called every frame.
		/// </summary>
		public virtual void OnRender(World world, RenderWindow window, GameContext context) { }

		/// <summary>
		/// Called every animation frame.
		/// </summary>	
		public virtual void OnAnimation(World world, RenderWindow window, GameContext context) { }

		/// <summary>
		/// Called every game tick.
		/// </summary>
		public virtual void OnPhysics(World world, GameContext context) { }
	}
}
