using System.Text.Json;

namespace WordleGame
{
    /// <summary>
    /// Game configuration class responsible for managing core Wordle game settings
    /// including maximum rounds, word list, and other game parameters
    /// </summary>
    public class GameConfiguration
    {
        /// <summary>
        /// Maximum number of rounds for the game, defaults to 6 rounds
        /// </summary>
        public int MaxRounds { get; set; } = 6;
        
        /// <summary>
        /// List of words available for the game, all words are converted to uppercase
        /// </summary>
        public List<string> WordList { get; set; } = new();

        /// <summary>
        /// Constructor that initializes game configuration and loads the word list
        /// </summary>
        public GameConfiguration()
        {
            LoadWordList();
        }

        /// <summary>
        /// Loads the word list, prioritizing JSON files, falls back to default words if files don't exist
        /// Loading order: config/words.json -> words.json -> default word list
        /// </summary>
        private void LoadWordList()
        {
            try
            {
                // First try to load from config directory
                if (File.Exists("config/words.json"))
                {
                    var jsonContent = File.ReadAllText("config/words.json");
                    var config = JsonSerializer.Deserialize<WordListConfig>(jsonContent);
                    if (config?.Words != null)
                    {
                        WordList = config.Words.Select(w => w.ToUpper()).ToList();
                    }
                }
                // If config directory doesn't exist, try loading from root directory
                else if (File.Exists("words.json"))
                {
                    var jsonContent = File.ReadAllText("words.json");
                    var config = JsonSerializer.Deserialize<WordListConfig>(jsonContent);
                    if (config?.Words != null)
                    {
                        WordList = config.Words.Select(w => w.ToUpper()).ToList();
                    }
                }
                // If no files exist, use default word list and save to file
                else
                {
                    // Fallback to default words if file doesn't exist
                    WordList = GetDefaultWords();
                    SaveWordList();
                }
            }
            catch (Exception)
            {
                // If there's an error during loading, use default word list
                WordList = GetDefaultWords();
            }
        }

        /// <summary>
        /// Saves the current word list to a JSON file
        /// Uses indented format for better readability and editing
        /// </summary>
        private void SaveWordList()
        {
            try
            {
                var config = new WordListConfig { Words = WordList };
                var jsonContent = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText("words.json", jsonContent);
            }
            catch (Exception)
            {
                // Ignore save errors, don't affect game operation
            }
        }

