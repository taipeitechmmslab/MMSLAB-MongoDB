using MongoDB.Bson; //匯入函式庫
namespace Lab6
{
    //定義library集合內的文件結構，並命名為LibraryDocument
    class LibraryDocument
    {
        public string _id { get; set; }
        public string book { get; set; }
        public float price { get; set; }
        public string[] authors { get; set; }
        public Borrower borrower { get; set; }
        public class Borrower
        {
            public string name { get; set; }
            public BsonDateTime timestamp { get; set; }
        }
    }
}