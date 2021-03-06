using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace flyrpc
{
	public class Client
	{
		private Protocol protocol;
		private Router router;
		private byte nextSeq = 0;
		private Dictionary<int, Action<Client, int, byte[]>> callbacks;
		private Dictionary<int, Timer> timers;

		public event Action OnConnect;
		public event Action<UInt16,byte[]> OnCommand;
		public event Action OnClose;

		public Client ()
		{
			router = new Router ();
			protocol = new Protocol ();
			protocol.OnConnect += () => OnConnect ();
			protocol.OnClose += () => OnClose ();
			protocol.OnPacket += HandleOnPacket;
			callbacks = new Dictionary<int, Action<Client, int, byte[]>> ();
			timers = new Dictionary<int, Timer> ();
		}

		public void Connect (string host, int port)
		{
			protocol.Connect (host, port);
		}

		public void Close ()
		{
			protocol.Close ();
		}

		protected void HandleOnPacket (Packet pkt)
		{
			byte subType = (byte)(pkt.flag & Protocol.SubTypeBits);
			if (subType == Protocol.TypeRPC &&
				(pkt.flag & Protocol.FlagResp) != 0) {
				int cbid = (pkt.cmd << 16) | pkt.seq;
				if (callbacks [cbid] != null) {
					if ((pkt.flag & Protocol.FlagError ) != 0) {
						// bigendian bytes to uint32
						uint x = BitConverter.ToUInt32(pkt.msgBuff, 0);
						// x = swapEndianness(x);
						x = ((x & 0x000000ff) << 24) +  // First byte
							((x & 0x0000ff00) << 8) +   // Second byte
								((x & 0x00ff0000) >> 8) +   // Third byte
								((x & 0xff000000) >> 24);   // Fourth byte
						callbacks[cbid](this, (int)x, null);
					} else {
					    callbacks [cbid] (this, 0, pkt.msgBuff);
					}
					callbacks.Remove (cbid);
					if (timers [cbid] != null) {
						timers [cbid].Dispose ();
						timers.Remove (cbid);
					}
				}
				return;
			}
			OnCommand (pkt.cmd, pkt.msgBuff);
			router.emitPacket (this, pkt);
		}

		public void OnMessage (UInt16 cmd, Action<Client, byte[]> handler)
		{
			router.AddRoute (cmd, handler);
		}

		protected byte SendPacket (byte flag, UInt16 cmd, byte[] buffer)
		{
			Packet p = new Packet ();
			p.flag = flag;
			p.cmd = cmd;
			p.seq = nextSeq ++;
			p.msgBuff = buffer;
			protocol.SendPacket (p);
			return p.seq;
		}

		public void SendMessage (UInt16 cmd, byte[] buffer)
		{
			this.SendPacket (Protocol.TypeRPC, cmd, buffer);
		}

		/// <summary>
		/// Sends the message.
		/// </summary>
		/// <param name="cmd">Cmd.</param>
		/// <param name="buffer">Buffer.</param>
		/// <param name="callback">Callback.</param>
		public void SendMessage (UInt16 cmd, byte[] buffer, Action<Client, int, byte[]> callback)
		{
			byte seq = this.SendPacket (Protocol.TypeRPC | Protocol.FlagReq, cmd, buffer);
			int cbid = cmd << 16 | seq;
			callbacks [cbid] = callback;
			timers [cbid] = new Timer (new TimerCallback ((object obj) => {
				callback (this, 1000, null);
				timers [cbid].Dispose ();
				callbacks.Remove (cbid);
				timers.Remove (cbid);
			}), cbid, 5000, Timeout.Infinite);
		}
	}
}
