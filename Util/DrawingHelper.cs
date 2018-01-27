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

namespace FYJ.Winui.Util
{
    public class DrawingHelper
    {
        #region 分割图片
        private static ImageSource SplitImage(BitmapSource source, Int32Rect clipRect)
        {
            ImageSource results = null;
            var stride = clipRect.Width * ((source.Format.BitsPerPixel + 7) / 8);
            var pixelsCount = clipRect.Width * clipRect.Height;//tileWidth * tileHeight;
            var tileRect = new Int32Rect(0, 0, clipRect.Width, clipRect.Height);

            var pixels = new int[pixelsCount];
            //var copyRect = new Int32Rect(col * tileWidth, row * tileHeight, tileWidth, tileHeight);
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

        private static ImageSource[] Get9CellImageSource(BitmapSource source, Int32Rect clipRect)
        {
            ImageSource[] results = new ImageSource[9];
            Int32Rect rect = Int32Rect.Empty;
            int rightSideWidth = (int)(source.PixelWidth - clipRect.X - clipRect.Width);
            //top-left
            rect.Width = clipRect.X;
            rect.Height = clipRect.Y;
            results[0] = SplitImage(source, rect);
            //return results;
            //top-middle
            rect.X += rect.Width;
            rect.Width = clipRect.Width;
            results[1] = SplitImage(source, rect);
            //top-right
            rect.X += rect.Width;
            rect.Width = rightSideWidth;
            results[2] = SplitImage(source, rect);

            //left side
            rect = Int32Rect.Empty;
            rect.Y = clipRect.Y;
            rect.Width = clipRect.X;
            rect.Height = clipRect.Height;
            results[3] = SplitImage(source, rect);

            //middle
            rect.X += rect.Width;
            rect.Width = clipRect.Width;
            results[4] = SplitImage(source, rect);

            //right side
            rect.X += rect.Width;
            rect.Width = rightSideWidth;
            results[5] = SplitImage(source, rect);
            //bottom-left
            rect = Int32Rect.Empty;
            rect.Y = clipRect.Y + clipRect.Height;
            // rect.X = clipRect.X;
            rect.Height = source.PixelHeight - clipRect.Height - clipRect.Y;
            rect.Width = clipRect.X;
            results[6] = SplitImage(source, rect);

            //bottom-middle
            rect.X += rect.Width;
            rect.Width = clipRect.Width;
            results[7] = SplitImage(source, rect);
            //bottom-right
            rect.X += rect.Width;
            rect.Width = rightSideWidth;
            results[8] = SplitImage(source, rect);
            return results;
        }
        #endregion


        /// <summary>
        /// 创建平铺的图片
        /// </summary>
        /// <param name="source">源图片</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <returns></returns>
        public static BitmapSource CreateTileImage(BitmapSource source, int width, int height)
        {
            RenderTargetBitmap composeImage = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Default);
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            Brush bc = new SolidColorBrush(Colors.Blue);
            drawingContext.DrawRectangle(bc,null,new Rect(0,0,width,height));
            //int i = width/(int)source.Width;
            //int j = height/(int)source.Height;
            for(int i=0;i<=width/(int)source.Width;i++)
            {
                for(int j=0;j<=height/(int)source.Height;j++)
                {
                      drawingContext.DrawImage(source, new Rect(i * source.Width, j*source.Height, source.Width, source.Height));
                }
            }
            drawingContext.Close();
            composeImage.Render(drawingVisual);

            return composeImage;
        }

       
    }
}
