﻿using System;
using System.Collections.Generic;

using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using Rocket.Unturned.Plugins;
using SDG.Unturned;
using UnityEngine;
using fr34kyn01535.Uconomy;
using Steamworks;
using Rocket.Core;
using Logger = Rocket.Core.Logging;

namespace uShop
{
    public class uShop : RocketPlugin<Configuration>
    {
        public DatabaseMgr ShopDB;
        public static uShop Instance;
        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList
                {
                    {
                        "buy_command_usage",
                        "Usage: /buy [v.]<name or id> [amount] [quality of 25, 50, 75, or 100] (last 2 optional and only for items, default 1 amount, 100 quality)."
                    },
                    {
                        "cost_command_usage",
                        "Usage: /cost [v.]<name or id>."
                    },
                    {
                        "sell_command_usage",
                        "Usage: /sell <name or id> [amount] (optional)."
                    },
                    {
                        "shop_command_usage",
                        "Usage: /shop <add/rem/chng/buy> [v.]<itemid> <cost>  <cost> is not required for rem, buy is only for items."
                    },
                    {
                        "error_giving_item",
                        "There was an error giving you {0}.  You have not been charged."
                    },
                    {
                        "error_getting_cost",
                        "There was an error getting the cost of {0}!"
                    },
                    {
                        "item_cost_msg",
                        "The item {0} costs {1} {2} to buy and gives {3} {4} when you sell it."
                    },
                    {
                        "vehicle_cost_msg",
                        "The vehicle {0} costs {1} {2} to buy."
                    },
                    {
                        "item_buy_msg",
                        "You have bought {5} {0} for {1} {2}.  You now have {3} {4}."
                    },
                    {
                        "vehicle_buy_msg",
                        "You have bought 1 {0} for {1} {2}.  You now have {3} {4}."
                    },
                    {
                        "not_enough_currency_msg",
                        "You do not have enough {0} to buy {1} {2}."
                    },
                    {
                        "buy_items_off",
                        "I'm sorry, but the ability to buy items is turned off."
                    },
                    {
                        "buy_vehicles_off",
                        "I'm sorry, but the ability to buy vehicles is turned off."
                    },
                    {
                        "item_not_available",
                        "I'm sorry, but {0} is not available in the shop."
                    },
                    {
                        "vehicle_not_available",
                        "I'm sorry, but {0} is not available in the shop."
                    },
                    {
                        "could_not_find",
                        "I'm sorry, I couldn't find an id for {0}."
                    },
                    {
                        "sell_items_off",
                        "I'm sorry, but the ability to sell items is turned off."
                    },
                    {
                        "not_have_item_sell",
                        "I'm sorry, but you don't have any {0} to sell."
                    },
                    {
                        "not_enough_items_sell",
                        "I'm sorry, but you don't have {0} {1} to sell."
                    },
                    {
                        "not_enough_ammo_sell",
                        "I'm sorry, but you don't have enough ammo in {0} to sell."
                    },
                    {
                        "sold_items",
                        "You have sold {0} {1} to the shop and receive {2} {3} in return.  Your balance is now {4} {5}."
                    },
                    {
                        "no_sell_price_set",
                        "The shop is not buying {0} right now"
                    },
                    {
                        "no_itemid_given",
                        "An itemid is required."
                    },
                    {
                        "no_cost_given",
                        "A cost is required."
                    },
                    {
                        "invalid_amt",
                        "You have entered in an invalid amount."
                    },
                    {
                        "v_not_provided",
                        "You must specify v for vehicle or just an item id.  Ex. /shop rem/101"
                    },
                    {
                        "invalid_id_given",
                        "You need to provide a valid item or vehicle id."
                    },
                    {
                        "no_permission_shop_chng",
                        "You don't have permission to use the shop chng msg."
                    },
                    {
                        "no_permission_shop_add",
                        "You don't have permission to use the shop add msg."
                    },
                    {
                        "no_permission_shop_rem",
                        "You don't have permission to use the shop rem msg."
                    },
                    {
                        "no_permission_shop_buy",
                        "You don't have permission to use the shop buy msg."
                    },
                    {
                        "changed",
                        "changed"
                    },
                    {
                        "added",
                        "added"
                    },
                    {
                        "changed_or_added_to_shop",
                        "You have {0} the {1} with cost {2} to the shop."
                    },
                    {
                        "error_adding_or_changing",
                        "There was an error adding/changing {0}!"
                    },
                    {
                        "removed_from_shop",
                        "You have removed the {0} from the shop."
                    },
                    {
                        "not_in_shop_to_remove",
                        "{0} wasn't in the shop, so couldn't be removed."
                    },
                    {
                        "not_in_shop_to_set_buyback",
                        "{0} isn't in the shop so can't set a buyback price."
                    },
                    {
                        "set_buyback_price",
                        "You set the buyback price for {0} to {1} in the shop."
                    },
                    {
                        "set_buyback_price",
                        "You set the buyback price for {0} to {1} in the shop."
                    },
                    {
                        "no_permission_vehicle",
                        "You have no permission to buy {0} in the shop!"
                    },
                    {
                        "no_permission_vehicle_cost",
                        "You have no permission to view the cost of {0}!"
                    },
                    {
                        "no_permission_item_cost",
                        "You have no permission to view the cost of {0}!"
                    },
                    {
                        "no_permission_item",
                        "You have no permission to buy {0} in the shop!"
                    },
                    {
                        "no_permission_sells_item",
                        "You have no permission to sell {0} !"
                    },
                    {
                        "invalid_shop_command",
                        "You entered an invalid shop command."
                    }
                };
            }
        }
        
        protected override void Load()
        {
            Logger.Logger.Log("uShop - Loading . . .");
            uShop.Instance = this;
            this.ShopDB = new DatabaseMgr();
            Logger.Logger.Log("uShop - Loaded!");
            Logger.Logger.Log("Reborn by TrueThat , ZaupShop Credits to Zamirathe :)");
        }

        public delegate void PlayerShopBuy(UnturnedPlayer player, decimal amt, byte items, ushort item, string type="item");
        public event PlayerShopBuy OnShopBuy;
        public delegate void PlayerShopSell(UnturnedPlayer player, decimal amt, byte items, ushort item);
        public event PlayerShopSell OnShopSell;

        public void Buy(UnturnedPlayer playerid, string[] components0)
        {
            string message;
            if (components0.Length == 0)
            {
                message = uShop.Instance.Translate("buy_command_usage", new object[] {});
                // We are going to print how to use
                UnturnedChat.Say(playerid, message);
                return;
            }
            byte amttobuy = 1;
            if (components0.Length > 1)
            {
                if (!byte.TryParse(components0[1], out amttobuy))
                {
                    message = uShop.Instance.Translate("invalid_amt", new object[] { });
                    UnturnedChat.Say(playerid, message);
                    return;
                }
            }
            string[] components = Parser.getComponentsFromSerial(components0[0], '.');
            if ((components.Length == 2 && components[0].Trim() != "v") || (components.Length == 1 && components[0].Trim() == "v") || components.Length > 2 || components0[0].Trim() == string.Empty)
            {
                message = uShop.Instance.Translate("buy_command_usage", new object[] { });
                // We are going to print how to use
                UnturnedChat.Say(playerid, message);
                return;
            }
            ushort id;
            switch (components[0])
            {
                case "v":
                    if (!uShop.Instance.Configuration.Instance.CanBuyVehicles)
                    {
                        message = uShop.Instance.Translate("buy_vehicles_off", new object[] { });
                        UnturnedChat.Say(playerid, message);
                        return;
                    }
                    string name = null;
                    if (!ushort.TryParse(components[1], out id))
                    {
                        Asset[] array = Assets.find(EAssetType.VEHICLE);
                        Asset[] array2 = array;
                        for (int i = 0; i < array2.Length; i++)
                        {
                            VehicleAsset vAsset = (VehicleAsset)array2[i];
                            if (vAsset != null && vAsset.vehicleName != null && vAsset.vehicleName.ToLower().Contains(components[1].ToLower()))
                            {
                                id = vAsset.id;
                                name = vAsset.vehicleName;
                                break;
                            }
                        }
                    }
                    if (Assets.find(EAssetType.VEHICLE, id) == null)
                    {
                        message = uShop.Instance.Translate("could_not_find", new object[] { components[1] });
                        UnturnedChat.Say(playerid, message);
                        return;
                    }
                    else if (name == null && id != 0)
                    {
                        name = ((VehicleAsset)Assets.find(EAssetType.VEHICLE, id)).vehicleName;
                    }
                    //Permission Checker
                    string vperm = "buyv." + id;
                    if (!playerid.HasPermission("buyv.*"))
                    { 
                        if (!playerid.HasPermission(vperm))
                        {
                            UnturnedChat.Say(playerid, Translate("no_permission_vehicle", name));
                            return;
                        }
                    }
                    decimal cost = uShop.Instance.ShopDB.GetVehicleCost(id);
                    decimal balance = Uconomy.Instance.Database.GetBalance(playerid.CSteamID.ToString());
                    if (cost <= 0m)
                    {
                        message = uShop.Instance.Translate("vehicle_not_available", new object[] {name});
                        UnturnedChat.Say(playerid, message);
                        return;
                    }
                    if (balance < cost)
                    {
                        message = uShop.Instance.Translate("not_enough_currency_msg", new object[] {Uconomy.Instance.Configuration.Instance.MoneyName, "1", name});
                        UnturnedChat.Say(playerid, message);
                        return;
                    }
                    if (!playerid.GiveVehicle(id))
                    {
                        message = uShop.Instance.Translate("error_giving_item", new object[] { name });
                        UnturnedChat.Say(playerid, message);
                        return;
                    }
                    decimal newbal = Uconomy.Instance.Database.IncreaseBalance(playerid.CSteamID.ToString(), (cost * -1));
                    message = uShop.Instance.Translate("vehicle_buy_msg", new object[] {name, cost, Uconomy.Instance.Configuration.Instance.MoneyName, newbal, Uconomy.Instance.Configuration.Instance.MoneyName});
                    if (uShop.Instance.OnShopBuy != null)
                        uShop.Instance.OnShopBuy(playerid, cost, 1, id, "vehicle");
                    playerid.Player.gameObject.SendMessage("ZaupShopOnBuy", new object[] { playerid, cost, amttobuy, id, "vehicle" }, SendMessageOptions.DontRequireReceiver);
                    UnturnedChat.Say(playerid, message);
                    break;
                default:
                    if (!uShop.Instance.Configuration.Instance.CanBuyItems)
                    {
                        message = uShop.Instance.Translate("buy_items_off", new object[] { });
                        UnturnedChat.Say(playerid, message);
                        return;
                    }
                    name = null;
                    if (!ushort.TryParse(components[0], out id))
                    {
                        Asset[] array = Assets.find(EAssetType.ITEM);
                        Asset[] array2 = array;
                        for (int i = 0; i < array2.Length; i++)
                        {
                            ItemAsset vAsset = (ItemAsset)array2[i];
                            if (vAsset != null && vAsset.itemName != null && vAsset.itemName.ToLower().Contains(components[0].ToLower()))
                            {
                                id = vAsset.id;
                                name = vAsset.itemName;
                                break;
                            }
                        }
                    }
                    if (Assets.find(EAssetType.ITEM, id) == null)
                    {
                        message = uShop.Instance.Translate("could_not_find", new object[] {components[0]});
                        UnturnedChat.Say(playerid, message);
                        return;
                    }
                    else if (name == null && id != 0)
                    {
                        name = ((ItemAsset)Assets.find(EAssetType.ITEM, id)).itemName;
                    }
                    //Permission checker
                    string iperm = "buy." + id;
                    if (!playerid.HasPermission("buy.*"))
                    {
                        if (!playerid.HasPermission(iperm))
                        {
                            UnturnedChat.Say(playerid, Translate("no_permission_item", name));
                            return;
                        }
                    }
                    cost = decimal.Round(uShop.Instance.ShopDB.GetItemCost(id) * amttobuy, 2);
                    balance = Uconomy.Instance.Database.GetBalance(playerid.CSteamID.ToString());
                    if (cost <= 0m)
                    {
                        message = uShop.Instance.Translate("item_not_available", new object[] {name});
                        UnturnedChat.Say(playerid, message);
                        return;
                    }
                    if (balance < cost)
                    {
                        message = uShop.Instance.Translate("not_enough_currency_msg", new object[] {Uconomy.Instance.Configuration.Instance.MoneyName, amttobuy, name});
                        UnturnedChat.Say(playerid, message);
                        return;
                    }
                    playerid.GiveItem(id, amttobuy);
                    newbal = Uconomy.Instance.Database.IncreaseBalance(playerid.CSteamID.ToString(), (cost * -1));
                    message = uShop.Instance.Translate("item_buy_msg", new object[] {name, cost, Uconomy.Instance.Configuration.Instance.MoneyName, newbal, Uconomy.Instance.Configuration.Instance.MoneyName, amttobuy});
                    if (uShop.Instance.OnShopBuy != null)
                        uShop.Instance.OnShopBuy(playerid, cost, amttobuy, id);
                    playerid.Player.gameObject.SendMessage("ZaupShopOnBuy", new object[] { playerid, cost, amttobuy, id, "item" }, SendMessageOptions.DontRequireReceiver);
                    UnturnedChat.Say(playerid, message);
                    break;
            }
        }
        public void Cost(UnturnedPlayer playerid, string[] components)
        {
            string message;
            if (components.Length == 0 || (components.Length == 1 && (components[0].Trim() == string.Empty || components[0].Trim() == "v")))
            {
                message = uShop.Instance.Translate("cost_command_usage", new object[] { });
                // We are going to print how to use
                UnturnedChat.Say(playerid, message);
                return;
            }
            if (components.Length == 2 && (components[0] != "v" || components[1].Trim() == string.Empty))
            {
                message = uShop.Instance.Translate("cost_command_usage", new object[] { });
                // We are going to print how to use
                UnturnedChat.Say(playerid, message);
                return;
            }
            ushort id;
            switch (components[0])
            {
                case "v":
                    string name = null;
                    if (!ushort.TryParse(components[1], out id))
                    {
                        Asset[] array = Assets.find(EAssetType.VEHICLE);
                        Asset[] array2 = array;
                        for (int i = 0; i < array2.Length; i++)
                        {
                            VehicleAsset vAsset = (VehicleAsset)array2[i];
                            if (vAsset != null && vAsset.vehicleName != null && vAsset.vehicleName.ToLower().Contains(components[1].ToLower()))
                            {
                                id = vAsset.id;
                                name = vAsset.vehicleName;
                                break;
                            }
                        }
                    }
                    if (Assets.find(EAssetType.VEHICLE, id) == null)
                    {
                        message = uShop.Instance.Translate("could_not_find", new object[] {components[1]});
                        UnturnedChat.Say(playerid, message);
                        return;
                    }
                    else if (name == null && id != 0)
                    {
                        name = ((VehicleAsset)Assets.find(EAssetType.VEHICLE, id)).vehicleName;
                    }
                    if (!Configuration.Instance.CorrespondBuyPermWithCostPerm)
                    {
                        string costvperm = "costv." + id;

                        if (!playerid.HasPermission(costvperm))
                        {
                            UnturnedChat.Say(playerid, Translate("no_permission_vehicle_cost", name));
                            return;
                        }
                    }
                    else if (!playerid.HasPermission("buyv.*"))
                    {
                        string vperm = "buyv." + id;

                        if (!playerid.HasPermission(vperm))
                        {
                            UnturnedChat.Say(playerid, Translate("no_permission_vehicle_cost", name));
                            return;
                        }
                    }
                    decimal cost = uShop.Instance.ShopDB.GetVehicleCost(id);
                    message = uShop.Instance.Translate("vehicle_cost_msg", new object[] {name, cost.ToString(), Uconomy.Instance.Configuration.Instance.MoneyName});
                    if (cost <= 0m)
                    {
                        message = uShop.Instance.Translate("error_getting_cost", new object[] {name});
                    }
                    UnturnedChat.Say(playerid, message);
                    break;
                default:
                    name = null;
                    if (!ushort.TryParse(components[0], out id))
                    {
                        Asset[] array = Assets.find(EAssetType.ITEM);
                        Asset[] array2 = array;
                        for (int i = 0; i < array2.Length; i++)
                        {
                            ItemAsset iAsset = (ItemAsset)array2[i];
                            if (iAsset != null && iAsset.itemName != null && iAsset.itemName.ToLower().Contains(components[0].ToLower()))
                            {
                                id = iAsset.id;
                                name = iAsset.itemName;
                                break;
                            }
                        }
                    }
                    if (Assets.find(EAssetType.ITEM, id) == null)
                    {
                        message = uShop.Instance.Translate("could_not_find", new object[] {components[0]});
                        UnturnedChat.Say(playerid, message);
                        return;
                    }
                    else if (name == null && id != 0)
                    {
                        name = ((ItemAsset)Assets.find(EAssetType.ITEM, id)).itemName;
                    }
                    if (!Configuration.Instance.CorrespondBuyPermWithCostPerm)
                    {
                        string costiperm = "cost." + id;

                        if (!playerid.HasPermission(costiperm))
                        {
                            UnturnedChat.Say(playerid, Translate("no_permission_vehicle_cost", name));
                            return;
                        }
                    }
                    else if (!playerid.HasPermission("buy.*"))
                    {
                        string iperm = "buy." + id;
                        if (!playerid.HasPermission(iperm))
                        {
                            UnturnedChat.Say(playerid, Translate("no_permission_item_cost", name));
                            return;
                        }
                    }
                    cost = uShop.Instance.ShopDB.GetItemCost(id);
                    decimal bbp = uShop.Instance.ShopDB.GetItemBuyPrice(id);
                    message = uShop.Instance.Translate("item_cost_msg", new object[] {name, cost.ToString(), Uconomy.Instance.Configuration.Instance.MoneyName, bbp.ToString(), Uconomy.Instance.Configuration.Instance.MoneyName});
                    if (cost <= 0m)
                    {
                        message = uShop.Instance.Translate("error_getting_cost", new object[] {name});
                    }
                    UnturnedChat.Say(playerid, message);
                    break;
            }
        }
        public void Sell(UnturnedPlayer playerid, string[] components)
        {
            string message;
            if (components.Length == 0 || (components.Length > 0 && components[0].Trim() == string.Empty))
            {
                message = uShop.Instance.Translate("sell_command_usage", new object[] { });
                // We are going to print how to use
                UnturnedChat.Say(playerid, message);
                return;
            }
            byte amttosell = 1;
            if (components.Length > 1)
            {
                if (!byte.TryParse(components[1], out amttosell))
                {
                    message = uShop.Instance.Translate("invalid_amt", new object[] { });
                    UnturnedChat.Say(playerid, message);
                    return;
                }
            }
            byte amt = amttosell;
            ushort id;
            if (!uShop.Instance.Configuration.Instance.CanSellItems)
            {
                message = uShop.Instance.Translate("sell_items_off", new object[] { });
                UnturnedChat.Say(playerid, message);
                return;
            }
            string name = null;
            ItemAsset vAsset = null;
            if (!ushort.TryParse(components[0], out id))
            {
                Asset[] array = Assets.find(EAssetType.ITEM);
                Asset[] array2 = array;
                for (int i = 0; i < array2.Length; i++)
                {
                    vAsset = (ItemAsset)array2[i];
                    if (vAsset != null && vAsset.itemName != null && vAsset.itemName.ToLower().Contains(components[0].ToLower()))
                    {
                        id = vAsset.id;
                        name = vAsset.itemName;
                        break;
                    }
                }
            }
            if (Assets.find(EAssetType.ITEM, id) == null)
            {
                message = uShop.Instance.Translate("could_not_find", new object[] {components[0]});
                UnturnedChat.Say(playerid, message);
                return;
            }
            else if (name == null && id != 0)
            {
                vAsset = (ItemAsset)Assets.find(EAssetType.ITEM, id);
                name = vAsset.itemName;
            }
            // Permission Checker 
            string isellperm = "sell." + id;
            if (!playerid.HasPermission("sell.*"))
            {
                if (!playerid.HasPermission(isellperm))
                {
                    UnturnedChat.Say(playerid, Translate("no_permission_sell_item", name));
                    return;
                }
            }
            // Get how many they have
            if (playerid.Inventory.has(id) == null)
            {
                message = uShop.Instance.Translate("not_have_item_sell", new object[] {name});
                UnturnedChat.Say(playerid, message);
                return;
            }
            List<InventorySearch> list = playerid.Inventory.search(id, true, true);
            if (list.Count == 0 || (vAsset.amount == 1 && list.Count < amttosell))
            {
                message = uShop.Instance.Translate("not_enough_items_sell", new object[] {amttosell.ToString(), name});
                UnturnedChat.Say(playerid, message);
                return;
            }
            if (vAsset.amount > 1)
            {
                int ammomagamt = 0;
                foreach (InventorySearch ins in list)
                {
                    ammomagamt += ins.jar.item.amount;
                }
                if (ammomagamt < amttosell)
                {
                    message = uShop.Instance.Translate("not_enough_ammo_sell", new object[] {name});
                    UnturnedChat.Say(playerid, message);
                    return;
                }
            }
            // We got this far, so let's buy back the items and give them money.
            // Get cost per item.  This will be whatever is set for most items, but changes for ammo and magazines.
            decimal price = uShop.Instance.ShopDB.GetItemBuyPrice(id);
            if (price <= 0.00m)
            {
                message = uShop.Instance.Translate("no_sell_price_set", new object[] { name });
                UnturnedChat.Say(playerid, message);
                return;
            }
            byte quality = 100;
            decimal peritemprice = 0;
            decimal addmoney = 0;
            switch (vAsset.amount)
            {
                case 1:
                    // These are single items, not ammo or magazines
                    while (amttosell > 0)
                    {
                        if (playerid.Player.equipment.checkSelection(list[0].page, list[0].jar.x, list[0].jar.y))
                        {
                            playerid.Player.equipment.dequip();
                        }
                        if (uShop.Instance.Configuration.Instance.QualityCounts)
                            quality = list[0].jar.item.quality;
                        peritemprice = decimal.Round(price * (quality / 100.0m), 2);
                        addmoney += peritemprice;
                        playerid.Inventory.removeItem(list[0].page, playerid.Inventory.getIndex(list[0].page, list[0].jar.x, list[0].jar.y));
                        list.RemoveAt(0);
                        amttosell--;
                    }
                    break;
                default:
                    // This is ammo or magazines
                    byte amttosell1 = amttosell;
                    while (amttosell > 0)
                    {
                        if (playerid.Player.equipment.checkSelection(list[0].page, list[0].jar.x, list[0].jar.y))
                        {
                            playerid.Player.equipment.dequip();
                        }
                        if (list[0].jar.item.amount >= amttosell)
                        {
                            byte left = (byte)(list[0].jar.item.amount - amttosell);
                            list[0].jar.item.amount = left;
                            playerid.Inventory.sendUpdateAmount(list[0].page, list[0].jar.x, list[0].jar.y, left);
                            amttosell = 0;
                            if (left == 0)
                            {
                                playerid.Inventory.removeItem(list[0].page, playerid.Inventory.getIndex(list[0].page, list[0].jar.x, list[0].jar.y));
                                list.RemoveAt(0);
                            }
                        }
                        else
                        {
                            amttosell -= list[0].jar.item.amount;
                            playerid.Inventory.sendUpdateAmount(list[0].page, list[0].jar.x, list[0].jar.y, 0);
                            playerid.Inventory.removeItem(list[0].page, playerid.Inventory.getIndex(list[0].page, list[0].jar.x, list[0].jar.y));
                            list.RemoveAt(0);
                        }
                    }
                    peritemprice = decimal.Round(price * ((decimal)amttosell1 / (decimal)vAsset.amount), 2);
                    addmoney += peritemprice;
                    break;
            }
            decimal balance = Uconomy.Instance.Database.IncreaseBalance(playerid.CSteamID.ToString(), addmoney);
            message = uShop.Instance.Translate("sold_items", new object[] {amt, name, addmoney, Uconomy.Instance.Configuration.Instance.MoneyName, balance, Uconomy.Instance.Configuration.Instance.MoneyName});
            if (uShop.Instance.OnShopSell != null)
                uShop.Instance.OnShopSell(playerid, addmoney, amt, id);
            playerid.Player.gameObject.SendMessage("ZaupShopOnSell", new object[] { playerid, addmoney, amt, id }, SendMessageOptions.DontRequireReceiver);
            UnturnedChat.Say(playerid, message);
        }

    }
}
