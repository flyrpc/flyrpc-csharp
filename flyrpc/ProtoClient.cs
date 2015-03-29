using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace imsglib
{
    public class ProtoClient
    {
        private string host;
        private int port;
        private TcpClient client;
        private NetworkStream stream;
        private BufferedStream bfs;
        private Thread thread;

        public event Action<byte[], int> OnData;
        public event Action OnClose;
        public event Action OnError;

        public ProtoClient(string host, int port) {
            this.host = host;
            this.port = port;
            this.thread = new Thread(Start);
            this.thread.Start();
        }

        private void Start() {
            this.client = new TcpClient(this.host, this.port);
            this.stream = this.client.GetStream();
            this.bfs = new BufferedStream(this.stream);
            byte[] bytes;
            while((bytes = this.ReadPacket()) != null) {
                Console.WriteLine("sss:" + Encoding.ASCII.GetString(bytes));
				this.HandlePacket(bytes);
            }
        }

		private void HandlePacket(byte[] bytes) {
		}

        private byte[] ReadPacket() {
			byte[] bytes = this.ReadBytes(2);
			if(bytes == null) return null;
            int length = System.BitConverter.ToInt16(bytes, 0);
//            Console.WriteLine("readed length:"+length);

			bytes = this.ReadBytes(length);
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
