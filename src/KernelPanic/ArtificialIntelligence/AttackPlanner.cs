using System;
using System.Collections.Generic;
using KernelPanic.Entities;
using KernelPanic.Entities.Units;
using KernelPanic.Players;
using KernelPanic.Purchasing;
using Microsoft.Xna.Framework;

namespace KernelPanic.ArtificialIntelligence
{
    internal sealed class AttackPlanner : Planner
    {
        private readonly Dictionary<Type, PurchasableAction<Unit>> mActions;
        private double[] mUnitDistribution;

        #region Konstruktor

        public AttackPlanner(Player player, Dictionary<Type, PurchasableAction<Unit>> actions) : base(player)
        {
            mActions = actions;
            mUnitDistribution = new[] {1/8d, 1/8d, 1/8d, 1/8d, 1/8d, 1/8d, 1/8d, 1/8d};
            BuyEntity<Firefox>();
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

        #region Update

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
            string[] choices = new[] {"Bug", "Virus", "Trojaner", "Nokia", "Thunderbird", "Settings", "Firefox", "Bluescreen"};

            Random numberGenerator = new Random();
            double number = numberGenerator.NextDouble();
            int choiceIndex = 0;
            double upperBound = mUnitDistribution[0];
            for (int i = 1; i < 8; i++)
            {
                if (number <= upperBound) break;
                else
                {
                    choiceIndex++;
                    upperBound += mUnitDistribution[i];
                }
            }

            return choices[choiceIndex];
        }

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
                // Console.WriteLine(String.Join(",", defenceData.Select(p => p.ToString()).ToArray()));
            BuySingleUnit(choice);
            // TroupeParade();
        }
        
        #endregion
        
    }
}