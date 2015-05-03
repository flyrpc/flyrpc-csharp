// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;

namespace flyrpc
{
	public class Router
	{
		private Dictionary<UInt16, Action<Client, byte[]>> messageHandlers;

		public Router ()
		{
			this.messageHandlers = new Dictionary<UInt16, Action<Client, byte[]>> ();
		}

		public void AddRoute (UInt16 cmd, Action<Client, byte[]> handler)
		{
			this.messageHandlers.Add (cmd, handler);
		}

		public void emitPacket (Client client, Packet pkt)
		{
			Action<Client, byte[]> action;
			if(messageHandlers.TryGetValue(pkt.cmd, out action)) {
				action(client, pkt.msgBuff);
			}
		}
	}
}
