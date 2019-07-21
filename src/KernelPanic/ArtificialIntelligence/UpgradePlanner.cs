using System;
using System.Collections.Generic;
using KernelPanic.Players;
using KernelPanic.Purchasing;
using KernelPanic.Upgrades;

namespace KernelPanic.ArtificialIntelligence
{
    internal sealed class UpgradePlanner : Planner
    {
        private readonly Func<Upgrade.Id, SinglePurchasableAction<Upgrade>> mUpgradeLookup;
        private double[] mTierPriority;
        private List<double[]> mTierDistribution;
        private List<Upgrade.Id[]> mTierDictionnary;

        public UpgradePlanner(Player player, Func<Upgrade.Id, SinglePurchasableAction<Upgrade>> upgradeLookup) : base(player)
        {
            mUpgradeLookup = upgradeLookup;
            mTierPriority = new[] {0.8d, 0.05, 0.05, 0.05, 0.05};

            // Initialize distributions for each tier
            double[] tier1Distribution = new[] {1 / 4d, 1 / 4d, 1 / 4d, 1 / 4d};
            double[] tier2Distribution = new[] { 1 / 4d, 1 / 4d, 1 / 4d, 1 / 4d };
            double[] tier3Distribution = new[] { 1 / 4d, 1 / 4d, 1 / 4d, 1 / 4d };
            double[] tier4Distribution = new[] { 1 / 4d, 1 / 4d, 1 / 4d, 1 / 4d };
            double[] tier5Distribution = new[] { 1 / 4d, 1 / 4d, 1 / 4d, 1 / 4d };
            mTierDistribution = new List<double[]>();
            mTierDistribution.Add(new double[0]);
            mTierDistribution.Add(tier1Distribution);
            mTierDistribution.Add(tier2Distribution);
            mTierDistribution.Add(tier3Distribution);
            mTierDistribution.Add(tier4Distribution);
            mTierDistribution.Add(tier5Distribution);

            /*
             * initialize dictionnary for each upgrade - upgrade i of tier k can be accessed
             * by calling mTierDictionnary[k][i]
             */
            Upgrade.Id[] tier1Upgrades = new[]
                {Upgrade.Id.IncreaseLp1, Upgrade.Id.IncreaseGs1, Upgrade.Id.IncreaseVs1, Upgrade.Id.DecreaseAi1};
            Upgrade.Id[] tier2Upgrades = new[]
                {Upgrade.Id.IncreaseLp2, Upgrade.Id.IncreaseGs2, Upgrade.Id.IncreaseVs2, Upgrade.Id.IncreaseBitcoins};
            Upgrade.Id[] tier3Upgrades = new[]
            {
                Upgrade.Id.CdBoomerang, Upgrade.Id.IncreaseGsNokia, Upgrade.Id.IncreaseGsFirefox,
                Upgrade.Id.MoreTrojanChildren1
            };
            Upgrade.Id[] tier4Upgrades = new[]
            {
                Upgrade.Id.EmpDuration, Upgrade.Id.AdditionalFirefox1, Upgrade.Id.IncreaseSettingsArea1,
                Upgrade.Id.IncreaseSettingsHeal1, Upgrade.Id.MoreTrojanChildren2,
            };
            Upgrade.Id[] tier5Upgrades = new[]
            {
                Upgrade.Id.EmpTwoTargets, Upgrade.Id.BeginningTier5, Upgrade.Id.AdditionalFirefox2,
                Upgrade.Id.IncreaseSettingsArea2, Upgrade.Id.IncreaseSettingsHeal2
            };
            mTierDictionnary = new List<Upgrade.Id[]>();
            mTierDictionnary.Add(new Upgrade.Id[0]);
            mTierDictionnary.Add(tier1Upgrades);
            mTierDictionnary.Add(tier2Upgrades);
            mTierDictionnary.Add(tier3Upgrades);
            mTierDictionnary.Add(tier4Upgrades);
            mTierDictionnary.Add(tier5Upgrades);
        }

        private int ChooseTier()
        {
            Random numberGenerator = new Random();
            double number = numberGenerator.NextDouble();
            int tier = 1;
            double upperBound = mTierPriority[0];
            for (int i = 0; i < 4; i++)
            {
                if (number <= upperBound) break;
                else
                {
                    tier++;
                    upperBound += mTierPriority[i];
                }
            }

            return tier;
        }

        private Upgrade.Id MakeChoice(int tier)
        {
            Random numberGenerator = new Random();
            double probability = numberGenerator.NextDouble();
            int upgradeIndex = 0;

            // make choice based on tier & probability
            double upperBound = mTierDistribution[tier][0];
            for (int i = 1; i < mTierDistribution[tier].Length; i++)
            {
                if (probability <= upperBound) break;
                else
                {
                    upgradeIndex++;
                    upperBound += mTierDistribution[tier][i];
                }
            }

            return mTierDictionnary[tier][upgradeIndex];
        }

        private void BuyUpgrade(Upgrade.Id id)
        {
            SinglePurchasableAction<Upgrade> upgrade = mUpgradeLookup.Invoke(id);
            upgrade.TryPurchase(mPlayer);
        }
    }
}