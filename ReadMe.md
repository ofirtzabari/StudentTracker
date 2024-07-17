# Student Tracker
## Overview

### The Student Tracker is an application designed to manage student data and grades. It allows users to load course data from a CSV file, view and edit student details, and apply grade factors to assignments.
Features:

    Load course data from a CSV file.
    View and edit student details and grades.
    Apply grade factors to all students for a specific assignment.

## Instructions for Use
### Loading a Course

    Press the "Load Course" button and choose the course file (.csv).

    CSV columns must be formatted as follows:
        Name
        LastName
        ZehutNumber
        Year
        The remaining columns should follow the format: {assignment name} - {percentage of the grade}. Duplicate column names are not allowed.

    Press the "Load" button to load the course data.

### Viewing and Editing Student Data

    Look at the student list and select a student.
    View the student details and grades.
    To change a grade, select the assignment and update the grade.

### Applying a Factor

    Press the "Factor" button.
    Enter the factor value (note: factors cannot be negative).

### Important Notes

    Using a CSV file with the same course name as one already loaded will delete the current course data.
    Only the Save button will save the data to the course file.
    Factors cannot be negative.

### Dependencies

    CsvHelper
    Newtonsoft.Json / GroupDocs.Conversion

### Project Structure

    .gitattributes
    .gitignore
    ClassLibrary
    ReadMe.md
    StudentTracker.sln
    StudentTracker
        App.xaml
        App.xaml.cs
        AssemblyInfo.cs
        FactorWindow.xaml
        FactorWindow.xaml.cs
        MainWindow.xaml
        MainWindow.xaml.cs
        StudentTracker.csproj
    pics

# How to Contribute

    Fork the repository.
    Create a new branch (git checkout -b feature-branch).
    Commit your changes (git commit -am 'Add new feature').
    Push to the branch (git push origin feature-branch).
    Create a new Pull Request.
