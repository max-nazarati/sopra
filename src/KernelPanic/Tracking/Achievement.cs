using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using KernelPanic.Events;

namespace KernelPanic.Tracking
{
    internal enum Achievement
    {
        Win1,
        Win10,
        Win100,

        Lose1,
        Lose10,
        Lose100
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

                default:
                    throw new InvalidEnumArgumentException(nameof(achievement), (int) achievement, typeof(Achievement));
            }
        }

        internal static string Description(this  Achievement achievement)
        {
            throw new NotImplementedException();
        }
    }

    internal abstract class AchievementProgress
    {
        internal static AchievementProgress Track(Achievement achievement)
        {
            switch (achievement)
            {
                case Achievement.Win1:
                    return new SingleEventCountProgress(Event.Id.GameWon, 1);
                case Achievement.Win10:
                    return new SingleEventCountProgress(Event.Id.GameWon, 10);
                case Achievement.Win100:
                    return new SingleEventCountProgress(Event.Id.GameWon, 100);
                case Achievement.Lose1:
                    return new SingleEventCountProgress(Event.Id.GameLost, 1);
                case Achievement.Lose10:
                    return new SingleEventCountProgress(Event.Id.GameLost, 10);
                case Achievement.Lose100:
                    return new SingleEventCountProgress(Event.Id.GameLost, 100);
                default:
                    throw new ArgumentOutOfRangeException(nameof(achievement), achievement, null);
            }
        }
    }

    internal sealed class SingleEventCountProgress : AchievementProgress
    {
        private int mRemaining;

        internal SingleEventCountProgress(Event.Id eventId, int count)
        {
            mRemaining = count;
        }
    }
}