using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Application
{
	public class Header {
		public byte flag;
		public byte cmd;
		public byte seq;
	}

	public class Packet {
		public byte flag;
		public byte cmd;
		public byte seq;
		public UInt16 length;
		public byte[] msgBuff;
	}

	public class Protocol
	{
		public const byte FlagRPC = 0x80;
		public const byte FlagResp = 0x40;
		public const byte FlagError = 0x20;
		public const byte FlagBuffer = 0x10;
		private string host;
		private int port;
		private TcpClient client;
		private NetworkStream stream;
		private BufferedStream bfs;
		private BinaryReader br;
		private Thread thread;

		public Protocol (string host, int port)
		{
			this.host = host;
			this.port = port;
			this.thread = new Thread(Start)
		}

		private void Start() {
			this.client = new TcpClient(this.host, this.port)
			this.stream = this.client.GetStream()
			this.bfs = new BufferedStream(this.stream);
			this.br = new BinaryReader(this.bfs);
			byte[] bytes;
			while((bytes = this.ReadPacket()) != null) {
                Console.WriteLine("sss:" + Encoding.ASCII.GetString(bytes));
				this.HandlePacket(bytes);
			}
		}

		private void HandlePacket(byte[] bytes) {
		}

        private byte[] ReadPacket() {
			// byte[] bytes = this.ReadBytes(2);
			// if(bytes == null) return null;
            // int length = System.BitConverter.ToInt16(bytes, 0);
//  Console.WriteLine("readed length:"+length);

			Packet p = new Packet();
			p.flag = this.br.ReadByte();
			p.cmd = this.br.ReadByte();
			p.seq = this.br.ReadByte();
			p.length = this.br.ReadUInt16();

			IPAddress.NetworkToHostOrder(p.length);

			byte[] bytes = this.br.ReadBytes(length);
			if(bytes == null) return null;
//            Console.WriteLine("readed bytes:" + bytes);
            return bytes;
        }

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

        public void Close() {
        }
	}
}

