using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WindWingLeagueSeason2App
{
    public class NetworkData
    {
        public async Task<string> RequestAsync(string id)
        {
            await Task.Delay(500);

            string[] data = id.Split(';');

            if(data.Length == 0)
            {
                return "EC";
            }

            switch(data[0])
            {
                case "Leaderboards":
                    // number: {name;points}
                    //return "3{(Michael,100);(Thomas,200);(Maria,20)}";
                    return "25{(Quorthon,162,RDB);(Minik,127,MCL);(kypE,134,RDB);(BARTEQ,75,HAS);(Skomek,80,TRS);(Rogar2630,61,FRI);(Patryk913,84,TRS);(Giro,68,ARO);(Yomonoe,44,MCL);(Copy JR,39,RNL);(R4zor,37,MER);(slepypirat,34,RPT);(koczejk,26,HAS);(Shiffer,26,RPT);(cichy7220,23,MER);(Allu,21,OTH);(Myslav,14,OTH);(Hokejode,11,ARO);(Paw3lo,10,OTH);(Lewandor,16,OTH);(xVenox,1,OTH);(Kamilos61,0,RNL);(Grok12,-1,FRI);([SOL]NikoMon,-2,OTH);(Bany,-2,OTH)}";

                case "Login":
                    if(data.Length < 3)
                    {
                        return "BI";
                    }
                    return "OK;" + data[1]; // "OK;username"

                default:
                    return "NF";
            }
        }

        public string Request(string id)
        {
            return RequestAsync(id).Result;
        }
    }
}
