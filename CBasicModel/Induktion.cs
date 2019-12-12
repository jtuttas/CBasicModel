using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace CBasicModel
{
    class Induktion
    {
        Image onImage;
        Image offImage;
        Boolean state;
        Boolean force_state;
        int xPos;
        int yPos;


        public Induktion(int x, int y)
        {
            onImage = Image.FromFile("led1.gif");
            offImage = Image.FromFile("led0.gif");
            state = false;
            xPos = x;
            yPos = y;
        }


        public void check(int x, int w)
        {
            if (force_state)
            {
                state = true;
            }
            else
            {
                int c = xPos + onImage.Width / 2;
                if ((x + w) > xPos & (x - w / 2) < xPos) state = true;
                else state = false;
            }

        }

        public void setState(Boolean s)
        {
            if (force_state) state = true;
            else state = s;
        }

        public void forceState(Boolean s)
        {
            force_state = s;
        }

        public Boolean getState(){
                return state;
        }

        public void paint(Graphics g)
        {
            if (state) g.DrawImage(onImage, xPos, yPos);
            else g.DrawImage(offImage, xPos, yPos);
        }
    }
}
