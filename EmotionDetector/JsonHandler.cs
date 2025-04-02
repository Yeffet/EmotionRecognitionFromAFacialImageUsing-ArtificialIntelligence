using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace EmotionDetector
{
    internal class JsonHandler
    {
        public static List<Tuple<string, string, double[]>> LoadFeatures(string filePath)
        {
            try
            {
                string json = File.ReadAllText(filePath);

                // המרה למבנה הנתונים כך שהמחרוזת הראשונה זה השם של הרגש, הרשימה היא של תכונות לכל תמונה
                var data = JsonConvert.DeserializeObject<Dictionary<string, List<Dictionary<string, object>>>>(json);

                // יצירת רשימה לשמירת הנתונים כדאי שיהיה נוח לעבודה עם KNN
                List<Tuple<string, string, double[]>> dataset = new List<Tuple<string, string, double[]>>();

                foreach (var emotion in data)
                {
                    foreach (var entry in emotion.Value)
                    {
                        string imageName = entry["image"].ToString();
                        double[] features = new double[]
                        {
                        Convert.ToDouble(entry["mean_intensity"]),
                        Convert.ToDouble(entry["stddev_intensity"]),
                        Convert.ToDouble(entry["min_pixel"]),
                        Convert.ToDouble(entry["max_pixel"]),
                        Convert.ToDouble(entry["dark_ratio"]),
                        Convert.ToDouble(entry["light_ratio"]),
                        Convert.ToDouble(entry["mean_edge_contrast"])
                        };

                        dataset.Add(new Tuple<string, string, double[]>(imageName, emotion.Key, features));
                    }
                }
                Console.WriteLine("Features successfully loaded from JSON.");
                return dataset;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading JSON file: " + ex.Message);
                return new List<Tuple<string, string, double[]>>();
            }

        }
    }
}
