using Microsoft.Office.Interop.Excel;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TestCaseExtractor.ViewModel.CheckBoxTree;

namespace TestCaseExtractor
{
    public class ExcelWrapper
    {
        private enum TextAlignment
        {
            TopLeft,
            TopRight,
            TopCenter,
            Center,
            CenterMiddle
        }

        private Microsoft.Office.Interop.Excel.Application _app;
		private Workbook _workbook;
		private Sheets _worksheets;
		private Worksheet _worksheet;
		private Worksheet _worksheet1;
		private Range _range;
		//private ItemViewModel _rootViewModel;
		private string _path;
		private int _sheetID;
		private string _fileName;
		public bool _documentIsValid;

        public bool DocumentIsValid
        {
            get
            {
                return this._documentIsValid;
            }
            private set
            {
                this._documentIsValid = value;
            }
        }

        public void Initialize(ItemViewModel rootViewModel, string path)
        {

        }

        public void CreateDocument()
        {

        }

        private void TraverseTree(TestSuiteViewModel checkedTestSuite, string currentPath)
        {

        }

        private void GenerateSheetForTestSuite(ITestSuiteBase testSuite, IEnumerable<TestCaseViewModel> testCases, string testSuitePath)
        {

        }

        private void GenerateSheetHeader(string path)
        {

        }

        private int GenerateTestCaseHeader(int row, ITestSuiteEntry testCaseEntry)
        {
            return 0;
        }

        private int GenerateTestCase(ITestSuiteEntry testCaseEntry, int startRow)
        {
            string arg = RefinementWindow.createComments ? "E" : "D";
            int num = this.GenerateTestCaseHeader(startRow, testCaseEntry);
            TestActionCollection actions = testCaseEntry.get_TestCase().get_Actions();
            int count = actions.Count;
            int num2 = num + count + 1;
            this._range = this._worksheet.get_Range("B" + num, arg + (num2 - 2));
            this.DrawAllSolidBorders(this._range, 0);
            for (int i = 0; i < count; i++)
            {
                if (!(actions[i] is ISharedStepReference))
                {
                    ITestStep testStep = (ITestStep)actions[i];
                    string text = "B" + num;
                    string text2 = "C" + num;
                    string text3 = "D" + num;
                    string text4 = "E" + num;
                    this.WriteRange(text, text, (i + 1).ToString() + ".", null, null, new bool?(false), new bool?(false), new ExcelWrapper.TextAlignment?(ExcelWrapper.TextAlignment.TopCenter), null);
                    this.WriteRange(text2, text2, testStep.get_Title(), null, null, new bool?(false), new bool?(false), new ExcelWrapper.TextAlignment?(ExcelWrapper.TextAlignment.TopLeft), null);
                    this.WriteRange(text3, text3, testStep.get_ExpectedResult(), null, null, new bool?(false), new bool?(false), new ExcelWrapper.TextAlignment?(ExcelWrapper.TextAlignment.TopLeft), null);
                    if (RefinementWindow.createComments)
                    {
                        this.WriteRange(text4, text4, "", null, null, new bool?(false), new bool?(false), new ExcelWrapper.TextAlignment?(ExcelWrapper.TextAlignment.TopLeft), null);
                    }
                }
                num++;
            }
            return num2;
        }

        private void GenerateTableHeader(string testSuitePath)
        {
            
        }

