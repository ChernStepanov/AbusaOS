using AbusaOS.Windows;
using Cosmos.System.Graphics;
using System;

namespace AbusaOS.ModuleSystem
{
    public class ApiApplicationModule : IApplicationModule
    {
        readonly Func<IKernelApi, Window> constructor;

        public string Name { get; }
        public Bitmap Icon { get; }

        public ApiApplicationModule(string name, Func<IKernelApi, Window> constructor, Bitmap icon)
        {
            Name = name;
            this.constructor = constructor;
            Icon = icon;
        }

        public Window CreateWindow(IKernelApi api)
        {
            return constructor(api);
        }

        public void Initialize(IKernelApi api)
        {
        }
    }
}
