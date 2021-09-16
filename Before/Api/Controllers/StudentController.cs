﻿using System;
using CSharpFunctionalExtensions;
using Logic.Dtos;
using Logic.Students;
using Logic.Students.Commands;
using Logic.Students.Queries;
using Logic.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/students")]
    public sealed class StudentController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly Messages _messages;
        private readonly StudentRepository _studentRepository;
        private readonly CourseRepository _courseRepository;

        public StudentController(UnitOfWork unitOfWork, Messages messages)
        {
            _unitOfWork = unitOfWork;
            _messages = messages;
            _studentRepository = new StudentRepository(unitOfWork);
            _courseRepository = new CourseRepository(unitOfWork);
        }

        [HttpGet]
        public IActionResult GetList(string enrolled, int? number)
        {
            var students = _messages.Dispatch(new GetListQuery(enrolled, number));
            return Ok(students);
        }

        [HttpPost]
        public IActionResult Register([FromBody] NewStudentDto dto)
        {
            var command = new RegisterCommand(dto.Name, dto.Email, dto.Course1, dto.Course1Grade, dto.Course2, dto.Course2Grade);
            var result = _messages.Dispatch(command);

            return result.IsSuccess ? Ok() : Error(result.Error);
        }

        [HttpDelete("{id}")]
        public IActionResult Unregister(long id)
        {
            Result result = _messages.Dispatch(new UnregisterCommand(id));
            return 
        }

        [HttpPost("{id}/enrollments")]
        public IActionResult Enroll(long id, StudentEnrollmentDto dto)
        {
            Student student = _studentRepository.GetById(id);
            if (student == null)
                return Error($"No student found for Id {id}");

            Course course = _courseRepository.GetByName(dto.Course);
            if (course == null)
                return Error($"Course is incorrect {dto.Course}");

            bool success = Enum.TryParse(dto.Grade, out Grade grade);
            if (!success)
                return Error($"Grade is incorrect '${dto.Grade}'");

            student.Enroll(course, grade);

            _unitOfWork.Commit();

            return Ok();
        }

        [HttpPut("{id}/enrollments/{enrollmentNumber}")]
        public IActionResult Transfer(long id, int enrollmentNumber, [FromBody] StudentTransferDto dto)
        {
            Student student = _studentRepository.GetById(id);
            if (student == null)
                return Error($"No student found for Id {id}");

            Course course = _courseRepository.GetByName(dto.Course);
            if (course == null)
                return Error($"Course is incorrect {dto.Course}");

            bool success = Enum.TryParse(dto.Grade, out Grade grade);
            if (!success)
                return Error($"Grade is incorrect '${dto.Grade}'");

            Enrollment enrollment = student.GetEnrollment(enrollmentNumber);
            if (enrollment == null)
                return Error($"No enrollment found with number '{enrollmentNumber}'");

            enrollment.Update(course, grade);

            _unitOfWork.Commit();

            return Ok();
        }

        [HttpPost("{id}/enrollments/{enrollmentNumber}/deletion")]
        public IActionResult Disenroll(long id, int enrollmentNumber, StudentDisenrollmentDto dto)
        {
            Student student = _studentRepository.GetById(id);
            if (student == null)
                return Error($"No student found for Id {id}");

            if (string.IsNullOrWhiteSpace(dto.Comment))
                return Error("Disenrollment comment is required");

            Enrollment enrollment = student.GetEnrollment(enrollmentNumber);
            if (enrollment == null)
                return Error($"No enrollment found with number '{enrollmentNumber}'");

            student.RemoveEnrollment(enrollment, dto.Comment);

            _unitOfWork.Commit();

            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult EditPersonalInfo(long id, [FromBody] StudentPersonalInfoDto dto)
        {
            var command = new EditPersonalInfoCommand(id, dto.Name, dto.Email);
            var result = _messages.Dispatch(command);

            return result.IsSuccess ? Ok() : Error(result.Error);
        }

        private bool HasEnrollmentChanged(string newCourseName, string newGrade, Enrollment enrollment)
        {
            if (string.IsNullOrWhiteSpace(newCourseName) && enrollment == null)
                return false;

            if (string.IsNullOrWhiteSpace(newCourseName) || enrollment == null)
                return true;

            return newCourseName != enrollment.Course.Name || newGrade != enrollment.Grade.ToString();
        }
    }
}
