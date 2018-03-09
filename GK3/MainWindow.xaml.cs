using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace GK3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BitmapImage _bitmapImage;
        private List<Bitmap> ResorceBitmaps = new List<Bitmap>
        {
            Properties.Resources.Lenna,
            Properties.Resources.Bocian,
            Properties.Resources.Ja,
            Properties.Resources.Lis,
            Properties.Resources.Cat,
            Properties.Resources.Octopus
        };

        private ColorQuantizer _colorQuantizer;

        public MainWindow()
        {
            InitializeComponent();
            loadResorceBitmaps();
            _bitmapImage = BitmapToBitmapImage(Properties.Resources.Lenna);
            UpdateScene();
        }

        private void loadResorceBitmaps()
        {
            ObservableCollection<ImageButton> imageButtons = new ObservableCollection<ImageButton>();
            foreach (Bitmap resorceBitmap in ResorceBitmaps)
            {
                imageButtons.Add(new ImageButton() { BitmapImage = BitmapToBitmapImage(resorceBitmap)});
            }
            ImageButtonsControl.ItemsSource = imageButtons;
        }

        private BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }

        private void UpdateScene()
        {
            int Kr;
            int Kg;
            int Kb;
            int K;
            int cube;
            if(!Int32.TryParse(KRedText.Text, out Kr)) return;
            if(!Int32.TryParse(KGreenText.Text, out Kg)) return;
            if(!Int32.TryParse(KBlueText.Text, out Kb)) return;
            string[] ks = KText.Text.Split('/');
            if(ks.Length != 2) return;
            if (!Int32.TryParse(ks[0], out K)) return;
            if (!Int32.TryParse(ks[1], out cube)) return;
            if(PopularityAlgorithmRadionButton.IsChecked.Value) _colorQuantizer = new ColorQuantizerPopularityAlgorithm(_bitmapImage, K, cube);
            if(AverageDitheringRadionButton.IsChecked.Value) _colorQuantizer = new ColorQuantizerAverageDithering(_bitmapImage, Kr, Kg, Kb);
            if(ErrorDiffusionDitheringRadionButton.IsChecked.Value) _colorQuantizer = new ColorQuantizerErrorDiffusionDithering(_bitmapImage, Kr, Kg, Kb);
            if(OrderedDithering1RadionButton.IsChecked.Value) _colorQuantizer = new ColorQuantizerOrderedDithering1(_bitmapImage, Kr, Kg, Kb);
            if (OrderedDithering2RadionButton.IsChecked.Value) _colorQuantizer = new ColorQuantizerOrderedDithering2(_bitmapImage, Kr, Kg, Kb);
            ImageIn.Source = _bitmapImage;
            ImageOut.Source = _colorQuantizer.Process();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                _bitmapImage = new BitmapImage(new Uri(openFileDialog.FileName));
                ((ObservableCollection<ImageButton>) ImageButtonsControl.ItemsSource).Insert(0, new ImageButton() {BitmapImage = _bitmapImage});
                UpdateScene();
            }
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.Image image = (System.Windows.Controls.Image) sender;
            _bitmapImage = (BitmapImage)image.Source;
            UpdateScene();
        }

        private void RadionButton_OnChecked(object sender, RoutedEventArgs e)
        {
            if (_bitmapImage != null) UpdateScene();
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            if(_bitmapImage != null) UpdateScene();
        }
    }
}
