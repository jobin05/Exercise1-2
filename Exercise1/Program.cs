using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Newtonsoft.Json;

namespace Exercise1
{
    [Serializable()]
    struct WordStructre
    {        
        public String word;       
        public int Occurrence;
        public int Rank;
    }
  
    class Program
    {

       static string input = "Ban that bad cat from the car! It ate a bat in the bar";
        //static string input = @"Devise and implement, in the programming language of your choice, an efficient algorithm that reports a requested
        //number of top, i.e. 100, smallest value value distinct words language and their total occurrence across a given number of lists
        //containing words ordered as per Exercise ";
        //static string input = "Bbn Bcn Ben Bdn Bfn Ban Ban aaa aaaa";
        //static string input = " z y y q l  t b a t r w q l k j h g f d s a z x c v b n m  p o i u y t r e w q 1  c j f e aa ad ab aaa a ";
        //static string input = "hi hello";
        //static string input = "I’am Iam iam, you’re Youre,";
        //static string input = "boat born";

        //   static string input = "car cat";
        // static string input = "boat born";
        static void Main(string[] args)
        {
            

            string filePath = string.Empty;
            int printCount = 10;

            if (args.Length == 0)
            {
                Console.Write("\n Enter input file path ");
                filePath = Console.ReadLine();
                Console.Clear();
                Console.WriteLine("This number is only using for display purpose. Full list get serialize in the bin file for future Use in Exercise 2 ");
                Console.WriteLine("Enter Required Number  ");
                int.TryParse(Console.ReadLine(), out printCount);
                
            }
            if (args.Length > 0)
            {
                filePath = args[0];
                if(args.Count() > 1)
                int.TryParse(args[1], out printCount);
                
            }
            if (!IsInputFileIsValid(filePath))
            {
                
                Console.Write("\n invalid File Pathe. Press Any key to Exit ");
                Console.ReadLine();
                return ;
            }
           

            input = File.ReadAllText(filePath);
            string[] inputArray = input.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
            
            LinkedList<WordStructre> waitedList = new LinkedList<WordStructre>();
            foreach (String st in inputArray)
            {
                String word = st;
                char[] array = word.ToArray();

               
                RemoveUnWantedCharBetweenWord_ToLower(ref word);

             
                // If only number in the word. No need to count,
                if (word == "")
                    continue;
                #region Adding First Word into the list 
                if (waitedList.Count == 0)
                {
                    waitedList.AddLast(new WordStructre {  word = word,  Occurrence = 1 });
                    continue;
                }
                #endregion
                else
                {

                    LinkedListNode<WordStructre> currentNode = waitedList.First;
                    while (currentNode != null)
                    {
                        bool loopBreak = false;

                        #region only case when we add less level word to the list     currentNode.Value.word.Length > word.Length                   
                        if (currentNode.Value.word.Length > word.Length)
                        {
                            waitedList.AddBefore(currentNode, new WordStructre {  word = word, Occurrence = 1 });
                            loopBreak = true;
                            break;
                        }
                        #endregion
                        #region If the same level word
                        if (currentNode.Value.word.Length == word.Length)
                        {
                            // If mor than one Occurrence of a same word
                            if (CompareWord.Equal == StrcmpNext(currentNode.Value.word, word))
                            {

                                WordStructre updatedNode = currentNode.Value;
                                updatedNode.Occurrence += 1;
                                currentNode.Value = updatedNode;
                                loopBreak = true;
                                break;
                            }
                            //if new word is Small  we can simply add new in the begin Since it a sorted list
                            if (CompareWord.NewWordSmallThanCurrentNodeAddBefore == StrcmpNext(currentNode.Value.word, word))
                            {
                                {
                                    waitedList.AddBefore(currentNode, new WordStructre {word = word, Occurrence = 1 });
                                    loopBreak = true;
                                }
                            }
                            //Add New is Large so add after the current node
                            else
                            {
                                if (currentNode.Next != null)
                                {
                                    //Check for the next node in the list,add if next node word is small the word
                                    if (CompareWord.NewWordSmallThanCurrentNodeAddBefore == StrcmpNext(currentNode.Next.Value.word, word))
                                    {
                                        waitedList.AddAfter(currentNode, new WordStructre {  word = word, Occurrence = 1 });
                                        loopBreak = true;
                                    }
                                    // Add next level word for frist time
                                    else if (CompareWord.CountLargerThan == StrcmpNext(currentNode.Next.Value.word, word))
                                    {
                                        waitedList.AddAfter(currentNode, new WordStructre {  word = word, Occurrence = 1 });
                                        loopBreak = true;
                                    }
                                }
                                else
                                {
                                    waitedList.AddAfter(currentNode, new WordStructre {  word = word, Occurrence = 1 });
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
                                waitedList.AddAfter(currentNode, new WordStructre {  word = word, Occurrence = 1 });
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
            updateRankInList(ref waitedList);
            SaveResultInTextFile(waitedList, printCount, Path.GetFileNameWithoutExtension(filePath));
            serializerLinkedList(waitedList, Path.GetFileNameWithoutExtension(filePath));
            //PrintResult(waitedList, printCount);
            Console.ReadLine();

        }
        
        static void updateRankInList(ref LinkedList<WordStructre> list)
        {
            int Rank = 1;
            LinkedListNode<WordStructre> currentNode = list.First;
            while(currentNode != null)
            {
                WordStructre node = currentNode.Value;
                node.Rank = Rank++;
                currentNode.Value = node;
                currentNode = currentNode.Next;
            }
        }
        static bool IsInputFileIsValid(string filepath)
        {
            FileInfo info = new FileInfo(filepath);
            if (!info.Exists)
                return false;
            if (info.Extension != ".txt")
                return false;
            return true;
        }
        static void PrintResult(LinkedList<WordStructre> list,int count)
        {
            int RankCount = 1;
            Console.WriteLine("Rank" + "\t" + "Word"+  "\t" + "Occurrence");
            foreach (WordStructre st in list)
            {
                if (count < RankCount)
                    break;
                Console.WriteLine(RankCount++ + "\t" + st.word + "\t" + st.Occurrence);
            }
        }
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
        static void RemoveUnWantedCharBetweenWord_ToLower(ref string word)
        {
            int updatePositon = 0;
            char[] newWord = new char[word.Length];
            int x = 0;
            for (x = 0; x < word.Length; x++)
            {
                char eachCharFirst = word.ElementAt(x);
                if ((eachCharFirst >= 65 && eachCharFirst <= 90) || (eachCharFirst >= 97 && eachCharFirst <= 122))
                {
                    if (eachCharFirst >= 65 && eachCharFirst <= 90)
                    {
                      
                        newWord[updatePositon++] = (char)(eachCharFirst + 32);

                    }
                    else
                    {
                        newWord[updatePositon++] = eachCharFirst;
                    }
                }
            }
            //if(x > updatePositon)
            //    newWord[updatePositon++] = '\0';
            word = new string(newWord);
            word= word.Trim('\0');

        }
        /// <summary>
        /// IsLetterCountGreater
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        static int IsLetterCountGreater(string first, string second)
        {
            // Count Same
            if (first.Count() == second.Count())
                return 0;
            //First Word count less than second, Return -1
            else if (first.Count() > second.Count())
                return -1;
            else // else first word count greater than second , return 1
                return 1;
        }
      enum CompareWord { Equal,NewWordLargerThanCurrentNodeAddAfter,NewWordSmallThanCurrentNodeAddBefore,CountLargerThan};
        
        /// <summary>
        /// Only excute of the same number of char or extrea in the second
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
                        if(first[i] < second[i])
                        {
                            return CompareWord.NewWordLargerThanCurrentNodeAddAfter;
                        }
                        else
                        {
                            return CompareWord.NewWordSmallThanCurrentNodeAddBefore;
                        }
                    }

                    if (i== first.Length-1)
                    {
                        return CompareWord.Equal;
                    }
                }
            }catch(Exception ex)
            {
                int x = 10;
                return 0;
            }

            // int len = first.Length;
            //for (int x = 0, y = 0; x < len; x++)
            //{               
            //    char eachCharFirst = first.ElementAt(x);             

            //        char eachCharSecond = second.ElementAt(y);

            //        if (eachCharSecond == eachCharFirst)
            //        {
            //            y++;
            //            continue;
            //        }
            //        return eachCharSecond - eachCharFirst;

            //    }

            //}
            //return -1;
        }

        static void serializerLinkedList(LinkedList<WordStructre> lists,string fileName)
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
        static LinkedList<WordStructre> DeserializerLinkedList(string fileName)
        {
            if (File.Exists(fileName))
            {
                Stream FileStream1 = File.OpenRead(fileName);
                BinaryFormatter deserializer = new BinaryFormatter();
                LinkedList<WordStructre> result = (LinkedList<WordStructre>)deserializer.Deserialize(FileStream1);
                FileStream1.Close();
                return result;
            }
            return null;
        }
        static string findFindTheFileName()
        {
          String st = @".\" +System.DateTime.Now.ToLongTimeString()+".json";
           
            st = st.Replace(":", "-");
            return st.Replace(" ", "");


        }


    }
   
}
