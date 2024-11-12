using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LundsLångLager
{
    internal class StoolFactory
    {
        private int idNumComponent = 0;
        private string idLetterComponent = "AA";

        public Stool MakeNewStool() //Två konstruktorer, ena för random palltyp
        {
            return new Stool(AssignStoolID(), AssignStoolType());
        }

        public Stool MakeNewStool(StoolType stoolType)
        {
            return new Stool(AssignStoolID(), stoolType);
        }

        private string AssignStoolID() //Jag kan inte ha två pallar med samma ID med denna kod
        {
            idNumComponent++;
            if (idNumComponent > 999)
                idNumComponent = 1;
            string strIdNumComponent = $"{idNumComponent:D3}"; //Output: 001 - 999
            string stoolID = idLetterComponent + strIdNumComponent;
            return stoolID;
        }

        private StoolType AssignStoolType() //random StoolType
        {
            Random random = new Random();
            if (random.Next(0, 2) == 0)
                return StoolType.Whole;
            else
                return StoolType.Half;
        }
    }
}
