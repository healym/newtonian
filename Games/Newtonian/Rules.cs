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
            return !attacker.Acted && attacker.Tile.HasNeighbor(target.Tile);
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

        public static Boolean CanHeal(Unit u)
        {
            return u.Tile.Owner == u.Owner;
        }

        public static Boolean CanPickup(Unit u)
        {
            return !IsFull(u) && IsNextToResources(u);
        }

        public static Boolean IsNextToResources(Unit u)
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

        public static Boolean IsFull(Unit u)
        {
            if (u.Job == AI.MANAGER)
            {
                return u.Blueium + u.Redium < AI.MANAGER.CarryLimit;
            }
            if (u.Job == AI.PHYSICIST)
            {
                return u.Blueium + u.BlueiumOre + u.Redium + u.RediumOre < AI.PHYSICIST.CarryLimit;
            }
            if (u.Job == AI.INTERN)
            {
                return u.BlueiumOre + u.RediumOre < AI.INTERN.CarryLimit;
            }
            return false; // should never happen, outside domain
        }

        public static Boolean CanSabotageMachine(Unit saboteur, Machine target)
        {
            return false;
        }


    }
}
