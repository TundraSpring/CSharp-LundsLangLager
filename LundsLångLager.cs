using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LundsLångLager
{
    internal class LundsLångLager
    {
        private Stool[,] storage = new Stool[20, 2];
        private int wholeStoolCost = 75; //Dessa två är fältvärden, på det sättet
        private int halfStoolCost = 39;  //kan man ändra kostnaden överallt på samma
                                         //gång genom att bara ändra på dessa värden

        public int WholeStoolCost 
        {
            get
            {
                return wholeStoolCost;
            }
        }

        public int HalfStoolCost //LundsLångLager är den enda klassen som borde komma ihåg priset för att förvara
        {                        //Pallar på Lunds Långlager, därför så behöver det skickas genom get-property
            get                  //när andra klasser behöver denna information.
            {
                return halfStoolCost;
            }
        }

        public bool TryToStoreNewStool(Stool newStool)
        {
            bool emptySpotIsFilled = false;
            for (int slot = 0; slot < storage.GetLength(0); slot++)
            {
                emptySpotIsFilled = TryToPutStoolInStorageSlot(slot, newStool);
                if (emptySpotIsFilled)
                    break;
            }
            return emptySpotIsFilled;
        }

        private bool TryToPutStoolInStorageSlot(int slot, Stool newStool)
        {
            bool emptySpotIsFilled;
            emptySpotIsFilled = TryToPutStoolInFirstSubslot(slot, newStool); //En lagerplats kallas för "slot", och
                                                                             //består av två värden, "subslots".
            if (!emptySpotIsFilled && newStool.StoolType == StoolType.Half)  //Två halvpallar på samma slot förvaras
            {                                                                //i var sin subslot.
                emptySpotIsFilled = TryToPutStoolInHalfOccupiedSlot(slot, newStool);
            }
            return emptySpotIsFilled;
        }

        private bool TryToPutStoolInFirstSubslot(int slot, Stool newStool)
        {
            if (storage[slot, 0] == null)
            {
                newStool.ArrivalTime = DateTime.Now;
                storage[slot, 0] = newStool;  //Första subslot'en är storage[slot, 0]
                return true;                  //Andra  subslot'en är storage[slot, 1]
            }
            else
                return false;
        }

        private bool TryToPutStoolInHalfOccupiedSlot(int slot, Stool newStool)
        {
            if (storage[slot, 0].StoolType == StoolType.Half && storage[slot, 1] == null)
            {
                newStool.ArrivalTime = DateTime.Now;
                storage[slot, 1] = newStool; //Andra subslot'en kan bara vara tomma eller
                return true;                 //innehålla halvpallar
            }
            else
                return false;
        }

        public Stool? TryToRetrieveStool(string requestedStoolID)
        {
            (int slot, int subslot) position = SearchForStoolPosition(requestedStoolID);
            if (position.slot != -1)
                return RetrieveStool(position.slot, position.subslot);
            else
                return null;
        }

        private Stool RetrieveStool(int slot, int subslot)
        {
            Stool retrievedStool = storage[slot, subslot];
            TryToFillHalfEmptySlotWithRemainingHalfStool(slot, subslot);
            return retrievedStool;
        }

        private void TryToFillHalfEmptySlotWithRemainingHalfStool(int slot, int subslot)
        {
            if (subslot == 0 && storage[slot, 1] != null)
            {
                storage[slot, 0] = storage[slot, 1]; //Mina variabelnamn är på engelska. Jag
                storage[slot, 1] = null;             //tror de förväntas vara det i professionella
            }                                        //sammanhang
            else
            {
                storage[slot, subslot] = null;
            }
        }

        public (StoolType, int position)? TryToGetStoolAndPositionWithStoolID(string requestedStoolID)
        {
            (int slot, int subslot) position = SearchForStoolPosition(requestedStoolID);
            if (position.slot != -1)
                return (storage[position.slot, position.subslot].StoolType, position.slot + 1);
            else
                return null; //Att returnera null anses fult men för ett relativt litet projekt som detta
        }                    //så är det inte viktigt att tänka på

        private (int slot, int subslot) SearchForStoolPosition(string requestedStoolID) //Tuple används för att returnera
        {                                                                               //mer än ett värde
            for (int slot = 0; slot < storage.GetLength(0); slot++)
            {
                for (int subslot = 0; subslot < storage.GetLength(1); subslot++)
                {
                    if (storage[slot, subslot] != null)
                    {
                        if (storage[slot, subslot].StoolID == requestedStoolID)
                        {
                            return (slot, subslot); //Denna kodsatsen ser dålig ut men jag hittade
                        }                           //inget alternativ som jag tyckte var bättre, så
                    }                               //den fick duga
                }
            }
            return (-1, -1);
        }

        public void CompressHalfEmptySlots() //Denna metoden har void som returtyp för att
        {                                    //programmet kan enbart krasha här om jag har kodat det fel
            for (int slot = 0; slot < storage.GetLength(0); slot++) 
            {
                (int slot, int subslot) emptySubslot = TryToFindFirstEmptySubslotInStorage();
                if (emptySubslot.slot != -1)
                {
                    Stool? stool = TryToRetrieveLastHalfStool(emptySubslot.slot, emptySubslot.subslot);
                    if (stool != null)
                    {
                        storage[emptySubslot.slot, emptySubslot.subslot] = stool;
                    }
                }
            }
        }

        private (int slot, int subslot) TryToFindFirstEmptySubslotInStorage() //Jag tycker det är värt att ha metodnamn
        {                                                                     //som dessa om de är väldigt tydliga
            for (int slot = 0; slot < storage.GetLength(0); slot++)           //med vad dem gör
            {
                if (storage[slot, 0] == null)
                {
                    return (slot, 0);
                }
                else if (storage[slot, 0].StoolType == StoolType.Half && storage[slot, 1] == null)
                {
                    return (slot, 1);
                }
            }
            return (-1, -1);
        }

        private Stool? TryToRetrieveLastHalfStool(int emptySlot, int emptySubslot)
        {
            for (int slot = storage.GetLength(0) - 1; slot >= 0 ; slot--)
            {
                for (int subslot = storage.GetLength(1) - 1; subslot >= 0; subslot--)
                {
                    if (storage[slot, subslot] != null)
                    {
                        bool isHalfStool = (storage[slot, subslot].StoolType == StoolType.Half);
                        bool notBeforeEmptySlot = ThisSlotComesAfterOtherSlot(slot, subslot, emptySlot, emptySubslot);
                        if (isHalfStool && notBeforeEmptySlot)
                            return RetrieveStool(slot, subslot);
                    }
                }
            }
            return null;
        }

        private bool ThisSlotComesAfterOtherSlot(int slot, int subslot, int otherSlot, int otherSubslot)
        {
            if (otherSlot < slot || (otherSlot == slot && otherSubslot < subslot))
                return true;   //Den här metoden skapades så att kodsatsen på rad 177-180 skulle
            else               //bli enklare att förstå. Det tar mer utrymme, men jag tycker det
                return false;  //värt det.
        }

        public bool TryToMoveStoolToRequestedSlot(string requestedStoolID, int i)
        {
            (int i, int j) currentPosition = SearchForStoolPosition(requestedStoolID);
            if (currentPosition.i != -1)
            {
                Stool requestedStool = storage[currentPosition.i, currentPosition.j]; //A copy of the stool is created.
                bool successfulTransfer = TryToPutStoolInStorageSlot(i, requestedStool);
                if (successfulTransfer)
                    RetrieveStool(currentPosition.i, currentPosition.j); //If it is successfully put into the slot,
                return successfulTransfer;                               //then the original is erased.
            }                                                            //This was the least convoluted way of doing this
            return false;
        }                                                                //Kommentarerna i engelska ovan är de enda två
                                                                         //som jag skrev innan jag la till kommentarer
                                                                         //för presentationen
        public Stool[,] ReturnStorageArray() 
        {
            return storage;
        }

        public int GetStorageCapacity()
        {
            return storage.GetLength(0);
        }
    }
}
