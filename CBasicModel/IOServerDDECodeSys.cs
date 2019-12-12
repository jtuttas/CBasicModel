using System;
using System.Collections.Generic;
using System.Text;
using NDde.Client;


namespace CBasicModel
{
    class IOServerDDECodeSys : IOServer
    {
            private DdeClient ps;

            public IOServerDDECodeSys(DdeClient plc)
            {
                ps = plc;
                ps.Disconnected += OnDisconnected;
            }

            override public void add(BooleanItem bi)
            {
                Console.WriteLine("Füge hinzu:" + bi.ToString()+" connected="+connected);
                pool.Add(bi);
                if (connected)
                {

                    ps.StartAdvise("%qx" + bi.byteAdr + "." + bi.bitAdr, 1, true, true, 60000, this);
                    //ps.BeginRequest("%qx" + bi.byteAdr + "." + bi.bitAdr, 1, OnRequestComplete, ps);

                    String s = ps.Request("%qx" + bi.byteAdr + "." + bi.bitAdr, 60000);
                    //Console.WriteLine("Request=" + s);
                    
                    //Console.WriteLine("%qx" + bi.byteAdr + "." + bi.bitAdr + "=" + s);
                    if (s.StartsWith("1")) bi.setState(true);
                    else bi.setState(false);
                    
                    //Console.WriteLine("state)" + bi.getState());
                }
                else
                {
                    Console.WriteLine("** Achtung Eingang hinzugefügt aber noch nicht verbunden !");
                }

                
                if (listener != null) listener.outputChanged(bi);
                else
                {
                    Console.WriteLine("** Achtung Listener ist NULL!");
                }
            }

            private static void OnRequestComplete(IAsyncResult ar)
            {
                try
                {
                    DdeClient client = (DdeClient)ar.AsyncState;
                    byte[] data = client.EndRequest(ar);
                    Console.WriteLine("OnRequestComplete: " + Encoding.ASCII.GetString(data));
                }
                catch (Exception e)
                {
                    Console.WriteLine("OnRequestComplete: " + e.Message);
                }
            }
            override public Boolean connect()
            {
                try
                {
                    Console.WriteLine("DDE try Connect");
                    ps.Connect();
                    /*
                    ThreadStart del;
                    del = new ThreadStart(run);
                    runner = new Thread(del);
                    runner.Start();
                     */
                    ps.Advise += OnAdvise;
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
                    try
                    {
                        if (state)
                        {
                            ps.Poke("%ix" + byteadr + "." + bitadr, "1", 60000);
                            //Console.WriteLine("Write Bool: %ix"+ byteadr+"."+bitadr+" state="+state);
                        }
                        else
                        {
                            ps.Poke("%ix" + byteadr + "." + bitadr, "0", 60000);
                            //Console.WriteLine("Write Bool: %ix" + byteadr + "." + bitadr+" state="+state);
                        }
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

            private void OnAdvise(object sender, DdeAdviseEventArgs args)
            {
                //Console.WriteLine("-> OnAdvise:" + args.Text+" sender="+sender+"\n");
                //Console.WriteLine("-> OnAdvise: Item=" + args.Item+"Value="+args.Text);
                //Console.WriteLine("OK");
                String itemString = args.Item;
                String byteAdr = itemString.Substring(3, itemString.IndexOf(".")-3);
                String bitAdr = itemString.Substring(itemString.IndexOf(".")+1);

                //Console.WriteLine("Byte=(" + byteAdr + ") Bit=(" + bitAdr+")");
                BooleanItem bi = getBooleanItem(Int32.Parse(byteAdr),Int32.Parse(bitAdr));
                //Console.WriteLine("Found: "+bi.ToString()+" NewValue="+args.Data.ToString());
                if (args.Data[0] == 49)
                {
                    bi.setState(true);
                }
                else
                {
                    bi.setState(false);
                }

                if (listener != null) listener.outputChanged(bi);
            }

            private BooleanItem getBooleanItem(int byteAdr, int bitAdr)
            {
               foreach (BooleanItem tmpItem in pool)
               {
                   if (tmpItem.byteAdr == byteAdr && tmpItem.bitAdr == bitAdr) return tmpItem;
               }
               return null;
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
