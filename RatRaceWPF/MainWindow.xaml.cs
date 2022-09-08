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
using RatRaceLibary.Items;

namespace RatRaceWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        RaceManager raceManager = new RaceManager();
        SpeedShoes speedshoes = new SpeedShoes();
        Carrot carrot = new Carrot();
        UltimateDice ultimateDice = new UltimateDice();
        List<CoolItems> coolItems = new List<CoolItems>();
        List<TextBlock> lanes = new List<TextBlock>();
        List<GetRats> rats = new List<GetRats>();
        List<NextRace> tracks = new List<NextRace>();

        int money;
        string PlayerName;
        public MainWindow()
        {
            InitializeComponent();

            int NumberRats = RNG.Range(2, 8);
            string[] names = { "Lui", "Palle", "Humus", "Ost", "Paladin", "Bard", "Kartoffel", "Ratatouille", "Flæskesteg", "BrunSovs", "Bluey", "Rory" };
            for (int i = 0; i <= NumberRats; i++)
            {
                string RatName = names[i];
                raceManager.CreateRat(RatName);
            }

            
            foreach (var RatsInRats in raceManager.Rats)
            {
                rats.Add(new GetRats { Name = RatsInRats.Name, Type = RatsInRats.Type });
                RunnersTable.ItemsSource = rats;
                SelectRat.Items.Add(RatsInRats.Name);
            }

            string TrackName = "";
            int NumberTrack = RNG.Range(20, 51);
            int tracklength = NumberTrack;
            int whichTrack = RNG.Range(1, 4);
            switch (whichTrack)
            {
                case 1:
                    Console.WriteLine("The track for this race is a plain grass field! \nPlain rats will have an advantage");
                    TrackName = "Plain grass field";
                    break;
                case 2:
                    Console.WriteLine("The track for this race is a forrest! \nForrets rats will have an advantage");
                    TrackName = "Forrest";
                    break;
                case 3:
                    Console.WriteLine("The track for this race is a sand beach! \nBeach rats will have an advantage");
                    TrackName = "Beach";
                    break;
            }
            raceManager.CreateTrack(TrackName, tracklength);
            raceManager.CreateRace(raceManager.RaceID, raceManager.Rats, raceManager.Tracks[0]);
            
            tracks.Add(new NextRace { RaceNumber = raceManager.Races[0].RaceID, Tracki = raceManager.Tracks[0].Name, Rats = NumberRats });
            NextRaceTable.ItemsSource = tracks;

            coolItems.Add(new CoolItems { ItemName = "speedyshoes", Boost = "+2 spaces", Price = 200 });
            coolItems.Add(new CoolItems { ItemName = "Carrot", Boost = "+2 Headstart", Price = 300 });
            coolItems.Add(new CoolItems { ItemName = "Ultimate Dice", Boost = "Higher Rolls", Price = 500 });
            ItemsTable.ItemsSource = coolItems;
            foreach (var Itemss in coolItems)
            {
                SelectItem.Items.Add(Itemss.ItemName);
            }
            for (int i = 0; i < tracklength; i++)
            {
                RaceGrid.RowDefinitions.Add(new RowDefinition());
            }

            lanes.Add(Lane1);
            lanes.Add(Lane2);
            lanes.Add(Lane3);
            lanes.Add(Lane4);
            lanes.Add(Lane5);
            lanes.Add(Lane6);
            lanes.Add(Lane7);
            lanes.Add(Lane8);
            for (int i = 0; i < rats.Count; i++)
            {
                lanes[i].Text = rats[i].Name;
            }
            
        }

        public class GetRats
        {
            public string Name { get; set; }
            public string Type { get; set; }
        }

        public class CoolItems
        {
            public string ItemName { get; set; }
            public string Boost { get; set; }
            public int Price { get; set; }
        }

        public class NextRace
        {
            public int RaceNumber { get; set; }
            public string Tracki { get; set; }
            public int Rats { get; set; }
        }

        public class PlaceBets
        {
            public string WPlayer { get; set; }
            public string WRatName { get; set; }
            public int WAmount { get; set; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PlayerName = textbox_playername.Text;
            money = RNG.Range(100, 1000);
            raceManager.CreatePlayer(PlayerName, money);
            raceManager.CreatePlayer();
            textbox_playermoney.Text = money.ToString();
            ButtonInfo.IsEnabled = false;
            textbox_playername.IsEnabled = false;
            SubmitButton.IsEnabled = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            bool winnerFound = false;
            int crntTurn = 1;
            while (!winnerFound)
            {

                for (int i = 0; i < raceManager.Rats.Count; i++)
                {
                    raceManager.Rats[i].MoveRat(raceManager.Tracks[0]);
                    Grid.SetRow(lanes[i], raceManager.Rats[i].Position);
                }

                int bestPositioneringIHeleDasWelt = 0;

                foreach (Rat item in raceManager.Rats)
                {
                    if (item.Position >= raceManager.Tracks[0].TrackLength && item.Position > bestPositioneringIHeleDasWelt)
                    {
                        winnerFound = true;
                        bestPositioneringIHeleDasWelt = item.Position;
                        raceManager.Races[0].winner = item;
                        winnerrat.Content = item.Name;
                    }
                }
                crntTurn++;
            }
            raceManager.Races[0].GetWinner();
            if (raceManager.bookmaker.Bets.Count != 0)
            {
                raceManager.bookmaker.Bets[0].PayWinnings();
                textbox_playermoney.Text = raceManager.Players[0].Money.ToString();
            }
            ButtonStart.IsEnabled = false;
            ResetButton.Visibility = Visibility.Visible;
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            raceManager.bookmaker.Bets.Clear();
            Rat BettedRat = null;
            CoolItems items = null;
            
            int a = SelectRat.SelectedIndex;
            BettedRat = raceManager.Rats[a];
            int BettedAmount = int.Parse(BetAmount.Text);

            int b = SelectItem.SelectedIndex;
            try
            {
                items = coolItems[b];
            }
            catch (Exception)
            {
            }
            if (items != null)
            {
                switch (items.ItemName)
                {
                    case "speedyshoes":
                        speedshoes.Equip(BettedRat);
                        break;
                    case "Carrot":
                        carrot.Equip(BettedRat);
                        break;
                    case "Ultimate Dice":
                        ultimateDice.Equip(BettedRat);
                        break;
                }
                raceManager.Players[0].Money -= items.Price;
            }

            raceManager.Players[0].Money -= BettedAmount;
            raceManager.bookmaker.PlaceBet(raceManager.Races[0], BettedRat, raceManager.Players[0], BettedAmount);
            textbox_playermoney.Text = raceManager.Players[0].Money.ToString();

            List<PlaceBets> placeBets = new List<PlaceBets>();
            placeBets.Add(new PlaceBets { WPlayer = PlayerName, WRatName = BettedRat.Name, WAmount = BettedAmount });
            PlacedBetsTable.ItemsSource = placeBets;

        }

        private void textbox_playername_GotFocus(object sender, RoutedEventArgs e)
        {
            textbox_playername.Text = null;
        }

        private void BetAmount_GotFocus(object sender, RoutedEventArgs e)
        {
            BetAmount.Text = null;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < raceManager.Rats.Count; i++)
            {
                raceManager.Rats[i].ResetRat();
                Grid.SetRow(lanes[i], raceManager.Rats[i].Position);
            }
            raceManager.Tracks.Clear();

            raceManager.Rats.Clear();
            rats.Clear();
            SelectRat.Items.Clear();

            coolItems.Clear();
            SelectItem.Items.Clear();

            raceManager.bookmaker.Bets.Clear();
            tracks.Clear();
            RaceGrid.RowDefinitions.Clear();
            PlacedBetsTable.ItemsSource = null;

            int NumberRats = RNG.Range(2, 8);
            string[] names = { "Lui", "Palle", "Humus", "Ost", "Paladin", "Bard", "Kartoffel", "Ratatouille", "Flæskesteg", "BrunSovs", "Bluey", "Rory" };
            for (int i = 0; i <= NumberRats; i++)
            {
                string RatName = names[i];
                raceManager.CreateRat(RatName);
            }


            foreach (var RatsInRats in raceManager.Rats)
            {
                rats.Add(new GetRats { Name = RatsInRats.Name, Type = RatsInRats.Type });
                RunnersTable.ItemsSource = null;
                RunnersTable.ItemsSource = rats;
                SelectRat.Items.Add(RatsInRats.Name);
            }

            string TrackName = "";
            int NumberTrack = RNG.Range(20, 51);
            int tracklength = NumberTrack;
            int whichTrack = RNG.Range(1, 4);
            switch (whichTrack)
            {
                case 1:
                    Console.WriteLine("The track for this race is a plain grass field! \nPlain rats will have an advantage");
                    TrackName = "Plain grass field";
                    break;
                case 2:
                    Console.WriteLine("The track for this race is a forrest! \nForrets rats will have an advantage");
                    TrackName = "Forrest";
                    break;
                case 3:
                    Console.WriteLine("The track for this race is a sand beach! \nBeach rats will have an advantage");
                    TrackName = "Beach";
                    break;
            }
            raceManager.CreateTrack(TrackName, tracklength);
            raceManager.CreateRace(raceManager.RaceID, raceManager.Rats, raceManager.Tracks[0]);

            tracks.Add(new NextRace { RaceNumber = raceManager.Races[0].RaceID, Tracki = raceManager.Tracks[0].Name, Rats = NumberRats });
            NextRaceTable.ItemsSource = null;
            NextRaceTable.ItemsSource = tracks;

            coolItems.Add(new CoolItems { ItemName = "speedyshoes", Boost = "+2 spaces", Price = 200 });
            coolItems.Add(new CoolItems { ItemName = "Carrot", Boost = "+2 Headstart", Price = 300 });
            coolItems.Add(new CoolItems { ItemName = "Ultimate Dice", Boost = "Higher Rolls", Price = 500 });
            ItemsTable.ItemsSource = coolItems;
            foreach (var Itemss in coolItems)
            {
                SelectItem.Items.Add(Itemss.ItemName);
            }
            for (int i = 0; i < tracklength; i++)
            {
                RaceGrid.RowDefinitions.Add(new RowDefinition());
            }

            lanes.Add(Lane1);
            lanes.Add(Lane2);
            lanes.Add(Lane3);
            lanes.Add(Lane4);
            lanes.Add(Lane5);
            lanes.Add(Lane6);
            lanes.Add(Lane7);
            lanes.Add(Lane8);
            for (int i = 0; i < rats.Count; i++)
            {
                lanes[i].Text = rats[i].Name;
            }
            ResetButton.Visibility = Visibility.Hidden;
            ButtonStart.IsEnabled = true;
        }
    }
}
