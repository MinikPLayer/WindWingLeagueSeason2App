using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Network;
using Network.Converter;
using Network.Packets;

namespace WindWingLeagueSeason2App
{
    public class NetworkData
    {
        string ip;
        short port;

        TcpConnection connection;

        bool connected = false;

        async Task<string> ConnectionError()
        {
            Debug.Log("[ERROR] Is not ready");
            bool value = await App.Current.MainPage.DisplayAlert("Błąd połączenia", "Nie udalo się połączyć z serwerem, sprobuj ponownie później", "Spróbuj ponownie", "Wyjdź");
            if (!value)
            {
                Environment.Exit(1);
                return "NC;Brak połączenia";
            }
            else
            {
                Connect();
                return "NC;Brak połączenia";
            }
        }

        async Task<string> SendAndGetResponse(string key, string data)
        {
            //Debug.Log("Waiting for response... Connected: " + connected.ToString());
            if(!connected || !connection.IsAlive)
            {
                return await ConnectionError();
            }

            //Debug.Log("Registering data handler");

            bool ready = false;
            string response = "";
            connection.RegisterRawDataHandler(key, (rawData, connection) =>
            {
                ready = true;
                response = RawDataConverter.ToUTF16_LittleEndian_String(rawData);
            });

            //Debug.Log("Registered data handler\nSending data with key: \"" + key + "\" and data: \"" + data + "\"");

            Debug.Log("Sending \"" + key + "\" and data: \"" + data + "\"");
            connection.SendRawData(RawDataConverter.FromUTF16_LittleEndian_String(key, data));

            //Debug.Log("Sent");

            const int delay = 10;

            for(int i = 0;i<5000 / delay && !ready;i++)
            {
                await Task.Delay(delay);
            }

            connection.UnRegisterRawDataHandler(key);

            if (!ready)
            {
                await ConnectionError();
            }


            

            return response;
        }

        public async Task<string> RequestAsync(string id)
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

            /*switch (data[0])
            {
                case "Leaderboards":
                    return await SendAndGetResponse("Leaderboards", rest);

                case "Login":
                    return await SendAndGetResponse("Login", rest);

                case "SeasonsCount":
                    return await SendAndGetResponse("Info", "SC");

                case "Info":
                    return await SendAndGetResponse("Info", rest);

                default:
                    return await SendAndGetResponse(data[0], rest);
            }*/

            return await SendAndGetResponse(data[0], rest);


        }

        public string Request(string id)
        {
            return RequestAsync(id).Result;
        }

        public NetworkData(string ip, short port)
        {
            this.ip = ip;
            this.port = port;

            //ClientConnectionContainer container = ConnectionFactory.CreateClientConnectionContainer(ip, port);
            //container.ConnectionEstablished += Container_ConnectionEstablished;
            //container.ConnectionLost += Container_ConnectionLost;
            Connect();
        }

        async Task Connect()
        {
            if (connected) return;
            connection = ConnectionFactory.CreateTcpConnection(ip, port, out ConnectionResult connectionResult);

            if (connectionResult != ConnectionResult.Connected)
            {
                Debug.Log("[ERROR] Connection error: " + connectionResult);
                return;
            }
            connected = true;
            connection.ConnectionClosed += Container_ConnectionLost;
        }

        ~NetworkData()
        {
            connection.Close(Network.Enums.CloseReason.ClientClosed);
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
        }
    }
}
