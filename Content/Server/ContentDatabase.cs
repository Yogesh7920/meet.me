using MongoDB.Driver;

namespace Content
{
    internal class ContentDatabase
    {
        private MongoClient mongoClient;

        public ContentDatabase()
        {
            mongoClient = new MongoClient("mongodb://127.0.0.1:27017");
        }
    }
}