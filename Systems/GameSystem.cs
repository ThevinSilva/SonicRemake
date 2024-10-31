using System.Diagnostics;
using Arch.Core;
using SFML.Graphics;

namespace SonicRemake.Systems;

public abstract class GameSystem
{
	protected GameSystem()
	{
		_log = new Log(GetType());
	}

	protected readonly Log _log;

	/// <summary>
	/// Called when the system is started. Runs only once, before the main loop.
	/// </summary>
	public virtual void OnStart(World world) { }

	/// <summary>
	/// Called every game tick.
	/// </summary>	
	public virtual void OnTick(World world, GameContext context) { }

	/// <summary>
	/// Called every frame.
	/// </summary>
	public virtual void OnRender(World world, RenderWindow window, GameContext context) { }

	public void Start(World world)
	{
		try
		{
			Measure(() => OnStart(world));
		}
		catch (Exception e)
		{
			_log.Error(e);
		}
	}

	public void Tick(World world, GameContext context)
	{
		try
		{
			Measure(() => OnTick(world, context));
		}
		catch (Exception e)
		{
			_log.Error(e);
		}
	}

	public void Render(World world, RenderWindow window, GameContext context)
	{
		try
		{
			Measure(() => OnRender(world, window, context));
		}
		catch (Exception e)
		{
			_log.Error(e);
		}
	}

	private static TimeSpan Measure(Action action)
	{
		var stopwatch = Stopwatch.StartNew();
		action();
		stopwatch.Stop();
		return stopwatch.Elapsed;
	}
}
