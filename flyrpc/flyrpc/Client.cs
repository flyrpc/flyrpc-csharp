using System;
using System.Collections;
using System.Collections.Generic;

namespace flyrpc
{
    public class Client {
		private Protocol protocol;
		private Router router;
        private byte nextSeq = 0;
		private Dictionary<int, Action<Client, int, byte[]>> callbacks;

        public Client(string host, int port) {
			router = new Router();
			protocol = new Protocol(host, port);
			protocol.OnPacket += HandleOnPacket;
			callbacks = new Dictionary<int, Action<Client, int, byte[]>>();
        }

        void HandleOnPacket (Packet pkt) {
			byte subType = (byte)(pkt.flag & Protocol.SubTypeBits);
			if(subType == Protocol.TypeRPC &&
			   (pkt.flag & Protocol.FlagResp) != 0) {
				int cbid = (pkt.cmd << 16) | pkt.seq;
				if(callbacks[cbid] != null) {
					callbacks[cbid](this, 0, pkt.msgBuff);
				}
				return;
			}
            router.emitPacket(this, pkt);
        }

		public void OnMessage(UInt16 cmd, Action<Client, byte[]> handler) {
			router.AddRoute(cmd, handler);
		}

		public byte SendPacket(byte flag, UInt16 cmd, byte[] buffer) {
            Packet p = new Packet();
			p.flag = flag;
            p.cmd = cmd;
            p.seq = nextSeq ++;
            p.msgBuff = buffer;
            protocol.SendPacket(p);
			return p.seq;
		}

        public void SendMessage(UInt16 cmd, byte[] buffer) {
			this.SendPacket(Protocol.TypeRPC, cmd, buffer);
        }

		/// <summary>
		/// Sends the message.
		/// </summary>
		/// <param name="cmd">Cmd.</param>
		/// <param name="buffer">Buffer.</param>
		/// <param name="callback">Callback.</param>
		public void SendMessage(UInt16 cmd, byte[] buffer, Action<Client, int, byte[]> callback) {
			byte seq = this.SendPacket(Protocol.TypeRPC | Protocol.FlagReq, cmd, buffer);
			int cbid = cmd << 16 | seq;
			callbacks[cbid] = callback;
		}
    }
}
