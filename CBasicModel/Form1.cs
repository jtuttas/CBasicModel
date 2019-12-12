using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using NDde.Client;

namespace CBasicModel
{
    public partial class Form1 : Form , PlcListener
    {
        private delegate void setVerbindenTextHandler();

        Image s7Image;
        Image fluidImage;
        IOServer ios;
        Modell modell;
        BooleanItem plc_h1 = new BooleanItem(5, 0);
        BooleanItem plc_h2 = new BooleanItem(5, 1);
        BooleanItem plc_h3 = new BooleanItem(5, 2);
        BooleanItem plc_h4 = new BooleanItem(5, 3);
        Boolean selected = false;
        int xSelected;

        Image sopenImage;
        Image sclosedImage;
        Image h0Image;
        Image h1Image;
        Image k0Image;
        Image k1Image;
        Image led0Image;
        Image led1Image;
        Boolean s0Pressed=true;
        Boolean s1Pressed;
        Boolean resetPressed;
        Boolean handSteuerung=true;

        Image pultRot0Image;
        Image pultRot1Image;
        Image pultGruen0Image;
        Image pultGruen1Image;
        Image pultWeiss0Image;
        Image pultWeiss1Image;
        Image pultS0Image;
        Image pultS1Image;
        Image pultT0Image;
        Image pultT1Image;
        Image pultT0rotImage;
        Image pultT1rotImage;
        Image pultT0gruenImage;
        Image pultT1gruenImage;


        public Form1()
        {
            s7Image = Image.FromFile("s7panel.jpg");
            fluidImage = Image.FromFile("PanelFesto.jpg");
            InitializeComponent();
            modell = new Modell(bg.CreateGraphics());
            sopenImage = Image.FromFile("sopen.gif");
            sclosedImage = Image.FromFile("sclosed.gif");
            h0Image = Image.FromFile("h0.gif");
            h1Image = Image.FromFile("h1.gif");
            k0Image = Image.FromFile("k0.gif");
            k1Image = Image.FromFile("k1.gif");
            led0Image = Image.FromFile("led_0.gif");
            led1Image = Image.FromFile("led_1.gif");

            // Bilder f. Bedienpult

            pultRot0Image = Image.FromFile("rot0.png");
            pultRot1Image = Image.FromFile("rot1.png");
            pultGruen0Image = Image.FromFile("gruen0.png");
            pultGruen1Image = Image.FromFile("gruen1.png");
            pultWeiss0Image = Image.FromFile("weiss0.png");
            pultWeiss1Image = Image.FromFile("weiss1.png");
            pultS0Image = Image.FromFile("stl.png");
            pultS1Image = Image.FromFile("str.png");
            pultT0Image = Image.FromFile("t0.png");
            pultT1Image = Image.FromFile("t1.png");
            pultT0rotImage = Image.FromFile("t0rot.png");
            pultT1rotImage = Image.FromFile("t1rot.png");
            pultT0gruenImage = Image.FromFile("t0gruen.png");
            pultT1gruenImage = Image.FromFile("t1gruen.png");

            this.pictureBox2.Image = pultS0Image;
            this.pictureBox3.Image = pultT0rotImage;
            this.pictureBox4.Image = pultT0gruenImage;
            this.pictureBox5.Image = pultT0Image;
            
            
        }

        public void stateChanged(String s)
        {
            status.Text = s;
        }
        
