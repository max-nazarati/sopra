using System;
using System.Collections.Generic;
using KernelPanic.Entities;
using KernelPanic.Entities.Units;
using KernelPanic.Events;
using KernelPanic.Players;
using KernelPanic.Purchasing;
using Microsoft.Xna.Framework;

namespace KernelPanic.ArtificialIntelligence
{
    internal sealed class AttackPlanner : Planner
    {
        private readonly Dictionary<Type, PurchasableAction<Unit>> mActions;
        private readonly double[] mUnitDistribution;
        private const int NumberOfUnits = 8;
        private const int IndexBug = 0;
        private const int IndexVirus = 1;
        private const int IndexTrojaner = 2;
        private const int IndexNokia = 3;
        private const int IndexThunderbird = 4;
        private const int IndexSettings = 5;
        private const int IndexFirefox = 6;
        private const int IndexBluescreen = 7;

        #region Konstruktor

        public AttackPlanner(Player player, Dictionary<Type, PurchasableAction<Unit>> actions) : base(player)
        {
            mActions = actions;
            mUnitDistribution = new[] {1/8d, 1/8d, 1/8d, 1/8d, 1/8d, 1/8d, 1/8d, 1/8d};
            var eventCenter = EventCenter.Default;
            eventCenter.Subscribe(Event.Id.DamagedBase,
                e => UpdateHeuristic(Event.Id.DamagedBase, e),
                e => e.IsActivePlayer(Event.Key.Defender));
            eventCenter.Subscribe(Event.Id.KilledUnit,
                e => UpdateHeuristic(Event.Id.KilledUnit, e),
                e => !e.IsActivePlayer(Event.Key.Attacker));
            // TODO delete those calls after Heros are finished
            // BuyEntity<Firefox>();
            BuyEntity<Settings>();
            BuyEntity<Bluescreen>();
            BuyEntity<Bug>();
        }

        #endregion

        private void BuyEntity<T>(int amount=1) where T : Unit
        {
            for (var i = 0; i < amount; i++)
            {
                mActions[typeof(T)].TryPurchase(mPlayer);
            }
        }

        #region Heuristic

        /// <summary>
        /// Choose randomly a unit based on distribution - this is done
        /// by generating a number in [0, 1] and compute the corresponding
        /// choice, e.g.:
        /// mUnitDistribution = [1/2, 1/4, 1/24, 1/24, 1/24, 1/24, 1/24, 1/24]
        /// is isomorph to:
        /// [0, 1/2] x (1/2, 3/4] x (3/4, 19/24] x (19/24, 20/24] x (20/24, 21/24] x (21/24, 22/24] x (22/24, 23/24] x (23/24, 1]
        ///  Bug          Virus        Trojaner         Nokia         Thunderbird       Settings        Firefox        Bluescreen
        /// </summary>
        /// <returns></returns>
        public string MakeChoice()
        {
            string[] choices = new[] { "Bug", "Virus", "Trojaner", "Nokia", "Thunderbird", "Settings", "Firefox", "Bluescreen" };

            Random numberGenerator = new Random();
            double probability = numberGenerator.NextDouble();
            int choiceIndex = 0;
            double upperBound = mUnitDistribution[0];
            for (int i = 1; i < 8; i++)
            {
                if (probability <= upperBound) break;
                else
                {
                    choiceIndex++;
                    upperBound += mUnitDistribution[i];
                }
            }

            return choices[choiceIndex];
        }

        private int UnitTypeToIndex(Unit type)
        {
            int index;
            switch (type)
            {
                case Bug _:
                    index = IndexBug;
                    break;
                case Virus _:
                    index = IndexVirus;
                    break;
                case Trojan _:
                    index = IndexTrojaner;
                    break;
                case Nokia _:
                    index = IndexNokia;
                    break;
                case Thunderbird _:
                    index = IndexThunderbird;
                    break;
                case Settings _:
                    index = IndexSettings;
                    break;
                case Firefox _:
                    index = IndexFirefox;
                    break;
                case Bluescreen _:
                    index = IndexBluescreen;
                    break;
                default:
                    index = -1;
                    break;
            }

            return index;
        }

        private void UpdateHeuristic(Event.Id id, Event handler)
        {
            double weight;
            Unit unitType;
            switch (id)
            {
                case Event.Id.DamagedBase:
                    weight = 0.005;
                    unitType = handler.Get<Unit>(Event.Key.Unit);
                    IncreaseUnitProbability(UnitTypeToIndex(unitType), weight);
                    break;
                case Event.Id.KilledUnit:
                    weight = 0.0001;
                    unitType = handler.Get<Unit>(Event.Key.Unit);
                    DecreaseUnitProbability(UnitTypeToIndex(unitType), weight);
                    break;
            }
        }

        public void IncreaseUnitProbability(int unitIndex, double probability)
        {
            if (mUnitDistribution[unitIndex] >= 1)
            {
                return;
            }

            mUnitDistribution[unitIndex] += probability;
            for (int i = 0; i < NumberOfUnits; i++)
            {
                mUnitDistribution[i] /= (1 + probability);
            }
        }

        public void DecreaseUnitProbability(int unitIndex, double probability)
        {
            if (mUnitDistribution[unitIndex] <= 0)
            {
                return;
            }
            mUnitDistribution[unitIndex] -= probability;
            for (int i = 0; i < NumberOfUnits; i++)
            {
                mUnitDistribution[i] /= (1 - probability);
            }
        }

        #endregion

        #region Update


        public void BuySingleUnit(string choice)
        {
            switch (choice)
            {
                case "Bug":
                    BuyEntity<Bug>();
                    break;
                case "Virus":
                    BuyEntity<Virus>();
                    break;
                case "Trojaner":
                    BuyEntity<Trojan>();
                    break;
                case "Nokia":
                    BuyEntity<Nokia>();
                    break;
                case "Thunderbird":
                    BuyEntity<Thunderbird>();
                    break;
                case "Settings":
                    BuyEntity<Settings>();
                    break;
                case "Firefox":
                    BuyEntity<Firefox>();
                    break;
                case "Bluescreen":
                    BuyEntity<Bluescreen>();
                    break;
            }
        }

        public void Update(int[] attackData, GameTime gameTime)
        {
            base.Update();
            var choice = MakeChoice();
            BuySingleUnit(choice);
            // Console.WriteLine("Distribution of Attackplanner: \n");
            // Console.WriteLine(DistributionToString());
            // Console.WriteLine("\n ========== \n");
        }

        #endregion


        #region Visualization
        
        /*
        public string DistributionToString()
        {
            string result = "";
            result += "Bug: " + mUnitDistribution[0] + "\n";
            result += "Virus: " + mUnitDistribution[1] + "\n";
            result += "Trojaner: " + mUnitDistribution[2] + "\n";
            result += "Nokia: " + mUnitDistribution[3] + "\n";
            result += "Thunderbird: " + mUnitDistribution[4] + "\n";
            result += "Settings: " + mUnitDistribution[5] + "\n";
            result += "Firefox: " + mUnitDistribution[6] + "\n";
            result += "Bluescreen: " + mUnitDistribution[7];

            return result;
        } */

        #endregion
    }
}