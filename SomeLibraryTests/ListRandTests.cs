using SomeLibrary;

namespace SomeLibraryTests
{
    public class ListRandTests
    {
        public static IEnumerable<object[]> DeterministicListRandGenerator => new List<object[]>()
        {
            new[] { BindSomeNodes(GenerateListRand(0)) },
            new[] { BindSomeNodes(GenerateListRand(1)) },
            new[] { BindSomeNodes(GenerateListRand(2)) },
            new[] { BindSomeNodes(GenerateListRand(3)) },
            new[] { BindSomeNodes(GenerateListRand(10)) },
            new[] { BindSomeNodes(GenerateListRand(100)) },
        };

        public static IEnumerable<object[]> NonDeterministicListRandGenerator => new List<object[]>()
        {
            new[] { BindRandomNodes(GenerateListRand(0)) },
            new[] { BindRandomNodes(GenerateListRand(1)) },
            new[] { BindRandomNodes(GenerateListRand(2)) },
            new[] { BindRandomNodes(GenerateListRand(3)) },
            new[] { BindRandomNodes(GenerateListRand(10)) },
            new[] { BindRandomNodes(GenerateListRand(100)) },
        };

        [Theory]
        [MemberData(nameof(DeterministicListRandGenerator))]
        public void SerializeDesirializeDeterministicTest(ListRand list)
        {
            string path = "RandList.txt";
            var deserialized = new ListRand();

            using (var fs = File.Create(path))
            {
                list.Serialize(fs);
            }

            using (var fs = File.OpenRead(path))
            {
                deserialized.Deserialize(fs);
            }

            Assert.True(CompareListRand(list, deserialized));
        }

        [Theory]
        [MemberData(nameof(NonDeterministicListRandGenerator))]
        public void SerializeDesirializeNonDeterministicTest(ListRand list)
        {
            string path = "RandList.txt";
            var deserialized = new ListRand();

            using (var fs = File.Create(path))
            {
                list.Serialize(fs);
            }

            using (var fs = File.OpenRead(path))
            {
                deserialized.Deserialize(fs);
            }

            Assert.True(CompareListRand(list, deserialized));
        }

        private static ListRand GenerateListRand(int count)
        {
            if (count == 0)
            {
                var list = new ListRand();
                list.Head = null;
                list.Tail = null;
                list.Count = 0;
                return list;
            }

            var nodes = new ListNode[count];

            for (int i = 0; i < count; i++)
            {
                nodes[i] = new ListNode();
                nodes[i].Data = $"Node[{i}]";
                nodes[i].Prev = null;
                nodes[i].Next = null;
                nodes[i].Rand = null;
            }

            for (int i = 0; i < count; i++)
            {
                nodes[i].Prev = i > 0 ? nodes[i - 1] : null;
                nodes[i].Next = i < count - 1 ? nodes[i + 1] : null;
            }

            {
                var list = new ListRand();
                list.Head = nodes[0];
                list.Tail = nodes[count - 1];
                list.Count = count;
                return list;
            }
        }

        private static ListRand BindSomeNodes(ListRand list)
        {
            var nodes = new ListNode[list.Count];
            int counter = 0;

            for (var temp = list.Head; temp != null; temp = temp.Next)
            {
                nodes[counter] = temp;
                counter += 1;
            }

            for (int i = 2; i < nodes.Length; i += 2)
            {
                nodes[i - 2].Rand = nodes[i];
            }

            for (int i = 3; i < nodes.Length; i += 2)
            {
                nodes[i].Rand = nodes[i - 2];
            }

            return list;
        }

        private static ListRand BindRandomNodes(ListRand list)
        {
            var random = new Random();
            var nodes = new ListNode[list.Count];
            int counter = 0;

            for (var temp = list.Head; temp != null; temp = temp.Next)
            {
                nodes[counter] = temp;
                counter += 1;
            }

            for (int i = 0; i < list.Count; i++)
            {
                var randomNode = nodes[random.Next(list.Count)];
                nodes[i].Rand = randomNode;
            }

            return list;
        }

        private static bool CompareListRand(ListRand lhs, ListRand rhs)
        {
            if (lhs.Count != rhs.Count)
            {
                return false;
            }

            var lhsNodes = new ListNode[lhs.Count];
            var lhsIndices = new Dictionary<ListNode, int>();

            var rhsNodes = new ListNode[rhs.Count];
            var rhsIndices = new Dictionary<ListNode, int>();

            int counter = 0;

            for (var temp = lhs.Head; temp != null; temp = temp.Next)
            {
                lhsNodes[counter] = temp;
                lhsIndices[temp] = counter;
                counter += 1;
            }

            counter = 0;

            for (var temp = rhs.Head; temp != null; temp = temp.Next)
            {
                rhsNodes[counter] = temp;
                rhsIndices[temp] = counter;
                counter += 1;
            }

            for (int i = 0; i < lhsNodes.Length; i++)
            {
                var lhsNode = lhsNodes[i];
                var rhsNode = rhsNodes[i];

                if (lhsNode.Data != rhsNode.Data)
                {
                    return false;
                }

                if (lhsNode.Rand != null || rhsNode.Rand != null)
                {
                    if (lhsNode.Rand == null)
                    {
                        return false;
                    }
                    if (rhsNode.Rand == null)
                    {
                        return false;
                    }

                    var lhsRandIndex = lhsIndices[lhsNode.Rand];
                    var rhsRandIndex = rhsIndices[rhsNode.Rand];

                    if (lhsRandIndex != rhsRandIndex)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}