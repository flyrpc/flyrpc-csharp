using System;

namespace flyrpc
{
    public class Client {
		private Protocol protocol;
		private Router router;

        public Client(string host, int port) {
			router = new Router();
			protocol = new Protocol(host, port);
			protocol.OnPacket += HandleOnPacket;
        }

        void HandleOnPacket (Packet pkt) {
			if((pkt.flag & Protocol.FlagRPC) != 0 &&
			   (pkt.flag & Protocol.FlagResp) != 0) {
				router.emitPacket(pkt);
			}
        }

		public void OnMessage(UInt16 cmdId, Action<byte[]> handler) {
			router.AddRoute(cmdId, handler);
		}

    }
}
