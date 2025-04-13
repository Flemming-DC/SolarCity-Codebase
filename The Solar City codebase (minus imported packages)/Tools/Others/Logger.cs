using System;
using System.IO;
using UnityEngine;

public static class Logger
{
    static string filePath = @"C:\Users\Flemming\Documents\Spil - hjemmelavede\Unity\RPG 2\Assets\Notes\Log File.txt";
    static bool hasLogged = false;

    public static void LogAndPrint(string message)
    {
        Log(message);
        Debug.Log(message);
    }

    public static void Log(string message)
    {
        if (!hasLogged)
        {
            hasLogged = true;
            string newGameMessage = "------------ New Game -------------";
            string currentTime = DateTime.Now.ToString("MM/dd/yyyy h:mm:ss tt");
            File.AppendAllText(filePath, $"\n\n{newGameMessage}\n");
            File.AppendAllText(filePath, $"Time: {currentTime}\n");
        }
        File.AppendAllText(filePath, message + "\n");
    }



}
