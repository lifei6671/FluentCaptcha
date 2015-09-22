using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Minho.FluentCaptcha
{
    public class CaptchaColor
    {
        private static readonly List<Color> DefaultColors = new List<Color>
        {
            Color.Red,
            Color.Green,
            Color.Blue,
            Color.Black,
            Color.Purple,
            Color.Orange ,
            Color.BlueViolet,
            Color.Brown,
            Color.CadetBlue,
            Color.Chocolate,
            Color.DarkCyan,
            Color.Coral,
            Color.CornflowerBlue,
            Color.Cyan,
            Color.DarkKhaki,
            Color.FromArgb(253,84,84),
            Color.FromArgb(100,100,253),
            Color.FromArgb(204,24,10),
            Color.FromArgb(200,10,200)
        };

        private readonly List<Color> _colors;
        private static int _randomSeed;
        private readonly Random _random;

        #region 构造方法
        static CaptchaColor()
        {
            _randomSeed = 1;
        }
        /// <summary>
        /// 初始化 CaptchaColor 对象的实例
        /// </summary>
        public CaptchaColor() : this(DefaultColors)
        {

        }
        /// <summary>
        /// 初始化 CaptchaColor 对象的实例
        /// </summary>
        /// <param name="colors"></param>
        public CaptchaColor(IEnumerable<Color> colors)
        {
            _colors = new List<Color>(colors);
            int seed = Environment.TickCount + _randomSeed;
            _random = new Random(seed);
            _randomSeed++;
        } 
        #endregion

        /// <summary>
        /// 获取当前颜色集合
        /// </summary>
        public ICollection<Color> Colors { get { return new ReadOnlyCollection<Color>(_colors);} } 
        public CaptchaColor Add(Color color)
        {
            _colors.Add(color);
            return this;
        }

        public CaptchaColor AddRange(IEnumerable<Color> colors)
        {
            _colors.AddRange(colors);
            return this;
        }

        public CaptchaColor Remove(Color color)
        {
            _colors.Remove(color);
            return this;
        }

        public CaptchaColor Clear()
        {
            _colors.Clear();
            return this;
        }
        /// <summary>
        /// 获取一个随机颜色
        /// </summary>
        /// <returns></returns>
        public Color Next()
        {
            int index = _random.Next(0, _colors.Count);
            Color color = _colors[index];
            return color;
        }
        /// <summary>
        /// 随机获取指定个数的字体
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public Color[] Next(int num)
        {
            Color[] colors = new Color[num];

            for (int i = 0; i < num; i++)
            {
                int index = _random.Next(0, _colors.Count);

                Color color = _colors[index];
                colors[i] = color;
            }
            return colors;
        }
    }
}
