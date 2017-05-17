using Rocket.API;

namespace uShop
{
    public class Configuration : IRocketPluginConfiguration
    {
        public string ItemShopTableName;
        public string VehicleShopTableName;
        public bool CanBuyItems;
        public bool CanBuyVehicles;
        public bool CanSellItems;
        public bool QualityCounts;
        public bool CorrespondBuyPermWithCostPerm;

        public void LoadDefaults()
        {
            ItemShopTableName = "uconomyitemshop";
            VehicleShopTableName = "uconomyvehicleshop";
            CanBuyItems = true;
            CanBuyVehicles = true;
            CanSellItems = true;
            QualityCounts = true;
            CorrespondBuyPermWithCostPerm = true;
        }
    }
}