        public void outputChanged(BooleanItem bi)
        {
            Console.WriteLine("Form1: Output Chaged:"+bi.ToString());
            //status.Text="Output changed:"+bi.ToString();
            if (bi == plc_h1)
            {
                this.setImage(pictureBox6, pultRot1Image, pultRot0Image, bi.getState());
            }
            else if (bi == plc_h2)
            {
                this.setImage(pictureBox7, pultGruen1Image, pultGruen0Image, bi.getState());
            }
            else if (bi == plc_h3)
            {
                this.setImage(pictureBox8, pultWeiss1Image, pultWeiss0Image, bi.getState());
            }
            else if (bi == plc_h4)
            {
                this.setImage(pictureBox9, pultWeiss1Image, pultWeiss0Image, bi.getState());
            }
            else
            {
                if (modell!=null) modell.outputchanged(bi);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (ios!=null) ios.disconnect();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Console.WriteLine("ticke");
            modell.move();
            modell.repaint();
            if (!selected)
            {
                mat.Location = new Point(modell.xPos + bg.Location.X, modell.yPos + bg.Location.Y);
            }
            setImage(sb0, sclosedImage, sopenImage, modell.imove.getState());
            setImage(sb1, sclosedImage, sopenImage, modell.ileft.getState());
            setImage(sb2, sclosedImage, sopenImage, modell.iright.getState());
            setImage(pbs0, sclosedImage, sopenImage, s0Pressed);
            setImage(pbs1, sclosedImage, sopenImage, s1Pressed);
            setImage(pbreset, sclosedImage, sopenImage, resetPressed);
            setImage(pbhand, sclosedImage, sopenImage, handSteuerung);

            setImage(pbh1, h1Image, h0Image, plc_h1.getState());
            setImage(pbh2, h1Image, h0Image, plc_h2.getState());
            setImage(pbh3, h1Image, h0Image, plc_h3.getState());
            setImage(pbh4, h1Image, h0Image, plc_h4.getState());
            setImage(pbkl, k1Image, k0Image, modell.m_left.getState());
            setImage(pbkr, k1Image, k0Image, modell.m_right.getState());
            setImage(speedpb, k1Image, k0Image, modell.m_speed.getState());

            setImage(l52, led1Image, led0Image, modell.imove.getState());
            setImage(l51, led1Image, led0Image, modell.ileft.getState());
            setImage(l53, led1Image, led0Image, modell.iright.getState());
            setImage(ii42, led1Image, led0Image, s0Pressed);
            setImage(l43, led1Image, led0Image, s1Pressed);
            setImage(I44, led1Image, led0Image, resetPressed);
            setImage(I45, led1Image, led0Image, handSteuerung);
            setImage(l42, led1Image, led0Image, plc_h1.getState());
            setImage(l83, led1Image, led0Image, plc_h2.getState());
            setImage(l84, led1Image, led0Image, plc_h3.getState());
            setImage(l45, led1Image, led0Image, plc_h4.getState());
            setImage(l91, led1Image, led0Image, modell.m_left.getState());
            setImage(l90, led1Image, led0Image, modell.m_right.getState());
            setImage(i92, led1Image, led0Image, modell.m_speed.getState());
        }

        private void setImage(PictureBox pb, Image onImage, Image offImage, Boolean state)
        {
            if (state) pb.Image = onImage;
            else pb.Image = offImage;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            modell.reset();
        }

        private void s0_MouseDown(object sender, MouseEventArgs e)
        {
            if (ios != null) ios.setBoolean(4, 0, true);
            s0Pressed = true;
        }

        private void s0_MouseUp(object sender, MouseEventArgs e)
        {
            if (ios != null) ios.setBoolean(4, 0, false);
            s0Pressed = false;
        }

        private void s1_MouseDown(object sender, MouseEventArgs e)
        {
            if (ios != null) ios.setBoolean(4, 1, true);
            s1Pressed = true;

        }

        private void s1_MouseUp(object sender, MouseEventArgs e)
        {
            if (ios != null) ios.setBoolean(4, 1, false);
            s1Pressed = false;

        }

        private void ghost(Boolean b)
        {
            modbus.Enabled = b;
            codesys.Enabled = b;
            codesysPfad.Enabled = b;
            fluidsim.Enabled = b;
            fludsimOutMarke.Enabled = b;
            fluidsimInMarke.Enabled = b;
            fluidsimServer.Enabled = b;
            fluidsimTopic.Enabled = b;
            ipadr.Enabled = b;
            button1.Enabled = b;

        }

        private void verbinden_Click(object sender, EventArgs e)
        {
            if (ios!=null && ios.isConnected())
            {
                ios.disconnect();
                verbinden.Text = "Verbinden";
                status.Text = "Verbindung getrennt";
                this.statusStrip1.BackColor = Color.White;
                verbinden.BackColor = Color.PaleGreen;
                modell.stop();
                ghost(true);

            }
            else
            {
                if (modbus.Checked)
                {
                    verbinden.Enabled = false;
                    this.statusStrip1.BackColor = Color.Yellow;

                    status.Text = "Try to connect to Modbus Server!";
                    this.Update();
                    ios = new IOServerModbus();
                    ios.setListener(this);

                    bool res = ios.connect();
                    verbinden.Enabled = true;
                    if (res)
                    {
                        ios.add(plc_h1);
                        ios.add(plc_h2);
                        ios.add(plc_h3);
                        ios.add(plc_h4);
                        ios.setBoolean(4, 0, s0Pressed);
                        ios.setBoolean(4, 3, handSteuerung);
                        modell.setIOServer(ios);
                        verbinden.Text = "Trennen";
                        status.Text = "Verbunden!";
                        this.statusStrip1.BackColor = Color.Green;
                        verbinden.BackColor = Color.Red;
                        ghost(false);
                        modell.cont();
                        tabControl1.SelectedIndex = 0;
                    }
                    else
                    {
                        status.Text = "Verbindung fehlgeschlagen!";
                        this.statusStrip1.BackColor = Color.Red;

                    }
                }
                else if (codesys.Checked)
                {
                    verbinden.Enabled = false;
                    this.statusStrip1.BackColor = Color.Yellow;

                    status.Text = "Try to connect to CoDeSys DDE Server!";
                    this.Update();
                    ios = new IOServerDDECodeSys(new DdeClient("CODESYS", codesysPfad.Text));
                    ios.setListener(this);
                    
                    bool res = ios.connect();
                    verbinden.Enabled = true;
                    if (res)
                    {
                        ios.add(plc_h1);
                        ios.add(plc_h2);
                        ios.add(plc_h3);
                        ios.add(plc_h4);
                        ios.setBoolean(4, 2, s0Pressed);
                        ios.setBoolean(4, 5, handSteuerung);

                        modell.setIOServer(ios);
                        verbinden.Text = "Trennen";
                        status.Text = "Verbunden!";
                        this.statusStrip1.BackColor = Color.Green;
                        verbinden.BackColor = Color.Red;
                        ghost(false);
                        //modell.cont();
                        tabControl1.SelectedIndex = 0;
                    }
                    else
                    {
                        status.Text = "Verbindung fehlgeschlagen!";
                        this.statusStrip1.BackColor = Color.Red;

                    }
                }
                else if (fluidsim.Checked)
                {
                    verbinden.Enabled = false;
                    this.statusStrip1.BackColor = Color.Yellow;

                    status.Text = "Try to connect to FluidSim DDE Server!";
                    this.Update();
                    ios = new IOServerDDEFluidSim(new DdeClient(fluidsimServer.Text, fluidsimTopic.Text), fludsimOutMarke.Text, fluidsimInMarke.Text);
                    ios.setListener(this);
                    bool res = ios.connect();
                    verbinden.Enabled = true;
                    if (res)
                    {
                        ios.add(plc_h1);
                        ios.add(plc_h2);
                        ios.add(plc_h3);
                        ios.add(plc_h4);
                        ios.setBoolean(4, 2, s0Pressed);
                        ios.setBoolean(4, 5, handSteuerung);

                        modell.setIOServer(ios);
                        verbinden.Text = "Trennen";
                        status.Text = "Verbunden!";
                        this.statusStrip1.BackColor = Color.Green;

                        verbinden.BackColor = Color.Red;
                        ghost(false);
                        //modell.cont();
                        tabControl1.SelectedIndex = 0;
                    }
                    else
                    {
                        status.Text = "Verbindung fehlgeschlagen!";
                        this.statusStrip1.BackColor = Color.Red;

                    }
                }
                else
                {
                    /*
                    status.Text = "Try to connect to S7 via Profinet";
                    verbinden.Enabled = false;
                    this.statusStrip1.BackColor = Color.Yellow;

                    axS7TCP1.CP_IP = ipadr.Text;
                    axS7TCP1.Tzeit = 80000;
                    axS7TCP1.TsapE = "1.0";
                    if (comboBox1.Text == "S7-300")
                    {
                        axS7TCP1.TsapF = "3.2";
                    }
                    else
                    {
                        axS7TCP1.TsapF = "3.3";
                    }
                    status.Text = "verbinde...";
                    ios = new IOServerS7TCP(axS7TCP1);
                    ios.setListener(this);
                    ios.add(plc_h1);
                    ios.add(plc_h2);
                    ios.add(plc_h3);
                    ios.add(plc_h4);
                    ios.setBoolean(4, 2, s0Pressed);
                    ios.setBoolean(4, 5, handSteuerung);

                    res = ios.connect();
                    verbinden.Enabled = true;
                    if (res)
                    {
                        modell.setIOServer(ios);
                        verbinden.Text = "Trennen";
                        status.Text = "Verbunden!";
                        this.statusStrip1.BackColor = Color.Green;

                        verbinden.BackColor = Color.Red;
                        modell.cont();
                        ghost(false);
                        tabControl1.SelectedIndex = 0;
                    }
                    else
                    {
                        status.Text = "Verbindung fehlgeschlagen!";
                        verbinden.BackColor = Color.PaleGreen;
                        this.statusStrip1.BackColor = Color.Red;

                    }
                    */
                }
            }
        }

        private void mat_MouseDown(object sender, MouseEventArgs e)
        {
            selected = true;
            xSelected = e.X;
            mat.BackColor = Color.Yellow;
            //Console.WriteLine("Mouse Down at x=" + e.X);
            mat.Location = new Point(bg.Location.X + modell.xPos, modell.yPos - 5 + bg.Location.Y);
            //modell.repaint();
            mat.Refresh();
        }

        private void mat_MouseUp(object sender, MouseEventArgs e)
        {
            modell.xPos = mat.Location.X-bg.Location.X;
            modell.yPos = mat.Location.Y + 5 -bg.Location.Y;
            selected = false;
            mat.BackColor = Color.Gray;
        }

        private void mat_MouseMove(object sender, MouseEventArgs e)
        {
            if (selected)
            {
                //Console.WriteLine("x=" + e.X);
                int a = modell.xPos + e.X - xSelected;
                if (a< bg.Width && a>0)
                {
                    modell.xPos = a;
                    mat.Location = new Point(bg.Location.X + modell.xPos, modell.yPos - 5 + bg.Location.Y);
                }
                else 
                {
                    mat.Location = new Point(bg.Location.X + modell.xPos, modell.yPos - 5 + bg.Location.Y);
                }
                //modell.repaint();
            }

        }

        private void sb1_MouseDown(object sender, MouseEventArgs e)
        {
            modell.ileft.forceState(true);
        }

        private void sb1_MouseUp(object sender, MouseEventArgs e)
        {
            modell.ileft.forceState(false);
        }

        private void sb2_MouseDown(object sender, MouseEventArgs e)
        {
            modell.iright.forceState(true);
        }

        private void sb2_MouseUp(object sender, MouseEventArgs e)
        {
            modell.iright.forceState(false);
        }

        private void sb0_MouseDown(object sender, MouseEventArgs e)
        {
            modell.imove.forceState(true);
            if (ios != null) ios.setBoolean(4, 6, true);

        }

        private void sb0_MouseUp(object sender, MouseEventArgs e)
        {
            modell.imove.forceState(false);
            if (ios != null) ios.setBoolean(4, 6, false);

        }

        private void pbs0_MouseDown(object sender, MouseEventArgs e)
        {
            if (ios != null) ios.setBoolean(4, 0, false);
            s0Pressed = false;
            setImage(pictureBox3, pultT1rotImage, pultT0rotImage, true);
        }

        private void pbs1_MouseDown(object sender, MouseEventArgs e)
        {
            if (ios!=null) ios.setBoolean(4, 1, true);
            s1Pressed = true;
            setImage(pictureBox4, pultT1gruenImage, pultT0gruenImage, true);

        }

        private void pbs0_MouseUp(object sender, MouseEventArgs e)
        {
            if (ios != null) ios.setBoolean(4, 0, true);
            s0Pressed = true;
            setImage(pictureBox3, pultT1rotImage, pultT0rotImage, false);
        }

        private void pbs1_MouseUp(object sender, MouseEventArgs e)
        {
            if (ios != null) ios.setBoolean(4, 1, false);
            s1Pressed = false;
            setImage(pictureBox4, pultT1gruenImage, pultT0gruenImage, false);
        }

        private void pbh2_MouseDown(object sender, MouseEventArgs e)
        {
            this.plc_h2.forceState(true);
            this.outputChanged(plc_h2);
        }

        private void pbh2_MouseUp(object sender, MouseEventArgs e)
        {
            this.plc_h2.forceState(false);
            this.outputChanged(plc_h2);
        }

        private void pbh3_MouseDown(object sender, MouseEventArgs e)
        {
                this.plc_h3.forceState(true);
                this.outputChanged(plc_h3);
        }

        private void pbh3_MouseUp(object sender, MouseEventArgs e)
        {
            this.plc_h3.forceState(false);
            this.outputChanged(plc_h3);
        }

        private void pbh4_MouseDown(object sender, MouseEventArgs e)
        {
            this.plc_h4.forceState(true);
            this.outputChanged(plc_h4);
        }

        private void pbh4_MouseUp(object sender, MouseEventArgs e)
        {
            this.plc_h4.forceState(false);
            this.outputChanged(plc_h4);
        }


        private void pbkl_MouseDown(object sender, MouseEventArgs e)
        {
            modell.m_left.forceState(true);
            this.outputChanged(modell.m_left);
        }

        private void pbkl_MouseUp(object sender, MouseEventArgs e)
        {
            modell.m_left.forceState(false);
            this.outputChanged(modell.m_left);
        }

        private void pbkr_MouseDown(object sender, MouseEventArgs e)
        {
            modell.m_right.forceState(true);
            this.outputChanged(modell.m_right);
        }

        private void pbkr_MouseUp(object sender, MouseEventArgs e)
        {
            modell.m_right.forceState(false);
            this.outputChanged(modell.m_right);
        }

        private void setVerbindenText()
        {
            verbinden.Text = "Verbinden";
            ghost(true);
            this.statusStrip1.BackColor = Color.Red;

        }

        public void lostConnection(String s)
        {
            Console.WriteLine("Lost Connection!");
            verbinden.BackColor = Color.LightGreen;
            status.Text = "Lost connection !"+s;
            ios.disconnect();
            if (this.verbinden.InvokeRequired)
            {
                Console.WriteLine("Invoke!");
                verbinden.BeginInvoke(new setVerbindenTextHandler(setVerbindenText));
            }
            else
            {
                verbinden.Text = "Verbinden";
                ghost(true);
                this.statusStrip1.BackColor = Color.Red;

            }

        }

        private void fluidsim_CheckedChanged(object sender, EventArgs e)
        {
            pictureBox1.Image = this.fluidImage;
        }

        private void codesys_CheckedChanged(object sender, EventArgs e)
        {
            pictureBox1.Image = this.s7Image;

        }

        private void s7_CheckedChanged(object sender, EventArgs e)
        {
            pictureBox1.Image = this.s7Image;

        }

        private void plcsim_CheckedChanged(object sender, EventArgs e)
        {
            pictureBox1.Image = this.s7Image;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            codesysPfad.Text =  openFileDialog1.FileName;
        }

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (pictureBox2.Image == this.pultS0Image)
            {
                this.setImage(pictureBox2, pultS1Image, pultS0Image, true);
                if (ios != null) ios.setBoolean(4, 3, false);
                handSteuerung = false;

            }
            else
            {
                this.setImage(pictureBox2, pultS1Image, pultS0Image, false);
                if (ios != null) ios.setBoolean(4, 3, true);
                handSteuerung = true;
            }

        }

