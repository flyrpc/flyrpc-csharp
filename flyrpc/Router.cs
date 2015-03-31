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
		private Dictionary<UInt16, Action<byte[]>> messageHandlers;
		public Router ()
		{
            this.messageHandlers = new Dictionary<UInt16, Action<byte[]>>();
		}

		public void AddRoute(UInt16 cmd, Action<byte[]> handler)
		{
            this.messageHandlers.Add(cmd, handler);
		}

		public void emitPacket(Packet pkt) {
			Action<byte[]> action = messageHandlers[pkt.cmd];
			if (action != null) {
				action(pkt.msgBuff);
			}
		}
	}
}