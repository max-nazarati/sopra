using System.Collections.Generic;
using Accord.IO;
using KernelPanic.Entities;
using KernelPanic.Entities.Units;
using KernelPanic.Players;
using Newtonsoft.Json;

namespace KernelPanic.Waves
{
    internal sealed class Wave
    {
        [JsonProperty]
        internal int Index { get; }

        [JsonProperty]
        internal PlayerIndexed<List<Troupe>> Troupes { get; }

        internal int OriginalTroupeCountA { get; }
        internal int OriginalTroupeCountB { get; }

        /// <summary>
        /// A <see cref="Wave"/> is unbalanced, if one player doesn't have any troupes.
        /// </summary>
        [JsonProperty]
        internal bool Unbalanced { get; }

        /// <summary>
        /// Creates a new <see cref="Wave"/> with the given index and troupes.
        /// </summary>
        /// <param name="index">The <see cref="Wave"/>'s index.</param>
        /// <param name="troupes">The <see cref="Unit"/>'s initially in this wave.</param>
        [JsonConstructor]
        internal Wave(int index, PlayerIndexed<List<Troupe>> troupes)
        {
            Index = index;
            Troupes = troupes;
            OriginalTroupeCountA = Troupes.A.Count;
            OriginalTroupeCountB = Troupes.B.Count;
            Unbalanced = troupes.A.Count == 0 || troupes.B.Count == 0;
        }

        /// <summary>
        /// Removes all <see cref="Unit"/>s from this wave that have the <see cref="Unit.WantsRemoval"/> flag set. If
        /// one part is gets completely defeated, the corresponding players is awarded a experience point.
        /// </summary>
        /// <param name="players">The players.</param>
        internal void RemoveDead(PlayerIndexed<Player> players)
        {
            void Remove(IPlayerDistinction attacker, Player defender)
            {
                var units = Troupes.Select(attacker);
                var count = units.Count;
                if (count > 0 && units.RemoveAll(unit => unit.WantsRemoval) == count)
                    defender.ExperiencePoints++;
            }

            Remove(players.A, players.B);
            Remove(players.B, players.A);
        }

        /// <summary>
        /// Returns <c>true</c> if at least one player has no more <see cref="Unit"/>s in this <see cref="Wave"/>.
        /// </summary>
        internal bool AtLeastPartiallyDefeated => Troupes.A.Count == 0 || Troupes.B.Count == 0;

        /// <summary>
        /// Returns <c>true</c> if neither player has <see cref="Unit"/>s remaining in this <see cref="Wave"/>.
        /// </summary>
        internal bool FullyDefeated => Troupes.A.Count == 0 && Troupes.B.Count == 0;

        // True, if human player fully defeated the hostile wave
        internal bool FullyDefeatedByHuman => Troupes.B.Count == 0;
        internal bool FullyDefeatedByComputer => Troupes.A.Count == 0;
    }
}
