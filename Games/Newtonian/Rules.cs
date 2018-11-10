using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Joueur.cs.Games.Newtonian
{
    public static class Rules
    {
        public static Boolean CanAttack(Unit attacker, Unit target)
        {
            return attacker.Tile.HasNeighbor(target.Tile);
        }

        public static Boolean CanStun(Unit stunner, Unit target)
        {
            return false;
        }

        public static Boolean CanSabotageMachine(Unit saboteur)
        {
            return saboteur.Job == AI.INTERN && saboteur.Tile.GetNeighbors().Any(t => t.Machine != null);
        }

        public static Boolean CanWorkOnMachine(Unit worker)
        {
            return worker.Job == AI.PHYSICIST && worker.Tile.GetNeighbors().Any(t => t.Machine != null);
        }
    }
}
