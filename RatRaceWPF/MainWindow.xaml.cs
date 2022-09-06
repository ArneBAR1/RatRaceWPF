using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RatRaceLibary;

namespace RatRaceWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        RaceManager raceManager = new RaceManager();
        public MainWindow()
        {
            InitializeComponent();
            int NumberRats = RNG.Range(2, 9);
            string[] names = { "Lui", "Palle", "Humus", "Ost", "Paladin", "Bard", "Kartoffel", "Ratatouille", "Flæskesteg", "BrunSovs", "Bluey", "Rory" };
            for (int i = 0; i <= NumberRats; i++)
            {
                string RatName = names[i];
                raceManager.CreateRat(RatName);
            }

            List<GetRats> rats = new List<GetRats>();
            foreach (var RatsInRats in raceManager.Rats)
            {
                rats.Add(new GetRats { Name = RatsInRats.Name, Type = RatsInRats.Type });
                RunnersTable.ItemsSource = rats;
            }
        }
        public class GetRats
        {
            public string Name { get; set; }
            public string Type { get; set; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string PlayerName = "";
            PlayerName = textbox_playername.Text;
            int money = RNG.Range(100, 1000);
            raceManager.CreatePlayer(PlayerName, money);
            raceManager.CreatePlayer();
            textbox_playermoney.Text = money.ToString();
            ButtonInfo.IsEnabled = false;
            textbox_playername.IsEnabled = false;
        }
    }
}
