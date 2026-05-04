using AbusaOS.Controls;
using AbusaOS.ModuleSystem;
using AbusaOS.Utils;
using AbusaOS.Windows;
using Cosmos.Core.Memory;
using Cosmos.HAL.Audio;
using Cosmos.HAL.Drivers.Audio;
using Cosmos.System;
using Cosmos.System.Audio;
using Cosmos.System.Audio.IO;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using IL2CPU.API.Attribs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using Sys = Cosmos.System;

namespace AbusaOS
{
    public class Kernel : Sys.Kernel
    {
        public CosmosVFS fs = new();
        public static string version = "v0.3.0";
        const uint ScreenWidth = 1024;
        const uint ScreenHeight = 768;

        public static Color bgCol = Color.FromArgb(31, 32, 33);
        public static Color mainCol = Color.FromArgb(57, 64, 69);
        public static Color highlightCol = Color.FromArgb(255, 250, 250);
        public static Color textColLight = Color.FromArgb(59, 71, 79);
        public static Color textColDark = Color.FromArgb(200, 200, 204);
        public static VBECanvas canv;
        public static Font defFont;
        public static List<Window> windows = new();
        static List<Window> pendingCloseWindows = new();
        public static ModuleManager Modules { get; } = ModuleManager.Shared;
        static IKernelApi kernelApi = new KernelApi();
        List<Button> applicationsButtons = new();
        Button mainButton;
        public static int activeIndex = -1;
        bool mainBar;
        bool lastMouseDown;
        int framesSinceGc;
        string cachedTimeText = "";
        int cachedTimeWidth;
        static bool toggleMainMenuRequested;
        static bool closeMainMenuRequested;

        [ManifestResourceStream(ResourceName = "AbusaOS.Resource.blue.bmp")]
        static byte[] bgBytes;

        [ManifestResourceStream(ResourceName = "AbusaOS.Resource.cur.bmp")]
        static byte[] curBytes;

        [ManifestResourceStream(ResourceName = "AbusaOS.Resource.logo.bmp")]
        static byte[] logoBytes;

        [ManifestResourceStream(ResourceName = "AbusaOS.Resource.startup.wav")]
        static byte[] sampleAudioBytes;

        public static Bitmap bg, cursor, logo;

        void DrawTopbar()
        {
            NormalizeActiveIndex();
            canv.DrawFilledRectangle(bgCol, 0, 0, (int)canv.Mode.Width, 30);
            mainButton.Update(0, 0);
            string time = DateTime.Now.ToString("dddd, MMM d, yyyy. HH:mm");
            if (time != cachedTimeText)
            {
                cachedTimeText = time;
                cachedTimeWidth = defFont.Width * cachedTimeText.Length;
            }

            canv.DrawString(cachedTimeText, defFont, textColDark, (int)canv.Mode.Width - 20 - cachedTimeWidth, 10);
            canv.DrawString(activeIndex != -1 && windows.Count != 0 && activeIndex < windows.Count ? windows[activeIndex].title : "", defFont, textColDark, 170, 8);
        }

        void DrawMainBar()
        {
            canv.DrawFilledRectangle(bgCol, 10, 40, 300, applicationsButtons.Count * 50 + 40);
            canv.DrawString("Welcome to Abusa OS!", defFont, textColDark, 40, 70 - defFont.Height);
            for (int i = 0; i < applicationsButtons.Count; i++)
            {
                applicationsButtons[i].Update(10, 40);
                if (applicationsButtons[i].clickedOnce)
                {
                    Window instance = Modules.Applications[i].CreateWindow(kernelApi);
                    int mx = (int)MouseManager.X;
                    int my = (int)MouseManager.Y;
                    int dmx = MouseManager.DeltaX;
                    int dmy = MouseManager.DeltaY;
                    instance.Start(canv, mx, my, MouseManager.MouseState == MouseState.Left, dmx, dmy);
                    OpenWindow(instance);
                    mainBar = false;
                    break;
                }
            }
        }

