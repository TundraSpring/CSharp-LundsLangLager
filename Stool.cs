using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LundsLångLager
{
    internal class Stool
    {
        private string stoolID;
        private StoolType stoolType;
        private DateTime arrivalTime;

        public string StoolID
        {
            get
            {
                return stoolID;
            }
        }

        public StoolType StoolType
        {
            get
            {
                return stoolType;
            }   //Har ingen set-property för vilken klass som helst borde
        }       //inte kunna ändra på vad för palltyp det här

        public DateTime ArrivalTime
        {
            get
            {
                return arrivalTime;
            }
            set
            {
                arrivalTime = value;
            }
        }

        public Stool(string stoolID, StoolType stoolType)
        {
            this.stoolID = stoolID;          //Man kan inte skapa en pall utan att ge den id
            this.stoolType = stoolType;      //eller palltyp
        }

        public TimeSpan GetStorageTimespan()     //Denna koden var originellt i lager-klassen (LundsLångLager)
        {                                        //Men när den behövdes på andra ställen så lades den här istället
            if (arrivalTime == new DateTime())   //Samma med metoden nedan
                return new TimeSpan();
            else
                return DateTime.Now.Subtract(arrivalTime);
        }

        public int GetTotalStorageCost(int wholeStoolCost, int halfStoolCost)
        {
            int costPerHour;
            if (stoolType == StoolType.Whole)
                costPerHour = wholeStoolCost;
            else
                costPerHour = halfStoolCost;
            return (GetStorageTimespan().Hours + 1) * costPerHour;
        }
    }
}
