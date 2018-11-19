﻿using Shared.Contract;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

/// <summary>
/// Wir haben eine TcpCommunication eine klasse die die Verbindung zwischen Server und client regelt 
/// </summary>
namespace Shared.Communications
{
    public class TcpCommunication : ICommunication
    {
        public TcpClient _client { get; set; }
        private NetworkStream _nwStream;
        private bool _NWStreamNotSet = true;

        private MemoryStream _localBuffer;
        private List<DataPackage> _packageQueue;

        private readonly object _lock;
        private BackgroundWorker _backgroundWorker;
        private BackgroundWorker _backgroundWorker2;

        public TcpCommunication()
            : this(new TcpClient())
        { }

        public TcpCommunication(TcpClient client)
        {
            _client = client;
            _packageQueue = new List<DataPackage>();

            _lock = new object();

            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.DoWork += (_, __) => CheckNWStreamUpdates();
            _backgroundWorker.RunWorkerAsync();

            _backgroundWorker2 = new BackgroundWorker();
            _backgroundWorker2.DoWork += (_, __) => GetStream();
            _backgroundWorker2.RunWorkerAsync();


        }

        private void GetStream()
        {
            while (_NWStreamNotSet)
                try
                {
                    _nwStream = _client.GetStream();
                    _NWStreamNotSet = false;
                }
                catch
                {
                    return;
                }
        }

        public void SetNWStream()
        {
            _nwStream = _client.GetStream();
        }


        public void AddPackage(DataPackage dataPackage)
        {
            lock (_lock)
                _packageQueue.Add(dataPackage);
        }



        //schaut ob Pakete da sind
        public bool IsDataAvailable()
        {
            lock (_lock)
                return _packageQueue.Count != 0;
        }

        //holt die Pakete 
        public DataPackage Receive()
        {
            lock (_lock)
            {
                //nimm des erste package au sder liste und return es 
                var element = _packageQueue.First();
                _packageQueue.RemoveAt(0);
                return element;
            }
        }


        //eine Bessere Variante aber kompleziert hab es nicht verstanden deswegen IsDataAvailable und Recieve
        public void ReceiveCallback(Action<DataPackage> receiveCallback)
        {
            throw new NotImplementedException();
        }

        //Sendet Datenpackete
        public void Send(DataPackage data)
        {
            try
            {
                //_nwStream = _client.GetStream();
                var bytesToSend = data.ToByteArray();
                Console.WriteLine($"Sending : Header: {data.Header}, Size: {data.Size}, Payload: {data.Payload}");

                _nwStream.Write(bytesToSend, 0, bytesToSend.Length);
            }

            catch
            {
                Console.WriteLine("You are no longer connected to the server.");
            }

        }

        //läuft die ganze Zeit im Hintergrund
        private void CheckNWStreamUpdates()
        {
            //if (_NWStreamNotSet)
            //    return;

            while (true)
            {
                // prüft ob daten auf dem NetworkStream sind 
                //nicht sicher ob des schon so funktioniert wie es soll
                try
                {
                    if (_nwStream != null && _nwStream.DataAvailable)
                    {
                        ReadDataToBuffer();
                        CheckForNewPackages();
                    }
                    else
                        //wichtig sonst ript dein PC
                        Thread.Sleep(1);
                }
                catch
                {
                    return;
                }
            }
        }

        private void CheckForNewPackages()
        {
            if (_nwStream == null)
                return;
            //unser Data package besteht aus einem Header einem Payload und einer Size 
            //Size ist und soll ein Int32 nicht vereinfachen weil es müssen 4 bit sein
            //die Zeile schaut ob der MeomryStream schon 8bit groß ist das wäre dann die Size und der Header

            if (_localBuffer.Length < 2 * sizeof(Int32))
                return;

            _localBuffer.Seek(0, SeekOrigin.Begin);

            using (var reader = new BinaryReader(_localBuffer))
            {
                var package = new DataPackage();
                //schreibt die size in unser package 
                package.Size = reader.ReadInt32();
                //schreibt den Header in unser package 
                //ProtocolAction ist ein Enum weil wir ja eigentlich Strings wollten des ist aber kacke 
                //des wegen ein Enum mit int und Sprechenden Namen
                package.Header = (ProtocolActionEnum)reader.ReadInt32();
                // wenn size z.B. 100 <= MeomryStream Size ist z.B. 101
                // enthält der Stream ja ein Paket
                if (package.Size <= _localBuffer.Length)
                {
                    // da wir genau wissen das 8 bit size und header sind legen wir die Position auf danach
                    _localBuffer.Position = 2 * sizeof(Int32);

                    //absolut keine Ahnung hat was mit den Daten zu tun er hatte beim ausprobieren 
                    //glaub 4 Zeichen zu wenig und des war der fix 
                    var sizeOfPayload = package.Size - 2 * sizeof(Int32);

                    //Byte Array mit in der Größe des Payloads
                    var bytesToRead = new byte[sizeOfPayload];
                    //wird aus dem MemoryStream rausgelesen
                    _localBuffer.Read(bytesToRead, 0, sizeOfPayload);
                    //aus byte mach String
                    package.Payload = Encoding.ASCII.GetString(bytesToRead, 0, bytesToRead.Length);

                    lock (_lock)
                        //fertiges package wird an die Queue gehängt
                        _packageQueue.Add(package);
                }
            }
        }

        private void ReadDataToBuffer()
        {

            if (_nwStream == null)
                return;

            //die Größe der Daten
            var bytesToRead = new byte[_client.ReceiveBufferSize];
            //empfangen der Daten           
            _nwStream.Read(bytesToRead, 0, _client.ReceiveBufferSize);

            //fix of following problem: A player couldn't execute 2 or more commands 
            //because the TcpConnection tried to use the same MemoryStream. 
            //But every MemoryStream that was used will be disposed of, 
            //because of the  "using" statement in the "CheckForNewPackages" Method. 
            //using is the same as a try block followed by a finally block with "my_memory_stream.Dispose();" in it.
            _localBuffer = new MemoryStream();
            //immer am Anfang anfagen Willst ja "Hallo" und nicht "allo"
            _localBuffer.Seek(0, SeekOrigin.End);
            //schreibt die Daten in ein MemoryStream unsere warteschlange so zu sagen
            _localBuffer.Write(bytesToRead, 0, bytesToRead.Length);
        }


    }
}
