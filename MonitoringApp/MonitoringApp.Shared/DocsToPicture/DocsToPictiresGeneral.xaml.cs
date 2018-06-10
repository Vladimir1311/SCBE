﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MonitoringApp.Shared.DocsToPicture
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DocsToPictiresGeneral : Page
    {

        private readonly DocsToPictureConnector connector;

        public List<string> values = new List<string>()
        {
            "One record",
            "Two record",
            "Other data"
        };
        public DocsToPictiresGeneral()
        {
            this.InitializeComponent();
            connector = new DocsToPictureConnector();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            logsBlock.Text = await connector.GetSome();
        }
    }
}
