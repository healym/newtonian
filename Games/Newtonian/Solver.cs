using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Joueur.cs.Games.Newtonian
{
    public static class Solver
    {
        public static void RunPhysicist(Unit unit, IEnumerable<Machine> machines)
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

        public static void RunManager(Unit u)
        {
            var goalType = AI.REDIUM;
            if (Rules.OpenCapacity(u) > 0)
            {
                var goalTiles = AI.GAME.Tiles.Where(t => t.GetAmount(goalType) > 0).Select(t => t.ToPoint()).ToHashSet();
                var path = GetPath(u.ToPoint().Singular(), (p => goalTiles.Contains(p)));
                if (path != null && path.Count() > 0)
                {
                    foreach (Point step in path.Skip(1).SkipLast(1).Take(u.Moves))
                    {
                        u.Move(step.ToTile());
                    }
                    var pickup = path.Last().ToTile();
                    if (u.Tile == pickup || u.Tile.HasNeighbor(pickup))
                    {
                        u.Pickup(pickup, 0, goalType);
                    }
                }
                
            }

            if (Rules.OpenCapacity(u) == 0)
            {
                var goalTiles = u.Owner.GeneratorTiles.Select(t => t.ToPoint()).ToHashSet();
                var path = GetPath(u.ToPoint().Singular(), (p => goalTiles.Contains(p)));

                if (path != null && path.Count() > 0)
                {
                    foreach (Point step in path.Skip(1).SkipLast(1).Take(u.Moves))
                    {
                        u.Move(step.ToTile());
                    }
                    var jenny = path.Last().ToTile();
                    if (u.Tile == jenny || u.Tile.HasNeighbor(jenny))
                    {
                        u.Drop(jenny, AI.MANAGER.CarryLimit, AI.REDIUM);
                        u.Drop(jenny, AI.MANAGER.CarryLimit, AI.BLUEIUM);
                    }
                }
            }
        }

        public static IEnumerable<Point> GetPath(IEnumerable<Point> starts, Func<Point, bool> isGoal)
        {
            return new AStar<Point>(starts, isGoal, (a, b) => 1, p => 0, p =>
            {
                return p.GetNeighboors().Where(n => n.ToTile().IsPathable() || isGoal(n));
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

        public static void MoveAndPickup(Unit unit, IEnumerable<Tile> tiles, IEnumerable<string> oreTypes)
        {
            if (unit.OpenCapacity() == 0)
            {
                return;
            }
            Console.WriteLine("MoveAndPickup {0}->{1} {2}", unit.ToPoint(), tiles.Count(), String.Join(",", oreTypes));

            var goalPoints = tiles.Where(t => t.GetAmount(oreTypes) > 0).Select(t => t.ToPoint()).ToHashSet();

            Move(unit, goalPoints);

            var goalsInRange = unit.Tile.GetInRange().Where(t => goalPoints.Contains(t.ToPoint()));
            foreach (var pickup in goalsInRange)
            {
                foreach (var oreType in oreTypes)
                {
                    if (pickup.GetAmount(oreType) > 0 && unit.OpenCapacity() > 0)
                    {
                        unit.Pickup(pickup, 0, oreType);
                    }
                }
            }
        }

        public static void MoveAndDrop(Unit unit, IEnumerable<Tile> tiles, IEnumerable<string> oreTypes)
        {
            if (unit.GetAmount(oreTypes) == 0)
            {
                return;
            }
            Console.WriteLine("MoveAndDrop {0}->{1} {2}", unit.ToPoint(), tiles.Count(), String.Join(",", oreTypes));

            var goalPoints = tiles.Select(t => t.ToPoint()).ToHashSet();

            Move(unit, goalPoints);

            var goalsInRange = unit.Tile.GetInRange().Where(t => goalPoints.Contains(t.ToPoint()));
            foreach (var drop in goalsInRange)
            {
                foreach (var oreType in oreTypes)
                {
                    if (unit.GetAmount(oreType) > 0)
                    {
                        unit.Drop(drop, 0, oreType);
                    }
                }
            }
        }

        public static void Move(Unit unit, HashSet<Point> goalPoints)
        {
            if (unit.Moves == 0)
            {
                return;
            }

            var path = GetPath(unit.ToPoint().Singular(), (p => goalPoints.Contains(p)));
            if (path.Count() > 2)
            {
                foreach (Point step in path.Skip(1).SkipLast(1).Take(unit.Moves))
                {
                    unit.Move(step.ToTile());
                }
            }
        }
    }
}
