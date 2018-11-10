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

        public static Boolean CanStun(Job stunner, Job target)
        {
            if(stunner == AI.INTERN && target != AI.PHYSICIST)
            {
                 return false;
            }
            if(stunner == AI.MANAGER && target != AI.INTERN)
            {
                 return false;
            }
            if(stunner == AI.PHYSICIST && target != AI.MANAGER)
            {
                 return false;
            }
            else
            {
                return true;
            }
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
