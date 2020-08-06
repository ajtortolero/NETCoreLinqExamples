using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetCoreLinqExamples
{
    /// <summary>
    /// 
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            var lstSales = new List<Sales>()
            {
                new Sales { StationID = 10001, ProducID= 1, Year= 2019, Month = 1, Day = 1, SaleTotal = 10 },
                new Sales { StationID = 10001, ProducID= 1, Year= 2019, Month = 12, Day = 1, SaleTotal = 10 },
                new Sales { StationID = 10001, ProducID= 1, Year= 2020, Month = 1, Day = 1, SaleTotal = 10 },
                new Sales { StationID = 10001, ProducID= 1, Year= 2020, Month = 1, Day = 1, SaleTotal = 10 },
                new Sales { StationID = 10001, ProducID= 1, Year= 2020, Month = 1, Day = 1, SaleTotal = 10 },

                new Sales { StationID = 10001, ProducID= 1, Year= 2020, Month = 2, Day = 1, SaleTotal = 15 },
                new Sales { StationID = 10001, ProducID= 1, Year= 2020, Month = 2, Day = 1, SaleTotal = 10 },
                new Sales { StationID = 10001, ProducID= 1, Year= 2020, Month = 2, Day = 1, SaleTotal = 15 },

                new Sales { StationID = 10001, ProducID= 1, Year= 2020, Month = 3, Day = 1, SaleTotal = 10 },
                new Sales { StationID = 10001, ProducID= 1, Year= 2020, Month = 3, Day = 1, SaleTotal = 100 },
                new Sales { StationID = 10001, ProducID= 1, Year= 2020, Month = 3, Day = 1, SaleTotal = 15 },

                new Sales { StationID = 10002, ProducID= 1, Year= 2020, Month = 1, Day = 1, SaleTotal = 25 },
                new Sales { StationID = 10002, ProducID= 1, Year= 2020, Month = 1, Day = 1, SaleTotal = 25 },
                new Sales { StationID = 10002, ProducID= 1, Year= 2020, Month = 1, Day = 1, SaleTotal = 25 },
                new Sales { StationID = 10002, ProducID= 1, Year= 2020, Month = 1, Day = 1, SaleTotal = 25 },
                new Sales { StationID = 10002, ProducID= 1, Year= 2020, Month = 1, Day = 1, SaleTotal = 30 },

                new Sales { StationID = 10003, ProducID= 1, Year= 2020, Month = 2, Day = 1, SaleTotal = 20 },
                new Sales { StationID = 10003, ProducID= 1, Year= 2020, Month = 2, Day = 1, SaleTotal = 20 },
                new Sales { StationID = 10003, ProducID= 1, Year= 2020, Month = 2, Day = 1, SaleTotal = 20 },

                new Sales { StationID = 10004, ProducID= 1, Year= 2020, Month = 2, Day = 1, SaleTotal = 30 },
                new Sales { StationID = 10004, ProducID= 1, Year= 2020, Month = 2, Day = 1, SaleTotal = 30 },
                new Sales { StationID = 10004, ProducID= 1, Year= 2020, Month = 2, Day = 1, SaleTotal = 30 },
            };

            var lstSaleMonth = lstSales.GroupBy(Group => new {
                Group.Year
            }).Select(Select => new {
                Result = new
                {
                    Select.Key.Year,
                    Data = Enumerable.Range(1, 12).GroupBy(GroupMonth => new {
                        Select.Key.Year,
                        Month = GroupMonth
                    }).Select(SelectMonth => new {
                        Select.Key.Year,
                        SelectMonth.Key.Month,
                        MonthShortName = new DateTime(Select.Key.Year, SelectMonth.Key.Month, 1).ToString("MMM").Substring(0, 3).ToUpper(),
                        MonthName = new DateTime(Select.Key.Year, SelectMonth.Key.Month, 1).ToString("MMMM").ToUpper(),
                    }).ToList()
                }
            }).SelectMany(SelectMany => SelectMany.Result.Data);

            var lstSaleResultSpecific = lstSales.GroupBy(Group => new {
                Group.StationID
            }).Select(Select => new {
                Select.Key,
                Result = Select.GroupBy(GroupMonth => new {
                    SaleYear = GroupMonth.Year,
                    SaleMonth = GroupMonth.Month,
                }).Select(SelectMonth => new {
                    StationID = Select.Key,
                    SelectMonth.Key.SaleYear,
                    SelectMonth.Key.SaleMonth,
                    Total = Select.Where(Model => Model.Year == SelectMonth.Key.SaleYear && Model.Month == SelectMonth.Key.SaleMonth).Sum(Sum => Sum.SaleTotal)
                }).ToList()
            }).SelectMany(SelectMany => SelectMany.Result);

            var lstResultExplicit = lstSales.GroupBy(Group => new
            {
                Group.StationID
            }).Select(SelectStation => new
            {
                Result = lstSaleMonth.GroupBy(GroupInner => new
                {
                    GroupInner.Month,
                    GroupInner.MonthShortName,
                    GroupInner.MonthName,
                    GroupInner.Year
                }).Select(SelectMonth => new
                {
                    SelectStation.Key.StationID,
                    SelectMonth.Key.Year,
                    SelectMonth.Key.Month,
                    SelectMonth.Key.MonthShortName,
                    SelectMonth.Key.MonthName,
                    Total = SelectStation.Where(Model => Model.StationID == SelectStation.Key.StationID && (Model.Month == SelectMonth.Key.Month && (Model.Year == SelectMonth.Key.Year))).Sum(Sum => Sum.SaleTotal),
                })
            }).SelectMany(SelectMany => SelectMany.Result);

            var varSaleMonth = JsonConvert.SerializeObject(lstSaleMonth);
            var varSaleExplicit = JsonConvert.SerializeObject(lstResultExplicit);
            var varSaleSpecific = JsonConvert.SerializeObject(lstSaleResultSpecific);

            foreach (var iteSalemonth in lstSaleMonth)
            {
                Console.WriteLine("Year: {0}, Month: {1}", iteSalemonth.Year, iteSalemonth.Month);
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("---------------------------------------------------- Explicit Sales in Year and Month ----------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine();

            foreach (var iteSaleExplicit in lstResultExplicit)
            {
                Console.WriteLine("Year: {0}, Month: {1}, MonthName: {2}, StationID: {3}, TotalSale: {4} ", iteSaleExplicit.Year, iteSaleExplicit.Month, iteSaleExplicit.MonthName, iteSaleExplicit.StationID, iteSaleExplicit.Total);
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("---------------------------------------------------- Specific Sales in Year and Month ----------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine();

            foreach (var iteSaleSpecific in lstSaleResultSpecific)
            {
                Console.WriteLine("Year: {0}, Month: {1}, StationID: {2}, TotalSale: {3} ", iteSaleSpecific.SaleYear, iteSaleSpecific.SaleMonth, iteSaleSpecific.StationID, iteSaleSpecific.Total);
            }

            Console.ReadKey();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Sales
    {
        public int StationID { get; set; }
        public int ProducID { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public decimal SaleTotal { get; set; }
    }
}
