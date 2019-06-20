using System;
using System.IO;
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
    public class DataStorage
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

    class StorageManager
    {
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
        
        //private DataContractSerializer mSerializer;
        private static readonly string sFolder = "SaveFiles" + Path.DirectorySeparatorChar;
        /*private static int mDirLength = 0;
        internal int DirLength
        {
            get
            {
                return mDirLength;
            }
        }*/
        internal static string Folder
        {
            get
            {
                Directory.CreateDirectory(sFolder);
                return sFolder;
            }
        }

        internal static string[] Files { get; } = new string[5];

        public void SaveGame(string fileName, InGameState gameState)
        {
            Directory.CreateDirectory(sFolder);

            using (var file = File.CreateText(sFolder + fileName + ".json"))
                CreateSerializer(gameState.GameStateManager).Serialize(file, gameState.toDataStorage());
        }

        public DataStorage LoadGame(string fileName, GameStateManager gameStateManager)
        {
            using (var file = File.OpenText(sFolder + fileName + ".json"))
                return (DataStorage) CreateSerializer(gameStateManager).Deserialize(file, typeof(DataStorage));
        }

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
    }
}
