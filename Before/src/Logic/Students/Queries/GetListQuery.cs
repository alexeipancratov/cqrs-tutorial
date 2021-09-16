using Logic.Dtos;
using System.Collections.Generic;

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
    }
}
