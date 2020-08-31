using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Controls.Platform.Surfaces;
using System.IO;

namespace ImageCutter
{
    public class ImageSlicer : Image
    {
        static ImageSlicer()
        {
            AffectsRender<Image>(SourceProperty, SlicesProperty);
            AffectsMeasure<Image>(SourceProperty, SlicesProperty);
        }

        public static readonly StyledProperty<Thickness> SlicesProperty =
            AvaloniaProperty.Register<ImageSlicer, Thickness>(nameof(Slices));
        
        public Thickness Slices
        {
            get => GetValue(SlicesProperty);
            set => SetValue(SlicesProperty, value);
        }

        public override void Render(DrawingContext context)
        {
            Thickness slices = Slices;

            /*if (slices.IsEmpty)
            {
                base.Render(context);
            }
            else
            {*/
            var source = Source;

            if (source != null && Bounds.Width > 0 && Bounds.Height > 0)
            {
                double centerSrcWidth = source.PixelSize.Width - (slices.Left + slices.Right);
                double centerDestWidth = Bounds.Width - (slices.Left + slices.Right);

                //top left
                var topLeftSrc = new Rect(0, 0, slices.Left, slices.Top);
                var topLeftDest = topLeftSrc;
                context.DrawImage(source, 1, topLeftSrc, topLeftDest);

                //top center
                var topSrc = new Rect(slices.Left, 0, centerSrcWidth, slices.Top);
                var topDest = new Rect(slices.Left, 0, centerDestWidth, slices.Top);
                context.DrawImage(source, 1, topSrc, topDest);

                //top right
                var topRightSrc = new Rect(source.PixelSize.Width - slices.Right, 0, slices.Right, slices.Top);
                var topRightDest = new Rect(Bounds.Width - slices.Right, 0, slices.Right, slices.Top);
                context.DrawImage(source, 1, topRightSrc, topRightDest);


                double middleSrcHeight = source.PixelSize.Height - (slices.Top + slices.Bottom);
                double middleDestHeight = Bounds.Height - (slices.Top + slices.Bottom);
                //middle left
                var middleLeftSrc = new Rect(0, slices.Top, slices.Left, middleSrcHeight);
                var middleLeftDest = new Rect(0, slices.Top, slices.Left, middleDestHeight);
                context.DrawImage(source, 1, middleLeftSrc, middleLeftDest);

                //middle center
                var middleSrc = new Rect(slices.Left, slices.Top, centerSrcWidth, middleSrcHeight);
                var middleDest = new Rect(slices.Left, slices.Top, centerDestWidth, middleDestHeight);
                context.DrawImage(source, 1, middleSrc, middleDest);

                //middle right
                var middleRightSrc = new Rect(source.PixelSize.Width - slices.Right, slices.Top, slices.Right, middleSrcHeight);
                var middleRightDest = new Rect(Bounds.Width - slices.Right, slices.Top, slices.Right, middleDestHeight);
                context.DrawImage(source, 1, middleRightSrc, middleRightDest);

                
                double bottomSrcTop = source.PixelSize.Height - slices.Bottom;
                double bottomDestTop = Bounds.Height - slices.Bottom;
                //bottom left
                var bottomLeftSrc = new Rect(0, bottomSrcTop, slices.Left, slices.Bottom);
                var bottomLeftDest = new Rect(0, bottomDestTop, slices.Left, slices.Bottom);
                context.DrawImage(source, 1, bottomLeftSrc, bottomLeftDest);

                //bottom center
                var bottomSrc = new Rect(slices.Left, bottomSrcTop, centerSrcWidth, slices.Bottom);
                var bottomDest = new Rect(slices.Left, bottomDestTop, centerDestWidth, slices.Bottom);
                context.DrawImage(source, 1, bottomSrc, bottomDest);

                //bottom right
                var bottomRightSrc = new Rect(source.PixelSize.Width - slices.Right, bottomSrcTop, slices.Right, slices.Bottom);
                var bottomRightDest = new Rect(Bounds.Width - slices.Right, bottomDestTop, slices.Right, slices.Bottom);
                context.DrawImage(source, 1, bottomRightSrc, bottomRightDest);
            }
            //}


            var pen = new Pen(new SolidColorBrush(Colors.Red));

            //left slice
            if (slices.Left > 0)
                context.DrawLine(pen, new Point(slices.Left, 0), new Point(slices.Left, Bounds.Height));

            //top slice
            if (slices.Top > 0)
                context.DrawLine(pen, new Point(0, slices.Top), new Point(Bounds.Width, slices.Top));

            //right slice
            if (slices.Right > 0)
                context.DrawLine(pen, new Point(Bounds.Width - slices.Right, 0), new Point(Bounds.Width - slices.Right, Bounds.Height));

            //bottom slice
            if (slices.Bottom > 0)
                context.DrawLine(pen, new Point(0, Bounds.Height - slices.Bottom), new Point(Bounds.Width, Bounds.Height - slices.Bottom));
        }

        public void SaveSlicesToFiles(string folderPath, string prefix)
        {
            var source = Source;
            var slices = Slices;

            double centerSrcWidth = source.PixelSize.Width - (slices.Left + slices.Right);

            //top left
            ImageSubregionToFile(folderPath, prefix + "-TopLeft", new Rect(0, 0, slices.Left, slices.Top));

            //top center
            ImageSubregionToFile(folderPath, prefix + "-TopCenter", new Rect(slices.Left, 0, centerSrcWidth, slices.Top));

            //top right
            ImageSubregionToFile(folderPath, prefix + "-TopRight", new Rect(source.PixelSize.Width - slices.Right, 0, slices.Right, slices.Top));


            double middleSrcHeight = source.PixelSize.Height - (slices.Top + slices.Bottom);


            //middle left
            ImageSubregionToFile(folderPath, prefix + "-MiddleLeft", new Rect(0, slices.Top, slices.Left, middleSrcHeight));

            //middle center
            ImageSubregionToFile(folderPath, prefix + "-MiddleCenter", new Rect(slices.Left, slices.Top, centerSrcWidth, middleSrcHeight));

            //middle right
            ImageSubregionToFile(folderPath, prefix + "-MiddleRight", new Rect(source.PixelSize.Width - slices.Right, slices.Top, slices.Right, middleSrcHeight));

            
            double bottomSrcTop = source.PixelSize.Height - slices.Bottom;
            

            //bottom left
            ImageSubregionToFile(folderPath, prefix + "-BottomLeft", new Rect(0, bottomSrcTop, slices.Left, slices.Bottom));

            //bottom center
            ImageSubregionToFile(folderPath, prefix + "-BottomCenter", new Rect(slices.Left, bottomSrcTop, centerSrcWidth, slices.Bottom));

            //bottom right
            ImageSubregionToFile(folderPath, prefix + "-BottomRight", new Rect(source.PixelSize.Width - slices.Right, bottomSrcTop, slices.Right, slices.Bottom));
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

        //https://github.com/Kermalis/PokemonGameEngine/blob/master/MapEditor/Util/WriteableBitmapSurface.cs
        internal sealed class WriteableBitmapSurface : IFramebufferPlatformSurface
        {
            private readonly WriteableBitmap _bitmap;
            public WriteableBitmapSurface(WriteableBitmap bmp)
            {
                _bitmap = bmp;
            }
            public ILockedFramebuffer Lock()
            {
                return _bitmap.Lock();
            }
    }
    }
}