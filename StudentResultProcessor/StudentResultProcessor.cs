using System;
using System.Collections.Generic;
using System.IO;

public record Student(string Name, int Age, double Score);

public interface IStudentProcessor
{
    void AddStudent(Student student);
    void DisplayResults();
    void SaveResultsToFile();
}

public sealed class StudentResultProcessor : IStudentProcessor
{
    private readonly List<Student> students = new List<Student>();

    public void AddStudent(Student student)
    {
        students.Add(student);
    }

    private string GetGrade(double score)
    {
        if (score >= 90) return "A";
        if (score >= 80) return "B";
        if (score >= 70) return "C";
        if (score >= 60) return "D";
        return "F";
    }

    public void DisplayResults()
    {
        foreach (var student in students)
        {
            string grade = GetGrade(student.Score);
            Console.WriteLine($"{student.Name} ({student.Age} years) - Score: {student.Score} - Grade: {grade}");
        }
    }

    public void SaveResultsToFile()
    {
        string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\.."));
        string outputFile = Path.Combine(projectRoot, "results.txt");

        using (StreamWriter writer = new StreamWriter(outputFile))
        {
            foreach (var student in students)
            {
                string grade = GetGrade(student.Score);
                writer.WriteLine($"{student.Name} ({student.Age} years) - Score: {student.Score} - Grade: {grade}");
            }
        }

        Console.WriteLine($"Results saved to '{outputFile}'.");
    }
}

class Program
{
    static void Main()
    {
        StudentResultProcessor processor = new StudentResultProcessor();

        Console.Write("Enter the number of students: ");
        int count = int.Parse(Console.ReadLine());

        for (int i = 0; i < count; i++)
        {
            Console.WriteLine($"\nStudent {i + 1}:");

            Console.Write("Name: ");
            string name = Console.ReadLine();

            Console.Write("Age: ");
            int age = int.Parse(Console.ReadLine());

            Console.Write("Score: ");
            double score = double.Parse(Console.ReadLine());

            processor.AddStudent(new Student(name, age, score));
        }

        Console.WriteLine("\n--- Student Results ---");
        processor.DisplayResults();

        processor.SaveResultsToFile();
    }
}
