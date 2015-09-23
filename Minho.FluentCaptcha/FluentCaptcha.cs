using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Text;

namespace Minho.FluentCaptcha
{
    public class FluentCaptcha : ICapatcha
    {
        private CaptchaOptions _options;
        private static int _randomSeed;
        private readonly Random _random;
        private readonly CaptchaPoint _captchaPoint;
        private Bitmap _bitmap;
        private Graphics _graphics;
        private char[] _texts;
        static FluentCaptcha()
        {
            _randomSeed = 1;
        }

        public FluentCaptcha()
        {
            int seed = Environment.TickCount + _randomSeed;
            _random = new Random(seed);
            _randomSeed++;
            _captchaPoint = new CaptchaPoint();
            _options = new CaptchaOptions();
        }
        /// <summary>
        /// 生成图片的参数
        /// </summary>
        public CaptchaOptions Options { set { _options = value; } get { return _options; } }

        #region 私有方法

        #region 根据普通矩形得到圆角矩形的路径
        /// <summary>
        /// 根据普通矩形得到圆角矩形的路径
        /// </summary>
        /// <param name="rectangle">原始矩形</param>
        /// <param name="r">半径</param>
        /// <returns>图形路径</returns>
        private GraphicsPath GetRoundRectangle(Rectangle rectangle, int r)
        {
            int l = 2 * r;
            // 把圆角矩形分成八段直线、弧的组合，依次加到路径中
            GraphicsPath gp = new GraphicsPath();
            gp.AddLine(new Point(rectangle.X + r, rectangle.Y), new Point(rectangle.Right - r, rectangle.Y));
            gp.AddArc(new Rectangle(rectangle.Right - l, rectangle.Y, l, l), 270F, 90F);

            gp.AddLine(new Point(rectangle.Right, rectangle.Y + r), new Point(rectangle.Right, rectangle.Bottom - r));
            gp.AddArc(new Rectangle(rectangle.Right - l, rectangle.Bottom - l, l, l), 0F, 90F);

            gp.AddLine(new Point(rectangle.Right - r, rectangle.Bottom), new Point(rectangle.X + r, rectangle.Bottom));
            gp.AddArc(new Rectangle(rectangle.X, rectangle.Bottom - l, l, l), 90F, 90F);

            gp.AddLine(new Point(rectangle.X, rectangle.Bottom - r), new Point(rectangle.X, rectangle.Y + r));
            gp.AddArc(new Rectangle(rectangle.X, rectangle.Y, l, l), 180F, 90F);
            return gp;
        }
        #endregion

        /// <summary>
        /// 获取一个字符的路径
        /// </summary>
        /// <param name="s"></param>
        /// <param name="f"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        private static GraphicsPath TextPath(string s, Font f, Rectangle r)
        {
            StringFormat sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near };
            GraphicsPath gp = new GraphicsPath();
            gp.AddString(s, f.FontFamily, (int)f.Style, f.Size, r, sf);
            return gp;
        }

        private Font GetFont()
        {
            float fsize;
            string fname = Options.FontFamily.Next();
            switch (_options.FontWarp)
            {
                case NoiseLevel.Low:
                    fsize = Convert.ToInt32(_options.Height * 0.8);
                    break;
                case NoiseLevel.Medium:
                    fsize = Convert.ToInt32(_options.Height * 0.85);
                    break;
                case NoiseLevel.High:
                    fsize = Convert.ToInt32(_options.Height * 0.9);
                    break;
                case NoiseLevel.Extreme:
                    fsize = Convert.ToInt32(_options.Height * 0.95);
                    break;
                default:
                    fsize = Convert.ToInt32(_options.Height * 0.7);
                    break;
            }
            return new Font(fname, fsize, FontStyle.Bold);
        }
        /// <summary>
        /// 添加曲线
        /// </summary>
        /// <param name="textPath"></param>
        /// <param name="rect"></param>
        private void DrawTextPath(GraphicsPath textPath, Rectangle rect)
        {
            TryCreateBitmap();
            float warpDivisor;
            float rangeModifier;

            switch (_options.FontWarp)
            {
                case NoiseLevel.Low:
                    warpDivisor = 6F;
                    rangeModifier = 1F;
                    break;
                case NoiseLevel.Medium:
                    warpDivisor = 5F;
                    rangeModifier = 1.3F;
                    break;
                case NoiseLevel.High:
                    warpDivisor = 4.5F;
                    rangeModifier = 1.4F;
                    break;
                case NoiseLevel.Extreme:
                    warpDivisor = 4F;
                    rangeModifier = 1.5F;
                    break;
                default:
                    return;
            }

            var rectF = new RectangleF(Convert.ToSingle(rect.Left), 0, Convert.ToSingle(rect.Width), rect.Height);

            int hrange = Convert.ToInt32(rect.Height / warpDivisor);
            int wrange = Convert.ToInt32(rect.Width / warpDivisor);
            int left = rect.Left - Convert.ToInt32(wrange * rangeModifier);
            int top = rect.Top - Convert.ToInt32(hrange * rangeModifier);
            int width = rect.Left + rect.Width + Convert.ToInt32(wrange * rangeModifier);
            int height = rect.Top + rect.Height + Convert.ToInt32(hrange * rangeModifier);

            if (left < 0)
            {
                left = 0;
            }
            if (top < 0)
            {
                top = 0;
            }
            if (width > _options.Width)
            {
                width = _options.Width;
            }
            if (height > _options.Height)
            {
                height = _options.Height;
            }

            PointF leftTop = _captchaPoint.Next(left, left + wrange, top, top + hrange);
            PointF rightTop = _captchaPoint.Next(width - wrange, width, top, top + hrange);
            PointF leftBottom = _captchaPoint.Next(left, left + wrange, height - hrange, height);
            PointF rightBottom = _captchaPoint.Next(width - wrange, width, height - hrange, height);

            var points = new[] { leftTop, rightTop, leftBottom, rightBottom };
            Matrix m = new Matrix();
            m.Translate(0, 0);
            textPath.Warp(points, rectF, m, WarpMode.Perspective, 0);
        }

