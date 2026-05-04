using AbusaOS.Windows;
using Cosmos.System;
using System;
using System.Drawing;

namespace AbusaOS.ModuleSystem
{
    public interface IModule
    {
        string Name { get; }
        void Initialize(IKernelApi api);
    }

    public interface IWindowService
    {
        int ActiveIndex { get; }
        Window ActiveWindow { get; }
        void Open(Window window);
        void RequestClose(Window window);
        void SetActive(Window window);
        bool IsActive(Window window);
        void CloseMainMenu();
    }

    public interface IMessageService
    {
        void Show(string content, string title = "Message", MsgType type = MsgType.Info);
    }

    public interface IInputService
    {
        int MouseX { get; }
        int MouseY { get; }
        int DeltaX { get; }
        int DeltaY { get; }
        bool LeftMouseDown { get; }
        bool TryReadKey(out KeyEvent key);
        bool TryHandleGlobalKey(KeyEvent key);
    }

    public interface IFileSystemService
    {
        bool DirectoryExists(string path);
        bool FileExists(string path);
        string[] GetDirectories(string path);
        string[] GetFiles(string path);
        string ReadAllText(string path);
        string Combine(string path1, string path2);
        bool IsPathRooted(string path);
        string GetParent(string path);
    }

    public interface ILogService
    {
        void Info(string message);
        void Warning(string message);
        void Error(string message);
    }

    public interface ITimerService
    {
        DateTime Now { get; }
        void Sleep(int milliseconds);
    }

    public interface IThemeService
    {
        Color Background { get; }
        Color Main { get; }
        Color Highlight { get; }
        Color TextDark { get; }
        Color TextLight { get; }
    }
}
