using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Net.NetworkInformation;
using System.Net.PeerToPeer;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace P2P
{
    public class Messager: IDisposable
    {
        private P2PService localService;
        private string serviceUrl;
        private ServiceHost host;
        private PeerName peerName;
        private PeerNameRegistration peerNameRegistration;
        private List<PeerEntry> peerList;

        public event EventHandler<EventArgs> PeersAreFound;

        public Messager()
        {
            peerList = new List<PeerEntry>();

            // Получение конфигурационной информации из app.config
            string port = ConfigurationManager.AppSettings["port"];
            string username = ConfigurationManager.AppSettings["username"];
            string machineName = Environment.MachineName;
            string serviceUrl = null;

            //  Получение URL-адреса службы с использованием адресаIPv4 
            //  и порта из конфигурационного файла
            foreach (IPAddress address in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    serviceUrl = string.Format("net.tcp://{0}:{1}/P2PService", address, port);
                    break;
                }
            }

            // Выполнение проверки, не является ли адрес null
            if (serviceUrl == null)
            {
                // Отображение ошибки и завершение работы приложения
                throw new Exception("Networking Error - Не удается определить адрес конечной точки WCF");
            }

            // Регистрация и запуск службы WCF
            localService = new P2PService(this, username);
            host = new ServiceHost(localService, new Uri(serviceUrl));
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.None;
            host.AddServiceEndpoint(typeof(IP2PService), binding, serviceUrl);
            host.Open();
            Console.WriteLine("Service {0} is opened", serviceUrl);

            // Создание имени равноправного участника (пира)
            string name = "P2P " + ConfigurationManager.AppSettings["username"];
            peerName = new PeerName(name, PeerNameType.Unsecured);

            // Подготовка процесса регистрации имени равноправного участника в локальном облаке
            peerNameRegistration = new PeerNameRegistration(peerName, int.Parse(port));
            peerNameRegistration.Cloud = Cloud.AllLinkLocal;

            // Запуск процесса регистрации
            peerNameRegistration.Start();
            Console.WriteLine("Registration process is started");
        }

        public void FindPeers()
        {
            // Создание распознавателя и добавление обработчиков событий
            PeerNameResolver resolver = new PeerNameResolver();
            resolver.ResolveProgressChanged += resolver_ResolveProgressChanged;
            resolver.ResolveCompleted += resolver_ResolveCompleted;
            
            // Преобразование незащищенных имен пиров асинхронным образом
            resolver.ResolveAsync(new PeerName("0.P2P Sample"), 1);
        }

        void resolver_ResolveCompleted(object sender, ResolveCompletedEventArgs e)
        {
            // Сообщение об ошибке, если в облаке не найдены пиры
            //if (peerList.Count == 0)
            //{
                //peerList.Add(
                //   new PeerEntry
                //   {
                //       DisplayString = "Peers not found"
                //   });
                //Console.WriteLine("No Peers found");
            //}
        }

        void resolver_ResolveProgressChanged(object sender, ResolveProgressChangedEventArgs e)
        {
            PeerNameRecord peer = e.PeerNameRecord;

            foreach (IPEndPoint ep in peer.EndPointCollection)
            {
                if (ep.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    try
                    {
                        string endpointUrl = string.Format("net.tcp://{0}:{1}/P2PService", ep.Address, ep.Port);
                        NetTcpBinding binding = new NetTcpBinding();
                        binding.Security.Mode = SecurityMode.None;
                        IP2PService serviceProxy = ChannelFactory<IP2PService>.CreateChannel(
                            binding, new EndpointAddress(endpointUrl));
                        peerList.Add(
                           new PeerEntry
                           {
                               PeerName = peer.PeerName,
                               ServiceProxy = serviceProxy,
                               DisplayString = serviceProxy.GetName()

                           });
                        Console.WriteLine("Peer {0} is found", peer.PeerName);
                    }
                    catch (EndpointNotFoundException)
                    {
                        //peerList.Add(
                        //   new PeerEntry
                        //   {
                        //       PeerName = peer.PeerName,
                        //       DisplayString = "Unknown type"
                        //   });
                        Console.WriteLine("Peer {0} of unknown type is found", peer.PeerName);
                    }
                }
            }

            if (peerList.Count > 0)
            {
                PeersAreFound?.Invoke(this, EventArgs.Empty);
            }
        }

    public void SendMessageToPeer(string message)
        {
            try
            {
                foreach (PeerEntry peer in peerList)
                {
                    peer.ServiceProxy.SendMessage(message, ConfigurationManager.AppSettings["username"]);
                }
            }
            catch (CommunicationException e)
            {
                Console.WriteLine(e);
            }
        }

        internal void DisplayMessage(string message, string from)
        {
            // Показать полученное сообщение (вызывается из службы WCF)
            Console.WriteLine("Сообщение от {0} - {1}", from, message);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // Остановка регистрации
                peerNameRegistration.Stop();

                // Остановка WCF-сервиса
                host.Close();

                disposedValue = true;
            }
        }

        ~Messager()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
