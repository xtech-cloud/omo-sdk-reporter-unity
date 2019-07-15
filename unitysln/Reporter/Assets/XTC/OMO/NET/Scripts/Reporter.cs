using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace XTC.OMO.NET
{
    public class Reporter
    {
        public delegate void OnReplyDelegate(byte[] _reply, string _address, int _port);
        public delegate void OnExceptionDelegate(System.Exception _ex);

        public OnReplyDelegate onReply;
        public OnExceptionDelegate onException;

        private string host;
        private int port;
        private bool running_ = false;
        private UdpClient udpClient = null;
        private IPEndPoint remoteEP = null;

        private Queue<byte[]> outPool = new Queue<byte[]>();

        public void Setup(string _host, int _port)
        {
            host = _host;
            port = _port;
            remoteEP = new IPEndPoint(IPAddress.Parse(host), port);
            //auto allocate port
            udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
        }

        public void Run()
        {
            if (!running_)
            {
                running_ = true;

                Thread receiverThread = new Thread(runReceiver);
                receiverThread.Start();
                Thread senderThread = new Thread(runSender);
                senderThread.Start();
            }
        }

        public void Stop()
        {
            if (running_)
            {
                running_ = false;
                udpClient.Close();
                udpClient = null;
            }
        }

        public void Report(byte[] _data)
        {
            outPool.Enqueue(_data);
        }

        private void runReceiver()
        {
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            while (running_)
            {
                if (null == udpClient)
                    break;
                try
                {
                    if (udpClient.Available <= 0)
                        continue;
                    byte[] buf = udpClient.Receive(ref sender);
                    if (null == buf)
                        continue;
                    if (null != onReply) onReply(buf, sender.Address.ToString(), sender.Port);
                }
                catch (System.Exception ex)
                {
                    if (null != onException) onException(ex);
                }
            }
            Debug.Log("Reporter::receiver - thread exit");
        }

        private void runSender()
        {
            while (running_)
            {
                if (null == udpClient)
                    break;
                while (outPool.Count != 0)
                {
                    try
                    {
                        byte[] buf = outPool.Dequeue();
                        if (null == buf)
                            continue;
                        //must use endPoint
                        udpClient.Send(buf, buf.Length, remoteEP);
                    }
                    catch (System.Exception ex)
                    {
                        if (null != onException) onException(ex);
                    }
                }
                Thread.Sleep(50);
            }
            Debug.Log("Reporter::sender - thread exit");
        }
    }

    public static class ReporterMgr
    {
        private static Dictionary<string, Reporter> reporters = new Dictionary<string, Reporter>();

        public static Reporter NewReporter(string _name)
        {
            if (reporters.ContainsKey(_name))
                return null;

            Reporter reporter = new Reporter();
            reporters.Add(_name, reporter);
            return reporter;
        }

        public static void DeleteReporter(string _name)
        {
            if (!reporters.ContainsKey(_name))
                return;
            reporters.Remove(_name);
        }

        public static Reporter Find(string _name)
        {
            if (!reporters.ContainsKey(_name))
                return null;

            return reporters[_name];
        }

        public static void Run()
        {
            foreach (Reporter reporter in reporters.Values)
                reporter.Run();
        }

        public static void Stop()
        {
            foreach (Reporter reporter in reporters.Values)
                reporter.Stop();
        }
    }
}//XLobby.Manager
