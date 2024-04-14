using ClassLibrary;
using CsvHelper;
using Newtonsoft.Json;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;


namespace StudentTracker
{
    /// <summary>
    /// system to manage students grades
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Fields and Constants
        const String TBPATH = "Click Me To Browse";
        private String[] keyNames = null;
        private bool hasChanged = false;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized; // Set window state to Maximized
            this.ResizeMode = ResizeMode.NoResize; // Disable resizing

            pathTB.Text = TBPATH;
            //create folder if it doesn't exist
            if (!Directory.Exists("courses"))
                Directory.CreateDirectory("courses");

            gradesPanel.HorizontalAlignment = HorizontalAlignment.Center;
            gradesPanel.VerticalAlignment = VerticalAlignment.Center;

            InitializeComboBox();
        }

        /// <summary>
        /// file dialog to select the csv file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBlock_Clicked(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".csv"; // Default file extension
            dialog.Filter = "Text documents (.csv)|*.csv"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();


            // Process open file dialog box results
            if (result == true)
            {
                //pathTB.Text = dialog.FileName;
                int lastDotIndex = dialog.FileName.LastIndexOf('.');
                pathTB.Text = dialog.FileName.Substring(0, lastDotIndex);
            }
        }

        /// <summary>
        /// load the csv file from the path that selected, convert it to json, and save it in the courses folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadCSV(object sender, RoutedEventArgs e)
        {
            if (pathTB.Text == TBPATH)
            {
                MessageBox.Show("Please select file");
                return;
            }
            try
            {
                var reader = new StreamReader(pathTB.Text + ".csv");
                var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                var records = csv.GetRecords<dynamic>();



                //JSON serialization
                String json = JsonConvert.SerializeObject(records);
                string coursreName = System.IO.Path.GetFileName(pathTB.Text);
                //if the file already exists, delete it
                if (deleteCourseFile(coursreName))
                    //remove from the combobox
                    combox.Items.Remove(coursreName);

                DateTime localDate = DateTime.Now;
                var writer = new StreamWriter(System.IO.Path.Combine("courses", $"{coursreName}_{localDate.Date:dd-MM-yyyy}.json"));
                writer.Write(json);
                writer.Close();

                addItemCombox(coursreName);
                pathTB.Text = TBPATH;
                MessageBox.Show("Done");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Add ComboBox item with the name
        /// </summary>
        /// <param name="CourseName"></param>
        private void addItemCombox(String courseName)
        {
            combox.Items.Add(courseName);
            combox.SelectedItem = courseName;
        }

        /// <summary>
        /// when the user select a course from the combobox, read the json file and fill the studentsBox with the students
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void combox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (combox.SelectedItem == null)
                return;

            detailsBlock.Text = "";
            studentsBox.ItemsSource = null;
            tbfinal.Text = combox.SelectedItem.ToString() + " (Final Grades Average)";
            readJson(combox.SelectedItem.ToString());
            avgLabel.Content = "Average: ";
            this.Title = combox.SelectedItem.ToString();
        }

        /// <summary>
        /// read the json file and fill the studentsBox with the students
        /// </summary>
        /// <param name="fileName"></param>
        private void readJson(String? fileName)
        {

            fileName = SearchFile(fileName);

            var reader = new StreamReader(fileName);
            var json = reader.ReadToEnd();
            var objects = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);

            int gradesSize = objects[0].Keys.Count - 4;
            var keys = objects[0].Keys;

            // remove the keys: Name, LastName, ZehutNumber, Year
            keyNames = keys.Skip(4).ToList().ToArray();

            createGradeBox();

            List<Student> students = new List<Student>();
            Student s;
            foreach (var obj in objects)
            {
                s = new Student(obj["Name"], obj["LastName"], uint.Parse(obj["ZehutNumber"]), int.Parse(obj["Year"]));
                foreach (var key in keyNames)
                    s.Grades.Add(int.Parse(obj[key]));
                students.Add(s);
            }
            students.Sort();
            studentsBox.ItemsSource = students;
        }

        /// <summary>
        /// load the students from the student list file to the studentsBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void studentsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Student s = (Student)studentsBox.SelectedItem;
            if (s == null)
            {
                return;
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("Name: " + s.Name + "\n");
            sb.Append("Last Name: " + s.LastName + "\n");
            sb.Append("ID: " + s.ZehutNumber + "\n");
            sb.Append("Year: " + s.Year + "\n");

            detailsBlock.Text = sb.ToString();

            for (int i = 0; i < keyNames.Length; i++)
            {
                gradesPanel.Children.OfType<StackPanel>().ElementAt(i).Children.OfType<TextBox>().First().Text = s.Grades[i].ToString();
            }
            calcAverage();
        }

        /// <summary>
        /// create the grades panel
        /// </summary>
        private void createGradeBox()
        {
            gradesPanel.Children.Clear();
            for (int i = 0; i < keyNames.Length; i++)
            {
                var nameAndPercent = keyNames[i].Split('-');
                gradesPanel.Children.Add(createRow(nameAndPercent[0], "0", nameAndPercent[1], i));
            }
        }

        /// <summary>
        /// create component that contain label, textbox and label
        /// </summary>
        /// <param name="label"></param>
        /// <param name="text"></param>
        /// <param name="label2"></param>
        /// <param name="i"></param>
        /// <returns></returns>        
        private StackPanel createRow(string label, string text, string label2, int i)
        {
            const int MARGIN = 30;
            const int FONT_SIZE = 20;

            StackPanel sp = new StackPanel();
            sp.Width = gradesPanel.Width;
            sp.Orientation = Orientation.Horizontal;

            Label l = new Label();
            //l.Name = $"name{i}";
            l.HorizontalAlignment = HorizontalAlignment.Left;
            l.Width = 60;
            l.Content = label;
            l.FontSize = FONT_SIZE;
            l.Margin = new Thickness(MARGIN);
            sp.Children.Add(l);

            TextBox tb = new TextBox();
            tb.Name = $"grade{i}";
            tb.HorizontalAlignment = HorizontalAlignment.Center;
            tb.Text = text;
            tb.FontSize = FONT_SIZE;
            tb.Margin = new Thickness(MARGIN);
            tb.Width = 50;
            tb.TextAlignment = TextAlignment.Center;
            tb.TextChanged += grade_TextChanged;
            sp.Children.Add(tb);

            Label l2 = new Label();
            //l2.Name = $"percent{i}";
            l2.HorizontalAlignment = HorizontalAlignment.Right;
            l2.Content = label2;
            l2.FontSize = FONT_SIZE;
            l2.Margin = new Thickness(MARGIN);
            sp.Children.Add(l2);

            return sp;
        }

        /// <summary>
        /// calculate the average of the student grades
        /// </summary>
        private void calcAverage()
        {
            Student s = (Student)studentsBox.SelectedItem;
            if (s == null)
            {
                return;
            }

            double avg = 0;
            for (int i = 0; i < keyNames.Length; i++)
            {
                string p = gradesPanel.Children.OfType<StackPanel>().ElementAt(i).Children.OfType<Label>().Last().Content.ToString().Replace("%", "");
                double precent = double.Parse(p);
                string grade = gradesPanel.Children.OfType<StackPanel>().ElementAt(i).Children.OfType<TextBox>().First().Text;
                if (grade == "")
                    grade = "0";
                avg += double.Parse(grade) * (precent / 100);
            }
            avgLabel.Content = "Average: " + String.Format("{0:F2}", avg);
        }

        /// <summary>
        /// initialize the combobox with the courses names when the program start
        /// </summary>
        private void InitializeComboBox()
        {
            string[] courseFiles = Directory.GetFiles("courses", "*.json");

            foreach (string filePath in courseFiles)
            {
                string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
                string courseName = fileName.Split('_')[0];
                combox.Items.Add(courseName);
            }
        }

        /// <summary>
        /// search for the file that contain the course name in the courses folder
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string? SearchFile(string fileName)
        {
            string[] files = Directory.GetFiles("courses", fileName + "*");

            if (files.Length > 0)
            {
                return files[0];
            }

            return null;
        }

        /// <summary>
        /// check if the grade is a number and update the grades and average
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grade_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                if (!tb.Text.All(char.IsDigit))
                    tb.Text = "";
                else if (tb.Text == "")
                    return;
                else if (int.Parse(tb.Text) > 100)
                {
                    tb.Text = "100";
                }
                calcAverage();
                UpdateStudentGrades(tb);
            }
        }

        /// <summary>
        /// update the student grades
        /// </summary>
        /// <param name="gradeTextBox"></param>
        private void UpdateStudentGrades(TextBox gradeTextBox)
        {
            Student selectedStudent = (Student)studentsBox.SelectedItem;
            if (selectedStudent == null)
            {
                return;
            }

            int index = gradesPanel.Children.OfType<StackPanel>().ToList().FindIndex(sp => sp.Children.OfType<TextBox>().FirstOrDefault() == gradeTextBox);
            if (index >= 0)
            {
                string gradeText = gradeTextBox.Text.ToString();
                if (!string.IsNullOrEmpty(gradeText))
                {
                    int grade = int.Parse(gradeText);
                    selectedStudent.Grades[index] = grade;
                }
            }
            hasChanged = true;
        }

        /// <summary>
        /// save the data to the file and replace the old file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveFile(object sender, RoutedEventArgs e)
        {
            if (hasChanged == false)
            {
                MessageBox.Show("No changes were made");
                return;
            }
            //create new file
            string coursreName = combox.SelectedItem.ToString();
            //find the current file and delete it
            deleteCourseFile(coursreName);
            DateTime localDate = DateTime.Now;
            var writer = new StreamWriter(System.IO.Path.Combine("courses", $"{coursreName}_{localDate.Date:dd-MM-yyyy}.json"));

            //serialize the studentsv like it was in the file
            string json = SerializeStudents(studentsBox.ItemsSource.Cast<Student>().ToList());

            writer.Write(json);
            writer.Close();
            MessageBox.Show("Saved");
            hasChanged = false;
        }

        /// <summary>
        /// serialize the students to json
        /// </summary>
        /// <param name="students"></param>
        /// <returns></returns>
        private string SerializeStudents(List<Student> students)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");

            foreach (var student in students)
            {
                sb.Append("{");
                sb.Append("\"Name\": \"" + student.Name + "\",");
                sb.Append("\"LastName\": \"" + student.LastName + "\",");
                sb.Append("\"ZehutNumber\": \"" + student.ZehutNumber + "\",");
                sb.Append("\"Year\": \"" + student.Year + "\",");

                for (int i = 0; i < keyNames.Length; i++)
                {
                    string key = keyNames[i];
                    int grade = student.Grades[i];
                    sb.Append("\"" + key + "\": \"" + grade + "\"");
                    if (i < keyNames.Length - 1)
                    {
                        sb.Append(",");
                    }

                }

                sb.AppendLine("}");
                if (students.IndexOf(student) < students.Count - 1)
                    sb.Append(",");
            }

            sb.Append("]");

            return sb.ToString().TrimEnd(',');
        }

        /// <summary>
        /// manage the factor window and update the grades
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void factor(object sender, RoutedEventArgs e)
        {
            if (combox.SelectedItem == null)
            {
                MessageBox.Show("Please select a course");
                return;
            }

            FactorWindow factor = new FactorWindow(keyNames.ToList());
            bool? result = factor.ShowDialog();
            if (result == true)
            {
                string key = factor.Key;
                string value = factor.Value;
                //find the index of the key in the keyNames array
                int index = keyNames.ToList().FindIndex(k => k == key);
                //updeate all students
                foreach (Student s in studentsBox.ItemsSource)
                {
                    s.Grades[index] += int.Parse(value);
                    if (s.Grades[index] > 100)
                    {
                        s.Grades[index] = 100;
                    }
                }
                MessageBox.Show("Factor has given");
            }
            //update the grades panel
            studentsBox_SelectionChanged(null, null);
        }

        /// <summary>
        /// delete the course file from the courses folder
        /// </summary>
        /// <param name="courseName"></param>
        /// <returns></returns>
        private bool deleteCourseFile(string courseName)
        {

            string fileName = SearchFile(courseName);
            if (fileName != null)
                try
                {
                    File.Delete(fileName);
                    return true;
                }
                catch (IOException)
                {
                    MessageBox.Show("The file is open, please close it and try again");
                    return false;
                }
            return false;
        }
    }




}