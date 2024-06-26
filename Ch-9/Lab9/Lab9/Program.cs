//匯入函式庫
using MongoDB.Bson;
using MongoDB.Driver;
using System;
namespace Lab9
{
    class Program
    {
        static void Main(string[] args)
        {
            //Step1:連接MongoDB伺服器
            var client = new MongoClient("mongodb://localhost:27017");
            //Step2:取得MongoDB中，名為ntut的資料庫及名為taiwan的資料庫
            var dbNtut = client.GetDatabase("ntut") as MongoDatabaseBase;
            var dbTaiwan = client.GetDatabase("taiwan") as MongoDatabaseBase;
            //Step3:建立集合變數
            var colCustomers = dbNtut.GetCollection<CustomersDocument>("customers");
            var colPeople = dbTaiwan.GetCollection<PeopleDocument>("people");
            //Step4:顯示執行範例的控制介面
            controlPanel();
            #region 控制介面
            void controlPanel()
            {
                Console.WriteLine("--------------------------------");
                Console.WriteLine("1.計算來自台北市各個分區之消費者的總人數與平均年齡");
                Console.WriteLine("2.計算107年全台灣的出生與死亡人數、結婚與離婚人數");
                Console.WriteLine("請輸入編號1~2，選擇要執行的範例");
                try
                {
                    var num = int.Parse(Console.ReadLine()); //取得輸入的編號
                    Console.Clear(); //清除Console顯示的內容
                                     //使用switch判斷編號，選擇要執行的範例
                    switch (num)
                    {
                        case 1:
                            countTaipeiPeopleAndAvgAge();
                            break;
                        case 2:
                            countTaiwanBirthDeathMarryDivorce();
                            break;
                        default:
                            Console.WriteLine("請輸入正確編號"); //輸入錯誤的提示
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("請輸入正確編號"); //輸入錯誤的提示
                }
                finally
                {
                    controlPanel(); //結束後再次執行controlPanel()方法
                }
            }
            #endregion
            #region 1.計算來自台北市各個分區之消費者的總人數與平均年齡
            void countTaipeiPeopleAndAvgAge()
            {
                Console.WriteLine("1.計算來自台北市各個分區之消費者的總人數與平均年齡\n");
                //建立管線階段
                var pipeline = new BsonDocument[]
                {
                    new BsonDocument
                    {
                        {
                            "$match", new BsonDocument
                            {
                                {"city", "台北市"}
                            }
                        }
                    },
                    new BsonDocument
                    {
                        {
                            "$group", new BsonDocument
                            {
                                {"_id", "$district"},
                                {
                                    "count", new BsonDocument
                                    {
                                        {"$sum", 1}
                                    }
                                },
                                {
                                    "avgAge", new BsonDocument
                                    {
                                        {"$avg", "$age"}
                                    }
                                }
                            }
                        }
                    }
                };
                //傳入管線階段，以執行Aggregation Pipeline並取得結果
                var results = colCustomers.Aggregate<BsonDocument>(pipeline).ToListAsync().Result;
                //將結果顯示於Console
                foreach (var result in results)
                {
                    Console.WriteLine($"分區: {result["_id"]}");
                    Console.WriteLine($"總人數: {result["count"]}");
                    Console.WriteLine($"平均年齡: {result["avgAge"]}\n");
                }
            }
            #endregion
            #region 2.計算107年全台灣的出生與死亡人數、結婚與離婚人數
            void countTaiwanBirthDeathMarryDivorce()
            {
                Console.WriteLine("2.計算107年全台灣的出生與死亡人數、結婚與離婚人數\n");
                //建立管線階段
                var pipeline = new BsonDocument[]
                {
                    new BsonDocument
                    {
                        {
                            "$group", new BsonDocument
                            {
                                //將_id欄位設為固定值result-9-2
                                {"_id", "result-9-2"},
                                {
                                    //加總輸入資料的birth_total欄位值，並以birth欄位儲存
                                    "birth", new BsonDocument
                                    {
                                        {"$sum", "$birth_total"}
                                    }
                                },
                                {
                                    //加總輸入資料的death_total欄位值，並以death欄位儲存
                                    "death", new BsonDocument
                                    {
                                        {"$sum", "$death_total"}
                                    }
                                },
                                {
                                    //加總輸入資料的marry_pair欄位值，並以marry欄位儲存
                                    "marry", new BsonDocument
                                    {
                                        {"$sum", "$marry_pair"}
                                    }
                                },
                                {
                                    //加總輸入資料的divorce_pair欄位值，並以divorce欄位儲存
                                    "divorce", new BsonDocument
                                    {
                                        {"$sum", "$divorce_pair"}
                                    }
                                }
                            }
                        }
                    }
                };
                //傳入管線階段，以執行Aggregation Pipeline並取得結果的第一筆資料(因為結果只有一筆)
                var result = colPeople.Aggregate<BsonDocument>(pipeline).ToListAsync().Result[0];
                //將出生人數、死亡人數、結婚對數、離婚對數顯示於Console
                Console.WriteLine($"出生人數:{result["birth"]}");
                Console.WriteLine($"死亡人數:{result["death"]}");
                Console.WriteLine($"結婚對數:{result["marry"]}");
                Console.WriteLine($"離婚對數:{result["divorce"]}");
            }
            #endregion
        }
    }
}