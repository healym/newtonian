using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Joueur.cs.Games.Newtonian
{
    public static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var s in source)
            {
                action(s);
            }
        }

        public static T MinByValue<T, K>(this IEnumerable<T> source, Func<T, K> selector)
        {
            var comparer = Comparer<K>.Default;

            var enumerator = source.GetEnumerator();
            enumerator.MoveNext();

            var min = enumerator.Current;
            var minV = selector(min);

            while (enumerator.MoveNext())
            {
                var s = enumerator.Current;
                var v = selector(s);
                if (comparer.Compare(v, minV) < 0)
                {
                    min = s;
                    minV = v;
                }
            }
            return min;
        }

        public static T MaxByValue<T, K>(this IEnumerable<T> source, Func<T, K> selector)
        {
            var comparer = Comparer<K>.Default;

            var enumerator = source.GetEnumerator();
            enumerator.MoveNext();

            var max = enumerator.Current;
            var maxV = selector(max);

            while (enumerator.MoveNext())
            {
                var s = enumerator.Current;
                var v = selector(s);
                if (comparer.Compare(v, maxV) > 0)
                {
                    max = s;
                    maxV = v;
                }
            }
            return max;
        }

        public static IEnumerable<T> While<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            var enumerator = source.GetEnumerator();
            while (enumerator.MoveNext() && predicate(enumerator.Current))
            {
                yield return enumerator.Current;
            }
        }

        public static Func<T, TResult> Memoize<T, TResult>(this Func<T, TResult> func, IDictionary<T, TResult> cache = null)
        {
            cache = cache ?? new Dictionary<T, TResult>();
            return t =>
            {
                TResult result;
                if (!cache.TryGetValue(t, out result))
                {
                    result = func(t);
                    cache[t] = result;
                }
                return result;
            };
        }

        public static Tile ToTile(this Point point)
        {
            return AI.GAME.GetTileAt(point.x, point.y);
        }

        public static Point ToPoint(this Tile tile)
        {
            return new Point(tile.X, tile.Y);
        }

        public static Point ToPoint(this Unit unit)
        {
            return unit.Tile.ToPoint();
        }

        public static Point ToPoint(this Machine machine)
        {
            return machine.Tile.ToPoint();
        }

        public static IEnumerable<T> Singular<T>(this T item)
        {
            yield return item;
        }

        public static int GetAmount(this Tile tile, string oreType)
        {
            switch (oreType)
            {
                case AI.REDIUM:
                    return tile.Redium;
                case AI.REDIUMORE:
                    return tile.RediumOre;
                case AI.BLUEIUM:
                    return tile.Blueium;
                case AI.BLUEIUMORE:
                    return tile.BlueiumOre;
            }
            return 0;
        }

        public static int GetAmount(this Tile tile, IEnumerable<string> oreTypes)
        {
            return oreTypes.Sum(o => tile.GetAmount(o));
        }



        public static int GetAmount(this Unit unit, string oreType)
        {
            switch (oreType)
            {
                case AI.REDIUM:
                    return unit.Redium;
                case AI.REDIUMORE:
                    return unit.RediumOre;
                case AI.BLUEIUM:
                    return unit.Blueium;
                case AI.BLUEIUMORE:
                    return unit.BlueiumOre;
            }
            return 0;
        }

        public static int GetAmount(this Unit unit, IEnumerable<string> oreTypes)
        {
            return oreTypes.Sum(o => unit.GetAmount(o));
        }

        public static int GetAmount(this Machine machine, IEnumerable<string> oreTypes)
        {
            return oreTypes.Sum(o => machine.GetAmount(o));
        }

        public static int GetAmount(this Machine machine, string oreType)
        {
            return machine.Tile.GetAmount(oreType);
        }

        public static IEnumerable<Tile> GetInRange(this Tile tile)
        {
            yield return tile;
            foreach (var n in tile.GetNeighbors())
            {
                yield return n;
            }
        }

        public static IEnumerable<Unit> UsableUnits(this Player player, Job job = null)
        {
            return player.Units.Where(u => u != null && u.Tile != null && u.StunTime == 0 && (job == null || u.Job == job));
        }

        public static IEnumerable<Unit> UsableScorers(this Player player)
        {
            return player.Units.Where(u => u != null && u.Tile != null && u.StunTime == 0 && (u.Job == AI.MANAGER || u.Job == AI.PHYSICIST));
        }
        public static void SetLog(this GameObject gameObject, string message)
        {
            if (gameObject.Logs.Count == 0 || gameObject.Logs.Last() != message)
            {
                gameObject.Log(message);
            }
        }
    }
}
