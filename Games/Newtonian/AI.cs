// This is where you build your AI for the Newtonian game.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// <<-- Creer-Merge: usings -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
// you can add additional using(s) here
using System.Runtime.CompilerServices;
// <<-- /Creer-Merge: usings -->>

namespace Joueur.cs.Games.Newtonian
{
    /// <summary>
    /// This is where you build your AI for Newtonian.
    /// </summary>
    public class AI : BaseAI
    {
        #region Properties
        #pragma warning disable 0169 // the never assigned warnings between here are incorrect. We set it for you via reflection. So these will remove it from the Error List.
        #pragma warning disable 0649
        /// <summary>
        /// This is the Game object itself. It contains all the information about the current game.
        /// </summary>
        public readonly Game Game;
        /// <summary>
        /// This is your AI's player. It contains all the information about your player's state.
        /// </summary>
        public readonly Player Player;
        #pragma warning restore 0169
        #pragma warning restore 0649

        // <<-- Creer-Merge: properties -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
        // you can add additional properties here for your AI to use
        // <<-- /Creer-Merge: properties -->>
        #endregion


        #region Methods
        /// <summary>
        /// This returns your AI's name to the game server. Just replace the string.
        /// </summary>
        /// <returns>Your AI's name</returns>
        public override string GetName()
        {
            // <<-- Creer-Merge: get-name -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
            return "Newtonian C# Player"; // REPLACE THIS WITH YOUR TEAM NAME!
            // <<-- /Creer-Merge: get-name -->>
        }

        /// <summary>
        /// This is automatically called when the game first starts, once the Game and all GameObjects have been initialized, but before any players do anything.
        /// </summary>
        /// <remarks>
        /// This is a good place to initialize any variables you add to your AI or start tracking game objects.
        /// </remarks>
        public override void Start()
        {
            // <<-- Creer-Merge: start -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
            base.Start();

            Console.Clear();
            // <<-- /Creer-Merge: start -->>
        }

        /// <summary>
        /// This is automatically called every time the game (or anything in it) updates.
        /// </summary>
        /// <remarks>
        /// If a function you call triggers an update, this will be called before that function returns.
        /// </remarks>
        public override void GameUpdated()
        {
            // <<-- Creer-Merge: game-updated -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
            base.GameUpdated();
            /*this.DisplayMap(); // be careful using this as it will probably cause your client to time out in this function.
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;*/
            // <<-- /Creer-Merge: game-updated -->>
        }

        /// <summary>
        /// This is automatically called when the game ends.
        /// </summary>
        /// <remarks>
        /// You can do any cleanup of you AI here, or do custom logging. After this function returns, the application will close.
        /// </remarks>
        /// <param name="won">True if your player won, false otherwise</param>
        /// <param name="reason">A string explaining why you won or lost</param>
        public override void Ended(bool won, string reason)
        {
            // <<-- Creer-Merge: ended -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
            base.Ended(won, reason);
            // <<-- /Creer-Merge: ended -->>
        }


