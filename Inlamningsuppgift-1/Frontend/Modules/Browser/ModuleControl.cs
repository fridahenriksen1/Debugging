using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frontend.Modules.Browser
{
    // BUG Normalt sett ska de tio vanligaste orden i hemsidan dyka upp, men nu kommer inga ord. Bara sifforna 1 - 10
    /*
       Länken (http://www.textfiles.com/stories/3gables.txt) ska till exempel ge:
       1: the
       2: I
       3: a
       4: to
       5: of
       6: and
       7: you
       8: was
       9: that
       10: in
     */
    /* Först sökte jag i Browser för att se vad jag behöve kolla efter.
     * När jag hade gjort det så gick jag igenom med debuggen i koden många gånger om tills jag förstod att det
     * var något som inte stämde i GetTopTenMostFrequent, så jag gick in där & körde debuggen genom koden på den sidan. 
     * Sedan körde jag debuggen vid word & tenMostCommon & märkte att det kändes som det inte var något som stämde nära tenMostCommon.
     * Så när jag körde debuggen vid highest så såg jag att värdet ändrade sig från 0 och sedan -2147483648 när jag kom ner ett steg till word.
     * Skrev detta på papper även för att komma ihåg, sedan såg jag att highest & keyValue inte skulle
     * kunna bli så att det blir ett ord medan keyValue.Value <= highest,
     * Så jag ändrade keyValue.Value >= highest. Det gjorde jag för att att komma över minus så att det skulle kunna nås & bli ord.
     */
    public partial class ModuleControl : UserControl
    {
        private readonly WebFetcher _webFetcher;

        public ModuleControl()
        {
            InitializeComponent();

            SearchBar.GotFocus += SearchBar_GotFocus;
            SearchBar.KeyPress += SearchBar_KeyPress;

            _webFetcher = new WebFetcher(ProgressBar);
        }

        private async void SearchBar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;
            e.Handled = true;

            string pageContent;

            // TODO Vad gör denna try/catch-satsen och varför är det lägligt att skriva den här och inte innuti FetchSearch() metoden?
            /*Den är bra att använda ifall något blir fel så krashar inte programmet. Det är lägligt för att
             try testar/kollar koden & funkar det som det ska så körs det på. Men skulle det uppstå något fel
             så fångar catch upp och skriver ut ett felmeddelande (exeption.Message). Så genom att skriva den här så kan den fånga
             upp all kod inom browsern.
             */
            try
            {
                pageContent = await _webFetcher.FetchSearch(SearchBar.Text);
            }
            catch (Exception exception) when (exception is 
                    UriFormatException or 
                    HttpRequestException)
            {
                pageContent = exception.Message;
            }

            BrowserOutput.Text = pageContent;

            var words = WordHandler.SplitTextIntoWords(pageContent);
            var topTen = WordHandler.GetTopTenMostFrequent(words);

            string formatted = "";
            for (int i = 0; i < topTen.Length; i++)
            {
                formatted += $"{i + 1}: {topTen[i]}{Environment.NewLine}";
            }
            WordRankingOutput.Text = formatted;
        }

        private void SearchBar_GotFocus(object sender, EventArgs e)
        {
            SearchBar.Text = "";
        }
    }
}
