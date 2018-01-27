using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FYJ.Winui.Controls
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:FYJ.Winui.Controls"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:FYJ.Winui.Controls;assembly=FYJ.Winui.Controls"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误: 
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:Image9Cell/>
    ///
    /// </summary>
    public class Image9Cell : Image
    {
        static Image9Cell()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Image9Cell), new FrameworkPropertyMetadata(typeof(Image9Cell)));
        }

        public Int32Rect ContentRect
        {
            get;
            set;
        }

        public bool DrawImageWith9Cells
        {
            get;
            set;
        }

        private BitmapSource SplitImage(BitmapSource source, Int32Rect clipRect)
        {
            BitmapSource results = null;
            var stride = clipRect.Width * ((source.Format.BitsPerPixel + 7) / 8);
            var pixelsCount = clipRect.Width * clipRect.Height;
            var tileRect = new Int32Rect(0, 0, clipRect.Width, clipRect.Height);
            var pixels = new int[pixelsCount];

            source.CopyPixels(clipRect, pixels, stride, 0);
            var wb = new WriteableBitmap(
                clipRect.Width,
                clipRect.Height,
                source.DpiX,
                source.DpiY,
                source.Format,
                source.Palette);
            wb.Lock();
            wb.WritePixels(tileRect, pixels, stride, 0);
            wb.Unlock();

            results = wb;
            return results;
        }

        private Int32Rect[] Get9Int32Rect(BitmapSource source, Int32Rect contentRect)
        {
            Int32Rect[] results = new Int32Rect[9];
            results[0] = new Int32Rect(0, 0, contentRect.X, contentRect.Y);
            results[1] = new Int32Rect(contentRect.X, 0, contentRect.Width, contentRect.Y);
            results[2] = new Int32Rect(contentRect.X + contentRect.Width, 0, source.PixelWidth - contentRect.Width - contentRect.X, contentRect.Y);

            results[3] = new Int32Rect(0, contentRect.Y, contentRect.X, contentRect.Height);
            results[4] = new Int32Rect(contentRect.X, contentRect.Y, contentRect.Width, contentRect.Height);
            results[5] = new Int32Rect(contentRect.X + contentRect.Width, contentRect.Y, source.PixelWidth - contentRect.Width - contentRect.X, contentRect.Height);

            results[6] = new Int32Rect(0, contentRect.Y + contentRect.Height, contentRect.X, source.PixelHeight - contentRect.Y - contentRect.Height);
            results[7] = new Int32Rect(contentRect.X, contentRect.Y + contentRect.Height, contentRect.Width, source.PixelHeight - contentRect.Y - contentRect.Height);
            results[8] = new Int32Rect(contentRect.X + contentRect.Width, contentRect.Y + contentRect.Height, source.PixelWidth - contentRect.Width - contentRect.X, source.PixelHeight - contentRect.Y - contentRect.Height);

            return results;
        }

        private BitmapSource[] Get9CellImageSource(BitmapSource source, Int32Rect contentRect)
        {
            BitmapSource[] results = new BitmapSource[9];
            Int32Rect[] rects = Get9Int32Rect(source, contentRect);
            for (int i = 0; i < results.Length; i++)
            {
                results[i] = SplitImage(source, rects[i]);
            }

            return results;
        }

        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
            if (DrawImageWith9Cells == false)
            {

                base.OnRender(dc);
                return;
            }

            BitmapSource image = (BitmapSource)this.Source;
            BitmapSource resultImage = this.Create9CellImage(image, new Int32Rect(5, 31, image.PixelWidth - 10, image.PixelHeight - 31 - 34), (int)this.Width, (int)this.Height);
            dc.DrawImage(resultImage,new Rect(0,0,this.Width,this.Height));
        }

        /// <summary>
        /// 创建平铺的图片 
        /// </summary>
        /// <author>fangyj</author>
        /// <param name="source">源图片</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <returns></returns>
        private BitmapSource CreateTileImage(BitmapSource source, int width, int height)
        {
            RenderTargetBitmap composeImage = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Default);
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = source;
            brush.TileMode = TileMode.Tile;
            drawingContext.DrawRectangle(brush, null, new Rect(0, 0, width, height));
            drawingContext.Close();
            composeImage.Render(drawingVisual);

            return composeImage;
        }

        /// <summary>
        /// 创建9宫格图片
        /// </summary>
        /// <param name="source"></param>
        /// <param name="contentRect"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public BitmapSource Create9CellImage(BitmapSource source, Int32Rect contentRect, int width, int height)
        {
            BitmapSource[] images = Get9CellImageSource(source, contentRect);
            RenderTargetBitmap composeImage = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Default);
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
           // Brush bc = new SolidColorBrush(Colors.Blue);
           // drawingContext.DrawRectangle(bc, null, new Rect(0, 0, width, height));

            drawingContext.DrawImage(images[0],new Rect(0,0,contentRect.X,contentRect.Y));
            //横行平铺
            drawingContext.DrawImage(CreateTileImage(images[1], width - images[0].PixelWidth - images[2].PixelWidth, contentRect.Y), new Rect(contentRect.X, 0, width - images[0].PixelWidth - images[2].PixelWidth, contentRect.Y));
            drawingContext.DrawImage(images[2], new Rect(width - images[2].PixelWidth, 0, images[2].PixelWidth, images[2].PixelHeight));

            drawingContext.DrawImage(images[3], new Rect(0, contentRect.Y, images[3].PixelWidth,height- images[0].PixelHeight-images[6].PixelHeight));
            //两边平铺
            drawingContext.DrawImage(CreateTileImage(images[4], width - images[3].PixelWidth - images[5].PixelWidth, height - images[1].PixelHeight - images[7].PixelHeight), new Rect(contentRect.X, contentRect.Y, width - images[0].PixelWidth - images[2].PixelWidth, height - images[1].PixelHeight - images[7].PixelHeight));
            drawingContext.DrawImage(images[5], new Rect(width - images[5].PixelWidth, contentRect.Y, images[5].PixelWidth, height - images[0].PixelHeight - images[6].PixelHeight));

            drawingContext.DrawImage(images[6], new Rect(0, height - images[6].PixelHeight, images[6].PixelWidth, images[6].PixelHeight));
            //横向拉伸
            drawingContext.DrawImage(CreateTileImage(images[7], width - images[6].PixelWidth - images[8].PixelWidth, images[7].PixelHeight), new Rect(contentRect.X, height - images[6].PixelHeight, width - images[6].PixelWidth - images[8].PixelWidth, images[7].PixelHeight));
            drawingContext.DrawImage(images[8], new Rect(width-images[8].PixelWidth, height - images[6].PixelHeight, images[8].PixelWidth, images[8].PixelHeight));
           
            drawingContext.Close();
            composeImage.Render(drawingVisual);

            return composeImage;
        }
    }
}
