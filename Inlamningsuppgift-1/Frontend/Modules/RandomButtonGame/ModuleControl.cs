using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frontend.Modules.RandomButtonGame
{
    // BUG Spelet ser ut att fungera som det ska, knapparna hamnar huller om buller.
    // Men när man spelar om spelet så hamnar alla knappar på samma ställe igen istället för att slumpas ut på nya platser.
    /* Jag började med att använda debuggen & tryckte mig in på PrepNewGame(GameAreaPanel).
     * När jag kollade genom metoden tänkte jag att det skulle vara något som inte stämde där i, då
     * jag såg newButton.Location = new Point(x, y);. Förstod att det var något med att ändra
     * knapparnas plats. Jag skrev även upp stödord på papper för att komma ihåg vad kod gjorde.
     * Sedan förstod jag att det behövde läggas till eller ändras något med Random & såg då att
     * Random random = new Random(); hade massa siffror i sig vilket såg skumt ut, så jag testade att ta bort
     * det och då funkade programmet.
     * I början när jag kollade igenom koden så testade jag även att ta bort kod, bara för att se vad som
     * ändrades i spelet när man hade det uppe. Så kan säga att jag använda mig av felsökningsmetoden divide & conquer med. 
     *
     */
    public partial class ModuleControl : UserControl
    {
        //TODO Vad är skillnaden på DateTime och TimeSpan? Ge passande exempel på när man använder den ena gentemot den andra.
        /*DateTime används för att hitta skillnaden mellan datum & tid.
         * TimeSpan används genom att kolla dagar, timmar, minuter & sekunder.
         * T.ex
         * DateTime myBirthday = DateTime.Parse("12/24/1995");
         * TimeSpan myAge = DateTime.Now.Subtract(myBirthday);
         * Console.WriteLine(myAge.TotalDays);
         * Så ser man hur många dagar jag levt.
         */
        private TimeSpan _timePassed;

        private readonly Game _game;
        private readonly List<Score> _scores;

        public ModuleControl()
        {
            InitializeComponent();

            Timer.Interval = 10;
            Timer.Tick += Timer_Tick;

            StartButton.Click += StartButton_Click;

            _scores = new List<Score>();
            _game = new Game();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            Timer.Start();
            StartButton.Hide();

            _game.PrepNewGame(GameAreaPanel);
            
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _timePassed += TimeSpan.FromMilliseconds(10);
            
            if (_game.IsDone())
            {
                Score score = new Score {Text = ClockLabel.Text, Time = _timePassed}; 
                _scores.Add(score);

                UpdateScoreBoard();

                Timer.Stop();
                StartButton.Show();
                _timePassed = TimeSpan.Zero;
            }

            ClockLabel.Text = _timePassed.ToString("mm\\:ss\\:ff");
        }

        private void UpdateScoreBoard()
        {
            HighScorePanel.Controls.Clear();

            _scores.Sort();

            foreach (var score in _scores)
            {
                Label label = new Label();
                label.Text = score.Text;
                label.Dock = DockStyle.Top;

                HighScorePanel.Controls.Add(label);
            }
        }
    }

    struct Score : IComparable<Score>
    {
        public TimeSpan Time;
        public string Text;

        public int CompareTo(Score score)
        {
            return Time.CompareTo(score.Time);
        }
    }
}
