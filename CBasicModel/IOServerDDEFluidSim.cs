using System;
using System.Collections.Generic;
using System.Text;
using NDde.Client;

namespace CBasicModel
{
    class IOServerDDEFluidSim : IOServer
    {
            private DdeClient ps;
            private String inMarke,outMarke;
            Int32 outValue;
            Int32 inValue;

            public IOServerDDEFluidSim(DdeClient plc, String outm, String inm)
            {
                ps = plc;
                inMarke = inm;
                outMarke=outm;
                ps.Disconnected += OnDisconnected;
            }

            
            override public Boolean connect()
            {
                try
                {
                    Console.WriteLine("DDE try Connect");
                    ps.Connect();
                    ps.StartAdvise(outMarke, 1, true, false, 60000, this);
                    /*
                    ThreadStart del;
                    del = new ThreadStart(run);
                    runner = new Thread(del);
                    runner.Start();
                     */
                    inValue = 0;
                    outValue = 0;
                    ps.Advise += OnAdvise;
                    String s = ps.Request(outMarke, 60000);
                    inValue = Int32.Parse(s);
                    Console.WriteLine("inValue festgelegt auf" + inValue);
                    connected = true;
                    if (listener != null) listener.stateChanged("Connected");
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Connect Failed");
                    if (listener != null) listener.stateChanged("Connection failed");
                    connected = false;
                    return false;
                }
            }
            override public void add(BooleanItem bi)
            {

                Boolean oldState = bi.getState();

                // Initiales Mapping
                if ((inValue & 1) != 0 && bi.byteAdr == 4 && bi.bitAdr == 2) bi.setState(true);
                if ((inValue & 2) != 0 && bi.byteAdr == 4 && bi.bitAdr == 3) bi.setState(true);
                if ((inValue & 4) != 0 && bi.byteAdr == 4 && bi.bitAdr == 4) bi.setState(true);
                if ((inValue & 8) != 0 && bi.byteAdr == 4 && bi.bitAdr == 5) bi.setState(true);
                if ((inValue & 16) != 0 && bi.byteAdr == 9 && bi.bitAdr == 0) bi.setState(true);
                if ((inValue & 32) != 0 && bi.byteAdr == 9 && bi.bitAdr == 1) bi.setState(true);
                if ((inValue & 64) != 0 && bi.byteAdr == 9 && bi.bitAdr == 2) bi.setState(true);

                if ((inValue & 1) == 0 && bi.byteAdr == 4 && bi.bitAdr == 2) bi.setState(false);
                if ((inValue & 2) == 0 && bi.byteAdr == 4 && bi.bitAdr == 3) bi.setState(false);
                if ((inValue & 4) == 0 && bi.byteAdr == 4 && bi.bitAdr == 4) bi.setState(false);
                if ((inValue & 8) == 0 && bi.byteAdr == 4 && bi.bitAdr == 5) bi.setState(false);
                if ((inValue & 16) == 0 && bi.byteAdr == 9 && bi.bitAdr == 0) bi.setState(false);
                if ((inValue & 32) == 0 && bi.byteAdr == 9 && bi.bitAdr == 1) bi.setState(false);
                if ((inValue & 64) == 0 && bi.byteAdr == 9 && bi.bitAdr == 2) bi.setState(false);

                Console.WriteLine("DDE Fluidsim: Füge hinzu:" + bi.ToString());
                pool.Add(bi);

                if (listener != null)
                {
                    if (oldState != bi.getState())
                    {
                        Console.WriteLine("ADD Output Changed !");
                        listener.outputChanged(bi);
                    }
                }
            }

            override public void disconnect()
            {
                if (connected)
                {
                    try
                    {
                        ps.Disconnect();
                    }
                    catch (Exception ex)
                    {
                    }
                }
                connected = false;
            }

            override public void setBoolean(int byteadr, int bitadr, Boolean state)
            {
                if (connected)
                {
                    // Mapping
                    if (state)
                    {
                        if (byteadr == 4 && bitadr == 2) outValue |= 1;
                        if (byteadr == 4 && bitadr == 3) outValue |= 2;
                        if (byteadr == 4 && bitadr == 4) outValue |= 4;
                        if (byteadr == 4 && bitadr == 5) outValue |= 8;
                        if (byteadr == 5 && bitadr == 0) outValue |= 16;
                        if (byteadr == 5 && bitadr == 1) outValue |= 32;
                        if (byteadr == 5 && bitadr == 2) outValue |= 64;
                    }
                    else
                    {
                        if (byteadr == 4 && bitadr == 2) outValue &= ~1;
                        if (byteadr == 4 && bitadr == 3) outValue &= ~2;
                        if (byteadr == 4 && bitadr == 4) outValue &= ~4;
                        if (byteadr == 4 && bitadr == 5) outValue &= ~8;
                        if (byteadr == 5 && bitadr == 0) outValue &= ~16;
                        if (byteadr == 5 && bitadr == 1) outValue &= ~32;
                        if (byteadr == 5 && bitadr == 2) outValue &= ~64;

                    }


                    try
                    {
                        ps.Poke(inMarke, outValue.ToString(), 60000);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Write Bool:e=" + e);
                        //return false;
                    }
                }
                //return true;
            }

            public override void flush()
            {
            }

