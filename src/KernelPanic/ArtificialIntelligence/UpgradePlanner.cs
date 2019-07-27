using System;
using System.Collections.Generic;
using KernelPanic.Data;
using KernelPanic.Events;
using KernelPanic.Players;
using KernelPanic.Purchasing;
using KernelPanic.Upgrades;

namespace KernelPanic.ArtificialIntelligence
{
    internal sealed class UpgradePlanner : Planner, IDisposable
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
        private readonly List<double[]> mTierDistribution;

        /// <summary>
        /// mTierDictionnary identifies the upgrade id of upgrade j in tier i as (i, j), i.e.
        /// this Upgrade.Id = mTierDictionnary[i][j]
        /// </summary>
        private List<Upgrade.Id[]> mTierDictionnary;

        private CompositeDisposable mSubscriptions;

        public UpgradePlanner(Player player, Func<Upgrade.Id, SinglePurchasableAction<Upgrade>> upgradeLookup) : base(player)
        {
            mUpgradeLookup = upgradeLookup;
            mTierPriority = new[] {0, 0.2, 0.2, 0.2, 0.2, 0.2};

            // Initialize distributions for each tier
            var tier1Distribution = new[] { 0.25, 0.25, 0.25, 0.25 };
            var tier2Distribution = new[] { 0.25, 0.25, 0.25, 0.25 };
            var tier3Distribution = new[] { 0.2, 0.2, 0.2, 0.2, 0.2 };
            var tier4Distribution = new[] { 0.2, 0.2, 0.2, 0.2, 0.2 };
            var tier5Distribution = new[] { 0.2, 0.2, 0.2, 0.2, 0.2 };
            mTierDistribution = new List<double[]>
            {
                new double[0],
                tier1Distribution,
                tier2Distribution,
                tier3Distribution,
                tier4Distribution,
                tier5Distribution
            };

            // initialize dictionary of upgrades corresponding to their tiers
            Upgrade.Id[] tier1Upgrades = new[]
                {Upgrade.Id.IncreaseLp1, Upgrade.Id.IncreaseGs1, Upgrade.Id.IncreaseVs1, Upgrade.Id.DecreaseAi1};
            Upgrade.Id[] tier2Upgrades = new[]
                {Upgrade.Id.IncreaseLp2, Upgrade.Id.IncreaseGs2, Upgrade.Id.IncreaseVs2, Upgrade.Id.IncreaseBitcoins};
            Upgrade.Id[] tier3Upgrades = new[]
            {
                Upgrade.Id.IncreaseSettingsHeal2, Upgrade.Id.IncreaseGsNokia, Upgrade.Id.IncreaseGsFirefox,
                Upgrade.Id.MoreTrojanChildren1, Upgrade.Id.IncreaseSettingsArea2
            };
            Upgrade.Id[] tier4Upgrades = new[]
            {
                Upgrade.Id.EmpDuration, Upgrade.Id.AdditionalFirefox1, Upgrade.Id.IncreaseSettingsArea1,
                Upgrade.Id.IncreaseSettingsHeal1, Upgrade.Id.MoreTrojanChildren2,
            };
            Upgrade.Id[] tier5Upgrades = new[]
            {
                Upgrade.Id.CdBoomerang, Upgrade.Id.EmpTwoTargets, Upgrade.Id.AdditionalFirefox2,
                Upgrade.Id.DoubleClick, Upgrade.Id.IncreaseWifi
            };
            mTierDictionnary = new List<Upgrade.Id[]>
            {
                new[] {Upgrade.Id.Invalid},
                tier1Upgrades,
                tier2Upgrades,
                tier3Upgrades,
                tier4Upgrades,
                tier5Upgrades
            };

            var eventCenter = EventCenter.Default;
            mSubscriptions += eventCenter.Subscribe(Event.Id.UpgradeBought,
                e =>
                {
                    Upgrade upgrade = e.Get<Upgrade>(Event.Key.Upgrade);
                    RemoveAvailableUpdate(upgrade.Kind);
                });
        }