        bool MouseInMainMenu(int mouseX, int mouseY)
        {
            return mainBar &&
                mouseX >= 10 && mouseX <= 310 &&
                mouseY >= 40 && mouseY <= 40 + applicationsButtons.Count * 50 + 40;
        }

        public static void CloseMainMenu()
        {
            closeMainMenuRequested = true;
        }

        static bool IsMetaKey(KeyEvent key)
        {
            string name = key.Key.ToString();
            return name == "LWin" || name == "RWin" ||
                name == "LeftWindows" || name == "RightWindows" ||
                name == "LeftMeta" || name == "RightMeta" ||
                name == "Super" || name == "Meta";
        }

        public static bool TryHandleGlobalKey(KeyEvent key)
        {
            if (!IsMetaKey(key))
            {
                return false;
            }

            toggleMainMenuRequested = true;
            return true;
        }

        public static void ShowMessage(string content, string title = "Message", MsgType type = MsgType.Info)
        {
            OpenWindow(new MsgWindow(title, content, type));
        }

        public static void OpenWindow(Window window)
        {
            if (window == null)
            {
                return;
            }

            windows.Add(window);
            activeIndex = windows.Count - 1;
            CloseMainMenu();
        }

        public static void RequestCloseWindow(Window window)
        {
            if (window != null && !pendingCloseWindows.Contains(window))
            {
                pendingCloseWindows.Add(window);
            }
        }

        void ProcessWindowCloseRequests()
        {
            if (pendingCloseWindows.Count == 0)
            {
                NormalizeActiveIndex();
                return;
            }

            for (int i = 0; i < pendingCloseWindows.Count; i++)
            {
                Window window = pendingCloseWindows[i];
                int removedIndex = windows.IndexOf(window);
                if (removedIndex == -1)
                {
                    continue;
                }

                windows.RemoveAt(removedIndex);
                if (activeIndex == removedIndex)
                {
                    activeIndex = windows.Count > 0 ? windows.Count - 1 : -1;
                }
                else if (activeIndex > removedIndex)
                {
                    activeIndex--;
                }
            }

            pendingCloseWindows.Clear();
            NormalizeActiveIndex();
        }

        void NormalizeActiveIndex()
        {
            if (windows.Count == 0)
            {
                activeIndex = -1;
            }
            else if (activeIndex >= windows.Count)
            {
                activeIndex = windows.Count - 1;
            }
        }

        public static void FatalErrorInternal(Exception e)
        {
            canv.Clear(Color.DarkSlateBlue);
            Thread.Sleep(10);
            canv.Display();
            string[] lines = {
                 $"--- Abusa OS {version}",
                 "",
                 $"The system has encountered an uncaught fatal exception",
                 "",
                 "Message: ",
                 e.ToString(),
                 "",
                 "Click any key to reboot"
            };

            int y = 10;

            foreach (string line in lines)
            {
                canv.DrawString(line, PCScreenFont.Default, Color.White, 10, y);
                y += PCScreenFont.Default.Height + 5;
                Thread.Sleep(10);
                canv.Display();
            }

            canv.Display();

            System.Console.ReadKey(true);
            Sys.Power.Reboot();
        }

        public static void HandleUncaughtError(Exception e)
        {
            FatalErrorInternal(e);
        }

