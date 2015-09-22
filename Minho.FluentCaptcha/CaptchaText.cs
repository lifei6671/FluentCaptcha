using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Minho.FluentCaptcha
{
    public class CaptchaText
    {
        private static readonly char[] DefaultRandomText =
        {
            'A', 'a', 'B', 'b', 'D', 'd', 'E', 'e', 'F', 'f', 'G', 'g', 'H', 'h', 'J', 'j', 'K', 'k', 'L', 'M', 'm', 'N',
            'n', 'P', 'p', 'Q', 'q', 'R', 'r', 'S', 's', 'T', 't', 'U', 'u', 'V', 'v', 'W', 'w', 'X', 'x', 'Y', 'y', 'Z',
            'z', '2', '3', '4', '5', '6', '7', '8', '9'
        };

        private readonly List<char> _randomText;
        private static int _randomSeed;
        private readonly Random _random;

        #region 构造方法

        static CaptchaText()
        {
            _randomSeed = 1;
        }
        
        /// <summary>
        /// 初始化 CaptchaText 对象的实例
        /// </summary>
        public CaptchaText() : this(DefaultRandomText) { }
        /// <summary>
        /// 初始化 CaptchaText 对象的实例
        /// </summary>
        /// <param name="texts"></param>
        public CaptchaText(IEnumerable<char> texts)
        {
            _randomText = new List<char>(texts);
            int seed = Environment.TickCount + _randomSeed;
            _random = new Random(seed);
            _randomSeed++;
        } 
        #endregion

        public ICollection<char> Texts { get { return new ReadOnlyCollection<char>(_randomText);} } 
        public CaptchaText Add(char text)
        {
            _randomText.Add(text);
            return this;
        }

        public CaptchaText AddRange(IEnumerable<char> texts)
        {
            _randomText.AddRange(texts);
            return this;
        }

        public CaptchaText Remove(char text)
        {
            _randomText.Remove(text);
            return this;
        }

        public CaptchaText Clear()
        {
            _randomText.Clear();
            return this;
        }

        public char NextText()
        {
            int index = _random.Next(0, _randomText.Count);
            char text = _randomText[index];
            return text;
        }

        public char[] NextText(int num)
        {
            char[] texts = new char[num];

            for (int i = 0; i < num; i++)
            {
                int index = _random.Next(0, _randomText.Count);
                texts[i] = _randomText[index];
            }
            return texts;
        }
    }
}
