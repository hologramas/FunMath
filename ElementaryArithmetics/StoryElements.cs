//
// MIT License.
//

using System.Runtime.Serialization;

namespace ElementaryArithmetics.StoryElements
{
    [DataContract(Namespace ="")]
    public enum VariableGenre
    {
        Any = 0,
        Male = 1,
        Female = 2
    }

    [DataContract(Namespace = "")]
    public enum VariableType
    {
        Person = 0,
        Animal = 1,
        Food = 2,
        Thing = 3
    }

    [DataContract(Namespace ="")]
    public class StoryVariable
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string NamePlural { get; set; }

        [DataMember]
        public VariableGenre Genre { get; set; }

        [DataMember]
        public VariableType Type { get; set; }
    }

    [DataContract(Namespace ="")]
    public class StoryFormat
    {
        [DataMember]
        public int Version { get; set; }

        [DataMember]
        public ProblemType Type { get; set; }

        [DataMember]
        public Operator Operator { get; set; }

        [DataMember]
        public string DescriptionFormat { get; set; }
    }

    [DataContract(Namespace ="")]
    internal class StoryFormatContainer
    {
        [DataMember]
        public StoryFormat[] StoryFormats { get; set; }
    }

    [DataContract(Namespace ="")]
    internal class StoryVariableContainer
    {
        [DataMember]
        public StoryVariable[] Persons { get; set; }

        [DataMember]
        public StoryVariable[] Animals { get; set; }

        [DataMember]
        public StoryVariable[] Foods { get; set; }

        [DataMember]
        public StoryVariable[] Things { get; set; }
    }
}
