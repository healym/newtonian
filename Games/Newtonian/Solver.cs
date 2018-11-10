using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Joueur.cs.Games.Newtonian
{
    public static class Solver
    {
        public static void Work(Unit unit, IEnumerable<Machine> machines)
        {
            var goals = machines.Where(m => Rules.CanBeWorked(m)).Select(m => m.ToPoint()).ToHashSet();
            var path = GetPath(unit.ToPoint().Singular(), p => goals.Contains(p)).ToArray();
            if (path != null && path.Length > 2)
            {
                path.Skip(1).SkipLast(1).Take(unit.Moves).ForEach(p => unit.Move(p.ToTile()));
            }

            if (unit.Acted)
            {
                return;
            }
            var target = unit.Tile.GetNeighbors().FirstOrDefault(t => t.Machine != null && Rules.CanBeWorked(t.Machine));
            if (target != null)
            {
                unit.Act(target);
            }
        }

        public static void ManagerWork(Unit u)
        {
            var goalType = AI.PLAYER.Heat < AI.PLAYER.Pressure && AI.GAME.Tiles.Where(t => Extensions.GetAmount(t, AI.REDIUM) > 0).Count() > 0 ? AI.REDIUM : AI.BLUEIUM;
            if (Rules.OpenCapacity(u) > 0)
            {
                var goalTiles = AI.GAME.Tiles.Where(t => Extensions.GetAmount(t, goalType) > 0).ToHashSet();
                var path = GetPath(u.ToPoint().Singular(), (p => goalTiles.Contains(p.ToTile()))).ToArray();
                foreach(Point step in path.Skip(1).SkipLast(1).Take(u.Moves))
                {
                    u.Move(step.ToTile());
                }
                var pickup = path.Last().ToTile();
                var currentAmt = goalType == AI.REDIUM ? u.Redium : u.Blueium;
                var pickupAmt = AI.MANAGER.CarryLimit - currentAmt - (goalType == AI.REDIUM ? pickup.Redium : pickup.Blueium);
                if (u.Tile == pickup || u.Tile.HasNeighbor(pickup))
                {
                    u.Pickup(pickup, pickupAmt, goalType);
                }
            }
            else
            {
                var goalTiles = AI.GAME.Tiles.Where(t => t.Type == "generator").ToHashSet();
                var path = GetPath(u.ToPoint().Singular(), (p => goalTiles.Contains(p.ToTile()))).ToArray();
                foreach(Point step in path.Skip(1).SkipLast(1).Take(u.Moves))
                {
                    u.Move(step.ToTile());
                }
                var jenny = path.Last().ToTile();
                if(u.Tile == jenny || u.Tile.HasNeighbor(jenny))
                {
                    u.Drop(jenny, AI.MANAGER.CarryLimit, AI.REDIUM);
                    u.Drop(jenny, AI.MANAGER.CarryLimit, AI.BLUEIUM);

                }
            }
        }

        public static IEnumerable<Point> GetPath(IEnumerable<Point> starts, Func<Point, bool> isGoal)
        {
            return new AStar<Point>(starts, isGoal, (a, b) => 1, p => 0, p =>
            {
                return p.GetNeighboors().Where(n => n.ToTile().IsPathable() || isGoal(p));
            }).Path;
        }

        public static IEnumerable<Point> GetNeighboors(this Point point)
        {
            if (point.x > 0)
            {
                yield return new Point(point.x - 1, point.y);
            }
            if (point.y > 0)
            {
                yield return new Point(point.x, point.y - 1);
            }
            if (point.x < AI.GAME.MapWidth - 1)
            {
                yield return new Point(point.x + 1, point.y);
            }
            if (point.y < AI.GAME.MapHeight - 1)
            {
                yield return new Point(point.x, point.y + 1);
            }
        }
    }
}
