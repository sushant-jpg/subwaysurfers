using UnityEngine;

namespace LumenRush
{
    public enum RunState
    {
        Menu,
        Running,
        Paused,
        GameOver
    }

    public sealed class GameManager : MonoBehaviour
    {
        public RunnerConfig Config { get; private set; }

        public SaveManager Save { get; private set; }

        public MissionManager Missions { get; private set; }

        public ShopManager Shop { get; private set; }

        public ILeaderboardService Leaderboard { get; private set; }

        public SwipeInputManager Input { get; private set; }

        public PlayerController Player { get; private set; }

        public AudioManager Audio { get; private set; }

        public PowerUpManager PowerUps { get; private set; }

        public LevelGenerator World { get; private set; }

        public UIManager UI { get; private set; }

        public RunState State { get; private set; } = RunState.Menu;
        public float Distance { get; private set; }

        public int Coins { get; private set; }

        public int Jumps, Powers;
        public float Chase { get; private set; }

        public bool Revived { get; private set; }

        public float Speed => RunRules.Speed(Distance) * (PowerUps.Active(PowerKind.SpeedBurst) ? 1.3f : PowerUps.Active(PowerKind.SlowMotion) ? .65f : 1);
        public int Multiplier => 1 + Mathf.Min(4, (int)(Distance / 500)) + (PowerUps.Active(PowerKind.ScoreBooster) ? 2 : 0);
        readonly ScoreManager scoreManager = new ScoreManager();
        public int Score => scoreManager.Total(Coins);
        bool banked;
        float immunity;
        void Awake()
        {
            Time.maximumDeltaTime = .05f;
            Config = Resources.Load<RunnerConfig>("RunnerConfig");
            if (Config == null)
                Config = ScriptableObject.CreateInstance<RunnerConfig>();
            Save = new SaveManager(new LocalSaveStore());
            Missions = new MissionManager(Save);
            Shop = new ShopManager(Save);
            Leaderboard = new LocalLeaderboard(Save);
            Input = gameObject.AddComponent<SwipeInputManager>();
            Audio = gameObject.AddComponent<AudioManager>();
            Audio.Initialize(this);
            PowerUps = new PowerUpManager(this);
            World = new GameObject("Pooled city").AddComponent<LevelGenerator>();
            World.Initialize(this);
            Player = new GameObject("Aria - transit courier").AddComponent<PlayerController>();
            Player.Initialize(this);
            Player.ResetRunner();
            Player.gameObject.AddComponent<CosmeticEffects>().Initialize(this);
            var cameraObject = new GameObject("Follow camera");
            cameraObject.tag = "MainCamera";
            cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
            cameraObject.AddComponent<FollowCamera>().Initialize(this);
            UI = new GameObject("Mobile interface").AddComponent<UIManager>();
            UI.Initialize(this);
            Input.Pause += TogglePause;
            ApplySettings();
            string scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (scene == "Gameplay")
                StartRun();
            else
                UI.OpenScene(scene);
        }

        public void StartRun()
        {
            Time.timeScale = 1;
            Distance = 0;
            Coins = 0;
            Jumps = 0;
            Powers = 0;
            Chase = 0;
            Revived = false;
            banked = false;
            immunity = 2;
            scoreManager.Reset();
            PowerUps.Reset();
            World.ResetWorld();
            Player.ResetRunner();
            State = RunState.Running;
            UI.ShowHUD();
        }

        public void TogglePause()
        {
            if (State == RunState.Running)
            {
                State = RunState.Paused;
                Time.timeScale = 0;
                UI.ShowPause();
            }
            else if (State == RunState.Paused)
            {
                State = RunState.Running;
                Time.timeScale = 1;
                UI.ShowHUD();
            }
        }

        public void Home()
        {
            BankRun();
            Time.timeScale = 1;
            State = RunState.Menu;
            UI.ShowHome();
        }

        public void CollectCoin()
        {
            Coins += PowerUps.Active(PowerKind.DoubleCoins) ? 2 : 1;
            Audio.PlayTone(1050, .055f);
        }

        public void Hit(bool major)
        {
            if (State != RunState.Running || immunity > 0)
                return;
            Audio.PlayTone(65, .25f);
            if (Save.Data.haptics && Application.isMobilePlatform)
                Handheld.Vibrate();
            if (PowerUps.Absorb())
            {
                immunity = 1.2f;
                return;
            }

            if (!major && Chase < .6f)
            {
                Chase = 1;
                immunity = 1;
                return;
            }

            State = RunState.GameOver;
            UI.ShowGameOver();
        }

        public void Revive()
        {
            if (State != RunState.GameOver || Revived)
                return;
            Revived = true;
            Chase = 0;
            immunity = 3;
            World.ClearNearPlayer();
            State = RunState.Running;
            UI.ShowHUD();
        }

        void BankRun()
        {
            if (banked || Distance <= 0)
                return;
            banked = true;
            var d = Save.Data;
            d.coins += Coins;
            d.bestDistance = Mathf.Max(d.bestDistance, (int)Distance);
            Missions.Record(Coins, (int)Distance, Jumps, Powers);
            Leaderboard.Submit(Score);
            Save.Flush();
        }

        public void Retry()
        {
            BankRun();
            StartRun();
        }

        void Update()
        {
            if (State != RunState.Running)
                return;
            float dt = Time.deltaTime;
            float meters = Speed * dt;
            scoreManager.Advance(meters, Multiplier);
            Distance += meters;
            Chase = Mathf.Max(0, Chase - dt * .09f);
            immunity = Mathf.Max(0, immunity - dt);
            PowerUps.Tick(dt);
        }

        public void ApplySettings()
        {
            Application.targetFrameRate = Save.Data.lowGraphics ? 30 : 60;
            QualitySettings.vSyncCount = 0;
            QualitySettings.shadows = Save.Data.lowGraphics ? ShadowQuality.Disable : ShadowQuality.HardOnly;
            Audio.ApplySettings();
            Save.Flush();
        }

        void OnApplicationPause(bool paused)
        {
            if (paused)
            {
                if (State == RunState.Running)
                    TogglePause();
                Save.Flush();
            }
        }

        void OnApplicationQuit()
        {
            BankRun();
            Save.Flush();
        }
    }
}
