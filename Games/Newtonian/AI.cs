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
        public static Game GAME;
        public static Player PLAYER;
        public static Job INTERN;
        public static Job MANAGER;
        public static Job PHYSICIST;
        public const string REDIUM = "redium";
        public const string BLUEIUM = "blueium";
        public const string REDIUMORE = "redium ore";
        public const string BLUEIUMORE = "blueium ore";
        public static Random RANDOM = new Random();
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
            return "Onyx Plateau"; // REPLACE THIS WITH YOUR TEAM NAME!
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

            AI.GAME = this.Game;
            AI.PLAYER = this.Player;

            AI.INTERN = this.Game.Jobs.First(j => j.Title == "intern");
            AI.MANAGER = this.Game.Jobs.First(j => j.Title == "manager");
            AI.PHYSICIST = this.Game.Jobs.First(j => j.Title == "physicist");
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
            if ((this.Game.CurrentTurn - 5) % 50 == 0)
            {
                ClearUnitLogs();
            }
            if (this.Game.CurrentTurn % 50 == 0)
            {
                LogEnemies();
            }
            RunInterns();
            RunPhysicists();
            RunManagers();
            foreach (var unit in this.Player.Units.Where(u => u != null && u.Tile != null))
            {
                Solver.Stun(unit, this.Player.Opponent.Units);
                Solver.Attack(unit, this.Player.Opponent.Units);
            }

            return true;
            // <<-- /Creer-Merge: runTurn -->>
        }

        public void LogEnemies()
        {
            AI.GAME.Units.Where(u => u.Job == AI.INTERN).ForEach(u => LogIntern(u));
            AI.GAME.Units.Where(u => u.Job == AI.PHYSICIST).ForEach(u => LogPhysicist(u));
            AI.GAME.Units.Where(u => u.Job == AI.MANAGER).ForEach(u => LogManager(u));
        }

        public void LogIntern(Unit u)
        {
            string[] internSayings = { "I should be in charge of this dump.",
                                       "I wonder how much free coffee I can drink in a day.",
                                       "What does '360 wellness' even mean??",
                                       "Do you think anybody's noticed that I haven't figured out Git yet?"};
            int r = RANDOM.Next(internSayings.Count());
            u.Log(internSayings[r]);
            return;
        }

        public void LogPhysicist(Unit u)
        {
            string[] physSayings = { "Morning, Mr. Freeman",
                                     "al;sjhdfa;hd;sakhf;szd" };
            int r = RANDOM.Next(physSayings.Count());
            u.Log(physSayings[r]);
            return;
        }

        public void LogManager(Unit u)
        {
            string[] manSayings = { "hurr durr",
                                    "pbbbbbbbtttt",
                                    "We make such a good team",
                                    "Remember your 360 wellness!",
                                    "Investigations hurt science."};
            int r = RANDOM.Next(manSayings.Count());
            u.Log(manSayings[r]);
            return;
        }

        public void ClearUnitLogs()
        {
            AI.GAME.Units.ForEach(u => u.Log(""));
        }

        public void RunInterns()
        {
            foreach (var intern in this.Player.Units.Where(u => u != null && u.Tile != null && u.Job == AI.INTERN))
            {
                IEnumerable<string> oreTypes = new[] { AI.BLUEIUMORE, AI.REDIUMORE };

                var victoryTypes = new List<string>();
                if ((this.Player.Pressure * (this.Player.Heat + AI.MANAGER.CarryLimit * AI.GAME.RefinedValue)) > this.Game.VictoryAmount)
                {
                    victoryTypes.Add(AI.REDIUM);
                }
                if ((this.Player.Heat * (this.Player.Pressure + AI.MANAGER.CarryLimit * AI.GAME.RefinedValue)) > this.Game.VictoryAmount)
                {
                    victoryTypes.Add(AI.BLUEIUM);
                }

                if (victoryTypes.Count > 0)
                {
                    oreTypes = victoryTypes;
                }
                else if (intern.RediumOre != 0 && intern.BlueiumOre == 0)
                {
                    oreTypes = AI.REDIUMORE.Singular();
                }
                else if (intern.BlueiumOre != 0 && intern.RediumOre == 0)
                {
                    oreTypes = AI.BLUEIUMORE.Singular();
                }
                else
                {
                    var needyMachines = this.Game.Machines.Where(m => !Rules.CanBeWorked(m));
                    var redCount = needyMachines.Count(m => m.OreType == AI.REDIUM);
                    var blueCount = needyMachines.Count(m => m.OreType == AI.BLUEIUM);
                    if ((blueCount == 0 && redCount > 0) || (blueCount == 1 && redCount == 3))
                    {
                        oreTypes = AI.REDIUMORE.Singular();
                    }
                    if ((redCount == 0 && blueCount > 0) || (redCount == 1 && blueCount == 3))
                    {
                        oreTypes = AI.BLUEIUMORE.Singular();
                    }

                    //var closestMachine = Solver.FindNearest(this.Player.GeneratorTiles.Select(t => t.ToPoint()), needyMachines.Select(m => m.Tile.ToPoint())).First();
                    //oreTypes = closestMachine.ToTile().Machine.OreType == AI.REDIUM ? AI.REDIUMORE.Singular() : AI.BLUEIUMORE.Singular();
                }
                for (var i = 0; i < 4; i++)
                {
                    Solver.MoveAndPickup(intern, AI.GAME.Tiles.Where(t => t.Machine == null), oreTypes);
                }

                if (intern.GetAmount(oreTypes) > 0)
                {
                    var dropType = intern.RediumOre > intern.BlueiumOre ? AI.REDIUMORE : AI.BLUEIUMORE;
                    var machineType = intern.RediumOre > intern.BlueiumOre ? AI.REDIUM : AI.BLUEIUM;
                    var machines = AI.GAME.Machines.Where(m => m.OreType == machineType && !m.CanBeWorked());
                    if (machines.Count() == 0)
                    {
                        machines = AI.GAME.Machines.Where(m => m.OreType == machineType);
                    }
                    Solver.MoveAndDrop(intern, AI.GAME.Machines.Where(m => m.OreType == machineType).Select(m => m.Tile), dropType.Singular());
                    Solver.MoveAndDrop(intern, AI.GAME.Machines.Where(m => m.OreType == machineType).Select(m => m.Tile), dropType.Singular());
                }
            }
        }

        public void RunManagers()
        {
            foreach (var manager in this.Player.Units.Where(u => u != null && u.Tile != null && u.Job == AI.MANAGER))
            {
                var goalTypes = new[] { AI.REDIUM, AI.BLUEIUM };
                if (Rules.OpenCapacity(manager) > 0)
                {
                    Solver.MoveAndPickup(manager, AI.GAME.Tiles, goalTypes);
                }

                if (Rules.OpenCapacity(manager) < AI.MANAGER.CarryLimit)
                {
                    Solver.MoveAndDrop(manager, this.Player.GeneratorTiles, goalTypes);
                }
                IEnumerable<Machine> guardMachines = AI.GAME.Machines.Where(m => m.CanBeWorked());
                if (!guardMachines.Any())
                {
                    guardMachines = AI.GAME.Machines.Where(m => m.Tile.RediumOre > 0 || m.Tile.BlueiumOre > 0);
                    if (!guardMachines.Any())
                    {
                        guardMachines = AI.GAME.Machines;
                    }
                }
                Solver.Move(manager, guardMachines.SelectMany(m => m.Tile.GetNeighbors()).Select(t => t.ToPoint()).ToHashSet());
                Congratulate(manager);
            }
        }

        public void Congratulate(Unit m)
        {
            if (m.Tile.GetNeighbors().Any(t => t.Unit != null && t.Unit.Job == AI.PHYSICIST && t.Unit.Acted && t.Unit.Owner == m.Owner))
            {
                string[] surpriseSayings = { "Keep up the good work!", "Gordan Freeman, and about time, too." };
                int r = RANDOM.Next(surpriseSayings.Count());
                m.Log(surpriseSayings[r]);
            }
        }

        public void RunPhysicists()
        {
            foreach (var physicist in this.Player.Units.Where(u => u != null && u.Tile != null && u.Job == AI.PHYSICIST))
            {
                IEnumerable<Point> targets = AI.GAME.Machines.Where(m => m.CanBeWorked()).Select(m => m.ToPoint());
                if (!targets.Any())
                {
                    targets = AI.GAME.Machines.Where(m => m.Tile.RediumOre > 0 || m.Tile.BlueiumOre > 0).SelectMany(m => m.ToPoint().GetNeighbors());
                    if (!targets.Any())
                    {
                        targets = AI.GAME.Machines.SelectMany(m => m.ToPoint().GetNeighbors());
                    }
                }
                Solver.Move(physicist, targets.ToHashSet());
                Solver.Work(physicist, AI.GAME.Machines);
            }
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
