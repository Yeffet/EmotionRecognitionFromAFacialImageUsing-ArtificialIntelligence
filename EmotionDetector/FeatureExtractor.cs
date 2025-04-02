using System;
using System.IO;
using System.Collections.Generic;
using OpenCvSharp;
using Newtonsoft.Json;

class FeatureExtractor
{
    public static void ExtractFeatures(string datasetPath, string outputJsonFile)
    {
        Console.WriteLine($"Extracting features from: {datasetPath}");

        // יצירת מבנה נתונים לשמירת כל התכונות
        Dictionary<string, List<Dictionary<string, object>>> featureData = new Dictionary<string, List<Dictionary<string, object>>>();

        foreach (string folder in Directory.GetDirectories(datasetPath))
        {
            string emotion = Path.GetFileName(folder); // קבלת שם הרגש (angry, happy, etc.)
            string[] images = Directory.GetFiles(folder, "*.jpg");

            if (!featureData.ContainsKey(emotion))
                featureData[emotion] = new List<Dictionary<string, object>>();

            foreach (string imagePath in images)
            {
                Mat img = Cv2.ImRead(imagePath, ImreadModes.Grayscale);

                // **1. חישוב ממוצע בהירות**
                Scalar meanIntensity = Cv2.Mean(img);

                // **2. חישוב סטיית תקן**
                Mat mean = new Mat();
                Mat stddev = new Mat();
                Cv2.MeanStdDev(img, mean, stddev);
                double meanValue = mean.At<double>(0, 0);
                double stddevValue = stddev.At<double>(0, 0);

                // **3. מציאת ערכי מינימום ומקסימום**
                double minVal, maxVal;
                Cv2.MinMaxLoc(img, out minVal, out maxVal);

                // **4. חישוב יחס כהים/בהירים**
                int totalPixels = img.Rows * img.Cols;
                int darkPixels = 0, lightPixels = 0;

                for (int y = 0; y < img.Rows; y++)
                {
                    for (int x = 0; x < img.Cols; x++)
                    {
                        byte pixelValue = img.At<byte>(y, x);
                        if (pixelValue < 50) darkPixels++;
                        if (pixelValue > 200) lightPixels++;
                    }
                }

                double darkRatio = (double)darkPixels / totalPixels;
                double lightRatio = (double)lightPixels / totalPixels;

                // **5. חישוב ניגודיות עם Sobel**
                Mat sobelX = new Mat(), sobelY = new Mat();
                Cv2.Sobel(img, sobelX, MatType.CV_64F, 1, 0);
                Cv2.Sobel(img, sobelY, MatType.CV_64F, 0, 1);

                Mat sobelMagnitude = new Mat();
                Cv2.Magnitude(sobelX, sobelY, sobelMagnitude);

                Scalar meanEdgeContrast = Cv2.Mean(sobelMagnitude);

                // **6. שמירת התכונות של התמונה הנוכחית**
                var features = new Dictionary<string, object>
                {
                    { "image", Path.GetFileName(imagePath) },
                    { "mean_intensity", meanIntensity.Val0 },
                    { "stddev_intensity", stddevValue },
                    { "min_pixel", minVal },
                    { "max_pixel", maxVal },
                    { "dark_ratio", darkRatio },
                    { "light_ratio", lightRatio },
                    { "mean_edge_contrast", meanEdgeContrast.Val0 }
                };

                featureData[emotion].Add(features);
            }

            Console.WriteLine($"Extracted features for {emotion} images.");
        }

        // **7. שמירת נתונים ל-JSON בהתאם לסוג הסט**
        string json = JsonConvert.SerializeObject(featureData, Formatting.Indented);
        File.WriteAllText(outputJsonFile, json);

        Console.WriteLine($"Feature extraction completed. Features saved to '{outputJsonFile}'.");
    }
}
