using sReportsV2.Common.Enums;
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


        public void SetQuestionsValue(List<ConsensusQuestionDataOut> instanceQuestions) 
        {
            foreach (var question in instanceQuestions.Where(x => !string.IsNullOrWhiteSpace(x.Value)).ToList()) 
            {
                this.Questions.FirstOrDefault(x => x.ItemRef == question.ItemRef).Value = question.Value;
                this.Questions.FirstOrDefault(x => x.ItemRef == question.ItemRef).Comment = question.Comment;
            }
        }

    }
}