        /// <summary>
        /// Given upgrade locate its position in mTierDistribution
        /// </summary>
        /// <param name="id"></param>
        /// <returns>(i, j) where i is the tier and j the corresponding upgrade index</returns>
        private (int, int) LocateUpgrade(Upgrade.Id id)
        {
            int tier;
            int tierIndex;

            switch (id)
            {
                // Tier 1 Upgrades
                case Upgrade.Id.IncreaseLp1:
                    tier = 1;
                    tierIndex = 0;
                    break;
                case Upgrade.Id.IncreaseGs1:
                    tier = 1;
                    tierIndex = 1;
                    break;
                case Upgrade.Id.IncreaseVs1:
                    tier = 1;
                    tierIndex = 2;
                    break;
                case Upgrade.Id.DecreaseAi1:
                    tier = 1;
                    tierIndex = 3;
                    break;

                // Tier 2 Upgrades
                case Upgrade.Id.IncreaseLp2:
                    tier = 2;
                    tierIndex = 0;
                    break;
                case Upgrade.Id.IncreaseGs2:
                    tier = 2;
                    tierIndex = 1;
                    break;
                case Upgrade.Id.IncreaseVs2:
                    tier = 2;
                    tierIndex = 2;
                    break;
                case Upgrade.Id.IncreaseBitcoins:
                    tier = 2;
                    tierIndex = 3;
                    break;

                // Tier 3 Upgrades
                case Upgrade.Id.IncreaseSettingsHeal2:
                    tier = 3;
                    tierIndex = 0;
                    break;
                case Upgrade.Id.IncreaseGsNokia:
                    tier = 3;
                    tierIndex = 1;
                    break;
                case Upgrade.Id.IncreaseGsFirefox:
                    tier = 3;
                    tierIndex = 2;
                    break;
                case Upgrade.Id.MoreTrojanChildren1:
                    tier = 3;
                    tierIndex = 3;
                    break;
                case Upgrade.Id.IncreaseSettingsArea2:
                    tier = 3;
                    tierIndex = 4;
                    break;

                // Tier 4 Upgrades
                case Upgrade.Id.EmpDuration:
                    tier = 4;
                    tierIndex = 0;
                    break;
                case Upgrade.Id.AdditionalFirefox1:
                    tier = 4;
                    tierIndex = 1;
                    break;
                case Upgrade.Id.IncreaseSettingsArea1:
                    tier = 4;
                    tierIndex = 2;
                    break;
                case Upgrade.Id.IncreaseSettingsHeal1:
                    tier = 4;
                    tierIndex = 3;
                    break;
                case Upgrade.Id.MoreTrojanChildren2:
                    tier = 4;
                    tierIndex = 4;
                    break;

                // Tier 5 Upgrades
                case Upgrade.Id.CdBoomerang:
                    tier = 5;
                    tierIndex = 0;
                    break;
                case Upgrade.Id.EmpTwoTargets:
                    tier = 5;
                    tierIndex = 1;
                    break;
                case Upgrade.Id.AdditionalFirefox2:
                    tier = 5;
                    tierIndex = 2;
                    break;
                case Upgrade.Id.DoubleClick:
                    tier = 5;
                    tierIndex = 3;
                    break;
                case Upgrade.Id.IncreaseWifi:
                    tier = 5;
                    tierIndex = 4;
                    break;
                default:
                    tier = 0;
                    tierIndex = 0;
                    break;
            }

            return (tier, tierIndex);
        }

        /// <summary>
        /// Update Distribution of upgrades after upgrade is bought, e.g.:
        /// tier[i] = [0.25, 0.25, 0.25, 0.25]
        /// Then buying upgrade 3 of tier i updates to
        /// tier[i] = [1/3, 1/3, 0, 1/3]
        /// </summary>
        /// <param name="id"></param>
        private void RemoveAvailableUpdate(Upgrade.Id id)
        {
            var tier = LocateUpgrade(id).Item1;
            var tierIndex = LocateUpgrade(id).Item2;
            var upgradeProbability = mTierDistribution[tier][tierIndex];
            mTierDistribution[tier][tierIndex] = 0;
            for (var i = 0; i < mTierDistribution[tier].Length; i++)
            {
                mTierDistribution[tier][i] /= 1 - upgradeProbability;
            }

            // update tier priority
            var numberTierUpgrades = mTierDistribution[tier].Length;
            mTierPriority[tier] -= 1 / (double)numberTierUpgrades;
            for (var i = 1; i <= 5; i++)
            {
                mTierPriority[i] /= 1 - (1 / (double) numberTierUpgrades);
            }
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
            double upperBound = mTierPriority[1];
            for (int i = 1; i < 5; i++)
            {
                if (probability <= upperBound) break;
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
            upgrade.TryPurchase(Player);
        }

        public override void Update()
        {
            base.Update();
            int tier = ChooseTier();
            Upgrade.Id upgrade = MakeChoice(tier);
            BuyUpgrade(upgrade);
        }

        protected override void Dispose(bool disposing)
        {
            mSubscriptions.Dispose();
            base.Dispose(disposing);
        }
    }
}