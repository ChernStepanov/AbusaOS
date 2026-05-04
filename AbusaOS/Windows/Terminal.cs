using AbusaOS.Utils;
using Cosmos.System;
using Cosmos.System.Graphics;
using Color = System.Drawing.Color;

namespace AbusaOS.Windows
{
    public class Terminal : Window
    {
        public Color curcol = Color.White;
        int rown = 50, coln = 50;
        string[] content;
        private Color[][] colors;
        int at = 0;
        bool canwrite = false;
        bool contentDirty = true;
        Bitmap cachedContent;

        public string pwd = @"0:\";

        void MarkDirty()
        {
            contentDirty = true;
            cachedContent = null;
            InvalidateCache();
        }

        void print_newline()
        {
            at++;

            if (at >= rown)
            {
                at = rown - 1;
                for (int i = 0; i < rown - 1; i++)
                {
                    content[i] = content[i + 1];
                    colors[i] = new Color[coln];
                    for (int j = 0; j < colors[i].Length; j++)
                    {
                        colors[i][j] = colors[i + 1][j];
                    }
                }
            }

            content[at] = "";
            for (int i = 0; i < coln; i++)
            {
                colors[at][i] = Color.Black;
            }

            MarkDirty();
        }

        void print_char(char c)
        {
            if (content[at].Length >= coln)
            {
                print_newline();
            }

            if (c == '\n')
            {
                print_newline();
                return;
            }

            content[at] += c;
            colors[at][content[at].Length - 1] = curcol;
            MarkDirty();
        }

        public void print_str(string str)
        {
            foreach (char c in str)
            {
                print_char(c);
            }
        }

        public Terminal() : base(300, 300, 700, 300, "Terminal", Kernel.defFont, false)
        {
            rown = (int)(h / Kernel.defFont.Height);
            coln = (int)(w / Kernel.defFont.Width);
            content = new string[rown];
            colors = new Color[rown][];

            for (int i = 0; i < rown; i++)
            {
                colors[i] = new Color[coln];
                content[i] = "";
            }
        }

        public void print_clear()
        {
            for (int i = 0; i < rown; i++)
            {
                colors[i] = new Color[coln];
                content[i] = "";
            }
            at = 0;
            curcol = Color.White;
            MarkDirty();
        }

        void input_prefix()
        {
            curcol = Color.Yellow;
            print_str("AbusaOS");
            curcol = Color.White;
            print_char('@');
            curcol = Color.Aqua;
            print_str(pwd);
            curcol = Color.White;
            print_str("$- ");
            curcol = Color.White;
        }

        void parse_input(string s)
        {
            AbusaCLI.ParseCommand(s, this);
            input_prefix();
            canwrite = true;
        }

        void RenderStaticContent(VBECanvas canv)
        {
            int contentX = x + 1;
            int contentY = y + window_titlebarsize + 1;
            int contentWidth = w - 1;
            int contentHeight = h - 1;

            if (contentDirty || cachedContent == null)
            {
                canv.DrawFilledRectangle(Color.Black, contentX, contentY, contentWidth, contentHeight);
                for (int i = 0; i < content.Length; i++)
                {
                    for (int j = 0; j < content[i].Length; j++)
                    {
                        canv.DrawChar(content[i][j], font, colors[i][j], contentX + (j * Kernel.defFont.Width), contentY + i * Kernel.defFont.Height);
                    }
                }

                cachedContent = RenderCache.Capture(canv, contentX, contentY, contentWidth, contentHeight);
                contentDirty = false;
            }
            else
            {
                canv.DrawImage(cachedContent, contentX, contentY);
            }
        }

        public override void Start(VBECanvas canv, int mX, int mY, bool mD, int dmX, int dmY)
        {
            curcol = Color.White;
            print_str("Welcome to the Command Line Interface for Abusa OS!\n\n");
            canwrite = true;
            input_prefix();
        }

        string inpstr = "";
        public override void Update(VBECanvas canv, int mX, int mY, bool mD, int dmX, int dmY)
        {
            base.Update(canv, mX, mY, mD, dmX, dmY);
            RenderStaticContent(canv);

            if (canwrite && myIndex == Kernel.activeIndex)
            {
                int cursorX = x + 1 + ((content[at].Length + inpstr.Length) * Kernel.defFont.Width);
                int cursorY = y + 1 + window_titlebarsize + at * Kernel.defFont.Height;
                canv.DrawChar('_', font, curcol, cursorX, cursorY);
                canv.DrawString(inpstr, font, curcol, x + 1 + (content[at].Length * Kernel.defFont.Width), cursorY);

                if (KeyboardManager.TryReadKey(out KeyEvent key))
                {
                    if (Kernel.TryHandleGlobalKey(key))
                    {
                        return;
                    }

                    if (key.Key == ConsoleKeyEx.Backspace)
                    {
                        if (inpstr.Length > 0)
                        {
                            inpstr = inpstr.Remove(inpstr.Length - 1);
                        }
                    }
                    else if (key.Key == ConsoleKeyEx.Enter)
                    {
                        print_str(inpstr);
                        print_newline();
                        canwrite = false;
                        parse_input(inpstr);
                        inpstr = "";
                    }
                    else if (key.KeyChar >= ' ' && inpstr.Length < coln - 4)
                    {
                        inpstr += key.KeyChar;
                    }
                }
            }
        }
    }
}