        #region 生成随机颜色
        /// <summary>
        /// 生成随机浅颜色
        /// </summary>
        /// <returns>randomColor</returns>
        private Color GetRandomLightColor()
        {
            int low = 180;           //色彩的下限
            int high = 255;          //色彩的上限      
            var nRed = _random.Next(high) % (high - low) + low;
            var nGreen = _random.Next(high) % (high - low) + low;
            var nBlue = _random.Next(high) % (high - low) + low;
            Color color = Color.FromArgb(nRed, nGreen, nBlue);
            return color;
        }
        /// <summary>
        /// 生成随机深颜色
        /// </summary>
        /// <returns></returns>
        private Color GetRandomDeepColor()
        {
            int nRed, nGreen, nBlue;    // nBlue,nRed  nGreen 相差大一点 nGreen 小一些
            //int high = 255;       
            int redLow = 160;
            int greenLow = 100;
            int blueLow = 160;
            nRed = _random.Next(redLow);
            nGreen = _random.Next(greenLow);
            nBlue = _random.Next(blueLow);
            Color color = Color.FromArgb(nRed, nGreen, nBlue);
            return color;
        } 
        #endregion
        #endregion

        #region 添加噪点
        /// <summary>
        /// 添加背景噪点
        /// </summary>
        public virtual ICapatcha DrawBackgroud()
        {
            TryCreateBitmap();
            int density;
            int size;
            var rect = new Rectangle(new Point(0, 0), _bitmap.Size);
            switch (_options.Background)
            {
                case NoiseLevel.None:
                    goto default;
                case NoiseLevel.Low:
                    density = 30;
                    size = 40;
                    break;
                case NoiseLevel.Medium:
                    density = 18;
                    size = 40;
                    break;
                case NoiseLevel.High:
                    density = 16;
                    size = 39;
                    break;
                case NoiseLevel.Extreme:
                    density = 10;
                    size = 35;
                    break;
                default:
                    return this;
            }
            SolidBrush br = new SolidBrush(GetRandomDeepColor());
            SolidBrush lightBrush = new SolidBrush(GetRandomLightColor());
            SolidBrush deepBrush = new SolidBrush(_options.Colors.Next());
            int max = Convert.ToInt32(Math.Max(rect.Width, rect.Height) / size);
            for (int i = 0,len = Convert.ToInt32((rect.Width * rect.Height) / density); i <= len; i++)
            {
                _graphics.FillEllipse(br, _random.Next(rect.Width), _random.Next(rect.Height), _random.Next(max), _random.Next(max));
                _graphics.FillEllipse(lightBrush, _random.Next(rect.Width), _random.Next(rect.Height), _random.Next(max), _random.Next(max));
                _graphics.FillEllipse(deepBrush, _random.Next(rect.Width), _random.Next(rect.Height), _random.Next(max), _random.Next(max));
            }

            br.Dispose();
            return this;
        }
        #endregion

