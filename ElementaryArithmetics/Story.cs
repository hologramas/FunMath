//
//
//

namespace ElementaryArithmetics
{
    public class Story
    {
        public Story(ArithmeticOperation operation, ProblemType type, string description)
        {
            this.Operation = operation;
            this.ProblemType = type;
            this.StoryDescription = description;
        }

        public ArithmeticOperation Operation { get; private set; }
        public ProblemType ProblemType { get; private set; }
        public string StoryDescription { get; private set; }
    }
}
