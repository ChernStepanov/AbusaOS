using AbusaOS.Windows;
using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.Drawing;
using System.IO;
using System.Threading;

namespace AbusaOS.ModuleSystem
{
    public class KernelApi : IKernelApi
    {
        readonly IWindowService windows = new KernelWindowService();
        readonly IMessageService messages = new KernelMessageService();
        readonly IInputService input = new KernelInputService();
        readonly IFileSystemService fileSystem = new KernelFileSystemService();
        readonly ILogService log = new KernelLogService();
        readonly ITimerService timer = new KernelTimerService();
        readonly IThemeService theme = new KernelThemeService();

        public VBECanvas Canvas => Kernel.canv;
        public Font DefaultFont => Kernel.defFont;
        public Color MainColor => Theme.Main;
        public Color TextColorDark => Theme.TextDark;
        public Color TextColorLight => Theme.TextLight;
        public int ScreenWidth => (int)Kernel.canv.Mode.Width;
        public int ScreenHeight => (int)Kernel.canv.Mode.Height;
        public IWindowService Windows => windows;
        public IMessageService Messages => messages;
        public IInputService Input => input;
        public IFileSystemService FileSystem => fileSystem;
        public ILogService Log => log;
        public ITimerService Timer => timer;
        public IThemeService Theme => theme;

        public void OpenWindow(Window window)
        {
            Windows.Open(window);
        }

        public void ShowMessage(string content, string title = "Message", MsgType type = MsgType.Info)
        {
            Messages.Show(content, title, type);
        }

        public void CloseMainMenu()
        {
            Windows.CloseMainMenu();
        }
    }

    internal class KernelWindowService : IWindowService
    {
        public int ActiveIndex => Kernel.activeIndex;
        public Window ActiveWindow => ActiveIndex >= 0 && ActiveIndex < Kernel.windows.Count ? Kernel.windows[ActiveIndex] : null;

        public void Open(Window window)
        {
            Kernel.OpenWindow(window);
        }

        public void RequestClose(Window window)
        {
            Kernel.RequestCloseWindow(window);
        }

        public void SetActive(Window window)
        {
            int index = Kernel.windows.IndexOf(window);
            if (index != -1)
            {
                Kernel.activeIndex = index;
            }
        }

        public bool IsActive(Window window)
        {
            return ActiveWindow == window;
        }

        public void CloseMainMenu()
        {
            Kernel.CloseMainMenu();
        }
    }

    internal class KernelMessageService : IMessageService
    {
        public void Show(string content, string title = "Message", MsgType type = MsgType.Info)
        {
            Kernel.ShowMessage(content, title, type);
        }
    }

    internal class KernelInputService : IInputService
    {
        public int MouseX => (int)MouseManager.X;
        public int MouseY => (int)MouseManager.Y;
        public int DeltaX => MouseManager.DeltaX;
        public int DeltaY => MouseManager.DeltaY;
        public bool LeftMouseDown => MouseManager.MouseState == MouseState.Left;

        public bool TryReadKey(out KeyEvent key)
        {
            return KeyboardManager.TryReadKey(out key);
        }

        public bool TryHandleGlobalKey(KeyEvent key)
        {
            return Kernel.TryHandleGlobalKey(key);
        }
    }

    internal class KernelFileSystemService : IFileSystemService
    {
        public bool DirectoryExists(string path) => Directory.Exists(path);
        public bool FileExists(string path) => File.Exists(path);
        public string[] GetDirectories(string path) => Directory.GetDirectories(path);
        public string[] GetFiles(string path) => Directory.GetFiles(path);
        public string ReadAllText(string path) => File.ReadAllText(path);
        public string Combine(string path1, string path2) => Path.Join(path1, path2);
        public bool IsPathRooted(string path) => Path.IsPathRooted(path);

        public string GetParent(string path)
        {
            DirectoryInfo parent = Directory.GetParent(path);
            return parent != null ? parent.FullName : null;
        }
    }

    internal class KernelLogService : ILogService
    {
        public void Info(string message) => System.Console.WriteLine("[INFO] " + message);
        public void Warning(string message) => System.Console.WriteLine("[WARN] " + message);
        public void Error(string message) => System.Console.WriteLine("[ERR] " + message);
    }

    internal class KernelTimerService : ITimerService
    {
        public DateTime Now => DateTime.Now;

        public void Sleep(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }
    }

    internal class KernelThemeService : IThemeService
    {
        public Color Background => Kernel.bgCol;
        public Color Main => Kernel.mainCol;
        public Color Highlight => Kernel.highlightCol;
        public Color TextDark => Kernel.textColDark;
        public Color TextLight => Kernel.textColLight;
    }
}
