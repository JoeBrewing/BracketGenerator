using BracketGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BracketExtensions
{
    public static class Extensions
    {
        public static T TraverseTreeAndExecuteFunction<T>(this Match match, Func<Match, T, T> fn) where T : new()
        {
            var value = new T();
            return GetValue<T>(value, match, fn);
        }

        public static T GetValue<T>(T value, Match match, Func<Match, T, T> fn)
        {
            value = fn(match, value);

            if (!match.PreviousMatches.Any())
            {
                return value;
            }

            foreach (Match item in match.PreviousMatches)
            {
                value = GetValue<T>(value, item, fn);
            }

            return value;
        }

        public static int GetMatchCount(this Match match)
        {
            return match.TraverseTreeAndExecuteFunction<int>(MatchCount<int>());
        }

        public static bool SetMatchWinner(this Match match, string name, int round, int bracketDepth)
        {
            return match.TraverseTreeAndExecuteFunction<bool>
                (SetWinner<bool>(name, round, bracketDepth));
        }

        public static bool SetEdgeCaseMatches(this Match match, Stack<Participant> items, int bracketDepth)
        {
            var count = 0;
            var total = items.Count;
            var numedgematches = total / 2;
            while (count < numedgematches)
            {
                var seed = count + 1;
                var edge1 = items.Pop();
                var edge2 = items.Pop();
                match.TraverseTreeAndExecuteFunction<bool>
                    (SetEdgeCaseMatch<bool>(seed, edge1, edge2, 1, bracketDepth));
                count += 1;
            }
            return true;
        }

        public static Participant GetMatchWinner(this Match match, List<string> names, int round, int bracketDepth)
        {
            return match.TraverseTreeAndExecuteFunction<Participant>
                (GetWinner<Participant>(names, round, bracketDepth));
        }

        public static Func<Match, T, bool> SetEdgeCaseMatch<T>(int seed, Participant item1, Participant item2, int round, int bracketDepth)
        {
            return (match, successful) =>
            {
                var depth = bracketDepth - (round - 1);
                // ToDo: move conditional to property in different class
                if ((seed != match.Participant1.Seed && seed != match.Participant2.Seed)
                    || match.BracketDepth != depth)
                {
                    return false;
                }

                var edgeMatch = new Match()
                {
                    BracketDepth = bracketDepth + 1,
                    NextRound = match,
                    Participant1 = item1,
                    Participant2 = item2,
                    PreviousMatches = new List<Match>()
                };

                match.PreviousMatches.Add(edgeMatch);

                if (match.Participant1.Seed == seed)
                {
                    match.Participant2.Name = "Bye";
                    match.Participant2.Seed = null;
                }
                else
                {
                    match.Participant1.Name = "Bye";
                    match.Participant1.Seed = null;
                }

                return true;
            };
        }

        public static Func<Match, T, bool> SetWinner<T>(string name, int round, int bracketDepth)
        {
            return (match, successful) =>
            {
                var depth = bracketDepth - (round - 1);
                // ToDo: move conditional to property in different class
                if ((name != match.Participant1.Name && name != match.Participant2.Name) 
                    || match.BracketDepth != depth)
                {
                    return false;
                }

                var participant = name == match.Participant1.Name ?
                    match.Participant1 : match.Participant2;
                var nextRound = match.NextRound;
                if (nextRound.PreviousMatches[0] == match)
                {
                    nextRound.Participant1 = participant;
                    return true;
                }
                
                nextRound.Participant2 = participant;
                return true;
            };
        }

        public static Func<Match, T, Participant> GetWinner<T>(List<string> names, int round, int bracketDepth) where T : Participant
        {
            return (match, participant) =>
            {
                var depth = bracketDepth - (round - 1);
                // ToDo: move conditional to property in different class
                if (!names.Contains(match.Participant1.Name)
                    || !names.Contains(match.Participant2.Name) || match.BracketDepth != depth)
                {
                    return participant;
                }
                var nextRound = match.NextRound;
                return names.Contains(nextRound.Participant1.Name) ? nextRound.Participant1 : nextRound.Participant2;
            };
        }

        public static Func<Match, T, int> MatchCount<T>() where T : IComparable<T>
        {
            return (match, value) =>
            {
                var count = Convert.ToInt32(value);

                return count += 1;
            };
        }
    }
}
