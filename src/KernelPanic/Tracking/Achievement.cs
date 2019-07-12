using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

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

        internal enum Status
        {
            Locked,
            Unlocked,
            Failed
        }
    }
}