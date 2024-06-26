using MongoDB.Bson; //匯入函式庫

namespace Lab7
{
    //定義schools集合內的文件結構，並命名為SchoolsDocument
    class SchoolsDocument
    {
        public ObjectId _id { get; set; }
        public int schoolYear { get; set; }
        public string schoolCode { get; set; }
        public string schoolName { get; set; }
        public string educationLevel { get; set; }
        public int campusArea { get; set; }
    }
}