using System;
// using System.Security.Cryptography;
using KernelPanic.Players;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using KernelPanic.Events;

namespace KernelPanic
{
    internal sealed class BitcoinManager : IDisposable
    {
        [JsonProperty("players")]
        private readonly PlayerIndexed<Player> mPlayers;

        [JsonProperty("cooldown")]
        private readonly PlayerIndexed<CooldownComponent> mCooldown;

        private readonly IDisposable mSubscription;

        internal BitcoinManager(PlayerIndexed<Player> players) : this(players, InitialCooldown())
        {
        }

        private static PlayerIndexed<CooldownComponent> InitialCooldown()
        {
            var oneSecond = new TimeSpan(0, 0, 1);
            return new PlayerIndexed<CooldownComponent>(
                new CooldownComponent(oneSecond, false),
                new CooldownComponent(oneSecond, false)
            );
        }

        public void Dispose()
        {
            mSubscription.Dispose();
        }

        [JsonConstructor]
        private BitcoinManager(PlayerIndexed<Player> players, PlayerIndexed<CooldownComponent> cooldown)
        {
            mPlayers = players;
            mCooldown = cooldown;

            Subscribe(players.A);
            Subscribe(players.B);

            mSubscription = EventCenter.Default.Subscribe(Event.Id.SetupEnded, @event => Reset(), null);
        }

        private void Reset()
        {
            mCooldown.A.Reset();
            mCooldown.B.Reset();
            mSubscription.Dispose();
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