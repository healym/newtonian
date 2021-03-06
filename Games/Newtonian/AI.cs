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

        public static Machine CLOSEST_REDIUM;
        public static Machine CLOSEST_BLUEIUM;
        public static Machine HARDEST_REDIUM;
        public static Machine HARDEST_BLUEIUM;
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

            AI.CLOSEST_REDIUM = Solver.FindNearest(this.Player.SpawnTiles.Select(t => t.ToPoint()), this.Game.Machines.Where(m => m.OreType == AI.REDIUM).Select(m => m.ToPoint())).First().ToTile().Machine;
            AI.CLOSEST_BLUEIUM = Solver.FindNearest(this.Player.SpawnTiles.Select(t => t.ToPoint()), this.Game.Machines.Where(m => m.OreType == AI.BLUEIUM).Select(m => m.ToPoint())).First().ToTile().Machine;
            AI.HARDEST_REDIUM = Solver.FindNearest(this.Player.Opponent.SpawnTiles.Select(t => t.ToPoint()), this.Game.Machines.Where(m => m.OreType == AI.REDIUM).Select(m => m.ToPoint())).First().ToTile().Machine;
            AI.HARDEST_BLUEIUM = Solver.FindNearest(this.Player.Opponent.SpawnTiles.Select(t => t.ToPoint()), this.Game.Machines.Where(m => m.OreType == AI.BLUEIUM).Select(m => m.ToPoint())).First().ToTile().Machine;
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
            LogInventories();

            // Score();
            // Dos();
            // Assault();
            RunInterns();
            RunPhysicists();
            RunManagers();
            foreach (var unit in this.Player.Units.Where(u => u != null && u.Tile != null && u.StunTime == 0))
            {
                Solver.Stun(unit, this.Player.Opponent.Units);
                Solver.Attack(unit, this.Player.Opponent.Units);
            }
            CleanupPhysicists();
            CleanupCleanupEverybodyEverywhere();
            return true;
            // <<-- /Creer-Merge: runTurn -->>
        }

        public void Assault(IEnumerable<Unit> units)
        {
            //foreach (var unit in units)
            //{
            //    var targetJob = unit.Job == AI.INTERN ? AI.PHYSICIST : (unit.Job == AI.PHYSICIST ? AI.MANAGER : AI.INTERN);
            //    Solver.MoveAndAttack(AI.PLAYER.Opponent())
            //}
        }

        public void RunInterns()
        {
            foreach (var intern in this.Player.Units.Where(u => u != null && u.Tile != null && u.StunTime == 0 && u.Job == AI.INTERN))
            {
                var neighborMachine = intern.NeighborMachines().FirstOrDefault();
                if (neighborMachine != null && neighborMachine.OreType == AI.REDIUM && intern.RediumOre > 0)
                {
                    intern.Drop(neighborMachine.Tile, -1, AI.REDIUMORE);
                }
                if (neighborMachine != null && neighborMachine.OreType == AI.BLUEIUM && intern.BlueiumOre > 0)
                {
                    intern.Drop(neighborMachine.Tile, -1, AI.BLUEIUMORE);
                }
                while (neighborMachine != null && neighborMachine.OreType == AI.REDIUM && intern.Tile.RediumOre > 0 && intern.OpenCapacity() > 0)
                {
                    intern.Pickup(intern.Tile, -1, AI.REDIUMORE);
                    intern.Drop(neighborMachine.Tile, -1, AI.REDIUMORE);
                }
                while (neighborMachine != null && neighborMachine.OreType == AI.BLUEIUM && intern.Tile.BlueiumOre > 0 && intern.OpenCapacity() > 0)
                {
                    intern.Pickup(intern.Tile, -1, AI.BLUEIUMORE);
                    intern.Drop(neighborMachine.Tile, -1, AI.BLUEIUMORE);
                }

                IEnumerable<string> oreTypes = new[] { AI.BLUEIUMORE, AI.REDIUMORE };

                var victoryTypes = new List<string>();
                if ((this.Player.Pressure * (this.Player.Heat + AI.MANAGER.CarryLimit * AI.GAME.RefinedValue)) > this.Game.VictoryAmount)
                {
                    victoryTypes.Add(AI.REDIUMORE);
                }
                if ((this.Player.Heat * (this.Player.Pressure + AI.MANAGER.CarryLimit * AI.GAME.RefinedValue)) > this.Game.VictoryAmount)
                {
                    victoryTypes.Add(AI.BLUEIUMORE);
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
                    if (machines.Contains(AI.CLOSEST_REDIUM))
                    {
                        machines = AI.CLOSEST_REDIUM.Singular();
                    }
                    else if (machines.Contains(AI.CLOSEST_BLUEIUM))
                    {
                        machines = AI.CLOSEST_BLUEIUM.Singular();
                    }
                    Solver.MoveAndDrop(intern, machines.Select(m => m.Tile), dropType.Singular());
                    Solver.MoveAndDrop(intern, AI.PLAYER.Units.Where(u => {
                        var n = u.NeighborMachines().FirstOrDefault();
                        return n != null && n.OreType == machineType && (u.Job == AI.INTERN || u.Job == AI.PHYSICIST);
                    }).Select(m => m.Tile), dropType.Singular());
                }
            }
        }

        public void RunManagers()
        {
            var goalTypes = new[] { AI.REDIUM, AI.BLUEIUM };

            List<Unit> allManagers = this.Player.Units.Where(u => u != null && u.Tile != null && u.StunTime == 0 && u.Job == AI.MANAGER).ToList();
            var mules = allManagers.Where(m => m.FullCapacity() == 0).ToList();
            var orentTiles = this.Game.Tiles.Where(t => (t.Redium > 0 || t.Blueium > 0) && t.Type != "generator").ToList();
            while (orentTiles.Any())
            {
                // assign close managers
                var shortestPath = Solver.ShortestPath(mules.Select(m => m.Tile.ToPoint()), orentTiles.Select(t => t.ToPoint()));
                if (shortestPath.Count() < 2)
                {
                    break;
                }
                var closestManager = shortestPath.First().ToTile().Unit;
                var orentTile = shortestPath.Last().ToTile();
                Solver.MoveAndPickup(closestManager, orentTile.Singular().ToHashSet(), goalTypes);
                allManagers.Remove(closestManager);
                mules.Remove(closestManager);
                orentTiles.Remove(orentTile);
                Congratulate(closestManager);
            }
            foreach (var manager in allManagers)
            {
                if (manager.FullCapacity() > 0)
                {
                    Solver.MoveAndDrop(manager, this.Player.GeneratorTiles, goalTypes);
                }
                IEnumerable<Machine> guardMachines = AI.GAME.Machines.Where(m => m.CanBeWorked());
                if (!guardMachines.Any())
                {
                    guardMachines = AI.GAME.Machines.Where(m => m.Tile.RediumOre > 0 || m.Tile.BlueiumOre > 0);
                    if (!guardMachines.Any())
                    {
                        guardMachines = ((this.Player == this.Game.Players[0]) ? AI.CLOSEST_BLUEIUM : AI.CLOSEST_REDIUM).Singular();
                    }
                }
                Solver.Move(manager, guardMachines.SelectMany(m => m.Tile.GetNeighbors().Where(t => t.IsPathable() || t.Unit != null && t.Unit.Owner != manager.Owner)).Select(t => t.ToPoint()).ToHashSet());
                Solver.Move(manager, guardMachines.SelectMany(m => m.Tile.GetNeighbors()).Select(t => t.ToPoint()).ToHashSet());
                Congratulate(manager);
            }
        }

        public void Congratulate(Unit m)
        {
            if (m.Tile.GetNeighbors().Any(t => t.Unit != null && t.Unit.Job == AI.PHYSICIST && t.Unit.Acted && t.Unit.Owner == m.Owner))
            {
                string[] surpriseSayings = { "Keep up the good work!",
                                    "Gordan Freeman, and about time, too.",
                                    "We make such a good team",
                                    "Remember your 360 wellness!",
                                    "Lotta good synergy in here!"  };
                int r = RANDOM.Next(surpriseSayings.Count());
                m.SetLog(surpriseSayings[r]);
            }
        }

        public void RunPhysicists()
        {
            foreach (var physicist in this.Player.Units.Where(u => u != null && u.Tile != null && u.StunTime == 0 && u.Job == AI.PHYSICIST))
            {
                if(physicist.FullCapacity() > 0)
                {
                    physicist.Drop(physicist.Tile, -1, AI.REDIUM);
                    physicist.Drop(physicist.Tile, -1, AI.BLUEIUM);
                }
                var neighborMachine = physicist.NeighborMachines().FirstOrDefault();
                while(neighborMachine != null && neighborMachine.OreType == AI.REDIUM && physicist.Tile.GetAmount(AI.REDIUMORE) > 0 && physicist.OpenCapacity() > 0)
                {
                    physicist.Pickup(physicist.Tile, -1, AI.REDIUMORE);
                    physicist.Drop(neighborMachine.Tile, -1, AI.REDIUMORE);
                }
                while (neighborMachine != null && neighborMachine.OreType == AI.BLUEIUM && physicist.Tile.GetAmount(AI.BLUEIUMORE) > 0 && physicist.OpenCapacity() > 0)
                {
                    physicist.Pickup(physicist.Tile, -1, AI.BLUEIUMORE);
                    physicist.Drop(neighborMachine.Tile, -1, AI.BLUEIUMORE);
                }
                IEnumerable<Point> targets = AI.GAME.Machines.Where(m => m.CanBeWorked()).Select(m => m.ToPoint());
                if (!targets.Any())
                {
                    targets = AI.GAME.Machines.Where(m => m.Tile.RediumOre > 0 || m.Tile.BlueiumOre > 0).Select(m => m.ToPoint());
                    if (!targets.Any())
                    {

                        targets = ((this.Player == this.Game.Players[0]) ? AI.CLOSEST_BLUEIUM : AI.CLOSEST_REDIUM).ToPoint().Singular();
                    }
                }
                Solver.Move(physicist, targets.ToHashSet());
                Solver.Work(physicist, AI.GAME.Machines);

                // Don't block machines, pickup and drop Oren't
                foreach (Machine neighbor in physicist.NeighborMachines())
                {
                    if (neighbor.IsBlocked() && neighbor.GetAmount(neighbor.OreType) > 0 && physicist.OpenCapacity() > 0)
                    {
                        physicist.Pickup(neighbor.Tile, -1, neighbor.OreType);
                        physicist.SetLog(physicist.FullCapacity().ToString());
                        physicist.Drop(physicist.Tile, -1, neighbor.OreType);
                        physicist.SetLog(physicist.FullCapacity().ToString());
                    }
                }
            }
        }

        public void CleanupPhysicists()
        {
            foreach (var physicist in this.Player.Units.Where(u => u.Job == AI.PHYSICIST))
            {
                var neighborMachine = physicist.NeighborMachines().FirstOrDefault();
                while (neighborMachine != null && neighborMachine.OreType == AI.REDIUM && physicist.Tile.GetAmount(AI.REDIUM) > 0 && physicist.OpenCapacity() > 0)
                {
                    physicist.Pickup(physicist.Tile, -1, AI.REDIUM);
                    physicist.Drop(neighborMachine.Tile, -1, AI.REDIUM);
                }
                while (neighborMachine != null && neighborMachine.OreType == AI.BLUEIUM && physicist.Tile.GetAmount(AI.BLUEIUM) > 0 && physicist.OpenCapacity() > 0)
                {
                    physicist.Pickup(physicist.Tile, -1, AI.BLUEIUM);
                    physicist.Drop(neighborMachine.Tile, -1, AI.BLUEIUM);
                }
            }
            return;
        }

        public void CleanupCleanupEverybodyEverywhere()
        {
            foreach (var unit in this.Player.Units.Where(u => u != null && u.Tile != null && u.StunTime == 0))
            {
                if (unit.OpenCapacity() > 0)
                {
                    if (unit.Tile.GetAmount(AI.REDIUM) > 0)
                    {
                        unit.Pickup(unit.Tile, -1, AI.REDIUM);
                        unit.SetLog(unit.FullCapacity().ToString());
                    }
                    if (unit.Tile.GetAmount(AI.BLUEIUM) > 0)
                    {
                        unit.Pickup(unit.Tile, -1, AI.BLUEIUM);
                        unit.SetLog(unit.FullCapacity().ToString());
                    }
                    if (unit.Tile.GetAmount(AI.REDIUMORE) > 0)
                    {
                        unit.Pickup(unit.Tile, -1, AI.REDIUMORE);
                        unit.SetLog(unit.FullCapacity().ToString());
                    }
                    if (unit.Tile.GetAmount(AI.BLUEIUMORE) > 0)
                    {
                        unit.Pickup(unit.Tile, -1, AI.BLUEIUMORE);
                        unit.SetLog(unit.FullCapacity().ToString());
                    }
                }
            }
        }

        public void LogInventories()
        {
            AI.PLAYER.Units.ForEach(u => u.SetLog(u.FullCapacity().ToString()));
        }
        public void LogEnemies()
        {
            var interns = AI.GAME.Units.Where(u => u.Job == AI.INTERN).ToArray();
            var phys = AI.GAME.Units.Where(u => u.Job == AI.PHYSICIST).ToArray();
            var mans = AI.GAME.Units.Where(u => u.Job == AI.MANAGER).ToArray();
            var ri = RANDOM.Next(Math.Min(interns.Count(), Math.Min(phys.Count(), Math.Min(mans.Count(), 3))));
            for (var i = 0; i < ri; i++)
            {
                var randIntern = RANDOM.Next(interns.Count());
                LogIntern(interns[randIntern]);
            }

            for (var i = 0; i < ri; i++)
            {
                var randPhys = RANDOM.Next(interns.Count());
                LogPhysicist(phys[randPhys]);
            }

            for (var i = 0; i < ri; i++)
            {
                var randMan = RANDOM.Next(mans.Count());
                LogIntern(mans[randMan]);
            }
        }

        public void LogIntern(Unit u)
        {
            string[] internSayings = { "I should be in charge of this dump.",
                                       "I wonder how much free coffee I can drink in a day.",
                                       "What does '360 wellness' even mean??",
                                       "Do you think anybody's noticed that I haven't figured out Git yet?"};
            int r = RANDOM.Next(internSayings.Count());
            u.SetLog(internSayings[r]);
            return;
        }

        public void LogPhysicist(Unit u)
        {
            string[] physSayings = { "Morning, Mr. Freeman",
                                     "If the Silver Surfer and Iron Man team up, they'd be Alloys.",
                                     ""};
            int r = RANDOM.Next(physSayings.Count());
            u.SetLog(physSayings[r]);
            return;
        }

        public void LogManager(Unit u)
        {
            string[] manSayings = { "hurr durr",
                                    "pbbbbbbbtttt",
                                    "We make such a good team",
                                    "Remember your 360 wellness!",
                                    "Investigations hurt science.",
                                    "Lotta good synergy in here!" };
            int r = RANDOM.Next(manSayings.Count());
            u.SetLog(manSayings[r]);
            return;
        }

        public void ClearUnitLogs()
        {
            AI.GAME.Units.ForEach(u => u.SetLog(""));
        }

        public void Dos()
        {
            var interns = this.Player.UsableUnits().Where(u => u.Job == AI.INTERN).ToList();
            while (true)
            {
                var workableMachines = this.Game.Machines.Where(m => m.CanBeWorked() && m.Worked > 1);
                var path = Solver.ShortestPath(workableMachines.Select(m => m.ToPoint()), interns.Select(i => i.ToPoint()));
                if (path.Count() >= 1)
                {
                    var machinePoint = path.First();
                    var intern = path.Last().ToTile().Unit;
                    Solver.Move(intern, machinePoint.Singular().ToHashSet());
                    if (intern.Tile.HasNeighbor(machinePoint.ToTile()))
                    {
                        intern.Act(machinePoint.ToTile());
                    }
                    interns.Remove(intern);
                }
                else
                {
                    break;
                }
            }
        }

        public void Score()
        {
            var scoreTypes = new[] { AI.REDIUM, AI.BLUEIUM };
            var scorers = this.Player.UsableScorers().Where(u => u.OpenCapacity() > 0 && u.Moves > 0).ToList();
            while (true)
            {
                var refinedTiles = this.Game.Tiles.Where(t => (t.Blueium > 0 || t.Redium > 0) && t.Type != "generator");
                var path = Solver.ShortestPath(refinedTiles.Select(s => s.ToPoint()), scorers.Select(s => s.ToPoint()));
                if (path.Count() >= 1)
                {
                    var tile = path.First().ToTile();
                    var scorer = path.Last().ToTile().Unit;
                    Solver.MoveAndPickup(scorer, tile.Singular(), scoreTypes);
                    scorers.Remove(scorer);
                }
                else
                {
                    break;
                }
            }

            foreach (var unit in this.Player.UsableUnits().Where(u => u.Blueium > 0 || u.Redium > 0))
            {
                Solver.MoveAndDrop(unit, this.Player.GeneratorTiles, scoreTypes);
            }
        }
        #endregion
    }
}
