using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autofac;
using KernelPanic.Entities;
using KernelPanic.Table;
using Newtonsoft.Json;

namespace KernelPanic.Serialization
{
    internal static class StorageManager
    {
        #region Constructor

        static StorageManager()
        {
            Directory.CreateDirectory(sFolder);
        }

        #endregion

        #region Save & Load

        internal static IEnumerable<int> Slots => Enumerable.Range(0, 5);

        internal static void SaveGame(InGameState gameState)
        {
            if (!Slots.Contains(gameState.SaveSlot))
                throw new InvalidOperationException("InGameState has invalid save-slot " + gameState.SaveSlot);

            var serializer = CreateSerializer(gameState.GameStateManager);

            using (var file = File.CreateText(DataPath(gameState.SaveSlot)))
                serializer.Serialize(file, gameState.Data);
            using (var file = File.CreateText(InfoPath(gameState.SaveSlot)))
                serializer.Serialize(file, new Storage.Info {Timestamp = DateTime.Now});
        }

        internal static Storage LoadGame(int slot, GameStateManager gameStateManager)
        {
            if (!Slots.Contains(slot))
                throw new ArgumentOutOfRangeException(nameof(slot), slot, "invalid slot value");

            using (var file = File.OpenText(DataPath(slot)))
                return (Storage) CreateSerializer(gameStateManager).Deserialize(file, typeof(Storage));
        }

        internal static Storage.Info? LoadInfo(int slot, GameStateManager gameStateManager)
        {
            if (!Slots.Contains(slot))
                throw new ArgumentOutOfRangeException(nameof(slot), slot, "invalid slot value");

            if (!File.Exists(DataPath(slot)) || !File.Exists(InfoPath(slot)))
                return null;

            using (var file = File.OpenText(InfoPath(slot)))
                return (Storage.Info) CreateSerializer(gameStateManager).Deserialize(file, typeof(Storage.Info));
        }

        #endregion

        #region Private Helpers

        private static AutofacContractResolver CreateContractResolver(GameStateManager manager)
        {
            var builder = new ContainerBuilder();
            builder.Register(c => new Lane(manager.Sprite, manager.Sound));
            builder.Register(c => new Firefox(manager.Sprite));
            builder.Register(c => new Tower(manager.Sprite, manager.Sound));
            builder.Register(c => new StrategicTower(manager.Sprite, manager.Sound));
            builder.Register(c => new Trojan(manager.Sprite));

            return new AutofacContractResolver(builder.Build());
        }

        private static JsonSerializer CreateSerializer(GameStateManager gameStateManager)
        {
            return new JsonSerializer
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ContractResolver = CreateContractResolver(gameStateManager)
            };
        }

        private static readonly string sFolder = "SaveFiles" + Path.DirectorySeparatorChar;
        private static string DataPath(int slot) => Path.Combine(sFolder, "data" + slot + ".json");
        private static string InfoPath(int slot) => Path.Combine(sFolder, "info" + slot + ".json");

        #endregion

    }
}
