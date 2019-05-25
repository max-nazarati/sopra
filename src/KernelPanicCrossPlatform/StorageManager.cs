﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml;

namespace KernelPanic
{
    class StorageManager
    {
        private DataContractSerializer mSerializer;

        public void SaveGame(String fileName, Grid gameState)
        {
            mSerializer = new DataContractSerializer(typeof(Grid));
            FileStream writer = new FileStream(fileName, FileMode.Create);
            mSerializer.WriteObject(writer, gameState);
            writer.Close();
        }

        public Grid LoadGame(String fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open);
            XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
            mSerializer = new DataContractSerializer(typeof(Grid));
            Grid deserializedGameState = (Grid)mSerializer.ReadObject(reader, true);
            fs.Close();

            return deserializedGameState;
        }
    }
}
