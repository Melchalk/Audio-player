using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Аудиоплеер
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            File.Create(part).Close();
            File.Create(partForGirls).Close();
        }

        MediaPlayer player = new();
        Dictionary<string, string> musicDictionary = new();
        string lastSong;
        readonly string stop = "Stop", start = "Play";

        string partForGirls = @$"C:\Users\{Environment.UserName}\Downloads\MyMusic.txt";
        readonly string part = @$"C:\Users\{Environment.UserName}\Downloads\MyMusic.json";

        private void PlayOrStop(object sender, RoutedEventArgs e)
        {
            var func = (string)mainButton.Content;
            if (lastSong is null || ListOfMusic.Items.Count == 0) return;

            if (func == start)
                player.Play();
            else
            {
                Value.Value = player.Position.TotalSeconds;
                player.Pause();
            }
                
            mainButton.Content = func == start ? stop : start;

        }

        private void AddMusic(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                FileName = "MyMusic",
                DefaultExt = ".mp3",
                Filter = "Audio (.mp3)|*.mp3",
                InitialDirectory = @$"C:\Users\{Environment.UserName}\Downloads",
                Multiselect = true
            };

            if (dialog.ShowDialog() is false) return;

            if (dialog.FileNames.Length == 1)
            {
                string file = dialog.FileName;
                file = file[(file.LastIndexOf('\\') + 1)..];
                EditOrAdd add = new() { LastName = file };
                if (add.ShowDialog() == true && add.NameOfSong is not null)
                    file = add.NameOfSong;

                file = CurrectName(file, true);
                musicDictionary[file] = dialog.FileName;
                ListOfMusic.Items.Add(file);
            }
            else
            {
                foreach (string file in dialog.FileNames)
                {
                    var fileCopy = CurrectName(file[(file.LastIndexOf('\\') + 1)..], true);
                    musicDictionary[fileCopy] = file;
                    ListOfMusic.Items.Add(fileCopy);
                }
            }

        }

        private void NewPlay(object sender, SelectionChangedEventArgs e)
        {
            string? selectSong = (string?)ListOfMusic.SelectedItem;
            if (selectSong is null) return;

            if (lastSong != selectSong)
            {
                player.Open(new Uri(musicDictionary[selectSong]));
                player.Play();
                Thread.Sleep(500);
                NewMax();
                mainButton.Content = stop;
                lastSong = selectSong;
            }
            else
                mainButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private void NewValue(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.Pause();
            player.Position = TimeSpan.FromSeconds(Value.Value);
            Thread.Sleep(200);
            player.Play();
        }

        void NewMax() 
        => Value.Maximum = player.NaturalDuration.TimeSpan.TotalSeconds;
        

        private void NewVolume(object sender, RoutedPropertyChangedEventArgs<double> e)
        => player.Volume = Volume.Value;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            /*
            var newJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(part));
            if (newJson == null) return;
            musicDictionary = newJson;
            */
            string[] songs = File.ReadAllLines(partForGirls);
            for(int index = 0; index < songs.Length; index+=2)
                musicDictionary[songs[index]] = songs[index+1];
            
            foreach (var song in musicDictionary.Keys)
            {
                ListOfMusic.Items.Add(song);
            }
        }

        private void EditSong(object sender, RoutedEventArgs e)
        {
            if (ListOfMusic.SelectedItem is null)
                return;

            EditOrAdd edit = new() { LastName = (string)ListOfMusic.SelectedItem };
            string file = musicDictionary[(string)ListOfMusic.SelectedItem];
            if (edit.ShowDialog() == false) return;

            string? newName = edit.NameOfSong;

            if (newName is not null)
            {
                newName = CurrectName(newName, false);
                musicDictionary[newName] = file;
                ListOfMusic.Items[ListOfMusic.SelectedIndex] = newName;
            }
        }

        private void DelSong(object sender, RoutedEventArgs e)
        {
            if (ListOfMusic.SelectedItem is null)
                return;
            musicDictionary.Remove((string)ListOfMusic.SelectedItem);
            ListOfMusic.Items.Remove(ListOfMusic.SelectedItem);
            player.Close();
            mainButton.Content = start;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            File.WriteAllText(part, JsonConvert.SerializeObject(musicDictionary));

            string inFile = "";
            foreach(var par in  musicDictionary)
            {
                inFile += $"{par.Key}\n";
                inFile += $"{par.Value}\n";
            }
            File.WriteAllText(partForGirls, inFile);
        }

        string CurrectName(string file, bool isNew)
        {
            if (!isNew)
                musicDictionary.Remove(file);
            
            int n = 1;
            while (musicDictionary.ContainsKey(file))
            {
                if (!musicDictionary.ContainsKey(file + n.ToString()))
                {
                    file += n.ToString();
                    break;
                }
                n++;
            }
            return file;
        }
    }
}
