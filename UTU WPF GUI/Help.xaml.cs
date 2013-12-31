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
using System.Windows.Shapes;

namespace Info
{
    /// <summary>
    /// Interakční logika pro Nápověda.xaml
    /// </summary>
    public partial class Help : Window
    {
        public Help()
        {
            InitializeComponent();
            //mediaElement.Source = new Uri("c:\\Users\\Cendrb\\Music\\Linkin Park - Living Things\\01 - Linkin Park - Lost In The Echo.mp3");
            //mediaElement.LoadedBehavior = MediaState.Play;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
