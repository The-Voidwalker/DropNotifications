using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using static Terraria.ModLoader.ModContent;

namespace DropNotifications {
	public class DropNotifications : Mod {
		// Left blank
	}

	public class DropGlobalItem : GlobalItem {
		public override bool InstancePerEntity => true;
		// public override bool CloneNewInstances => true;

		public bool announced = false;

		public override GlobalItem Clone(Item item, Item itemClone) {
			DropGlobalItem myClone = (DropGlobalItem)base.Clone(item, itemClone);
			myClone.announced = announced;
			return myClone;
		}

		public override void Update(Item item, ref float gravity, ref float maxFallSpeed) {
			if( Main.netMode != NetmodeID.Server && !announced && (GetInstance<DropNotifConfig>().ItemWhitelist?.Any(x => x.Type == item.type) ?? false) ) {
				Main.NewText( item.Name + " has dropped!" );
			}
			announced = true;
		}

		public override void UpdateInventory(Item item, Player player) {
			announced = true; // Don't announce items from player inventories!
		}

		public override void NetSend(Item item, BinaryWriter writer) {
			writer.Write(announced);
		}

		public override void NetReceive(Item item, BinaryReader reader) {
			announced = reader.ReadBoolean();
		}
	}

	public class DropNotifConfig : ModConfig {
		public override ConfigScope Mode => ConfigScope.ClientSide;

		[Label("Item notification whitelist")]
		[Tooltip("Only items on this list will generate notifications!")]
		public List<ItemDefinition> ItemWhitelist { get; set; } = new List<ItemDefinition>() {
			new ItemDefinition(ItemID.CoinGun),
			new ItemDefinition(ItemID.DiscountCard),
			new ItemDefinition(ItemID.LuckyCoin),
			new ItemDefinition(ItemID.SlimeStaff),
			new ItemDefinition(ItemID.CorruptionKey),
			new ItemDefinition(ItemID.CrimsonKey),
			new ItemDefinition(ItemID.FrozenKey),
			new ItemDefinition(ItemID.HallowedKey),
			new ItemDefinition(ItemID.JungleKey),
			new ItemDefinition(ItemID.RodofDiscord),
		};
	}
}