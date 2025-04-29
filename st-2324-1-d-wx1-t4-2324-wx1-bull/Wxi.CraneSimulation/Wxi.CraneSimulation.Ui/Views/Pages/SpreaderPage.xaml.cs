using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Wpf;
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
using System.Windows.Threading;
using Wpf.Ui.Controls;
using Wxi.CraneSimulation.Core.Entities;
using Wxi.CraneSimulation.Core.Events.Args;
using Wxi.CraneSimulation.Core.Services;
using Wxi.CraneSimulation.Ui.ViewModels.Pages;

namespace Wxi.CraneSimulation.Ui.Views.Pages
{
    /// <summary>
    /// Interaction logic for SpeedPage.xaml
    /// </summary>
    public partial class SpreaderPage : INavigableView<DataViewModel>
    {
        public DataViewModel ViewModel { get; }
        private readonly DispatcherTimer timer;
        private readonly LineSeries lineSeries;
        private readonly HiveMQService _client;
        private int counter = 0;

        private string[] splitMessage = new string[] { "0", "355" };
        public SpreaderPage(DataViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            _client = new HiveMQService();
            _client.MessageReceived += PayloadMessage_MessageReceived;
            _client.ConnectToServer().ConfigureAwait(true);


            InitializeComponent();

            lineSeries = new LineSeries
            {
                Title = "Spreader Height",
                MarkerType = MarkerType.None,
                MarkerSize = 4,
                MarkerStroke = OxyColors.White,
                MarkerFill = OxyColors.Green,
                Color = OxyColors.Yellow,
            };



            var plotModel = new PlotModel { Title = "Spreader Height Graph", TitleColor = OxyColors.White, TextColor = OxyColors.White, PlotAreaBorderColor = OxyColor.Parse("#535353"), SubtitleColor = OxyColors.LightGray };
            plotModel.Series.Add(lineSeries);

            plotView.Model = plotModel;

            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };

            timer.Tick += Timer_Tick;

            timer.Start();
        }

        private async void PayloadMessage_MessageReceived(object sender, MessageReceivedArgs e)
        {
            await this.Dispatcher.InvokeAsync(async () =>
            {
                splitMessage = e.Message.Split(",");
            });
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            double newY = double.Parse(splitMessage[1]);

            lineSeries.Points.Add(new DataPoint(counter, newY));

            counter++;
            while (lineSeries.Points.Count > 150)
            {
                lineSeries.Points.RemoveAt(0);
            }

            for (int i = 0; i < lineSeries.Points.Count; i++)
            {
                lineSeries.Points[i] = new DataPoint(i, lineSeries.Points[i].Y);
            }


            plotView.Model.Axes[0].Maximum = 150;
            plotView.Model.Axes[1].Maximum = 375;
            plotView.Model.Axes[1].Minimum = 0;


            plotView.InvalidatePlot(true);
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _client.StartListening();
            await _client.SubscribeToTopic("simulation/crane/coordinates");
        }

    }
}
