using System.Collections.Generic;
using System.Linq;

namespace AbusaOS.ModuleSystem
{
    public class ModuleRegistry
    {
        readonly List<IApplicationModule> applications = new();

        public int Count => applications.Count;

        public IApplicationModule this[int index] => applications[index];

        public void Register(IApplicationModule module)
        {
            if (module != null && applications.All(application => application.Name != module.Name))
            {
                applications.Add(module);
            }
        }
    }
}
