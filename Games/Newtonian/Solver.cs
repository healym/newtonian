using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Joueur.cs.Games.Newtonian
{
    public static class Solver
    {
        public static void RunPhysicist(Unit unit)
        {
            Move(unit, AI.GAME.Machines.Where(m => m.CanBeWorked()).Select(m => m.ToPoint()).ToHashSet());
            Move(unit, AI.GAME.Machines.Where(m => m.Tile.Redium > 0 || m.Tile.Blueium > 0).Select(m => m.ToPoint()).ToHashSet());
            MoveAndAttack(unit, AI.PLAYER.Opponent.Units);

            if (!unit.Acted)
            {
                var machineTile = unit.Tile.GetNeighbors().FirstOrDefault(t => t.Machine != null && t.Machine.CanBeWorked());
                if (machineTile != null)
                {
                    unit.Act(machineTile);
                }
            }
        }

        public static void RunManagers()
        {
            foreach (var manager in AI.PLAYER.Units.Where(u => u != null && u.Tile != null && u.Job == AI.MANAGER))
            {
                var goalTypes = new [] { AI.REDIUM, AI.BLUEIUM };
                if (Rules.OpenCapacity(manager) > 0)
                {
                    MoveAndPickup(manager, AI.GAME.Tiles, goalTypes);
                }

                if (Rules.OpenCapacity(manager) == 0)
                {
                   MoveAndDrop(manager, AI.GAME.Tiles.Where(t => t.Type == "generator"), goalTypes);
                }
                MoveAndAttack(manager, AI.GAME.Units.Where(u => u.Owner != manager.Owner));
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
            var enemy = unit.Tile.GetNeighbors().FirstOrDefault(t => t.Unit != null && t.Unit.Owner != unit.Owner);
            if (enemy != null)
            {
                unit.Attack(enemy);
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