        /// <summary>
        /// Gets the default word list containing common 5-letter English words
        /// These words serve as a backup vocabulary to ensure the game always has available words
        /// </summary>
        /// <returns>List containing default words</returns>
        private List<string> GetDefaultWords()
        {
            return new List<string>
            {
                "ABOUT", "ABOVE", "ABUSE", "ACTOR", "ACUTE", "ADMIT", "ADOPT", "ADULT", "AFTER", "AGAIN",
                "AGENT", "AGREE", "AHEAD", "ALARM", "ALBUM", "ALERT", "ALIKE", "ALIVE", "ALLOW", "ALONE",
                "ALONG", "ALTER", "AMONG", "ANGER", "ANGLE", "ANGRY", "APART", "APPLE", "APPLY", "ARENA",
                "ARGUE", "ARISE", "ARRAY", "ASIDE", "ASSET", "AUDIO", "AUDIT", "AVOID", "AWARD", "AWARE",
                "BADLY", "BAKER", "BASES", "BASIC", "BASIS", "BEACH", "BEGAN", "BEGIN", "BEING", "BELOW",
                "BENCH", "BILLY", "BIRTH", "BLACK", "BLAME", "BLIND", "BLOCK", "BLOOD", "BOARD", "BOOST",
                "BOOTH", "BOUND", "BRAIN", "BRAND", "BREAD", "BREAK", "BREED", "BRIEF", "BRING", "BROAD",
                "BROKE", "BROWN", "BUILD", "BUILT", "BUYER", "CABLE", "CALIF", "CARRY", "CATCH", "CAUSE",
                "CHAIN", "CHAIR", "CHART", "CHASE", "CHEAP", "CHECK", "CHEST", "CHIEF", "CHILD", "CHINA",
                "CHOSE", "CIVIL", "CLAIM", "CLASS", "CLEAN", "CLEAR", "CLICK", "CLIMB", "CLOCK", "CLOSE",
                "COACH", "COAST", "COULD", "COUNT", "COURT", "COVER", "CRAFT", "CRASH", "CREAM", "CRIME",
                "CROSS", "CROWD", "CROWN", "CURVE", "CYCLE", "DAILY", "DANCE", "DATED", "DEALT", "DEATH",
                "DEBUT", "DELAY", "DEPTH", "DOING", "DOUBT", "DOZEN", "DRAFT", "DRAMA", "DRAWN", "DREAM",
                "DRESS", "DRINK", "DRIVE", "DROVE", "DYING", "EAGER", "EARLY", "EARTH", "EIGHT", "ELITE",
                "EMPTY", "ENEMY", "ENJOY", "ENTER", "ENTRY", "EQUAL", "ERROR", "EVENT", "EVERY", "EXACT",
                "EXIST", "EXTRA", "FAITH", "FALSE", "FAULT", "FIBER", "FIELD", "FIFTH", "FIFTY", "FIGHT",
                "FINAL", "FIRST", "FIXED", "FLASH", "FLEET", "FLOOR", "FLUID", "FOCUS", "FORCE", "FORTH",
                "FORTY", "FORUM", "FOUND", "FRAME", "FRANK", "FRAUD", "FRESH", "FRONT", "FRUIT", "FULLY",
                "FUNNY", "GIANT", "GIVEN", "GLASS", "GLOBE", "GOING", "GRACE", "GRADE", "GRAND", "GRANT",
                "GRASS", "GRAVE", "GREAT", "GREEN", "GROSS", "GROUP", "GROWN", "GUARD", "GUESS", "GUEST",
                "GUIDE", "HAPPY", "HARRY", "HEART", "HEAVY", "HENCE", "HENRY", "HORSE", "HOTEL", "HOUSE",
                "HUMAN", "IDEAL", "IMAGE", "INDEX", "INNER", "INPUT", "ISSUE", "JAPAN", "JIMMY", "JOINT",
                "JONES", "JUDGE", "KNOWN", "LABEL", "LARGE", "LASER", "LATER", "LAUGH", "LAYER", "LEARN",
                "LEASE", "LEAST", "LEAVE", "LEGAL", "LEVEL", "LEWIS", "LIGHT", "LIMIT", "LINKS", "LIVES",
                "LOCAL", "LOOSE", "LOWER", "LUCKY", "LUNCH", "LYING", "MAGIC", "MAJOR", "MAKER", "MARCH",
                "MARIA", "MATCH", "MAYBE", "MAYOR", "MEANT", "MEDIA", "METAL", "MIGHT", "MINOR", "MINUS",
                "MIXED", "MODEL", "MONEY", "MONTH", "MORAL", "MOTOR", "MOUNT", "MOUSE", "MOUTH", "MOVED",
                "MOVIE", "MUSIC", "NEEDS", "NEVER", "NEWLY", "NIGHT", "NOISE", "NORTH", "NOTED", "NOVEL",
                "NURSE", "OCCUR", "OCEAN", "OFFER", "ONION", "OPERA", "OTHER", "OUGHT",
                "OUTER", "OWNER", "PAINT", "PANEL", "PAPER", "PARTY", "PEACE", "PETER", "PHASE", "PHONE",
                "PHOTO", "PIECE", "PILOT", "PITCH", "PLACE", "PLAIN", "PLANE", "PLANT", "PLATE", "POINT",
                "POUND", "POWER", "PRESS", "PRICE", "PRIDE", "PRIME", "PRINT", "PRIOR", "PRIZE", "PROOF",
                "PROUD", "PROVE", "QUEEN", "QUICK", "QUIET", "QUITE", "RADIO", "RAISE", "RANGE", "RAPID",
                "RATIO", "REACH", "READY", "REALM", "REBEL", "REFER", "RELAX", "REPLY", "RIDER", "RIDGE",
                "RIGHT", "RIVAL", "RIVER", "ROBIN", "ROGER", "ROMAN", "ROUGH", "ROUND", "ROUTE", "ROYAL",
                "RURAL", "SALLY", "SCALE", "SCENE", "SCOPE", "SCOTT", "SCREW", "SEIZE", "SELLS", "SENDS",
                "SERVE", "SEVEN", "SHADE", "SHAKE", "SHAPE", "SHARE", "SHARP", "SHEEP", "SHEER", "SHEET",
                "SHELF", "SHELL", "SHIFT", "SHIRT", "SHOCK", "SHOOT", "SHORT", "SHOWN", "SHOWS", "SIDED",
                "SIGHT", "SIGNS", "SINCE", "SIXTH", "SIXTY", "SIZED", "SKILL", "SLEEP", "SLIDE", "SMALL",
                "SMART", "SMILE", "SMITH", "SMOKE", "SOLID", "SOLVE", "SORRY", "SOUND", "SOUTH", "SPACE",
                "SPARE", "SPEAK", "SPEED", "SPEND", "SPENT", "SPLIT", "SPOKE", "SPORT", "STAFF", "STAGE",
                "STAKE", "STAND", "START", "STATE", "STAYS", "STEAM", "STEEL", "STICK", "STILL", "STOCK",
                "STONE", "STOOD", "STORE", "STORM", "STORY", "STRIP", "STUCK", "STUDY", "STUFF", "STYLE",
                "SUGAR", "SUITE", "SUPER", "SWEET", "TABLE", "TAKEN", "TASTE", "TAXES", "TEACH", "TEETH",
                "TERRY", "TEXAS", "THANK", "THEFT", "THEIR", "THEME", "THERE", "THESE", "THICK", "THING",
                "THINK", "THIRD", "THOSE", "THREE", "THREW", "THROW", "THUMB", "TIGER", "TIGHT", "TIMED",
                "TIMER", "TIRED", "TITLE", "TODAY", "TOPIC", "TOTAL", "TOUCH", "TOUGH", "TOWER", "TRACK",
                "TRADE", "TRAIN", "TREAT", "TREND", "TRIAL", "TRIED", "TRIES", "TRUCK", "TRULY", "TRUNK",
                "TRUST", "TRUTH", "TWICE", "UNDER", "UNDUE", "UNION", "UNITY", "UNTIL", "UPPER", "UPSET",
                "URBAN", "USAGE", "USUAL", "VALID", "VALUE", "VIDEO", "VIRUS", "VISIT", "VITAL", "VOICE",
                "WASTE", "WATCH", "WATER", "WHEEL", "WHERE", "WHICH", "WHILE", "WHITE", "WHOLE", "WHOSE",
                "WOMAN", "WOMEN", "WORLD", "WORRY", "WORSE", "WORST", "WOULD", "WOUND", "WRITE", "WRONG",
                "WROTE", "YIELD", "YOUNG", "YOUTH"
            };
        }
    }

    /// <summary>
    /// Word list configuration class for JSON serialization and deserialization
    /// </summary>
    public class WordListConfig
    {
        /// <summary>
        /// Word list containing all words available for the game
        /// </summary>
        public List<string> Words { get; set; } = new();
    }
} 