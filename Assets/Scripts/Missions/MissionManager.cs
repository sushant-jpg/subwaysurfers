using System;

namespace LumenRush
{
    public sealed class MissionManager
    {
        readonly SaveManager save;
        public MissionManager(SaveManager save)
        {
            this.save = save;
            Refresh();
        }

        public void Refresh()
        {
            var d = save.Data;
            var now = DateTime.UtcNow;
            string day = now.ToString("yyyy-MM-dd"), week = now.AddDays(-((int)now.DayOfWeek + 6) % 7).ToString("yyyy-MM-dd");
            if (d.dailyPeriod != day)
            {
                d.dailyPeriod = day;
                d.dailyCoins = 0;
                d.dailyMissionClaimed = false;
            }

            if (d.weeklyPeriod != week)
            {
                d.weeklyPeriod = week;
                d.weeklyMeters = 0;
                d.weeklyMissionClaimed = false;
            }
        }

        public void Record(int coins, int meters, int jumps, int powers)
        {
            Refresh();
            var d = save.Data;
            d.totalCoins += coins;
            d.dailyCoins += coins;
            d.weeklyMeters += meters;
            d.totalJumps += jumps;
            d.totalPowers += powers;
            d.runs++;
            d.xp += meters / 5 + coins * 2;
        }

        public bool ClaimDailyReward()
        {
            if (!RunRules.CanClaimDaily(save.Data.dailyReward, DateTime.UtcNow))
                return false;
            save.Data.dailyReward = DateTime.UtcNow.ToString("yyyy-MM-dd");
            save.Data.coins += 100;
            save.Flush();
            return true;
        }

        public void ClaimMissions()
        {
            Refresh();
            var d = save.Data;
            if (d.dailyCoins >= 100 && !d.dailyMissionClaimed)
            {
                d.coins += 150;
                d.dailyMissionClaimed = true;
            }

            if (d.weeklyMeters >= 3000 && !d.weeklyMissionClaimed)
            {
                d.coins += 500;
                d.weeklyMissionClaimed = true;
            }

            int[] values = {d.totalCoins, d.totalJumps, d.totalPowers, d.bestDistance};
            int[] goals = {500, 25, 5, 3000};
            for (int i = 0; i < 4; i++)
                if (values[i] >= goals[i] && (d.claimedAchievements & (1 << i)) == 0)
                {
                    d.claimedAchievements |= 1 << i;
                    d.coins += 200;
                }

            save.Flush();
        }
    }
}