        private void WriteRange(
            string cell1, 
            string cell2, 
            string value, 
            double? columnWidth = null,
            double? rowHeight = null,
            bool? columnsAutofit = false,
            bool? rowsAutofit = false,
            ExcelWrapper.TextAlignment? alignment = null,
            bool? merge = null)
        {
            this._range = this._worksheet.get_Range(cell1, cell2);
            this._range.WrapText = true;
            if (merge.HasValue && merge.Value)
            {
                this._range.Merge(System.Reflection.Missing.Value);
            }
            if (alignment.HasValue)
            {
                switch (alignment.Value)
                {
                    case ExcelWrapper.TextAlignment.TopLeft:
                        this._range.VerticalAlignment = XlVAlign.xlVAlignTop;
                        this._range.HorizontalAlignment = XlHAlign.xlHAlignLeft;
                        break;
                    case ExcelWrapper.TextAlignment.TopRight:
                        this._range.VerticalAlignment = XlVAlign.xlVAlignTop;
                        this._range.HorizontalAlignment = XlHAlign.xlHAlignRight;
                        break;
                    case ExcelWrapper.TextAlignment.TopCenter:
                        this._range.VerticalAlignment = XlVAlign.xlVAlignTop;
                        this._range.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                        break;
                    case ExcelWrapper.TextAlignment.Center:
                        this._range.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                        break;
                    case ExcelWrapper.TextAlignment.CenterMiddle:
                        this._range.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                        this._range.VerticalAlignment = XlVAlign.xlVAlignCenter;
                        break;
                }
            }
            this._range.NumberFormat = "@";
            this._range.Value2 = value;
            if (columnWidth.HasValue && columnWidth.Value != 0.0)
            {
                this._range.Columns.ColumnWidth = columnWidth.Value;
            }
            if (rowHeight.HasValue && rowHeight.Value != 0.0)
            {
                this._range.Rows.RowHeight = rowHeight.Value;
            }
            if (columnsAutofit.HasValue && columnsAutofit.Value)
            {
                this._range.Columns.AutoFit();
            }
            if (rowsAutofit.HasValue && rowsAutofit.Value)
            {
                this._range.Rows.AutoFit();
            }
        }

        private void SetWorksheetName(string name)
        {
            try
            {
                this._worksheet.Name = name.Substring(0, System.Math.Min(name.Length, 31));
            }
            catch (COMException)
            {
                name = string.Concat(new object[]
				{
					"(",
					this._sheetID,
					") ",
					name
				});
                this._worksheet.Name = name.Substring(0, System.Math.Min(name.Length, 31));
                this._sheetID++;
            }
        }

        private void DrawAllSolidBorders(Range range, int colour)
        {
            Borders borders = range.Borders;
            borders.Color = colour;
            borders[XlBordersIndex.xlDiagonalDown].LineStyle = XlLineStyle.xlLineStyleNone;
            borders[XlBordersIndex.xlDiagonalUp].LineStyle = XlLineStyle.xlLineStyleNone;
            borders[XlBordersIndex.xlEdgeLeft].LineStyle = XlLineStyle.xlContinuous;
            borders[XlBordersIndex.xlEdgeTop].LineStyle = XlLineStyle.xlContinuous;
            borders[XlBordersIndex.xlEdgeBottom].LineStyle = XlLineStyle.xlContinuous;
            borders[XlBordersIndex.xlEdgeRight].LineStyle = XlLineStyle.xlContinuous;
            borders[XlBordersIndex.xlInsideVertical].LineStyle = XlLineStyle.xlContinuous;
            borders[XlBordersIndex.xlInsideHorizontal].LineStyle = XlLineStyle.xlContinuous;
        }

        private void DrawSolidBorders(Range range, int colour)
        {
            range.BorderAround2(System.Reflection.Missing.Value, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
        }

        public void SaveDocument()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = this._fileName;
            saveFileDialog.DefaultExt = ".xlsx";
            saveFileDialog.Filter = "Excel Workbook (.xlsx)|*.xlsx";
            bool? flag = saveFileDialog.ShowDialog();
            if (flag.HasValue && flag.Value)
            {
                try
                {
                    this._workbook.SaveAs(saveFileDialog.FileName, System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value, XlSaveAsAccessMode.xlNoChange, XlSaveConflictResolution.xlLocalSessionChanges, System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
                    this._workbook.Close(System.Type.Missing, System.Type.Missing, System.Type.Missing);
                    this._app.Quit();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Could not save document. " + ex.Message);
                }
                finally
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(this._worksheet);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(this._worksheets);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(this._workbook);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(this._app);
                    this._app = null;
                }
            }
        }
    }
}
