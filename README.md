# FluentCaptcha
简单验证码生成

面向对象编程实现验证码生成，自由控制验证码的各个属性。方法仅实现生成一个Bitmap对象，具体用在Winform还是WebForm或是mvc中可以自由写扩展。

使用如下：

CaptchaOptions options = new CaptchaOptions
{
    GaussianDeviation = 0.4
};
using(MemoryStream stream = new MemoryStream()) 
using (ICapatcha capatch = new Minho.FluentCaptcha.FluentCaptcha())
{
    capatch.Options = options;
    CaptchaResult captchaResult = capatch.DrawBackgroud().DrawLine().DrawText().Atomized().DrawBroder().DrawImage();
    using (captchaResult)
    {
        captchaResult.Bitmap.Save(stream, ImageFormat.Gif);
    }
}
