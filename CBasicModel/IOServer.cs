using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Collections;

namespace CBasicModel
{
    abstract class IOServer
    {
        public  Boolean connected = false;
        public  PlcListener listener;
        public Thread runner;
        public ArrayList pool = new ArrayList();
        
        public IOServer()
        {
        }

        public void setListener(PlcListener l)
        {
            listener = l;
        }

        public virtual void add(BooleanItem bi)
        {
            //Console.WriteLine("Füge hinzu:" + bi.ToString());
            pool.Add(bi);
            if (listener != null) listener.outputChanged(bi);
        }

        public abstract Boolean connect();
        public abstract void disconnect();
        public abstract void setBoolean(int byteadr, int bitadr, Boolean state);
        public abstract Boolean readBoolean(int byteadr, int bitadr);
        public abstract void flush();


        public Boolean isConnected()
        {
            return connected;
        }

      

        public void run()
        {
            //listener.stateChanged("run..."+connected);
            //String es="";
            while (connected)
            {
                if (connected)
                {
                    try
                    {
                        flush();
                        foreach (BooleanItem tmpItem in pool)
                        {
                            Boolean b = readBoolean(tmpItem.byteAdr, tmpItem.bitAdr);
                            if (b != tmpItem.getState())
                            {
                                tmpItem.setState(b);
                                listener.outputChanged(tmpItem);
                            }
                        }

                    }
                    catch (Exception w)
                    {
                        Console.WriteLine("Exception:" + w);
                        //es = w.ToString();
                        connected = false;
                    }
                }
                Thread.Sleep(150);                
            }
            if (listener != null) listener.lostConnection("");
            //runner.Join(100);
        }

    }
}
