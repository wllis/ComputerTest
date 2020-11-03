﻿using QTest.GPIO;
using QTest.Tools;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace QTest.Views
{
    /// <summary>
    /// GPIOView.xaml 的交互逻辑
    /// </summary>
    public partial class GPIOView : UserControl
    {
        public enum TypeEnum
        {
            Q300P = 0,
            Q500G6,
            Q500X,
            Q600P
        }
        private bool dirverStatus = false;
        private SuperIO gpio = null;

        public GPIOView()
        {
            InitializeComponent();
        }

        private void GPIO_Loaded(object sender, RoutedEventArgs e)
        {
            combobox_type.ItemsSource = System.Enum.GetNames(typeof(TypeEnum));
            string[] modelDatas = { "0", "1" };
            model_box.ItemsSource = modelDatas;
            val_box.ItemsSource = modelDatas;
        }

        private bool InitGPIODriver()
        {
            bool initResult = gpio.Initialize();
            if (!initResult)
            {
                driver_status.Text = "驱动加载失败!";
                driver_status.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                driver_status.Text = "驱动加载完成!";
                gpio.InitSuperIO();
                chip_name.Text = "芯片型号：ITE " + gpio.GetChipName();
                gpio.ExitSuperIo();
            }
            return initResult;
        }

        private void Minipc_type_DropDownClosed(object sender, EventArgs e)
        {
            Console.WriteLine("Minipc_type_DropDownClosed........." + combobox_type.Text);
            if(combobox_type.Text.Length == 0)
            {
                return;
            }
            if (gpio == null)
            {
                gpio = new SuperIO();
                dirverStatus = InitGPIODriver();
            }
            gpio.MinipcType = combobox_type.Text;
            if(!dirverStatus)
            {
                return;
            }
            else
            {
                model_btn.IsEnabled = true;
                val_btn.IsEnabled = true;
                TypeEnum type = (TypeEnum)Enum.Parse(typeof(TypeEnum),
                    combobox_type.SelectedItem.ToString(), false);
                gpio.InitSuperIO();
                gpio.InitLogicDevice();
                LoadGpioData(type);
                gpio.ExitSuperIo();
            }
        }

        private void LoadGpioData(TypeEnum type)
        {
            switch (type)
            {
                case TypeEnum.Q300P:
                    Console.WriteLine("Q300P...............");
                    gpio.SetGpioFunction(0x2c, 0x89);
                    LoadGpioModel(TypeEnum.Q300P);
                    LoadGpioValue(TypeEnum.Q300P);
                    break;
                case TypeEnum.Q500G6:
                    Console.WriteLine("Q500G6...............");
                    break;
                case TypeEnum.Q500X:
                    Console.WriteLine("Q500X...............");
                    break;
                case TypeEnum.Q600P:
                    Console.WriteLine("Q600P...............");
                    LoadGpioModel(TypeEnum.Q600P);
                    LoadGpioValue(TypeEnum.Q600P);

                    break;
                default:
                    break;
            }
        }

        private void LoadGpioModel(TypeEnum type)
        {
            switch (type)
            {
                case TypeEnum.Q300P:
                    char[] models_q300p = Q300P.ReadGpioModel(gpio);
                    FormatGpioModel(models_q300p);
                    break;
                case TypeEnum.Q500G6:
                    break;
                case TypeEnum.Q500X:
                    break;
                case TypeEnum.Q600P:
                    char[] models_q600p = Q600P.ReadGpioModel(gpio);
                    FormatGpioModel(models_q600p);

                    break;
                default:
                    break;
            }
        }

        private void LoadGpioValue(TypeEnum type)
        {
            switch (type)
            {
                case TypeEnum.Q300P:
                    char[] val_q300p = Q300P.ReadGpioValues(gpio);
                    FormatGpioValue(val_q300p);
                    break;
                case TypeEnum.Q500G6:
                    break;
                case TypeEnum.Q500X:
                    break;
                case TypeEnum.Q600P:
                    char[] val_q600p = Q600P.ReadGpioValues(gpio);
                    FormatGpioValue(val_q600p);
                    break;
                default:
                    break;
            }
        }

        private void Model_btn_Click(object sender, RoutedEventArgs e)
        {
            TypeEnum type = (TypeEnum)Enum.Parse(typeof(TypeEnum),
                    combobox_type.SelectedItem.ToString(), false);
            string[] arr = { gpio1_m.Text, gpio2_m.Text, gpio3_m.Text, gpio4_m.Text,
                gpio5_m.Text, gpio6_m.Text, gpio7_m.Text, gpio8_m.Text};

            Console.WriteLine("GPIO Model click:{0}", string.Join("", arr));
            gpio.InitSuperIO();
            switch (type)
            {
                case TypeEnum.Q300P:
                    Q300P.SetGpioModels(gpio, arr);
                    LoadGpioModel(TypeEnum.Q300P);
                    LoadGpioValue(TypeEnum.Q300P);
                    gpio.ExitSuperIo();
                    break;
                case TypeEnum.Q500G6:
                    break;
                case TypeEnum.Q500X:
                    break;
                case TypeEnum.Q600P:
                    Q600P.SetGpioModels(gpio, arr);

                    LoadGpioModel(TypeEnum.Q600P);
                    LoadGpioValue(TypeEnum.Q600P);

                    gpio.ExitSuperIo();
                    break;
                default:
                    break;
            }
        }

        private void Val_btn_Click(object sender, RoutedEventArgs e)
        {
            TypeEnum type = (TypeEnum)Enum.Parse(typeof(TypeEnum),
                    combobox_type.SelectedItem.ToString(), false);
            string[] arr = { gpio1_v.Text, gpio2_v.Text, gpio3_v.Text, gpio4_v.Text,
                gpio5_v.Text, gpio6_v.Text, gpio7_v.Text, gpio8_v.Text};

            Console.WriteLine("GPIO Value click:{0}", string.Join("", arr));
            gpio.InitSuperIO();
            switch (type)
            {
                case TypeEnum.Q300P:
                    Q300P.SetGpioValues(gpio, arr);
                    LoadGpioModel(TypeEnum.Q300P);
                    LoadGpioValue(TypeEnum.Q300P);
                    gpio.ExitSuperIo();
                    break;
                case TypeEnum.Q500G6:
                    break;
                case TypeEnum.Q500X:
                    break;
                case TypeEnum.Q600P:
                    Q600P.SetGpioValues(gpio, arr);

                    LoadGpioModel(TypeEnum.Q600P);
                    LoadGpioValue(TypeEnum.Q600P);

                    gpio.ExitSuperIo();
                    break;
                default:
                    break;
            }
        }

        private void FormatGpioModel(char[] arr)
        {
            gpio1_m.Text = arr[0].ToString();
            gpio2_m.Text = arr[1].ToString();
            gpio3_m.Text = arr[2].ToString();
            gpio4_m.Text = arr[3].ToString();
            gpio5_m.Text = arr[4].ToString();
            gpio6_m.Text = arr[5].ToString();
            gpio7_m.Text = arr[6].ToString();
            gpio8_m.Text = arr[7].ToString();
        }

        private void FormatGpioValue(char[] arr)
        {
            gpio1_v.Text = arr[0].ToString();
            gpio2_v.Text = arr[1].ToString();
            gpio3_v.Text = arr[2].ToString();
            gpio4_v.Text = arr[3].ToString();
            gpio5_v.Text = arr[4].ToString();
            gpio6_v.Text = arr[5].ToString();
            gpio7_v.Text = arr[6].ToString();
            gpio8_v.Text = arr[7].ToString();
        }

        private void Gpio_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-1]$");
            e.Handled = re.IsMatch(e.Text);
        }

        private void GPIO_Unloaded(object sender, RoutedEventArgs e)
        {
            if(gpio != null)
            {
                gpio.SysDispose();
            }
        }

        private void Model_box_DropDownClosed(object sender, EventArgs e)
        {
            Console.WriteLine(model_box.Text);
            if(model_box.Text.Length == 0)
            {
                return;
            }
            
            gpio1_m.Text = model_box.Text;
            gpio2_m.Text = model_box.Text;
            gpio3_m.Text = model_box.Text;
            gpio4_m.Text = model_box.Text;
            gpio5_m.Text = model_box.Text;
            gpio6_m.Text = model_box.Text;
            gpio7_m.Text = model_box.Text;
            gpio8_m.Text = model_box.Text;
        }

        private void Val_box_DropDownClosed(object sender, EventArgs e)
        {
            Console.WriteLine(val_box.Text);
            if (val_box.Text.Length == 0)
            {
                return;
            }
            gpio1_v.Text = val_box.Text;
            gpio2_v.Text = val_box.Text;
            gpio3_v.Text = val_box.Text;
            gpio4_v.Text = val_box.Text;
            gpio5_v.Text = val_box.Text;
            gpio6_v.Text = val_box.Text;
            gpio7_v.Text = val_box.Text;
            gpio8_v.Text = val_box.Text;
        }
    }
}
