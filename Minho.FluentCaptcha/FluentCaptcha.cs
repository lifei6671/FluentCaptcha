using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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
        /// <summary>
        /// 获取一个字符的路径
        /// </summary>
        /// <param name="s"></param>
        /// <param name="f"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        private static GraphicsPath TextPath(string s, Font f, Rectangle r)
        {
            var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near };
            var gp = new GraphicsPath();
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
        #endregion

        #region 添加噪点
        /// <summary>
        /// 添加噪点
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        public virtual ICapatcha AddNoise(Graphics g, Rectangle rect)
        {
            int density;
            int size;

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
            var br = new SolidBrush(_options.Colors.Next());
            int max = Convert.ToInt32(Math.Max(rect.Width, rect.Height) / size);
            for (int i = 0; i <= Convert.ToInt32((rect.Width * rect.Height) / density); i++)
            {
                g.FillEllipse(br, _random.Next(rect.Width), _random.Next(rect.Height), _random.Next(max), _random.Next(max));
            }
            br.Dispose();
            return this;
        }
        #endregion

        #region 添加线条
        /// <summary>
        /// 添加线条
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        public virtual ICapatcha AddLine(Graphics g, Rectangle rect)
        {
            int length;
            float width;
            int linecount;

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
            using (var p = new Pen(_options.Colors.Next(), width))
            {
                for (int l = 1; l <= linecount; l++)
                {
                    for (int i = 0; i <= length; i++)
                    {
                        pf[i] = _captchaPoint.Next(rect);
                    }

                    g.DrawCurve(p, pf, 1.75F);
                }
            }
            return this;
        }
        #endregion

        #region 添加文字
        /// <summary>
        /// 添加文字
        /// </summary>
        /// <param name="textPath"></param>
        /// <param name="rect"></param>
        public virtual ICapatcha WarpText(GraphicsPath textPath, Rectangle rect)
        {
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
                    return this;
            }

            var rectF = new RectangleF(Convert.ToSingle(rect.Left), 0, Convert.ToSingle(rect.Width), rect.Height);

            int hrange = Convert.ToInt32(rect.Height / warpDivisor);
            int wrange = Convert.ToInt32(rect.Width / warpDivisor);
            int left = rect.Left - Convert.ToInt32(wrange * rangeModifier);
            int top = rect.Top - Convert.ToInt32(hrange * rangeModifier);
            int width = rect.Left + rect.Width + Convert.ToInt32(wrange * rangeModifier);
            int height = rect.Top + rect.Height + Convert.ToInt32(hrange * rangeModifier);

            if (left < 0)
            { left = 0;}
            if (top < 0)
            { top = 0;}
            if (width > _options.Width)
            { width = _options.Width;}
            if (height > _options.Height)
            { height = _options.Height;}

            PointF leftTop = _captchaPoint.Next(left, left + wrange, top, top + hrange);
            PointF rightTop = _captchaPoint.Next(width - wrange, width, top, top + hrange);
            PointF leftBottom = _captchaPoint.Next(left, left + wrange, height - hrange, height);
            PointF rightBottom = _captchaPoint.Next(width - wrange, width, height - hrange, height);

            var points = new[] { leftTop, rightTop, leftBottom, rightBottom };
            var m = new Matrix();
            m.Translate(0, 0);
            textPath.Warp(points, rectF, m, WarpMode.Perspective, 0);
            return this;
        }
        #endregion

        #region 画图
        /// <summary>
        /// 画图
        /// </summary>
        /// <returns></returns>
        public virtual CaptchaResult DrawImage()
        {
            CaptchaResult captchaResult = new CaptchaResult
            {
                Bitmap = new Bitmap(_options.Width, _options.Height, PixelFormat.Format24bppRgb),
                Text = new string(_options.Text.NextText(_options.TextLength))
            };

            using (Graphics gr = Graphics.FromImage(captchaResult.Bitmap))
            {
                gr.SmoothingMode = SmoothingMode.AntiAlias;
                gr.CompositingMode = CompositingMode.SourceOver;
                gr.Clear(Color.White);
                
                int charOffset = 0;
                double charWidth = Convert.ToDouble(_options.Width / _options.TextLength);

                foreach (char c in captchaResult.Text)
                {
                    // establish font and draw area 
                    using (Font fnt = GetFont())
                    {
                        using (Brush fontBrush = new SolidBrush(_options.Colors.Next()))
                        {
                            Rectangle rectChar = new Rectangle(Convert.ToInt32(charOffset * charWidth), 0, Convert.ToInt32(charWidth), _options.Height);

                            // warp the character 
                            GraphicsPath gp = TextPath(c.ToString(CultureInfo.InvariantCulture), fnt, rectChar);
                            WarpText(gp, rectChar);
                            // draw the character 
                            gr.FillPath(fontBrush, gp);
                            charOffset += 1;
                        }
                    }
                }

                var rect = new Rectangle(new Point(0, 0), captchaResult.Bitmap.Size);
                AddNoise(gr, rect);
                AddLine(gr, rect);
                using (Pen solidBrush = new Pen(_options.BroderColor))
                {
                    gr.DrawRectangle(solidBrush, 0, 0, _options.Width - 1, _options.Height - 1);
                }
            }

            return captchaResult;
        } 
        #endregion
    }
}
