using SomeLibrary;

class Program
{
    public static void Main(string[] args)
    {
        int nodesCount = 5;
        string path = "TestRandList.txt";
        var nodes = new ListNode[nodesCount];

        for (int i = 0; i < nodesCount; i++)
        {
            nodes[i] = new ListNode();
            nodes[i].Data = $"Node[{i}]";
            nodes[i].Prev = null;
            nodes[i].Next = null;
            nodes[i].Rand = null;
        }

        for (int i = 0; i < nodesCount; i++)
        {
            nodes[i].Prev = i > 0 ? nodes[i - 1] : null;
            nodes[i].Next = i < nodesCount - 1 ? nodes[i + 1] : null;
        }

        nodes[0].Rand = nodes[nodesCount - 1];
        nodes[2].Rand = nodes[0];
        nodes[nodesCount - 1].Rand = nodes[0];

        var list = new ListRand();
        list.Head = nodes[0];
        list.Tail = nodes[nodesCount - 1];
        list.Count = nodesCount;

        using (var fs = File.Create(path))
        {
            list.Serialize(fs);
        }

        var otherList = new ListRand();

        using (var fs = File.OpenRead(path))
        {
            otherList.Deserialize(fs);
        }

        if (!CompareListRand(list, otherList))
        {
            Console.WriteLine("Something went wrong...");
        }
        else
        {
            Console.WriteLine("Everything is fine.");
        }
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