using AbusaOS.ModuleSystem;

namespace AbusaOS.Controls
{
    public abstract class Control
    {
        protected readonly IKernelApi Api;
        public int x, y;
        public bool Visible = true;
        public object Tag { get; set; }

        protected Control() : this(new KernelApi())
        {
        }

        protected Control(IKernelApi api)
        {
            Api = api;
        }

        public abstract void Update(int pX, int pY);
    }
}
