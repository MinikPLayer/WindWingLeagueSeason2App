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
            switch(id)
            {
                case "Leaderboards":
                    // number: {name;points}
                    //return "3{(Michael,100);(Thomas,200);(Maria,20)}";
                    return "25{(Quorthon,161);(Minik,119);(kypE,116);(BARTEQ,75);(Skomek,74);(Rogar2630,62);(Patryk913,58);(Giro,56);(Yomonoe,40);(Copy JR,39);(R4zor,37);(slepypirat,34);(koczejk,26);(Shiffer,26);(cichy7220,23);(Allu,21);(Myslav,12);(Hokejode,11);(Paw3lo,10);(Lewandor,6);(xVenox,1);(Kamilos61,0);(Grok12,-1);([SOL]NikoMon,-2);(Bany,-2)}";

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
