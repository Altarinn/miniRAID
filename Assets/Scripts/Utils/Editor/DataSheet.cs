using System.Collections;
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;

namespace miniRAID.Editor.Numericals
{
    public struct DataRow
    {
        public int rowID;
        
        public string address;
        // public string name;

        public List<KeyValuePair<string, XLCellValue>> parameters;
    }
    
    public class DataSheet : IEnumerable<DataRow>
    {
        private FileStream fs;
        private XLWorkbook wb;
        public IXLWorksheet sheet;

        private Dictionary<int, string> columnDict = new();
        private int addrColumn;
        private int nameColumn;
        
        public DataSheet(string path)
        {
            fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            wb = new XLWorkbook(fs);
            wb.Worksheets.TryGetWorksheet("ActionCore", out sheet);
            GetHeader();
        }

        ~DataSheet()
        {
            Close();
        }

        public void Close()
        {
            fs.Close();
        }

        public void GetHeader()
        {
            // Read sheet header
            int totalColumns = sheet.LastColumnUsed().ColumnNumber();
            for(int c = 1; c <= totalColumns; c++)
            {
                if (sheet.Cell(2, c).Value.TryGetText(out string s))
                {
                    if (s == "primary")
                    {
                        addrColumn = c;
                    }
                    else
                    {
                        columnDict.Add(c, s);
                    }
                }
            }
        }

        public IEnumerator<DataRow> GetEnumerator()
        {
            int totalRows = sheet.LastRowUsed().RowNumber();
            for (int r = 3; r < totalRows; r++)
            {
                DataRow data = new DataRow();
                if(!sheet.Cell(r, addrColumn).Value.TryGetText(out data.address)){ continue; }
                data.parameters = new();
                foreach (var kv in columnDict)
                {
                    data.parameters.Add(new KeyValuePair<string, XLCellValue>(kv.Value, sheet.Cell(r, kv.Key).Value));
                }

                yield return data;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}