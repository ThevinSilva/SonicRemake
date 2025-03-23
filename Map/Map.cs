using System.Numerics;
using Arch.Core;
using Newtonsoft.Json;
using SFML.Graphics;
using SFML.System;
using SonicRemake.Components;
using SonicRemake.Systems;
using TiledLib;
using TiledLib.Layer;


namespace SonicRemake.Maps;

public class TileManagementSystem : GameSystem
{

	private const string FILENAME = "./Assets/Sprites/Loop.tmj";

	private static Log _log = new Log(typeof(TileManagementSystem));
	private QueryDescription Query = new QueryDescription().WithAll<SolidTiles>();

	private Tile[] SolidTiles { get; set; }
	private uint[,] TileMap { get; set; }

	private Map _map;

	public override void OnStart(World world)
	{
		world.Query(in Query, (ref SolidTiles map) =>
		{
			using var stream = File.OpenRead(FILENAME);
			_map = Map.FromStream(stream, ts => File.OpenRead(Path.Combine(Path.GetDirectoryName(FILENAME), ts.Source)));

			SolidTiles = LoadSolidTiles("./Assets/Map/Tiles.json");
			TileMap = LoadMap();

			map.TileMap = TileMap;
			map.TileSet = SolidTiles;
			CreateDrawableEntities(world);
		});
	}

	private static Tile[] LoadSolidTiles(string location)
	{
		string text = File.ReadAllText(location);

		Tile[]? tileset = JsonConvert.DeserializeObject<Tile[]>(text) ?? throw new ArgumentNullException();

		_log.Information($"TileSet of size {tileset.Length} loaded in.");

		return tileset;
	}

	private uint[,] LoadMap()
	{
		// Base layer that contains the solidTile data
		var layer = _map.Layers.OfType<TileLayer>().First();
		uint[,] map = new uint[layer.Height, layer.Width];

		for (int y = 0, i = 0; y < layer.Height; y++)
			for (int x = 0; x < layer.Width; x++, i++)
				map[y, x] = layer.Data[i];

		_log.Information($"Tile Map of size {layer.Height} x {layer.Width} loaded in.");

		return map;
	}

	public void CreateDrawableEntities(World world)
	{
		for (int y = 0; y < TileMap.GetLength(0); y++)
		{
			for (int x = 0; x < TileMap.GetLength(1); x++)
			{
				uint id = MapUtil.GetId(TileMap[y, x]); // clearing gid of any flags

				// _log.Critical($"{id}");

				if (id <= 0) continue;

				// _log.Critical("I ran");

				var pos = new Vector2f(x * 16, y * 16);
				var scale = MapUtil.GetTransformedVector(TileMap[y, x]);
				var image = new Image(SolidTiles[id > 0 ? id - 1 : id].GetColorMatrix());

				world.Create(
				new Components.Transform(pos, scale),
					new Renderer(Layer.BackgroundTiles),
					new SpriteTexture(new Texture(image))
				);
			}
		}
	}


}
