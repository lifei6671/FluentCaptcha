using System.Drawing;
using System.Drawing.Drawing2D;

namespace Minho.FluentCaptcha
{
    public interface ICapatcha
    {
        /// <summary>
        /// 图片选项
        /// </summary>
        CaptchaOptions Options { set; get; }
        /// <summary>
        /// 添加噪点
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        ICapatcha AddNoise(Graphics g, Rectangle rect);
        /// <summary>
        /// 添加线条
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        ICapatcha AddLine(Graphics g, Rectangle rect);
        /// <summary>
        /// 写文字
        /// </summary>
        /// <param name="textPath"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        ICapatcha WarpText(GraphicsPath textPath, Rectangle rect);
        /// <summary>
        /// 画图
        /// </summary>
        /// <returns></returns>
        CaptchaResult DrawImage();
    }
}
