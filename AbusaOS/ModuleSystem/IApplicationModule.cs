using AbusaOS.Windows;
using Cosmos.System.Graphics;

namespace AbusaOS.ModuleSystem
{
    public interface IApplicationModule : IModule
    {
        Bitmap Icon { get; }
        Window CreateWindow(IKernelApi api);
    }
}
