using System;
using KernelPanic.Players;
using Microsoft.Xna.Framework;

namespace KernelPanic
{

    internal sealed class BitcoinManager
    {
        private readonly Player mPlayerA;
        private readonly Player mPlayerB;

        private readonly CooldownComponent mCooldownA;
        private readonly CooldownComponent mCooldownB;

        public BitcoinManager(Player playerA, Player playerB)
        {
            mPlayerA = playerA;
            mPlayerB = playerB;
            var oneSecond = new TimeSpan(0, 0, 1);
            mCooldownA = new CooldownComponent(oneSecond);
            mCooldownB = new CooldownComponent(oneSecond);
            mCooldownA.CooledDown += component => mPlayerA.Bitcoins++;
            mCooldownB.CooledDown += component => mPlayerB.Bitcoins++;
            mCooldownA.CooledDown += component => mCooldownA.Reset();
            mCooldownB.CooledDown += component => mCooldownB.Reset();
        }

        public void Upgrade(Player player)
        {
            var upgradeInterval = new TimeSpan(0, 0, 0, 0, 909);
            if (player.Equals(mPlayerA))
            {
                mCooldownA.Reset(upgradeInterval);
            }
            else if (player.Equals(mPlayerB))
            {
                mCooldownB.Reset(upgradeInterval);
            }
        }

        public void Update(GameTime gameTime)
        {
            mCooldownA.Update(gameTime);
            mCooldownB.Update(gameTime);
        }
    }
}