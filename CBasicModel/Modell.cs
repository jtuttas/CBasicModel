using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace CBasicModel
{
    class Modell 
    {

        IOServer ios;
        public BooleanItem m_left= new BooleanItem(5, 4);
        public BooleanItem m_right = new BooleanItem(5, 5);
        public BooleanItem m_speed = new BooleanItem(5, 6);

        Motor motor = new Motor();
        public Induktion ileft = new Induktion(52, 260);
        public Induktion iright = new Induktion(330, 260);
        public Induktion imove = new Induktion(189, 270);

        Graphics g;
        Boolean tig;
        Boolean speedup;
        Boolean stromBegrenzung=true;
        int tigCounter;
        DateTime timeStamp = System.DateTime.Now;


        public int xPos;
        public int yPos;
        int orgx;
        int orgy;
        int dx = 0;
        int width;

        public Modell(Graphics gr)
        {
            g = gr;
            xPos = 152;
            yPos = 242;
            orgx = xPos;
            orgy = yPos;
            width = 30;
        }

        public void setIOServer(IOServer io)
        {
            ios = io;
            ios.add(m_left);
            ios.add(m_right);
            ios.add(m_speed);
        }

        public void reset()
        {
            Console.WriteLine("Modell RESET!");
            xPos = orgx;
            yPos = orgy;
            timeStamp = new DateTime(1970, 1, 1);
            if (!motor.isok())
            {
                motor.reset();
                if (m_left.getState()) this.left();
                if (m_right.getState()) this.right();
                if (m_speed.getState()) speedup = true;
                else speedup = false;
            }
        }

        public void setStrombegrenzung(Boolean b)
        {
            stromBegrenzung = b;
        }

        public void move()
        {
            //Console.WriteLine("Bnad Move Speed=" + dx);

            xPos += dx;
            if (speedup) xPos += dx;
            if (xPos < 0) xPos = 0;
            if (xPos + width > g.VisibleClipBounds.Width) xPos = (int)g.VisibleClipBounds.Width - width;
        }

        public void repaint()
        {
            motor.paint(g, 299, 82);
            if (speedup) motor.paint(g, 299, 82);
            ileft.check(xPos, width);
            iright.check(xPos, width);
            ileft.paint(g);
            iright.paint(g);

            if (ios!=null) {
                //Console.WriteLine("ios!=null!");
                if (ileft.getState()) ios.setBoolean(4, 4, true);
                else  ios.setBoolean(4, 4, false);
                if (iright.getState()) ios.setBoolean(4, 5, true);
                else ios.setBoolean(4, 5, false);
                if (tig) ios.setBoolean(4, 6, true);
                else ios.setBoolean(4, 6, false);
            }
            if (motor.isMoving() && motor.isok())
            {
                tigCounter--;
                if (tigCounter < 0)
                {
                    tig = !tig;
                    if (speedup) tigCounter = 1;
                    else tigCounter = 2;
                }
            }
            imove.setState(tig);
            imove.paint(g);

        }

        public void left()
        {
            Console.WriteLine("Modell Left!");
            if (stromBegrenzung && !m_left.force_state)
            {
                DateTime now = System.DateTime.Now;
                if (timeStamp.AddMilliseconds(800) > now)
                {
                    Console.WriteLine("LEFT: Kaput wg. Strombegrenzung timeStamp=" + timeStamp + " div=" + (now-timeStamp));
                    motor.setok(false);
                }
                else
                {
                    Console.WriteLine("Umschaltzeit=" + (now - timeStamp));
                }
            }
            timeStamp = System.DateTime.Now;
            if (dx == 0)
            {
                if (motor.isok())
                {
                    dx = -2;
                    motor.right();
                }
            }
            else
            {
                if (dx != -2)
                {
                    dx = 0;
                    motor.setok(false);
                }
            }
        }

        public void right()
        {
            Console.WriteLine("Modell Right!");
            if (stromBegrenzung && !m_right.force_state)
            {
                DateTime now = System.DateTime.Now;
                if (timeStamp.AddMilliseconds(800) > now)
                {
                    Console.WriteLine("RIGHT: Kaput wg. Strombegrenzung timeStamp=" + timeStamp + " div=" + (now - timeStamp));
                    motor.setok(false);
                }
                else
                {
                    Console.WriteLine("Umschaltzeit=" + (now - timeStamp));
                }
            }
            timeStamp = System.DateTime.Now;
            if (dx == 0)
            {
                if (motor.isok())
                {
                    dx = 2;
                    motor.left();
                }
            }
            else
            {
                if (dx != 2)
                {
                    dx = 0;
                    motor.setok(false);
                }
            }
        }

        public void stop()
        {
            Console.WriteLine("Modell Stop");
            dx = 0;
            motor.halt();
            m_left.setState(false);
            m_right.setState(false);
        }

        public void cont()
        {
            if (m_left.getState()) this.left();
            if (m_right.getState()) this.right();
            if (m_speed.getState()) speedup = true;
            else speedup = false;

        }

        public void outputchanged(BooleanItem bi)
        {
            Console.WriteLine("Modell Output changed:" + bi.ToString());
            if (bi == m_left)
            {
                if (bi.getState()) this.left();
                else this.stop();
            }
            else if (bi == m_right)
            {
                if (bi.getState()) this.right();
                else this.stop();
            }
            else if (bi == m_speed)
            {
                if (bi.getState()) speedup = true;
                else speedup = false;
                //Console.WriteLine("Speedup=" + speedup);
            }
        }
    }
}
