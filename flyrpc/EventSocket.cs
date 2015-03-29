using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace imsglib
{
	public class EventSocket
	{
        public event Action<byte[], int> OnData;
		public event Action OnClose;
        public event Action OnOpen;
        public event Action<Exception> OnError;

		private Socket socket;
        private bool disposed = false;

        public EventSocket(string host, int port) {
        }

		public EventSocket (Socket socket)
		{
			this.socket = socket;
            this.GetNextChunk();
		}

		public void SendMessage ()
		{
		}

        public void Close() {
            if (disposed) {
                return;
            }
            if (socket != null) {
                socket.Close();
            }
        }

        private void GetNextChunk()
		{
            Console.WriteLine("reading");
            byte[] bytes = new byte[4096];
            socket.BeginReceive(bytes, 0, bytes.Length, SocketFlags.None, new AsyncCallback(EndGetChunk), bytes);
		}

        private void EndGetChunk(IAsyncResult result) {
            Console.WriteLine("recevied");
            int readed = socket.EndReceive(result);
            byte[] bytes = (byte[])result.AsyncState;
            Console.WriteLine("recevied: {0}", Encoding.ASCII.GetString(bytes));
            this.OnData(bytes, readed);
        }

	}
}

