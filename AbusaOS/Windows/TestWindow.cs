using AbusaOS.Controls;
using AbusaOS.ModuleSystem;
using Cosmos.System.Graphics;
using System.Drawing;

namespace AbusaOS.Windows
{
    internal class TestWindow : Window
    {
        public Label welcomeLabel;
        public Button clickButton;
        public Label clickLabel;
        int times;
        readonly IKernelApi api;

        public TestWindow() : this(new KernelApi())
        {
        }

        public TestWindow(IKernelApi api) : base(100, 100, 300, 300, "Test Window", api.DefaultFont, true)
        {
            this.api = api;
            welcomeLabel = new("Welcome to AbusaOS!", 20, 20, api.DefaultFont, Color.White);
            clickButton = new("Click Me!", 20, 50, Color.Green, api.DefaultFont, 10);
            clickLabel = new("You Clicked it 0 times", 20, 100, api.DefaultFont, Color.White);
            controls.Add(welcomeLabel); controls.Add(clickButton); controls.Add(clickLabel);
        }

        public override void Update(VBECanvas canv, int mX, int mY, bool mD, int dmX, int dmY)
        {
            base.Update(canv, mX, mY, mD, dmX, dmY);
            if (clickButton.clickedOnce)
            {
                times++;
                clickLabel.Text = $"You Clicked it {times} times";
            }
        }
    }
}
