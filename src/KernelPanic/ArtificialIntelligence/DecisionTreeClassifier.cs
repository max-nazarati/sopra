﻿using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.Math;
using Accord.Statistics.Filters;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace KernelPanic.ArtificialIntelligence
{
    class DecisionTreeClassifier
    {
        private DataTable mDataSet;
        private Codification mCodebook;

        /// <summary>
        /// Train a decision tree model based on its training set (mDataset)
        /// </summary>
        public void TrainModel()
        {
        Console.WriteLine("starting to train model");
        // split dataset into features (xTrain) and labels (yTrain)  
        var xTrain = mDataSet.ToJagged<double>("Bitcoins",
            "Bug", "Virus", "Trojaner", "Nokia", "Thunderbird", "Settings",
        "Firefox", "Bluescreen", "Kabel", "Mauszeigerschütze", "CD-Werfer",
        "Antivirusprogramm", "Lüftung", "Wifi-Router", "Schockfeld");
        var labels = mDataSet.ToArray<String>("Aktion");
        mCodebook = new Codification("Aktion", labels);
        var yTrain = mCodebook.Transform("Aktion", labels);
        
        // train decision tree model
        var teacher = new C45Learning();
        Model = teacher.Learn(xTrain, yTrain);

        // training statistics
        // var predicted = Model.Decide(xTrain);
        // var error = new ZeroOneLoss(yTrain).Loss(predicted);
        // Console.Write("finished training - ");
        // Console.WriteLine("training error: " + error);
        }

        /// <summary>
        /// Read csv file and store content as DataTable, e.g.:
        /// ==============================
        /// Bitcoins,Trojaner,Aktion
        /// 3,5,CD-Werfer
        /// ==============================
        /// to
        /// ==============================
        /// Bitcoins | Trojaner | Aktion
        /// ------------------------------
        ///     3          5     CD-Werfer
        /// ==============================
        /// </summary>
        /// <param name="filename">path to file</param>
        public void ReaderCsv(string filename)
        {
            DataTable resulTable = new DataTable();
            StreamReader reader = new StreamReader(File.OpenRead(filename));
            List<string> fileContent = new List<string>();

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (!String.IsNullOrWhiteSpace(line))
                {
                    fileContent.Add(line);
                }
            }

            String[] features = fileContent[0].Split(',');
            foreach (var feature in features)
            {
                resulTable.Columns.Add(feature);
            }

            for (int i = 1; i < fileContent.Count; i++)
            {
                String[] featuresContent = fileContent[i].Split(',');
                DataRow row = resulTable.NewRow();
                for (int j = 0; j < resulTable.Columns.Count; j++)
                {
                    row[resulTable.Columns[j]] = featuresContent[j];
                }
                resulTable.Rows.Add(row);
            }

            mDataSet = resulTable;
        }

        /*
        /// <summary>
        /// Translate tree into set of rules, e.g.:
        ///                     /   \
        /// Feature 1      <10 /     \ >= 10
        ///                   /       \
        ///                  no       yes
        /// Will be translated to:
        /// no : (1 <  10)
        /// yes: (1 >= 10)
        /// </summary>
        /// <returns>The set of rules</returns>
        */
        /*
        public override String ToString()
        {
            var model = Model;
            var rules = Model.ToRules();
            var result = rules.ToString(mCodebook, "Aktion",
                System.Globalization.CultureInfo.InvariantCulture);
            return result;
        } */

        /// <summary>
        /// Predict single label, e.g.:
        /// Bitcoins,Bug,Virus,Trojaner,Nokia,Thunderbird,Settings,Firefox,Bluescreen,Kabel,Mauszeigerschütze,CD-Werfer,Antivirusprogramm,Lüftung,Wifi-Router,Schockfeld
        /// 1,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0
        /// </summary>
        /// <param name="input">feature values stored in double[]</param>
        /// <returns>decoded prediction value</returns>
        public int Predict(double[] input)
        {
            double[][] formattedInput = {input};
            var predictions = Model.Decide(formattedInput);
            var prediction = predictions[0];
            return prediction;
        }
        /*
        public double Score(double[][] xTest, string[] yTest)
        {
            int[] decodedYTest = mCodebook.Transform("Aktion", yTest);
            int[] predictions = mModel.Decide(xTest);
            double error = new ZeroOneLoss(decodedYTest).Loss(predictions);

            return error;
        } */

        /*
        public int[] PredictMulti(double[][] input)
        {
            int[] predictions = mModel.Decide(input);
            return predictions;
        } */

        public string Revert(int prediction) => mCodebook.Revert("Aktion", prediction);
        // public Codification Codebook { get => mCodebook; set => mCodebook = value; }
        private DecisionTree Model { get; set; } //set => mModel = value; }
        // public DecisionSet Rules { get => mModel.ToRules();}
    }
}
