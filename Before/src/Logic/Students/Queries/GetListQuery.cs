using Dapper;
using Logic.Dtos;
using Logic.Utils;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Logic.Students.Queries
{
    public sealed class GetListQuery : IQuery<List<StudentDto>>
    {
        public string EnrolledIn { get; set; }

        public int? NumberOfCourses { get; set; }

        public GetListQuery(string enrolledIn, int? numberOfCourses)
        {
            EnrolledIn = enrolledIn;
            NumberOfCourses = numberOfCourses;
        }

        // NOTE: We return model of the higher level of our architecture (StudentDto), but that's okay
        // because our queries are not part of the onion architecture anymore. They are just a thin layer on top of DB access.
        internal sealed class GetListQueryHandler : IQueryHandler<GetListQuery, List<StudentDto>>
        {
            private readonly ConnectionString _connectionString;

            public GetListQueryHandler(ConnectionString connectionString)
            {
                _connectionString = connectionString;
            }

            public List<StudentDto> Handle(GetListQuery query)
            {
                const string sql = @"
SELECT s.*, e.Grade, c.Name CourseName, c.Credits
FROM dbo.Student s
LEFT JOIN (
    SELECT e.StudentId, COUNT(*) Number
	FROM dbo.Enrollment e
	GROUP BY e.StudentID) t on s.StudentID = t.StudentID
LEFT JOIN dbo.Enrollment e ON e.StudentID = s.StudentID
LEFT JOIN dbo.Course c ON c.CourseID = e.CourseID
WHERE (c.Name = @Course OR @Course IS NULL)
    AND (ISNULL(t.Number, 0) = @Number OR @Number IS NULL)
ORDER BY s.StudentID ASC";

                using (var connection = new SqlConnection(_connectionString.Value))
                {
                    List<StudentInDb> students = connection
                        .Query<StudentInDb>(sql, new
                        {
                            Course = query.EnrolledIn,
                            Number = query.NumberOfCourses
                        })
                        .ToList();

                    List<long> ids = students
                        .GroupBy(s => s.StudentId)
                        .Select(s => s.Key)
                        .ToList();

                    var result = new List<StudentDto>();

                    foreach (long id in ids)
                    {
                        List<StudentInDb> data = students
                            .Where(s => s.StudentId == id)
                            .ToList();

                        var dto = new StudentDto
                        {
                            Id = data[0].StudentId,
                            Name = data[0].Name,
                            // etc.
                        };

                        if (data.Count > 1)
                        {
                            dto.Course2 = data[1].CourseName;
                            // assign other fields.
                        }

                        result.Add(dto);
                    }

                    return result;
                }
            }

            // No need for encapsulation for this model since read model doesn't violate any invariants in the app.
            // It is used for reads only.
            private class StudentInDb
            {
                public readonly long StudentId;
                public readonly string Name;
                public readonly string Email;
                public readonly Grade? Grade;
                public readonly string CourseName;
                public readonly int? Credits;

                public StudentInDb(long studentId, string name, string email, Grade? grade, string courseName, int? credits)
                {
                    StudentId = studentId;
                    Name = name;
                    Email = email;
                    Grade = grade;
                    CourseName = courseName;
                    Credits = credits;
                }
            }
        }
    }
}
