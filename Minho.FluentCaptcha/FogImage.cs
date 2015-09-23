using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Minho.FluentCaptcha
{
    internal class FogImage
    {

        #region IImageProcessable 成员

        public unsafe void ProcessBitmap(System.Drawing.Bitmap bmp)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            Random rnd = new Random();
            for (int x = 0; x < width - 1; x++)
            {
                for (int y = 0; y < height - 1; y++)
                {
                    int k = rnd.Next(123456780, 123456789);
                    //像素块大小
                    int dx = x + k%7;
                    int dy = y + k%7;
                    //处理溢出
                    if (dx >= width)
                        dx = width - 1;
                    if (dy >= height)
                        dy = height - 1;
                    if (dx < 0)
                        dx = 0;
                    if (dy < 0)
                        dy = 0;

                    Color c1 = bmp.GetPixel(dx, dy);
                    bmp.SetPixel(x, y, c1);
                }
            }

        }

        #endregion

        #region IImageProcessable 成员

        public unsafe void UnsafeProcessBitmap(Bitmap bmp)
        {
            UnsafeProcessBitmap(bmp, 7);
        }


        #endregion


        public static unsafe void UnsafeProcessBitmap(Bitmap bmp, double N)
        {

            int width = bmp.Width;
            int height = bmp.Height;
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            byte* ptr = (byte*) (bmpData.Scan0);
            Random rnd = new Random();
            for (int i = 0; i < height - 1; i++)
            {
                for (int j = 0; j < width - 1; j++)
                {
                    int k = rnd.Next(-12345, 12345);
                    //像素块大小 常量N的大小决定雾化模糊度
                    int dj = Convert.ToInt32(j + k%N); //水平向右方向像素偏移后
                    int di = Convert.ToInt32(i + k%N); //垂直向下方向像素偏移后
                    if (dj >= width) dj = width - 1;
                    if (di >= height) di = height - 1;
                    if (di < 0)
                        di = 0;
                    if (dj < 0)
                        dj = 0;
                    //针对Format32bppArgb格式像素，指针偏移量为4的倍数 4*dj  4*di
                    //g(i,j)=f(di,dj)
                    ptr[bmpData.Stride*i + j*4 + 0] = ptr[bmpData.Stride*di + dj*4 + 0]; //B
                    ptr[bmpData.Stride*i + j*4 + 1] = ptr[bmpData.Stride*di + dj*4 + 1]; //G
                    ptr[bmpData.Stride*i + j*4 + 2] = ptr[bmpData.Stride*di + dj*4 + 2]; //R
                    // ptr += 4;  注意此处指针没移动，始终以bmpData.Scan0开始
                }
                //  ptr += bmpData.Stride - width * 4;
            }
            bmp.UnlockBits(bmpData);
        }
    }
}
