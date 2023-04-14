using AvnRepository;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.Reflection;
using WebAppAcademics.Client.Services;

namespace WebAppAcademics.Client.OfflineServices
{
    public class AppDBSyncRepo<T> : IRepository<T> where T : class
    {

        // injected
        IBlazorDbFactory _dbFactory;
        private readonly IAPIServices<T> _apiRepository;
        private readonly IJSRuntime _jsRuntime;
        string _dbName = "";
        string _primaryKeyName = "";
        bool _autoGenerateKey;

        IndexedDbManager manager;
        string storeName = "";
        string keyStoreName = "";
        Type entityType;
        PropertyInfo primaryKey;
        public bool IsOnline { get; set; } = true;

        public delegate void OnlineStatusEventHandler(object sender,
            OnlineStatusEventArgs e);
        public event OnlineStatusEventHandler OnlineStatusChanged;

        public AppDBSyncRepo(string dbName, string primaryKeyName, bool autoGenerateKey, IBlazorDbFactory dbFactory,
                                IAPIServices<T> apiRepository, IJSRuntime jsRuntime)
        {
            _dbName = dbName;
            _dbFactory = dbFactory;
            _apiRepository = apiRepository;
            _jsRuntime = jsRuntime;
            _primaryKeyName = primaryKeyName;
            _autoGenerateKey = autoGenerateKey;

            entityType = typeof(T);
            storeName = entityType.Name;
            keyStoreName = $"{storeName}{Globals.KeysSuffix}";
            primaryKey = entityType.GetProperty(primaryKeyName);

            _ = _jsRuntime.InvokeVoidAsync("connectivity.initialize",
                DotNetObjectReference.Create(this));
        }

        public string LocalStoreName
        {
            get { return $"{storeName}{Globals.LocalTransactionsSuffix}"; }
        }

        [JSInvokable("ConnectivityChanged")]
        public async void OnConnectivityChanged(bool isOnline)
        {
            IsOnline = isOnline;

            if (!isOnline)
            {
                OnlineStatusChanged?.Invoke(this,
                    new OnlineStatusEventArgs { IsOnline = false });
            }
            else
            {
                await Task.CompletedTask;
                //await SyncLocalToServer();
                OnlineStatusChanged?.Invoke(this,
                    new OnlineStatusEventArgs { IsOnline = true });
            }
        }

        private async Task EnsureManager()
        {
            if (manager == null)
            {
                manager = await _dbFactory.GetDbManager(_dbName);
                await manager.OpenDb();
            }
        }
      
        #region [Section - Delete Operation]

        public async Task<bool> DeleteAllCustomAsync()
        {
            if (IsOnline)
                return true;

            await DeleteAllOfflineAsync();
            return true;
        }

        private async Task DeleteAllOfflineAsync()
        {
            await EnsureManager();

            // clear the keys table
            await manager.ClearTableAsync(keyStoreName);

            // clear the data table
            await manager.ClearTableAsync(storeName);

            RecordDeleteAllAsync();
        }

        public async void RecordDeleteAllAsync()
        {
            if (IsOnline) return;

            var action = LocalTransactionTypes.DeleteAll;
            var record = new StoreRecord<LocalTransaction<T>>()
            {
                StoreName = LocalStoreName,
                Record = new LocalTransaction<T>
                {
                    Entity = default,
                    Action = action,
                    ActionName = action.ToString()
                }
            };

            await manager.AddRecordAsync(record);
        }

        public async Task<bool> DeleteAsync(string requestUri, int Id, T EntityToDelete)
        {
            bool deleted = false;

            if (IsOnline)
            {
                var onlineId = primaryKey.GetValue(EntityToDelete);
                deleted = await _apiRepository.DeleteAsync(requestUri, Id);
                var localEntity = await UpdateKeyToLocal(EntityToDelete);
                await DeleteOfflineAsync(requestUri, localEntity);
            }
            else
            {
                deleted = await DeleteOfflineAsync(requestUri, EntityToDelete);
            }

            return deleted;
        }

        public async Task<bool> DeleteOfflineAsync(string requestUri, T EntityToDelete)
        {
            await EnsureManager();
            var Id = primaryKey.GetValue(EntityToDelete);
            return await DeleteByIdAsync(requestUri, Convert.ToInt32(Id));
        }

        public async Task<bool> DeleteByIdAsync(string requestUri, int Id)
        {
            bool deleted = false;

            if (IsOnline)
            {
                var localId = await GetLocalId(Id);
                await DeleteByIdOfflineAsync(requestUri, localId);
                deleted = await _apiRepository.DeleteAsync(requestUri, Id);
            }
            else
            {
                deleted = await DeleteByIdOfflineAsync(requestUri, Id);
            }

            return deleted;
        }

