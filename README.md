# uShop
uShop - Uconomy (Unturned Economy) Shop

A simple shop plugin using the chat line. This allows you to have a shop for players to use Uconomy currency to buy items and vehicles. Buying vehicles is turned off by default.

Features:

1. Buying Items / Vehicles.
2. Sellins Items.
3. In-game shop "managing".
4. Shop Restrictions , set permissions to specific groups what they can buy/sell and what not!
5. Connects automaticlly & directly to the Uconomy Table.

Shop Restricions Example:
(Add these to anygroup you want to allow them to buy / sell something)
<Permission Cooldown="0">buy.20</Permission>
<Permission Cooldown="0">buyv.20</Permission>
<Permission Cooldown="0">sell.20</Permission>
(Add these to give all buy / sell permissions)
<Permission Cooldown="0">buy.*</Permission>
<Permission Cooldown="0">buyv.*</Permission>
<Permission Cooldown="0">sell.*</Permission>

There are 3 commands: /shop, /cost, /sell and /buy.
/cost and /buy are meant to be used by everyone that has permissions for them. Just put the command in the permissions file for the group.
/shop is meant for admins only, but is not limited to that. You can add groups to have the ability to use this command by adding shop in their group of commands and one or more of the following: shop.add (adding to the shop), shop.rem (removing from the shop), shop.chng (changing costs), or shop.* (all 3) to the group permissions.

Usage:
/buy <i or v>.<item name or id>/[amount] (optional): This will use the same name to id find as /i. i stands for item, v stands for vehicle.  Amount is only available for items and is optional, default is 1.
/sell <item name or id>/[amount] (optional): You can only sell items. 
/cost <i or v>.<item name or id>: Same as above but will display the user the cost of asked for item/vehicle.
/shop <add/rem/chng>/<i or v>.<itemid>/<cost>: This is the most complicated as it has multiple options. add (Adding), rem (Removing), chng (Change cost), i for Item and v for Vehicle and is required, itemids only (no names) for this command and one is required. Cost is not required for rem, but is required for the other two.

Only /shop can be run from both the console and in game.

Requirements:
Uconomy (Set up & connected to your MySQL Database)
Mysql

This will run off the same database as Uconomy, but just create 2 new tables for the items and vehicles in the shop to be stored in. The tables are created blank. An admin, or someone with ability to use /shop, will have to add the items/vehicles. These can only be added one at a time through the command. If you can access the table with something like phpmyadmin, feel free to add them in mass that way.
