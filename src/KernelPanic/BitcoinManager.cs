using System;
// using System.Security.Cryptography;
using KernelPanic.Players;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace KernelPanic
{
    internal sealed class BitcoinManager
    {
        [JsonProperty("players")]
        private readonly PlayerIndexed<Player> mPlayers;

        [JsonProperty("cooldown")]
        private readonly PlayerIndexed<CooldownComponent> mCooldown;

        internal BitcoinManager(PlayerIndexed<Player> players) : this(players, InitialCooldown())
        {
        }

        private static PlayerIndexed<CooldownComponent> InitialCooldown()
        {
            var oneSecond = new TimeSpan(0, 0, 1);
            return new PlayerIndexed<CooldownComponent>(
                new CooldownComponent(oneSecond), 
                new CooldownComponent(oneSecond)
            );
        }

        [JsonConstructor]
        private BitcoinManager(PlayerIndexed<Player> players, PlayerIndexed<CooldownComponent> cooldown)
        {
            mPlayers = players;
            mCooldown = cooldown;

            Subscribe(players.A);
            Subscribe(players.B);
        }

        private void Subscribe(Player player)
        {
            mCooldown.Select(player).CooledDown += component =>
            {
                player.Bitcoins++;
                component.Reset(TimerInterval(player));
            };
        }

        private static TimeSpan TimerInterval(Player player) =>
            player.IncreasedBitcoins
                ? TimeSpan.FromMilliseconds(909)
                : TimeSpan.FromSeconds(1);

        public void Update(GameTime gameTime)
        {
            mCooldown.A.Update(gameTime);
            mCooldown.B.Update(gameTime);
        }
    }
}