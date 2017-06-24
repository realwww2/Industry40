//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using I4.BaseCore;
//using I4.LocalConfig;
//using MongoDB.Driver;
//using MongoDB.Bson;
//using System.Threading;

//namespace I4.LocalCache
//{
//    public sealed class MongoDBCacheImp : ILocalCache
//    {
//        private bool _open;
//        private MongoClient _client;
//        private IMongoDatabase  _db;
//        private IMongoCollection<CaptureItem> _collection;

//        private DateTime _workingTime;
//        private const int MAXMINUTES = 100;//一个集合里最多cache 100分鐘的条数据
//        private const int MAXCOUNT = 10000;//一个集合里最多cache 1000条数据
//        private long _count;
//        //because the MongoDatabase.GetCollectionNames always return null, so we can't get the notworking collection by name
//        //

//        private string GetNewCollectionName(string prefix)
//        {
//            return string.Format("{0}_{1}", prefix,DateTime.Now.Ticks.ToString());
//        }

//        private IMongoCollection<CaptureItem> _currentCollection
//        {
//            get
//            {
  
//                TimeSpan span = DateTime.Now - _workingTime;
//                if (span.TotalMinutes > MAXMINUTES || _count > MAXCOUNT)
//                {
//                    _workingTime = DateTime.Now;
//                    lock (this) // the different write thead may access this code, so lock for this
//                    {
//                        _db.RenameCollectionAsync(_collection.CollectionNamespace.CollectionName, GetNewCollectionName("notworking"));
//                        _collection = _db.GetCollection<CaptureItem>(GetNewCollectionName("working"));
//                        _count = _collection.CountAsync(new BsonDocument()).Result;
//                    }
//                }
//                return _collection;

//            }
//        }
//        private IMongoCollection<CaptureItem> GetOneCollecction(IMongoDatabase db,string collectionStr)
//        {
//            var c = db.ListCollectionsAsync().Result;
//            var list = c.ToListAsync<BsonDocument>().Result;
//            foreach (BsonDocument b in list)
//            {
//                string collectionName = "dd";
//                if (collectionName.Contains(collectionStr))
//                {
//                    return db.GetCollection<CaptureItem>(collectionName);
//                }
//            }
//            return null;
//        }

//        public void Init(string configFullFile)
//        {
//            try
//            {
//                Config config = new Config(configFullFile);
//                _client = new MongoClient(config.GetKeyValue("connect"));
//                _open = true;
//                _db = _client.GetDatabase(config.GetKeyValue("database"));
//                _collection = GetOneCollecction(_db, "working");
//                if (_collection == null) _collection = _db.GetCollection<CaptureItem>(GetNewCollectionName("working"));
//                _count = _collection.CountAsync(new BsonDocument()).Result;
//                _workingTime = DateTime.Now;
//            }
//            catch(Exception ex)
//            {
//                throw new LocalCacheException(ex.Message, ex);
//            }
//        }

//        //not thread sync problem for write/read, because we always write working collection, and read notworking collection
//        public void Write(CaptureItem[] items)
//        {            
//            foreach (CaptureItem item in items)
//            {
//                _currentCollection.InsertOneAsync(item);
//                _count++;
//            }

//        }
//        //read one collection, return collecctionName
//        public IList<CaptureItem> ReadOneBatch(ref string batchName)
//        {
//            //check if there is notworking collection
//            while(true)
//            {
//                Thread.Sleep(1000);//check collection each 1 s
//                IMongoCollection notworkingCollection = GetOneCollecction(_db, "notworking");
//                if(notworkingCollection!=null)
//                {
//                    List<CaptureItem> cs = notworkingCollection.FindAllAs<CaptureItem>().ToList<CaptureItem>();
//                    batchName = notworkingCollection.Name;
//                    return cs;
//                }
//                AppGlobal.Instance.Logger.LogDebug("All notworking Mongo collection is upload, wait for new notworking collection...");
//            }
            

//        }
//        //delete one collection
//        public void Delete(string batchName)
//        {
//            _db.DropCollection(batchName);
//        }
//        public void Close()
//        {
//            if (_open) _client = null;
//        }
//    }
//}