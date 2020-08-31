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

namespace ImageCutter
{
    public class MainWindow : Window
    {
        Panel _pagesPanel = null;
        ImageSlicer _slicer = null;

        public MainWindow()
        {
            InitializeComponent();
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            _pagesPanel = this.Find<Panel>("PagesPanel");
            _slicer = this.Find<ImageSlicer>("Slicer");

            string cmdPath = null;
            foreach (string s in Environment.GetCommandLineArgs())
            {
                if (File.Exists(s) && (s.EndsWith(".png", StringComparison.OrdinalIgnoreCase)))
                {
                    cmdPath = s;
                    break;
                }
            }

            if (string.IsNullOrEmpty(cmdPath) || string.IsNullOrWhiteSpace(cmdPath))
                SetActivePage(0);
            else
            {
                _slicer.Source = new Bitmap(cmdPath);
                SetActivePage(1);
            }
        }

        public async void SliceExistingImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Directory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
                /*Filters = new List<FileDialogFilter>()
                {
                    new FileDialogFilter()
                    {
                        Extensions = new List<string>()
                        {
                            ".png"
                        },
                        Name = "PNG images"
                    }
                }*/
            };

            string[] paths = await dialog.ShowAsync(this);
            if (paths.Length > 0)
            {
                _slicer.Source = new Bitmap(paths[0]);
                SetActivePage(1);
            }
        }

        public async void SliceExistingImageSaveButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog dialog = new OpenFolderDialog();

            string path = await dialog.ShowAsync(this);
            if ((!string.IsNullOrEmpty(path)) && (!string.IsNullOrWhiteSpace(path)))
            {
                _slicer.SaveSlicesToFiles(path, "SlicedImage");
            }
        }

        void SetActivePage(int index)
        {
            for (int i = 0; i < _pagesPanel.Children.Count; i++)
                _pagesPanel.Children[i].IsVisible = i == index;
        }
    }
}