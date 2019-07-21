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
        /// <summary>
        /// mTierPriority sets the probability that tier k is chosen by the Upgradeplanner
        /// </summary>
        private double[] mTierPriority;
        
        /// <summary>
        /// mTierDistribution contains the distributions of upgrade probabilities for each tier,
        /// i.e. probability pij of upgrade j in tier i is stored in mTierDistribution[i][j]
        /// tier0: []
        /// tier1: [p11, p12, p13, p14]
        /// tier2: [p21, p22, p23, p24]
        /// tier3: [p31, p32, p33, p34, p35]
        /// tier4: [p41, p42, p43, p44]
        /// tier5: [p51, p52, p53, p54]
        /// </summary>
        private List<double[]> mTierDistribution;

        /// <summary>
        /// mTierDictionnary identifies the upgrade id of upgrade j in tier i as (i, j), i.e.
        /// this Upgrade.Id = mTierDictionnary[i][j]
        /// </summary>
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

            // initialize dictionnary of upgrades corresponding to their tiers
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

        /// <summary>
        /// Choose randomly a tier based on distribution - this is done
        /// by generating a number in [0, 1] and compute the corresponding
        /// choice, e.g.:
        /// mTierPriority = [0.8, 0.05, 0.05, 0.05, 0.05]
        /// is isomorph to:
        /// [0, 0.8] x (0.8, 0.85] x (0.85, 0.9] x (0.9, 0.95] x (0.95, 1]
        ///  Tier1        Tier2         Tier3         Tier4        Tier5
        /// </summary>
        /// <returns></returns>
        private int ChooseTier()
        {
            Random numberGenerator = new Random();
            double probability = numberGenerator.NextDouble();
            int tier = 1;
            double upperBound = mTierPriority[0];
            for (int i = 0; i < 4; i++)
            {
                if (probability <= upperBound) break;
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