using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;

namespace GemBees.Utils
{
	//TODO - These functions are surely implemented in a library somewhere...
	//see if we can find and depend on one as they probably have many more useful functions

	/// <summary>
	/// Utility methods relating to tiles in the world
	/// </summary>
    internal static class TileUtils
    {
		/// <summary>
		/// Depth first algorithm for checking nearby tiles in a diamond pattern to see if any satisfy a predicate.
		/// </summary>
		/// <param name="predicate">The function to check on nearby tiles</param>
		/// <param name="x">The x-coord of the tile to check around on the main tilemap</param>
		/// <param name="y">The y-coord of the tile to check around on the main tilemap</param>
		/// <param name="visited">Set of Points of coords that have already been checked, and should not be checked again</param>
		/// <param name="range">The range outwards to check, by Manhattan distance</param>
		/// <returns>True if any tiles within range satisfy the predicate, or false if none do</returns>
		public static bool CheckNearbyTiles(Func<Tile, bool> predicate, int x, int y, int range = 5, HashSet<Point> visited = null)
		{
			if (range < 1) { return false; }
			if (!WorldGen.InWorld(x, y)) { return false; }
			if (visited == null) { visited = new HashSet<Point>(); };
			Point currTile = new Point(x, y);
			if (visited.Contains(currTile)) { return false; }
			visited.Add(currTile);

			return predicate(Main.tile[currTile])
				|| CheckNearbyTiles(predicate, currTile.X + 1, currTile.Y, range - 1, visited)
				|| CheckNearbyTiles(predicate, currTile.X, currTile.Y + 1, range - 1, visited)
				|| CheckNearbyTiles(predicate, currTile.X - 1, currTile.Y, range - 1, visited)
				|| CheckNearbyTiles(predicate, currTile.X, currTile.Y - 1, range - 1, visited);
		}
	}
}
