namespace LumenRush
{
    public sealed class ShopManager
    {
        readonly SaveManager save;
        public ShopManager(SaveManager save)
        {
            this.save = save;
        }

        public bool BuyCharacter(int id)
        {
            if (id < 0 || id > 2)
                return false;
            var d = save.Data;
            if ((d.unlockedCharacters & (1 << id)) != 0)
            {
                d.character = id;
                save.Flush();
                return true;
            }

            int cost = id * 400;
            if (d.coins < cost)
                return false;
            d.coins -= cost;
            d.unlockedCharacters |= 1 << id;
            d.character = id;
            save.Flush();
            return true;
        }

        public bool BuyUpgrade()
        {
            var d = save.Data;
            int cost = RunRules.UpgradeCost(d.upgrade);
            if (d.upgrade >= 5 || d.coins < cost)
                return false;
            d.coins -= cost;
            d.upgrade++;
            save.Flush();
            return true;
        }

        public bool BuyCosmetic(bool board)
        {
            var d = save.Data;
            if (board ? d.board == 1 : d.trail == 1)
                return true;
            if (d.coins < 300)
                return false;
            d.coins -= 300;
            if (board)
                d.board = 1;
            else
                d.trail = 1;
            save.Flush();
            return true;
        }
    }

    public interface ILeaderboardService
    {
        void Submit(int score);
        int PersonalBest { get; }
    }

    public sealed class LocalLeaderboard : ILeaderboardService
    {
        readonly SaveManager save;
        public LocalLeaderboard(SaveManager save)
        {
            this.save = save;
        }

        public int PersonalBest => save.Data.highScore;
        public void Submit(int score)
        {
            save.Data.highScore = System.Math.Max(score, PersonalBest);
            save.Flush();
        }
    }
}