            override public Boolean readBoolean(int byteadr, int bitadr)
            {
                
                Console.WriteLine("ReadBolean ! byteadr="+byteadr+" bitadr="+bitadr);
//                Object b = null;
//                long r = ps.ReadOutputPoint(byteadr, bitadr, PointDataTypeConstants.S7_Bit, ref b);
//                return (Boolean)b;
                return true;
            }
            private BooleanItem getBooleanItem(int byteAdr, int bitAdr)
            {
                foreach (BooleanItem tmpItem in pool)
                {
                    if (tmpItem.byteAdr == byteAdr && tmpItem.bitAdr == bitAdr) return tmpItem;
                }
                return null;
            }

            private void OnAdvise(object sender, DdeAdviseEventArgs args)
            {
                //Console.WriteLine("-> OnAdvise:" + args.Text+" sender="+sender+"\n");
                //Console.WriteLine("-> OnAdvise: Item=" + args.Text);
                Int32 tmpInValue = Int32.Parse(args.Text);

                /*
                int bitAdr=0;
                int byteAdr=0;
                Boolean state;
                // Mapping
                if (tmpInValue>inValue) state=true;
                else state=false;

                BooleanItem bi;
                if (diff != 0)
                {
                    if ((diff & 1) != 0) { byteAdr = 4; bitAdr = 2; bi = this.getBooleanItem(byteAdr, bitAdr); bi.setState(state); if (listener != null && bi != null) listener.outputChanged(bi); }
                    if ((diff & 2) != 0) { byteAdr = 4; bitAdr = 3; bi = this.getBooleanItem(byteAdr, bitAdr); bi.setState(state); if (listener != null && bi != null) listener.outputChanged(bi); }
                    if ((diff & 4) != 0) { byteAdr = 4; bitAdr = 4; bi = this.getBooleanItem(byteAdr, bitAdr); bi.setState(state); if (listener != null && bi != null) listener.outputChanged(bi); }
                    if ((diff & 8) != 0) { byteAdr = 4; bitAdr = 5; bi = this.getBooleanItem(byteAdr, bitAdr); bi.setState(state); if (listener != null && bi != null) listener.outputChanged(bi); }
                    if ((diff & 16) != 0) { byteAdr = 9; bitAdr = 0; bi = this.getBooleanItem(byteAdr, bitAdr); bi.setState(state); if (listener != null && bi != null) listener.outputChanged(bi); }
                    if ((diff & 32) != 0) { byteAdr = 9; bitAdr = 1; bi = this.getBooleanItem(byteAdr, bitAdr); bi.setState(state); if (listener != null && bi != null) listener.outputChanged(bi); }
                    if ((diff & 64) != 0) { byteAdr = 9; bitAdr = 2; bi = this.getBooleanItem(byteAdr, bitAdr); bi.setState(state); if (listener != null && bi != null) listener.outputChanged(bi); }
                }

                //BooleanItem bi = this.getBooleanItem(byteAdr,bitAdr);
                //if (bi != null) bi.setState(state);
                Console.WriteLine("byteAdr=" + byteAdr + " bitAdr=" + bitAdr + " state=" + state);
                */
                BooleanItem bi;
                Boolean aktState;
                bi = this.getBooleanItem(4, 2);
                if (listener != null && bi != null)
                {
                    aktState = (tmpInValue & 1) !=0;
                    if (bi.getState() != aktState)
                    {
                        bi.setState((tmpInValue & 1) != 0);
                        listener.outputChanged(bi);
                    }
                }

                bi = this.getBooleanItem(4, 3);
                if (listener != null && bi != null)
                {
                    aktState = (tmpInValue & 2) !=0;
                    if (bi.getState() != aktState)
                    {
                        bi.setState((tmpInValue & 2) != 0);
                        listener.outputChanged(bi);
                    }
                }

                bi = this.getBooleanItem(4, 4);
                if (listener != null && bi != null)
                {
                    aktState = (tmpInValue & 4) !=0;
                    if (bi.getState() != aktState)
                    {
                        bi.setState((tmpInValue & 4) != 0);
                        listener.outputChanged(bi);
                    }
                }

                bi = this.getBooleanItem(4, 5);
                if (listener != null && bi != null)
                {
                    aktState = (tmpInValue & 8) !=0;
                    if (bi.getState() != aktState)
                    {
                        bi.setState((tmpInValue & 8) != 0);
                        listener.outputChanged(bi);
                    }
                }

                bi = this.getBooleanItem(9, 0);
                if (listener != null && bi != null)
                {
                    aktState = (tmpInValue & 16) !=0;
                    if (bi.getState() != aktState)
                    {
                        bi.setState((tmpInValue & 16) != 0);
                        listener.outputChanged(bi);
                    }
                }

                bi = this.getBooleanItem(9, 1);
                if (listener != null && bi != null)
                {
                    aktState = (tmpInValue & 32) !=0;
                    if (bi.getState() != aktState)
                    {
                        bi.setState((tmpInValue & 32) != 0);
                        listener.outputChanged(bi);
                    }
                }

                bi = this.getBooleanItem(9, 2);
                if (listener != null && bi != null)
                {
                    aktState = (tmpInValue & 64) !=0;
                    if (bi.getState() != aktState)
                    {
                        bi.setState((tmpInValue & 64) != 0);
                        listener.outputChanged(bi);
                    }
                }

                inValue = tmpInValue;
            }

          


            private void OnDisconnected(object sender, DdeDisconnectedEventArgs e)
            {
                Console.WriteLine("DDE: Ondisconnected!");
                if (listener != null)
                {
                    listener.stateChanged("Verbindung verloren!");
                    listener.lostConnection("");
                }
            }


    }

}
