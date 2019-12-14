
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using Modbus.Device;

namespace CBasicModel
{
    class IOServerModbus : IOServer
    {
        
        private bool[] coil_input = { false, false, false, false, false, false, false, false };
        private bool[] coil_output = { false, false, false, false, false, false, false, false };
        
        ModbusIpMaster master;
        TcpClient client;
        String ipadr="127.0.0.1";
        int ipport=502;

        public IOServerModbus(String ipadr,int ipport)
        {
            this.ipadr = ipadr;
            this.ipport = ipport;
        }

        public override bool connect()
        {
            try
            {
                client = new TcpClient(ipadr, ipport);
                master = ModbusIpMaster.CreateIp(client);

               
                connected = true;
                Console.WriteLine("Connected to " + ipadr + ":" + ipport);
                
                ThreadStart del;
                del = new ThreadStart(run);
                runner = new Thread(del);
                runner.Start();
                
                if (listener != null) listener.stateChanged("Connected");
                return true;
            }
            catch (System.Net.Sockets.SocketException e)
            {
                connected = false;
                return false;
            }
        }



        public override void disconnect()
        {
            client.Close();
            connected = false;
        }

        public  override void flush()
        {
            long start = DateTime.Now.Ticks;
            Console.Write("Flush..");

            ReadCoil();
            WriteCoil();
            long duration = DateTime.Now.Ticks - start;
            Console.WriteLine(".. finished "+(duration/10000)+" ms");

        }

        public override bool readBoolean(int byteadr, int bitadr)
        {
            //Console.WriteLine("Lese QX" + byteadr + "." + bitadr + " = " + coil_input[bitadr]);
            return coil_input[bitadr];
        }


        public override void setBoolean(int byteadr, int bitadr, bool state)
        {
            this.coil_output[bitadr] = state;
        }

        // 1. Run the Write - Part on a Threadpool Thread ...
        private bool WriteCoil()
        {
            master.WriteMultipleCoils(32, coil_output);

            return true;
        }
        private bool ReadCoil()
        {
            //Console.Write("Read..");
            this.coil_input= master.ReadCoils(40, 8);
            //Console.WriteLine(".. finished");
            return true;
        }
    }
}
