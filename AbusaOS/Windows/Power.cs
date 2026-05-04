using AbusaOS.Controls;
using AbusaOS.ModuleSystem;
using Cosmos.System.Graphics;

namespace AbusaOS.Windows
{
    internal class Power : Window
    {
        public Button shutdownButton, restartButton;
        readonly IKernelApi api;

        public Power() : this(new KernelApi())
        {
        }

        public Power(IKernelApi api) : base(0, 0, 140, 90, "Power...", api.DefaultFont, false, false)
        {
            this.api = api;
            x = api.ScreenWidth / 2 - 140 / 2;
            y = api.ScreenHeight / 2 - 90 / 2;
            shutdownButton = new Button("Shut Down", 20, 20, api.TextColorDark, api.DefaultFont);
            restartButton = new Button("Reboot", 20, 50, api.TextColorDark, api.DefaultFont);

            controls.Add(restartButton);
            controls.Add(shutdownButton);
        }

        public override void Update(VBECanvas canv, int mX, int mY, bool mD, int dmX, int dmY)
        {
            base.Update(canv, mX, mY, mD, dmX, dmY);

            if (shutdownButton.clickedOnce)
            {
                Cosmos.System.Power.Shutdown();
            }

            if (restartButton.clickedOnce)
            {
                Cosmos.System.Power.Reboot();
            }
        }
    }
}
