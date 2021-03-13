using MongoDB.Bson; //匯入函式庫
namespace Lab7
{
    //定義accounts集合內的文件結構，並命名為AccountsDocument
    class AccountsDocument
    {
        public string _id { get; set; }
        public string name { get; set; }
        public Currency[] currency { get; set; }
        public class Currency
        {
            public string type { get; set; }
            public double cash { get; set; }
            public BsonDateTime lastModified { get; set; }
        }
    }
}