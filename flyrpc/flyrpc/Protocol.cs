using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace flyrpc
{
	public class Header
	{
		public byte flag;
		public byte cmd;
		public byte seq;
	}

	public class Packet
	{
		public byte flag;
		public UInt16 cmd;
		public byte seq;
		public UInt16 length;
		public byte[] msgBuff;
	}

	public class Protocol
	{
		public const byte SubTypeBits = 0xb0;

		// sub types
		public const byte TypeRPC = 0xb0;
		public const byte TypePing = 0x80;
		public const byte TypeHello = 0x40;
		public const byte TypeMQ = 0x00;

		// ping flags
		public const byte FlagPing = 0x01;
		public const byte FlagPong = 0x02;

		// rpc flags
		public const byte FlagReq = 0x01;
		public const byte FlagResp = 0x02;
		public const byte FlagError = 0x04;
		public const byte FlagBuffer = 0x08;
		private string host;
		private int port;
		private TcpClient client;
		private bool running;
		private NetworkStream stream;
		private BufferedStream bfs;
		private BinaryReader br;
		private BinaryWriter bw;
		private Thread thread;

		public event Action<Packet> OnPacket;
		public event Action OnConnect;
		public event Action OnClose;
		public event Action OnError;

		public Protocol ()
		{
		}

		public void Connect (string host, int port)
		{
			this.host = host;
			this.port = port;
			this.thread = new Thread (Start);
			this.thread.Start ();
		}

		private void Start ()
		{
			Console.WriteLine ("Protocol Connect");
			this.client = new TcpClient (this.host, this.port);
			Console.WriteLine ("Protocol Connected");
			this.running = true;
			this.OnConnect ();
			this.stream = this.client.GetStream ();
			this.bfs = new BufferedStream (this.stream);
			this.br = new BinaryReader (this.bfs);
			this.bw = new BinaryWriter (this.stream);
			Console.WriteLine ("Protocol Read packet");
			Packet packet;
			while (running && (packet = this.ReadPacket()) != null) {
				Console.WriteLine ("sss: flag {0} cmd {1} seq {2} len {3}: buff {4}", packet.flag, packet.cmd, packet.seq, packet.length, packet.msgBuff);
				this.OnPacket (packet);
			}
			Console.WriteLine ("Protocol Closed");
			this.OnClose ();
		}

		private Packet ReadPacket ()
		{
			// byte[] bytes = this.ReadBytes(2);
			// if(bytes == null) return null;
			// int length = System.BitConverter.ToInt16(bytes, 0);
//  Console.WriteLine("readed length:"+length);

			Packet p = new Packet ();
			p.flag = this.br.ReadByte ();
			p.cmd = Helpers.ReadUInt16BE (this.br);
			p.seq = this.br.ReadByte ();
			p.length = Helpers.ReadUInt16BE (this.br);

			byte[] bytes = this.br.ReadBytes (p.length);
			if (bytes == null || bytes.Length < p.length)
				return null;
//            Console.WriteLine("readed bytes:" + bytes);
			p.msgBuff = bytes;
			return p;
		}

		public void SendPacket (Packet p)
		{
			this.bw.Write (p.flag);
			Helpers.WriteUInt16BE (this.bw, p.cmd);
			this.bw.Write (p.seq);
			if (p.msgBuff.Length > 65535) {
				throw new Exception ("Message buffer too long");
			}
			UInt16 len = (UInt16)p.msgBuff.Length;
			Helpers.WriteUInt16BE (this.bw, len);
			this.bw.Write (p.msgBuff);
		}

/*
        private byte[] ReadBytes(int length) {
            int readed = 0;
            byte[] bytes = new byte[length];
			int totalReaded = 0;
            while(totalReaded < length && (readed = this.bfs.Read(bytes, totalReaded, length - totalReaded)) != 0) {
				totalReaded += readed;
            }
			if(readed == 0) {
				return null;
			}
			return bytes;
        }
*/

		public void Close ()
		{
			if (this.client != null) {
				Console.WriteLine ("Closing");
				this.client.Close ();
			}
			running = false;
		}
	}

	public static class Helpers
	{
		// Note this MODIFIES THE GIVEN ARRAY then returns a reference to the modified array.
		public static byte[] Reverse (this byte[] b)
		{
			Array.Reverse (b);
			return b;
		}

		public static void WriteUInt16BE (this BinaryWriter binWtr, UInt16 val)
		{
			binWtr.Write (BitConverter.GetBytes (val).Reverse ());
		}

		public static UInt16 ReadUInt16BE (this BinaryReader binRdr)
		{
			return BitConverter.ToUInt16 (binRdr.ReadBytesRequired (sizeof(UInt16)).Reverse (), 0);
		}

		public static Int16 ReadInt16BE (this BinaryReader binRdr)
		{
			return BitConverter.ToInt16 (binRdr.ReadBytesRequired (sizeof(Int16)).Reverse (), 0);
		}

		public static UInt32 ReadUInt32BE (this BinaryReader binRdr)
		{
			return BitConverter.ToUInt32 (binRdr.ReadBytesRequired (sizeof(UInt32)).Reverse (), 0);
		}

		public static Int32 ReadInt32BE (this BinaryReader binRdr)
		{
			return BitConverter.ToInt32 (binRdr.ReadBytesRequired (sizeof(Int32)).Reverse (), 0);
		}

		public static byte[] ReadBytesRequired (this BinaryReader binRdr, int byteCount)
		{
			var result = binRdr.ReadBytes (byteCount);

			if (result.Length != byteCount)
				throw new EndOfStreamException (string.Format ("{0} bytes required from stream, but only {1} returned.", byteCount, result.Length));

			return result;
		}
	}
}

