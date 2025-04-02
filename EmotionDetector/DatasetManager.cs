using System;
using System.IO;

class DatasetManager
{
    public static void VerifyDataset(string datasetPath)
    {
        if (!Directory.Exists(datasetPath))
        {
            Console.WriteLine("Error: Dataset directory not found!");
            return;
        }

        string[] emotionFolders = Directory.GetDirectories(datasetPath);

        if (emotionFolders.Length == 0)
        {
            Console.WriteLine("Error: No emotion folders found in dataset!");
            return;
        }

        foreach (string folder in emotionFolders)
        {
            string emotion = Path.GetFileName(folder);
            string[] images = Directory.GetFiles(folder, "*.jpg");

            Console.WriteLine($"Emotion: {emotion} | Found {images.Length} images.");
        }
    }
}
