using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LundsLångLager
{
    internal class EventFileManager
    {
        readonly private string filePath = "StoolEventLog.txt";
        public EventFileManager()
        {
            MakeSureSaveFileExists();
        }

        private void MakeSureSaveFileExists() //Vet inte om denna metoden ens behövs, men den är här för att
        {                                     //se till att textfilen existerar när den behöver existera.
            if (!File.Exists(filePath))
            {
                var file = File.Create(filePath);
                file.Close(); //programmet kraschar om man försöker skriva i filen utan att stänga den efter man
            }                 //har skapat den.
        }

        public void RegisterStoolStoreEvent(Stool stool)
        {
            try
            {
                StreamWriter streamWriter = new StreamWriter(filePath, true);
                using (streamWriter)
                {
                    streamWriter.WriteLine("STOOL STORED:");
                    streamWriter.WriteLine("Time it got stored: {0}", stool.ArrivalTime);
                    streamWriter.WriteLine("ID: {0}", stool.StoolID);
                    streamWriter.WriteLine("Stool type: {0}\n", stool.StoolType);
                }
            }
            catch
            {
                Console.WriteLine("ERROR: UNABLE TO SAVE STOOL STORAGE ACTION DATA IN TEXTFILE");
            }
        }

        public void RegisterStoolRetrieveEvent(Stool stool, int wholeStoolCost, int halfStoolCost)
        {
            try
            {
                StreamWriter streamWriter = new StreamWriter(filePath, true);
                using (streamWriter)
                {
                    streamWriter.WriteLine("STOOL RETRIEVED:");
                    streamWriter.WriteLine("ID: {0}", stool.StoolID);
                    streamWriter.WriteLine("Stool type: {0}", stool.StoolType);
                    streamWriter.WriteLine("Time it got stored: {0}", stool.ArrivalTime);
                    streamWriter.WriteLine("Time it got retrieved: {0}", DateTime.Now);
                    streamWriter.WriteLine("Time in storage: {0}", stool.GetStorageTimespan());
                    streamWriter.WriteLine("Total cost: {0}\n", stool.GetTotalStorageCost(wholeStoolCost, halfStoolCost));
                }
            }
            catch
            {
                Console.WriteLine("ERROR: UNABLE TO SAVE STOOL RETRIEVAL ACTION DATA IN TEXTFILE");
            }
        }
    }
}
