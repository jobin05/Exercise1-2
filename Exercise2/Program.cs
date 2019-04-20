using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Exercise2
{
    class Program
    {
        [Serializable()]
        struct WordStructre
        {
            public String word;
            public int Occurrence;
            public int Rank;
        }

        static void Main(string[] args)
        {
            string filePath1 = string.Empty;
            string filePath2 = string.Empty;
            string filePath3 = string.Empty;
            int printCount = 10;

            #region Input  and validate
            if (args.Length == 0)
            {
                Console.Write("\n Enter 1 input file path ");
                filePath1 = Console.ReadLine();
                Console.Write("\n Enter 2 input file path ");
                filePath2 = Console.ReadLine();
                Console.Write("\n Enter 3  input file path ");
                filePath3 = Console.ReadLine();
                Console.Clear();
                Console.WriteLine("This number is only using for display purpose. Full list get serialize in the bin file for future Use in Exercise 2 ");
                Console.WriteLine("Enter Required Number  ");
                int.TryParse(Console.ReadLine(), out printCount);

            }
            if (args.Length > 0)
            {

                if (args.Count() > 3)
                {
                    filePath1 = args[0];
                    filePath2 = args[1];
                    filePath3 = args[2];
                    int.TryParse(args[3], out printCount);

                }
                // LinkedList<WaitedList> st= DeserializerLinkedList
            }
            if (!IsInputFileIsValid(filePath1) || !IsInputFileIsValid(filePath2) || !IsInputFileIsValid(filePath3))
            {
                Console.Write("\n invalid File Pathe. Press Any key to Exit ");
                Console.ReadLine();
                return;
            }
            #endregion
            LinkedList<WordStructre> list1 = DeserializerLinkedList(filePath1);
            LinkedList<WordStructre> list2 = DeserializerLinkedList(filePath2);
            LinkedList<WordStructre> list3 = DeserializerLinkedList(filePath3);

            concatenateLinkedList(ref list1, list2);
            concatenateLinkedList(ref list1, list3);
            
            updateRankInList(ref list1);
            PrintResult(list1, printCount);
           // SaveResultInTextFile(list1, printCount, "Final_output.txt");
            serializerLinkedList(list1, "Final_output.json");
            Console.ReadLine();
        }

        /// <summary>
        ///   concatenateLinkedList two linkedlist
        /// </summary>
        /// <param name="parentList"></param>
        /// <param name="SecondList"></param>
        static void concatenateLinkedList(ref LinkedList<WordStructre> parentList, LinkedList<WordStructre> SecondList)
        {
            foreach (WordStructre secondList in SecondList)
            {

                LinkedListNode<WordStructre> currentNode = parentList.First;
                while (currentNode != null)
                {
                    bool loopBreak = false;

                    #region only case when we add less level word to the list     currentNode.Value.word.Length > word.Length                   
                    if (currentNode.Value.word.Length > secondList.word.Length)
                    {
                        parentList.AddBefore(currentNode, new WordStructre { word = secondList.word, Occurrence = 1 });
                        loopBreak = true;
                        break;
                    }
                    #endregion
                    #region If the same level word
                    if (currentNode.Value.word.Length == secondList.word.Length)
                    {
                        // If mor than one Occurrence of a same word
                        if (CompareWord.Equal == StrcmpNext(currentNode.Value.word, secondList.word))
                        {

                            WordStructre updatedNode = currentNode.Value;
                            updatedNode.Occurrence = updatedNode.Occurrence + secondList.Occurrence;
                            currentNode.Value = updatedNode;
                            loopBreak = true;
                            break;
                        }
                        //if new word is Small  we can simply add new in the begin Since it a sorted list
                        if (CompareWord.NewWordSmallThanCurrentNodeAddBefore == StrcmpNext(currentNode.Value.word, secondList.word))
                        {
                            {
                                parentList.AddBefore(currentNode, new WordStructre { word = secondList.word, Occurrence = 1 });
                                loopBreak = true;
                            }
                        }
                        //Add New is Large so add after the current node
                        else
                        {
                            if (currentNode.Next != null)
                            {
                                //Check for the next node in the list,add if next node word is small the word
                                if (CompareWord.NewWordSmallThanCurrentNodeAddBefore == StrcmpNext(currentNode.Next.Value.word, secondList.word))
                                {
                                    parentList.AddAfter(currentNode, new WordStructre { word = secondList.word, Occurrence = 1 });
                                    loopBreak = true;
                                }
                                // Add next level word for frist time
                                else if (CompareWord.CountLargerThan == StrcmpNext(currentNode.Next.Value.word, secondList.word))
                                {
                                    parentList.AddAfter(currentNode, new WordStructre { word = secondList.word, Occurrence = 1 });
                                    loopBreak = true;
                                }
                            }
                            else
                            {
                                parentList.AddAfter(currentNode, new WordStructre { word = secondList.word, Occurrence = 1 });
                                loopBreak = true;
                            }
                        }
                    }
                    #endregion
                    #region Add next level word if current node next is null
                    else
                    {
                        if (currentNode.Next == null)
                        {
                            parentList.AddAfter(currentNode, new WordStructre { word = secondList.word, Occurrence = 1 });
                            loopBreak = true;
                        }
                    }
                    #endregion
                    if (loopBreak)
                        break;
                    currentNode = currentNode.Next;
                    //Reach the requested Rank

                }
            }
        }

        /// <summary>
        /// Update Rank
        /// </summary>
        /// <param name="list"></param>
        static void updateRankInList(ref LinkedList<WordStructre> list)
        {
            int Rank = 1;
            LinkedListNode<WordStructre> currentNode = list.First;
            while (currentNode != null)
            {
                WordStructre node = currentNode.Value;
                node.Rank = Rank++;
                currentNode.Value = node;
                currentNode = currentNode.Next;
            }
        }
       
        /// <summary>
        /// Print the result in text file
        /// </summary>
        /// <param name="list"></param>
        /// <param name="count"></param>
        static void PrintResult(LinkedList<WordStructre> list, int count)
        {
            int RankCount = 1;
            Console.WriteLine("Rank" + "\t" + "Word" + "\t" + "Occurrence");
            foreach (WordStructre st in list)
            {
                if (count < RankCount)
                    break;
                Console.WriteLine(RankCount++ + "\t" + st.word + "\t" + st.Occurrence);
            }
        }
        enum CompareWord { Equal, NewWordLargerThanCurrentNodeAddAfter, NewWordSmallThanCurrentNodeAddBefore, CountLargerThan };

        /// <summary>
        /// Comp the words
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        static CompareWord StrcmpNext(string first, string second)
        {
            try
            {
                if (first.Length != second.Length)
                    return CompareWord.CountLargerThan;

                for (int i = 0; ; i++)
                {
                    if (first[i] != second[i])
                    {
                        //return first[i] < second[i] ? -1 : 1;
                        if (first[i] < second[i])
                        {
                            return CompareWord.NewWordLargerThanCurrentNodeAddAfter;
                        }
                        else
                        {
                            return CompareWord.NewWordSmallThanCurrentNodeAddBefore;
                        }
                    }

                    if (i == first.Length - 1)
                    {
                        return CompareWord.Equal;
                    }
                }
            }
            catch (Exception ex)
            {
                int x = 10;
                return 0;
            }

        }
        
        /// <summary>
        /// DeserializerLinkedList the linked list
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        static LinkedList<WordStructre> DeserializerLinkedList(string fileName)
        {
            LinkedList<WordStructre> readList;
            Newtonsoft.Json.JsonSerializer seri = new JsonSerializer();
            using (StreamReader sw = new StreamReader(fileName))
            {

                readList = (LinkedList<WordStructre>)seri.Deserialize(sw, typeof(LinkedList<WordStructre>));
                //seri.Deserialize<LinkedList<WaitedList>>(
            }
            return readList;
        }
        /// <summary>
        /// Check the files are valid
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        static bool IsInputFileIsValid(string filepath)
        {
            FileInfo info = new FileInfo(filepath);
            if (!info.Exists)
                return false;
            if (info.Extension != ".json")
                return false;
            return true;
        }

        /// <summary>
        /// Save to a text file
        /// </summary>
        /// <param name="list"></param>
        /// <param name="count"></param>
        /// <param name="fileName"></param>
        static void SaveResultInTextFile(LinkedList<WordStructre> list, int count, string fileName)
        {
            int RankCount = 1;
            Console.WriteLine("Rank" + "\t" + "Word" + "\t" + "Occurrence");
            using (StreamWriter streamWriter = new StreamWriter(fileName + "_output.txt"))
            {
                foreach (WordStructre st in list)
                {
                    if (count < RankCount)
                        break;
                    streamWriter.WriteLine(RankCount + "\t" + st.word + "\t" + st.Occurrence);
                    Console.WriteLine(RankCount + "\t" + st.word + "\t" + st.Occurrence);
                    RankCount += 1;
                }

            }

        }
        /// <summary>
        /// 
        /// serialize the object to json or binary
        /// </summary>
        /// <param name="lists"></param>
        /// <param name="fileName"></param>
        static void serializerLinkedList(LinkedList<WordStructre> lists, string fileName)
        {

            Newtonsoft.Json.JsonSerializer seri = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter(fileName + "_output.json"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                seri.Serialize(writer, lists);

            }

            //Stream FileStream = File.Create(findFindTheFileName());
            //BinaryFormatter serializer = new BinaryFormatter();
            //serializer.Serialize(FileStream, lists);
            //FileStream.Close();
        }
    }
}