        public async Task<bool> DeleteByIdOfflineAsync(string requestUri, object Id)
        {
            await EnsureManager();
            try
            {
                RecordDeleteByIdAsync(requestUri, Id);
                var result = await manager.DeleteRecordAsync(storeName, Id);
                if (result.Failed) return false;

                if (IsOnline)
                {
                    // delete key map only if we're online.
                    var keys = await GetKeys();
                    if (keys.Count > 0)
                    {
                        var key = (from x in keys
                                   where x.LocalId.ToString() == Id.ToString()
                                   select x).FirstOrDefault();
                        if (key != null)
                            await manager.DeleteRecordAsync(keyStoreName, key.Id);
                    }
                }

                return true;
            }
            catch (Exception)
            {
                // log exception
                return false;
            }
        }

        public async void RecordDeleteByIdAsync(string requestUri, object id)
        {
            if (IsOnline) return;
            var action = LocalTransactionTypes.Delete;

            var entity = await GetByIdAsync(requestUri, Convert.ToInt32(id));

            var record = new StoreRecord<LocalTransaction<T>>()
            {
                StoreName = LocalStoreName,
                Record = new LocalTransaction<T>
                {
                    Entity = entity,
                    Action = action,
                    ActionName = action.ToString(),
                    Id = int.Parse(id.ToString())
                }
            };

            await manager.AddRecordAsync(record);
        }

        private async Task DeleteAllTransactionsAsync()
        {
            await EnsureManager();
            await manager.ClearTableAsync(LocalStoreName);
        }

        #endregion

        #region [Section - Get Operation]
        public async Task<List<T>> GetAllAsync(string requestUri)
        {
            return await GetAllAsync(requestUri, false);
        }

        public async Task<List<T>> GetAllAsync(string requestUri, bool dontSync = false)
        {
            if (IsOnline == true)
            {
                // retrieve all the data
                var list = await _apiRepository.GetAllAsync(requestUri);
                if (list != null)
                {
                    var allData = list.ToList();
                    if (!dontSync)
                    {
                        // clear the local db
                        await DeleteAllOfflineAsync();                        
                        var result = await manager.BulkAddRecordAsync<T>
                            (storeName, allData);
                        // get all the local data
                        var localList = await GetAllOfflineAsync();
                        var localData = (localList).ToList();
                        // record the primary keys
                        var keys = new List<OnlineOfflineKey>();
                        for (int i = 0; i < allData.Count(); i++)
                        {
                            var localId = primaryKey.GetValue(localData[i]);
                            var key = new OnlineOfflineKey()
                            {
                                Id = Convert.ToInt32(localId),
                                OnlineId = primaryKey.GetValue(allData[i]),
                                LocalId = localId,
                            };
                            keys.Add(key);
                        };
                        // remove all the keys
                        await manager.ClearTableAsync(keyStoreName);
                        // store all of the keys
                        result = await manager.BulkAddRecordAsync<OnlineOfflineKey>
                            (keyStoreName, keys);
                    }
                    // return the data
                    return allData;
                }
                else
                    return null;
            }
            else
                return await GetAllOfflineAsync();
        }

        public async Task<List<T>> GetAllOfflineAsync()
        {
            await EnsureManager();
            var array = await manager.ToArray<T>(storeName);
            if (array == null)
                return new List<T>();
            else
                return array.ToList();
        }

        public async Task<T> GetByIdAsync(string requestUri, int Id)
        {
            if (IsOnline)
                return await _apiRepository.GetByIdAsync(requestUri, Id);
            else
                return await GetByIdOfflineAsync(Id);
        }

        public async Task<T> GetByIdOfflineAsync(object Id)
        {
            await EnsureManager();
            var items = await manager.Where<T>(storeName, _primaryKeyName, Id);
            if (items.Any())
                return items.First();
            else
                return default;
        }
               
        #endregion

        #region [Section - Save Operation]
        public async Task<T> SaveAsync(string requestUri, T Entity)
        {
            T returnValue;

            if (IsOnline)
            {
                returnValue = await _apiRepository.SaveAsync(requestUri, Entity);
                var Id = primaryKey.GetValue(returnValue);
                await InsertOfflineAsync(returnValue);
            }
            else
            {
                returnValue = await InsertOfflineAsync(Entity);
            }
            return returnValue;
        }

