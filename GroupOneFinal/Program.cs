/***************************************************************
 * MAIN PROGRAM
 * 
 * CS 3110-C1
 * Group 1 - Kari Tyitye, Katelyn Stearn, Joyce Oldham
 * This program runs a game of Battleship with and enemy AI.
 * The player inputs coordinates and tries to find all of the
 * enemy ship locations to win the game.
 **************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace BattleShipFinal
{
    class Program
    {
        //Main method.
        static void Main(string[] args)
        {
            //Hide enemy ships position.
            bool isShowShips = false;

            //Player and enemy ship fleets.
            Fleet PlayerFleet = new Fleet();
            Fleet EnemyFleet = new Fleet();

            Dictionary<char, int> Coordinates = PopulateDictionary();
            PrintHeader();
            PrintIndentToRightGrid();

            //Print Maps.
            PrintMap(PlayerFleet.FirePositions, PlayerFleet, EnemyFleet, isShowShips);

            int Game;

            //User input for coordinates.
            for (Game = 1; Game < 101; Game++)
            {
                PlayerFleet.StepsTaken++;

                ShipPosition position = new ShipPosition();

                //Ask for user input for bomb coordinate.
                ForegroundColor = ConsoleColor.White;
                WriteLine("Enter coordinates for bomb (e.g. A3).");
                string input = ReadLine();
                position = AnalyzeInput(input, Coordinates);

                //If coordinate is out of bounds, ask for new coordinate.
                if (position.x == -1 || position.y == -1)
                {
                    WriteLine("This coordinate is out of bounds!");
                    Game--;
                    continue;
                }

                //If coordinate was chosen once already, ask for new coordinate.
                if (PlayerFleet.FirePositions.Any(EFP => EFP.x == position.x && EFP.y == position.y))
                {
                    WriteLine("This coordinate was already chosen!");
                    Game--;
                    continue;
                }

                //Enemy chooses random coordinate and moves.
                EnemyFleet.Fire();

                var index = PlayerFleet.FirePositions.FindIndex(p => p.x == position.x && p.y == position.y);

                if (index == -1)
                    PlayerFleet.FirePositions.Add(position);

                Clear();

                PlayerFleet.AllShipsPosition.OrderBy(o => o.x).ThenBy(n => n.y).ToList();
                PlayerFleet.CheckShipStatus(EnemyFleet.FirePositions);

                EnemyFleet.AllShipsPosition.OrderBy(o => o.x).ThenBy(n => n.y).ToList();
                EnemyFleet.CheckShipStatus(PlayerFleet.FirePositions);

                PrintHeader();
                for (int h = 0; h < 19; h++)
                {
                    Write(" ");
                }

                PrintMap(PlayerFleet.FirePositions, PlayerFleet, EnemyFleet, isShowShips);

                Commentator(PlayerFleet, true);
                Commentator(EnemyFleet, false);
                if (EnemyFleet.IsObliteratedAll || PlayerFleet.IsObliteratedAll) { break; }
            }

            ForegroundColor = ConsoleColor.White;
            //If player wins.
            if (EnemyFleet.IsObliteratedAll && !PlayerFleet.IsObliteratedAll)
            {
                WriteLine("Game Ended, you win!");
            }
            //If player loses.
            else if (!EnemyFleet.IsObliteratedAll && PlayerFleet.IsObliteratedAll)
            {
                WriteLine("Game Ended, you lose.");
            }
            //If game is a draw.
            else
            {
                WriteLine("Game Ended, draw.");
            }
            //Print amount of moves it took to end game.
            WriteLine("Total moves taken:{0} ", Game);
            ReadLine();
        }

        //Print legend next to grid.
        static void PrintStatistic(int x, int y, Fleet fleet)
        {
            //Title.
            if (x == 1 && y == 10)
            {
                ForegroundColor = ConsoleColor.White;
                Write("Ships left:   ");
            }

            //Carrier ship.
            if (x == 2 && y == 10)
            {
                //If sunk, display in red.
                if (fleet.IsCarrierSunk)
                {
                    ForegroundColor = ConsoleColor.Red;
                    Write("CARRIER       ");
                }
                //Display green.
                else
                {
                    ForegroundColor = ConsoleColor.DarkGreen;
                    Write("CARRIER       ");
                }

            }

            //Battleship.
            if (x == 3 && y == 10)
            {
                //If sunk, display in red.
                if (fleet.IsBattleshipSunk)
                {
                    ForegroundColor = ConsoleColor.Red;
                    Write("BATTLESHIP    ");
                }
                //Display green.
                else
                {
                    ForegroundColor = ConsoleColor.DarkGreen;
                    Write("BATTLESHIP    ");
                }
            }

            //Destroyer.
            if (x == 4 && y == 10)
            {
                //If sunk, display in red.
                if (fleet.IsDestroyerSunk)
                {
                    ForegroundColor = ConsoleColor.Red;
                    Write("DESTROYER     ");
                }
                //Display green.
                else
                {
                    ForegroundColor = ConsoleColor.DarkGreen;
                    Write("DESTROYER     ");
                }
            }

            //Submarine.
            if (x == 5 && y == 10)
            {
                //If sunk, display in red.
                if (fleet.IsSubmarineSunk)
                {
                    ForegroundColor = ConsoleColor.Red;
                    Write("SUBMARINE     ");
                }
                //Display green.
                else
                {
                    ForegroundColor = ConsoleColor.DarkGreen;
                    Write("SUBMARINE     ");
                }
            }

            //Patrol Boat.
            if (x == 6 && y == 10)
            {
                //If sunk, display in red.
                if (fleet.IsPatrolBoatSunk)
                {
                    ForegroundColor = ConsoleColor.Red;
                    Write("PATROL BOAT   ");
                }
                //Display green.
                else
                {
                    ForegroundColor = ConsoleColor.DarkGreen;
                    Write("PATROL BOAT   ");
                }
            }

            //Space to align grid under legend.
            if (x > 6 && y == 10)
            {
                for (int i = 0; i < 14; i++)
                {
                    Write(" ");
                }
            }
        }

        //Print Player Grid.
        static void PrintMap(List<ShipPosition> positions, Fleet PlayerFleet, Fleet EnemyFleet, bool showEnemyShips)
        {
            //Border around labels.
            string border1 = ("---"); //UPDATE - Made this so it fits the space in the front and added a dash for the extra long 10 column
            string border2 = ("#---#---#---#---#---#---#---#---#---#---#");
            PrintHeader();
            WriteLine();
            if (!showEnemyShips)
                showEnemyShips = PlayerFleet.IsObliteratedAll;

            List<ShipPosition> SortedLFirePositions = positions.OrderBy(o => o.x).ThenBy(n => n.y).ToList();
            List<ShipPosition> SortedShipsPositions = EnemyFleet.AllShipsPosition.OrderBy(o => o.x).ThenBy(n => n.y).ToList();

            SortedShipsPositions = SortedShipsPositions.Where(FP => !SortedLFirePositions.Exists(ShipPos => ShipPos.x == FP.x && ShipPos.y == FP.y)).ToList();

            int hitCounter = 0;
            int EnemyshipCounter = 0;
            int myShipCounter = 0;
            int EnemyHitCounter = 0;
            char row = 'A';

            try
            {
                for (int x = 1; x < 11; x++)
                {
                    for (int y = 1; y < 11; y++)
                    {
                        bool keepGoing = true;

                        #region row indicator
                        if (y == 1)
                        {
                            //Print border for header on left.
                            ForegroundColor = ConsoleColor.Yellow;
                            Write(border1);
                            ForegroundColor = ConsoleColor.DarkCyan;
                            Write(border2);

                            //Space between grids.
                            PrintIndentToRightGrid();

                            //Print border for header on right.
                            ForegroundColor = ConsoleColor.Yellow;
                            Write(border1);
                            ForegroundColor = ConsoleColor.DarkCyan;
                            WriteLine(border2);

                            //Add row numbers.
                            ForegroundColor = ConsoleColor.Yellow;
                            Write(" " + row + " ");
                        }
                        #endregion

                        //If statements for ship locations on grid.
                        if (SortedLFirePositions.Count != 0 && SortedLFirePositions[hitCounter].x == x && SortedLFirePositions[hitCounter].y == y)
                        {

                            if (SortedLFirePositions.Count - 1 > hitCounter)
                                hitCounter++;

                            //If a HIT, print a red X.
                            if (EnemyFleet.AllShipsPosition.Exists(ShipPos => ShipPos.x == x && ShipPos.y == y))
                            {
                                ForegroundColor = ConsoleColor.DarkCyan;
                                Write("| ");
                                ForegroundColor = ConsoleColor.Red;
                                Write("X ");

                                //Add square border in proper color of grid.
                                if (y == 10)
                                {
                                    ForegroundColor = ConsoleColor.DarkCyan;
                                    Write("|");
                                }
                                keepGoing = false;
                            }
                            //If a MISS, print a gray X.
                            else
                            {
                                ForegroundColor= ConsoleColor.DarkCyan;
                                Write("| ");
                                ForegroundColor = ConsoleColor.Gray;
                                Write("X ");

                                //Add square border in proper color of grid.
                                if (y == 10)
                                {
                                    ForegroundColor = ConsoleColor.DarkCyan;
                                    Write("|");
                                }
                                keepGoing = false;
                            }
                        }

                        if (keepGoing && showEnemyShips && SortedShipsPositions.Count != 0 && SortedShipsPositions[EnemyshipCounter].x == x && SortedShipsPositions[EnemyshipCounter].y == y)
                        {
                            //Show player ship positions on grid with S.
                            if (SortedShipsPositions.Count - 1 > EnemyshipCounter)
                                EnemyshipCounter++;

                            ForegroundColor = ConsoleColor.DarkCyan;
                            Write("| ");
                            ForegroundColor = ConsoleColor.Green;
                            Write("S ");

                            if (y == 10)
                            {
                                Write("|");
                            }
                            keepGoing = false;
                        }

                        if (keepGoing)
                        {
                            //Print the grid sqares.
                            ForegroundColor = ConsoleColor.DarkCyan;
                            Write("| . ");

                            if (y == 10)
                            {
                                Write("|");
                            }
                        }

                        //Print legend.
                        PrintStatistic(x, y, PlayerFleet);

                        //Space between grids.
                        if (y == 10)
                        {
                            Write("   ");
                            PrintEnemyRow(x, row, PlayerFleet, EnemyFleet, ref myShipCounter, ref EnemyHitCounter);
                            
                            //Add row numbers.
                            row++;
                        }
                    }
                    WriteLine();
                }
            }

            //Exception catch.
            catch (Exception e)
            {
                string error = e.Message.ToString();
            }

            //Print border on left.
            ForegroundColor = ConsoleColor.Yellow;
            Write(border1);
            ForegroundColor = ConsoleColor.DarkCyan;
            Write(border2);

            //Space between grids.
            PrintIndentToRightGrid();

            //Print border on right.
            ForegroundColor = ConsoleColor.Yellow;
            Write(border1);
            ForegroundColor = ConsoleColor.DarkCyan;
            WriteLine(border2);
        }

        //Print enemy grid.
        static void PrintEnemyRow(int x, char row, Fleet PlayerFleet, Fleet EnemyFleet, ref int MyshipCounter, ref int EnemyHitCounter)
        {
            //Random ship positions.
            List<ShipPosition> EnemyFirePositions = new List<ShipPosition>();
            Random random = new Random();
            List<ShipPosition> SortedLFirePositions = EnemyFleet.FirePositions.OrderBy(o => o.x).ThenBy(n => n.y).ToList();
            List<ShipPosition> SortedLShipsPositions = PlayerFleet.AllShipsPosition.OrderBy(o => o.x).ThenBy(n => n.y).ToList();

            SortedLShipsPositions = SortedLShipsPositions.Where(FP => !SortedLFirePositions.Exists(ShipPos => ShipPos.x == FP.x && ShipPos.y == FP.y)).ToList();

            try
            {
                //Row labels.
                ForegroundColor = ConsoleColor.Yellow;
                Write(" " + row + " ");

                for (int y = 1; y < 11; y++)
                {
                    bool keepGoing = true;

                    if (SortedLFirePositions.Count != 0 && SortedLFirePositions[EnemyHitCounter].x == x && SortedLFirePositions[EnemyHitCounter].y == y)
                    {

                        if (SortedLFirePositions.Count - 1 > EnemyHitCounter)
                            EnemyHitCounter++;

                        //If a HIT, print a red X.
                        if (PlayerFleet.AllShipsPosition.Exists(ShipPos => ShipPos.x == x && ShipPos.y == y))
                        {
                            ForegroundColor = ConsoleColor.DarkCyan;
                            Write("| ");
                            ForegroundColor = ConsoleColor.Red;
                            Write("X ");

                            //Add square border in proper color of grid.
                            if (y == 10)
                            {
                                ForegroundColor = ConsoleColor.DarkCyan;
                                Write("|");
                            }
                            keepGoing = false;
                        }
                        //If a MISS, print a gray X.
                        else
                        {
                            ForegroundColor = ConsoleColor.DarkCyan;
                            Write("| ");
                            ForegroundColor = ConsoleColor.Gray;
                            Write("X ");

                            //Add square border in proper color of grid.
                            if (y == 10)
                            {
                                ForegroundColor = ConsoleColor.DarkCyan;
                                Write("|");
                            }
                            keepGoing = false;
                        }
                    }

                    if (keepGoing && SortedLShipsPositions.Count != 0 && SortedLShipsPositions[MyshipCounter].x == x && SortedLShipsPositions[MyshipCounter].y == y)
                    {
                        //Show player ship positions on grid with S.
                        if (SortedLShipsPositions.Count - 1 > MyshipCounter)
                            MyshipCounter++;

                        ForegroundColor = ConsoleColor.DarkCyan;
                        Write("| ");
                        ForegroundColor = ConsoleColor.Green;
                        Write("S ");

                        if (y == 10)
                        {
                            ForegroundColor = ConsoleColor.DarkCyan;
                            Write("|");
                        }
                        keepGoing = false;
                    }

                    if (keepGoing)
                    {
                        //Print the grid squares.
                        ForegroundColor = ConsoleColor.DarkCyan;
                        Write("| . ");

                        if (y == 10)
                        {
                            Write("|");
                        }
                    }
                    //Print legend.
                    PrintStatistic(x, y, EnemyFleet);
                }
            }

            //Exception catch.
            catch (Exception e)
            {
                string error = e.Message.ToString();
            }
        }

        //Coordinate positions taken and analyzed.
        static ShipPosition AnalyzeInput(string input, Dictionary<char, int> Coordinates)
        {
            ShipPosition pos = new ShipPosition();

            char[] inputSplit = input.ToUpper().ToCharArray();

            if (inputSplit.Length < 2 || inputSplit.Length > 4)
            {
                return pos;
            }

            if (Coordinates.TryGetValue(inputSplit[0], out int value))
            {
                pos.x = value;
            }
            else
            {
                return pos;
            }

            if (inputSplit.Length == 3)
            {
                if (inputSplit[1] == '1' && inputSplit[2] == '0')
                {
                    pos.y = 10;
                    return pos;
                }
                else
                {
                    return pos;
                }
            }

            if (inputSplit[1] - '0' > 9)
            {
                return pos;
            }
            else
            {
                pos.y = inputSplit[1] - '0';
            }
            return pos;
        }

        //Print header to grids.
        static void PrintHeader()
        {
            ForegroundColor = ConsoleColor.Yellow;
            Write("   ");
            for (int i = 1; i < 11; i++)
            {
                if (i == 0)
                {
                    Console.Write("   |");
                }
                else if (i < 10)
                {
                    //One digit.
                    Console.Write("| " + i + " ");
                }
                else
                {
                    //Two digits.
                    Console.Write("| " + i);
                }
            }
            Write("|");
        }

        //Dictionary for coordinates.
        static Dictionary<char, int> PopulateDictionary()
        {
            Dictionary<char, int> Coordinate =
                     new Dictionary<char, int>
                     {
                         { 'A', 1 },
                         { 'B', 2 },
                         { 'C', 3 },
                         { 'D', 4 },
                         { 'E', 5 },
                         { 'F', 6 },
                         { 'G', 7 },
                         { 'H', 8 },
                         { 'I', 9 },
                         { 'J', 10 }
                     };
            return Coordinate;
        }

        //Output when ships are sank whether enemy ship or player ship.
        static void Commentator(Fleet shipFleet, bool isMyShip)
        {

            string title = isMyShip ? "Your" : "Enemy";

            //If Battleship sunk.
            if (shipFleet.CheckPBattleship && shipFleet.IsBattleshipSunk)
            {
                ForegroundColor = ConsoleColor.DarkRed;
                WriteLine("{0} {1} has been sunk!", title, nameof(shipFleet.Battleship));
                shipFleet.CheckPBattleship = false;
            }

            //If Carrier sunk.
            if (shipFleet.CheckCarrier && shipFleet.IsCarrierSunk)
            {
                ForegroundColor = ConsoleColor.DarkRed;
                WriteLine("{0} {1} has been sunk!", title, nameof(shipFleet.Carrier));
                shipFleet.CheckCarrier = false;
            }

            //If Destroyer sunk.
            if (shipFleet.CheckDestroyer && shipFleet.IsDestroyerSunk)
            {
                ForegroundColor = ConsoleColor.DarkRed;
                WriteLine("{0} {1} has been sunk!", title, nameof(shipFleet.Destroyer));
                shipFleet.CheckDestroyer = false;
            }

            //If Patrol Boat sunk.
            if (shipFleet.CheckPatrolBoat && shipFleet.IsPatrolBoatSunk)
            {
                ForegroundColor = ConsoleColor.DarkRed;
                WriteLine("{0} {1} has been sunk!", title, nameof(shipFleet.PatrolBoat));
                shipFleet.CheckPatrolBoat = false;
            }

            //If submarine sunk.
            if (shipFleet.CheckSubmarine && shipFleet.IsSubmarineSunk)
            {
                ForegroundColor = ConsoleColor.DarkRed;
                WriteLine("{0} {1} has been sunk!", title, nameof(shipFleet.Submarine));
                shipFleet.CheckSubmarine = false;
            }
        }
        
        //Prints the spaces between the grids.
        static void PrintIndentToRightGrid()
        {
            for (int h = 0; h < 17; h++)
            {
                Write(" ");
            }
        }
    }
}