        protected override void BeforeRun()
        {
            try
            {
                try { VFSManager.RegisterVFS(fs); } catch { }

                canv = new VBECanvas(new Mode(ScreenWidth, ScreenHeight, ColorDepth.ColorDepth32));

                bool throwTestError = false;
                if (throwTestError) { throw new Exception("This is a test exception"); }

                FontLoader fontLoader = new FontLoader();
                byte[] fontData = fontLoader.LoadFont();
                defFont = PCScreenFont.LoadFont(fontData);
                MouseManager.ScreenWidth = canv.Mode.Width;
                MouseManager.ScreenHeight = canv.Mode.Height;
                MouseManager.X = MouseManager.ScreenWidth / 2;
                MouseManager.Y = MouseManager.ScreenHeight / 2;

                bg = new Bitmap(bgBytes);
                cursor = new Bitmap(curBytes);
                logo = new Bitmap(logoBytes);

                mainButton = new Button("Main menu", 0, 0, mainCol, defFont, 7, logo);

                Modules.EnsureInitialized(kernelApi);

                for (int i = 0; i < Modules.Applications.Count; i++)
                {
                    applicationsButtons.Add(new Button(Modules.Applications[i].Name, 30, 40 + i * 50, mainCol, defFont, 10, Modules.Applications[i].Icon, 240));
                }

                try
                {
                    AudioMixer mixer = new();
                    MemoryAudioStream audioStream = new(new SampleFormat(AudioBitDepth.Bits16, 2, true), 48000, sampleAudioBytes);
                    AC97 driver = AC97.Initialize(bufferSize: 4096);
                    mixer.Streams.Add(audioStream);

                    AudioManager audioManager = new()
                    {
                        Stream = mixer,
                        Output = driver
                    };
                    audioManager.Enable();
                }
                catch (Exception ex)
                {
                    ShowMessage(ex.Message, "Audio Driver Initialization Error", MsgType.Error);
                }
            }
            catch (Exception e)
            {
                HandleUncaughtError(e);
            }
        }

        public void DrawCursor(uint x, uint y)
        {
            int xPos = (int)x;
            int yPos = (int)y;

            if (yPos > canv.Mode.Height - 16)
            {
                yPos = (int)canv.Mode.Height - 16;
            }

            canv.DrawImageAlpha(cursor, xPos, yPos);
        }

        protected override void Run()
        {
            try
            {
                canv.DrawImage(bg, 0, 0);
                DrawTopbar();

                if (mainButton.clickedOnce)
                {
                    mainBar = !mainBar;
                }

                int mouseX = (int)MouseManager.X;
                int mouseY = (int)MouseManager.Y;
                bool mouseDown = MouseManager.MouseState == MouseState.Left;
                int deltaX = MouseManager.DeltaX;
                int deltaY = MouseManager.DeltaY;

                bool activeWindowConsumesKeyboard = activeIndex >= 0 &&
                    activeIndex < windows.Count &&
                    windows[activeIndex] is Terminal;

                if (!activeWindowConsumesKeyboard &&
                    KeyboardManager.TryReadKey(out KeyEvent key))
                {
                    TryHandleGlobalKey(key);
                }

                if (toggleMainMenuRequested)
                {
                    mainBar = !mainBar;
                    toggleMainMenuRequested = false;
                }

                for (int i = 0; i < windows.Count; i++)
                {
                    if (i != activeIndex)
                    {
                        windows[i].Update(canv, mouseX, mouseY, mouseDown, deltaX, deltaY);
                    }
                }

                ProcessWindowCloseRequests();

                if (activeIndex != -1 && windows.Count > 0)
                {
                    windows[activeIndex].Update(canv, mouseX, mouseY, mouseDown, deltaX, deltaY);
                }

                ProcessWindowCloseRequests();

                if (closeMainMenuRequested)
                {
                    mainBar = false;
                    closeMainMenuRequested = false;
                }

                if (mainBar)
                {
                    DrawMainBar();
                }

                if (mouseDown && !lastMouseDown && mainBar &&
                    !MouseInMainMenu(mouseX, mouseY) &&
                    !mainButton.Hovered(mouseX, mouseY))
                {
                    mainBar = false;
                    activeIndex = -1;
                }

                lastMouseDown = mouseDown;

                DrawCursor(MouseManager.X, MouseManager.Y);
                canv.Display();

                framesSinceGc++;
                if (framesSinceGc >= 120)
                {
                    Heap.Collect();
                    framesSinceGc = 0;
                }
            }
            catch (Exception e)
            {
                HandleUncaughtError(e);
            }
        }
    }
}