        public async Task<T> InsertOfflineAsync(T Entity)
        {
            await EnsureManager();

            try
            {
                var onlineId = primaryKey.GetValue(Entity);

                var record = new StoreRecord<T>()
                {
                    StoreName = storeName,
                    Record = Entity
                };
                var result = await manager.AddRecordAsync<T>(record);
                var allItems = await GetAllOfflineAsync();
                var last = allItems.Last();
                var localId = primaryKey.GetValue(last);

                // record in the keys database
                var key = new OnlineOfflineKey()
                {
                    Id = Convert.ToInt32(localId),
                    OnlineId = onlineId,
                    LocalId = localId
                };
                var storeRecord = new StoreRecord<OnlineOfflineKey>
                {
                    DbName = _dbName,
                    StoreName = keyStoreName,
                    Record = key
                };
                await manager.AddRecordAsync(storeRecord);

                RecordInsertAsync(last);

                return last;
            }
            catch (Exception)
            {
                // log exception
                return default;
            }
        }

        public async void RecordInsertAsync(T Entity)
        {
            if (IsOnline) return;
            try
            {
                var action = LocalTransactionTypes.Insert;

                var record = new StoreRecord<LocalTransaction<T>>()
                {
                    StoreName = LocalStoreName,
                    Record = new LocalTransaction<T>
                    {
                        Entity = Entity,
                        Action = action,
                        ActionName = action.ToString()
                    }
                };

                await manager.AddRecordAsync(record);
            }
            catch (Exception)
            {
                // log exception
            }
        }

        #endregion

        #region [Section - Update Operation]
        public async Task<T> UpdateAsync(string requestUri, int _Id, T EntityToUpdate)
        {
            T returnValue;

            if (IsOnline)
            {
                returnValue = await _apiRepository.UpdateAsync(requestUri, _Id, EntityToUpdate);
                var Id = primaryKey.GetValue(returnValue);
                returnValue = await UpdateKeyToLocal(returnValue);
                if (returnValue != null)
                {
                    await UpdateOfflineAsync(returnValue);
                }
            }
            else
            {
                returnValue = await UpdateOfflineAsync(EntityToUpdate);
            }
            return returnValue;
        }

        public async Task<T> UpdateOfflineAsync(T EntityToUpdate)
        {
            await EnsureManager();
            object Id = primaryKey.GetValue(EntityToUpdate);
            try
            {
                await manager.UpdateRecord(new UpdateRecord<T>()
                {
                    StoreName = storeName,
                    Record = EntityToUpdate,
                    Key = Id
                });

                RecordUpdateAsync(EntityToUpdate);

                return EntityToUpdate;
            }
            catch (Exception)
            {
                // log exception
                return default;
            }
        }

        public async void RecordUpdateAsync(T Entity)
        {
            if (IsOnline) return;
            try
            {
                var action = LocalTransactionTypes.Update;

                var record = new StoreRecord<LocalTransaction<T>>()
                {
                    StoreName = LocalStoreName,
                    Record = new LocalTransaction<T>
                    {
                        Entity = Entity,
                        Action = action,
                        ActionName = action.ToString()
                    }
                };

                await manager.AddRecordAsync(record);
            }
            catch (Exception)
            {
                // log exception
            }
        }

        private async Task<T> UpdateKeyToLocal(T Entity)
        {
            var OnlineId = primaryKey.GetValue(Entity);
            OnlineId = JsonConvert.DeserializeObject<object>(OnlineId.ToString());

            var keys = await GetKeys();
            if (keys == null) return default;

            var item = (from x in keys
                        where x.OnlineId.ToString() == OnlineId.ToString()
                        select x).FirstOrDefault();

            if (item == null) return default;

            var key = item.LocalId;

            var typeName = key.GetType().Name;

            if (typeName == nameof(Int64))
            {
                if (primaryKey.PropertyType.Name == nameof(Int32))
                    key = Convert.ToInt32(key);
            }
            else if (typeName == "string")
            {
                if (primaryKey.PropertyType.Name != "string")
                    key = key.ToString();
            }

            primaryKey.SetValue(Entity, key);

            return Entity;
        }

        private async Task<T> UpdateKeyFromLocal(T Entity)
        {
            var LocalId = primaryKey.GetValue(Entity);
            LocalId = JsonConvert.DeserializeObject<object>(LocalId.ToString());

            var keys = await GetKeys();
            if (keys == null) return default;

            var item = (from x in keys
                        where x.LocalId.ToString() == LocalId.ToString()
                        select x).FirstOrDefault();

            if (item == null) return default;

            var key = item.OnlineId;

            var typeName = key.GetType().Name;

            if (typeName == nameof(Int64))
            {
                if (primaryKey.PropertyType.Name == nameof(Int32))
                    key = Convert.ToInt32(key);
            }
            else if (typeName == "string")
            {
                if (primaryKey.PropertyType.Name != "string")
                    key = key.ToString();
            }

            primaryKey.SetValue(Entity, key);

            return Entity;
        }


