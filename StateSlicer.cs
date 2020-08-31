using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Controls.Platform.Surfaces;
using System;
using System.IO;

namespace ImageCutter
{
    public class StateSlicer : Image
    {
        static StateSlicer()
        {
            AffectsRender<Image>(SourceProperty, HorizontalStatesProperty, VerticalStatesProperty);
            AffectsMeasure<Image>(SourceProperty, HorizontalStatesProperty, VerticalStatesProperty);
        }

        public static readonly StyledProperty<int> HorizontalStatesProperty =
            AvaloniaProperty.Register<StateSlicer, int>(nameof(HorizontalStates));
        
        public int HorizontalStates
        {
            get => GetValue(HorizontalStatesProperty);
            set => SetValue(HorizontalStatesProperty, value);
        }

        public static readonly StyledProperty<int> VerticalStatesProperty =
            AvaloniaProperty.Register<StateSlicer, int>(nameof(VerticalStates));
        
        public int VerticalStates
        {
            get => GetValue(VerticalStatesProperty);
            set => SetValue(VerticalStatesProperty, value);
        }

        public override void Render(DrawingContext context)
        {
            var source = Source;
            int hStates = HorizontalStates;
            int vStates = VerticalStates;

            if (source != null && Bounds.Width > 0 && Bounds.Height > 0)
            {
                context.DrawImage(source, 1, new Rect(source.PixelSize.ToSize(1)), new Rect(0, 0, Bounds.Width, Bounds.Height));
            }

            var pen = new Pen(new SolidColorBrush(Colors.Red));

            for (int h = 0; h < hStates; h++)
            {
                double x = (Bounds.Width / hStates) * h;
                context.DrawLine(pen, new Point(x, 0), new Point(x, Bounds.Height));
            }

            for (int v = 0; v < vStates; v++)
            {
                double y = (Bounds.Height / vStates) * v;
                context.DrawLine(pen, new Point(0, y), new Point(Bounds.Width, y));
            }
        }

        public void SaveStatesToFiles(string folderPath, string prefix)
        {
            var source = Source;
            int hStates = HorizontalStates;
            int vStates = VerticalStates;

            Console.WriteLine("STATE COUNT: " + hStates + ", " + vStates);
            double xMult = Source.PixelSize.Width / hStates;
            double yMult = Source.PixelSize.Height / vStates;

            for (int y = 0; y < vStates; y++)
            {
                for (int x = 0; x < hStates; x++)
                {
                    ImageSubregionToFile(folderPath, prefix + "-(" + x + "," + y + ")", new Rect(x * xMult, y * yMult, xMult, yMult));
                }
            }
        }

        void ImageSubregionToFile(string folderPath, string fileName, Rect subregion)
        {
            if ((subregion.Width > 0) && (subregion.Height > 0))
            {
                WriteableBitmap bmp = new WriteableBitmap(new PixelSize((int)subregion.Width, (int)subregion.Height), Vector.One);
                
                using (IRenderTarget rtb = AvaloniaLocator.Current.GetService<IPlatformRenderInterface>().CreateRenderTarget(new object[] { new WriteableBitmapSurface(bmp) }))
                {
                    using (IDrawingContextImpl ctx = rtb.CreateDrawingContext(null))
                    {
                        ctx.DrawImage(Source.PlatformImpl, 1, subregion, subregion.WithX(0).WithY(0));
                    }
                }

                bmp.Save(Path.Combine(folderPath, fileName + ".png"));
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var source = Source;
            var result = new Size();

            if (source != null)
            {
                result = Stretch.Fill.CalculateSize(availableSize, source.Size, StretchDirection);
            }
            
            return result;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var source = Source;

            if (source != null)
            {
                var sourceSize = source.Size;
                var result = Stretch.Fill.CalculateSize(finalSize, sourceSize);
                return result;
            }
            else
            {
                return new Size();
            }
        }
    }
}