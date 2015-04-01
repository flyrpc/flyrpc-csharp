using System;

namespace flyrpc
{
    public class Client {
		private Protocol protocol;
		private Router router;
        private byte nextSeq = 0;

        public Client(string host, int port) {
			router = new Router();
			protocol = new Protocol(host, port);
			protocol.OnPacket += HandleOnPacket;
        }

        void HandleOnPacket (Packet pkt) {
			if((pkt.flag & Protocol.FlagRPC) != 0 &&
			   (pkt.flag & Protocol.FlagResp) != 0) {
                // FIXME HandleResponse
			}
            router.emitPacket(this, pkt);
        }

		public void OnMessage(UInt16 cmd, Action<Client, byte[]> handler) {
			router.AddRoute(cmd, handler);
		}

        public void SendMessage(UInt16 cmd, byte[] buffer) {
            Packet p = new Packet();
            p.flag = 0;// Protocol.FlagRPC | Protocol.Resp;//
            p.cmd = cmd;
            p.seq = nextSeq ++;
            p.msgBuff = buffer;
            protocol.SendPacket(p);
        }

    }
}
