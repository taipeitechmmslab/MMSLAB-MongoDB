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
                //建立字串類型的Map函式(@表示讓字串內的跳脫字元失效)
                string map = @"
                    function() {
                        emit(this.district, { count: 1, age: this.age });
                    }
                ";
                //建立字串類型的Reduce函式(@表示讓字串內的跳脫字元失效)
                string reduce = @"
                    function(key, values) {
                        var reduced = {count:0, age:0};
                        for(var idx=0 ; idx<values.length ; idx++)
                        {
                            var val = values[idx];
                            reduced.age += val.age;
                            reduced.count+= val.count;
                        }
                        return reduced;
                    }
                ";
                //建立字串類型的Finalize函式(@表示讓字串內的跳脫字元失效)
                string finalize = @"
                    function(key, reduced) {
                        reduced.avgAge = reduced.age / reduced.count;
                        return reduced;
                    }
                ";
                //建立查詢條件為city欄位為台北市
                var builderCustomersFilter = Builders<CustomersDocument>.Filter;
                var filter = builderCustomersFilter.Eq(e => e.city, "台北市");
                //建立Map-Reduce的其他選項，包含篩選條件、Finalize函式、輸出方式
                var options = new MapReduceOptions<CustomersDocument, BsonDocument>
                {
                    Filter = filter,
                    Finalize = finalize,
                    OutputOptions = MapReduceOutputOptions.Inline
                };
                //傳入Map函式、Reduce函式及其他選項，以執行Map-Reduce並取得結果
                var result = colCustomers.MapReduce(map, reduce, options).ToListAsync().Result;
                //使用foreach遍歷Map-Reduce結果，並轉為Json格式顯示於Console
                foreach (var data in result)
                {
                    Console.WriteLine(data.ToJson());
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