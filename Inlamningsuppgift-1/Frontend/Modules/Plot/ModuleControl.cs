using System;
using System.Drawing;
using System.Windows.Forms;
using OxyPlot;
using OxyPlot.Series;

namespace Frontend.Modules.Plot
{
    // BUG Funktionen "x^2" (x upphöjt till två) ska se ut som en uppvänd parabol (https://www.wolframalpha.com/input/?i=x%5E2).
    // Kolla om det inte är någon miss som skett
    /* Jag började med att öppna grafritaren & såg att den första(min x,2) såg ut som en uppvänd parabol &
     * tänkte att man skulle kunna hitta hur den är skriven någonstans i koden så man skulle kunna skriva x^2 likadant som den.
     * Sedan gick jag igenom med debuggen & kollade igenom koden & så tryckte jag mig in på CreateDataForPlot.
     * När jag kom in dit så såg jag FunctionCollection() över & förstod att det var där graferna blev uppritade.
     * Jag kollade igenom vilken som var vilken & såg att x^2 var den som var näst sist "x\u00b2".
     * Så jag tog bort det som den var deklarerad till och bytte ut det så att det blev samma värde som "min(x, 2)" hade,
     * alltså satte jag in Math.Pow(x, 2) i "x\u00b2". Jag använde mig även av rubberducking, pratade högt för mig själv om hur jag gjorde & vad jag trodde
     * behövde göras. Fick en större helhetsbild då.
     */
    public partial class ModuleControl : UserControl
    {
        private readonly FunctionCollection _funcCollection;

        public ModuleControl()
        {
            InitializeComponent();
            PlotView.Model = new PlotModel();

            PlaceholderButton.Hide();

            _funcCollection = new FunctionCollection();

            string[] functionTexts = _funcCollection.GetAvailableFunctions();
            for (int i = 0; i < functionTexts.Length; i++)
            {
                string function = functionTexts[i];
                var button = MakeButton(function);

                button.Click += Button_Click;
                ButtonsPanel.Controls.Add(button);
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;

            var series = _funcCollection.CreateDataForPlot(button.Text);
            SwapInNewSeries(series);
        }

        private void SwapInNewSeries(Series series)
        {
            PlotView.Model.Series.Clear();
            PlotView.Model.Series.Add(series);

            // Säger till OxyPlot att skapa om hela grafen på nytt
            PlotView.Model.InvalidatePlot(true);
            PlotView.Invalidate();
        }

        private Button MakeButton(string name)
        {
            Button newButton = new Button
            {
                Dock = DockStyle.Top,
                Location = new Point(0, 0),
                Margin = new Padding(6),
                Name = "Button",
                Size = new Size(194, 34),
                TabIndex = 0,
                Text = name,
                UseVisualStyleBackColor = true
            };

            return newButton;
        }
    }
}
