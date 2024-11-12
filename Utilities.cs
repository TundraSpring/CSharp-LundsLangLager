using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LundsLångLager
{
    internal static class Utilities
    {
        public static string StoolTypeToSwedish(StoolType stoolType, bool PascalCase) //Min enda statiska
        {                                                                             //klass och metod
            if (stoolType == StoolType.Whole)
            {
                if (PascalCase)
                    return "Helpall";
                else
                    return "helpall";
            }
            else
            {
                if (PascalCase)
                    return "Halvpall";
                else
                    return "halvpall";
            }
        }



















        public bool PalletSortingFromHell()
        {
            try
            {
                List<Pallet> pallets = GetLonelyHalfPallets();
                if (pallets.Count > 1)
                {
                    int pairCount = pallets.Count / 2;
                    List<int> storageIDs = new List<int>();
                    for (int i = 0; i < pairCount; i++)
                    {
                        storageIDs.Add((int)pallets[i].storageID);
                    }
                    List<int> palletIDs = new List<int>();
                    for (int i = pallets.Count - 1; i < pairCount; i--)
                    {
                        palletIDs.Add(pallets[i].palletID);
                    }
                    SortLonelyHalfPallets(palletIDs, storageIDs);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Pallet> GetLonelyHalfPallets()
        {
            List<Pallet> pallets = new List<Pallet>();

            string STOREDPROCEDURE = "SELECT PalletID, PalletSize, StorageID, ArrivalTime " +
                                     "FROM Pallet p1 " +
                                     "WHERE NOT EXISTS (SELECT 1 FROM Pallet p2 WHERE p2.StorageID = p1.StorageID AND p2.PalletID <> p1.PalletID) AND p1.StorageID IS NOT NULL AND p1.PalletSize = 50";
            using (SqlConnection connection = new SqlConnection(strConnection))
            {
                SqlCommand command = new SqlCommand(STOREDPROCEDURE, connection);

                connection.Open();
                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        int palletID = Convert.ToInt32(dataReader["PalletID"]);
                        int palletSize = Convert.ToInt32(dataReader["PalletSize"]);
                        int? storageID = ConvertDatabaseValueToInt32(dataReader["StorageID"]);
                        DateTime arrivalTime = Convert.ToDateTime(dataReader["ArrivalTime"]);
                        Pallet pallet = new Pallet(palletID, palletSize, storageID, arrivalTime);
                        pallets.Add(pallet);
                    }
                }
                connection.Close();
            }
            return pallets;
        }

        public List<Pallet> SortLonelyHalfPallets(List<int> palletIDs, List<int> storageIDs)
        {
            string STOREDPROCEDURE = "UPDATE Pallet " +
                                     "SET StorageID = @newStorageID " +
                                     "WHERE PalletID = @palletID";
            using (SqlConnection connection = new SqlConnection(strConnection))
            {
                SqlCommand command = new SqlCommand(STOREDPROCEDURE, connection);
                command.Parameters.AddWithValue("@PalletID", requestedPalletID);
                command.ExecuteNonQuery();

                connection.Open();
                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        int palletID = Convert.ToInt32(dataReader["PalletID"]);
                        int palletSize = Convert.ToInt32(dataReader["PalletSize"]);
                        int? storageID = ConvertDatabaseValueToInt32(dataReader["StorageID"]);
                        DateTime arrivalTime = Convert.ToDateTime(dataReader["ArrivalTime"]);
                        Pallet pallet = new Pallet(palletID, palletSize, storageID, arrivalTime);
                        pallets.Add(pallet);
                    }
                }
                connection.Close();
            }
            return pallets;
        }
    }
}
