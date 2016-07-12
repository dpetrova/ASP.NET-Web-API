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

namespace GraphicalInterface
{
    using System.Net.Http;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string GetAllPostsEndpoint = "http://localhost:6459/api/categories";

        public MainWindow()
        {
            InitializeComponent();

            this.button.Click += button_Click;
        }

        //synchronous
        //private void button_Click(object sender, RoutedEventArgs e)
        //{
        //    var httpClient = new HttpClient();
        //    var response = httpClient.GetAsync(GetAllPostsEndpoint).Result;
        //    MessageBox.Show(response.ToString());
        //}


        //asynchronous
        //private async void button_Click(object sender, RoutedEventArgs e)
        //{
        //    var httpClient = new HttpClient();
        //    var response = await httpClient.GetAsync(GetAllPostsEndpoint);
        //    MessageBox.Show(response.ToString());
        //}

        //better way:
        private void button_Click(object sender, RoutedEventArgs e)
        {
            var task = this.PrintPosts();
        }

        private async Task PrintPosts()
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(GetAllPostsEndpoint);
            MessageBox.Show(response.ToString());
        }
    }
}
