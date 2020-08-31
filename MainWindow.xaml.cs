using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageCutter
{
    public class MainWindow : Window
    {
        Panel _pagesPanel = null;
        ImageSlicer _nineGridSlicer = null;
        StateSlicer _stateSlicer = null;

        Window _thisWindow = null;

        public MainWindow()
        {
            InitializeComponent();
        }
        
        string _cmdPath = null;
        bool _hasCmdPath = false;
        
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            _pagesPanel = this.Find<Panel>("PagesPanel");
            _nineGridSlicer = this.Find<ImageSlicer>("NineGrid");
            _stateSlicer = this.Find<StateSlicer>("StateSlicer");
            _thisWindow = this;

            foreach (string s in Environment.GetCommandLineArgs())
            {
                if (File.Exists(s) && (s.EndsWith(".png", StringComparison.OrdinalIgnoreCase)))
                {
                    _cmdPath = s;
                    _hasCmdPath = true;
                    break;
                }
            }

            SetActivePage(0);
        }

        public async void SliceIntoNineGridButton_Click(object sender, RoutedEventArgs e)
        {
            string path = await BrowseForOrGetImageFromCmd();
            if (path != null)
            {
                _nineGridSlicer.Source = new Bitmap(path);
                SetActivePage(1);
            }
        }

        public async void SliceIntoStatesButton_Click(object sender, RoutedEventArgs e)
        {
            string path = await BrowseForOrGetImageFromCmd();
            if (path != null)
            {
                _stateSlicer.Source = new Bitmap(path);
                SetActivePage(2);
            }
        }

        public async Task<string> BrowseForOrGetImageFromCmd()
        {
            if (_hasCmdPath)
                return _cmdPath;
            else
            {
                OpenFileDialog dialog = new OpenFileDialog()
                {
                    Directory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
                };

                string[] paths = await dialog.ShowAsync(this);
                if (paths.Length > 0)
                {
                    return paths[0];
                }
                else
                    return null;
            }
        }

        public async void SliceIntoNineGridSaveButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog dialog = new OpenFolderDialog();

            string path = await dialog.ShowAsync(this);
            if ((!string.IsNullOrEmpty(path)) && (!string.IsNullOrWhiteSpace(path)))
            {
                _nineGridSlicer.SaveSlicesToFiles(path, "NineGridSlicedImage");
            }
        }

        public async void SliceIntoStatesSaveButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog dialog = new OpenFolderDialog();

            string path = await dialog.ShowAsync(this);
            if ((!string.IsNullOrEmpty(path)) && (!string.IsNullOrWhiteSpace(path)))
            {
                _stateSlicer.SaveStatesToFiles(path, "StateSlicedImage");
            }
        }

        void SetActivePage(int index)
        {
            for (int i = 0; i < _pagesPanel.Children.Count; i++)
                _pagesPanel.Children[i].IsVisible = i == index;
        }
    }
}