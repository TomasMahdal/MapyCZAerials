﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace MapAerials
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel viewModel;
        public MainWindow()
        {
            // set language
            Languages.SetLanguageDictionary();

            // verify if is not another instance running
            if (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1) {
                MessageBox.Show(Languages.GetString("dialog_anotherInstance"), "MapAerials", MessageBoxButton.OK, MessageBoxImage.Warning);
                Environment.Exit(0);
            }

            // init window
            InitializeComponent();

            // init viewmodel
            viewModel = new MainViewModel(this);
            DataContext = viewModel;

            // select first map type (so combobox selected item won't be null)
            comboMapType.SelectedIndex = 0;
        }

        /// <summary>
        /// open hyperlink in user's default web browser
        /// </summary>
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            viewModel.StartServer();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            viewModel.StopServer();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            viewModel.CopyURL();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
    }
}
