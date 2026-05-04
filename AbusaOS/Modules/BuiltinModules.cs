using AbusaOS.ModuleSystem;
using AbusaOS.Windows;

namespace AbusaOS.Modules
{
    internal static class BuiltinModules
    {
        public static void Register(ModuleManager manager, IKernelApi kernelApi)
        {
            manager.RegisterApplicationModule(new ApiApplicationModule("Calculator", api => new Calc(api), new Calc().logo), kernelApi);
            manager.RegisterApplicationModule(new ApiApplicationModule("Terminal", api => new Terminal(), new Terminal().logo), kernelApi);
            manager.RegisterApplicationModule(new ApiApplicationModule("Test Window", api => new TestWindow(api), new TestWindow().logo), kernelApi);
            manager.RegisterApplicationModule(new ApiApplicationModule("Input Field Test", api => new UITest(api), new UITest().logo), kernelApi);
            manager.RegisterApplicationModule(new ApiApplicationModule("About Abusa OS...", api => new About(api), new About().logo), kernelApi);
            manager.RegisterApplicationModule(new ApiApplicationModule("Power...", api => new Power(api), new Power().logo), kernelApi);
            manager.RegisterApplicationModule(new ApiApplicationModule("Explorer", api => new Explorer(api), new Explorer().logo), kernelApi);
        }
    }
}
