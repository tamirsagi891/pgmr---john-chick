using System.IO;
using Elad.Events;
using UnityEngine;
using UnityEngine.Events;
using Logger = Nemesh.Logger;

namespace Elad.Save_Load_System
{
    public static class SaveGameOnJson
    {
        
        public const string SaveDirectory = "/SaveData/";
        public const string FileName = "SaveGame.sav";

       

        public static SaveData CurrentSaveData = new SaveData();
        
        public static bool SaveGame()
        {
            var dir = Application.persistentDataPath + SaveDirectory;

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string json = JsonUtility.ToJson(CurrentSaveData, true);
            
            
            // File.WriteAllText(dir + wantedFileName, json);
            File.WriteAllText(dir + FileName, json);

            GUIUtility.systemCopyBuffer = dir;
            return true;
        }

        public static void LoadGame()
        {
            characterEvents.OnJsonLoadStart.Invoke();
            string fullPath = Application.persistentDataPath + SaveDirectory + FileName;

            SaveData tempSaveData = new SaveData();

            if (File.Exists(fullPath))
            {
                string json = File.ReadAllText(fullPath);
                tempSaveData = JsonUtility.FromJson<SaveData>(json);
            }
            else
            {
                Logger.Log("Save file is not exist");
            }

            CurrentSaveData = tempSaveData;
            
            characterEvents.OnJsonLoadFinish.Invoke();
        }
        
    }
}
