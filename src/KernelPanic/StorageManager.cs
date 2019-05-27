using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace KernelPanic
{
    class StorageManager
    {
        private DataContractSerializer mSerializer;

        public void SaveGame(String fileName, AGameState gameState)
        {
            mSerializer = new DataContractSerializer(typeof(AGameState));
            FileStream writer = new FileStream(fileName, FileMode.Create);
            mSerializer.WriteObject(writer, gameState);
            writer.Close();
        }

        public AGameState LoadGame(String fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open);
            XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
            mSerializer = new DataContractSerializer(typeof(Grid));
            AGameState deserializedGameState = (AGameState)mSerializer.ReadObject(reader, true);
            fs.Close();

            return deserializedGameState;
        }
    }
}
