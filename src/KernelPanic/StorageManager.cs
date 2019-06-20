using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;
using KernelPanic.Entities;
using KernelPanic.Table;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace KernelPanic
{
    [DataContract]
    internal sealed class DataStorage
    {
        [DataMember]
        internal Board Board { get; set; }
        [DataMember]
        internal Player PlayerA { get; set; }
        [DataMember]
        internal Player PlayerB { get; set; }
        [DataMember]
        internal TimeSpan GameTime { get; set; }
    }

    internal struct DataInfo
    {
        internal DateTime Timestamp { get; set; }
    }

    internal static class StorageManager
    {
        #region Constructor

        static StorageManager()
        {
            Directory.CreateDirectory(sFolder);
        }

        #endregion

        #region Save & Load

        // TODO: Remove when not used any more.
        internal static class Debug
        {
            internal static int NextSaveSlot =>
                Directory.GetFiles(sFolder).Length / 2 % Slots.Count();
        }

        internal static IEnumerable<int> Slots => Enumerable.Range(0, 5);

        internal static void SaveGame(int slot, InGameState gameState)
        {
            if (!Slots.Contains(slot))
                throw new ArgumentOutOfRangeException(nameof(slot), slot, "invalid slot value");

            var serializer = CreateSerializer(gameState.GameStateManager);

            using (var file = File.CreateText(DataPath(slot)))
                serializer.Serialize(file, gameState.ToDataStorage());
            using (var file = File.CreateText(InfoPath(slot)))
                serializer.Serialize(file, new DataInfo {Timestamp = DateTime.Now});
        }

        internal static DataStorage LoadGame(int slot, GameStateManager gameStateManager)
        {
            if (!Slots.Contains(slot))
                throw new ArgumentOutOfRangeException(nameof(slot), slot, "invalid slot value");

            using (var file = File.OpenText(DataPath(slot)))
                return (DataStorage) CreateSerializer(gameStateManager).Deserialize(file, typeof(DataStorage));
        }

        internal static DataInfo? LoadStorageInfo(int slot, GameStateManager gameStateManager)
        {
            if (!Slots.Contains(slot))
                throw new ArgumentOutOfRangeException(nameof(slot), slot, "invalid slot value");

            if (!File.Exists(DataPath(slot)) || !File.Exists(InfoPath(slot)))
                return null;

            using (var file = File.OpenText(InfoPath(slot)))
                return (DataInfo) CreateSerializer(gameStateManager).Deserialize(file, typeof(DataInfo));
        }

        #endregion

        #region Private Helpers

        private static AutofacContractResolver CreateContractResolver(GameStateManager manager)
        {
            var builder = new ContainerBuilder();
            builder.Register(c => new Lane(manager.Sprite, manager.Sound));
            builder.Register(c => new EntityGraph(Rectangle.Empty, manager.Sprite));
            builder.Register(c => new Firefox(manager.Sprite));
            builder.Register(c => new Tower(manager.Sprite, manager.Sound));
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

        // Taken from https://www.newtonsoft.com/json/help/html/DeserializeWithDependencyInjection.htm.
        private sealed class AutofacContractResolver : DefaultContractResolver
        {
            private readonly IContainer mContainer;

            public AutofacContractResolver(IContainer container)
            {
                mContainer = container;
            }

            protected override JsonObjectContract CreateObjectContract(Type objectType)
            {
                // use Autofac to create types that have been registered with it
                if (mContainer.IsRegistered(objectType))
                {
                    JsonObjectContract contract = ResolveContact(objectType);
                    contract.DefaultCreator = () => mContainer.Resolve(objectType);

                    return contract;
                }

                return base.CreateObjectContract(objectType);
            }

            private JsonObjectContract ResolveContact(Type objectType)
            {
                // attempt to create the contact from the resolved type
                if (mContainer.ComponentRegistry.TryGetRegistration(new TypedService(objectType), out var registration))
                {
                    Type viewType = (registration.Activator as ReflectionActivator)?.LimitType;
                    if (viewType != null)
                    {
                        return base.CreateObjectContract(viewType);
                    }
                }

                // fall back to using the registered type
                return base.CreateObjectContract(objectType);
            }
        }

        #endregion

    }
}
