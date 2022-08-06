using BepInEx.Logging;
using ProjectM;
using ProjectM.CastleBuilding;
using ProjectM.Network;
using ProjectM.UI;
using StunLocalization;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnhollowerRuntimeLib;
using Unity.Collections;
using Unity.Entities;
using Wetstone.API;

namespace VMods.Shared
{
	public static class Utils
	{
		#region Consts

		public static readonly PrefabGUID SevereGarlicDebuff = new(1582196539);

		public static readonly PrefabGUID PvPProtectionBuff = new(1111481396);

		public static readonly PrefabGUID BloodPotion = new(828432508);

		#endregion

		#region Variables

		private static World _currentWorld = null;

		private static ComponentType[] _containerComponents = null;

		#endregion

		#region Properties

		public static World CurrentWorld => _currentWorld ??= VWorld.IsServer ? VWorld.Server : VWorld.Client;

		public static ManualLogSource Logger { get; private set; }
		public static string PluginName { get; private set; }

		private static ComponentType[] ContainerComponents
		{
			get
			{
				if(_containerComponents == null)
				{
					_containerComponents = new[]
					{
						ComponentType.ReadOnly(Il2CppType.Of<Team>()),
						ComponentType.ReadOnly(Il2CppType.Of<CastleHeartConnection>()),
						ComponentType.ReadOnly(Il2CppType.Of<InventoryBuffer>()),
						ComponentType.ReadOnly(Il2CppType.Of<NameableInteractable>()),
					};
				}
				return _containerComponents;
			}
		}

		#endregion

		#region Public Methods

		public static void Initialize(ManualLogSource logger, string pluginName)
		{
			Logger = logger;
			PluginName = pluginName;
		}

		public static void Deinitialize()
		{
			Logger = null;
		}

		public static NativeArray<Entity> GetStashEntities(EntityManager entityManager)
		{
			var query = entityManager.CreateEntityQuery(ContainerComponents);
			return query.ToEntityArray(Allocator.Temp);
		}

		public static IEnumerable<Entity> GetAlliedStashes(EntityManager entityManager, TeamCheckerMain teamChecker, Entity character)
		{
			foreach(var stash in GetStashEntities(entityManager))
			{
				if(teamChecker.IsAllies(character, stash))
				{
					yield return stash;
				}
			}
		}

		public static int GetStashItemCount(EntityManager entityManager, TeamCheckerMain teamChecker, Entity character, PrefabGUID itemGUID, StoredBlood? storedBlood = null)
		{
			int stashCount = 0;
			int stashesCounted = 0;
			var stashes = GetAlliedStashes(entityManager, teamChecker, character);
			foreach(var stash in stashes)
			{
				var stashInventory = entityManager.GetBuffer<InventoryBuffer>(stash);

				for(int i = 0; i < stashInventory.Length; i++)
				{
					var stashItem = stashInventory[i];

					if(stashItem.ItemType == itemGUID)
					{
						if(storedBlood != null)
						{
							var itemStoredBlood = entityManager.GetComponentData<StoredBlood>(stashItem.ItemEntity._Entity);
							if(storedBlood.Value.BloodType != itemStoredBlood.BloodType || storedBlood.Value.BloodQuality != itemStoredBlood.BloodQuality)
							{
								continue;
							}
						}
						stashCount += stashItem.Stacks;
					}
				}
				stashesCounted++;
			}
			return stashesCounted == 0 ? -1 : stashCount;
		}

		public static string GetItemName(PrefabGUID itemGUID, GameDataSystem gameDataSystem = null, EntityManager? entityManager = null, NativeHashMap<PrefabGUID, Entity> prefabLookupMap = null)
		{
			if(itemGUID == PrefabGUID.Empty)
			{
				return string.Empty;
			}
			entityManager ??= CurrentWorld.EntityManager;
			gameDataSystem ??= CurrentWorld.GetExistingSystem<GameDataSystem>();
			prefabLookupMap ??= CurrentWorld.GetExistingSystem<PrefabCollectionSystem>().PrefabLookupMap;
			try
			{
				var itemName = GameplayHelper.TryGetItemName(gameDataSystem, entityManager.Value, prefabLookupMap, itemGUID);
				if(Localization.HasKey(itemName))
				{
					return Localization.Get(itemName);
				}
			}
			catch(Exception)
			{
			}
			return $"[{itemGUID}]";
		}

		public static void SendMessage(Entity userEntity, string message, ServerChatMessageType messageType)
		{
			if(!VWorld.IsServer)
			{
				return;
			}
			EntityManager em = VWorld.Server.EntityManager;
			int index = em.GetComponentData<User>(userEntity).Index;
			NetworkId id = em.GetComponentData<NetworkId>(userEntity);

			Entity entity = em.CreateEntity(
				ComponentType.ReadOnly<NetworkEventType>(),
				ComponentType.ReadOnly<SendEventToUser>(),
				ComponentType.ReadOnly<ChatMessageServerEvent>()
			);

			ChatMessageServerEvent ev = new()
			{
				MessageText = message,
				MessageType = messageType,
				FromUser = id,
				TimeUTC = DateTime.Now.ToFileTimeUtc()
			};

			em.SetComponentData<SendEventToUser>(entity, new()
			{
				UserIndex = index
			});
			em.SetComponentData<NetworkEventType>(entity, new()
			{
				EventId = NetworkEvents.EventId_ChatMessageServerEvent,
				IsAdminEvent = false,
				IsDebugEvent = false
			});

			em.SetComponentData(entity, ev);
		}

