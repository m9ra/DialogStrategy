using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Knowledge;

namespace DialogStrategy.Learning
{
    class PathCounting
    {
        private readonly AnotatedData2[] _data;

        public PathCounting(IEnumerable<AnotatedData2> data)
        {
            _data = data.ToArray();
        }

        public void Train()
        {
            var patterns = getPatterns(_data);
            var missingPatterns = new List<DataPattern>(patterns);

            var builders = new List<ClassifierBuilder>();
            while (missingPatterns.Count > 0)
            {
                var targetPattern = missingPatterns[0];
                var mostConcreteBuilder = createBuilder(targetPattern, missingPatterns);
                builders.Add(mostConcreteBuilder);

                //check which data patterns are not covered yet
                var missingPatternsCopy = missingPatterns.ToArray();
                missingPatterns.Clear();
                foreach (var pattern in missingPatternsCopy)
                {
                    if (!isCovered(pattern, builders))
                        missingPatterns.Add(pattern);
                }
            }

            throw new NotImplementedException("Build conditions");
        }

        private ClassifierBuilder createBuilder(DataPattern targetPattern, IEnumerable<DataPattern> missingPatterns)
        {
            var builder = new ClassifierBuilder(targetPattern);
            var distribution = new Dictionary<int, List<DataPattern>>();

            foreach (var pattern in missingPatterns)
            {
                var distance = builder.PatternDistance(pattern);

                List<DataPattern> patterns;
                if (!distribution.TryGetValue(distance, out patterns))
                    distribution[distance] = patterns = new List<DataPattern>();

                patterns.Add(pattern);
            }

            var minimum = distribution.Keys.Min();
            foreach (var pattern in distribution[minimum])
            {
                builder.AcceptPattern(pattern);
            }

            return builder;
        }

        private bool isCovered(DataPattern pattern, IEnumerable<ClassifierBuilder> builders)
        {
            foreach (var builder in builders)
            {
                var hasCoveredPattern = builder.PatternDistance(pattern) != 0;
                if (hasCoveredPattern)
                    //at least one builder covers given pattern
                    return true;
            }

            return true;
        }

        private IEnumerable<DataPattern> getPatterns(IEnumerable<AnotatedData2> data)
        {
            throw new NotImplementedException();
        }

        private OperatorClassifier findOptimalClassifier(ResultGroup model)
        {
            throw new NotImplementedException();
        }

        private ResultGroups countResultGroups(IEnumerable<AnotatedData2> data)
        {
            foreach (var entry in data)
            {
                var paths = getPaths(entry);


            }

            throw new NotImplementedException();
        }


        private IEnumerable<KnowledgePath> getPaths(AnotatedData2 data)
        {
            throw new NotImplementedException();
        }
    }
}
