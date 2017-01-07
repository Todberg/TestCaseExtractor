# TestCaseExtractor
TestCaseExtractor is a small tool for exporting MTM (Microsoft Test Manager) test cases to Microsoft Excel.

# How to use TestCaseExtractor
1. Open TestCaseExtractor.exe
2. Press the 'Connect TFS' button
3. Select an existing TFS Team Project or add a new TFS Server
4. Find and select a test plan or test suite
5. Press the 'Extract' button
6. Select the test cases you want to extract
5. Press the 'Extract Selected' button

Create Comments Column: 
This option generates an extra excel comments column, which can be handy when customers review your test cases.

Include Test Case Summaries:
This option includes the test case summaries (if available).

# Limitations
- Verified to work on Windows7/8 x86/x64 with TFS 2013.
- Please note that 'Shared Steps' are NOT supported (if this has any interest, please contact me).