		public static bool TryGetPrefabGUIDForItemName(GameDataSystem gameDataSystem, LocalizationKey itemName, out PrefabGUID prefabGUID)
			=> TryGetPrefabGUIDForItemName(gameDataSystem, Localization.Get(itemName), out prefabGUID);

		public static bool TryGetPrefabGUIDForItemName(GameDataSystem gameDataSystem, string itemName, out PrefabGUID prefabGUID)
		{
			foreach(var entry in gameDataSystem.ItemHashLookupMap)
			{
				var item = gameDataSystem.ManagedDataRegistry.GetOrDefault<ManagedItemData>(entry.Key);
				if(Localization.Get(item.Name, false) == itemName)
				{
					prefabGUID = entry.Key;
					return true;
				}
			}
			prefabGUID = PrefabGUID.Empty;
			return false;
		}

		public static bool TryGiveItem(EntityManager entityManager, NativeHashMap<PrefabGUID, ItemData> itemDataMap, Entity target, PrefabGUID itemType, int itemStacks, out int remainingStacks, out Entity newEntity, bool dropRemainder = false)
		{
			if(!VWorld.IsServer)
			{
				remainingStacks = itemStacks;
				newEntity = Entity.Null;
				return false;
			}
			itemDataMap ??= CurrentWorld.GetExistingSystem<GameDataSystem>().ItemHashLookupMap;

			unsafe
			{
				// Some hacky code to create a null-able that won't be GC'ed by the IL2CPP domain.
				var bytes = stackalloc byte[Marshal.SizeOf<FakeNull>()];
				var bytePtr = new IntPtr(bytes);
				Marshal.StructureToPtr<FakeNull>(new()
				{
					value = 7,
					has_value = true
				}, bytePtr, false);
				var boxedBytePtr = IntPtr.Subtract(bytePtr, 0x10);
				var fakeInt = new Il2CppSystem.Nullable<int>(boxedBytePtr);

				return InventoryUtilitiesServer.TryAddItem(entityManager, itemDataMap, target, itemType, itemStacks, out remainingStacks, out newEntity, startIndex: fakeInt, dropRemainder: dropRemainder);
			}
		}

		public static void ApplyBuff(Entity user, Entity character, PrefabGUID buffGUID)
		{
			ApplyBuff(new FromCharacter()
			{
				User = user,
				Character = character,
			}, buffGUID);
		}

		public static void ApplyBuff(FromCharacter fromCharacter, PrefabGUID buffGUID)
		{
			var des = VWorld.Server.GetExistingSystem<DebugEventsSystem>();
			var buffEvent = new ApplyBuffDebugEvent()
			{
				BuffPrefabGUID = buffGUID
			};
			des.ApplyBuff(fromCharacter, buffEvent);
		}

		public static void RemoveBuff(FromCharacter fromCharacter, PrefabGUID buffGUID)
		{
			RemoveBuff(fromCharacter.Character, buffGUID);
		}

		public static void RemoveBuff(Entity charEntity, PrefabGUID buffGUID, EntityManager? entityManager = null)
		{
			entityManager ??= CurrentWorld.EntityManager;
			if(BuffUtility.HasBuff(entityManager.Value, charEntity, buffGUID))
			{
				BuffUtility.TryGetBuff(entityManager.Value, charEntity, buffGUID, out var buffEntity);
				entityManager.Value.AddComponent<DestroyTag>(buffEntity);
			}
		}

		public static string GetCharacterName(ulong platformId, EntityManager? entityManager = null)
		{
			entityManager ??= CurrentWorld.EntityManager;
			var users = entityManager.Value.CreateEntityQuery(ComponentType.ReadOnly<User>()).ToEntityArray(Allocator.Temp);
			foreach(var userEntity in users)
			{
				var userData = entityManager.Value.GetComponentData<User>(userEntity);
				if(userData.PlatformId == platformId)
				{
					return userData.CharacterName.ToString();
				}
			}
			return null;
		}

		public static AdminLevel GetAdminLevel(Entity userEntity, EntityManager? entityManager = null)
		{
			entityManager ??= CurrentWorld.EntityManager;
			if(entityManager.Value.HasComponent<AdminUser>(userEntity))
			{
				var adminUser = entityManager.Value.GetComponentData<AdminUser>(userEntity);
				return adminUser.Level;
			}
			return AdminLevel.None;
		}

		public static void LogAllComponentTypes(Entity entity, EntityManager? entityManager = null)
		{
			if(entity == Entity.Null)
			{
				return;
			}

			entityManager ??= CurrentWorld.EntityManager;

			Logger.LogMessage($"---");
			var types = entityManager.Value.GetComponentTypes(entity);
			foreach(var t in types)
			{
				Logger.LogMessage($"Component Type: {t} (Shared? {t.IsSharedComponent}) | {t.GetManagedType().FullName}");
			}
			Logger.LogMessage($"---");
		}

		public static void LogAllComponentTypes(EntityQuery entityQuery)
		{
			var types = entityQuery.GetQueryTypes();
			Logger.LogMessage($"---");
			foreach(var t in types)
			{
				Logger.LogMessage($"Query Component Type: {t}");
			}
			Logger.LogMessage($"---");
		}

		#endregion

		#region Nested

		private struct FakeNull
		{
			public int value;
			public bool has_value;
		}

		#endregion
	}
}
