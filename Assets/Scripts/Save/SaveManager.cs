using System;
using System.IO;
using UnityEngine;

namespace LumenRush
{
    [Serializable]
    public sealed class SaveData
    {
        public int version = 1, coins, highScore, xp, character, upgrade, bestDistance;
        public int unlockedCharacters = 1, trail, board, totalCoins, totalJumps, totalPowers, runs;
        public int claimedAchievements, dailyCoins, weeklyMeters;
        public string dailyReward = "", dailyPeriod = "", weeklyPeriod = "";
        public bool dailyMissionClaimed, weeklyMissionClaimed, music = true, sound = true, haptics = true, lowGraphics;
    }

    public interface ISaveStore
    {
        SaveData Load();
        void Write(SaveData data);
    }

    public sealed class LocalSaveStore : ISaveStore
    {
        readonly string path = Path.Combine(Application.persistentDataPath, "lumen-save.json");
        public SaveData Load()
        {
            foreach (var file in new[]{path, path + ".bak"})
                try
                {
                    if (File.Exists(file))
                    {
                        var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(file));
                        if (data != null && data.version == 1)
                            return data;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning("Save recovery: " + e.Message);
                }

            return new SaveData();
        }

        public void Write(SaveData data)
        {
            try
            {
                File.WriteAllText(path + ".tmp", JsonUtility.ToJson(data, true));
                if (File.Exists(path))
                    File.Copy(path, path + ".bak", true);
                File.Copy(path + ".tmp", path, true);
                File.Delete(path + ".tmp");
            }
            catch (Exception e)
            {
                Debug.LogWarning("Unable to save progress: " + e.Message);
            }
        }
    }

    public sealed class SaveManager
    {
        readonly ISaveStore store;
        public SaveData Data { get; private set; }

        public SaveManager(ISaveStore store)
        {
            this.store = store;
            Data = store.Load();
        }

        public void Flush() => store.Write(Data);
    }
}
