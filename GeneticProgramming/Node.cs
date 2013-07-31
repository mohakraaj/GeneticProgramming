using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace GeneticProgramming
{
     public interface tree 
    {
        dynamic Evaluate(List<dynamic> obj);
    }


     public static class ExtensionMethods
     {
         // Deep clone
         public static T DeepClone<T>(this T a)
         {
             using (MemoryStream stream = new MemoryStream())
             {
                 BinaryFormatter formatter = new BinaryFormatter();
                 formatter.Serialize(stream, a);
                 stream.Position = 0;
                 return (T)formatter.Deserialize(stream);
             }
         }
     }

    [Serializable]
    class FunctionWrapper
    {
        public String name;
        public int childrenCount;
        public Func<List<dynamic>, dynamic> function;
        public FunctionWrapper(String name, int count, Func<List<dynamic>, dynamic> function)
        {
            this.name = name;
            this.childrenCount = count;
            this.function = function;
        }

    }

    [Serializable]
    class Node : tree
    {
        public String name;
        public dynamic function;
        public List<tree> children = new List<tree>();

        public Node()
        { 
        }
        public Node(FunctionWrapper fw, List<tree> children)
        {
            this.name = fw.name;
            this.function = fw.function;
            this.children = children;
           
        }

        public dynamic Evaluate(List<dynamic> obj)
        {
            //Console.WriteLine("fjkn");
            var result = from child in children select child.Evaluate(obj);
            return function(result.ToList());
        }
      
    }

    [Serializable]
    class paramnode : tree
    {
        public int index;
        public paramnode(int index)
        {
            this.index = index;
        }

        public dynamic Evaluate(List<dynamic> obj)
        {
            return obj[index];
        }

    }

    [Serializable]
    class consnode : tree
    {
        public int value;

        public consnode(int value)
        {
            this.value = value;
        }

        public dynamic Evaluate(List<dynamic> obj)
        {
            return value;
        }

    }
}
