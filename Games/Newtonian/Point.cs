﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Joueur.cs.Games.Newtonian
{
    public struct Point
    {
        public int x;
        public int y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object obj)
        {
            Point o = (Point)obj;
            return o.x == x && o.y == y;
        }

        public override int GetHashCode()
        {
            return x * AI.GAME.MapWidth + y;
        }

        public override string ToString()
        {
            return String.Format("({0},{1})", x, y);
        }
    }
}
