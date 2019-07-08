using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using KernelPanic.Events;

namespace KernelPanic.Tracking
{
    internal enum Achievement
    {
    }

    internal static class Achievements
    {
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        internal static string Title(this Achievement achievement)
        {
            switch (achievement)
            {
                default:
                    throw new InvalidEnumArgumentException(nameof(achievement), (int) achievement, typeof(Achievement));
            }
        }

        internal static string Description(this  Achievement achievement)
        {
            throw new NotImplementedException();
        }
    }
}