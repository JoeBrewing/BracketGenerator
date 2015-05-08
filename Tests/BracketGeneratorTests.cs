namespace TreeSpikeTests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using System.Linq;
    using BracketGenerator;
    using BracketExtensions;

    [TestClass]
    public class BracketGeneratorTests
    {
        [TestMethod]
        public void should_get_correct_bracket_height_for_ideal_bracket()
        {
            BracketGenerator bracket = new BracketGenerator();
            bracket.Participants.Add(new Participant() { Name = "Joe", Seed = null });
            bracket.Participants.Add(new Participant() { Name = "Rob", Seed = null });
            bracket.Participants.Add(new Participant() { Name = "Nate", Seed = null });
            bracket.Participants.Add(new Participant() { Name = "Stephen", Seed = null });

            Assert.AreEqual(2, bracket.BracketHeight);
            Assert.AreEqual(0, bracket.NumberOfByes);
        }
        
        [TestMethod]
        public void should_get_correct_bracket_height()
        {
            BracketGenerator bracket = new BracketGenerator();
            bracket.Participants.Add(new Participant() { Name = "Joe", Seed = null });
            bracket.Participants.Add(new Participant() { Name = "Rob", Seed = null });
            bracket.Participants.Add(new Participant() { Name = "Nate", Seed = null });
            bracket.Participants.Add(new Participant() { Name = "Stephen", Seed = null });
            bracket.Participants.Add(new Participant() { Name = "Justin", Seed = null });
            bracket.Participants.Add(new Participant() { Name = "Blake", Seed = null });

            Assert.AreEqual(2, bracket.BracketHeight);
        }

        [TestMethod]
        public void should_get_correct_match_count_using_delegate()
        {
            BracketGenerator bracket = new BracketGenerator();
            bracket.Participants.Add(new Participant() { Name = "Joe", Seed = null });
            bracket.Participants.Add(new Participant() { Name = "Rob", Seed = null });
            bracket.Participants.Add(new Participant() { Name = "Nate", Seed = null });
            bracket.Participants.Add(new Participant() { Name = "Stephen", Seed = null });
            bracket.GenerateBracket();

            Assert.AreEqual(bracket.Participants.Count() - 1, bracket.FinalMatch.GetMatchCount());
        }

        [TestMethod]
        public void should_get_correct_match_count_using_delegate_if_edge_case_matches()
        {
            BracketGenerator bracket = new BracketGenerator();
            bracket.Participants.Add(new Participant() { Name = "Joe", Seed = null });
            bracket.Participants.Add(new Participant() { Name = "Rob", Seed = null });
            bracket.Participants.Add(new Participant() { Name = "Nate", Seed = null });
            bracket.Participants.Add(new Participant() { Name = "Stephen", Seed = null });
            bracket.Participants.Add(new Participant() { Name = "Justin", Seed = null });
            bracket.Participants.Add(new Participant() { Name = "Blake", Seed = null });
            bracket.GenerateBracket();

            Assert.AreEqual(bracket.Participants.Count() - 1, bracket.FinalMatch.GetMatchCount());
        }

        // ToDo: Is there a better way to do this? This seems to test both GetMatchWinner and SetMatchWinner
        // I'm not really sure how to test that SetMatchWinner works without getting the match winner
        [TestMethod]
        public void should_set_correct_winner()
        {
            BracketGenerator bracket = new BracketGenerator();
            bracket.Participants.Add(new Participant() { Name = "Joe", Seed = 1 });
            bracket.Participants.Add(new Participant() { Name = "Rob", Seed = 4 });
            bracket.Participants.Add(new Participant() { Name = "Nate", Seed = 2 });
            bracket.Participants.Add(new Participant() { Name = "Stephen", Seed = 3 });
            bracket.GenerateBracket();
            bracket.FinalMatch.SetMatchWinner("Joe", 1, bracket.BracketHeight);
            var winnerName = bracket.FinalMatch.GetMatchWinner(
                new List<string>(){"Joe", "Rob"}, 1, bracket.BracketHeight).Name;
            Assert.AreEqual("Joe", winnerName);
        }

        [TestMethod]
        public void should_assign_edge_case_participants_if_needed()
        {
            BracketGenerator bracket = new BracketGenerator();
            bracket.Participants.Add(new Participant() { Name = "Joe", Seed = null });
            bracket.Participants.Add(new Participant() { Name = "Rob", Seed = null });
            bracket.Participants.Add(new Participant() { Name = "Nate", Seed = null });
            bracket.Participants.Add(new Participant() { Name = "Stephen", Seed = null });
            bracket.Participants.Add(new Participant() { Name = "Justin", Seed = null });
            bracket.Participants.Add(new Participant() { Name = "Blake", Seed = null });

            bracket.SetBracketOrder();
            Assert.AreEqual(4, bracket.EdgeCaseParticipants.Count); 
        }

        [TestMethod]
        public void should_assign_byes_if_needed()
        {
            BracketGenerator bracket = new BracketGenerator();
            bracket.Participants.Add(new Participant() { Name = "Joe", Seed = null });
            bracket.Participants.Add(new Participant() { Name = "Rob", Seed = null });
            bracket.Participants.Add(new Participant() { Name = "Nate", Seed = null });
            bracket.Participants.Add(new Participant() { Name = "Stephen", Seed = null });
            bracket.Participants.Add(new Participant() { Name = "Justin", Seed = null });
            bracket.Participants.Add(new Participant() { Name = "Blake", Seed = null });
            bracket.Participants.Add(new Participant() { Name = "Kamika", Seed = null });

            bracket.SetBracketOrder();
            Assert.AreEqual(1, bracket.Participants.Count(k => k.Name == "Bye"));
        }

        [TestMethod]
        public void should_assign_seeds_to_randomized_participants()
        {
            BracketGenerator bracket = new BracketGenerator();
            bracket.Participants.Add(new Participant() { Name = "Joe", Seed = null });
            bracket.Participants.Add(new Participant() { Name = "Rob", Seed = null });
            bracket.Participants.Add(new Participant() { Name = "Nate", Seed = null });
            bracket.Participants.Add(new Participant() { Name = "Stephen", Seed = null });

            bracket.AssignSeeds();
            Assert.AreEqual(4, bracket.Participants[3].Seed);
        }

        [TestMethod]
        public void bracket_height_should_be_zero_if_no_participants_exist()
        {
            BracketGenerator bracket = new BracketGenerator();

            Assert.AreEqual(0, bracket.BracketHeight);
        }

        [TestMethod]
        public void bracket_should_set_edge_case_participants()
        {
            BracketGenerator bracket = new BracketGenerator();

            var orderedList = new List<Participant>();
            List<string> names = new List<string>()
            {
                "Rob",
                "Joe",
                "Kamika",
                "Matthew",
                "Stephen",
                "George",
                "Nate",
                "Matt",
                "Jeff",
                "Shayla",
                "Kelly",
            };

            int count = 0;
            while (count < 11)
            {
                bracket.Participants.Add(new Participant() { Name = names[count], Seed = count + 1 });
                count += 1;
            }

            bracket.SetEdgeCaseParticipants(bracket.Participants);

            Assert.AreEqual(6, bracket.EdgeCaseParticipants.Count);
        }

        [TestMethod]
        public void bracket_should_put_edge_case_participants_in_correct_order()
        {
            BracketGenerator bracket = new BracketGenerator();

            var orderedList = new List<Participant>();
            List<string> names = new List<string>()
            {
                "Rob",
                "Joe",
                "Kamika",
                "Matthew",
                "Stephen",
                "George",
                "Nate",
                "Matt",
                "Jeff",
                "Shayla",
                "Kelly",
            };

            int count = 0;
            while (count < 11)
            {
                bracket.Participants.Add(new Participant() { Name = names[count], Seed = count + 1 });
                count += 1;
            }

            bracket.SetEdgeCaseParticipants(bracket.Participants);
            bracket.SetEdgeCaseOrderBySeed();

            orderedList.Add(bracket.EdgeCaseParticipants.ElementAt(4));
            orderedList.Add(bracket.EdgeCaseParticipants.ElementAt(1));
            orderedList.Add(bracket.EdgeCaseParticipants.ElementAt(2));
            orderedList.Add(bracket.EdgeCaseParticipants.ElementAt(3));
            orderedList.Add(bracket.EdgeCaseParticipants.ElementAt(5));
            orderedList.Add(bracket.EdgeCaseParticipants.ElementAt(0));

            
            var orderedStack = new Stack<Participant>(orderedList);
            Assert.AreEqual(orderedStack.Count, bracket.EdgeCaseParticipantsInBracketOrder.Count);
            Assert.AreEqual(orderedStack.ElementAt(4), bracket.EdgeCaseParticipantsInBracketOrder.ElementAt(4));
        }

        [TestMethod]
        public void bracket_should_put_seeded_participants_in_correct_order()
        {
            BracketGenerator bracket = new BracketGenerator();

            var orderedList = new List<Participant>();
            List<string> names = new List<string>()
            {
                "Rob",
                "Joe",
                "Kamika",
                "Matthew",
                "Stephen",
                "George",
                "Nate",
                "Matt",
                "Jeff",
                "Shayla",
                "Kelly",
                "Stephanie",
                "Blake",
                "Jim",
                "Reid",
                "Jake"
            };

            int count = 0;
            while (count < 16)
            {
                bracket.Participants.Add(new Participant() { Name = names[count], Seed = count + 1 });
                count += 1; 
            }

            orderedList.Add(bracket.Participants.ElementAt(14));
            orderedList.Add(bracket.Participants.ElementAt(1));
            orderedList.Add(bracket.Participants.ElementAt(9));
            orderedList.Add(bracket.Participants.ElementAt(6));
            orderedList.Add(bracket.Participants.ElementAt(13));
            orderedList.Add(bracket.Participants.ElementAt(2));
            orderedList.Add(bracket.Participants.ElementAt(10));
            orderedList.Add(bracket.Participants.ElementAt(5));
            orderedList.Add(bracket.Participants.ElementAt(12));
            orderedList.Add(bracket.Participants.ElementAt(3));
            orderedList.Add(bracket.Participants.ElementAt(11));
            orderedList.Add(bracket.Participants.ElementAt(4));
            orderedList.Add(bracket.Participants.ElementAt(8));
            orderedList.Add(bracket.Participants.ElementAt(7));
            orderedList.Add(bracket.Participants.ElementAt(15));
            orderedList.Add(bracket.Participants.ElementAt(0));

            bracket.SetBracketOrder();
            count = bracket.ParticipantsInBracketOrder.Count;
            var orderedStack = new Stack<Participant>(orderedList);
            Assert.AreEqual(orderedStack.Count, bracket.ParticipantsInBracketOrder.Count);
            Assert.AreEqual(orderedStack.ElementAt(4), bracket.ParticipantsInBracketOrder.ElementAt(4));
            Assert.AreEqual(orderedStack.ElementAt(0), bracket.ParticipantsInBracketOrder.ElementAt(0));
            Assert.AreEqual(orderedStack.ElementAt(count - 1), bracket.ParticipantsInBracketOrder.ElementAt(count - 1));
        }
    }
}
