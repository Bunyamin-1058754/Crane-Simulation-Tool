// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Wpf.Ui.Controls;
using Wxi.CraneSimulation.Core.Entities;
using Wxi.CraneSimulation.Core.Events.Args;
using Wxi.CraneSimulation.Core.Services;
using Wxi.CraneSimulation.Ui.ViewModels.Pages;

namespace Wxi.CraneSimulation.Ui.Views.Pages
{
    public partial class DataPage : INavigableView<DataViewModel>
    {
        private readonly HiveMQService _client;
        private readonly FirebaseDataLogService _logService;
        private string _solutiondir;

        List<string> spreaderPosition = new List<string>();
        List<string> containerPosition = new List<string>();
        List<DataLog> loggedData = new List<DataLog>();

        DispatcherTimer timer = new DispatcherTimer();
        public DataViewModel ViewModel { get; }

        public DataPage(DataViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();

            _client = new HiveMQService();
            _logService = new FirebaseDataLogService();

            elEmergencyButton.Visibility = Visibility.Hidden;
            elMovementButton.Visibility = Visibility.Hidden;

            _client.MessageReceived += PayloadMessage_MessageReceived;
        }
        private async void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (!await _client.ConnectToServer())
            {
                ShowMessageBox("Error", "Connection to server failed. \nTry again later.");
            }
            else
            {
                loggedData = new List<DataLog>();

                _client.StartListening();
                await _client.SubscribeToTopic("simulation/crane/image");
                await _client.SubscribeToTopic("simulation/crane/coordinates");
                await _client.SubscribeToTopic("simulation/container/coordinates");
                await _client.SubscribeToTopic("simulation/paused");

                lblStatus.Content = "Connected";
                elStatusButton.Fill = Brushes.Green;

                SwitchButtons();
            }
        }

        private async void BtnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            if (!await _client.DisconnectFromServer())
            {
                ShowMessageBox("Error", "Something went wrong. \nTry again later.");
            }
            else
            {
                _client.StopListening();
                lblStatus.Content = "Disconnected";
                elStatusButton.Fill = Brushes.Red;
                elMovementButton.Visibility = Visibility.Hidden;
                SwitchButtons();
                SaveToDatabase(loggedData);
            }
        }

        public async void SaveToDatabase(List<DataLog> messages)
        {
            foreach (var message in messages)
            {
                await _logService.CreateAsync(message);
            }
        }

        private async void PayloadMessage_MessageReceived(object sender, MessageReceivedArgs e)
        {
            await this.Dispatcher.InvokeAsync(async () =>
            {
                if (e.Topic == "simulation/crane/image")
                {
                    DisplayImages(e.Message);
                }
                else
                {

                    if (e.Message == "pause")
                    {
                        if (elEmergencyButton.Visibility == Visibility.Visible)
                        {
                            elEmergencyButton.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            elEmergencyButton.Visibility = Visibility.Visible;
                        }
                    }
                    string[] splitMessage = e.Message.Split(",");
                    var data = new DataLog()
                    {
                        Id = Guid.NewGuid(),
                        IsError = false,
                        Date = DateTime.Now,
                        X = splitMessage[0],
                        Y = splitMessage[1]

                    };

                    if (e.Topic == "simulation/crane/coordinates")
                    {
                        data.Name = "Crane";
                        lstCranePosition.Items.Clear();
                        spreaderPosition.Add(data.ToString());
                    }
                    else if (e.Topic == "simulation/container/coordinates")
                    {
                        data.Name = "Container";
                        lstContainerPosition.Items.Clear();
                        containerPosition.Add(data.ToString());
                    }

                    elMovementButton.Visibility = Visibility.Visible;
                    loggedData.Add(data);
                    ReverseLists();
                    StartTimer();
                }
            });
        }

        public void StartTimer()
        {
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Start();
        }

        public void ReverseLists()
        {
            var reverseListCrane = new List<string>(spreaderPosition);
            var reverseListContainer = new List<string>(containerPosition);

            reverseListContainer.Reverse();
            reverseListCrane.Reverse();

            foreach (var item in reverseListCrane)
            {
                lstCranePosition.Items.Add(item);
            }
            foreach (var item in reverseListContainer)
            {
                lstContainerPosition.Items.Add(item);
            }
        }

        private void DisplayImages(string image)
        {
            _solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            _solutiondir = Directory.GetParent(_solutiondir).Parent.FullName;
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri($"{_solutiondir}\\Wxi.CraneSimulator.PyCore\\PythonImages\\image_{image}.png", UriKind.RelativeOrAbsolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();

            python.Source = bitmap;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            elMovementButton.Visibility = Visibility.Hidden;
            timer.Stop();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var messages = await _logService.GetAllAsync();

            spreaderPosition = messages.Where(m => m.Name == "Crane").Select(m => m.ToString()).TakeLast(10).ToList();
            containerPosition = messages.Where(m => m.Name == "Container").Select(m => m.ToString()).TakeLast(10).ToList();

            ReverseLists();
        }

        private async void ShowMessageBox(string title, string message)
        {
            var messageBox = new Wpf.Ui.Controls.MessageBox
            {
                Title = title,
                Content = message,
                CloseButtonText = "Ok",
                ShowTitle = true,
            };

            await messageBox.ShowDialogAsync();
        }

        private void SwitchButtons()
        {
            btnConnect.IsEnabled = !btnConnect.IsEnabled;
            btnDisconnect.IsEnabled = !btnDisconnect.IsEnabled;
        }
    }
}