        private void pictureBox3_MouseDown(object sender, MouseEventArgs e)
        {
            this.setImage(pictureBox3, pultT1rotImage, pultT0rotImage, true);
            if (ios != null) ios.setBoolean(4, 0, false);
            s0Pressed = false;
        }

        private void pictureBox3_MouseUp(object sender, MouseEventArgs e)
        {
            this.setImage(pictureBox3, pultT1rotImage, pultT0rotImage, false);
            if (ios != null) ios.setBoolean(4, 0, true);
            s0Pressed = true;
        }

        private void pictureBox4_MouseDown(object sender, MouseEventArgs e)
        {
            this.setImage(pictureBox4, pultT1gruenImage, pultT0gruenImage, true);
            if (ios != null) ios.setBoolean(4, 1, true);
            s1Pressed = true;

        }

        private void pictureBox4_MouseUp(object sender, MouseEventArgs e)
        {
            this.setImage(pictureBox4, pultT1gruenImage, pultT0gruenImage, false);
            if (ios != null) ios.setBoolean(4, 1, false);
            s1Pressed = false;


        }

        private void pictureBox5_MouseDown(object sender, MouseEventArgs e)
        {
            this.setImage(pictureBox5, pultT1Image, pultT0Image, true);
            if (ios != null) ios.setBoolean(4, 2, true);
            resetPressed = true;
        }

