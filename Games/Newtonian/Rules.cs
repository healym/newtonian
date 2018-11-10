﻿using System;
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

        public static Boolean CanStun(Job stunner, Job target)
        {
            return (stunner == AI.INTERN && target == AI.PHYSICIST)
              || (stunner == AI.PHYSICIST && target == AI.MANAGER)
              || (stunner == AI.MANAGER && target == AI.INTERN);
        }

        public static Boolean CanBeWorked(Machine m)
        {
            if (m.Worked > 0)
            {
                return true; // machines already worked can continue to be worked
            }
            if (m.OreType == "Redium")
            {
                return m.Tile.RediumOre >= m.RefineInput;
            }
            else
            {
                return m.Tile.BlueiumOre >= m.RefineInput;
            }
        }
    }
}
