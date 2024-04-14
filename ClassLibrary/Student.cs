namespace ClassLibrary
{
    public class Student : IComparable
    {
        
        public string Name { get; set; }
        public string LastName { get; set; }
        public uint ZehutNumber { get; set; }
        public int Year { get; set; }
        public List<int> Grades { get; set; }

        public Student(string name, string lastName, uint zehutNumber, int year)
        {
            Name = name;
            LastName = lastName;
            ZehutNumber = zehutNumber;
            Year = year;
            Grades = new List<int>();
        }

        //tostring
        public override string ToString()
        {
            return Name + " " + LastName + " " + ZehutNumber;
        }

        public int CompareTo(object? obj)
        {
            //compare by name
            return Name.CompareTo(((Student)obj).Name);
        }
    }
}
