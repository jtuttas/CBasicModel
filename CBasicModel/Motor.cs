using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace CBasicModel
{
    class Motor
    {
        Image[] img = new Image[6];
        Image kaputt;
        int counter;

        Boolean ok;

        int state;

    public void left(){
        state = 1;
    }

    public void right() {
        state = -1;
    }

    public void halt(){
        state = 0;
    }

    public void setok(Boolean s){
        ok = s;
    }

    public Boolean isok(){
        return ok;
    }


    public Motor() {
        img[0] = Image.FromFile("image1.gif");
        img[1] = Image.FromFile("image2.gif");
        img[2] = Image.FromFile("image3.gif");
        img[3] = Image.FromFile("image4.gif");
        img[4] = Image.FromFile("image5.gif");
        img[5] = Image.FromFile("image6.gif");
        kaputt = Image.FromFile("image0.gif");
        counter = 0;
        ok = true;
        state = 0;
    }

        public Boolean isMoving()
        {
            return state != 0;
        }

    public void reset(){
        ok = true;
        state = 0;
        counter = 0;
    }

    public void paint(Graphics g, int  x , int y) {
        if (ok) {
            g.DrawImage(img[counter], x, y);
            counter = counter + state;
            if (counter > 5) counter = 0;
            if (counter < 0) counter = 5;
        }
        else g.DrawImage(kaputt, x, y);
    }

    }
}
