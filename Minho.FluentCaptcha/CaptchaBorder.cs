using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Minho.FluentCaptcha
{
    public class CaptchaBorder
    {
        public CaptchaBorder()
        {
            Color = Color.FromArgb(128, 128, 128);
            Style = BorderStyle.RoundRectangle;
            Radius = 2;
        }
        /// <summary>
        /// 边框颜色
        /// </summary>
        public Color Color { set; get; }
        /// <summary>
        /// 边框样式
        /// </summary>
        public BorderStyle Style { set; get; }
        /// <summary>
        /// 圆角边框的圆角半径
        /// </summary>
        public int Radius { set; get; }
    }
    /// <summary>
    /// 边框样式
    /// </summary>
    public enum BorderStyle
    {
        /// <summary>
        /// 无边框
        /// </summary>
        None = 0,
        /// <summary>
        /// 矩形边框
        /// </summary>
        Rectangle = 1,
        /// <summary>
        /// 圆角边框
        /// </summary>
        RoundRectangle = 2
    }
}
