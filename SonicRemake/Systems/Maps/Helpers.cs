using SFML.System;

namespace SonicRemake.Systems.Maps;

public static class MapUtil
{
	private const uint FLIPPED_HORIZONTALLY_FLAG = 0x80000000;
	private const uint FLIPPED_VERTICALLY_FLAG = 0x40000000;
	private const uint FLIPPED_DIAGONALLY_FLAG = 0x20000000;
	private const uint ROTATED_HEXAGONAL_120_FLAG = 0x10000000;
	public static Vector2f GetTransformedVector(uint gid)
	{
		var x = (gid & FLIPPED_HORIZONTALLY_FLAG) != 0 ? -1f : 1f;
		var y = (gid & FLIPPED_VERTICALLY_FLAG) != 0 ? -1f : 1f;
		var diagonal = (gid & FLIPPED_DIAGONALLY_FLAG) != 0;

		return new Vector2f(diagonal ? -x : x, diagonal ? -y : y);
	}

	/// <summary>
	/// Clears the global id of all the flags
	/// </summary>
	/// <param name="gid">Global Id</param>
	/// <returns></returns>
	public static uint GetId(uint gid) => gid & ~(FLIPPED_HORIZONTALLY_FLAG |
									FLIPPED_VERTICALLY_FLAG |
									FLIPPED_DIAGONALLY_FLAG |
									ROTATED_HEXAGONAL_120_FLAG);
}
