//匯入函式庫
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;

namespace Lab6
{
    class Program
    {
        static void Main(string[] args)
        {
            //Step1:連接MongoDB伺服器
            var client = new MongoClient("mongodb://localhost:27017");
            //Step2:取得MongoDB中，名為ntut的資料庫及名為library的集合
            var db = client.GetDatabase("ntut") as MongoDatabaseBase;
            //Step3:使用db.GetCollection取得後續會使用到的集合
            var colLibrary = db.GetCollection<LibraryDocument>("library");
            //Step4:使用Builders建立後續會使用到的運算子
            var builderLibraryFilter = Builders<LibraryDocument>.Filter;
            var builderLibraryProjection = Builders<LibraryDocument>.Projection;
            var builderLibrarySort = Builders<LibraryDocument>.Sort;
            //Step5:顯示執行範例的控制介面
            controlPanel();
            #region 控制介面
            void controlPanel()
            {
                Console.WriteLine("--------------------------------");
                Console.WriteLine("1.查詢特定作者的所有書籍");
                Console.WriteLine("2.查詢王小明在特定日期借閱的書籍");
                Console.WriteLine("3.查詢未被借閱的書籍");
                Console.WriteLine("4.查詢特定價格以上的書籍");
                Console.WriteLine("5.查詢書名包含特定關鍵字的書籍，並以價格低至高排序");
                Console.WriteLine("\n請輸入編號1~5，選擇要執行的功能");
                try
                {
                    var num = int.Parse(Console.ReadLine()); //取得輸入的編號
                    Console.Clear(); //清除Console顯示的內容
                                     //使用switch判斷編號，選擇要執行的範例
                    switch (num)
                    {
                        case 1:
                            findAuthor();
                            break;
                        case 2:
                            findBorrow();
                            break;
                        case 3:
                            findNoBorrow();
                            break;
                        case 4:
                            findPrice();
                            break;
                        case 5:
                            findKeyword();
                            break;
                        default:
                            Console.WriteLine("\n請輸入正確內容"); //輸入錯誤的提示
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("\n請輸入正確內容"); //輸入錯誤的提示
                }
                finally
                {
                    controlPanel(); //結束後再次執行controlPanel()方法
                }
            }
            #endregion
            #region 1.查詢特定作者的所有書籍
            void findAuthor()
            {
                Console.WriteLine("1.查詢特定作者的所有書籍\n");
                Console.WriteLine("請輸入作者名稱");
                //取得輸入的作者名稱
                var author = Console.ReadLine();
                //建立查詢條件為authors欄位包含特定作者
                var filter = builderLibraryFilter.AnyIn(e => e.authors, new string[] { author });
                //進行查詢並取得結果
                var result = colLibrary.Find(filter).ToListAsync().Result;
                //判斷有無結果
                if (result.Count == 0)
                {
                    Console.WriteLine("\n查無資料");
                }
                else
                {
                    Console.WriteLine("\n查詢結果");
                    //使用foreach遍歷查詢結果，將書名顯示於Console
                    foreach (LibraryDocument doc in result)
                    {
                        Console.WriteLine(doc.book);
                    }
                }
            }
            #endregion
            #region 2.查詢王小明在特定日期借閱的書籍
            void findBorrow()
            {
                Console.WriteLine("2.查詢王小明在特定日期借閱的書籍\n");
                Console.WriteLine("請輸入月份");
                //取得輸入的月份
                var month = int.Parse(Console.ReadLine());
                Console.WriteLine("請輸入日期");
                //取得輸入的日期
                var day = int.Parse(Console.ReadLine());
                //建立查詢條件為borrower.name欄位為王小明
                var nameFilter = builderLibraryFilter.Eq(e => e.borrower.name, "王小明");
                //建立查詢條件為borrower.timestamp欄位大於等於2015年特定日期的00:00:00
                var timeUpperFilter = builderLibraryFilter.Gte(e => e.borrower.timestamp,
                new DateTime(2015, month, day, 0, 0, 0));
                //建立查詢條件為borrower.timestamp欄位小於等於2015年特定日期的23:59:59
                var timeLowerFilter = builderLibraryFilter.Lte(e => e.borrower.timestamp,
                new DateTime(2015, month, day, 23, 59, 59));
                //建立查詢條件為符合上述所有條件
                var filter = builderLibraryFilter.And(nameFilter, timeUpperFilter, timeLowerFilter);
                //進行查詢並取得結果
                var result = colLibrary.Find(filter).ToListAsync().Result;
                //判斷有無結果
                if (result.Count == 0)
                {
                    Console.WriteLine("\n查無資料");
                }
                else
                {
                    Console.WriteLine("\n查詢結果");
                    //使用foreach遍歷查詢結果，將王小明在特定日期借閱的書籍顯示於Console
                    foreach (LibraryDocument doc in result)
                    {
                        Console.WriteLine($"王小明借了{doc.book}");
                    }
                }
            }
            #endregion
            #region 3.查詢未被借閱的書籍
            void findNoBorrow()
            {
                Console.WriteLine("3.查詢未被借閱的書籍\n");
                //建立查詢條件為borrower欄位不存在
                var filter = builderLibraryFilter.Exists(e => e.borrower, false);
                //進行查詢並取得結果
                var result = colLibrary.Find(filter).ToListAsync().Result;
                Console.WriteLine("查詢結果");
                //使用foreach遍歷查詢結果，將書名顯示於Console
                foreach (LibraryDocument doc in result)
                {
                    Console.WriteLine(doc.book);
                }
            }
            #endregion
            #region 4.查詢特定價格以上的書籍
            void findPrice()
            {
                Console.WriteLine("4.查詢特定價格以上的書籍\n");
                Console.WriteLine("請輸入價格");
                //取得輸入的價格
                var price = int.Parse(Console.ReadLine());
                //建立查詢條件為price欄位大於等於輸入的價格
                var filter = builderLibraryFilter.Where(e => e.price >= price);
                //進行查詢並取得結果
                var result = colLibrary.Find(filter).ToListAsync().Result;
                //判斷有無結果
                if (result.Count == 0)
                {
                    Console.WriteLine("\n查無資料");
                }
                else
                {
                    Console.WriteLine("\n查詢結果");
                    //使用foreach遍歷查詢結果，將書名顯示於Console
                    foreach (LibraryDocument doc in result)
                    {
                        Console.WriteLine(doc.book);
                    }
                }
            }
            #endregion
            #region 5.查詢書名包含特定關鍵字的書籍，並以價格低至高排序
            void findKeyword()
            {
                Console.WriteLine("5.查詢書名包含特定關鍵字的書籍，並以價格低至高排序\n");
                Console.WriteLine("請輸入關鍵字");
                //取得輸入的關鍵字
                var keyword = Console.ReadLine();
                //建立正規表達式的格式為包含特定關鍵字且不區分大小寫
                var pattern = new BsonRegularExpression(keyword, "i");
                //建立查詢條件為book欄位符合正規表達式的格式
                var filter = builderLibraryFilter.Regex(e => e.book, pattern);
                //建立映射條件包含book、price欄位
                var projection = builderLibraryProjection.Include(e => e.book).Include(e => e.price);
                //建立排序條件為price欄位遞增
                var sort = builderLibrarySort.Ascending(e => e.price);
                //進行查詢、映射與排序，並取得結果
                var result = colLibrary.Find(filter).Project(projection).Sort(sort).ToListAsync().Result;
                //判斷有無結果
                if (result.Count == 0)
                {
                    Console.WriteLine("\n查無資料");
                }
                else
                {
                    Console.WriteLine("\n查詢結果");
                    //使用foreach遍歷查詢、映射與排序結果，將書籍資訊顯示於Console
                    foreach (BsonDocument bsonDoc in result)
                    {
                        //因為映射後為BsonDocument類型，需將其反序列化為LibraryDocument類型
                        var doc = BsonSerializer.Deserialize<LibraryDocument>(bsonDoc);
                        Console.WriteLine($"書名: {doc.book}, 價格: {doc.price}");
                    }
                }
            }
            #endregion
        }
    }
}