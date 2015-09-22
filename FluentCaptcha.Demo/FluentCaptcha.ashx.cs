using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;
using Minho.FluentCaptcha;

namespace FluentCaptcha.Demo
{
    /// <summary>
    /// FluentCaptcha 的摘要说明
    /// </summary>
    public class FluentCaptcha : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            ICapatcha capatch = new Minho.FluentCaptcha.FluentCaptcha();
            using (var b = capatch.DrawImage())
            {
                b.Bitmap.Save(context.Response.OutputStream, ImageFormat.Gif);
            }
            context.Response.Cache.SetNoStore();
            context.Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);

            context.Response.ContentType = "image/gif";
            context.Response.StatusCode = 200;
            context.Response.StatusDescription = "OK";
            context.ApplicationInstance.CompleteRequest();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}