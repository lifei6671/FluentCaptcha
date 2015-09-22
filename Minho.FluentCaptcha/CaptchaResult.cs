using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Minho.FluentCaptcha
{
    public class CaptchaResult : IDisposable
    {
        public Bitmap Bitmap { set; get; }
        public string Text { set; get; }

        public void Dispose()
        {
            Bitmap.Dispose();
        }
    }
}
