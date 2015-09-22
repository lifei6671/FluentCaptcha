using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Minho.FluentCaptcha
{
    internal class CaptchaPoint
    {
        private static int _randomSeed;
        private readonly Random _random;

        static CaptchaPoint()
        {
            _randomSeed = 1;
        }
        public CaptchaPoint()
        {
            int seed = Environment.TickCount + _randomSeed;
            _random = new Random(seed);
            _randomSeed++;
        }
        /// <summary>
        /// 获取一个指定 X 和 Y 坐标限定值之间的随机位置
        /// </summary>
        /// <param name="xmin"></param>
        /// <param name="xmax"></param>
        /// <param name="ymin"></param>
        /// <param name="ymax"></param>
        /// <returns></returns>
        public PointF Next(int xmin, int xmax, int ymin, int ymax)
        {
            return new PointF(_random.Next(xmin, xmax), _random.Next(ymin, ymax));
        }
        /// <summary>
        /// 随机获取一个 x 和 y 坐标
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public PointF Next(Rectangle rect)
        {
            return Next(rect.Left, rect.Width, rect.Top, rect.Bottom);
        }
    }
}