        #endregion

        #region [Section - Sync Operation]
        public async Task<bool> SyncLocalToServer()
        {
            if (!IsOnline) return false;

            await EnsureManager();

            var array = await manager.ToArray<LocalTransaction<T>>(LocalStoreName);
            if (array == null || array.Count == 0)
                return true;
            else
            {
                foreach (var localTransaction in array.ToList())
                {
                    try
                    {
                        switch (localTransaction.Action)
                        {
                            case LocalTransactionTypes.Insert:
                                var insertedEntity = await
                                    _apiRepository.SaveAsync(APICallParameters.RequestUriSave, localTransaction.Entity);
                                // update the keys table
                                var localId = primaryKey.GetValue(localTransaction.Entity);
                                var onlineId = primaryKey.GetValue(insertedEntity);
                                var key = new OnlineOfflineKey()
                                {
                                    Id = Convert.ToInt32(localId),
                                    OnlineId = onlineId,
                                    LocalId = localId
                                };
                                await manager.AddRecordAsync<OnlineOfflineKey>
                                    (new StoreRecord<OnlineOfflineKey>
                                    {
                                        StoreName = keyStoreName,
                                        Record = key
                                    });
                                break;
                            case LocalTransactionTypes.Update:
                                localTransaction.Entity = await UpdateKeyFromLocal(localTransaction.Entity);
                                await _apiRepository.UpdateAsync(APICallParameters.RequestUriUpdate, APICallParameters.Id, localTransaction.Entity);
                                onlineId = primaryKey.GetValue(localTransaction.Entity);
                                break;
                            case LocalTransactionTypes.Delete:
                                localTransaction.Entity = await UpdateKeyFromLocal
                                    (localTransaction.Entity);
                                onlineId = primaryKey.GetValue(localTransaction.Entity);
                                await _apiRepository.DeleteAsync(APICallParameters.RequestUriDelete, APICallParameters.Id);
                                break;

                            case LocalTransactionTypes.DeleteAll:
                                await _apiRepository.DeleteAsync(APICallParameters.RequestUriDelete, APICallParameters.Id);
                                break;

                            default:
                                break;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                await DeleteAllTransactionsAsync();

                // TODO: Get all new records since last online
                // Get last record id
                // ask for new records since that id was recorded in the database
                // may require a time stamp field in the data record (invasive!)

                return true;
            }
        }


        #endregion

        #region [Section - Global Operation]
        private async Task<object> GetLocalId(object OnlineId)
        {
            var keys = await GetKeys();
            var item = (from x in keys
                        where x.OnlineId.ToString() == OnlineId.ToString()
                        select x).FirstOrDefault();
            var localId = item.LocalId;
            localId = JsonConvert.DeserializeObject<object>(localId.ToString());
            return localId;
        }

        private async Task<object> GetOnlineId(object LocalId)
        {
            var keys = await GetKeys();
            var item = (from x in keys
                        where x.LocalId.ToString() == LocalId.ToString()
                        select x).FirstOrDefault();
            var onlineId = item.OnlineId;
            onlineId = JsonConvert.DeserializeObject<object>(onlineId.ToString());
            return onlineId;
        }

        private async Task<List<OnlineOfflineKey>> GetKeys()
        {
            await EnsureManager();
            var returnList = new List<OnlineOfflineKey>();

            var array = await manager.ToArray<OnlineOfflineKey>(keyStoreName);
            if (array == null) return null;

            foreach (var key in array)
            {
                var onlineId = key.OnlineId;
                key.OnlineId = JsonConvert.DeserializeObject<object>(onlineId.ToString());

                var localId = key.LocalId;
                key.LocalId = JsonConvert.DeserializeObject<object>(localId.ToString());

                returnList.Add(key);
            }

            return returnList;
        }

        #endregion

        public async ValueTask DisposeAsync()
        {
            await _jsRuntime.InvokeVoidAsync("connectivity.dispose");
        }

        #region [Section - AVN Repository]

        public Task DeleteAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(T EntityToDelete)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteByIdAsync(object Id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> GetAsync(QueryFilter<T> Filter)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetByIdAsync(object Id)
        {
            throw new NotImplementedException();
        }

        public Task<T> InsertAsync(T Entity)
        {
            throw new NotImplementedException();
        }

        public Task<T> UpdateAsync(T EntityToUpdate)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
