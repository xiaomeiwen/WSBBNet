using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace WSBB
{
    public class Scoreboard : MonoBehaviour
    {

        public int guiDrawOrder;

        [HideInInspector]
        public Team_Manager teamManager;

        public Rect timeAreaSec;
        public Rect timeAreaSecLarge;
        public Rect timeAreaMin;
        public Rect timeAreaMinLarge;
        public Rect homeArea;
        public Rect homeAreaLarge;
        public Rect awayArea;
        public Rect awayAreaLarge;
        public Rect quarterArea;
        public Rect quarterAreaLarge;
        public Rect shotClockArea;
        public Rect shotClockAreaLarge;
        public Rect boardArea;

        public GUISkin clockSkin;
        public GUISkin clockSkin_Large;
        public GUISkin scoreSkin;
        public GUISkin scoreSkin_Large;
        public GUISkin otherSkin;
        public GUISkin otherSkin_Large;
        public GUISkin backdropSkin;
        public GUISkin backdropSkin_Large;

        public Texture2D boardImage;

        public bool framerate;
        public Rect framerateArea;
        public float calcedFramerate;
        public int framerateDisDelay;


        private bool dataLoaded = false;

        public static int quarterNumber;

        public static float gameTime;
        public static float shotTime;

        public static Scoreboard instance;

        void Awake()
        {
            teamManager = GameObject.Find("Team_Manager").GetComponent<Team_Manager>();

            Scoreboard.quarterNumber = 0;
            Scoreboard.shotTime = 45;
            Scoreboard.gameTime = GameData.quarterLength * 60;
            if (Screen.height != ScreenHelpers.idealScreenHeight)
            {
                timeAreaSec = timeAreaSecLarge;
                timeAreaMin = timeAreaMinLarge;
                homeArea = homeAreaLarge;
                awayArea = awayAreaLarge;
                quarterArea = quarterAreaLarge;
                shotClockArea = shotClockAreaLarge;
                boardArea = ScreenHelpers.scaleRectToScreen(boardArea);
                clockSkin = clockSkin_Large;
                scoreSkin = scoreSkin_Large;
                otherSkin = otherSkin_Large;
                backdropSkin = backdropSkin_Large;
                framerateArea = ScreenHelpers.scaleRectToScreen(framerateArea);
            }
            framerateDisDelay = 0;
            Scoreboard.instance = this;
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (GameData.instance == null)
            {
                return;
            }
            else if (!dataLoaded)
            {
                Scoreboard.gameTime = GameData.quarterLength * 60;
                dataLoaded = true;
            }

            if (GameAdmin.GamePause)
                return;

            Scoreboard.gameTime -= Time.deltaTime;
            Scoreboard.shotTime -= Time.deltaTime;

            if (Scoreboard.gameTime <= 0)
                Scoreboard.gameTime = 0;
            if (Scoreboard.shotTime <= 0)
                Scoreboard.shotTime = 0;

            if (framerate)
            {
                framerateDisDelay += 1;
                if (framerateDisDelay > 50)
                {
                    calcedFramerate = 1 / Time.deltaTime;
                    framerateDisDelay = 0;
                }
            }
        }

        public void OnGUI()
        {
            int minutes = (int)(gameTime / 60);
            int seconds = (int)(gameTime % 60);
            int shotClockSec = (int)Scoreboard.shotTime;
            GUI.skin = clockSkin;
            GUI.depth = guiDrawOrder;
            GUI.Label(boardArea, boardImage);

            GUI.skin = backdropSkin;
			timeAreaSec = new Rect(370,30,40,38);
			timeAreaMin = new Rect(298, 30, 80, 76);
            GUI.Label(timeAreaSec, "88");
            GUI.Label(timeAreaMin, "88");
			homeArea = new Rect (175, 20, 80, 76);
            GUI.Label(homeArea, "88");
			awayArea = new Rect (475, 20, 80, 76);
            GUI.Label(awayArea, "88");
			shotClockArea = new Rect(415,30, 80, 76);
            GUI.Label(shotClockArea, "88");
			quarterArea = new Rect (237, 30, 80, 76);
            GUI.Label(quarterArea, "88");

            GUI.skin = clockSkin;
            if (seconds >= 10)
                GUI.Label(timeAreaSec, "" + seconds);
            else
                GUI.Label(timeAreaSec, "0" + seconds);

            if (minutes >= 10)
                GUI.Label(timeAreaMin, "" + minutes);
            else
                GUI.Label(timeAreaMin, "0" + minutes);

            GUI.skin = scoreSkin;

            if (teamManager.homeTeam.score < 10)
                GUI.Label(homeArea, "0" + teamManager.homeTeam.score);
            else
                GUI.Label(homeArea, "" + teamManager.homeTeam.score);


            if (teamManager.awayTeam.score < 10)
                GUI.Label(awayArea, "0" + teamManager.awayTeam.score);
            else
                GUI.Label(awayArea, "" + teamManager.awayTeam.score);

            GUI.skin = otherSkin;
            GUI.Label(quarterArea, " " + Scoreboard.quarterNumber);
            if (shotClockSec >= 10)
                GUI.Label(shotClockArea, "" + shotClockSec);
            else
                GUI.Label(shotClockArea, "0" + shotClockSec);

            if (GameAdmin.GamePause)
                return;
            GUI.skin = null;
            if (framerate)
            {
                GUI.Box(framerateArea, "" + calcedFramerate);
            }
        }
    }
}

/*
namespace WSBB
{
    public class Scoreboard : MonoBehaviour
    {

        public Text quarter, minutes, seconds, home, away, shot;
        public static float gameTime, shotTime;
        public static int homeScore, awayScore, quarterNumber;
        public Team_Manager teamManager;
        public static Scoreboard instance;

        string formatHelper(float value)
        {
            if (value < 10)
                return "0" + value;
            else return "" + value;
        }

        void Awake()
        {
            teamManager = GameObject.Find("Team_Manager").GetComponent<Team_Manager>();
        }

        void Start()
        {
            gameTime = GameData.quarterLength * 60;
            instance = this;
        }

        // Update is called once per frame
        void Update()
        {
            gameTime -= Time.deltaTime;
            shotTime -= Time.deltaTime;
            if (shotTime < 0) shotTime = 0;
            if (gameTime < 0) gameTime = 0;
            
            home.text = formatHelper(teamManager.homeTeam.score);
            away.text = formatHelper(teamManager.awayTeam.score);
            minutes.text = formatHelper((int)(gameTime / 60));
            seconds.text = formatHelper((int)gameTime % 60);
            shot.text = formatHelper((int)shotTime);
            quarter.text = quarterNumber.ToString();
            
        }
    }
}
*/


