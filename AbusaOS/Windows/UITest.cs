using AbusaOS.Controls;
using AbusaOS.ModuleSystem;
using Cosmos.System.Graphics;

namespace AbusaOS.Windows
{
    internal class UITest : Window
    {
        public InputField field1;
        readonly IKernelApi api;

        public UITest() : this(new KernelApi())
        {
        }

        public UITest(IKernelApi api) : base(100, 100, 200, 100, "Input Field Test", api.DefaultFont, true, false)
        {
            this.api = api;
            field1 = new(20, 20, 100, font, 5);

            controls.Add(field1);
        }

        public override void Update(VBECanvas canv, int mX, int mY, bool mD, int dmX, int dmY)
        {
            base.Update(canv, mX, mY, mD, dmX, dmY);
            if (resizing)
            {
                field1.width = w - 40;
            }
            if (field1.submittedOnce)
            {
                api.ShowMessage("Your Input is: " + field1.Value, "Test", MsgType.Info);
            }
        }
    }
}
