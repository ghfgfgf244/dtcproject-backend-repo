using dtc.Application.DTOs.Exams;
using dtc.Application.Interfaces.Exams;
using dtc.Domain.Entities.Exams;
using dtc.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dtc.Application.Services.Exams
{
    public class QuestionService : IQuestionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public QuestionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<QuestionResponseDto> CreateQuestionAsync(CreateQuestionRequestDto request)
        {
            var question = new Question(
                content: request.Content,
                correctAnswer: request.CorrectAnswer,
                a: request.AnswerA,
                b: request.AnswerB,
                c: request.AnswerC,
                d: request.AnswerD,
                imageLink: request.ImageLink,
                explanation: request.Explanation
            );

            await _unitOfWork.Questions.AddAsync(question);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(question);
        }

        public async Task<QuestionResponseDto> UpdateQuestionAsync(int id, UpdateQuestionRequestDto request)
        {
            var question = await _unitOfWork.Questions.GetByIdAsync(id);
            if (question == null) throw new Exception("Question not found");

            question.UpdateContent(
                content: request.Content,
                a: request.AnswerA,
                b: request.AnswerB,
                c: request.AnswerC,
                d: request.AnswerD,
                correctAnswer: request.CorrectAnswer,
                explanation: request.Explanation
            );

            await _unitOfWork.Questions.UpdateAsync(question);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(question);
        }

        public async Task<bool> DeleteQuestionAsync(int id)
        {
            var question = await _unitOfWork.Questions.GetByIdAsync(id);
            if (question == null) throw new Exception("Question not found");

            await _unitOfWork.Questions.RemoveAsync(question);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<QuestionResponseDto> GetQuestionDetailAsync(int id)
        {
            var question = await _unitOfWork.Questions.GetByIdAsync(id);
            if (question == null) throw new Exception("Question not found");

            return MapToDto(question);
        }

        public async Task<IEnumerable<QuestionResponseDto>> GetAllQuestionsAsync()
        {
            var questions = await _unitOfWork.Questions.GetAllAsync();
            var dtos = new List<QuestionResponseDto>();
            foreach (var q in questions)
            {
                dtos.Add(MapToDto(q));
            }
            return dtos;
        }

        private QuestionResponseDto MapToDto(Question question)
        {
            return new QuestionResponseDto
            {
                Id = question.Id,
                Content = question.Content,
                AnswerA = question.AnswerA,
                AnswerB = question.AnswerB,
                AnswerC = question.AnswerC,
                AnswerD = question.AnswerD,
                CorrectAnswer = question.CorrectAnswer,
                ImageLink = question.ImageLink,
                Explanation = question.Explanation,
                CreatedAt = question.CreatedAt
            };
        }
    }
}
