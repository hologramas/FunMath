//
// MIT License.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using ElementaryArithmetics.StoryElements;

namespace ElementaryArithmetics
{
    public class StoryGenerator
    {
        private List<ProblemType> problemTypes;
        private StoryVariableContainer storyVariableValues;
        private Dictionary<ProblemType, Dictionary<Operator, List<StoryFormat>>> storyFormats = new Dictionary<ProblemType, Dictionary<Operator, List<StoryFormat>>>();
        private Dictionary<VariableType, Dictionary<VariableGenre, IEnumerator<StoryVariable>>>  valueDispenser = new Dictionary<VariableType, Dictionary<VariableGenre, IEnumerator<StoryVariable>>>();
        private Random rand = new Random();
        private static char[] IdSplitter = new char[] { 'q' };

        public StoryGenerator(IEnumerable<ProblemType> problemTypes)
        {
            this.problemTypes = new List<ProblemType>(problemTypes);
        }

        public Story CreateStory(ArithmeticOperation operation)
        {
            var type = this.problemTypes[this.rand.Next(this.problemTypes.Count)];
            var compatibleStories = this.storyFormats[type][operation.Operator];
            var storyFormat = compatibleStories[this.rand.Next(compatibleStories.Count)];

            var storyVariables = new Dictionary<string, StoryVariable>();
            var storyReplaceValues = new Dictionary<string, string>();

            foreach (Match match in Regex.Matches(storyFormat.DescriptionFormat, @"#\w+#"))
            {
                string[] variableParts = match.Value.Split(IdSplitter);
                string variableId = variableParts[0].Replace("#", string.Empty);
                string variableKey = "#" + variableId + "#";

                StoryVariable variableValue;
                if (!storyVariables.ContainsKey(variableKey))
                {
                    VariableType variableType = ParseVariableType(variableId[0]);
                    VariableGenre variableGenre = ParseVariableGenre(variableId[1]);
                    if (!this.valueDispenser.ContainsKey(variableType))
                    {
                        this.valueDispenser.Add(variableType, new Dictionary<VariableGenre, IEnumerator<StoryVariable>>());
                    }
                    if (!this.valueDispenser[variableType].ContainsKey(variableGenre))
                    {
                        this.valueDispenser[variableType].Add(variableGenre, this.GetStoryVariableValueEnumarable(variableType, variableGenre).GetEnumerator());
                    }

                    if (!this.valueDispenser[variableType][variableGenre].MoveNext())
                    {
                        this.valueDispenser[variableType][variableGenre] = this.GetStoryVariableValueEnumarable(variableType, variableGenre).GetEnumerator();
                        this.valueDispenser[variableType][variableGenre].MoveNext();
                    }

                    variableValue = this.valueDispenser[variableType][variableGenre].Current;
                    storyVariables.Add(variableKey, variableValue);
                }
                else
                {
                    variableValue = storyVariables[variableKey];
                }

                if ((variableParts.Length > 1) && (!storyReplaceValues.ContainsKey(variableKey)))
                {
                    storyReplaceValues.Add(variableKey, variableValue.Name);
                    storyReplaceValues.Add("#" + variableId + "qg#", (variableValue.Genre == VariableGenre.Male) ? "he" : "she");
                    storyReplaceValues.Add("#" + variableId + "qcLO#", (operation.LeftOperand.Resolve() == 1) ? variableValue.Name : variableValue.NamePlural);
                    storyReplaceValues.Add("#" + variableId + "qcRO#", (operation.RightOperand.Resolve() == 1) ? variableValue.Name : variableValue.NamePlural);
                    storyReplaceValues.Add("#" + variableId + "qcTO#", (operation.Resolve() == 1) ? variableValue.Name : variableValue.NamePlural);
                    storyReplaceValues.Add("#" + variableId + "qcSI#", variableValue.Name);
                    storyReplaceValues.Add("#" + variableId + "qcPL#", variableValue.NamePlural);
                }
            }

            string storyDescription = storyFormat.DescriptionFormat;
            foreach (var variable in storyVariables)
            {
                storyDescription = storyDescription.Replace(variable.Key, variable.Value.Name);
            }

            foreach (var replaceValue in storyReplaceValues)
            {
                storyDescription = storyDescription.Replace(replaceValue.Key, replaceValue.Value);
            }

            storyDescription = storyDescription.Replace("$LO$", operation.LeftOperand.Resolve().ToString());
            storyDescription = storyDescription.Replace("$RO$", operation.RightOperand.Resolve().ToString());
            storyDescription = storyDescription.Replace("$TO$", operation.Resolve().ToString());

            return new Story(operation, type, storyDescription);
        }

