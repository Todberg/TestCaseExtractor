using Microsoft.Office.Interop.Excel;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using TestCaseExtractor.ViewModel.CheckBoxTree;
using ExcelApplication = Microsoft.Office.Interop.Excel.Application;

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

        private ExcelApplication _app;
		private Workbook _workbook;
		private Sheets _worksheets;
		private Worksheet _worksheet;
		private Worksheet _worksheet1;
		private Range _range;
		private ItemViewModel _rootViewModel;
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
            this._rootViewModel = rootViewModel;
			this._path = path;
			
            this._app = new ExcelApplication();
            this._app.Visible = false;
			this._workbook = this._app.Workbooks.Add(Missing.Value);
			this._worksheets = this._workbook.Worksheets;
            this._worksheet1 = this._workbook.Worksheets.get_Item(1);
			this._sheetID = 1;
			this._documentIsValid = false;
			this._app.DisplayAlerts = false;
        }

        public void CreateDocument()
        {
            if (this._rootViewModel.GetType() == typeof(TestPlanViewModel))
            {
                TestPlanViewModel testPlanViewModel = (TestPlanViewModel)this._rootViewModel;
                
                IEnumerable<TestSuiteViewModel> checkedTestSuites = (
                    from testSuite in testPlanViewModel.Children
                    where (testSuite.IsChecked.HasValue && testSuite.IsChecked.Value) || !testSuite.IsChecked.HasValue
                    select testSuite).Cast<TestSuiteViewModel>();

                foreach (var testSuiteViewModel in checkedTestSuites)
                {
                    this._fileName = testPlanViewModel.Name;
                    this.TraverseTree(testSuiteViewModel, this._path);
                }
            }
            else
            {
                TestSuiteViewModel testSuiteViewModel = (TestSuiteViewModel)this._rootViewModel;
                this._fileName = testSuiteViewModel.Name;
                this.TraverseTree(testSuiteViewModel, this._path);
            }

            this._worksheet1.Delete();
        }

        private void TraverseTree(TestSuiteViewModel checkedTestSuite, string currentPath)
        {
            IEnumerable<TestCaseViewModel> checkedTestCases =
                from testCase in checkedTestSuite.ChildrenTestCases
                where testCase.IsChecked.HasValue && testCase.IsChecked.Value
                select testCase;

            if (checkedTestCases.Count() > 0)
            {
                this.GenerateSheetForTestSuite(checkedTestSuite.TestSuite.TFSTestSuiteBase, checkedTestCases, currentPath + "/" + checkedTestSuite.Name);
                this._documentIsValid = true;
            }

            IEnumerable<TestSuiteViewModel> checkedTestSuites = 
                from testSubSuite in checkedTestSuite.ChildrenTestSuites
                where (testSubSuite.IsChecked.HasValue && testSubSuite.IsChecked.Value) || testSubSuite == null
                select testSubSuite;

            foreach (TestSuiteViewModel testSuite in checkedTestSuites)
                TraverseTree(testSuite, currentPath + "/" + checkedTestSuite.Name);
        }

        private void GenerateSheetForTestSuite(ITestSuiteBase testSuite, IEnumerable<TestCaseViewModel> testCases, string testSuitePath)
        {
            this._worksheet = (Microsoft.Office.Interop.Excel.Worksheet)this._workbook.Sheets.Add();

            this.SetWorksheetName(testSuite.Title);
			this.GenerateSheetHeader(testSuitePath);
			int startRow = 4;

			foreach (TestCaseViewModel testCase in testCases)
			{
				startRow = this.GenerateTestCase(testCase.TestCase.TFSTestSuiteEntry, startRow);
			}
        }

        private void GenerateSheetHeader(string path)
        {
            this._range = this._worksheet.get_Range("A1", "A1");
            this._range.Columns.ColumnWidth = 2.14;
            string cell = RefinementWindow.createComments ? "E2" : "D2";
            this.WriteRange("B2", cell, path, null, new double?(40.0), new bool?(false), new bool?(false), new ExcelWrapper.TextAlignment?(ExcelWrapper.TextAlignment.CenterMiddle), new bool?(true));
            this._range = this._worksheet.get_Range("B2", cell);
            this._range.Interior.Color = 13037518;
            this.DrawAllSolidBorders(this._range, 0);
        }

        private int GenerateTestCaseHeader(int row, ITestSuiteEntry testCaseEntry)
        {
            string arg = RefinementWindow.createComments ? "E" : "D";

            ExcelWrapper.TextAlignment? alignment = new ExcelWrapper.TextAlignment?(ExcelWrapper.TextAlignment.TopRight);

			this.WriteRange("B" + row, arg + row,  "ID: " + testCaseEntry.Id.ToString(), null, null, new bool?(false), new bool?(true), alignment, new bool?(true));
			int num = row + 1;
			ExcelWrapper.TextAlignment? alignment2 = new ExcelWrapper.TextAlignment?(ExcelWrapper.TextAlignment.CenterMiddle);
			this.WriteRange("B" + num, arg + num, testCaseEntry.Title, null, new double?(35.0), new bool?(false), new bool?(false), alignment2, new bool?(true));

			num++;
			
            if (RefinementWindow.includeDescription && !string.IsNullOrEmpty(testCaseEntry.TestCase.Description))
			{
				this.WriteRange("B" + num, arg + num, testCaseEntry.TestCase.Description, null, 60.0, false, false, TextAlignment.CenterMiddle, true);
				this._range = this._worksheet.get_Range("B" + num, arg + num);
				this.DrawSolidBorders(this._range, 0);
				num++;
			}

			var array = new []
			{
				new
				{
					cell = "B" + num,
					title = "#",
					columnWidth = 2.86
				},
				new
				{
					cell = "C" + num,
					title = "Action",
					columnWidth = 70.0
				},
				new
				{
					cell = "D" + num,
					title = "Expected Result",
					columnWidth = 70.0
				},
				new
				{
					cell = "E" + num,
					title = "Comments",
					columnWidth = 40.0
				}
			};

			this._range = this._worksheet.get_Range("B" + row, arg + num);
			this._range.Interior.Color = 13037518;
			this.DrawSolidBorders(this._range, 0);
			int num2;
			int num3;

			if (RefinementWindow.createComments)
			{
				num2 = 4;
				num3 = 1;
			}
			else
			{
				num2 = 3;
				num3 = 2;
			}

			for (int i = 0; i < num2; i++)
				this.WriteRange(array[i].cell, array[i].cell, array[i].title, array[i].columnWidth, null, false, true, TextAlignment.Center, null);
            
			this._range = this._worksheet.get_Range(array[0].cell, array[array.Length - num3].cell);
			this._range.Interior.Color = 13037518;
			this.DrawAllSolidBorders(this._range, 0);
			return num + 1;
        }

        private int GenerateTestCase(ITestSuiteEntry testCaseEntry, int startRow)
        {
            string createComments = RefinementWindow.createComments ? "E" : "D";
            int currentRowNumber = this.GenerateTestCaseHeader(startRow, testCaseEntry);
            
            TestActionCollection actions = testCaseEntry.TestCase.Actions;
            int numberOfActions = actions.Count;
            
            int num2 = currentRowNumber + numberOfActions + 1;
            this._range = this._worksheet.get_Range("B" + currentRowNumber, createComments + (num2 - 2));
            this.DrawAllSolidBorders(this._range, 0);
            
            for (int i = 0; i < numberOfActions; i++)
            {
                if (actions[i] is ISharedStepReference)
                {
                    //ISharedStepReference isr = (ISharedStepReference)actions[i];
                    //ISharedStep ss = isr.FindSharedStep();
                    //TestActionCollection sharedActions = ss.Actions;

                    //foreach (ITestAction action in sharedActions)
                    //{
                    //    ITestStep ts = (ITestStep)action;
                    //    string text = "B" + currentRowNumber;
                    //    string text2 = "C" + currentRowNumber;
                    //    string text3 = "D" + currentRowNumber;
                    //    string text4 = "E" + currentRowNumber;
                    //    this.WriteRange(text, text, (i + 1).ToString() + ".", null, null, false, false, TextAlignment.TopCenter, null);
                    //    this.WriteRange(text2, text2, ts.Title.ToString(), null, null, false, false, TextAlignment.TopLeft, null);
                    //    this.WriteRange(text3, text3, ts.ExpectedResult.ToString(), null, null, false, false, TextAlignment.TopLeft, null);

                    //    if (RefinementWindow.createComments)
                    //    {
                    //        this.WriteRange(text4, text4, "", null, null, false, false, TextAlignment.TopLeft, null);
                    //    }
                    //    currentRowNumber++;
                    //}
                }
                else
                {
                    ITestStep testStep = (ITestStep)actions[i];
                    string text = "B" + currentRowNumber;
                    string text2 = "C" + currentRowNumber;
                    string text3 = "D" + currentRowNumber;
                    string text4 = "E" + currentRowNumber;
                    this.WriteRange(text, text, (i + 1).ToString() + ".", null, null, false, false, TextAlignment.TopCenter, null);
                    this.WriteRange(text2, text2, testStep.Title.ToString(), null, null, false, false, TextAlignment.TopLeft, null);
                    this.WriteRange(text3, text3, testStep.ExpectedResult.ToString(), null, null, false, false, TextAlignment.TopLeft, null);

                    if (RefinementWindow.createComments)
                    {
                        this.WriteRange(text4, text4, "", null, null, false, false, TextAlignment.TopLeft, null);
                    }
                }
                currentRowNumber++;
            }

            return num2;
        }

        private void GenerateTableHeader(string testSuitePath)
        {
            var array = new []
			{
				new
				{
					cell = "B3",
					title = "Work Item ID",
					columnWidth = 15.0
				},
				new
				{
					cell = "C3",
					title = "Title",
					columnWidth = 30.0
				},
				new
				{
					cell = "D3",
					title = "Description",
					columnWidth = 30.0
				},
				new
				{
					cell = "E3",
					title = "Action",
					columnWidth = 75.0
				},
				new
				{
					cell = "F3",
					title = "Expected Result",
					columnWidth = 75.0
				}
			};

			this.WriteRange("B2", "F2", testSuitePath, null, 40.0, false, false, TextAlignment.CenterMiddle, true);
			this._range = this._worksheet.get_Range("B2", "F2");
			this._range.Interior.Color = 13037518;
			this.DrawAllSolidBorders(this._range, 0);

			this._range = this._worksheet.get_Range(array[0].cell, array[array.Length - 1].cell);
			this._range.Interior.Color = 13037518;
			this.DrawAllSolidBorders(this._range, 0);
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
                this._range.Merge(Missing.Value);
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
                this._worksheet.Name = name.Substring(0, Math.Min(name.Length, 31));
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
                this._worksheet.Name = name.Substring(0, Math.Min(name.Length, 31));
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
            range.BorderAround2(Missing.Value, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic, Missing.Value, Missing.Value);
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
                    this._workbook.SaveAs(saveFileDialog.FileName, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, XlSaveAsAccessMode.xlNoChange, XlSaveConflictResolution.xlLocalSessionChanges, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                    this._workbook.Close(Type.Missing, Type.Missing, Type.Missing);
                    this._app.Quit();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not save document. " + ex.Message);
                }
                finally
                {
                    Marshal.ReleaseComObject(this._worksheet);
                    Marshal.ReleaseComObject(this._worksheets);
                    Marshal.ReleaseComObject(this._workbook);
                    Marshal.ReleaseComObject(this._app);
                    this._app = null;
                }
            }
        }
    }
}
