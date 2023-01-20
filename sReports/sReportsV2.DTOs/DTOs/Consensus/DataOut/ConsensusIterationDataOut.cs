using sReportsV2.Common.Enums;
using sReportsV2.DTOs.DTOs.FormConsensus.DTO;
using sReportsV2.DTOs.Form.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Consensus.DataOut
{
    public class ConsensusIterationDataOut
    {
        public string Id { get; set; }
        public List<ConsensusQuestionDataOut> Questions { get; set; }
        public List<string> UserRefs { get; set; }
        public List<string> OutsideUserRefs { get; set; }
        public IterationState? State { get; set; }
        public List<QuestionOccurenceConfigDTO> QuestionOccurences { get; set; }

        public void SetQuestionsValue(List<ConsensusQuestionDataOut> instanceQuestions) 
        {
            foreach (var question in instanceQuestions) 
            {
                this.Questions.FirstOrDefault(x => x.ItemRef == question.ItemRef).Value = question.Value;
                this.Questions.FirstOrDefault(x => x.ItemRef == question.ItemRef).Comment = question.Comment;
            }
        }

        public bool ExistsQuestionForItem(string itemId)
        {
            return Questions.Any(x => x.ItemRef == itemId);
        }

        public QuestionOccurenceType? GetQuestionOccurenceTypeByLevel(string level)
        {
            return QuestionOccurences.FirstOrDefault(x => x.Level.Equals(level))?.Type;
        }

        public bool IsDisabledAddQuestion(int index, QuestionOccurenceType? questionOccurenceType)
        {
            return IsFormLevel() || questionOccurenceType == QuestionOccurenceType.Same && index != 0;
        }


        private bool IsFormLevel()
        {
            return QuestionOccurences.Any(x => x.Type == QuestionOccurenceType.Same && x.Level.Equals(FormItemLevel.Form.ToString()));
        }
    }
}