        private void pictureBox5_MouseUp(object sender, MouseEventArgs e)
        {
            this.setImage(pictureBox5, pultT1Image, pultT0Image, false);
            if (ios != null) ios.setBoolean(4, 2, false);
            resetPressed = false;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            modell.setStrombegrenzung(checkBox1.Checked);
        }

        private void pbh1_MouseDown(object sender, MouseEventArgs e)
        {
            this.plc_h1.forceState(true);
            this.outputChanged(plc_h1);

        }

        private void pbh1_MouseUp(object sender, MouseEventArgs e)
        {
            this.plc_h1.forceState(false);
            this.outputChanged(plc_h1);

        }

        private void speedpb_MouseDown(object sender, MouseEventArgs e)
        {
            modell.m_speed.forceState(true);
            this.outputChanged(modell.m_speed);

        }

        private void speedpb_MouseUp(object sender, MouseEventArgs e)
        {
            modell.m_speed.forceState(false);
            this.outputChanged(modell.m_speed);

        }

        private void pbreset_MouseDown(object sender, MouseEventArgs e)
        {
            if (ios != null) ios.setBoolean(4, 2, true);
            resetPressed = true;
            setImage(pictureBox5, pultT1Image, pultT0Image, true);

        }


        private void pbhand_Click(object sender, EventArgs e)
        {
            if (pbhand.Image == sopenImage)
            {
                if (ios != null) ios.setBoolean(4, 3, true);
                this.handSteuerung = true;
                this.setImage(pictureBox2, pultS1Image, pultS0Image, false);
            }
            else
            {
                if (ios != null) ios.setBoolean(4, 3, false);
                this.handSteuerung = false;
                this.setImage(pictureBox2, pultS1Image, pultS0Image, true);
            }
        }

        private void pbreset_MouseUp(object sender, MouseEventArgs e)
        {
            if (ios != null) ios.setBoolean(4, 2, false);
            resetPressed = false;
            setImage(pictureBox5, pultT1Image, pultT0Image, false);

        }
       
    }
}