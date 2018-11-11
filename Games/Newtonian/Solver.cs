using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Joueur.cs.Games.Newtonian
{
    public static class Solver
    {
        public static IEnumerable<Point> GetPath(IEnumerable<Point> starts, Func<Point, bool> isGoal)
        {
            return new AStar<Point>(starts, isGoal, (a, b) => 1, p => 0, p =>
            {
                return p.GetNeighbors().Where(n => n.ToTile().IsPathable() || isGoal(n));
            }).Path;
        }

        public static IEnumerable<Point> GetNeighbors(this Point point)
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

        public static IEnumerable<Point> GetDiagonals(this Point point)
        {
            if (point.x > 0)
            {
                if (point.y > 0)
                {
                    yield return new Point(point.x - 1, point.y - 1);
                }
                if (point.y < AI.GAME.MapHeight - 1)
                {
                    yield return new Point(point.x - 1, point.y + 1);
                }
            }
            if (point.x < AI.GAME.MapWidth - 1)
            {
                if (point.y > 0)
                {
                    yield return new Point(point.x + 1, point.y - 1);
                }
                if (point.y < AI.GAME.MapHeight - 1)
                {
                    yield return new Point(point.x + 1, point.y + 1);
                }
            }
        }

        public static void MoveAndPickup(Unit unit, IEnumerable<Tile> tiles, IEnumerable<string> oreTypes)
        {
            if (unit.OpenCapacity() == 0)
            {
                return;
            }

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

        public static void MoveAndAttack(Unit unit, IEnumerable<Unit> targets)
        {
            Move(unit, targets.Where(t => t.Tile != null).Select(t => t.Tile.ToPoint()).ToHashSet());
            if (Rules.CanAttack(unit))
            {
                var enemy = unit.Tile.GetNeighbors().FirstOrDefault(t => t.Unit != null && targets.Contains(t.Unit));
                if (enemy != null)
                {
                    unit.Attack(enemy);
                }
            }
        }

        public static void Stun(Unit unit, IEnumerable<Unit> targets)
        {
            if (!unit.Acted)
            {
                foreach(var target in targets.Where(t => t.Owner != unit.Owner && unit.Tile.HasNeighbor(t.Tile)))
                {
                    if (Rules.CanStun(unit.Job, target.Job) && target.StunImmune == 0)
                    {
                        unit.Act(target.Tile);
                        return;
                    }
                }
            }
        }

        public static void Attack(Unit unit, IEnumerable<Unit> targets)
        {
            if (Rules.CanAttack(unit))
            {
                foreach (var target in targets.Where(t => t.Owner != unit.Owner && unit.Tile.HasNeighbor(t.Tile)))
                {
                    unit.Attack(target.Tile);
                    return;
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

        public static IEnumerable<Point> FindNearest(IEnumerable<Point> starts, IEnumerable<Point> goals)
        {
            var goalSet = goals.ToHashSet();
            var search = new AStar<Point>(starts, p => false, (p1, p2) => 1, p => 0, p =>
            {
                if (goalSet.Contains(p) && !p.ToTile().IsPathable())
                {
                    return Enumerable.Empty<Point>();
                }
                return p.GetNeighbors().Where(n => n.ToTile().IsPathable() || n.ToTile().Unit != null || goalSet.Contains(n));
            });
            return goals.Where(g => search.GScore.ContainsKey(g)).OrderBy(g => search.GScore[g]);
        }
    }
}
