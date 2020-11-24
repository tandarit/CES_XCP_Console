using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml.Serialization;
using CES_XCP_Console.View.Pages;
using Microsoft.Win32;
using CES_XCP_Console.Model;
using CES_XCP_Console.ViewModel;

namespace CES_XCP_Console
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ContentFrame.Navigate(new HomePage());
        }
        private void ButtonFechar_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            int index = ListViewMenu.SelectedIndex;
            //MoveCursorMenu(index);

            switch (index)
            {
                case 0:
                    ContentFrame.Navigate(new HomePage());
                    break;
                case 1:
                    ContentFrame.Navigate(new SettingPage());
                    break;
                case 2:
                    //ContentFrame.Navigate(new SettingPage());
                    break;
                case 3:
                    //ContentFrame.Navigate(new SettingPage());
                    break;
                case 4:
                    //ContentFrame.Navigate(new OwnDatabasePage());
                    break;
                case 5:
                    ContentFrame.Navigate(new OwnDatabasePage());
                    break;
                default:
                    break;
            }
            
        }

        private void MoveCursorMenu(int index)
        {
            //TrainsitionigContentSlide.OnApplyTemplate();
            //GridCursor.Margin = new Thickness(0, (100 + (60 * index)), 0, 0);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\CES_XCP_Console");
            XCPEnviroment locEnv;

            if (key!=null)
            {
                String configPath = key.GetValue("ConfigFilePath").ToString();
                locEnv = LoadEnviromentSettings(configPath);
                if(locEnv!=null)
                {
                    MainWindowVM.sXcpEnv = locEnv;
                }
            }
        }

        private XCPEnviroment LoadEnviromentSettings(string fileName)
        {
            XCPEnviroment xcpEnv;
            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                XmlSerializer xml = new XmlSerializer(typeof(XCPEnviroment));
                xcpEnv = (XCPEnviroment)xml.Deserialize(stream);
                stream.Close();
            }
            return xcpEnv;
        }
    }
}
