using System.Text;

namespace SomeLibrary
{
    public class ListNode
    {
        public ListNode Prev;
        public ListNode Next;
        public ListNode Rand;
        public string Data;
    }

    public class ListRand
    {
        public ListNode Head;
        public ListNode Tail;
        public int Count;

        public void Serialize(FileStream s)
        {
            using (var sw = new StreamWriter(s))
            {
                var nodesToIndices = new Dictionary<ListNode, int>(Count);
                int counter = 0;

                WriteObjectBegin(sw);
                WriteValue(sw, Count);

                // Write nodes.
                WriteObjectBegin(sw);

                for (var temp = Head; temp != null; temp = temp.Next)
                {
                    nodesToIndices[temp] = counter;
                    counter += 1;
                    WriteNode(sw, temp);
                }

                WriteObjectEnd(sw);

                // Write connections.
                WriteObjectBegin(sw);

                for (var temp = Head; temp != null; temp = temp.Next)
                {
                    WriteRandomConnection(sw, temp, nodesToIndices);
                }

                WriteObjectEnd(sw);

                WriteObjectEnd(sw);
            }
        }

        public void Deserialize(FileStream s)
        {
            using (var sr = new StreamReader(s))
            {
                ReadObjectBegin(sr);
                Count = ReadValue<int>(sr);

                if (Count == 0)
                {
                    Head = null;
                    Tail = null;
                    return;
                }

                var nodes = new ListNode[Count];

                // Read nodes.
                ReadObjectBegin(sr);

                for (int i = 0; i < Count; i++)
                {
                    nodes[i] = ReadNode(sr);
                }

                ReadObjectEnd(sr);

                // Read connections.
                ReadObjectBegin(sr);

                for (int i = 0; i < Count; i++)
                {
                    nodes[i].Prev = i > 0 ? nodes[i - 1] : null;
                    nodes[i].Next = i < Count - 1 ? nodes[i + 1] : null;
                    ReadRandomConnection(sr, nodes[i], nodes);
                }

                ReadObjectEnd(sr);

                ReadObjectEnd(sr);

                Head = nodes[0];
                Tail = nodes[Count - 1];
            }
        }

        private static void WriteNode(StreamWriter sw, ListNode node)
        {
            WriteObjectBegin(sw);
            WriteValue(sw, node.Data);
            WriteObjectEnd(sw);
        }

        private static ListNode ReadNode(StreamReader sr)
        {
            var node = new ListNode();
            node.Prev = null;
            node.Next = null;
            node.Rand = null;

            ReadObjectBegin(sr);

            if (TryReadValue(sr, out string value))
            {
                node.Data = value;
            }

            ReadObjectEnd(sr);

            return node;
        }

        private static void WriteRandomConnection(
            StreamWriter sw,
            ListNode node,
            Dictionary<ListNode, int> nodesToIndices)
        {
            WriteObjectBegin(sw);

            if (node.Rand != null)
            {
                WriteValue(sw, nodesToIndices[node.Rand]);
            }
            else
            {
                WriteEmpty(sw);
            }

            WriteObjectEnd(sw);
        }

        private static void ReadRandomConnection(
            StreamReader sr,
            ListNode node,
            ListNode[] nodes)
        {
            ReadObjectBegin(sr);

            if (TryReadValue(sr, out int connection))
            {
                node.Rand = nodes[connection];
            }

            ReadObjectEnd(sr);
        }

        private static void WriteObjectBegin(StreamWriter sw)
        {
            sw.Write("{");
        }

        private static void ReadObjectBegin(StreamReader sr)
        {
            ReadChar(sr, '{');
        }

        private static void WriteObjectEnd(StreamWriter sw)
        {
            sw.Write("}");
        }

        private static void ReadObjectEnd(StreamReader sr)
        {
            ReadChar(sr, '}');
        }

        private static void WriteValue<T>(StreamWriter sw, T value)
        {
            WriteQuote(sw);

            if (value != null)
            {
                sw.Write(value.ToString());
            }

            WriteQuote(sw);
        }

        private static void WriteEmpty(StreamWriter sw)
        {
            WriteQuote(sw);
            WriteQuote(sw);
        }

        private static T ReadValue<T>(StreamReader sr) where T : IConvertible
        {
            ReadQuote(sr);
            var builder = new StringBuilder();

            while (!CheckQuote(sr))
            {
                var chr = (char)sr.Read();
                builder.Append(chr);
            }

            ReadQuote(sr);

            return (T)Convert.ChangeType(builder.ToString(), typeof(T));
        }

        private static bool TryReadValue<T>(StreamReader sr, out T value) where T : IConvertible
        {
            ReadQuote(sr);

            if (CheckQuote(sr))
            {
                ReadQuote(sr);
                value = default;
                return false;
            }

            var builder = new StringBuilder();

            while (!CheckQuote(sr))
            {
                var chr = (char)sr.Read();
                builder.Append(chr);
            }

            ReadQuote(sr);

            value = (T)Convert.ChangeType(builder.ToString(), typeof(T));
            return true;
        }

        private static void WriteQuote(StreamWriter sr)
        {
            sr.Write("\"");
        }

        private static bool CheckQuote(StreamReader sr)
        {
            return CheckChar(sr, '\"');
        }

        private static void ReadQuote(StreamReader sr)
        {
            ReadChar(sr, '\"');
        }

        private static bool CheckChar(StreamReader sr, char chr)
        {
            return (char)sr.Peek() == chr;
        }

        private static void ReadChar(StreamReader sr, char chr)
        {
            if ((char)sr.Peek() != chr)
            {
                throw new ArgumentException("Input stream is not valid.");
            }

            sr.Read();
        }
    }
}