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

namespace StudentTracker
{
    /// <summary>
    /// window to add a factor to the students
    /// </summary>
    public partial class FactorWindow : Window
    {
        public string Key { get; set;}
        public string Value { get; set; }
        public FactorWindow(List<string> keys)
        {
            InitializeComponent();
            //open in the center of the screen
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            keysList.ItemsSource = keys;
        }

        private void close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void save(object sender, RoutedEventArgs e)
        {
            //check if the user has selected a key and entered a value
            if (keysList.SelectedItem == null || points.Text == "")
            {
                MessageBox.Show("Please select a key and enter number of points to factor");
                return;
            }
            DialogResult = true;
            Key = keysList.SelectedItem.ToString();
            //validate input is a number between 0-100
            if (int.TryParse(points.Text, out int result))
            {
                if (result >= 0 && result <= 100)
                {
                    Value = points.Text;
                }
                else
                {
                    MessageBox.Show("Please enter a number between 0-100");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Please enter a number between 0-100");
                return;
            }
            Value = points.Text;
        }

        private void factor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                if (!tb.Text.All(char.IsDigit))
                    tb.Text = "";
                else if (tb.Text == "")
                    return;
                else if (int.Parse(tb.Text) > 100)
                {
                    tb.Text = "100";
                }
            }
        }
    }
}
