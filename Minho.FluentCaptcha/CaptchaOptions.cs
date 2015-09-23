using System.Drawing;

namespace Minho.FluentCaptcha
{
    /// <summary>
    /// 生成验证码图片的参数
    /// </summary>
    public class CaptchaOptions
    {
        public CaptchaOptions()
        {
            Width = 140;
            Height = 45;
            TextLength = 4;
            Background = NoiseLevel.Medium;
            FontFamily = new CaptchaFontFamily();
            Text = new CaptchaText();
            FontWarp = NoiseLevel.Low;;
            Colors = new CaptchaColor();
            Line = NoiseLevel.Medium;
            Broder = new CaptchaBorder
            {
                Color = Color.FromArgb(128, 128, 128),
                Style = BorderStyle.RoundRectangle,
                Radius = 2
            };
            GaussianDeviation = 0.66;
        }
        /// <summary>
        /// 图片高斯模糊的阀值,默认为 0
        /// </summary>
        public double GaussianDeviation { set; get; }
        /// <summary>
        /// 是否显示边框
        /// </summary>
        public CaptchaBorder Broder { set; get; }
        /// <summary>
        /// 字体间距级别
        /// </summary>
        public NoiseLevel FontWarp { set; get; }
        /// <summary>
        /// 图片宽度
        /// </summary>
        public int Width { set; get; }
        /// <summary>
        /// 图片高度
        /// </summary>
        public int Height { set; get; }
        /// <summary>
        /// 字符个数
        /// </summary>
        public int TextLength { set; get; }
        /// <summary>
        /// 背景噪点级别
        /// </summary>
        public NoiseLevel Background { set; get; }
        /// <summary>
        /// 线条噪点
        /// </summary>
        public NoiseLevel Line { set; get; }
        /// <summary>
        /// 字体集合
        /// </summary>
        public CaptchaFontFamily FontFamily { set; get; }
        /// <summary>
        /// 字符集合
        /// </summary>
        public CaptchaText Text { set; get; }
        /// <summary>
        /// 字体颜色集合
        /// </summary>
        public CaptchaColor Colors { set; get; }
    }
}
