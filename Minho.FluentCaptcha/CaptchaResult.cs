using System;
using System.Drawing;

namespace Minho.FluentCaptcha
{
    public class CaptchaResult : IDisposable
    {
        /// <summary>
        /// 图片对象
        /// </summary>
        public Bitmap Bitmap { set; get; }
        /// <summary>
        /// 图片上的文字
        /// </summary>
        public string Text { set; get; }

        public void Dispose()
        {
            Bitmap.Dispose();
        }
    }
}
