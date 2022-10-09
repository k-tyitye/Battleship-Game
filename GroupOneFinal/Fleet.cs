/***************************************************************
 * SHIP FLEET FOR PLAYERS
 * 
 * CS 3110-C1
 * Group 1 - Kari Tyitye, Katelyn Stearn, Joyce Oldham
 **************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleShipFinal
{
    class Fleet
    {
        //Randomize ships.
        Random random = new Random();
        
        //Ship sizes.
        private const int CARRIER = 5;
        private const int BATTLESHIP = 4;
        private const int DESTROYER = 3;
        private const int SUBMARINE = 3;
        private const int PATROLBOAT = 2;

        //Generate ship positions.
        public Fleet()
        {
            Carrier = GeneratePosistion(CARRIER, AllShipsPosition);
            Battleship = GeneratePosistion(BATTLESHIP, AllShipsPosition);
            Destroyer = GeneratePosistion(DESTROYER, AllShipsPosition);
            Submarine = GeneratePosistion(SUBMARINE, AllShipsPosition);
            PatrolBoat = GeneratePosistion(PATROLBOAT, AllShipsPosition);
        }

        //Steps to end game.
        public int StepsTaken { get; set; } = 0;

        //Set ship locations.
        public List<ShipPosition> Carrier { get; set; }//5
        public List<ShipPosition> Battleship { get; set; }//4
        public List<ShipPosition> Destroyer { get; set; }//3
        public List<ShipPosition> Submarine { get; set; }//3
        public List<ShipPosition> PatrolBoat { get; set; }//2
        public List<ShipPosition> AllShipsPosition { get; set; } = new List<ShipPosition>();
        public List<ShipPosition> FirePositions { get; set; } = new List<ShipPosition>();

        //Are ships sunk?
        public bool IsCarrierSunk { get; set; } = false;
        public bool IsBattleshipSunk { get; set; } = false;
        public bool IsDestroyerSunk { get; set; } = false;
        public bool IsSubmarineSunk { get; set; } = false;
        public bool IsPatrolBoatSunk { get; set; } = false;
        public bool IsObliteratedAll { get; set; } = false;

        public bool CheckCarrier { get; set; } = true;
        public bool CheckPBattleship { get; set; } = true;
        public bool CheckDestroyer { get; set; } = true;
        public bool CheckSubmarine { get; set; } = true;
        public bool CheckPatrolBoat { get; set; } = true;

        //Checks for ship status if they are sunk or not.
        public Fleet CheckShipStatus(List<ShipPosition> HitPositions)
        {
            IsCarrierSunk = Carrier.Where(C => !HitPositions.Any(H => C.x == H.x && C.y == H.y)).ToList().Count == 0;
            IsBattleshipSunk = Battleship.Where(B => !HitPositions.Any(H => B.x == H.x && B.y == H.y)).ToList().Count == 0;
            IsDestroyerSunk = Destroyer.Where(D => !HitPositions.Any(H => D.x == H.x && D.y == H.y)).ToList().Count == 0;
            IsSubmarineSunk = Submarine.Where(S => !HitPositions.Any(H => S.x == H.x && S.y == H.y)).ToList().Count == 0;
            IsPatrolBoatSunk = PatrolBoat.Where(P => !HitPositions.Any(H => P.x == H.x && P.y == H.y)).ToList().Count == 0;
            IsObliteratedAll = IsCarrierSunk && IsBattleshipSunk && IsDestroyerSunk && IsSubmarineSunk && IsPatrolBoatSunk;
            return this;
        }

        //Generates the random positions for the ships.
        public List<ShipPosition> GeneratePosistion(int size, List<ShipPosition> AllPosition)
        {
            List<ShipPosition> positions = new List<ShipPosition>();

            bool IsExist = false;

            do
            {
                positions = GeneratePositionRandomly(size);
                IsExist = positions.Where(AP => AllPosition.Exists(ShipPos => ShipPos.x == AP.x && ShipPos.y == AP.y)).Any();
            }
            while (IsExist);
            AllPosition.AddRange(positions);

            return positions;
        }

        //Generates the random positions for the ships.
        public List<ShipPosition> GeneratePositionRandomly(int size)
        {
            List<ShipPosition> positions = new List<ShipPosition>();

            int direction = random.Next(1, size);
            int row = random.Next(1, 11);
            int col = random.Next(1, 11);

            if (direction % 2 != 0)
            {
                if (row - size > 0)
                {
                    for (int i = 0; i < size; i++)
                    {
                        ShipPosition pos = new ShipPosition();
                        pos.x = row - i;
                        pos.y = col;
                        positions.Add(pos);
                    }
                }
                else
                {
                    for (int i = 0; i < size; i++)
                    {
                        ShipPosition pos = new ShipPosition();
                        pos.x = row + i;
                        pos.y = col;
                        positions.Add(pos);
                    }
                }
            }
            else
            {
                if (col - size > 0)
                {
                    for (int i = 0; i < size; i++)
                    {
                        ShipPosition pos = new ShipPosition();
                        pos.x = row;
                        pos.y = col - i;
                        positions.Add(pos);
                    }
                }
                else
                {
                    for (int i = 0; i < size; i++)
                    {
                        ShipPosition pos = new ShipPosition();
                        pos.x = row;
                        pos.y = col + i;
                        positions.Add(pos);
                    }
                }
            }
            return positions;
        }

        //Random shots fired for enemy.
        public Fleet Fire()
        {
            ShipPosition EnemyShootPos = new ShipPosition();
            bool alreadyShot = false;
            do
            {
                EnemyShootPos.x = random.Next(1, 11);
                EnemyShootPos.y = random.Next(1, 11);
                alreadyShot = FirePositions.Any(EFP => EFP.x == EnemyShootPos.x && EFP.y == EnemyShootPos.y);
            }
            while (alreadyShot);

            FirePositions.Add(EnemyShootPos);
            return this;
        }
    }
}
