namespace BracketGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using BracketExtensions;

    public class BracketGenerator
    {
        public List<Participant> Participants { get; set; }
        public Stack<Participant> ParticipantsInBracketOrder { get; set; }
        public Stack<Participant> EdgeCaseParticipantsInBracketOrder { get; set; }
        public Match FinalMatch { get; set; }

        public List<Participant> EdgeCaseParticipants { get; set; }
        private List<int> BracketSeedOrder { get; set; }
        private int _bracketheight;

        public int BracketHeight
        {
            get
            {
                if (!Participants.Any())
                {
                    return 0;
                }
                if (_bracketheight > 0)
                {
                    return _bracketheight;
                }

                return _bracketheight = GetBracketHeight(Participants.Count, 1);
            }

            set
            {
                _bracketheight = value;
            }
        }

        public int Mid
        {
            get
            {
                return Participants.Count / 2;
            }
        }

        public int MidPlusOne
        {
            get
            {
                return Mid + 1;
            }
        }

        public int NumberOfByes
        {
            get
            {
                var height = BracketHeight;
                var ideal = (int)Math.Pow(2, height);
                var next_ideal = (int)Math.Pow(2, height + 1);
                var mid = ((next_ideal - ideal) / 2) + ideal;
                if (Participants.Count < mid && Participants.Count >= ideal)
                {
                    return 0;
                }

                return ideal - Participants.Count();
            }
        }

        public BracketGenerator()
        {
            Participants = new List<Participant>();
            BracketSeedOrder = new List<int>();
            FinalMatch = new Match();
        }

        public void GenerateBracket()
        {
            if (Participants.Count == 2)
            {
                return;
            }

            SetBracketOrder();

            CreateMatch(FinalMatch);
            if (EdgeCaseParticipants != null)
            {
                var height = BracketHeight;
                FinalMatch.SetEdgeCaseMatches(EdgeCaseParticipantsInBracketOrder, height);
                BracketHeight = height + 1;
            }
        }

        private bool IsFinalMatchFull(Match match)
        {
            return (match.PreviousMatches.Count > 1 && match.NextRound == null);
        }

        private void CreateMatch(Match match)
        {
            if (IsFinalMatchFull(match))
            {
                return;
            }

            if (match.BracketDepth == BracketHeight)
            {
                match.Participant1 = ParticipantsInBracketOrder.Pop();
                match.Participant2 = ParticipantsInBracketOrder.Pop();
                CreateMatch(match.NextRound);
                return;
            }

            if (match.PreviousMatches.Count > 1 && match.NextRound != null)
            {
                CreateMatch(match.NextRound);
                return;
            }

            Match previousMatch = new Match()
            {
                NextRound = match,
                Participant1 = new Participant(),
                Participant2 = new Participant(),
                PreviousMatches = new List<Match>(),
                BracketDepth = match.BracketDepth + 1
            };

            match.PreviousMatches.Add(previousMatch);
            CreateMatch(previousMatch);
        }

        public void SetEdgeCaseParticipants(List<Participant> list)
        {
            var ideal = (int)Math.Pow(2, BracketHeight);

            if (list.Count > ideal)
            {
                int difference = Participants.Count - ideal;
                int skip = ideal - difference;
                EdgeCaseParticipants = list.Skip(skip).ToList();
            }
        }

        public void SetBracketOrder()
        {
            AddByes();

            ParticipantsInBracketOrder = new Stack<Participant>(Participants);

            if (ParticipantsInBracketOrder.First().Seed == null)
            {
                RandomizeParticipants();
                AssignSeeds();
            }

            SetOrderBySeed();
            SetEdgeCaseOrderBySeed();
        }

        public void AssignSeeds()
        {
            var index = 0;
            foreach (var item in Participants)
            {
                item.Seed = index += 1;
            }
        }

        private void AddByes()
        {
            Participant bye = new Participant()
            {
                Name = "Bye",
                Seed = null
            };

            if (NumberOfByes == 0)
            {
                return;
            }

            int index = 0;
            var count = NumberOfByes;
            while (index < count)
            {
                Participants.Add(bye);
                index += 1;
            }
        }

        private void SetOrderBySeed()
        {
            var orderedList = ParticipantsInBracketOrder.OrderBy(k => k.Seed).ToList();
            SetEdgeCaseParticipants(orderedList);
            ParticipantsInBracketOrder.Clear();

            var height = BracketHeight;
            var ideal = (int)Math.Pow(2, height);
            orderedList = orderedList.Take(ideal).ToList();
            var decrementValue = height - 1;
            SetBracketSeedOrder(1, 1, orderedList, decrementValue, height);
            AssignBracketOrderToParticipants(1, height, orderedList);
        }

        public void SetEdgeCaseOrderBySeed()
        {
            if (EdgeCaseParticipants == null)
            {
                return;
            }
            EdgeCaseParticipantsInBracketOrder = new Stack<Participant>(EdgeCaseParticipants);
            var orderedList = EdgeCaseParticipantsInBracketOrder.OrderBy(k => k.Seed).ToList();
            EdgeCaseParticipantsInBracketOrder.Clear();
            var edgecaseheight = GetBracketHeight(EdgeCaseParticipants.Count, 1);
            int decrementValue = edgecaseheight - 1;
            BracketSeedOrder.Clear();
            SetBracketSeedOrder(1, 1, orderedList, decrementValue, edgecaseheight);
            AssignBracketOrderToEdgeCaseParticipants(1, edgecaseheight, orderedList);
        }

        private void AssignBracketOrderToParticipants(int iteration, int height, List<Participant> items)
        {
            if (iteration > Math.Pow(2, height)) 
            {
                return;
            }
            var index = BracketSeedOrder[iteration - 1];
            var item = items[index];
            ParticipantsInBracketOrder.Push(item);
            iteration += 1;
            AssignBracketOrderToParticipants(iteration, height, items);
        }

        private void AssignBracketOrderToEdgeCaseParticipants(int iteration, int height, List<Participant> items)
        {
            if (iteration > items.Count)
            {
                return;
            }
            var index = BracketSeedOrder[iteration - 1];
            var item = items[index];
            EdgeCaseParticipantsInBracketOrder.Push(item);
            iteration += 1;
            AssignBracketOrderToEdgeCaseParticipants(iteration, height, items);
        }

        private int SetBracketSeedOrder(int iteration, int insideIteration, List<Participant> orderedList, int decrementValue, int depth)
        { 
            var modvalue = (depth - decrementValue);
            var assignmentvalue = 0;

            if (iteration > orderedList.Count)
            {
                return iteration;
            }
            if (modvalue == depth)
            {
                iteration = SetTopGroupingSeed(iteration, insideIteration, orderedList);
                return iteration;
            }
            else if (insideIteration == 4)
            {
                assignmentvalue = ((orderedList.Count / 2) - 1) - modvalue;
                BracketSeedOrder.Add(assignmentvalue);
                insideIteration = 1;
                iteration += 1;
                decrementValue -= 1;
                iteration = SetBracketSeedOrder(iteration, insideIteration, orderedList, decrementValue, depth);
            }
            else if (insideIteration == 1)
            {
                assignmentvalue = (orderedList.Count - modvalue) - 1;
                BracketSeedOrder.Add(assignmentvalue);
            }
            else if (insideIteration == 2)
            {
                assignmentvalue = modvalue;
                BracketSeedOrder.Add(assignmentvalue);
            }
            else if (insideIteration == 3)
            {
                assignmentvalue = (orderedList.Count / 2) + modvalue;
                BracketSeedOrder.Add(assignmentvalue);
            }

            iteration += 1;
            insideIteration += 1;
            return SetBracketSeedOrder(iteration, insideIteration, orderedList, decrementValue, depth);
        }

        private int SetTopGroupingSeed(int iteration, int insideIteration, List<Participant> orderedList)
        {
            if(iteration > orderedList.Count)
            {
                return iteration;
            }

            var assignmentvalue = 0;

            if (insideIteration == 4)
            {
                BracketSeedOrder.Add(assignmentvalue);
                iteration += 1;
                return iteration;
            }
            else if (insideIteration == 1)
            {
                assignmentvalue = orderedList.Count / 2;
            }
            else if (insideIteration == 2)
            {
                assignmentvalue = (orderedList.Count / 2) - 1;
            }
            else if (insideIteration == 3)
            {
                assignmentvalue = orderedList.Count - 1;
            }

            BracketSeedOrder.Add(assignmentvalue);
            iteration += 1;
            insideIteration += 1;
            return SetTopGroupingSeed(iteration, insideIteration, orderedList);
        }

        private void RandomizeParticipants()
        {
            Random random = new Random();
            var list = ParticipantsInBracketOrder.ToList();
            int index = 0;
            int first = 0;
            int second = 0;
            while (index <= list.Count)
            {
                first = random.Next(0, list.Count);
                second = random.Next(0, list.Count);
                var temp = list[first];
                list[first] = list[second];
                list[second] = temp;
                index += 1;
            }

            ParticipantsInBracketOrder = new Stack<Participant>(list);
        }

        private int GetBracketHeight(int count, int iteration)
        {
            var ideal = Math.Pow(2, iteration);

            if (ideal == count)
            {
                return iteration;
            }

            var next_ideal = Math.Pow(2, iteration + 1);
            var mid = ((next_ideal - ideal) / 2) + ideal;

            if (count > ideal && count <= mid)
            {
                return iteration;
            }
            if (count > ideal && count < next_ideal)
            {
                return iteration += 1;
            }

            return GetBracketHeight(count, iteration += 1);
        }

        
    }

    public class Match
    {
        public Match()
        {
            Participant1 = new Participant();
            Participant2 = new Participant();
            NextRound = null;
            PreviousMatches = new List<Match>();
            BracketDepth = 1;
        }

        public Participant Participant1 { get; set; }
        public Participant Participant2 { get; set; }
        public Match NextRound { get; set; }
        public List<Match> PreviousMatches { get; set; }
        public int BracketDepth { get; set; }
    }

    public class Participant
    {
        public Participant()
        {
            Name = string.Empty;
            Seed = null;
        }

        public string Name { get; set; }
        public int? Seed { get; set; }
    }
}
