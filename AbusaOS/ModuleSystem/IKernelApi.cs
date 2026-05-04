using AbusaOS.Windows;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System.Drawing;

namespace AbusaOS.ModuleSystem
{
    public interface IKernelApi
    {
        VBECanvas Canvas { get; }
        Font DefaultFont { get; }
        Color MainColor { get; }
        Color TextColorDark { get; }
        Color TextColorLight { get; }
        int ScreenWidth { get; }
        int ScreenHeight { get; }
        IWindowService Windows { get; }
        IMessageService Messages { get; }
        IInputService Input { get; }
        IFileSystemService FileSystem { get; }
        ILogService Log { get; }
        ITimerService Timer { get; }
        IThemeService Theme { get; }

        void OpenWindow(Window window);
        void ShowMessage(string content, string title = "Message", MsgType type = MsgType.Info);
        void CloseMainMenu();
    }
}
