using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using KernelPanic.Entities;
using KernelPanic.Entities.Buildings;
using KernelPanic.Entities.Units;
using KernelPanic.Events;
using KernelPanic.Players;

namespace KernelPanic.Tracking
{
    internal enum Achievement
    {
        Win1,
        Win10,
        Win100,

        Lose1,
        Lose10,
        Lose100,

        AptGetUpgrade,
        BitcoinAddict,
        IronFortress,
        HighInference,
        BugsFixed,

        EmptySlot,
        
        NumberOfAchievements
    }

    internal static class Achievements
    {
        #region Display Information

        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        internal static string Title(this Achievement achievement)
        {
            switch (achievement)
            {
                case Achievement.Win1:
                    return "First Victory!";
                case Achievement.Win10:
                    return "GG EASY";
                case Achievement.Win100:
                    return "Is Dis Tetris?";

                case Achievement.Lose1:
                    return "Unlucky Loss";
                case Achievement.Lose10:
                    return "Rekt";
                case Achievement.Lose100:
                    return "Complete Humiliation";

                case Achievement.AptGetUpgrade:
                    return "while true; DO sudo apt-get upgrade; DONE";
                case Achievement.BitcoinAddict:
                    return "Bitcoin Addiction";
                case Achievement.IronFortress:
                    return "Iron Fortress";
                case Achievement.HighInference:
                    return "High Inference";
                case Achievement.BugsFixed:
                    return "Fix your Code!";

                case Achievement.EmptySlot:
                    return "Fool!";

                case Achievement.NumberOfAchievements:
                    goto default;
                default:
                    throw new InvalidEnumArgumentException(nameof(achievement), (int) achievement, typeof(Achievement));
            }
        }

        internal static string Description(this Achievement achievement)
        {
            // TODO: Return correct descriptions.
            return achievement.ToString();
        }

        #endregion

        #region Connecting

        internal enum Status
        {
            Locked,
            Unlocked,
            Failed
        }

        internal static void ConnectComponents(this Achievement achievement, ProgressConnector progressConnector)
        {
            switch (achievement)
            {
                case Achievement.Win1:
                    progressConnector.ConnectCounter(Event.Id.GameWon);
                    break;
                case Achievement.Win10:
                    progressConnector.ConnectCounter(Event.Id.GameWon, target: 10);
                    break;
                case Achievement.Win100:
                    progressConnector.ConnectCounter(Event.Id.GameWon, target: 100);
                    break;

                case Achievement.Lose1:
                    progressConnector.ConnectCounter(Event.Id.GameLost);
                    break;
                case Achievement.Lose10:
                    progressConnector.ConnectCounter(Event.Id.GameLost, target: 10);
                    break;
                case Achievement.Lose100:
                    progressConnector.ConnectCounter(Event.Id.GameLost, target: 100);
                    break;

                case Achievement.AptGetUpgrade:
                    progressConnector.ConnectCounter(Event.Id.UpgradeBought, Event.Key.Price, 10, @event => @event.IsActivePlayer(Event.Key.Buyer));
                    break;

                case Achievement.BitcoinAddict:
                    progressConnector.ConnectComparison(Event.Id.BitcoinChanged, Event.Key.Price, 1100, @event => @event.IsActivePlayer(Event.Key.Buyer));
                    break;

                case Achievement.IronFortress:
                    progressConnector.ConnectCounter(Event.Id.GameWon, condition: Is100Percent);
                    break;

                case Achievement.HighInference:
                    progressConnector.ConnectCounter(Event.Id.GameWon);
                    progressConnector.ConnectCounter(Event.Id.BuildingPlaced,
                        condition: PlacedNonWifi,
                        resultingStatus: Status.Failed);
                    break;

                case Achievement.BugsFixed:
                    progressConnector.ConnectCounter(Event.Id.KilledUnit, target: 3, condition: IsEnemyBug);
                    break;

                case Achievement.EmptySlot:
                    progressConnector.ConnectCounter(Event.Id.LoadEmptySlot);
                    break;

                case Achievement.NumberOfAchievements:
                    goto default;
                default:
                    throw new InvalidEnumArgumentException(nameof(achievement), (int) achievement, typeof(Achievement));
            }
        }

        private static bool Is100Percent(Event @event)
        {
            return @event.Get<Player>(Event.Key.Winner).Base.Power == 100;
        }

        private static bool PlacedNonWifi(Event @event)
        {
            var building = @event.Get<Building>(Event.Key.Building);
            return @event.IsActivePlayer(Event.Key.Buyer) && !(building is WifiRouter);
        }

        private static bool IsEnemyBug(Event @event)
        {
            return @event.IsActivePlayer(Event.Key.Defender) && @event.Get<Unit>(Event.Key.Unit) is Bug;
        }

        #endregion

        #region Achievement Sets

        internal const int Count = (int) Achievement.NumberOfAchievements;

        internal static Achievement[] GloballyTracked =>
            new[]
            {
                // TODO: Complete this list.
                Achievement.BitcoinAddict,
                Achievement.Lose1,
                Achievement.Lose10,
                Achievement.Lose100,
                Achievement.Win1,
                Achievement.Win10,
                Achievement.Win100
            };

        internal static Achievement[] PerGame =>
            new[]
            {
                // TODO: Complete this list.
                Achievement.BugsFixed,
                Achievement.HighInference
            };

        #endregion
    }
}