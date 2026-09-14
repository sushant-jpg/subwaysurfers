using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace LumenRush
{
    public sealed class UIManager : MonoBehaviour
    {
        GameManager game;
        RectTransform safe, panel, hud;
        Font font;
        Text score, coins, distance, powers, notice;
        float refresh;
        static readonly Color Ink = new Color(.035f, .07f, .105f, .96f), Muted = new Color(.57f, .69f, .73f), Accent = new Color(.24f, .98f, .79f);
        public void Initialize(GameManager game)
        {
            this.game = game;
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(540, 960);
            scaler.matchWidthOrHeight = .5f;
            gameObject.AddComponent<GraphicRaycaster>();
            if (EventSystem.current == null)
            {
                var es = new GameObject("UI input");
                es.AddComponent<EventSystem>();
                es.AddComponent<StandaloneInputModule>();
            }

            safe = Rect("Safe area", transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            safe.gameObject.AddComponent<SafeArea>();
            ShowHome();
        }

        RectTransform Rect(string name, Transform parent, Vector2 min, Vector2 max, Vector2 offsetMin, Vector2 offsetMax)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var r = go.GetComponent<RectTransform>();
            r.SetParent(parent, false);
            r.anchorMin = min;
            r.anchorMax = max;
            r.offsetMin = offsetMin;
            r.offsetMax = offsetMax;
            return r;
        }

        Text Label(Transform parent, string value, float y, int size, Color color, float height = 50)
        {
            var r = Rect(value, parent, new Vector2(0, 1), Vector2.one, new Vector2(30, -y - height), new Vector2(-30, -y));
            var text = r.gameObject.AddComponent<Text>();
            text.font = font;
            text.text = value;
            text.fontSize = size;
            text.color = color;
            text.alignment = TextAnchor.MiddleLeft;
            text.supportRichText = true;
            return text;
        }

        void Button(string title, float y, Action action, bool primary = false)
        {
            var r = Rect(title, panel, new Vector2(0, 1), Vector2.one, new Vector2(30, -y - 55), new Vector2(-30, -y));
            var bg = r.gameObject.AddComponent<Image>();
            bg.color = primary ? Accent : new Color(.11f, .18f, .23f);
            var b = r.gameObject.AddComponent<Button>();
            b.targetGraphic = bg;
            var colors = b.colors;
            colors.highlightedColor = new Color(.8f, 1, 1);
            colors.pressedColor = new Color(.5f, .75f, .75f);
            b.colors = colors;
            var tr = Rect("Label", r, Vector2.zero, Vector2.one, new Vector2(20, 0), new Vector2(-20, 0));
            var text = tr.gameObject.AddComponent<Text>();
            text.font = font;
            text.text = title;
            text.fontSize = 19;
            text.fontStyle = FontStyle.Bold;
            text.color = primary ? Ink : Color.white;
            text.alignment = TextAnchor.MiddleLeft;
            b.onClick.AddListener(() =>
            {
                game.Audio.PlayTone(660, .06f);
                action();
            });
        }

        void Clear()
        {
            if (panel != null)
            {
                panel.gameObject.SetActive(false);
                Destroy(panel.gameObject);
            }

            if (hud != null)
            {
                hud.gameObject.SetActive(false);
                Destroy(hud.gameObject);
            }

            panel = null;
            hud = null;
        }

        void Page(string eyebrow, string title, string subtitle)
        {
            Clear();
            panel = Rect("Panel", safe, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            panel.gameObject.AddComponent<Image>().color = Ink;
            Label(panel, eyebrow, 35, 14, Accent);
            Label(panel, title, 85, 40, Color.white, 70);
            Label(panel, subtitle, 162, 17, Muted, 75);
            notice = Label(panel, "", 775, 16, Accent, 65);
        }

        string Wallet => game.Save.Data.coins.ToString("N0") + " TOKENS";
        public void ShowHome()
        {
            Page("AFTERLIGHT TRANSIT  /  01", "LUMEN RUSH", "Own the night. Find your flow.");
            // The compact wordmark leaves the city visible below the navigation panel.
            panel.GetComponent<Image>().color = new Color(.025f, .055f, .09f, .88f);
            Label(panel, "PERSONAL BEST    " + game.Save.Data.highScore.ToString("N0"), 237, 15, Accent);
            Button("START A RUN                                      →", 305, game.StartRun, true);
            Button("COURIERS                   Characters & style", 376, ShowCharacters);
            Button("THE EXCHANGE           Upgrades & cosmetics", 441, ShowShop);
            Button("ASSIGNMENTS              Missions & achievements", 506, ShowMissions);
            Button("DAILY DROP                 Collect your reward", 571, () =>
            {
                ShowHome();
                notice.text = game.Missions.ClaimDailyReward() ? "100 tokens delivered. Come back tomorrow." : "Today's drop is collected. Returns at 00:00 UTC.";
            });
            Button("YOUR RECORD              Profile & leaderboard", 636, ShowProfile);
            Button("SETTINGS", 701, ShowSettings);
            Label(panel, "SWIPE TO MOVE  ·  UP TO JUMP  ·  DOWN TO SLIDE", 857, 12, Muted);
        }

        public void ShowHUD()
        {
            Clear();
            hud = Rect("Run HUD", safe, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            var top = Rect("Top scrim", hud, new Vector2(0, 1), Vector2.one, new Vector2(0, -165), Vector2.zero);
            top.gameObject.AddComponent<Image>().color = new Color(.025f, .055f, .09f, .82f);
            score = Label(hud, "0", 22, 38, Color.white);
            coins = Label(hud, "", 75, 18, Accent);
            distance = Label(hud, "", 111, 13, Muted);
            powers = Label(hud, "", 185, 17, Accent, 230);
            var pause = Rect("Pause", hud, Vector2.one, Vector2.one, new Vector2(-95, -80), new Vector2(-25, -25));
            pause.gameObject.AddComponent<Image>().color = Ink;
            var button = pause.gameObject.AddComponent<Button>();
            button.onClick.AddListener(game.TogglePause);
            var label = Label(pause, "II", 0, 24, Color.white, 55);
            label.rectTransform.offsetMin = new Vector2(23, -55);
            label.rectTransform.offsetMax = new Vector2(0, 0);
            Label(hud, "←  →  CHANGE LANE     ↑  JUMP     ↓  SLIDE", 870, 13, Color.white);
        }

        public void ShowPause()
        {
            Page("TAKE A BREATHER", "RUN PAUSED", "Your route will be right here.");
            Button("RESUME", 310, game.TogglePause, true);
            Button("RESTART", 380, game.Retry);
            Button("FINISH & RETURN HOME", 450, game.Home);
        }

        public void ShowGameOver()
        {
            Page("ROUTE COMPLETE", "NICE RUN.", "Every street is a new start.");
            Label(panel, game.Score.ToString("N0"), 265, 62, Accent, 85);
            Label(panel, $"{(int)game.Distance:N0} METERS     +{game.Coins} TOKENS", 360, 19, Color.white);
            if (!game.Revived)
                Button("SECOND WIND                      Free revive", 455, game.Revive, true);
            Button("RUN AGAIN", 525, game.Retry, game.Revived);
            Button("COLLECT & RETURN HOME", 595, game.Home);
            Label(panel, "One free revive per run. No ads. No payment.", 685, 16, Muted);
        }

        public void OpenScene(string scene)
        {
            if (scene == "CharacterSelection")
                ShowCharacters();
            else if (scene == "Shop")
                ShowShop();
        }

        void ShowCharacters()
        {
            Page("COURIER COLLECTIVE", "FIND YOUR STYLE", Wallet + "\nEvery courier has identical movement and abilities.");
            string[] names = {"ARIA  /  Tide jacket", "NIKO  /  Ember jacket", "SOL  /  Ultraviolet jacket"};
            for (int i = 0; i < 3; i++)
            {
                int id = i;
                bool owned = (game.Save.Data.unlockedCharacters & (1 << i)) != 0;
                Button(names[i] + (game.Save.Data.character == i ? "   ✓" : owned ? "   Equip" : "   " + (i * 400)), 290 + i * 75, () =>
                {
                    bool ok = game.Shop.BuyCharacter(id);
                    game.Player.ResetRunner();
                    ShowCharacters();
                    if (!ok)
                        notice.text = "Keep running to earn more tokens.";
                }, game.Save.Data.character == i);
            }

            Label(panel, "Original courier designs. Cosmetic changes only.", 570, 17, Muted, 65);
            Button("← BACK", 700, ShowHome);
        }

        void ShowShop()
        {
            var d = game.Save.Data;
            Page("THE EXCHANGE", "MAKE IT YOURS", Wallet + "\nEverything is earned through play.");
            Button(d.upgrade >= 5 ? "POWER DURATION  /  MAX LEVEL" : $"POWER DURATION +1s   /   {RunRules.UpgradeCost(d.upgrade)} tokens", 290, () =>
            {
                bool ok = game.Shop.BuyUpgrade();
                ShowShop();
                notice.text = ok ? "Power duration upgraded." : "Maximum level or insufficient tokens.";
            }, true);
            Label(panel, $"Duration upgrade {d.upgrade}/5  ·  Applies to all power-ups", 355, 16, Muted);
            Button(d.trail == 1 ? "AFTERGLOW TRAIL  /  OWNED" : "AFTERGLOW TRAIL  /  300 tokens", 430, () => BuyCosmetic(false));
            Button(d.board == 1 ? "AERO DECK  /  OWNED" : "AERO DECK  /  300 tokens", 505, () => BuyCosmetic(true));
            Button("← BACK", 700, ShowHome);
        }

        void BuyCosmetic(bool board)
        {
            bool ok = game.Shop.BuyCosmetic(board);
            ShowShop();
            notice.text = ok ? "Cosmetic equipped for your next run." : "Not enough tokens yet.";
        }

        void ShowMissions()
        {
            game.Missions.Refresh();
            var d = game.Save.Data;
            Page("ASSIGNMENTS", "KEEP MOVING", Wallet + "\nClaim completed assignments to receive tokens.");
            Label(panel, $"DAILY  /  Collect tokens       {Math.Min(100, d.dailyCoins)}/100\nReward: 150 tokens" + (d.dailyMissionClaimed ? "  ·  CLAIMED" : ""), 255, 19, Color.white, 80);
            Label(panel, $"WEEKLY  /  Run distance       {Math.Min(3000, d.weeklyMeters)}/3,000 m\nReward: 500 tokens" + (d.weeklyMissionClaimed ? "  ·  CLAIMED" : ""), 350, 19, Color.white, 80);
            Label(panel, $"CAREER ACHIEVEMENTS  /  200 tokens each\nCollector   {d.totalCoins}/500 coins\nSkybound   {d.totalJumps}/25 jumps\nCharged   {d.totalPowers}/5 power-ups\nLong haul   {d.bestDistance}/3,000 m best", 450, 18, Muted, 180);
            Button("CLAIM COMPLETED REWARDS", 635, () =>
            {
                int before = game.Save.Data.coins;
                game.Missions.ClaimMissions();
                ShowMissions();
                notice.text = $"{game.Save.Data.coins - before} tokens claimed.";
            }, true);
            Button("← BACK", 705, ShowHome);
        }

        void ShowProfile()
        {
            var d = game.Save.Data;
            Page("YOUR RECORD", "THE LONG RUN", $"COURIER LEVEL {RunRules.Level(d.xp)}   /   {d.xp:N0} XP");
            Label(panel, $"PERSONAL LEADERBOARD\n01    YOU                         {d.highScore:N0}", 275, 22, Accent, 100);
            Label(panel, $"{d.runs:N0} runs completed\n{d.bestDistance:N0} m longest route\n{d.totalCoins:N0} lifetime tokens\n{d.totalJumps:N0} jumps\n{d.totalPowers:N0} power-ups collected", 410, 23, Color.white, 210);
            Label(panel, "Local record on this device. Online rankings are not connected.", 630, 16, Muted, 60);
            Button("← BACK", 715, ShowHome);
        }

        void ShowSettings()
        {
            var d = game.Save.Data;
            Page("PREFERENCES", "YOUR RHYTHM", "Tune your run.");
            Button("MUSIC   /   " + (d.music ? "ON" : "OFF"), 280, () =>
            {
                d.music = !d.music;
                game.ApplySettings();
                ShowSettings();
            });
            Button("SOUND   /   " + (d.sound ? "ON" : "OFF"), 350, () =>
            {
                d.sound = !d.sound;
                game.ApplySettings();
                ShowSettings();
            });
            Button("HAPTICS   /   " + (d.haptics ? "ON" : "OFF"), 420, () =>
            {
                d.haptics = !d.haptics;
                game.ApplySettings();
                ShowSettings();
            });
            Button("GRAPHICS   /   " + (d.lowGraphics ? "BATTERY  ·  30 FPS" : "QUALITY  ·  60 FPS"), 490, () =>
            {
                d.lowGraphics = !d.lowGraphics;
                game.ApplySettings();
                ShowSettings();
            });
            Label(panel, "Keyboard: A/D or arrows to move; Space to jump;\nS to slide; P or Escape to pause.\nTouch: swipe anywhere outside buttons.", 575, 16, Muted, 110);
            Button("← BACK", 715, ShowHome);
        }

        void Update()
        {
            if (hud == null || game.State != RunState.Running)
                return;
            refresh -= Time.unscaledDeltaTime;
            if (refresh > 0)
                return;
            refresh = .1f;
            score.text = game.Score.ToString("N0");
            coins.text = $"◈  {game.Coins}     /     ×{game.Multiplier}";
            distance.text = $"{(int)game.Distance:N0} M   ·   {game.World.District}";
            string active = game.Chase > .1f ? "PATROL CLOSE  /  Stay clear to recover\n" : "";
            foreach (PowerKind kind in Enum.GetValues(typeof(PowerKind)))
                if (game.PowerUps.Active(kind))
                    active += $"{kind}   {game.PowerUps.Remaining(kind):0.0}s\n";
            if (game.Distance < 65)
                active += "FOLLOW THE TOKENS\nOrange: jump  ·  Overhead: slide\nCargo pods: change lanes";
            powers.text = active;
        }
    }
}
