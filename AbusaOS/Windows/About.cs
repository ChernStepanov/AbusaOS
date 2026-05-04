using AbusaOS.Controls;
using AbusaOS.ModuleSystem;
using Cosmos.System.Graphics;
using IL2CPU.API.Attribs;

namespace AbusaOS.Windows
{
    internal class About : Window
    {
        [ManifestResourceStream(ResourceName = "AbusaOS.Resource.logotext.bmp")]
        static byte[] logotext;
        Bitmap logoImg;

        Label creds, creds1, creds2, creds3;
        ImageView logoView;
        readonly IKernelApi api;

        public About() : this(new KernelApi())
        {
        }

        public About(IKernelApi api) : base(300, 300, 500, 170, "System info", api.DefaultFont)
        {
            this.api = api;
            logo = Kernel.logo;
            logoImg = new Bitmap(logotext);
            creds = new("Created by", 20, 60, font, api.TextColorDark);
            creds1 = new("Abusa Development Group LLC", 40, 80, font, api.TextColorDark);
            creds2 = new("Credits: Iceik _Kot (Design)", 20, 100, font, api.TextColorDark);
            creds3 = new($"Version {Kernel.version}", 20, 130, font, api.TextColorDark);
            logoView = new(logoImg, 20, 10);
            controls.Add(creds);
            controls.Add(creds1);
            controls.Add(creds2);
            controls.Add(creds3);
            controls.Add(logoView);
        }
    }
}