        #region 添加线条
        /// <summary>
        /// 添加线条
        /// </summary>
        public virtual ICapatcha DrawLine()
        {
            TryCreateBitmap();
            int length;
            float width;
            int linecount;
            var rect = new Rectangle(new Point(0, 0), _bitmap.Size);
            switch (_options.Line)
            {
                case NoiseLevel.None:
                    goto default;
                case NoiseLevel.Low:
                    length = 4;
                    width = Convert.ToSingle(_options.Height / 31.25);
                    linecount = 1;
                    break;
                case NoiseLevel.Medium:
                    length = 5;
                    width = Convert.ToSingle(_options.Height / 27.7777);
                    linecount = 2;
                    break;
                case NoiseLevel.High:
                    length = 3;
                    width = Convert.ToSingle(_options.Height / 25);
                    linecount = 3;
                    break;
                case NoiseLevel.Extreme:
                    length = 3;
                    width = Convert.ToSingle(_options.Height / 22.7272);
                    linecount = 4;
                    break;
                default:
                    return this;
            }

            var pf = new PointF[length + 1];
            using (var p = new Pen(GetRandomDeepColor(), width))
            {
                for (int l = 1; l <= linecount; l++)
                {
                    for (int i = 0; i <= length; i++)
                    {
                        pf[i] = _captchaPoint.Next(rect);
                    }

                    _graphics.DrawCurve(p, pf, 1.75F);
                }
            }
            return this;
        }
        #endregion

        #region 添加文字
        /// <summary>
        /// 添加文字
        /// </summary>
        /// <returns></returns>
        public virtual ICapatcha DrawText()
        {
            TryCreateBitmap();
            int charOffset = 0;
            double charWidth = Convert.ToDouble(_options.Width / _options.TextLength);

            foreach (char c in _texts)
            {
                // establish font and draw area 
                using (Font fnt = GetFont())
                {
                    using (Brush fontBrush = new SolidBrush(_options.Colors.Next()))
                    {
                        Rectangle rectChar = new Rectangle(Convert.ToInt32(charOffset * charWidth), 0, Convert.ToInt32(charWidth), _options.Height);

                        GraphicsPath gp = TextPath(c.ToString(CultureInfo.InvariantCulture), fnt, rectChar);
                        DrawTextPath(gp, rectChar);
                        _graphics.FillPath(fontBrush, gp);
                        charOffset += 1;
                    }
                }
            }
            return this;
        }
   
        #endregion

        #region 添加边框
        /// <summary>
        /// 添加边框
        /// </summary>
        /// <returns></returns>
        public virtual ICapatcha DrawBroder()
        {
            TryCreateBitmap();
            Rectangle rectangle = new Rectangle(0, 0, _bitmap.Width - 1, _bitmap.Height - 1);

            using (GraphicsPath graphicsPath = GetRoundRectangle(rectangle, _options.Broder.Radius))
            {
                using (Pen pen = new Pen(_options.Broder.Color))
                {
                    _graphics.DrawPath(pen, graphicsPath);
                }
            }

            return this;
        }
        
        #endregion

        #region 实现雾化
        /// <summary>
        /// 实现雾化
        /// </summary>
        /// <returns></returns>
        public virtual ICapatcha Atomized()
        {
            if (_options.GaussianDeviation > 0)
            {
                FogImage.UnsafeProcessBitmap(_bitmap, _options.GaussianDeviation);
            }
            return this;
        } 
        #endregion

        #region 绘制图片

        /// <summary>
        /// 画图
        /// </summary>
        /// <returns></returns>
        public virtual CaptchaResult DrawImage()
        {
            TryCreateBitmap();
            CaptchaResult captchaResult = new CaptchaResult
            {
                Bitmap = _bitmap,
                Text = new string(_texts)
            };

            
            return captchaResult;
        }

        #endregion

        #region 尝试创建图片对象
        private void TryCreateBitmap()
        {
            if (_bitmap == null)
            {
                _bitmap = new Bitmap(_options.Width, _options.Height, PixelFormat.Format24bppRgb);
            }
            if (_graphics == null)
            {
                _graphics = Graphics.FromImage(_bitmap);
                _graphics.SmoothingMode = SmoothingMode.HighQuality;
                _graphics.CompositingMode = CompositingMode.SourceOver;
                _graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                _graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
                _graphics.TextContrast = 5;
                _graphics.CompositingQuality = CompositingQuality.AssumeLinear;
                
                _graphics.Clear(Color.White);
            }
            if (_texts == null)
            {
                _texts =_options.Text.NextText(_options.TextLength);
            }
        } 
        #endregion

        public void Dispose()
        {
            _graphics.Dispose();
        }
    }
}
