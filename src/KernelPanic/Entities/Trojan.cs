using Newtonsoft.Json;

namespace KernelPanic.Entities
{
    internal sealed class Trojan : Troupe
    {
        [JsonProperty]
        internal bool SpawnsDouble { get; set; }

        internal Trojan(SpriteManager spriteManager)
            : base(20, 3, 30, 6, spriteManager.CreateTrojan(), spriteManager)
        {
        }

        protected override void DidDie()
        {
            for (var i = 0; i < (SpawnsDouble ? 10 : 5); ++i)
                Wave.SpawnChild(new Bug(SpriteManager) {Sprite = {Position = Sprite.Position}});

            base.DidDie();
        }
    }
}
