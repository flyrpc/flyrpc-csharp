using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Msg;

namespace flyrpc
{
	class MainClass
	{
		public static ManualResetEvent ReadDone = new ManualResetEvent (false);

		public static void Main (string[] args)
		{
            Protocol protocol = new Protocol("127.0.0.1", 5555);
            protocol.OnPacket += OnPacket;
            // ProtoClient client = new ProtoClient("127.0.0.1", 6666);
			/* Console.WriteLine ("Hello World!"); */
			/* TcpClient client = new TcpClient(); */
            /* client.Connect ("127.0.0.1", 6666); */
            /* NetworkStream ns = client.GetStream(); */
            /* byte[] buf = new byte[1024]; */
            /* int readed = ns.Read(buf, 0, 1024); */
            /* Console.WriteLine(Encoding.ASCII.GetString(buf) + readed); */
            /* readed = ns.Read(buf, 0, 1024); */
            /* Console.WriteLine(Encoding.ASCII.GetString(buf) + readed); */
            /* Socket socket = client.Client; */
            /* EventSocket es = new EventSocket(socket); */
            /* es.OnData += OnData; */
                    /*
					MsgClient mc = new MsgClient (ns, Encoding.ASCII.GetBytes ("123456"));
					MsgClient mc2 = new MsgClient (ns, Encoding.ASCII.GetBytes ("123456"));
					IAsyncResult result = ns.BeginWrite (mc.ByteBuffer, 0, mc.ByteBuffer.Length, new AsyncCallback (WriteCallback), mc);
					doOtherStuff ();
					IAsyncResult result2 = ns.BeginRead (mc2.ByteBuffer, 0, mc2.ByteBuffer.Length, new AsyncCallback (WriteCallback), mc2);
					doOtherStuff ();
					ReadDone.WaitOne ();
					/*
                    BinaryReader(n);
                    BinaryWriter w = new BinaryWriter(n);
                    w.Write("0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
                    w.Flush();
                    */
            // Thread.Sleep(1000);
		}

        public static void OnPacket(Packet packet) {
            Console.WriteLine("Packet {0} {1} {2} {3} {4}", packet.flag, packet.cmd, packet.seq, packet.length, packet.msgBuff);
			Console.WriteLine("msgBuff {0} {1} {2}", packet.msgBuff[0], packet.msgBuff[1], packet.msgBuff[2]);
			Hello hello = Hello.Deserialize(packet.msgBuff);
			Console.WriteLine("Deserilized {0} {1} {2}", hello, hello.Id, hello.Name);
        }

        public static void OnData(byte[] bytes, int readed) {
            Console.WriteLine("readed");
            Console.WriteLine(readed);
        }

		public static void doOtherStuff ()
		{
			for (int x=1; x<=5; x++) {
				Console.WriteLine ("Thread {0} ({1}) - doOtherStuff(): {2}...",
                        Thread.CurrentThread.GetHashCode (),
                        Thread.CurrentThread.ThreadState, x);
				Thread.Sleep (1000);
			}
		}

		public static void WriteCallback (IAsyncResult asyncResult)
		{
			MsgClient mc = (MsgClient)asyncResult.AsyncState;
			mc.NetStream.EndWrite (asyncResult);
			Console.WriteLine ("Thread {0} ({1}) - WriteCallback(): Sent {2} bytes...",
                    Thread.CurrentThread.GetHashCode (),
                    Thread.CurrentThread.ThreadState, mc.ByteBuffer.Length);
		}
	}

	class MsgClient
	{
		public byte[] ByteBuffer;
		public int TotalBytesRcvd = 0;
		public NetworkStream NetStream;

		public MsgClient (NetworkStream s, byte[] byteBuffer)
		{
			this.NetStream = s;
			this.ByteBuffer = byteBuffer;
		}

	}

}
