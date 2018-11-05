using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

/// <summary>
/// Wir haben eine TcpCommunication eine klasse die die Verbindung zwischen Server und client regelt 
/// 
/// </summary>
namespace TCP_Model
{
    public class TcpCommunication : ICommunication
    {
        // Wir arbeiten mit TcpClient und NetworkStream
        private TcpClient _client;
        private NetworkStream _nwStream;

        //Der Memory und die Liste ist um DatenPakete richtig zu empfangen
        private MemoryStream _localBuffer;
        private List<DataPackage> _packageQueue;

        //Das Lock ist ein Scloss um beim Multithreading keine Probleme zu bekommen
        // wenn ein codeabschnitt von einem Treahd bearbeitet wird darf kein anderer drauf zugreifen
        private readonly object _lock;
        // Der Backgroundworker macht das genau das was im Namen steht
        private BackgroundWorker _backgroundWorker;


        public TcpCommunication()
            : this(new TcpClient("127.0.0.1", 8080))
        { }

        public TcpCommunication(TcpClient client)
        {
            _client = client;
            _nwStream = _client.GetStream();

            _localBuffer = new MemoryStream();
            _packageQueue = new List<DataPackage>();

            _lock = new object();
            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.DoWork += (_, __) => CheckForUpdates();
            _backgroundWorker.RunWorkerAsync();
        }

        //schaut ob Pakete da sind
        public bool IsDataAvailable()
        {
            lock(_lock)
                return _packageQueue.Count != 0;
        }

        //holt die Pakete 
        public DataPackage Receive()
        {
            lock (_lock)
            {
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
            _nwStream = _client.GetStream();
            var bytesToSend = data.ToByteArray();
            Console.WriteLine($"Sending : Header: {data.Header}, Size: {data.Size}, Payload: {data.Payload}");
            _nwStream.Write(bytesToSend, 0, bytesToSend.Length);
        }

        //läuft die ganze Zeit im Hintergrund
        private void CheckForUpdates()
        {
            while (true)
            {
                if (_nwStream.DataAvailable)
                {
                    ReadDataToBuffer();
                    CheckForNewPackages();
                }
                else
                    //wichtig sonst ript dein PC
                    Thread.Sleep(1);
            }

        }

        //jetzt wirds komplieziert
        private void CheckForNewPackages()
        {
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
                package.Header = (ProtocolAction)reader.ReadInt32();
                // wenn size z.B. 100 <= MeomryStream Size ist z.B.
                if (package.Size <= _localBuffer.Length )
                {
                    _localBuffer.Position = 2 * sizeof(Int32);

                    var sizeOfPayload = package.Size - 2 * sizeof(Int32);
                    var bytesToRead = new byte[sizeOfPayload];
                    var bytesRead = _localBuffer.Read(bytesToRead, 0, sizeOfPayload);
                    package.Payload = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);

                    lock(_lock)
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
            var bytesRead = _nwStream.Read(bytesToRead, 0, _client.ReceiveBufferSize);
            //immer am Anfang anfagen Willst ja "Hallo" und "allo"
            _localBuffer.Seek(0, SeekOrigin.End);
            //schreibt die Daten in ein MemoryStream unsere warteschlange so zu sagen
            _localBuffer.Write(bytesToRead, 0, bytesRead);
        }
    }
}
