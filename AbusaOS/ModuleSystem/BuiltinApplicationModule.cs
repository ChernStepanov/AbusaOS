using AbusaOS.Windows;
using Cosmos.System.Graphics;
using System;

namespace AbusaOS.ModuleSystem
{
    public class BuiltinApplicationModule : IApplicationModule
    {
        readonly Func<Window> constructor;

        public string Name { get; }
        public Bitmap Icon { get; }

        public BuiltinApplicationModule(string name, Func<Window> constructor, Bitmap icon)
        {
            Name = name;
            this.constructor = constructor;
            Icon = icon;
        }

        public Window CreateWindow(IKernelApi api)
        {
            return constructor();
        }

        public void Initialize(IKernelApi api)
        {
        }
    }
}
