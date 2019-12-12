using EasyModbus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;


namespace CBasicModel
{
    class IOServerModbus : IOServer
    {
        ModbusClient modbusClient;
        private bool[] coil_input = { false, false, false, false, false, false, false, false };
        private bool[] coil_output = { false, false, false, false, false, false, false, false };
        private int coil_offset = 4;

        public override bool connect()
        {
            
            modbusClient = new ModbusClient("192.168.178.74", 502);
            modbusClient.ReceiveDataChanged += new EasyModbus.ModbusClient.ReceiveDataChangedHandler(UpdateReceiveData);
            modbusClient.SendDataChanged += new EasyModbus.ModbusClient.SendDataChangedHandler(UpdateSendData);
            modbusClient.ConnectedChanged += new EasyModbus.ModbusClient.ConnectedChangedHandler(UpdateConnectedChanged);
            try
            {
                modbusClient.Connect();
                modbusClient.ConnectionTimeout = 10000;
                connected = true;
                Console.WriteLine("Connected..");
                ThreadStart del;
                del = new ThreadStart(run);
                runner = new Thread(del);
                runner.Start();
                
                if (listener != null) listener.stateChanged("Connected");
                return true;
            }
            catch (EasyModbus.Exceptions.ConnectionException ex)
            {
                Console.WriteLine("Exception:" + ex.Message);
                return false;
            }
        }

        string receiveData = null;
        void UpdateReceiveData(object sender)
        {
            receiveData = "Rx: " + BitConverter.ToString(modbusClient.receiveData).Replace("-", " ") + System.Environment.NewLine;
            Console.WriteLine("UpdateReceiveData" + receiveData);
        }

        string sendData = null;
        void UpdateSendData(object sender)
        {
            sendData = "Tx: " + BitConverter.ToString(modbusClient.sendData).Replace("-", " ") + System.Environment.NewLine;
            Console.WriteLine("UpdateReceiveData" + receiveData);
        }
        private void UpdateConnectedChanged(object sender)
        {
            if (modbusClient.Connected)
            {
                Console.WriteLine("Connected to Server");
            }
            else
            {
                Console.WriteLine("Not Connected to Server");
            }
        }


        public override void disconnect()
        {
            modbusClient.Disconnect();
            connected = false;
        }

        public  override void flush()
        {
            //Console.WriteLine("Flush!");
            // Write _asynchronously_, your app will stay responsive.
            WriteCoilAsync(modbusClient);
            ReadCoilAsync(modbusClient);
            // then read asynchronously. Again, App will stay responsive.
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
        private void WriteCoilAsync(ModbusClient client)
        {
           
                try
                {
                    client.WriteMultipleCoils(coil_offset*8, coil_output);
                    //Console.WriteLine("Write " + coil_output);
                }
                catch (System.IO.IOException e)
                {
                    Console.WriteLine("EXCEPTION Write Coils!"+e.Message);
                    Console.WriteLine(e.StackTrace);                                        
                }
           
        }
        private void ReadCoilAsync(ModbusClient client)
        {
           
            try
            {
                this.coil_input= client.ReadCoils((coil_offset+1) * 8,8);
                    //Console.WriteLine("Write " + coil_output);
                }
                catch (System.IO.IOException e)
                {
                    Console.WriteLine("EXCEPTION Write Coils!" + e.Message);
                    Console.WriteLine(e.StackTrace);
                }
            
        }
    }
}