        /// <summary>
        /// This is called every time it is this AI.player's turn.
        /// </summary>
        /// <returns>Represents if you want to end your turn. True means end your turn, False means to keep your turn going and re-call this function.</returns>
        public bool RunTurn()
        {
            // <<-- Creer-Merge: runTurn -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
            // Put your game logic here for runTurn
            /*DisplayMap();
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;*/

            /*
            Please note: This code is intentionally bad. You should try to optimize everything here. THe code here is only to show you how to use the game's
                        mechanics with the MegaMinerAI server framework.
            */

            // Goes through all the units that you own.
            foreach (Unit unit in this.Player.Units) {
                // Only tries to do something if the unit actually exists.
                if (unit != null && unit.Tile != null) {
                    if (unit.Job.Title == "physicist") {
                        // If the unit is a physicist, tries to work on machines that are ready, but if there are none,
                        // it finds and attacks enemy managers.

                        // Tries to find a workable machine for blueium ore.
                        // Note: You need to get redium ore as well.
                        Tile target = null;

                        // Goes through all the machines in the game and picks one that is ready to process ore as its target.
                        foreach (Machine machine in this.Game.Machines) {
                            if (machine.Tile.BlueiumOre >= machine.RefineInput) {
                                target = machine.Tile;
                            }
                        }

                        if (target == null) {
                            // Chases down enemy managers if there are no machines that are ready to be worked.
                            foreach (Unit enemy in this.Game.Units) {
                                // Only does anything if the unit that we found is a manager and belongs to our opponent.
                                if (enemy.Tile != null && enemy.Owner == this.Player.Opponent && enemy.Job.Title == "manager") {
                                    // Moves towards the manager.
                                    while (unit.Moves > 0 && this.FindPath(unit.Tile, enemy.Tile).Count > 0) {
                                        // Moves until there are no moves left for the physicist.
                                        if (!unit.Move(this.FindPath(unit.Tile, enemy.Tile)[0])) {
                                            break;
                                        }
                                    }
                                    if (unit.Tile == enemy.Tile.TileEast || unit.Tile == enemy.Tile.TileWest ||
                                        unit.Tile == enemy.Tile.TileNorth || unit.Tile == enemy.Tile.TileSouth) {
                                        if (enemy.StunTime == 0 && enemy.StunImmune == 0) {
                                            // Stuns the enemy manager if they are not stunned and not immune.
                                            unit.Act(enemy.Tile);
                                        }
                                        else {
                                            // Attacks the manager otherwise.
                                            unit.Attack(enemy.Tile);
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                        else {
                            // Gets the tile of the targeted machine if adjacent to it.
                            bool adjacent = false;
                            foreach (Tile tile in target.GetNeighbors()) {
                                if (tile == unit.Tile) {
                                    adjacent = true;
                                }
                            }
                            // If there is a machine that is waiting to be worked on, go to it.
                            while (unit.Moves > 0 && this.FindPath(unit.Tile, target).Count > 1) {// && !adjacent) {
                                if (!unit.Move(this.FindPath(unit.Tile, target)[0])) {
                                    break;
                                }
                            }
                            // Acts on the target machine to run it if the physicist is adjacent.
                            if (adjacent && !unit.Acted) {
                                unit.Act(target);
                            }
                        }
                    }
                    else if (unit.Job.Title == "intern") {
                        // If the unit is an intern, collects blueium ore.
                        // Note: You also need to collect redium ore.

                        // Goes to gather resources if currently carrying less than the carry limit.
                        if (unit.BlueiumOre < unit.Job.CarryLimit) {
                            // Your intern's current target.
                            Tile target = null;

                            // Goes to collect blueium ore that isn't on a machine.
                            foreach (Tile tile in this.Game.Tiles) {
                                if (tile.BlueiumOre > 0 && tile.Machine == null) {
                                    target = tile;
                                    break;
                                }
                            }
                            // Moves towards our target until at the target or out of moves.
                            if (this.FindPath(unit.Tile, target).Count > 0) {
                                while (unit.Moves > 0 && this.FindPath(unit.Tile, target).Count > 0) {
                                    if (!unit.Move(this.FindPath(unit.Tile, target)[0])) {
                                        break;
                                    }
                                }
                            }
                            // Picks up the appropriate resource once we reach our target's tile.
                            if (unit.Tile == target && target.BlueiumOre > 0) {
                                unit.Pickup(target, 0, "blueium ore");
                            }
                        }
                        else {
                            // Deposits blueium ore in a machine for it if we have any.

                            // Finds a machine in the game's tiles that takes blueium ore.
                            foreach (Tile tile in this.Game.Tiles) {
                                if (tile.Machine != null && tile.Machine.OreType == "blueium") {
                                    // Moves towards the found machine until we reach it or are out of moves.
                                    while (unit.Moves > 0 && this.FindPath(unit.Tile, tile).Count > 1) {
                                        if (!unit.Move(this.FindPath(unit.Tile, tile)[0])) {
                                            break;
                                        }
                                    }
                                    // Deposits blueium ore on the machine if we have reached it.
                                    if (this.FindPath(unit.Tile, tile).Count <= 1) {
                                        unit.Drop(tile, 0, "blueium ore");
                                    }
                                }
                            }
                        }
                    }
                    else if (unit.Job.Title == "manager") {
                        // Finds enemy interns, stuns, and attacks them if there is no blueium to take to the generator.
                        Tile target = null;

                        foreach (Tile tile in this.Game.Tiles) {
                            if (tile.Blueium > 1 && unit.Blueium < unit.Job.CarryLimit) {
                                target = tile;
                            }
                        }
                        if (target == null && unit.Blueium == 0) {
                            foreach (Unit enemy in this.Game.Units) {
                                // Only does anything for an intern that is owned by your opponent.
                                if (enemy.Tile != null && enemy.Owner == this.Player.Opponent && enemy.Job.Title == "intern") {
                                    // Moves towards the intern until reached or out of moves.
                                    while (unit.Moves > 0 && this.FindPath(unit.Tile, enemy.Tile).Count > 1) {
                                        if (!unit.Move(this.FindPath(unit.Tile, enemy.Tile)[0])) {
                                            break;
                                        }
                                    }
                                    // Either stuns or attacks the intern if we are within range.
                                    if (unit.Tile == enemy.Tile.TileEast || unit.Tile == enemy.Tile.TileWest ||
                                        unit.Tile == enemy.Tile.TileNorth || unit.Tile == enemy.Tile.TileSouth) {
                                        if (enemy.StunTime == 0 && enemy.StunImmune == 0) {
                                            // Stuns the enemy intern if they are not stunned and not immune.
                                            unit.Act(enemy.Tile);
                                        }
                                        else {
                                            // Attacks the intern otherwise.
                                            unit.Attack(enemy.Tile);
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                        else if (target != null) {
                            // Moves towards our target until at the target or out of moves.
                            while (unit.Moves > 0 && this.FindPath(unit.Tile, target).Count > 1) {
                                if (!unit.Move(this.FindPath(unit.Tile, target)[0])) {
                                    break;
                                }
                            }
                            // Picks up blueium once we reach our target's tile.
                            if (this.FindPath(unit.Tile, target).Count <= 1 && target.Blueium > 0) {
                                unit.Pickup(target, 0, "blueium");
                            }
                        }
                        else if (target == null && unit.Blueium > 0) {
                            // Stores a tile that is part of your generator.
                            Tile genTile = this.Player.GeneratorTiles[0];

                            // Goes to your generator and drops blueium in.
                            while (unit.Moves > 0 && this.FindPath(unit.Tile, genTile).Count > 0) {
                                if (!unit.Move(this.FindPath(unit.Tile, genTile)[0])) {
                                    break;
                                }
                            }

                            // Deposits blueium in our generator if we have reached it.
                            if (this.FindPath(unit.Tile, genTile).Count <= 1) {
                                unit.Drop(unit.Tile, 0, "blueium");
                            }
                        }
                    }
                }
            }

            return true;
            // <<-- /Creer-Merge: runTurn -->>
        }

        /// <summary>
        /// A very basic path finding algorithm (Breadth First Search) that when given a starting Tile, will return a valid path to the goal Tile.
        /// </summary>
        /// <remarks>
        /// This is NOT an optimal pathfinding algorithm. It is intended as a stepping stone if you want to improve it.
        /// </remarks>
        /// <param name="start">the starting Tile</param>
        /// <param name="goal">the goal Tile</param>
        /// <returns>A list of Tiles representing the path where the first element is a valid adjacent Tile to the start, and the last element is the goal. Or an empty list if no path found.</returns>
        List<Tile> FindPath(Tile start, Tile goal)
        {
            // no need to make a path to here...
            if (start == goal)
            {
                return new List<Tile>();
            }

            // the tiles that will have their neighbors searched for 'goal'
            Queue<Tile> fringe = new Queue<Tile>();

            // How we got to each tile that went into the fringe.
            Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();

            // Enqueue start as the first tile to have its neighbors searched.
            fringe.Enqueue(start);

            // keep exploring neighbors of neighbors... until there are no more.
            while (fringe.Any())
            {
                // the tile we are currently exploring.
                Tile inspect = fringe.Dequeue();

                // cycle through the tile's neighbors.
                foreach (Tile neighbor in inspect.GetNeighbors())
                {
                    if (neighbor == goal)
                    {
                        // Follow the path backward starting at the goal and return it.
                        List<Tile> path = new List<Tile>();
                        path.Add(goal);

                        // Starting at the tile we are currently at, insert them retracing our steps till we get to the starting tile
                        for (Tile step = inspect; step != start; step = cameFrom[step])
                        {
                            path.Insert(0, step);
                        }

                        return path;
                    }

                    // if the tile exists, has not been explored or added to the fringe yet, and it is pathable
                    if (neighbor != null && !cameFrom.ContainsKey(neighbor) && neighbor.IsPathable())
                    {
                        // add it to the tiles to be explored and add where it came from.
                        fringe.Enqueue(neighbor);
                        cameFrom.Add(neighbor, inspect);
                    }

                } // foreach(neighbor)

            } // while(fringe not empty)

            // if you're here, that means that there was not a path to get to where you want to go.
            //   in that case, we'll just return an empty path.
            return new List<Tile>();
        }

        // <<-- Creer-Merge: methods -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
        // you can add additional methods here for your AI to call
        private void DisplayMap() {
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = ConsoleColor.White;
            Console.Write(new string(' ', this.Game.MapWidth + 2));
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine();
            for (int y = 0; y < this.Game.MapHeight; y++) {
                Console.BackgroundColor = ConsoleColor.White;
                Console.Write(' ');
                for (int x = 0; x < this.Game.MapWidth; x++) {
                    Tile t = this.Game.Tiles[y * this.Game.MapWidth + x];

                    // Background color
                    if (t.Machine != null) {
                        Console.BackgroundColor = ((t.Machine.OreType == "redium") ? ConsoleColor.DarkRed : ConsoleColor.DarkBlue);
                    } else if (t.IsWall == true) {
                        if (t.Decoration == 1 || t.Decoration == 2) {
                            Console.BackgroundColor = ConsoleColor.DarkGray;  // Black;
                        } else {
                            Console.BackgroundColor = ConsoleColor.DarkGray;
                        }
                    } else {
                        if (t.Decoration == 1 || t.Decoration == 2) {
                            Console.BackgroundColor = ConsoleColor.DarkYellow;
                        } else {
                            Console.BackgroundColor = ConsoleColor.Gray;
                        }
                    }

                    // Character to display
                    char foreground = t.Machine == null ? '·' : 'M';
                    Console.ForegroundColor = ConsoleColor.White;

                    // Tile specific stuff
                    if (t.Unit != null) {
                        Console.ForegroundColor = t.Unit.Owner == this.Player ? ConsoleColor.Green : ConsoleColor.Red;
                        foreground = t.Unit.Job.Title[0] == 'i' ? 'I' : t.Unit.Job.Title[0] == 'm' ? 'M' : 'P'; //t.Unit.ShipHealth > 0 ? 'S' : 'C';
                    }
                    if(t.Blueium > 0 || t.Redium > 0) {
                        Console.BackgroundColor = t.Blueium >= t.Redium ? ConsoleColor.DarkBlue : ConsoleColor.DarkRed;
                        if(foreground == '·') {
                            foreground = 'R';
                        }
                    }
                    else if(t.BlueiumOre > 0 || t.RediumOre > 0) {
                        Console.BackgroundColor = t.BlueiumOre >= t.RediumOre ? ConsoleColor.DarkBlue : ConsoleColor.DarkRed;
                        if(foreground == '·') {
                            foreground = 'O';
                        }
                    }
                    else if(t.Owner != null) {
                        if(t.Type == "spawn") {
                            Console.BackgroundColor = t.Owner == this.Player ? ConsoleColor.Cyan : ConsoleColor.Magenta;
                        } else if(t.Type == "generator") {
                            Console.BackgroundColor = t.Owner == this.Player ? ConsoleColor.DarkCyan : ConsoleColor.DarkMagenta;
                        }
                        /*if (false && this.Game.Units.Any(u => u.Path.Contains(t))) {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            foreground = '*';
                        } else if (t.Decoration) {
                            Console.ForegroundColor = ConsoleColor.White;
                            foreground = '.';*/
                    } else if(t.Type == "conveyor") {
                        if(t.Direction == "north") {
                            foreground = '^';
                        } else if(t.Direction == "east") {
                            foreground = '>';
                        } else if(t.Direction == "west") {
                            foreground = '<';
                        } else if(t.Direction == "blank") {
                            foreground = '_';
                        } else {
                            foreground = 'V';
                        }
                    }

                    Console.Write(foreground);
                }

                Console.BackgroundColor = ConsoleColor.White;
                Console.Write(' ');
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(y);
                Console.WriteLine();
            }

            Console.BackgroundColor = ConsoleColor.White;
            Console.Write(new string(' ', this.Game.MapWidth + 2));
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();
            // Clear everything past here
            int left = Console.CursorLeft;
            int top = Console.CursorTop;
            Console.Write(new string(' ', Math.Max(Console.WindowHeight, Console.WindowWidth * (Console.WindowHeight - top) - 1)));
            Console.SetCursorPosition(left, top);
        }
        // <<-- /Creer-Merge: methods -->>
        #endregion
    }
}
