/*
 * Copyright (c) 2006-2007, Second Life Reverse Engineering Team
 * All rights reserved.
 *
 * - Redistribution and use in source and binary forms, with or without
 *   modification, are permitted provided that the following conditions are met:
 *
 * - Redistributions of source code must retain the above copyright notice, this
 *   list of conditions and the following disclaimer.
 * - Neither the name of the Second Life Reverse Engineering Team nor the names
 *   of its contributors may be used to endorse or promote products derived from
 *   this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Text;
using libsecondlife;
using libsecondlife.Packets;

namespace libsecondlife
{
    #region Enums

    public enum InventoryType : sbyte
    {
        Unknown = -1,
        Texture = 0,
        Sound = 1,
        CallingCard = 2,
        Landmark = 3,
        [Obsolete] Script = 4,
        [Obsolete] Clothing = 5,
        Object = 6,
        Notecard = 7,
        Category = 8,
        Folder = 8,
        RootCategory = 0,
        LSL = 10,
        [Obsolete] LSLBytecode = 11,
        [Obsolete] TextureTGA = 12,
        [Obsolete] Bodypart = 13,
        [Obsolete] Trash = 14,
        Snapshot = 15,
        [Obsolete] LostAndFound = 16,
        Attachment = 17,
        Wearable = 18,
        Animation = 19,
        Gesture = 20
    }

    [Flags]
    public enum InventorySortOrder : int
    {
        /// <summary>Sort by name</summary>
        ByName = 0,
        /// <summary>Sort by date</summary>
        ByDate = 1,
        /// <summary>Sort folders by name, regardless of whether items are
        /// sorted by name or date</summary>
        FoldersByName = 2,
        /// <summary>Place system folders at the top</summary>
        SystemFoldersToTop = 4
    }

    #endregion Enums

    #region Inventory Object Classes

    public abstract class InventoryBase
    {
        public readonly LLUUID UUID;
        public LLUUID ParentUUID;
        public string Name;
        public LLUUID OwnerID;

        public InventoryBase(LLUUID itemID)
        {
            if (itemID == LLUUID.Zero)
                throw new ArgumentException("Inventory item ID cannot be NULL_KEY (LLUUID.Zero)");
            UUID = itemID;
        }

        public override int GetHashCode()
        {
            return UUID.GetHashCode() ^ ParentUUID.GetHashCode() ^ Name.GetHashCode() ^ OwnerID.GetHashCode();
        }

        public override bool Equals(object o)
        {
            InventoryBase inv = o as InventoryBase;
            return inv != null && Equals(inv);
        }

        public virtual bool Equals(InventoryBase o)
        {
            return o.UUID == UUID
                && o.ParentUUID == ParentUUID
                && o.Name == Name
                && o.OwnerID == OwnerID;
        }
    }

    public class InventoryItem : InventoryBase
    {
        public LLUUID AssetUUID;
        public Permissions Permissions;
        public AssetType AssetType;
        public InventoryType InventoryType;
        public LLUUID CreatorID;
        public string Description;
        public LLUUID GroupID;
        public bool GroupOwned;
        public int SalePrice;
        public SaleType SaleType;
        public uint Flags;
        /// <summary>Time and date this inventory item was created, stored as
        /// UTC (Coordinated Universal Time)</summary>
        public DateTime CreationDate;

        public InventoryItem(LLUUID itemID) 
            : base(itemID) { }

        public InventoryItem(InventoryType type, LLUUID itemID) : base(itemID) { InventoryType = type; }

        public override int GetHashCode()
        {
            return AssetUUID.GetHashCode() ^ Permissions.GetHashCode() ^ AssetType.GetHashCode() ^
                InventoryType.GetHashCode() ^ Description.GetHashCode() ^ GroupID.GetHashCode() ^
                GroupOwned.GetHashCode() ^ SalePrice.GetHashCode() ^ SaleType.GetHashCode() ^
                Flags.GetHashCode() ^ CreationDate.GetHashCode();
        }

        public override bool Equals(object o)
        {
            InventoryItem item = o as InventoryItem;
            return item != null && Equals(item);
        }

        public override bool Equals(InventoryBase o)
        {
            InventoryItem item = o as InventoryItem;
            return item != null && Equals(item);
        }

        public bool Equals(InventoryItem o)
        {
            return base.Equals(o as InventoryBase)
                && o.AssetType == AssetType
                && o.AssetUUID == AssetUUID
                && o.CreationDate == CreationDate
                && o.Description == Description
                && o.Flags == Flags
                && o.GroupID == GroupID
                && o.GroupOwned == GroupOwned
                && o.InventoryType == InventoryType
                && o.Permissions.Equals(Permissions)
                && o.SalePrice == SalePrice
                && o.SaleType == SaleType;
        }
    }

    public class InventoryTexture     : InventoryItem { public InventoryTexture(LLUUID itemID) : base(itemID) { InventoryType = InventoryType.Texture; } }
    public class InventorySound       : InventoryItem { public InventorySound(LLUUID itemID) : base(itemID) { InventoryType = InventoryType.Sound; } }
    public class InventoryCallingCard : InventoryItem { public InventoryCallingCard(LLUUID itemID) : base(itemID) { InventoryType = InventoryType.CallingCard; } }
    public class InventoryLandmark    : InventoryItem { public InventoryLandmark(LLUUID itemID) : base(itemID) { InventoryType = InventoryType.Landmark; } }
    public class InventoryObject      : InventoryItem { public InventoryObject(LLUUID itemID) : base(itemID) { InventoryType = InventoryType.Object; } }
    public class InventoryNotecard    : InventoryItem { public InventoryNotecard(LLUUID itemID) : base(itemID) { InventoryType = InventoryType.Notecard; } }
    public class InventoryCategory    : InventoryItem { public InventoryCategory(LLUUID itemID) : base(itemID) { InventoryType = InventoryType.Category; } }
    public class InventoryLSL         : InventoryItem { public InventoryLSL(LLUUID itemID) : base(itemID) { InventoryType = InventoryType.LSL; } }
    public class InventorySnapshot    : InventoryItem { public InventorySnapshot(LLUUID itemID) : base(itemID) { InventoryType = InventoryType.Snapshot; } }
    public class InventoryAttachment  : InventoryItem { public InventoryAttachment(LLUUID itemID) : base(itemID) { InventoryType = InventoryType.Attachment; } }

    public class InventoryWearable : InventoryItem
    {
        public InventoryWearable(LLUUID itemID) : base(itemID) { InventoryType = InventoryType.Wearable; }

        public WearableType WearableType
        {
            get { return (WearableType)Flags; }
            set { Flags = (uint)value; }
        }
    }

    public class InventoryAnimation   : InventoryItem { public InventoryAnimation(LLUUID itemID) : base(itemID) { InventoryType = InventoryType.Animation; } }
    public class InventoryGesture     : InventoryItem { public InventoryGesture(LLUUID itemID) : base(itemID) { InventoryType = InventoryType.Gesture; } }

    public class InventoryFolder : InventoryBase
    {
        public AssetType PreferredType;
        public int Version;
        public int DescendentCount;

        public InventoryFolder(LLUUID itemID)
            : base(itemID) { }

        public override int GetHashCode()
        {
            return PreferredType.GetHashCode() ^ Version.GetHashCode() ^ DescendentCount.GetHashCode();
        }

        public override bool Equals(object o)
        {
            InventoryFolder folder = o as InventoryFolder;
            return folder != null && Equals(folder);
        }

        public override bool Equals(InventoryBase o)
        {
            InventoryFolder folder = o as InventoryFolder;
            return folder != null && Equals(folder);
        }

        public bool Equals(InventoryFolder o)
        {
            return base.Equals(o as InventoryBase)
                && o.DescendentCount == DescendentCount
                && o.PreferredType == PreferredType
                && o.Version == Version;
        }
    }

    #endregion Inventory Object Classes

    public class InventoryManager
    {
        /// <summary>
        /// Callback for inventory item creation finishing
        /// </summary>
        /// <param name="success">Whether the request to create an inventory
        /// item succeeded or not</param>
        /// <param name="item">Inventory item being created. If success is
        /// false this will be null</param>
        public delegate void ItemCreatedCallback(bool success, InventoryItem item);
        /// <summary>
        /// Callback for an inventory folder updating
        /// </summary>
        /// <param name="folderID">UUID of the folder that was updated</param>
        public delegate void FolderUpdatedCallback(LLUUID folderID);
        /// <summary>
        /// Callback when an inventory object is received from another avatar
        /// or a primitive
        /// </summary>
        /// <param name="fromAgentID"></param>
        /// <param name="fromAgentName"></param>
        /// <param name="parentEstateID"></param>
        /// <param name="regionID"></param>
        /// <param name="position"></param>
        /// <param name="timestamp"></param>
        /// <param name="type"></param>
        /// <param name="objectID"></param>
        /// <param name="fromTask"></param>
        /// <returns>True to accept the inventory offer, false to reject it</returns>
        public delegate bool ObjectReceivedCallback(LLUUID fromAgentID, string fromAgentName, uint parentEstateID, 
            LLUUID regionID, LLVector3 position, DateTime timestamp, AssetType type, LLUUID objectID, bool fromTask);

        public event FolderUpdatedCallback OnInventoryFolderUpdated;
        public event ObjectReceivedCallback OnInventoryObjectReceived;

        private SecondLife _Client;
        private Inventory _Store;
        private Dictionary<LLUUID, List<DescendantsResult>> _FolderRequests = new Dictionary<LLUUID, List<DescendantsResult>>();
        private Dictionary<uint, ItemCreatedCallback> _ItemCreatedCallbacks = new Dictionary<uint, ItemCreatedCallback>();
        private uint _ItemCreatedCallbackPos = 0;

        /// <summary>Partial mapping of AssetTypes to folder names</summary>
        private static readonly string[] _NewFolderNames = new string[]
        {
            "Textures",
            "Sounds",
            "Calling Cards",
            "Landmarks",
            "Scripts",
            "Clothing",
            "Objects",
            "Notecards",
            "New Folder",
            "Inventory",
            "Scripts",
            "Scripts",
            "Uncompressed Images",
            "Body Parts",
            "Trash",
            "Photo Album",
            "Lost And Found",
            "Uncompressed Sounds",
            "Uncompressed Images",
            "Uncompressed Images",
            "Animations",
            "Gestures"
        };

        private static readonly string[] _AssetTypeNames = new string[]
        {
            "texture",
	        "sound",
	        "callcard",
	        "landmark",
	        "script",
	        "clothing",
	        "object",
	        "notecard",
	        "category",
	        "root",
	        "lsltext",
	        "lslbyte",
	        "txtr_tga",
	        "bodypart",
	        "trash",
	        "snapshot",
	        "lstndfnd",
	        "snd_wav",
	        "img_tga",
	        "jpeg",
	        "animatn",
	        "gesture",
	        "simstate"
        };

        private static readonly string[] _InventoryTypeNames = new string[]
        {
            "texture",
	        "sound",
	        "callcard",
	        "landmark",
	        String.Empty,
	        String.Empty,
	        "object",
	        "notecard",
	        "category",
	        "root",
	        "script",
	        String.Empty,
	        String.Empty,
	        String.Empty,
	        String.Empty,
	        "snapshot",
	        String.Empty,
	        "attach",
	        "wearable",
	        "animation",
	        "gesture",
        };

        #region Properties

        public Inventory Store { get { return _Store; } }

        #endregion Properties

        public InventoryManager(SecondLife client)
        {
            _Client = client;

            _Client.Network.RegisterCallback(PacketType.UpdateCreateInventoryItem, new NetworkManager.PacketCallback(UpdateCreateInventoryItemHandler));
            _Client.Network.RegisterCallback(PacketType.SaveAssetIntoInventory, new NetworkManager.PacketCallback(SaveAssetIntoInventoryHandler));
            _Client.Network.RegisterCallback(PacketType.BulkUpdateInventory, new NetworkManager.PacketCallback(BulkUpdateInventoryHandler));
            _Client.Network.RegisterCallback(PacketType.MoveInventoryItem, new NetworkManager.PacketCallback(MoveInventoryItemHandler));
            _Client.Network.RegisterCallback(PacketType.InventoryDescendents, new NetworkManager.PacketCallback(InventoryDescendentsHandler));
            _Client.Network.RegisterCallback(PacketType.FetchInventoryReply, new NetworkManager.PacketCallback(FetchInventoryReplyHandler));
            // Watch for inventory given to us through instant message
            _Client.Self.OnInstantMessage += new MainAvatar.InstantMessageCallback(Self_OnInstantMessage);
        }

        #region File & Folder Public Methods

        /// <summary>
        /// If you have a list of inventory item IDs (from a cached inventory, perhaps) 
        /// you can use this function to request an update from the server for those items.
        /// </summary>
        /// <param name="itemIDs">A list of LLUUIDs of the items to request.</param>
        public void FetchInventory(List<LLUUID> itemIDs)
        {
            FetchInventoryPacket fetch = new FetchInventoryPacket();
            fetch.AgentData = new FetchInventoryPacket.AgentDataBlock();
            fetch.AgentData.AgentID = _Client.Network.AgentID;
            fetch.AgentData.SessionID = _Client.Network.SessionID;

            fetch.InventoryData = new FetchInventoryPacket.InventoryDataBlock[itemIDs.Count];
            // TODO: Make sure the packet doesnt overflow.
            for (int i = 0; i < itemIDs.Count; ++i)
            {
                fetch.InventoryData[i] = new FetchInventoryPacket.InventoryDataBlock();
                fetch.InventoryData[i].ItemID = itemIDs[i];
                fetch.InventoryData[i].OwnerID = _Client.Network.AgentID;
            }

            _Client.Network.SendPacket(fetch);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemID"></param>
        public void FetchInventory(LLUUID itemID)
        {
            List<LLUUID> list = new List<LLUUID>(1);
            list.Add(itemID);
            FetchInventory(list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void Remove(InventoryBase obj)
        {
            List<InventoryBase> temp = new List<InventoryBase>(1);
            temp.Add(obj);
            Remove(temp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objects"></param>
        public void Remove(List<InventoryBase> objects)
        {
            List<LLUUID> items = new List<LLUUID>(objects.Count);
            List<LLUUID> folders = new List<LLUUID>(objects.Count);
            foreach (InventoryBase obj in objects)
            {
                if (obj is InventoryFolder)
                {
                    folders.Add(obj.UUID);
                }
                else
                {
                    items.Add(obj.UUID);
                }
            }
            Remove(items, folders);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="folders"></param>
        public void Remove(List<LLUUID> items, List<LLUUID> folders)
        {
            if ((items == null && items.Count == 0) && (folders == null && folders.Count == 0))
                return;

            RemoveInventoryObjectsPacket rem = new RemoveInventoryObjectsPacket();
            rem.AgentData.AgentID = _Client.Network.AgentID;
            rem.AgentData.SessionID = _Client.Network.SessionID;

            if (items == null || items.Count == 0)
            {
                // To indicate that we want no items removed:
                rem.ItemData = new RemoveInventoryObjectsPacket.ItemDataBlock[1];
                rem.ItemData[0] = new RemoveInventoryObjectsPacket.ItemDataBlock();
                rem.ItemData[0].ItemID = LLUUID.Zero;
            }
            else
            {
                rem.ItemData = new RemoveInventoryObjectsPacket.ItemDataBlock[items.Count];
                for (int i = 0; i < items.Count; ++i)
                {
                    rem.ItemData[i] = new RemoveInventoryObjectsPacket.ItemDataBlock();
                    rem.ItemData[i].ItemID = items[i];
                    // Update local copy
                    _Store.RemoveNodeFor(_Store[items[i]]);
                }
            }

            if (folders == null || folders.Count == 0)
            {
                // To indicate we want no folders removed:
                rem.FolderData = new RemoveInventoryObjectsPacket.FolderDataBlock[1];
                rem.FolderData[0] = new RemoveInventoryObjectsPacket.FolderDataBlock();
                rem.FolderData[0].FolderID = LLUUID.Zero;
            }
            else
            {
                rem.FolderData = new RemoveInventoryObjectsPacket.FolderDataBlock[folders.Count];
                for (int i = 0; i < folders.Count; ++i)
                {
                    rem.FolderData[i] = new RemoveInventoryObjectsPacket.FolderDataBlock();
                    rem.FolderData[i].FolderID = folders[i];
                    // Update local copy:
                    _Store.RemoveNodeFor(_Store[folders[i]]);
                }
            }
        }

        #endregion File & Folder Public Methods

        #region Searching
        private Dictionary<IAsyncResult, FindResult> FindDescendantsMap = new Dictionary<IAsyncResult, FindResult>();

        /// <summary>
        /// Starts a search for any items whose names match the regex within 
        /// the spacified folder.
        /// </summary>
        /// <remarks>Use the AsyncWaitHandle of the returned value to run the search synchronously.</remarks>
        /// <param name="baseFolder">The UUID of the folder to look in.</param>
        /// <param name="regex">The regex that results match.</param>
        /// <param name="recurse">Recurse into and search inside subfolders of baseFolder.</param>
        /// <param name="refresh">Re-download the contents of baseFolder (and its subdirectories, if recursing)</param>
        /// <param name="callback">The AsyncCallback to call when the search is complete.</param>
        /// <param name="asyncState">An object that will be passed back to the caller.</param>
        /// <returns>An IAsyncResult that represents this find operation, and can be passed to EndFindObjects.</returns>
        public IAsyncResult BeginFindObjects(LLUUID baseFolder, string regex, bool recurse, bool refresh, AsyncCallback callback, object asyncState)
        {
            return BeginFindObjects(baseFolder, new Regex(regex), recurse, refresh, false, callback, asyncState);
        }

        public IAsyncResult BeginFindObjects(LLUUID baseFolder, string regex, bool recurse, bool refresh, bool firstOnly, AsyncCallback callback, object asyncState)
        {
            return BeginFindObjects(baseFolder, new Regex(regex), recurse, refresh, firstOnly, callback, asyncState);
        }

        public IAsyncResult BeginFindObjects(LLUUID baseFolder, Regex regexp, bool recurse, bool refresh, bool firstOnly, AsyncCallback callback, object asyncState)
        {
            FindResult result = new FindResult(regexp, recurse, callback);
            result.FirstOnly = firstOnly;
            result.AsyncState = asyncState;
            result.FoldersWaiting = 1;
            if (refresh)
            {
                lock (FindDescendantsMap)
                {
                    IAsyncResult descendReq = BeginRequestFolderContents(baseFolder, _Client.Network.AgentID, true, true, recurse && !firstOnly, InventorySortOrder.ByName, new AsyncCallback(SearchDescendantsCallback), baseFolder);
                    FindDescendantsMap.Add(descendReq, result);
                }
            }
            else
            {
                result.Result = LocalFind(baseFolder, regexp, recurse, firstOnly);
                result.CompletedSynchronously = true;
                result.IsCompleted = true;
            }
            return result;
        }

        public List<InventoryBase> EndFindObjects(IAsyncResult result)
        {
            if (result is FindResult)
            {
                FindResult fr = result as FindResult;
                if (!fr.IsCompleted) fr.AsyncWaitHandle.WaitOne();
                return fr.Result;
            }
            else
            {
                throw new Exception("EndFindObjects must be passed the return value of BeginFindObjects.");
            }
        }

        public void SearchDescendantsCallback(IAsyncResult result)
        {
            EndRequestFolderContents(result);
            LLUUID updatedFolder = (LLUUID)result.AsyncState;
            FindResult find = null;
            lock (FindDescendantsMap)
            {
                if (FindDescendantsMap.TryGetValue(result, out find))
                    FindDescendantsMap.Remove(result);
                else
                    return;
            }
            Interlocked.Decrement(ref find.FoldersWaiting);
            List<InventoryBase> folderContents = _Store.GetContents(updatedFolder);
            foreach (InventoryBase obj in folderContents)
            {
                if (find.Regex.IsMatch(obj.Name))
                {
                    find.Result.Add(obj);
                    if (find.FirstOnly)
                    {
                        find.IsCompleted = true;
                        return;
                    }
                }
                if (find.Recurse && obj is InventoryFolder)
                {
                    Interlocked.Increment(ref find.FoldersWaiting);
                    lock (FindDescendantsMap)
                    {
                        IAsyncResult descendReq = BeginRequestFolderContents(
                            obj.UUID,
                            _Client.Network.AgentID,
                            true,
                            true,
                            true,
                            InventorySortOrder.ByName,
                            new AsyncCallback(SearchDescendantsCallback),
                            obj.UUID);
                        FindDescendantsMap.Add(descendReq, find);
                    }
                }
            }

            if (Interlocked.Equals(find.FoldersWaiting, 0))
            {
                find.IsCompleted = true;
            }
        }


        private List<InventoryBase> LocalFind(LLUUID baseFolder, Regex regexp, bool recurse, bool firstOnly)
        {
            List<InventoryBase> objects = new List<InventoryBase>();
            List<InventoryFolder> folders = new List<InventoryFolder>();

            List<InventoryBase> contents = _Store.GetContents(baseFolder);
            foreach (InventoryBase inv in contents)
            {
                if (regexp.IsMatch(inv.Name))
                {
                    objects.Add(inv);
                    if (firstOnly)
                        return objects;
                }
                if (inv is InventoryFolder)
                {
                    folders.Add(inv as InventoryFolder);
                }
            }
            // Recurse outside of the loop because subsequent calls to FindObjects may
            // modify the baseNode.Nodes collection.
            // FIXME: I'm pretty sure this is not necessary
            if (recurse)
            {
                foreach (InventoryFolder folder in folders)
                {
                    objects.AddRange(LocalFind(folder.UUID, regexp, true, firstOnly));
                }
            }
            return objects;
        }

        private class FindObjectsByPathState
        {
            public FindResult Result;
            public LLUUID Folder;
            public int Level;

            public FindObjectsByPathState(FindResult result, LLUUID folder, int level)
            {
                Result = result;
                Folder = folder;
                Level = level;
            }
        }

        public List<InventoryBase> FindObjectsByPath(LLUUID baseFolder, string[] path, bool refresh, bool firstOnly)
        {
            IAsyncResult r = BeginFindObjectsByPath(baseFolder, path, refresh, firstOnly, null, null);
            return EndFindObjects(r);
        }
        
        public IAsyncResult BeginFindObjectsByPath(LLUUID baseFolder, string[] path, bool refresh, bool firstOnly, AsyncCallback callback, object asyncState)
        {
            if (path.Length == 0)
                throw new ArgumentException("Empty path is not supported");
            
            FindResult result = new FindResult(path, callback);
            result.FirstOnly = firstOnly;
            result.AsyncState = asyncState;
            
            if (refresh)
            {
                result.FoldersWaiting = 1;
                BeginRequestFolderContents(
                    baseFolder,
                    _Client.Network.AgentID,
                    true,
                    true,
                    false,
                    InventorySortOrder.ByName,
                    new AsyncCallback(FindObjectsByPathCallback),
                    new FindObjectsByPathState(result, baseFolder, 0));
            }
            else
            {
                result.Result = LocalFind(baseFolder, path, 0, firstOnly);
                result.CompletedSynchronously = true;
                result.IsCompleted = true;
            }

            return result;
        }

        private void FindObjectsByPathCallback(IAsyncResult result)
        {
            EndRequestFolderContents(result);
            FindObjectsByPathState state = (FindObjectsByPathState)result.AsyncState;

            Interlocked.Decrement(ref state.Result.FoldersWaiting);
            List<InventoryBase> folderContents = _Store.GetContents(state.Folder);

            foreach (InventoryBase obj in folderContents)
            {
                if (obj.Name.CompareTo(state.Result.Path[state.Level]) == 0)
                {
                    if (state.Level == state.Result.Path.Length - 1)
                    {
                        state.Result.Result.Add(obj);

                        if (state.Result.FirstOnly)
                        {
                            state.Result.IsCompleted = true;
                            return;
                        }
                    }
                    else if (obj is InventoryFolder)
                    {
                        Interlocked.Increment(ref state.Result.FoldersWaiting);
                        BeginRequestFolderContents(
                            obj.UUID,
                            _Client.Network.AgentID,
                            true,
                            true,
                            false,
                            InventorySortOrder.ByName,
                            new AsyncCallback(FindObjectsByPathCallback),
                            new FindObjectsByPathState(state.Result, obj.UUID, state.Level + 1));
                    }
                }
            }

            if (Interlocked.Equals(state.Result.FoldersWaiting, 0))
                state.Result.IsCompleted = true;
        }

        private List<InventoryBase> LocalFind(LLUUID baseFolder, string[] path, int level, bool firstOnly)
        {
            List<InventoryBase> objects = new List<InventoryBase>();
            List<InventoryFolder> folders = new List<InventoryFolder>();
            List<InventoryBase> contents = _Store.GetContents(baseFolder);

            foreach (InventoryBase inv in contents)
            {
                if (inv.Name.CompareTo(path[level]) == 0)
                {
                    if (level == path.Length - 1)
                    {
                        objects.Add(inv);
                        if (firstOnly) return objects;
                    }
                    else if (inv is InventoryFolder)
                        objects.AddRange(LocalFind(inv.UUID, path, level + 1, firstOnly));
                }
            }

            return objects;
        }

        #endregion

        #region Folder Actions

        public void RequestFolderContents(LLUUID folder, LLUUID owner, bool folders, bool items, bool recurse,
            InventorySortOrder order)
        {
            EndRequestFolderContents(BeginRequestFolderContents(folder, owner, folders, items, recurse, order, null, null));
        }

        public IAsyncResult BeginRequestFolderContents(LLUUID folder, LLUUID owner, bool folders, bool items, bool recurse, InventorySortOrder order, AsyncCallback callback, object asyncState)
        {
            DescendantsResult result = new DescendantsResult(callback);
            result.AsyncState = asyncState;
            result.Folders = folders;
            result.Items = items;
            result.Recurse = recurse;
            result.SortOrder = order;
            return InternalFolderContentsRequest(folder, owner, result);
        }

        /// <summary>
        /// Returns the UUID of the folder (category) that defaults to
        /// containing 'type'. The folder is not necessarily only for that
        /// type
        /// </summary>
        /// <remarks>This will create a new inventory folder on the fly if
        /// one does not exist</remarks>
        /// <param name="type"></param>
        /// <returns>The UUID of the desired or newly created folder, or
        /// LLUUID.Zero on failure</returns>
        public LLUUID FindFolderForType(AssetType type)
        {
            if (_Store == null) 
            {
                _Client.Log("Inventory is null, FindFolderForType() lookup cannot continue", 
                    Helpers.LogLevel.Error);
                return LLUUID.Zero;
            }

            // Folders go in the root
            if (type == AssetType.Folder)
                return _Store.RootFolder.UUID;

            // Loop through each top-level directory and check if PreferredType
            // matches the requested type
            List<InventoryBase> contents = _Store.GetContents(_Store.RootFolder.UUID);
            foreach (InventoryBase inv in contents)
            {
                if (inv is InventoryFolder)
                {
                    InventoryFolder folder = inv as InventoryFolder;

                    if (folder.PreferredType == type)
                        return folder.UUID;
                }
            }

            // No match found, create one
            return CreateFolder(_Store.RootFolder.UUID, type, String.Empty);
        }

        public LLUUID CreateFolder(LLUUID parentID, AssetType preferredType, string name)
        {
            LLUUID id = LLUUID.Random();

            // Assign a folder name if one is not already set
            if (String.IsNullOrEmpty(name))
            {
                if (preferredType >= AssetType.Texture && preferredType <= AssetType.Gesture)
                {
                    name = _NewFolderNames[(int)preferredType];
                }
                else
                {
                    name = "New Folder";
                }
            }

            // Create the new folder locally
            InventoryFolder newFolder = new InventoryFolder(id);
            newFolder.Version = 1;
            newFolder.DescendentCount = 0;
            newFolder.ParentUUID = parentID;
            newFolder.PreferredType = preferredType;
            newFolder.Name = name;
            newFolder.OwnerID = _Client.Network.AgentID;

            try
            {
                _Store[newFolder.UUID] = newFolder;
            }
            catch (InventoryException ie)
            {
                _Client.Log(ie.Message, Helpers.LogLevel.Warning);
            }

            // Create the create folder packet and send it
            CreateInventoryFolderPacket create = new CreateInventoryFolderPacket();
            create.AgentData.AgentID = _Client.Network.AgentID;
            create.AgentData.SessionID = _Client.Network.SessionID;
            create.FolderData.FolderID = id;
            create.FolderData.ParentID = parentID;
            create.FolderData.Type = (sbyte)preferredType;
            create.FolderData.Name = Helpers.StringToField(name);

            _Client.Network.SendPacket(create);

            return id;
        }

        public void RemoveDescendants(LLUUID folder)
        {
            PurgeInventoryDescendentsPacket purge = new PurgeInventoryDescendentsPacket();
            purge.AgentData.AgentID = _Client.Network.AgentID;
            purge.AgentData.SessionID = _Client.Network.SessionID;
            purge.InventoryData.FolderID = folder;
            _Client.Network.SendPacket(purge);

            // Update our local copy:
            if (_Store.Contains(folder))
            {
                List<InventoryBase> contents = _Store.GetContents(folder);
                foreach (InventoryBase obj in contents) {
                    _Store.RemoveNodeFor(obj);
                }
            }
        }

        public void RemoveFolder(LLUUID folder)
        {
            List<LLUUID> folders = new List<LLUUID>(1);
            folders.Add(folder);
            Remove(null, folders);
        }

        #endregion Folder Actions

        #region Item Actions

        public void BeginCreateItem(LLUUID parentFolder, string name, string description, AssetType type, InventoryType invType,
            PermissionMask nextOwnerMask, ItemCreatedCallback callback)
        {
            // Even though WearableType 0 is Shape, in this context it is treated as NOT_WEARABLE
            BeginCreateItem(parentFolder, name, description, type, invType, (WearableType)0, nextOwnerMask, callback);
        }

        public void BeginCreateItem(LLUUID parentFolder, string name, string description, AssetType type, InventoryType invType,
            WearableType wearableType,  PermissionMask nextOwnerMask, ItemCreatedCallback callback)
        {
            CreateInventoryItemPacket create = new CreateInventoryItemPacket();
            create.AgentData.AgentID = _Client.Network.AgentID;
            create.AgentData.SessionID = _Client.Network.SessionID;

            create.InventoryBlock.CallbackID = RegisterInventoryCallback(callback);
            create.InventoryBlock.FolderID = parentFolder;
            create.InventoryBlock.TransactionID = LLUUID.Random();
            create.InventoryBlock.NextOwnerMask = (uint)nextOwnerMask;
            create.InventoryBlock.Type = (sbyte)type;
            create.InventoryBlock.InvType = (sbyte)invType;
            create.InventoryBlock.WearableType = (byte)wearableType;
            create.InventoryBlock.Name = Helpers.StringToField(name);
            create.InventoryBlock.Description = Helpers.StringToField(description);

            _Client.Network.SendPacket(create);
        }

        public void BeginCreateItemFromAsset(byte[] data, string name, string description, AssetType assetType, 
            InventoryType invType, LLUUID folderID, ItemCreatedCallback callback)
        {
            string url = _Client.Network.CurrentSim.Caps.CapabilityURI("NewFileAgentInventory");

            if (url != String.Empty)
            {
                Hashtable query = new Hashtable();
                query.Add("folder_id", folderID);
                query.Add("asset_type", AssetTypeToString(assetType));
                query.Add("inventory_type", InventoryTypeToString(invType));
                query.Add("name", name);
                query.Add("description", description);

                byte[] postData = LLSD.LLSDSerialize(query);

                // Make the request
                CapsRequest request = new CapsRequest(url, _Client.Network.CurrentSim);
                request.OnCapsResponse += new CapsRequest.CapsResponseCallback(CreateItemFromAssetResponse);
                request.MakeRequest(postData, "application/xml", _Client.Network.CurrentSim.udpPort, 
                    new KeyValuePair<ItemCreatedCallback, byte[]>(callback, data));
            }
            else
            {
                throw new Exception("NewFileAgentInventory capability is not currently available");
            }
        }

        private void CreateItemFromAssetResponse(object response, HttpRequestState state)
        {
            Hashtable contents = (Hashtable)response;
            KeyValuePair<ItemCreatedCallback, byte[]> kvp = (KeyValuePair<ItemCreatedCallback, byte[]>)state.State;
            ItemCreatedCallback callback = kvp.Key;
            byte[] itemData = (byte[])kvp.Value;

            string status = (string)contents["state"];

            if (status == "upload")
            {
                string uploadURL = (string)contents["uploader"];

                // This makes the assumption that all uploads go to CurrentSim, to avoid
                // the problem of HttpRequestState not knowing anything about simulators
                CapsRequest upload = new CapsRequest(uploadURL, _Client.Network.CurrentSim);
                upload.OnCapsResponse += new CapsRequest.CapsResponseCallback(CreateItemFromAssetResponse);
                upload.MakeRequest(itemData, "application/octet-stream", _Client.Network.CurrentSim.udpPort, kvp);
            }
            else if (status == "complete")
            {
                //FIXME: Callback successfully
                callback(true, null);
            }
            else
            {
                // Failure
                try { callback(false, null); }
                catch (Exception e) { _Client.Log(e.ToString(), Helpers.LogLevel.Error); }
            }
        }

        public void CopyItem(LLUUID currentOwner, LLUUID itemID, LLUUID parentID, string newName)
        {
            throw new NotImplementedException();
        }

        public void CopyItemFromNotecard(LLUUID objectID, LLUUID notecardID, LLUUID folderID, LLUUID itemID)
        {
            CopyInventoryFromNotecardPacket copy = new CopyInventoryFromNotecardPacket();
            copy.AgentData.AgentID = _Client.Network.AgentID;
            copy.AgentData.SessionID = _Client.Network.SessionID;

            copy.NotecardData.ObjectID = objectID;
            copy.NotecardData.NotecardItemID = notecardID;

            copy.InventoryData = new CopyInventoryFromNotecardPacket.InventoryDataBlock[1];
            copy.InventoryData[0] = new CopyInventoryFromNotecardPacket.InventoryDataBlock();
            copy.InventoryData[0].FolderID = folderID;
            copy.InventoryData[0].ItemID = itemID;

            _Client.Network.SendPacket(copy);
        }

        public void MoveItem(LLUUID itemID, LLUUID parentID, string newName)
        {
            throw new NotImplementedException();
        }

        public void RemoveItem(LLUUID item)
        {
            List<LLUUID> items = new List<LLUUID>(1);
            items.Add(item);
            Remove(items, null);
        }

        public void GiveItem(LLUUID itemID, string itemName, AssetType assetType, LLUUID recipient, bool doEffect)
        {
            byte[] bucket = new byte[17];
            bucket[0] = (byte)assetType;
            Buffer.BlockCopy(itemID.GetBytes(), 0, bucket, 1, 16);

            _Client.Self.InstantMessage(
                _Client.Self.Name,
                recipient,
                itemName,
                LLUUID.Random(),
                InstantMessageDialog.InventoryOffered,
                InstantMessageOnline.Online,
                _Client.Self.Position,
                _Client.Network.CurrentSim.ID,
                bucket);

            if (doEffect)
            {
                _Client.Self.BeamEffect(_Client.Network.AgentID, recipient, LLVector3d.Zero, 
                    _Client.Settings.DEFAULT_EFFECT_COLOR, 1f, LLUUID.Random());
            }
        }

        /// <summary>
        /// Rez an object from inventory
        /// </summary>
        /// <param name="simulator">Simulator to place object in</param>
        /// <param name="rotation">Rotation of the object when rezzed</param>
        /// <param name="position">Vector of where to place object</param>
        /// <param name="item">InventoryObject object containing item details</param>
        public LLUUID RezFromInventory(Simulator simulator, LLQuaternion rotation, LLVector3 position, InventoryObject item)
        {
            return RezFromInventory(simulator, rotation, position, item, _Client.Self.ActiveGroup, LLUUID.Random());
        }

        /// <summary>
        /// Rez an object from inventory
        /// </summary>
        /// <param name="simulator">Simulator to place object in</param>
        /// <param name="rotation">Rotation of the object when rezzed</param>
        /// <param name="position">Vector of where to place object</param>
        /// <param name="item">InventoryObject object containing item details</param>
        /// <param name="groupOwner">LLUUID of group to own the object</param>
        public LLUUID RezFromInventory(Simulator simulator, LLQuaternion rotation, LLVector3 position, InventoryObject item,
            LLUUID groupOwner)
        {
            return RezFromInventory(simulator, rotation, position, item, groupOwner, LLUUID.Random());
        }

        /// <summary>
        /// Rez an object from inventory
        /// </summary>
        /// <param name="simulator">Simulator to place object in</param>
        /// <param name="rotation">Rotation of the object when rezzed</param>
        /// <param name="position">Vector of where to place object</param>
        /// <param name="item">InventoryObject object containing item details</param>
        /// <param name="groupOwner">LLUUID of group to own the object.</param>        
        /// <param name="queryID">User defined queryID to correlate replies.</param>
        public LLUUID RezFromInventory(Simulator simulator, LLQuaternion rotation, LLVector3 position, InventoryObject item,
            LLUUID groupOwner, LLUUID queryID)
        {
            RezObjectPacket add = new RezObjectPacket();

            add.AgentData.AgentID = _Client.Network.AgentID;
            add.AgentData.SessionID = _Client.Network.SessionID;
            add.AgentData.GroupID = groupOwner;

            add.RezData.FromTaskID = LLUUID.Zero;
            add.RezData.BypassRaycast = 1;
            add.RezData.RayStart = position;
            add.RezData.RayEnd = position;
            add.RezData.RayTargetID = LLUUID.Zero;
            add.RezData.RayEndIsIntersection = false;
            add.RezData.RezSelected = false;
            add.RezData.RemoveItem = false;
            add.RezData.ItemFlags = item.Flags;
            add.RezData.GroupMask = (uint)item.Permissions.GroupMask;
            add.RezData.EveryoneMask = (uint)item.Permissions.EveryoneMask;
            add.RezData.NextOwnerMask = (uint)item.Permissions.NextOwnerMask;

            add.InventoryData.ItemID = item.UUID;
            add.InventoryData.FolderID = item.ParentUUID;
            add.InventoryData.CreatorID = item.CreatorID;
            add.InventoryData.OwnerID = item.OwnerID;
            add.InventoryData.GroupID = item.GroupID;
            add.InventoryData.BaseMask = (uint)item.Permissions.BaseMask;
            add.InventoryData.OwnerMask = (uint)item.Permissions.OwnerMask;
            add.InventoryData.GroupMask = (uint)item.Permissions.GroupMask;
            add.InventoryData.EveryoneMask = (uint)item.Permissions.EveryoneMask;
            add.InventoryData.NextOwnerMask = (uint)item.Permissions.NextOwnerMask;
            add.InventoryData.GroupOwned = item.GroupOwned;
            add.InventoryData.TransactionID = queryID;
            add.InventoryData.Type = (sbyte)item.InventoryType;
            add.InventoryData.InvType = (sbyte)item.InventoryType;
            add.InventoryData.Flags = item.Flags;
            add.InventoryData.SaleType = (byte)item.SaleType;
            add.InventoryData.SalePrice = item.SalePrice;
            add.InventoryData.Name = Helpers.StringToField(item.Name);
            add.InventoryData.Description = Helpers.StringToField(item.Description);
            add.InventoryData.CreationDate = (int)Helpers.DateTimeToUnixTime(item.CreationDate);

            _Client.Network.SendPacket(add, simulator);
            return queryID;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string AssetTypeToString(AssetType type)
        {
            return _AssetTypeNames[(int)type];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string InventoryTypeToString(InventoryType type)
        {
            return _InventoryTypeNames[(int)type];
        }

        internal void InitializeRootNode(LLUUID rootFolderID)
        {
            InventoryFolder rootFolder = new InventoryFolder(rootFolderID);
            rootFolder.Name = String.Empty;
            rootFolder.ParentUUID = LLUUID.Zero;

            _Store = new Inventory(_Client, this, rootFolder);
        }

        #region Private Helper Functions

        private uint RegisterInventoryCallback(ItemCreatedCallback callback)
        {
            if (_ItemCreatedCallbackPos == UInt32.MaxValue)
                _ItemCreatedCallbackPos = 0;

            _ItemCreatedCallbackPos++;

            if (_ItemCreatedCallbacks.ContainsKey(_ItemCreatedCallbackPos))
                _Client.Log("Overwriting an existing ItemCreatedCallback", Helpers.LogLevel.Warning);

            _ItemCreatedCallbacks[_ItemCreatedCallbackPos] = callback;

            return _ItemCreatedCallbackPos;
        }

        private InventoryItem CreateInventoryItem(InventoryType type, LLUUID id)
        {
            switch (type)
            {
                case InventoryType.Texture: return new InventoryTexture(id);
                case InventoryType.Sound: return new InventorySound(id);
                case InventoryType.CallingCard: return new InventoryCallingCard(id);
                case InventoryType.Landmark: return new InventoryLandmark(id);
                case InventoryType.Object: return new InventoryObject(id);
                case InventoryType.Notecard: return new InventoryNotecard(id);
                case InventoryType.Category: return new InventoryCategory(id);
                case InventoryType.LSL: return new InventoryLSL(id);
                case InventoryType.Snapshot: return new InventorySnapshot(id);
                case InventoryType.Attachment: return new InventoryAttachment(id);
                case InventoryType.Wearable: return new InventoryWearable(id);
                case InventoryType.Animation: return new InventoryAnimation(id);
                case InventoryType.Gesture: return new InventoryGesture(id);
                default: return new InventoryItem(type, id);
            }
        }

        private void EndRequestFolderContents(IAsyncResult result)
        {
            result.AsyncWaitHandle.WaitOne();
        }

        private DescendantsResult InternalFolderContentsRequest(LLUUID folder, LLUUID owner, DescendantsResult parameters)
        {
            lock (_FolderRequests)
            {
                List<DescendantsResult> requestsForFolder;
                if (!_FolderRequests.TryGetValue(folder, out requestsForFolder))
                {
                    requestsForFolder = new List<DescendantsResult>();
                    _FolderRequests.Add(folder, requestsForFolder);
                }
                lock (requestsForFolder)
                    requestsForFolder.Add(parameters);
            }

            FetchInventoryDescendentsPacket fetch = new FetchInventoryDescendentsPacket();
            fetch.AgentData.AgentID = _Client.Network.AgentID;
            fetch.AgentData.SessionID = _Client.Network.SessionID;

            fetch.InventoryData.FetchFolders = parameters.Folders;
            fetch.InventoryData.FetchItems = parameters.Items;
            fetch.InventoryData.FolderID = folder;
            fetch.InventoryData.OwnerID = owner;
            fetch.InventoryData.SortOrder = (int)parameters.SortOrder;

            _Client.Network.SendPacket(fetch);
            return parameters;
        }

        private void HandleDescendantsRetrieved(LLUUID uuid)
        {
            List<DescendantsResult> satisfiedResults = null;
            lock (_FolderRequests)
            {
                if (_FolderRequests.TryGetValue(uuid, out satisfiedResults))
                    _FolderRequests.Remove(uuid);
            }
            if (satisfiedResults == null)
                return;
            lock (satisfiedResults)
            {
                List<InventoryBase> contents = _Store.GetContents(uuid);
                foreach (DescendantsResult result in satisfiedResults)
                {
                    if (result.Recurse)
                    {
                        bool done = true;

                        foreach (InventoryBase obj in contents)
                        {
                            if (obj is InventoryFolder)
                            {
                                done = false;
                                DescendantsResult child = new DescendantsResult(null);
                                child.Folders = result.Folders;
                                child.Items = result.Items;
                                child.Recurse = result.Recurse;
                                child.SortOrder = result.SortOrder;
                                child.Parent = result;
                                result.AddChild(child);
                                InternalFolderContentsRequest(obj.UUID, obj.OwnerID, child);
                            }
                        }
                        if (done)
                            result.IsCompleted = true;
                    }
                    else
                    {
                        result.IsCompleted = true;
                    }
                }
            }
        }

        #endregion Private Helper Functions

        #region Callbacks

        private void SaveAssetIntoInventoryHandler(Packet packet, Simulator simulator)
        {
            SaveAssetIntoInventoryPacket save = (SaveAssetIntoInventoryPacket)packet;

            // FIXME: Find this item in the inventory structure and mark the parent as needing an update
            //save.InventoryData.ItemID;
        }

        private void InventoryDescendentsHandler(Packet packet, Simulator simulator)
        {
            InventoryDescendentsPacket reply = (InventoryDescendentsPacket)packet;
            InventoryFolder parentFolder = null;

            if (_Store.Contains(reply.AgentData.FolderID) && 
                _Store[reply.AgentData.FolderID] is InventoryFolder)
            {
                parentFolder = _Store[reply.AgentData.FolderID] as InventoryFolder;
            }
            else
            {
                _Client.Log("Don't have a reference to FolderID " + reply.AgentData.FolderID.ToStringHyphenated() +
                    " or it is not a folder", Helpers.LogLevel.Error);
                return;
            }

            if (reply.AgentData.Version < parentFolder.Version)
            {
                _Client.Log("Got an outdated InventoryDescendents packet for folder " + parentFolder.Name +
                    ", this version = " + reply.AgentData.Version + ", latest version = " + parentFolder.Version,
                    Helpers.LogLevel.Warning);
                return;
            }

            if (reply.AgentData.Descendents > 0)
            {
                // InventoryDescendantsReply sends a null folder if the parent doesnt contain any folders.
                if (reply.FolderData[0].FolderID != LLUUID.Zero)
                {
                    // Iterate folders in this packet
                    for (int i = 0; i < reply.FolderData.Length; i++)
                    {
                        InventoryFolder folder = new InventoryFolder(reply.FolderData[i].FolderID);
                        folder.ParentUUID = reply.FolderData[i].ParentID;
                        folder.Name = Helpers.FieldToUTF8String(reply.FolderData[i].Name);
                        folder.PreferredType = (AssetType)reply.FolderData[i].Type;
                        folder.OwnerID = reply.AgentData.OwnerID;

                        _Store[folder.UUID] = folder;
                    }
                }

                // InventoryDescendantsReply sends a null item if the parent doesnt contain any items.
                if (reply.ItemData[0].ItemID != LLUUID.Zero)
                {
                    // Iterate items in this packet
                    for (int i = 0; i < reply.ItemData.Length; i++)
                    {
                        if (reply.ItemData[i].ItemID != LLUUID.Zero)
                        {
                            InventoryItem item = CreateInventoryItem((InventoryType)reply.ItemData[i].InvType,reply.ItemData[i].ItemID);
                            item.ParentUUID = reply.ItemData[i].FolderID;
                            item.CreatorID = reply.ItemData[i].CreatorID;
                            item.AssetType = (AssetType)reply.ItemData[i].Type;
                            item.AssetUUID = reply.ItemData[i].AssetID;
                            item.CreationDate = Helpers.UnixTimeToDateTime((uint)reply.ItemData[i].CreationDate);
                            item.Description = Helpers.FieldToUTF8String(reply.ItemData[i].Description);
                            item.Flags = reply.ItemData[i].Flags;
                            item.Name = Helpers.FieldToUTF8String(reply.ItemData[i].Name);
                            item.GroupID = reply.ItemData[i].GroupID;
                            item.GroupOwned = reply.ItemData[i].GroupOwned;
                            item.Permissions = new Permissions(
                                reply.ItemData[i].BaseMask,
                                reply.ItemData[i].EveryoneMask,
                                reply.ItemData[i].GroupMask,
                                reply.ItemData[i].NextOwnerMask,
                                reply.ItemData[i].OwnerMask);
                            item.SalePrice = reply.ItemData[i].SalePrice;
                            item.SaleType = (SaleType)reply.ItemData[i].SaleType;
                            item.OwnerID = reply.AgentData.OwnerID;

                            _Store[item.UUID] = item;
                        }
                    }
                }
            }

            parentFolder.Version = reply.AgentData.Version;
            parentFolder.DescendentCount = reply.AgentData.Descendents;

            if (OnInventoryFolderUpdated != null)
            {
                try { OnInventoryFolderUpdated(parentFolder.UUID); }
                catch (Exception e) { _Client.Log(e.ToString(), Helpers.LogLevel.Error); }
            }

            // For RequestFolderContents - only call the handler if we've retrieved all the descendants.
            if (_FolderRequests.ContainsKey(parentFolder.UUID) && parentFolder.DescendentCount == _Store.GetContents(parentFolder.UUID).Count)
                HandleDescendantsRetrieved(parentFolder.UUID);
        }

        /// <summary>
        /// UpdateCreateInventoryItem packets are received when a new inventory item 
        /// is created. This may occur when an object that's rezzed in world is
        /// taken into inventory, when an item is created using the CreateInventoryItem
        /// packet, or when an object is purchased.
        /// </summary>
        private void UpdateCreateInventoryItemHandler(Packet packet, Simulator simulator)
        {
            UpdateCreateInventoryItemPacket reply = packet as UpdateCreateInventoryItemPacket;

            foreach (UpdateCreateInventoryItemPacket.InventoryDataBlock dataBlock in reply.InventoryData)
            {
                if (dataBlock.InvType == (sbyte)InventoryType.Folder) {
                    _Client.Log("Received InventoryFolder in an UpdateCreateInventoryItem packet.", Helpers.LogLevel.Error);
                    continue;
                }

                InventoryItem item = CreateInventoryItem((InventoryType)dataBlock.InvType,dataBlock.ItemID);
                item.AssetType = (AssetType)dataBlock.Type;
                item.AssetUUID = dataBlock.AssetID;
                item.CreationDate = DateTime.FromBinary(dataBlock.CreationDate);
                item.Description = Helpers.FieldToUTF8String(dataBlock.Description);
                item.Flags = dataBlock.Flags;
                item.GroupID = dataBlock.GroupID;
                item.GroupOwned = dataBlock.GroupOwned;
                item.Name = Helpers.FieldToUTF8String(dataBlock.Name);
                item.OwnerID = dataBlock.OwnerID;
                item.ParentUUID = dataBlock.FolderID;
                item.Permissions = new Permissions(
                        dataBlock.BaseMask,
                        dataBlock.EveryoneMask,
                        dataBlock.GroupMask,
                        dataBlock.NextOwnerMask,
                        dataBlock.OwnerMask);
                item.SalePrice = dataBlock.SalePrice;
                item.SaleType = (SaleType)dataBlock.SaleType;

                _Store[item.UUID] = item;

                ItemCreatedCallback callback;
                if (_ItemCreatedCallbacks.TryGetValue(dataBlock.CallbackID, out callback))
                {
                    _ItemCreatedCallbacks.Remove(dataBlock.CallbackID);

                    try { callback(true, item); }
                    catch (Exception e) { _Client.Log(e.ToString(), Helpers.LogLevel.Error); }
                }
            }
        }

        private void MoveInventoryItemHandler(Packet packet, Simulator simulator)
        {
            MoveInventoryItemPacket move = (MoveInventoryItemPacket)packet;

            for (int i = 0; i < move.InventoryData.Length; i++)
            {
                // FIXME: Do something here
                string newName = Helpers.FieldToUTF8String(move.InventoryData[i].NewName);

                _Client.DebugLog(String.Format("MoveInventoryItemHandler: Item {0} is moving to Folder {1} with new name \"{2}\"",
                    move.InventoryData[i].ItemID.ToStringHyphenated(), move.InventoryData[i].FolderID.ToStringHyphenated(),
                    newName));
            }
        }

        private void BulkUpdateInventoryHandler(Packet packet, Simulator simulator)
        {
            BulkUpdateInventoryPacket update = packet as BulkUpdateInventoryPacket;

            if (update.FolderData.Length > 0 && update.FolderData[0].FolderID != LLUUID.Zero)
            {
                foreach (BulkUpdateInventoryPacket.FolderDataBlock dataBlock in update.FolderData)
                {

                    if (_Store.Contains(dataBlock.FolderID))
                        _Client.Log("Received BulkUpdate for unknown folder: " + dataBlock.FolderID, Helpers.LogLevel.Warning);

                    InventoryFolder folder = new InventoryFolder(dataBlock.FolderID);
                    folder.Name = Helpers.FieldToUTF8String(dataBlock.Name);
                    folder.OwnerID = update.AgentData.AgentID;
                    folder.ParentUUID = dataBlock.ParentID;
                    _Store[folder.UUID] = folder;
                }
            }

            if (update.ItemData.Length > 0 && update.ItemData[0].ItemID != LLUUID.Zero)
            {
                foreach (BulkUpdateInventoryPacket.ItemDataBlock dataBlock in update.ItemData)
                {
                    if (!_Store.Contains(dataBlock.ItemID))
                        _Client.Log("Received BulkUpdate for unknown item: " + dataBlock.ItemID, Helpers.LogLevel.Warning);

                    // FIXME: Write a helper function that will either fetch an item out of the store or create it
                    // and use that here instead, to prevent overwriting already fetched AssetIDs on an update
                    InventoryItem item = CreateInventoryItem((InventoryType)dataBlock.InvType, dataBlock.ItemID);

                    item.AssetType = (AssetType)dataBlock.Type;
                    if (dataBlock.AssetID != LLUUID.Zero) item.AssetUUID = dataBlock.AssetID;
                    item.CreationDate = DateTime.FromBinary(dataBlock.CreationDate);
                    item.Description = Helpers.FieldToUTF8String(dataBlock.Description);
                    item.Flags = dataBlock.Flags;
                    item.GroupID = dataBlock.GroupID;
                    item.GroupOwned = dataBlock.GroupOwned;
                    item.Name = Helpers.FieldToUTF8String(dataBlock.Name);
                    item.OwnerID = dataBlock.OwnerID;
                    item.ParentUUID = dataBlock.FolderID;
                    item.Permissions = new Permissions(
                        dataBlock.BaseMask,
                        dataBlock.EveryoneMask,
                        dataBlock.GroupMask,
                        dataBlock.NextOwnerMask,
                        dataBlock.OwnerMask);
                    item.SalePrice = dataBlock.SalePrice;
                    item.SaleType = (SaleType)dataBlock.SaleType;

                    _Store[item.UUID] = item;

                    ItemCreatedCallback callback;
                    if (_ItemCreatedCallbacks.TryGetValue(dataBlock.CallbackID, out callback))
                    {
                        _ItemCreatedCallbacks.Remove(dataBlock.CallbackID);

                        try { callback(true, item); }
                        catch (Exception e) { _Client.Log(e.ToString(), Helpers.LogLevel.Error); }
                    }
                }
            }
        }

        private void FetchInventoryReplyHandler(Packet packet, Simulator simulator)
        {
            FetchInventoryReplyPacket reply = packet as FetchInventoryReplyPacket;

            foreach (FetchInventoryReplyPacket.InventoryDataBlock dataBlock in reply.InventoryData) 
            {
                if (dataBlock.InvType == (sbyte)InventoryType.Folder)
                {
                    _Client.Log("Received FetchInventoryReply for inventory folder!", Helpers.LogLevel.Error);
                    continue;
                }

                InventoryItem item = CreateInventoryItem((InventoryType)dataBlock.InvType,dataBlock.ItemID);
                item.AssetType = (AssetType)dataBlock.Type;
                item.AssetUUID = dataBlock.AssetID;
                item.CreationDate = DateTime.FromBinary(dataBlock.CreationDate);
                item.Description = Helpers.FieldToUTF8String(dataBlock.Description);
                item.Flags = dataBlock.Flags;
                item.GroupID = dataBlock.GroupID;
                item.GroupOwned = dataBlock.GroupOwned;
                item.Name = Helpers.FieldToUTF8String(dataBlock.Name);
                item.OwnerID = dataBlock.OwnerID;
                item.ParentUUID = dataBlock.FolderID;
                item.Permissions = new Permissions(
                    dataBlock.BaseMask, 
                    dataBlock.EveryoneMask, 
                    dataBlock.GroupMask, 
                    dataBlock.NextOwnerMask, 
                    dataBlock.OwnerMask);
                item.SalePrice = dataBlock.SalePrice;
                item.SaleType = (SaleType)dataBlock.SaleType;

                _Store[item.UUID] = item;
            }
        }
        private void Self_OnInstantMessage(InstantMessage im, Simulator simulator)
        {
            // TODO: MainAvatar.InstantMessageDialog.GroupNotice can also be an inventory offer, should we
            // handle it here?

            if (OnInventoryObjectReceived != null && 
                (im.Dialog == InstantMessageDialog.InventoryOffered || im.Dialog == InstantMessageDialog.TaskInventoryOffered))
            {
                AssetType type = AssetType.Unknown;
                LLUUID objectID = LLUUID.Zero;
                bool fromTask = false;

                if (im.Dialog == InstantMessageDialog.InventoryOffered)
                {
                    if (im.BinaryBucket.Length == 17)
                    {
                        type = (AssetType)im.BinaryBucket[0];
                        objectID = new LLUUID(im.BinaryBucket, 1);
                        fromTask = false;
                    }
                    else
                    {
                        _Client.Log("Malformed inventory offer from agent", Helpers.LogLevel.Warning);
                        return;
                    }
                }
                else if (im.Dialog == InstantMessageDialog.TaskInventoryOffered)
                {
                    if (im.BinaryBucket.Length == 1)
                    {
                        type = (AssetType)im.BinaryBucket[0];
                        fromTask = true;
                    }
                    else
                    {
                        _Client.Log("Malformed inventory offer from object", Helpers.LogLevel.Warning);
                        return;
                    }
                }

                // Find the folder where this is going to go
                LLUUID destinationFolderID = FindFolderForType(type);

                // Fire the callback
                try
                {
                    ImprovedInstantMessagePacket imp = new ImprovedInstantMessagePacket();
                    imp.AgentData.AgentID = _Client.Network.AgentID;
                    imp.AgentData.SessionID = _Client.Network.SessionID;
                    imp.MessageBlock.FromGroup = false;
                    imp.MessageBlock.ToAgentID = im.FromAgentID;
                    imp.MessageBlock.Offline = 0;
                    imp.MessageBlock.ID = im.IMSessionID;
                    imp.MessageBlock.Timestamp = 0;
                    imp.MessageBlock.FromAgentName = Helpers.StringToField(_Client.Self.Name);
                    imp.MessageBlock.Message = new byte[0];
                    imp.MessageBlock.ParentEstateID = 0;
                    imp.MessageBlock.RegionID = LLUUID.Zero;
                    imp.MessageBlock.Position = _Client.Self.Position;

                    if (OnInventoryObjectReceived(im.FromAgentID, im.FromAgentName, im.ParentEstateID, im.RegionID, im.Position,
                        im.Timestamp, type, objectID, fromTask))
                    {
                        // Accept the inventory offer
                        switch (im.Dialog)
                        {
                            case InstantMessageDialog.InventoryOffered:
                                imp.MessageBlock.Dialog = (byte)InstantMessageDialog.InventoryAccepted;
                                break;
                            case InstantMessageDialog.TaskInventoryOffered:
                                imp.MessageBlock.Dialog = (byte)InstantMessageDialog.TaskInventoryOffered;
                                break;
                            case InstantMessageDialog.GroupNotice:
                                imp.MessageBlock.Dialog = (byte)InstantMessageDialog.GroupNoticeInventoryAccepted;
                                break;
                        }

                        imp.MessageBlock.BinaryBucket = destinationFolderID.GetBytes();
                    }
                    else
                    {
                        // Decline the inventory offer
                        switch (im.Dialog)
                        {
                            case InstantMessageDialog.InventoryOffered:
                                imp.MessageBlock.Dialog = (byte)InstantMessageDialog.InventoryDeclined;
                                break;
                            case InstantMessageDialog.TaskInventoryOffered:
                                imp.MessageBlock.Dialog = (byte)InstantMessageDialog.TaskInventoryDeclined;
                                break;
                            case InstantMessageDialog.GroupNotice:
                                imp.MessageBlock.Dialog = (byte)InstantMessageDialog.GroupNoticeInventoryDeclined;
                                break;
                        }

                        imp.MessageBlock.BinaryBucket = new byte[0];
                    }

                    _Client.Network.SendPacket(imp, simulator);
                }
                catch (Exception e)
                {
                    _Client.Log(e.ToString(), Helpers.LogLevel.Error);
                }
            }
        }

        #endregion Callbacks
    }

    class FindResult : IAsyncResult
    {
        public List<InventoryBase> Result;
        public int FoldersWaiting;
        public bool FirstOnly;

        private AsyncCallback callback;
        private Regex regex;
        private string[] path;
        private bool recurse;
        private AutoResetEvent waitHandle;
        private bool complete;
        private bool sync;
        private object asyncstate;

        #region Properties

        public bool Recurse { get { return recurse; } }
        public Regex Regex { get { return regex; } }
        public string[] Path { get { return path; } }
        public AsyncCallback Callback { get { return callback; } }

        #region IAsyncResult Members

        public object AsyncState
        {
            get { return asyncstate; }
            set { asyncstate = value; }
        }

        public WaitHandle AsyncWaitHandle
        {
            get { return waitHandle; }
        }

        public bool CompletedSynchronously
        {
            get { return sync; }
            set { sync = value; }
        }

        public bool IsCompleted
        {
            get { return complete; }
            set
            {
                if (value)
                {
                    waitHandle.Set();
                    if (callback != null)
                    {
                        callback(this);
                    }
                }
                complete = value;
            }
        }

        #endregion

        #endregion Properties

        public FindResult(Regex regex, bool recurse, AsyncCallback callback)
        {
            this.waitHandle = new AutoResetEvent(false);
            this.callback = callback;
            this.recurse = recurse;
            this.regex = regex;
            this.Result = new List<InventoryBase>();
        }

        public FindResult(string[] path, AsyncCallback callback)
        {
            this.waitHandle = new AutoResetEvent(false);
            this.callback = callback;
            this.path = path;
            this.Result = new List<InventoryBase>();
        }
    }

    class DescendantsResult : IAsyncResult
    {
        public bool Folders = true;
        public bool Items = true;
        public bool Recurse = false;
        public InventorySortOrder SortOrder = InventorySortOrder.ByName;
        public DescendantsResult Parent;

        private AsyncCallback _Callback;
        private ManualResetEvent _AsyncWaitHandle;
        private object _AsyncState;
        private bool _IsCompleted;
        private List<DescendantsResult> _ChildrenWaiting = new List<DescendantsResult>();

        #region Properties

        #region IAsyncResult Members

        public object AsyncState
        {
            get { return _AsyncState; }
            set { _AsyncState = value; }
        }

        public WaitHandle AsyncWaitHandle
        {
            get { return _AsyncWaitHandle; }
        }

        public bool CompletedSynchronously
        {
            get { return false; }
        }

        public bool IsCompleted
        {
            get { return _IsCompleted; }
            set
            {
                _IsCompleted = value;
                if (value)
                {
                    if (_ChildrenWaiting.Count == 0)
                    {
                        if (Parent != null)
                        {
                            Parent.ChildComplete(this);
                        }
                        else
                        {
                            _AsyncWaitHandle.Set();
                            if (_Callback != null)
                                _Callback(this);
                        }
                    }
                }
            }
        }

        #endregion

        #endregion Properties

        public DescendantsResult(AsyncCallback callback)
        {
            _Callback = callback;
            _AsyncWaitHandle = new ManualResetEvent(false);
        }

        public void AddChild(DescendantsResult child)
        {
            lock (_ChildrenWaiting)
            {
                if (!child.IsCompleted)
                    _ChildrenWaiting.Add(child);
            }
        }

        public void ChildComplete(DescendantsResult child)
        {
            lock (_ChildrenWaiting)
            {
                _ChildrenWaiting.Remove(child);
                if (_ChildrenWaiting.Count == 0)
                    IsCompleted = true;
            }
        }
    }
}