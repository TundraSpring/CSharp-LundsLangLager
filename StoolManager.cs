using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LundsLångLager
{
    internal class StoolManager //Hanterar bara input och output från respektive till användaren.
    {                           //Att skapa pallar, lagra pallar, samt att skriva till fil,
                                //Är det som de tre nedan gör. Detta är ett exempel på objekt-
                                //orienterad programmering ("OOP").
        StoolFactory stoolFactory = new StoolFactory();
        LundsLångLager lundsLångLager = new LundsLångLager();
        EventFileManager eventFileManager = new EventFileManager();

        public StoolManager()
        {
            InitializeStartingStools();
        }
            
        private void InitializeStartingStools() //Det är enklare att återanvända metoder än att
        {                                       //Göra allt det här igen manuellt
            for (int i = 0; i < 3; i++)
                lundsLångLager.TryToStoreNewStool(stoolFactory.MakeNewStool(StoolType.Whole));
            for (int i = 0; i < 3; i++)
                lundsLångLager.TryToStoreNewStool(stoolFactory.MakeNewStool(StoolType.Half));
        }

        public void Run() //Huvudmetoden
        {
            while (true)
            {
                DisplayUserOptions();
                string userInput = GetUserInput();
                bool exitProgram = ApplyUserInput(userInput);
                if (exitProgram)
                    break;         //Jag föredrar att anända if utan måsvingar, för att det tar mindre rader.
                else
                    ClearDisplayAfterUserInput();
            }
        }

        private void DisplayUserOptions()
        {
            Console.WriteLine("Välj ett alternativ:\n" +                    // \n för att få konsolen att hoppa
                              "1: Lämna en pall hos Lunds Långlager\n" +    // till nästa rad
                              "2: Hämta en pall med ett specifikt ID från Lunds Långlager\n" +
                              "3: Sök efter pall med ett specifikt ID hos Lunds Långlager\n" +
                              "4: Flytta pall från en plats till en annan\n" +
                              "5: Skriv ut listan på alla pallar hos Lunds Långlager\n" +
                              "6: Komprimera lagret så att max en halvpall är ensam\n" +
                              "0: Avsluta programmet");
        }

        private string GetUserInput()
        {
            while (true)
            {
                string userInput = Console.ReadLine();
                if (userInput == "1" || userInput == "2" || userInput == "3" || userInput == "4" || userInput == "5" || userInput == "6" || userInput == "0")
                {
                    return userInput; //Denna metoden kommer bara att sluta när en av de sju alternativen är valda
                }
            }
        }

        private bool ApplyUserInput(string userInput)
        {
            bool exitProgram = false;
            switch (userInput)
            {
                case "1":
                    DeliverStool();   
                    break;            
                case "2":             
                    RetrieveStool();
                    break;            
                case "3":            
                    SearchForStool();
                    break;
                case "4":                     //Detta är ett till exempel på OOP-tänkande. Valen leder till sina egna
                    MoveStoolToAnotherSlot(); //metoder, som i sin tur kallar på metoder i de objekten av de andra tre
                    break;                    //klasserna, som i sin tur gör det dem ska. DeliverStool() lägger inte
                case "5":                     //till pallen i lagret, för att lagret hanteras av en annan klass. Metoden
                    PrintAllStorageContent(); //som faktiskt lägger till pallen i lagret finns i lundsLångLager, så därför
                    break;                    //kallas denna metod av DeliverStool, på rad 108
                case "6":
                    lundsLångLager.CompressHalfEmptySlots();   //Denna var den enda som var för kort för att bli gjord
                    Console.WriteLine("Halvpallar sorterade"); //till sin egna metod.
                    break;
                case "0":
                    exitProgram = true;
                    break;
            }
            return exitProgram;
        }

        private void DeliverStool()
        {
            Stool stool = stoolFactory.MakeNewStool();
            bool success = lundsLångLager.TryToStoreNewStool(stool); //Denna raden
            if (success)
            {
                Console.WriteLine("{0} skapad och lämnad", Utilities.StoolTypeToSwedish(stool.StoolType, true));
                eventFileManager.RegisterStoolStoreEvent(stool);
            }
            else
            {
                Console.WriteLine("Lyckades inte lämna {0}, det fanns inte tillräckligt med plats", Utilities.StoolTypeToSwedish(stool.StoolType, false));
            }
        }

        private void RetrieveStool()
        {
            Console.WriteLine("Skriv in ID't på pallen som du vill hämta");
            string requestedStoolId = Console.ReadLine();
            Stool? retrievedStool = lundsLångLager.TryToRetrieveStool(requestedStoolId);
            if (retrievedStool != null)
            {
                string pallTyp = Utilities.StoolTypeToSwedish(retrievedStool.StoolType, false); //Här är ett svenskt
                                                                                                //variabel-namn. Vad tycker
                                                                                                //läraren?
                int cost = retrievedStool.GetTotalStorageCost(lundsLångLager.WholeStoolCost, lundsLångLager.HalfStoolCost);
                Console.WriteLine("Hämtade {0}. Pris: {1}", pallTyp, cost);
                eventFileManager.RegisterStoolRetrieveEvent(retrievedStool, lundsLångLager.WholeStoolCost, lundsLångLager.HalfStoolCost);
            }
            else
                Console.WriteLine("Pallen finns inte");
        }

        private void SearchForStool()
        {
            Console.WriteLine("Skriv in ID't på pallen som du vill söka efter");
            string requestedStoolId = Console.ReadLine();
            (StoolType stoolType, int position)? stoolAndPos = lundsLångLager.TryToGetStoolAndPositionWithStoolID(requestedStoolId);
            if (stoolAndPos != null)
            {
                string sweStoolType = Utilities.StoolTypeToSwedish(stoolAndPos.Value.stoolType, false);
                int position = stoolAndPos.Value.position;
                int capacity = lundsLångLager.GetStorageCapacity();
                Console.WriteLine("Pallen {0} (en {1}) finns på lagerplats {2} av {3}", requestedStoolId, sweStoolType, position, capacity);
            }
            else
                Console.WriteLine("Pallen {0} finns inte i lagret", requestedStoolId);
        }

        private void MoveStoolToAnotherSlot()
        {                                 
            string? requestedStoolID = GetIdToMoveStool();
            if (requestedStoolID == null)
            {
                Console.WriteLine("Pallen finns inte.");
                return; //Här provade jag att använda return istället för flera nästlade if-statements
            }

            int? requestedStoragePosition = GetPositionToMoveStool();
            if (requestedStoragePosition == null)
            {
                Console.WriteLine("Platsen finns inte.");
                return;
            }

            bool success = lundsLångLager.TryToMoveStoolToRequestedSlot(requestedStoolID, (int)requestedStoragePosition - 1);
            if (success)                                                               // ^här^ så behövde jag konvertera
                Console.WriteLine("Flyttade pallen");                                  // int? till int för annars så
            else                                                                       // klagar kompilatorn
                Console.WriteLine("Det fanns inte utrymme för pallen på den platsen");
        }

        private string? GetIdToMoveStool()
        {
            Console.WriteLine("Skriv in ID't på pallen som du vill ändra plats på");
            string requestedStoolID = Console.ReadLine();
            if (lundsLångLager.TryToGetStoolAndPositionWithStoolID(requestedStoolID) != null)
                return requestedStoolID;                     //Vad tycker läraren om denna koden
            else
                return null;
        }

        private int? GetPositionToMoveStool()
        {
            Console.WriteLine("Skriv in platsen i lagret som du vill flytta pallen till (1 - {0})", lundsLångLager.GetStorageCapacity());
            Int32.TryParse(Console.ReadLine(), out int requestedStoragePosition);
            if (requestedStoragePosition >= 1 && requestedStoragePosition <= lundsLångLager.GetStorageCapacity())
                return requestedStoragePosition;
            else
                return null;
        }

        private void PrintAllStorageContent()
        {
            Stool[,] storage = lundsLångLager.ReturnStorageArray();
            Console.WriteLine("Full lista på alla pallar:");
            for (int slot = 0; slot < storage.GetLength(0); slot++)
            {
                Console.Write("Lagerplats {0}:\t", slot + 1);
                if (storage[slot, 0] != null && storage[slot, 1] != null) //Här valde jag att använda tre if-statements
                {                                                         //istället för en nästlad loop
                    Console.Write("ID: {0}, Halvpall, sattes in {1}\n", storage[slot, 0].StoolID, storage[slot, 0].ArrivalTime);
                    Console.Write("\t\tID: {0}, Halvpall, sattes in {1}\n", storage[slot, 1].StoolID, storage[slot, 1].ArrivalTime);
                }
                else if (storage[slot, 0] != null)
                {
                    string pallTyp = Utilities.StoolTypeToSwedish(storage[slot, 0].StoolType, true);
                    Console.Write("ID: {0}, {1}, sattes in {2}\n", storage[slot, 0].StoolID, pallTyp, storage[slot, 0].ArrivalTime);
                }
                else
                {
                    Console.Write("Tom\n");
                }
            }
            Console.WriteLine();
        }

        private void ClearDisplayAfterUserInput()
        {
            Console.ReadLine();
            Console.Clear();
        }
    }
}
