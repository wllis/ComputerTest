﻿using QTest.instances;
using QTest.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace QTest.Views
{
    /// <summary>
    /// SensorView.xaml 的交互逻辑
    /// </summary>
    public partial class SensorView : UserControl
    {
        private CpuTemperatureReader cpuCelsius;
        List<SensorData> sensorList;
        ObservableCollection<SensorData> baseDataList = new ObservableCollection<SensorData>();
        Thread th;
        bool allow = true;

        public SensorView()
        {
            InitializeComponent();

            th = new Thread(WorkThread)
            {
                IsBackground = true
            };
            th.Start();
        }

        private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Console.WriteLine("OnLoaded......................");
            //loadingPanel.Visibility = Visibility.Collapsed;
        }

        private void WorkThread()
        {
            Console.WriteLine("start get cpu info ...");
            cpuCelsius = new CpuTemperatureReader();
            while (allow)
            {
                sensorList = cpuCelsius.GetTemperaturesInCelsius();
                if (sensorList != null)
                {
                    if (sensorListView.Items.Count == 0)
                    {
                        foreach (SensorData sensor in sensorList)
                        {
                            baseDataList.Add(sensor);
                        }
                        this.Dispatcher.Invoke((Action)delegate ()
                        {
                            sensorListView.ItemsSource = baseDataList;
                            loadingPanel.Visibility = Visibility.Collapsed;
                        });
                    }
                    else
                    {
                        for (int i = 0; i < sensorList.Count; i++)
                        {
                            this.Dispatcher.Invoke((Action)delegate ()
                            {
                                baseDataList[i] = sensorList[i];
                            });
                        }
                    }
                }
                Thread.Sleep(2000);
            }
        }

        private void OnUnloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (th != null)
            {
                try
                {
                    th.Abort();
                    allow = false;
                    cpuCelsius.Dispose();
                    Console.WriteLine("Sensor Exit!!!");
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        
    }
}