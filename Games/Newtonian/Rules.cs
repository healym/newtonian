using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Joueur.cs.Games.Newtonian
{
    public static class Rules
    {
        public static bool CanAttack(this Unit attacker, Unit target)
        {
            return !attacker.Acted && attacker.Tile.HasNeighbor(target.Tile);
        }

        public static bool CanStun(Job stunner, Job target)
        {
            return (stunner == AI.INTERN && target == AI.PHYSICIST)
                || (stunner == AI.PHYSICIST && target == AI.MANAGER)
                || (stunner == AI.MANAGER && target == AI.INTERN);
        }

        public static bool CanBeWorked(this Machine m)
        {
            if (m.Worked > 0)
            {
                return true; // machines already worked can continue to be worked
            }
            if (m.OreType == AI.REDIUM)
            {
                return m.Tile.RediumOre >= m.RefineInput;
            }
            else
            {
                return m.Tile.BlueiumOre >= m.RefineInput;
            }
        }

        public static bool CanHeal(this Unit u)
        {
            return u.Tile.Owner == u.Owner;
        }

        public static bool CanPickup(this Unit u)
        {
            return OpenCapacity(u) > 0 && IsNextToResources(u);
        }

        public static bool IsNextToResources(this Unit u)
        {
            if (u.Job == AI.INTERN || u.Job == AI.PHYSICIST)
            {
                if(u.Tile.GetNeighbors().Any(t => t.RediumOre > 0 || t.BlueiumOre > 0))
                {
                    return true;
                }
            }
            if (u.Job == AI.MANAGER || u.Job == AI.PHYSICIST)
            {
                if (u.Tile.GetNeighbors().Any(t => t.Redium > 0 || t.Blueium > 0))
                {
                    return true;
                }
            }
            return false;
        }

        public static int OpenCapacity(this Unit u)
        {
            return u.Job.CarryLimit - (u.Blueium + u.BlueiumOre + u.Redium + u.RediumOre);
        }

        public static bool CanSabotageMachine(this Unit saboteur, Machine target)
        {
            return false;
        }


    }
}
