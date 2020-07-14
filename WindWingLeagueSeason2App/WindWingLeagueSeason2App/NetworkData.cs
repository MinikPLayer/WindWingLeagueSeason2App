using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Network;
using Network.Converter;
using Network.Packets;
using Xamarin.Forms;

namespace WindWingLeagueSeason2App
{
    public class NetworkData
    {
        string ip;
        short port;

        TcpConnection connection;

        bool connectedInternal = false;

        public bool connected = false;
        public bool connectionLost = false;

        public int timeout = 5000;

        class Packet
        {
            public string key = "";
            public string data = "";
        }

        List<Packet> packets = new List<Packet>();

        async Task<bool> ConnectionError()
        {
            try
            {

                //Debug.Log("[ERROR] Connection error");
#if DEBUG
                bool value = await Views.MainPage.singleton.DisplayAlert("Błąd połączenia", "Nie udalo się połączyć z serwerem, sprobuj ponownie później", "Więcej informacji", "Spróbuj ponownie");
                if (value)
                {
                    string packetsStr = "";
                    for(int i = 0;i<packets.Count;i++)
                    {
                        packetsStr += packets[i].key + ":" + packets[i].data + "\n";
                    }
                    await Views.MainPage.singleton.DisplayAlert("Błąd połączenia", packetsStr, "OK");
                }
                return await Connect(true);
#else
                bool value = await Views.MainPage.singleton.DisplayAlert("Błąd połączenia", "Nie udalo się połączyć z serwerem, sprobuj ponownie później", "Wyjdź", "Spróbuj ponownie");
                if (value)
                {
                    Environment.Exit(-1);
                    return true;
                }
                else
                {
                    return await Connect(true);
                }
#endif

                

            }
            catch(Exception e)
            {
                Debug.Log("[NetworkData.ConnectionError] Crashed? " + e.ToString());
                return false;
            }
              
        }

        bool receivingData = false;
        async Task<string> SendAndGetResponse(string key, string data, bool force = false)
        {
            Packet p = new Packet { key = key, data = data };
            packets.Add(p);
            if((!connected || !connection.IsAlive) && !connectedInternal && !force)
            {
                while(!await ConnectionError() && !connected)
                {
                    await Task.Delay(100);
                }
            }

            while(receivingData)
            {
                await Task.Delay(10);
            }

            receivingData = true;
            bool ready = false;
            string response = "";
            connection.RegisterRawDataHandler(key, (rawData, connection) =>
            {
                ready = true;
                response = RawDataConverter.ToUTF16_LittleEndian_String(rawData);
            });

            Debug.Log("Sending \"" + key + "\" and data: \"" + data + "\"");
            connection.SendRawData(RawDataConverter.FromUTF16_LittleEndian_String(key, data));


            const int delay = 10;

            for(int i = 0;i< timeout / delay && !ready;i++)
            {
                await Task.Delay(delay);
            }

            connection.UnRegisterRawDataHandler(key);

            if (!ready)
            {
                await ConnectionError();
            }


            packets.Remove(p);
            receivingData = false;
            return response;
        }

        public async Task<string> RequestAsync(string id, bool force = false)
        {
            string[] data = id.Split(';');
            string key = data[0];
            string rest = "";
            for(int i = 1;i<data.Length;i++)
            {
                rest += data[i];
                if(i != data.Length - 1)
                {
                    rest += ';';
                }
            }

            if (data.Length == 0)
            {
                return "EC;Pusty pakiet";
            }

            return await SendAndGetResponse(data[0], rest, force);

        }

        public string Request(string id)
        {
            return RequestAsync(id).Result;
        }

        public NetworkData(string ip, short port)
        {
            this.ip = ip;
            this.port = port;

            Connect();
        }

        async Task<bool> Connect(bool reLogin = false)
        {
            if (connected) return true;
            connection = ConnectionFactory.CreateTcpConnection(ip, port, out ConnectionResult connectionResult);

            if (connectionResult != ConnectionResult.Connected)
            {
                Debug.Log("[ERROR] Connection error: " + connectionResult);
                return false;
            }

            connection.ConnectionClosed += Container_ConnectionLost;


            string response = await RequestAsync("WP;" + App.appVersion.ToString(), true);
            if (response != "OK")
            {
                bool r = await Views.MainPage.singleton.DisplayAlert("Błąd połączenia", "Błąd przetwarzania połączenia", "Więcej informacji", "OK");
                if (r)
                {
                    await Views.MainPage.singleton.DisplayAlert("Więcej informacji", response, "OK");
                }
                Environment.Exit(10);
                return false;
            }

            connectedInternal = true;

            if (reLogin || connectionLost)
            {
                connectionLost = false;

                if (Views.LoginScreen.loggedIn)
                {
                    while (!await Views.InitScreen.LoginT(Views.LoginScreen.username, Views.LoginScreen.token))
                    {
                        bool result = await Views.MainPage.singleton.DisplayAlert("Błąd", "Nie udało się ponownie zalogować na server po utracie połączenia", "Spróbuj ponownie", "Zaloguj się");

                        if (!result)
                        {
                            connected = true;
                            Views.MainPage.singleton.CloseAllPages();
                            Views.MainPage.singleton.NavigateFromMenu((int)Models.MenuItemType.Login);
                            return false;
                        }
                    }
                }
            }
            else
            {

                //connected = true;
                string serverV = await RequestAsync("Info;ServerVersion");
                string appV = await RequestAsync("Info;AppLatestVersion");
                //connected = false;
                try
                {
                    int protocol = int.Parse(serverV);
                    int latestVersion = int.Parse(appV);

                    if (protocol > App.serverSupportVersion)
                    {
                        bool result = await Views.MainPage.singleton.DisplayAlert("Błąd protokołu servera", "Dostępna krytyczna aktualizacja aplikacji, ta wersja może nie działać poprawnie. Kontynuować?", "OK", "Wyjdź");
                        if (!result)
                        {
                            Environment.Exit(2);
                        }
                    }
                    else if(latestVersion > App.appVersion)
                    {
                        bool result = await Views.MainPage.singleton.DisplayAlert("Dostępna aktualizacja", "Dostępna aktualizacja aplikacji, Kontynuować?", "OK", "Wyjdź");
                        if (!result)
                        {
                            Environment.Exit(2);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Cannot parse server version " + e.ToString());
                    bool result = await Views.MainPage.singleton.DisplayAlert("Błąd wersji", "Nie udało się pobrać informacji o wersji servera, aplikacja może nie działać poprawnie:\n" + e.ToString(), "OK", "Wyjdź");
                    if (!result)
                    {
                        Environment.Exit(2);
                    }
                }
            }
            connected = true;

            return true;
        }

        ~NetworkData()
        {
            connection.Close(Network.Enums.CloseReason.ClientClosed);
        }

        async void ConnectionLost()
        {
            while (!await ConnectionError())
            {
                await Task.Delay(100);
            }
        }

        private void Container_ConnectionLost(Network.Enums.CloseReason closeReason, Connection connection)
        {
            if (connection != null)
            {
                Debug.Log($"Connection lost, closeReason: {closeReason}");
            }
            else
            {
                Debug.Log("Connection lost");
            }

            connected = false;
            //Views.LoginScreen.loggedIn = false;
            connectionLost = true;
            connectedInternal = false;

            //ConnectionLost();
            Device.BeginInvokeOnMainThread(ConnectionLost);
        }
    }
}
