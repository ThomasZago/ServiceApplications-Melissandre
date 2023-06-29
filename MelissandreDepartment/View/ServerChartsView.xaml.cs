﻿using System;
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

namespace MelissandreDepartment.View
{
    /// <summary>
    /// Logique d'interaction pour ServerChartsView.xaml
    /// </summary>
    public partial class ServerChartsView : Page
    {
        private static ServerChartsView _instance;
        private static readonly object _lockObject = new object();

            private ServerChartsView()
            {
                InitializeComponent();
                this.webBrowser.Source=new Uri("http://localhost:3000/login");
            }

    public static ServerChartsView Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObject)
                    {
                        if (_instance == null)
                        {
                            _instance = new ServerChartsView();
                        }
                    }
                }
                return _instance;
            }
        }
    }
}
