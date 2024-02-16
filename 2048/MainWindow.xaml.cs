using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _2048
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int[,] arr = new int[4, 4];
        Random rand = new();
        int score = 0, high_score = 0;
        MediaPlayer player = new();
        
        public MainWindow()
        {
            InitializeComponent();
            CreateBadges();
            LoadHighScore();
            DisplayBadges();
            high_score_label.Content = high_score;
            player.Open(new Uri("Herknungr.mp3", UriKind.Relative));
            player.MediaEnded += Player_MediaEnded;
            player.Play();
        }

        private void Player_MediaEnded(object? sender, EventArgs e)
        {
            player.Position = TimeSpan.Zero;
            player.Play();
        }

        private void LoadHighScore()
        {
            if (File.Exists("high_score.json"))
            {
                using (StreamReader sr = new("high_score.json"))
                {
                    string str = sr.ReadToEnd();
                    high_score = JsonSerializer.Deserialize<int>(str);
                }
            }
            else
                high_score = 0;
        }

        private void CreateBadges()
        {
            try
            {
                for (int i = 0; i < arr.GetLength(0); i++)
                {
                    for (int j = 0; j < arr.GetLength(1); j++)
                    {
                        arr[i, j] = 0;
                    }
                }

                int badges = rand.Next(2, 5); //количество начальных фишек
                while (badges != 0)
                {
                    int x = new Random().Next(0, 4);
                    int y = new Random().Next(0, 4);
                    if (arr[x, y] == 0)
                    {
                        int z = new Random().Next(1, 11);
                        arr[x, y] = (z == 10) ? 4 : 2;
                        badges--;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void NewBadge()
        {
            bool empty_cell = false;

            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    if (arr[i, j] == 0)
                        empty_cell = true;
                }
            }
            if (empty_cell)
            {
                int badges = 1; //новая фишка после хода
                while (badges != 0)
                {
                    int x = new Random().Next(0, 4);
                    int y = new Random().Next(0, 4);
                    if (arr[x, y] == 0)
                    {
                        int z = new Random().Next(1, 11);
                        arr[x, y] = (z == 10) ? 4 : 2;
                        badges--;
                    }
                }
            }
            else
            {
                if (score > high_score)
                {
                    high_score = score;
                    high_score_label.Content = high_score;
                    MessageBox.Show($"You loose! Your score is  {score}  , it`s a new record");
                    score = 0;
                    score_label.Content = 0;
                    using (StreamWriter sw = new("high_score.json"))
                    {
                        string str = JsonSerializer.Serialize(high_score);
                        sw.WriteLine(str);
                    }
                }
                else
                    MessageBox.Show($"You loose! Your score is{score}");
                LoadHighScore();
                CreateBadges();
            }
        }

        private void DisplayBadges()
        {
            string name = "";
            foreach (UIElement el in main_grid.Children)
            {
                if (el is Border border)
                {
                    var label = border.Child as Label;
                    for (int i = 0; i < arr.GetLength(0); i++)
                    {
                        for (int j = 0; j < arr.GetLength(1); j++)
                        {
                            name = $"rc{i}{j}";
                            if (border.Name == name && arr[i, j] != 0)
                            {
                                label.Content = arr[i, j];
                                string key = label.Content.ToString() + "but";
                                Style style = TryFindResource(key) as Style;
                                if (style != null)
                                {
                                    border.Style = style;
                                }
                            }
                            if (border.Name == name && arr[i, j] == 0)
                            {
                                label.Content = arr[i, j];
                                Style style = TryFindResource("InnerButton") as Style;
                                if (style != null)
                                {
                                    border.Style = style;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Score()
        {
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                    score += arr[i, j];
            }
            score_label.Content = score;
        }

        private void Win()
        {
            
            if (score > high_score)
            {
                high_score = score;
                high_score_label.Content = high_score;
                MessageBox.Show($"You win! Well done! It`s a new record {score}");
                using (StreamWriter sw = new("high_score.json"))
                {
                    string str = JsonSerializer.Serialize(score);
                    sw.WriteLine(str);
                }
            }
            else
                MessageBox.Show($"You win! Your score is {score}");
            score = 0;
            score_label.Content = 0;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            player.Stop();
        }

        private void make_move(object sender, KeyEventArgs e)
        {
            try
            {
                bool ismove = false;
                switch (e.Key)
                {
                    case Key.Left:
                        {
                            for (int i = 0; i < arr.GetLength(0); i++)
                            {
                                for (int j = 1; j < arr.GetLength(1); j++)
                                {
                                    int z = j;
                                    while (arr[i, z - 1] == 0 && arr[i, z] != 0)
                                    {
                                        arr[i, z - 1] = arr[i, z];
                                        arr[i, z] = 0;
                                        if (z > 1)
                                            z--;
                                        ismove = true;
                                    }
                                    while (arr[i, z - 1] == arr[i, z] && arr[i,z] != 0)
                                    {
                                        
                                        arr[i, z - 1] *= 2;
                                        if (arr[i, z - 1] == 2048)
                                            Win();
                                        arr[i, z] = 0;
                                        if (z > 1)
                                            z--;
                                        ismove = true;
                                    }
                                }
                            }
                        } break;
                    case Key.Right:
                        {
                            for (int i = 0; i < arr.GetLength(0); i++)
                            {
                                for (int j = arr.GetLength(1) - 2; j >= 0; j--)
                                {
                                    int z = j;
                                    while (arr[i, z + 1] == 0 && arr[i, z] != 0)
                                    {
                                        arr[i, z + 1] = arr[i, z];
                                        arr[i, z] = 0;
                                        if (z < arr.GetLength(1) - 2)
                                            z++;
                                        ismove = true;
                                    }
                                    while (arr[i, z + 1] == arr[i, z] && arr[i, z] != 0)
                                    {

                                        arr[i, z + 1] *= 2;
                                        if (arr[i, z + 1] == 2048)
                                            Win();
                                        arr[i, z] = 0;
                                        if (z < arr.GetLength(1) - 2)
                                            z++;
                                        ismove = true;
                                    }
                                }
                            }
                        }
                        break;
                    case Key.Up:
                        {
                            for (int j = 0; j < arr.GetLength(1); j++)
                            {
                                for (int i = 1; i < arr.GetLength(0); i++)
                                {
                                    int z = i;
                                    while (arr[z - 1, j] == 0 && arr[z,j] != 0)
                                    {
                                        arr[z - 1, j] = arr[z, j];
                                        arr[z, j] = 0;
                                        if (z > 1)
                                            z--;
                                        ismove = true;
                                    }
                                    while (arr[z-1,j] == arr[z,j] && arr[z,j] != 0)
                                    {
                                        arr[z - 1, j] *= 2;
                                        if (arr[z - 1, j] == 2048)
                                            Win();
                                        arr[z, j] = 0;
                                        if (z > 1)
                                            z--;
                                        ismove = true;
                                    }
                                }
                            }
                        }break;
                    case Key.Down: 
                        {
                            for (int j = 0; j < arr.GetLength(1); j++)
                            {
                                for (int i = arr.GetLength(0) - 2; i >= 0; i--)
                                {
                                    int z = i;
                                    while (arr[z + 1, j] == 0 && arr[z, j] != 0)
                                    {
                                        arr[z + 1, j] = arr[z, j];
                                        arr[z, j] = 0;
                                        if (z < arr.GetLength(0) - 2)
                                            z++;
                                        ismove = true;
                                    }
                                    while (arr[z + 1, j] == arr[z, j] && arr[z, j] != 0)
                                    {
                                        arr[z + 1, j] *= 2;
                                        if (arr[z + 1, j] == 2048)
                                            Win();
                                        arr[z, j] = 0;
                                        if (z < arr.GetLength(0) - 2)
                                            z++;
                                        ismove = true;
                                    }
                                }
                            }
                        }
                    break;
                }
                Score();
                NewBadge();
                DisplayBadges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}