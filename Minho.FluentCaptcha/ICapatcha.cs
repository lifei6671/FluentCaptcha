using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Minho.FluentCaptcha
{
    public interface ICapatcha : IDisposable
    {
        /// <summary>
        /// 图片选项
        /// </summary>
        CaptchaOptions Options { set; get; }
        /// <summary>
        /// 添加背景噪点
        /// </summary>
        /// <returns></returns>
        ICapatcha DrawBackgroud();
        /// <summary>
        /// 添加线条
        /// </summary>
        /// <returns></returns>
        ICapatcha DrawLine();
        /// <summary>
        /// 写文字
        /// </summary>
        /// <returns></returns>
        ICapatcha DrawText();
        /// <summary>
        /// 画图
        /// </summary>
        /// <returns></returns>
        CaptchaResult DrawImage();
        /// <summary>
        /// 添加边框
        /// </summary>
        /// <returns></returns>
        ICapatcha DrawBroder();
        /// <summary>
        /// 雾化图片
        /// </summary>
        /// <returns></returns>
        ICapatcha Atomized();
    }
}
