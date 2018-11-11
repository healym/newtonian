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
            var goals = machines.Where(m => Rules.CanBeWorked(m) && m.Tile.GetNeighbors().All(t => t.Unit == null || t.Unit.Job != AI.PHYSICIST || t.Unit.Owner != AI.PLAYER)).Select(m => m.ToPoint()).ToHashSet();
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

        public static void RunManagers()
        {
            foreach (var u in AI.PLAYER.Units.Where(u => u != null && u.Tile != null && u.Job == AI.MANAGER))
            {
                var goalTypes = new [] { AI.REDIUM, AI.BLUEIUM };
                if (Rules.OpenCapacity(u) > 0)
                {
                    MoveAndPickup(u, AI.GAME.Tiles, goalTypes);
                }

                if (Rules.OpenCapacity(u) == 0)
                {
                   MoveAndDrop(u, AI.GAME.Tiles.Where(t => t.Type == "generator"), goalTypes);
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
