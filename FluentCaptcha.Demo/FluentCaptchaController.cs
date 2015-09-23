using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Minho.FluentCaptcha;

namespace FluentCaptcha.Demo
{
    public class FluentCaptchaController : Controller
    {
        public ActionResult FluentCaptcha()
        {
            CaptchaOptions options = new CaptchaOptions
            {
                GaussianDeviation = 0.4
            };
            MemoryStream stream = new MemoryStream();
            using (ICapatcha capatch = new Minho.FluentCaptcha.FluentCaptcha())
            {
                capatch.Options = options;
                CaptchaResult captchaResult = capatch.DrawBackgroud().DrawLine().DrawText().Atomized().DrawBroder().DrawImage();
                using (captchaResult)
                {
                    captchaResult.Bitmap.Save(stream, ImageFormat.Gif);
                    
                }
            }
            return File(stream.ToArray(), "image/gif");
        } 
    }
}
