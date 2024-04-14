Instructions for use:
1. press the "Load Course" button and choose the course file(.csv).
	-CSV columns must be like this:
	Name
	LastName
	ZehutNumber
	Year
	- The other columns:
	{assignment name} - {percentage of the grade} (two columns with the same name are not allowed)
2. press the "Load" button to load the course data.
3. look at the student in the list and select one.
4. now you can see the student details and the grades.
5. if you want to change the grade, select the assignment and change the grade.
6. if you want to give factor to all students in some assignment, press the "Factor" button and enter the factor.

*use csv with the same name of course that already loaded will cause delete the current course data*
*only save button will save the data to the courseFile*
*factor cant be negative*

Nugget:
-CsvHelper
-Newtonsoft.Json / GroupDocs.Conversion