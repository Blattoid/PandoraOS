using System.Drawing;
using Graphics = Cosmos.System.Graphics;

namespace Pandora.Applets
{
    class SVGAGraphics
    {
        Graphics.SVGAIIScreen screen;
        void Init()
        {
            screen = new Graphics.SVGAIIScreen();
            screen.Clear();
        }
        public void demo(string[] maincommand)
        {
            Init();

            Graphics.Pen pen;
            int colour = 0;

            //colour squares
            for (int v = 0; v <= 2; v += 1)
            {
                for (int w = 0; w <= 2; w += 1)
                {
                    for (int y = 0; y <= 254; y += 3)
                    {
                        for (int x = 0; x <= 254; x += 3)
                        {
                            Color pixelcolour = new Color();
                            pixelcolour = Color.FromArgb(x, y, 44 * (v + w));
                            pen = new Graphics.Pen(pixelcolour);

                            draw_chunky_pixel(x + 260 * w, y + 260 * v, pen);

                            colour++;
                            if (colour == 3) colour = 0;
                        }
                    }
                }
            }
            //eyes
            screen.Clear();
            pen = new Graphics.Pen(Color.White);
            for (int y = 50; y <= 700; y += 42)
            {
                for (int x = 100; x <= 1000; x += 102)
                {
                    Graphics.Point pixel = new Graphics.Point(x, y);
                    for (int i = 20; i <= 50; i += 3) screen.DrawEllipse(pen, pixel, i, 20);
                }
            }
            //triangles
            screen.CreateCursor();
            screen.SetCursor(true, 200, 300);
        }
        private void draw_chunky_pixel(int x, int y, Graphics.Pen pen)
        {
            for (int y_delta = y; y_delta <= y + 3; y_delta++)
            {
                for (int x_delta = x; x_delta <= x + 3; x_delta++)
                {
                    Graphics.Point pixel = new Graphics.Point(x_delta, y_delta);
                    screen.DrawPoint(pen, pixel);
                }
            }
        }
    }
}
