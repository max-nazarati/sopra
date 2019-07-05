using System.Collections.Generic;
using KernelPanic.Entities;
using KernelPanic.Entities.Units;
using KernelPanic.Players;

namespace KernelPanic.Waves
{
    internal sealed class Wave
    {
        internal int Index { get; }
        private readonly PlayerIndexed<List<Troupe>> mUnits;

        /// <summary>
        /// Creates a new <see cref="Wave"/> with the given index and units.
        /// </summary>
        /// <param name="index">The <see cref="Wave"/>'s index.</param>
        /// <param name="units">The <see cref="Unit"/>'s initially in this wave.</param>
        internal Wave(int index, PlayerIndexed<List<Troupe>> units)
        {
            Index = index;
            mUnits = units;
        }

        /// <summary>
        /// Removes all <see cref="Unit"/>s from this wave that have the <see cref="Unit.WantsRemoval"/> flag set. If
        /// one part is gets completely defeated, the corresponding players is awarded a experience point.
        /// </summary>
        /// <param name="players">The players.</param>
        internal void RemoveDead(PlayerIndexed<Player> players)
        {
            void Remove(Player player)
            {
                var units = mUnits.Select(player);
                var count = units.Count;
                if (count > 0 && units.RemoveAll(unit => unit.WantsRemoval) == count)
                    player.ExperiencePoints++;
            }

            Remove(players.A);
            Remove(players.B);
        }

        /// <summary>
        /// Returns <c>true</c> if at least one player has no more <see cref="Unit"/>s in this <see cref="Wave"/>.
        /// </summary>
        internal bool AtLeastPartiallyDefeated => mUnits.A.Count == 0 || mUnits.B.Count == 0;

        /// <summary>
        /// Returns <c>true</c> if neither player has <see cref="Unit"/>s remaining in this <see cref="Wave"/>.
        /// </summary>
        internal bool FullyDefeated => mUnits.A.Count == 0 && mUnits.B.Count == 0;
    }
}
