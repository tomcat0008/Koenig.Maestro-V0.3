using Koenig.Maestro.Entity;
using Koenig.Maestro.Entity.Enums;
using Koenig.Maestro.Operation.Data;
using Koenig.Maestro.Operation.Framework;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koenig.Maestro.Operation.Reporting.ReportRepository
{
    public class OrderSummaryReport : ExcelReportBase
    {
        
        public OrderSummaryReport(TransactionContext context) : base(context) { }

        List<DataRow> baseData;
        List<string> reportGroups;
        Dictionary<string, int> grpCol = new Dictionary<string, int>();
        int currentRow = 2;
        int productColumn = 3;
        int totalColumn = 1;
        int groupStartCol = 5;
        public override void LoadData()
        {
            MaestroReportDefinition reportDef = (MaestroReportDefinition)context.Bag["REPORT_DEF"];

            DateTime beginDate = (DateTime)context.Bag[MessageDataExtensionKeys.BEGIN_DATE];
            DateTime endDate = (DateTime)context.Bag[MessageDataExtensionKeys.END_DATE];

            SpCall call = new SpCall(reportDef.ProcedureName);

            call.SetDateTime("@BEGIN_DATE", beginDate);
            call.SetDateTime("@END_DATE", endDate);
            reportData = context.Database.ExecuteDataSet(call);

            if (reportData.Tables[0].Rows.Count == 0)
                throw new Exception(string.Format("No data can be found between {0} and {1}", beginDate, endDate));

            baseData = reportData.Tables[0].AsEnumerable().ToList();
        }

        protected override void RenderWithoutTemplate()
        {
            using (var p = new ExcelPackage())
            {
                workbook = p.Workbook;
                RenderTotalSheet();
                RenderReportGroupSheets();

                string fileName = string.Format(reportDefinition.FileName, DateTime.Today.ToString("yyyyMMdd"));
                string path = Path.Combine(MaestroApplication.Instance.ReportSavePath, fileName);
                //string path = Path.Combine(@"D:\TEMP\REPORT", string.Format(reportDefinition.FileName, DateTime.Today.ToString("yyyyMMdd")));
                int fileCnt = 1;
                while (File.Exists(path))
                {
                    fileName = string.Format(reportDefinition.FileName, DateTime.Today.ToString("yyyyMMdd") + "_" + fileCnt.ToString("000"));
                    fileCnt++;
                    path = Path.Combine(MaestroApplication.Instance.ReportSavePath, fileName);
                }


                if (MaestroApplication.Instance.SaveReportsOnServer)
                {
                    p.SaveAs(new FileInfo(path));
                }
                
                
                using (MemoryStream fileStream = new MemoryStream())
                {
                    p.SaveAs(fileStream);
                    byte[] data = fileStream.ToArray();
                    using (var compressedStream = new MemoryStream())
                    {
                        using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
                        {
                            
                            zipStream.Write(data, 0, data.Length);

                        }

                        
                        context.TransactionObject = Convert.ToBase64String(compressedStream.ToArray());
                        //File.WriteAllBytes(@"D:\TEMP\REPORT\report.xlsx.zip", compressedStream.ToArray());
                    }

                }

                

                
                //To set values in the spreadsheet use the Cells indexer.
                //ws.Cells["A1"].Value = "This is cell A1";
                //Save the new workbook. We haven't specified the filename so use the Save as method.
                //p.SaveAs(new FileInfo(@"c:\workbooks\myworkbook.xlsx"));
            }
        }

        void RenderReportGroupSheets()
        {

            reportGroups.ForEach(delegate (string repGrp)
            {
                RenderReportGroup(repGrp);
            });

            
        }

        int pshStartCol = 2, pshDateMergeWidth = 2;
        int pshStartRow = 1;

        void RenderReportGroup(string repGroup)
        {
            ExcelWorksheet sheet = workbook.Worksheets.Add(repGroup); //add report group sheet

            List<DataRow> grpData = baseData.Where(r => r.Field<string>("REPORT_GROUP").Equals(repGroup)).ToList();

            //insert date
            sheet.Cells[pshStartRow, pshStartCol, pshStartRow + 1, (pshStartCol + pshDateMergeWidth) ].Merge = true;
            sheet.Cells[pshStartRow, pshStartCol, pshStartRow + 1, (pshStartCol + pshDateMergeWidth)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            sheet.Cells[pshStartRow, pshStartCol].Value = ((DateTime)context.Bag[MessageDataExtensionKeys.BEGIN_DATE]).ToString("dddd, dd MMMM yyyy") + " - " + ((DateTime)context.Bag[MessageDataExtensionKeys.END_DATE]).ToString("dddd, dd MMMM yyyy");
            sheet.Cells[pshStartRow, pshStartCol].Style.Font.Bold = true;

            int companyCol = pshStartCol + pshDateMergeWidth + 1;
            

            //insert customer & companies
            List<string> customers = grpData.Select(r => r.Field<string>("CUSTOMER_NAME")).Distinct().ToList();
            Dictionary<string, int> companyAddress = CreateCompanyRegister(grpData, customers, sheet, companyCol);

            //insert products
            List<string> productGroups = grpData.Select(r => r.Field<string>("PRODUCT_GROUP_NAME")).Distinct().OrderBy(p => p).ToList();

            int rowNow = pshStartRow + 2;

            foreach (string pg in productGroups)
                rowNow = InsertProductGroupsForReportGroup(sheet, pg, grpData, companyAddress, rowNow);


            //insert sum
            int sumColumn = companyAddress.Values.Max() + 1;
            string sumFormula = "=SUM({0}:{1})";
            sheet.Cells[pshStartRow + 1, sumColumn].Value = "Total";
            
            sheet.Cells[pshStartRow + 1, sumColumn].Style.Font.Bold = true;
            for (int sumRow = (pshStartRow + 2); sumRow<rowNow; sumRow++ )
            {
                sheet.Cells[sumRow, sumColumn].Formula = string.Format(sumFormula, sheet.Cells[sumRow, pshStartCol+3].Address, sheet.Cells[sumRow, sumColumn-1].Address);
            }




            sheet.Cells[pshStartRow, pshStartCol, rowNow - 1, sumColumn].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Double);
            sheet.Cells[pshStartRow, pshStartCol, rowNow - 1, pshStartCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            sheet.Cells[pshStartRow, pshStartCol + 2, rowNow - 1, pshStartCol + 2].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            sheet.Cells[pshStartRow, sumColumn, rowNow - 1, sumColumn].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

            sheet.Cells[pshStartRow + 2, pshStartCol, rowNow - 1, pshStartCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            sheet.Cells[pshStartRow + 2, pshStartCol, rowNow - 1, pshStartCol].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(155, 194, 230));

            sheet.Cells[pshStartRow + 2, pshStartCol+1, rowNow - 1, pshStartCol+1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            sheet.Cells[pshStartRow + 2, pshStartCol+1, rowNow - 1, pshStartCol+1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(189, 215, 238));

            sheet.Cells[pshStartRow + 2, pshStartCol + 2, rowNow - 1, pshStartCol+2].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            sheet.Cells[pshStartRow + 2, pshStartCol + 2, rowNow - 1, pshStartCol+2].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(221, 235, 247));

            sheet.Cells[pshStartRow, companyAddress.Values.Min(), pshStartRow + 1, sumColumn].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

            sheet.Cells[pshStartRow, companyAddress.Values.Min(), pshStartRow+1, companyAddress.Values.Max()].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            sheet.Cells[pshStartRow, companyAddress.Values.Min(), pshStartRow+1, companyAddress.Values.Max()].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(242, 242, 242));

            sheet.Cells[pshStartRow, sumColumn, rowNow-1, sumColumn].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            sheet.Cells[pshStartRow, sumColumn, rowNow-1, sumColumn].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(217, 217, 217));
            sheet.Cells[pshStartRow + 2, sumColumn, rowNow-1, sumColumn].Style.Font.Color.SetColor(Color.Red);

            for (int i = pshStartCol; i < sumColumn; i++)
                sheet.Column(i).AutoFit();

            sheet.Column(3).Width = 30;

        }


        int InsertProductData(ExcelWorksheet sheet, List<DataRow> labelRows, Dictionary<string, int> companyAddress,  string product, string label, int rowNr )
        {
            int rowNow = rowNr;
            string companyColumnName = "QB_COMPANY";
            if (!string.IsNullOrWhiteSpace(labelRows.First().Field<string>("ADDRESS_CODE")))
                companyColumnName = "ADDRESS_CODE";


            if (!label.Equals(product))
            {
                sheet.Cells[rowNow, pshStartCol + 2].Value = label;

                foreach (KeyValuePair<string, int> kvp in companyAddress)
                {
                    int quantity = labelRows.Where(r=>r.Field<string>(companyColumnName).Equals(kvp.Key)).Select(r => r.Field<int>("QUANTITY")).Sum();
                    sheet.Cells[rowNow, kvp.Value].Value = quantity;
                }

                

                rowNow++;
            }
            else
            {

                List<string> units = labelRows.Select(r => r.Field<string>("UNIT_NAME")).Distinct().OrderBy(u => u).ToList();

                foreach (string unit in units)
                {

                    sheet.Cells[rowNow, pshStartCol + 2].Value = unit.Equals(MaestroApplication.Instance.UNKNOWN_ITEM_NAME) ? string.Empty : unit ;

                    foreach (KeyValuePair<string, int> kvp in companyAddress)
                    {
                        int quantity = labelRows.Where(r => r.Field<string>(companyColumnName).Equals(kvp.Key) && r.Field<string>("UNIT_NAME").Equals(unit)).Select(r => r.Field<int>("QUANTITY")).Sum();
                        sheet.Cells[rowNow, kvp.Value].Value = quantity;
                    }

                    rowNow++;
                }

            }

            

            return rowNow;
        }


        int InsertProductGroupsForReportGroup(ExcelWorksheet sheet, string pg, List<DataRow> grpData, Dictionary<string, int> companyAddress, int rowNow)
        {
            int pgStartRow = rowNow;

            List<DataRow> pgRows = grpData.Where(r => r.Field<string>("PRODUCT_GROUP_NAME").Equals(pg)).ToList();
            List<string> products = pgRows.Select(r => r.Field<string>("PRODUCT_NAME")).Distinct().OrderBy(p => p).ToList();

            foreach (string product in products)
            {
                int prStartRow = rowNow;
                List<DataRow> productRows = pgRows.Where(r => r.Field<string>("PRODUCT_NAME").Equals(product)).ToList();
                List<string> labels = productRows.Select(r => r.Field<string>("REPORT_LABEL")).Distinct().OrderBy(l => l).ToList();

                int prodRowStart = rowNow;
                sheet.Cells[rowNow, pshStartCol + 1].Value = product;

                foreach (string label in labels)
                {

                    List<DataRow> labelRows = productRows.Where(r => r.Field<string>("REPORT_LABEL").Equals(label)).ToList();
                    rowNow = InsertProductData(sheet, labelRows, companyAddress, product, label, rowNow);
                }

                sheet.Cells[prodRowStart, pshStartCol + 1, rowNow - 1, pshStartCol + 1].Merge = true;

                sheet.Cells[prodRowStart, pshStartCol + 1, rowNow - 1, pshStartCol + 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;                sheet.Cells[rowNow - 1, pshStartCol + 1, rowNow - 1, companyAddress.Values.Max() + 1].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

            }


            sheet.Cells[pgStartRow, pshStartCol, rowNow - 1, pshStartCol].Merge = true;
            if (rowNow - 1 > pgStartRow)
            {
                sheet.Cells[pgStartRow, pshStartCol, rowNow - 1, pshStartCol].Style.TextRotation = 180;
                sheet.Cells[pgStartRow, pshStartCol, rowNow - 1, pshStartCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            }
            sheet.Cells[pgStartRow, pshStartCol].Value = pg;

            sheet.Cells[pgStartRow, pshStartCol, rowNow - 1, companyAddress.Values.Max()+1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick);

            return rowNow;
        }


        Dictionary<string, int> CreateCompanyRegister(List<DataRow> grpData, List<string> customers, ExcelWorksheet sheet, int companyCol)
        {
            Dictionary<string, int> companyAddress = new Dictionary<string, int>();

            foreach (string customer in customers)
            {

                List<DataRow> customerRows = grpData.Where(r => r.Field<string>("CUSTOMER_NAME").Equals(customer)).ToList();
                string companyColumnName = customerRows.Select(r => r.Field<string>("ADDRESS_CODE")).Where(s => !string.IsNullOrWhiteSpace(s)).Count() > 0 ? "ADDRESS_CODE" : "QB_COMPANY";


                //get companies of customer
                List<string> companies = customerRows.Select(r => r.Field<string>(companyColumnName)).Distinct().OrderBy(c => c).ToList();

                if (companies.Count > 1)
                    sheet.Cells[pshStartRow, companyCol, pshStartRow, companyCol + companies.Count - 1].Merge = true;

                sheet.Cells[pshStartRow, companyCol].Value = customer;

                companies.ForEach(delegate (string company)
                {
                    sheet.Cells[pshStartRow+1, companyCol].Value = company;
                    companyAddress.Add(company, companyCol);
                    companyCol++;
                });

            }

            return companyAddress;
        }

        void RenderTotalSheet()
        {
            

            ExcelWorksheet ws = workbook.Worksheets.Add("TOTALS");
            int startRow = currentRow;
            
            //insert report groups for customers

            reportGroups = baseData.Select(r => r.Field<string>("REPORT_GROUP")).Distinct().ToList();
            int col = groupStartCol;
            reportGroups.ForEach(delegate(string repGrp)
            {
                ws.Cells[currentRow, col].Value = repGrp;
                grpCol.Add(repGrp, col);
                col++;
            });

            totalColumn = grpCol.Values.Max()+1;
            ws.Cells[currentRow, totalColumn].Value = "TOTAL";
            ws.Cells[currentRow, productColumn - 1, currentRow, totalColumn].Style.Font.Bold = true;

            currentRow++;
            //render product group
            baseData.Select(r => r.Field<string>("PRODUCT_GROUP_NAME")).Distinct().ToList().ForEach(pg => RenderProductGroup(pg, ws));


            ws.HeaderFooter.FirstFooter.LeftAlignedText = string.Format("Printed on {0}", DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss"));
            
            FinalStyling(ws, startRow);




        }

        void FinalStyling(ExcelWorksheet ws, int startRow)
        {

            //insert date
            string caption = ((DateTime)context.Bag[MessageDataExtensionKeys.BEGIN_DATE]).ToString("dddd, dd MMMM yyyy") + " - " + ((DateTime)context.Bag[MessageDataExtensionKeys.END_DATE]).ToString("dddd, dd MMMM yyyy");
            //string caption = string.Format("Order with Delivery date {0}", ((DateTime)context.Bag[MessageDataExtensionKeys.BEGIN_DATE]).ToString("dddd, dd MMMM yyyy"));
            ws.Cells[1, 2].Value = caption;
            ws.Cells[1, 2].Style.Font.Bold = true;

            for (int i = 3; i < totalColumn; i++)
                ws.Column(i).AutoFit();

            ws.Cells[startRow, totalColumn, currentRow - 1, totalColumn].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

            //ws.Cells[startRow+1, productColumn - 1, currentRow - 1, productColumn - 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

            ws.Cells[startRow+1, productColumn - 1, currentRow - 1, productColumn].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            ws.Cells[startRow, productColumn +1 , currentRow - 1, productColumn+1].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;


            
            ws.Cells[startRow, productColumn - 1, currentRow - 1, totalColumn].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Double);

            ws.Cells[startRow + 1, productColumn - 1, currentRow - 1, productColumn - 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ws.Cells[startRow+1, productColumn - 1, currentRow - 1, productColumn - 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(155, 194, 230));

            ws.Cells[startRow + 1, productColumn, currentRow - 1, productColumn].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ws.Cells[startRow+1, productColumn , currentRow - 1, productColumn].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(189, 215, 238));

            ws.Cells[startRow + 1, productColumn+1, currentRow - 1, productColumn+1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ws.Cells[startRow + 1, productColumn+1, currentRow - 1, productColumn+1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(221, 235, 247));


            ws.Cells[startRow, grpCol.Values.Min(), startRow, grpCol.Values.Max()].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ws.Cells[startRow, grpCol.Values.Min(), startRow, grpCol.Values.Max()].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(242, 242, 242));

            ws.Cells[startRow, totalColumn, currentRow - 1, totalColumn].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ws.Cells[startRow, totalColumn, currentRow-1, totalColumn].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(217, 217, 217));
            ws.Cells[startRow + 1, totalColumn, currentRow - 1, totalColumn].Style.Font.Color.SetColor(Color.Red);

        }

        void RenderProductGroup(string productGroupName, ExcelWorksheet ws)
        {
            List<DataRow> pgData = baseData.Where(r => r.Field<string>("PRODUCT_GROUP_NAME").Equals(productGroupName)).ToList();

            List<string> products = pgData.Select(r => r.Field<string>("PRODUCT_NAME")).Distinct().ToList();
            int groupStart = currentRow;
            products.ForEach(delegate(string productName) 
            {
                List<DataRow> productData = pgData.Where(r => r.Field<string>("PRODUCT_NAME").Equals(productName)).ToList();
                RenderProduct(productName, productData);
            });

            if(currentRow - 1 > groupStart)
            {
                ws.Cells[groupStart, productColumn - 1, currentRow - 1, productColumn - 1].Merge = true;
                ws.Cells[groupStart, productColumn - 1].Style.TextRotation = 180;
                ws.Cells[groupStart, productColumn - 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            }

            ws.Cells[groupStart, productColumn - 1, currentRow - 1, totalColumn].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick);
            ws.Cells[groupStart, productColumn - 1].Value = productGroupName;
            
            


        }

        void RenderProduct(string productName, List<DataRow> productRows)
        {
            //if report label == productName, use pname in the first abd unit in the second column
            //else use productName and reportLabel
            string reportLabel = productRows.First().Field<string>("REPORT_LABEL");

            if (reportLabel.Equals(productName))
                RenderProduct(reportLabel, "UNIT_NAME", productRows);
            else
                RenderProduct(productName, "REPORT_LABEL", productRows);



        }

        void RenderProduct(string firstColumnValue, string secondColumnName,  List<DataRow> productRows)
        {
            ExcelWorksheet ws = workbook.Worksheets["TOTALS"];
            List<string> secondLevelValues = productRows.Select(r => r.Field<string>(secondColumnName)).Distinct().ToList();
            int startRow = currentRow;
            //only 1 unit or reportLabel ? means only 1 line
            if (secondLevelValues.Count == 1)
            {
                ws.Cells[currentRow, productColumn].Value = firstColumnValue;
                ws.Cells[currentRow, productColumn+1].Value = secondLevelValues[0].Equals(MaestroApplication.Instance.UNKNOWN_ITEM_NAME) ? string.Empty : secondLevelValues[0] ;
                ws.Cells[currentRow, productColumn - 1, currentRow, totalColumn].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                InsertRowValues(ws, productRows, 0);
                currentRow++;
            }
            else
            {
                using (ExcelRange range = ws.Cells[currentRow, productColumn, (currentRow + secondLevelValues.Count - 1), productColumn])
                {
                    range.Merge = true;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Value = firstColumnValue;
                }

                for(int i=0;i<secondLevelValues.Count;i++)
                {
                    ws.Cells[(currentRow + i), productColumn + 1].Value = secondLevelValues[i].Equals(MaestroApplication.Instance.UNKNOWN_ITEM_NAME) ? string.Empty : secondLevelValues[i];

                    List<DataRow> subRows = productRows.Where(r => r.Field<string>(secondColumnName).Equals(secondLevelValues[i])).ToList();

                    InsertRowValues(ws, subRows, i);

                }
                
                currentRow += secondLevelValues.Count;
                ws.Cells[startRow, productColumn - 1, currentRow-1, totalColumn].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

            }
        }


        void InsertRowValues(ExcelWorksheet ws, List<DataRow> productRows, int offsetRow)
        {
            foreach (string grp in reportGroups)
            {
                int val = productRows.Where(r=>r.Field<string>("REPORT_GROUP").Equals(grp)).Select(r => r.Field<int>("QUANTITY")).Sum();
                if (val > 0)
                    ws.Cells[currentRow + offsetRow, grpCol[grp]].Value = val;
            }

            int total = productRows.Select(r => r.Field<int>("QUANTITY")).Sum();
            if (total > 0)
                ws.Cells[currentRow + offsetRow, totalColumn].Value = total;

        }
    }
}
