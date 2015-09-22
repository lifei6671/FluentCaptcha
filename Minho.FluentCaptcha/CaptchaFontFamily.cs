using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Minho.FluentCaptcha
{
    /// <summary>
    /// 验证码图片所需的字体
    /// </summary>
    public class CaptchaFontFamily
    {
        private static readonly List<string> DefaultFontFamily = new List<string> { "arial", "arial black", "comic sans ms", "courier new", "estrangelo edessa", "franklin gothic medium", "georgia", "lucida console", "lucida sans unicode", "mangal", "microsoft sans serif", "palatino linotype", "sylfaen", "tahoma", "times new roman", "trebuchet ms", "verdana" };

        private readonly List<string> _randomFontFamily;
        private static int _randomSeed;
        private readonly Random _random;

        static CaptchaFontFamily()
        {
            _randomSeed = 1;
        }

        /// <summary>
        /// 初始化字体集合对象的实例
        /// </summary>
        public CaptchaFontFamily():this(DefaultFontFamily)
        {
        }
        /// <summary>
        /// 初始化字体集合对象的实例
        /// </summary>
        /// <param name="fonts"></param>
        public CaptchaFontFamily(IEnumerable<string> fonts)
        {
            _randomFontFamily = new List<string>(fonts);
            int seed = Environment.TickCount + _randomSeed;
            _random = new Random(seed);
            _randomSeed++;
        }
        /// <summary>
        /// 获取的当前字体集合
        /// </summary>
        public ICollection<string> FontFamily { get { return new ReadOnlyCollection<string>(_randomFontFamily);} } 
        /// <summary>
        /// 添加一个字体
        /// </summary>
        /// <param name="font"></param>
        /// <returns></returns>
        public CaptchaFontFamily Add(string font)
        {
            _randomFontFamily.Add(font);
            return this;
        }
        /// <summary>
        /// 将字体集合添加到当前集合列表中
        /// </summary>
        /// <param name="fonts"></param>
        /// <returns></returns>
        public CaptchaFontFamily AddRange(IEnumerable<string> fonts)
        {
            _randomFontFamily.AddRange(fonts);
            return this;
        }
        /// <summary>
        /// 移除一个字体
        /// </summary>
        /// <param name="font"></param>
        /// <returns></returns>
        public CaptchaFontFamily Remove(string font)
        {
            _randomFontFamily.Remove(font);
            return this;
        }
        /// <summary>
        /// 清空当前所有字体集合
        /// </summary>
        /// <returns></returns>
        public CaptchaFontFamily Clear()
        {
            _randomFontFamily.Clear();
            return this;
        }
        /// <summary>
        /// 获取一个随机字体
        /// </summary>
        /// <returns></returns>
        public string Next()
        {
            int index = _random.Next(0, _randomFontFamily.Count);
            string font = _randomFontFamily[index];
            return font;
        }
        /// <summary>
        /// 随机获取指定个数的字体
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public string[] Next(int num)
        {
            string[] texts = new string[num];
            
            for (int i = 0; i < num; i++)
            {
                int index = _random.Next(0, _randomFontFamily.Count);

                string font = _randomFontFamily[index];
                texts[i] = font;
            }
            return texts;
        }
    }
}