        public async Task LoadStoryDataAsync(Uri storiesSourceUri, Uri storyElementsSourceUri)
        {
            Task[] loadingTasks = new Task[2];
            loadingTasks[0] = this.LoadStoryFormatsAsync(storiesSourceUri).ContinueWith((Task<IEnumerable<StoryFormat>> task) => 
            {
                foreach (var format in task.Result)
                {
                    this.AddStoryFormatToGenerator(format);
                }
            });

            loadingTasks[1] = this.LoadStoryVariablesAsync(storyElementsSourceUri).ContinueWith((Task<StoryVariableContainer> task) => 
            {
                this.storyVariableValues = task.Result;
            });

            await Task.WhenAll(loadingTasks);
        }

        private IEnumerable<StoryVariable> GetStoryVariableValueEnumarable(VariableType type, VariableGenre genre)
        {
            StoryVariable[] variables;
            switch (type)
            {
                case VariableType.Person:
                    variables = this.storyVariableValues.Persons;
                    break;
                case VariableType.Animal:
                    variables = this.storyVariableValues.Animals;
                    break;
                case VariableType.Food:
                    variables = this.storyVariableValues.Foods;
                    break;
                case VariableType.Thing:
                default:
                    variables = this.storyVariableValues.Things;
                    break;
            }

            List<StoryVariable> filteredVariables = new List<StoryVariable>();
            if (genre == VariableGenre.Any)
            {
                filteredVariables.AddRange(variables);
            }
            else
            {
                filteredVariables.AddRange(variables.Where(v =>
                {
                    return (v.Genre == genre);
                }));
            }

            for (int i = 0; i < filteredVariables.Count; i++)
            {
                int pick = this.rand.Next(filteredVariables.Count - i);
                var pickedValue = filteredVariables[pick];
                filteredVariables[pick] = filteredVariables[i];
                filteredVariables[i] = pickedValue;
                yield return pickedValue;
            }
        }

        private VariableType ParseVariableType(char value)
        {
            VariableType type;
            switch (value)
            {
                case 'P':
                    type = VariableType.Person;
                    break;
                case 'A':
                    type = VariableType.Animal;
                    break;
                case 'F':
                    type = VariableType.Food;
                    break;
                case 'T':
                    type = VariableType.Thing;
                    break;
                default:
                    throw new FormatException("Unrecognized VariableType");
            }
            return type;
        }

        private VariableGenre ParseVariableGenre(char value)
        {
            VariableGenre genre;
            switch (value)
            {
                case 'M':
                    genre = VariableGenre.Male;
                    break;
                case 'F':
                    genre = VariableGenre.Female;
                    break;
                case 'A':
                    genre = VariableGenre.Any;
                    break;
                default:
                    throw new FormatException("Unrecognized VariableGenre");
            }
            return genre;
        }

        private void AddStoryFormatToGenerator(StoryFormat format)
        {
            if (!this.storyFormats.ContainsKey(format.Type))
            {
                this.storyFormats.Add(format.Type, new Dictionary<Operator, List<StoryFormat>>());
            }
            if (!this.storyFormats[format.Type].ContainsKey(format.Operator))
            {
                this.storyFormats[format.Type].Add(format.Operator, new List<StoryFormat>());
            }

            this.storyFormats[format.Type][format.Operator].Add(format);
        }

        private async Task<IEnumerable<StoryFormat>> LoadStoryFormatsAsync(Uri sourceUri)
        {
            var storyFormatContainer = Activator.CreateInstance<StoryFormatContainer>();
            if (sourceUri.ToString().StartsWith("ms-appx:///Assets/"))
            {
                Windows.Storage.IStorageFile file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(sourceUri);
                using (var stream = await file.OpenStreamForReadAsync())
                {
                    var serializer = new DataContractJsonSerializer(storyFormatContainer.GetType());
                    storyFormatContainer = (StoryFormatContainer)serializer.ReadObject(stream);
                }
            }
            else
            {
                throw new NotImplementedException();
            }

            return storyFormatContainer.StoryFormats;
        }

        private async Task<StoryVariableContainer> LoadStoryVariablesAsync(Uri sourceUri)
        {
            var variableContainer = Activator.CreateInstance<StoryVariableContainer>();
            if (sourceUri.ToString().StartsWith("ms-appx:///Assets/"))
            {
                Windows.Storage.IStorageFile file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(sourceUri);
                using (var stream = await file.OpenStreamForReadAsync())
                {
                    var serializer = new DataContractJsonSerializer(variableContainer.GetType());
                    variableContainer = (StoryVariableContainer)serializer.ReadObject(stream);
                }
            }
            else
            {
                throw new NotImplementedException();
            }

            return variableContainer;
        }
    }
}
