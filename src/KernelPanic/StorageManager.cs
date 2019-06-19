using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;
using Microsoft.Xna.Framework;

namespace KernelPanic
{
    [DataContract]
    public class DataStorage
    {
        [DataMember]
        internal Table.Board Board { get; set; }
        [DataMember]
        internal Player PlayerA { get; set; }
        [DataMember]
        internal Player PlayerB { get; set; }
        [DataMember]
        internal TimeSpan GameTime { get; set; }


    }
    class StorageManager
    {
        
        //private DataContractSerializer mSerializer;
        private const string mFolder = "SaveFiles\\";
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
                Directory.CreateDirectory(mFolder);
                return mFolder;
            }
        }

        internal static string[] Files { get; } = new string[5];

        public void SaveGame(String fileName, InGameState gameState)
        {
            Directory.CreateDirectory(mFolder);
            
            using (StreamWriter file = File.CreateText(mFolder + fileName + ".json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.TypeNameHandling = TypeNameHandling.Auto;
                serializer.Formatting = Newtonsoft.Json.Formatting.Indented;
                serializer.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                serializer.Serialize(file, gameState.toDataStorage());
            }

                //mDirLength += 1;
                Directory.CreateDirectory(mFolder);
            /*fileName = mFolder + fileName;
            mSerializer = new DataContractSerializer(typeof(InGameState));
            var settings = new XmlWriterSettings { Indent = true };
            var writer = XmlWriter.Create(fileName, settings);
            mSerializer.WriteObject(writer, gameState);
            writer.Close();*/
        }
        
       
        public DataStorage LoadGame(String fileName, GameStateManager gameStateManager)
        {
            var builder = new ContainerBuilder();
            builder.Register(c => new Table.Lane(gameStateManager.Sprite, gameStateManager.Sound)).As<Table.Lane>();
            builder.Register(c => new EntityGraph(Rectangle.Empty, gameStateManager.Sprite));
            builder.Register(c => new Entities.Firefox(gameStateManager.Sprite)).As<Entities.Firefox>();
            builder.Register(c => new Entities.Tower(gameStateManager.Sprite, gameStateManager.Sound)).As<Entities.Tower>();
            fileName = mFolder + fileName;
            using (StreamReader file = File.OpenText(fileName + ".json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.TypeNameHandling = TypeNameHandling.Auto;
                serializer.ContractResolver = new AutofacContractResolver(builder.Build());
                serializer.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                
                return (DataStorage)serializer.Deserialize(file, typeof(DataStorage));
            }
        }

        public class AutofacContractResolver : DefaultContractResolver
        {
            private readonly IContainer _container;

            public AutofacContractResolver(IContainer container)
            {
                _container = container;
            }

            protected override JsonObjectContract CreateObjectContract(Type objectType)
            {
                // use Autofac to create types that have been registered with it
                if (_container.IsRegistered(objectType))
                {
                    JsonObjectContract contract = ResolveContact(objectType);
                    contract.DefaultCreator = () => _container.Resolve(objectType);

                    return contract;
                }

                return base.CreateObjectContract(objectType);
            }

            private JsonObjectContract ResolveContact(Type objectType)
            {
                // attempt to create the contact from the resolved type
                IComponentRegistration registration;
                if (_container.ComponentRegistry.TryGetRegistration(new TypedService(objectType), out registration))
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
