using S22.Xmpp.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Castoriadis.XMPP
{
    public class Program
    {
        static void Main(string[] args)
        {
            using (var client = new XmppClient("jabber.dk", "castoriadis", "georgina"))
            {
                var q = new Queue<string>();
                var ev = new AutoResetEvent(false);
                client.Message += new EventHandler<S22.Xmpp.Im.MessageEventArgs>((o,e) => {
                    lock(q) {
                        q.Enqueue(e.Message.Body);
                    }
                    ev.Set();
                });
                client.Connect();
                var admin = new S22.Xmpp.Jid("inz@jabber.dk");
                client.SendMessage(admin, "You're granted access to Castoriadis!");

                Console.MainClass._readCommand = () =>
                {
                    ev.WaitOne();
                    lock (q)
                    {
                        return q.Dequeue();
                    }
                };
                Console.MainClass._writeResult = s =>
                {
                    var msg = s == null ? "(null)" : s.ToString();
                    if (string.IsNullOrWhiteSpace(msg))
                    {
                        msg = "(empty string)";
                    }
                    client.SendMessage(admin, msg);
                };
                Console.MainClass.Main(args);
            }
        }
    }
}
