using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneticProgramming
{
    class Genetic
    {
        public List<FunctionWrapper> fwlist = new List<FunctionWrapper>();
        Random myRand = new Random();
        public dynamic dataset;
        public Genetic()
        {
            fwlist.Add(ifw);
            fwlist.Add(gtw);
            fwlist.Add(addw);
            fwlist.Add(subw);
            fwlist.Add(mulw);
           dataset = createdataset();
        }
        
        static dynamic iffun(List<dynamic> obj)
        {
            if (obj[0] > 0)
                return obj[1];
            else
                return obj[2];
        }
        FunctionWrapper ifw = new FunctionWrapper("if", 3, (Func<List<dynamic>, dynamic>)iffun);

        static dynamic isgreater(List<dynamic> obj)
        {
            if (obj[0] > obj[1])
                return 1;
            else
                return 0;
        }
        FunctionWrapper gtw = new FunctionWrapper("isgreater", 2, (Func<List<dynamic>, dynamic>)isgreater);

        static dynamic multiply(List<dynamic> obj)
        {
            if (obj[0] > obj[1])
                return 1;
            else
                return 0;
        }
        FunctionWrapper mulw = new FunctionWrapper("Multiply", 2, (Func<List<dynamic>, dynamic>)multiply);

        static dynamic add(List<dynamic> obj)
        {
            return obj[0] + obj[1];
        }
        FunctionWrapper addw = new FunctionWrapper("add", 2, (Func<List<dynamic>, dynamic>)add);

        static dynamic subtract(List<dynamic> obj)
        {
            return obj[0] - obj[1];
        }
        FunctionWrapper subw = new FunctionWrapper("subtract", 2, (Func<List<dynamic>, dynamic>)subtract);

        public dynamic exampletree()
        {
            return new Node(ifw, new List<tree>{new Node(gtw, new List<tree>{new paramnode(0), new consnode(3)}),
                                                new Node(addw, new List<tree>{new paramnode(1),new consnode(5)}),
                                                new Node(subw, new List<tree>{new paramnode(1),new consnode(2)})
            });
        }

        public tree makerandomtree(int parameterCount, double functionProb=0.6, double parameterProb=0.5, int maxdepth=4)
        {
           
            if (myRand.NextDouble() < functionProb && maxdepth>0)
            {
                var fun = fwlist[myRand.Next(0, fwlist.Count)];
                List<tree> children= new List<tree>();
                for (int i = 0; i < fun.childrenCount; i++)
                {
                    children.Add(makerandomtree(parameterCount,functionProb,parameterCount,maxdepth-1));
                }

                return new Node(fun, children);
            }
            else if (myRand.NextDouble() < parameterProb)
            {
                return new paramnode(myRand.Next(0, parameterCount));
            }
            else
            {
                return new consnode(myRand.Next(0, 10));
            }
        }

        public int hiddenfunction(int x, int y)
        {
            return ((int)Math.Pow(x, 2) + 2 * y + 3 * x + 5);
        }

        public List<List<dynamic>> createdataset()
        {
            List<List<dynamic>> dataset = new List<List<dynamic>>();
            for (int i = 0; i < 200; i++)
            {
                List<dynamic> data = new List<dynamic>();
                int x = myRand.Next(0, 40);
                int y = myRand.Next(0, 40);
                data.Add((dynamic)x);
                data.Add((dynamic)y);
                data.Add(hiddenfunction(x, y));
                dataset.Add(data);
            }
            return dataset;
        }

        public int scorefunction(tree t)
        {
            int diff = 0;
            foreach(var data in dataset)
            {
                diff += Math.Abs(data[2] - (int)t.Evaluate(data));
            }

            return diff;
        }

        public tree mutation(tree t,int parametercount, double mutationProb = 0.3)
        {
            if (myRand.NextDouble() < mutationProb)
            {
                return makerandomtree(parametercount);
            }

            if (t is Node)
            {
                Node mutable = (Node)t.DeepClone();
                List<tree> newchild = new List<tree>();
                foreach(var child in mutable.children)
                    newchild.Add(mutation(child,parametercount,mutationProb));
                mutable.children=newchild;
                return mutable;
            }
            else
            {
                return t.DeepClone();
            }
        }

        public tree crossover(tree t1, tree t2, double crossprob = 0.7, int top = 1)
        {
            if (myRand.NextDouble() < crossprob && (top==0))
                return t2.DeepClone();
            else
            {
                if(t1 is Node && t2 is Node)
                {
                    Node t=(Node)t1.DeepClone();
                    List<tree> newchild = new List<tree>();
                    Node tN2=(Node)t2;
                    foreach (var child in t.children)
                    {

                        newchild.Add(crossover(child,tN2.children[myRand.Next(0,tN2.children.Count)],crossprob,top-1));
                    }
                    t.children=newchild;

                    return t;
                }
                else
                {
                    return t1.DeepClone();
                }
            }
        }

        private int selectindex()
        { 
             var num= (int)(Math.Log(myRand.NextDouble())/Math.Log(0.7));
             //Console.WriteLine(num);
             return num;
        }

       

        public tree evolve(int parameterCount, int popsize, int maxgen = 500, double mutprob = 0.1, double crossprb = 0.4, double pnew = 0.05)
        { 
            List<tree> population= new List<tree>();
           

            for(int i=0;i<popsize;i++)
                population.Add(makerandomtree(parameterCount));
            while (maxgen-- > 0)
            {
                var scoredpop = from person in population orderby scorefunction(person) ascending select person;
                List<tree> newpopulation = new List<tree>();
             
                population = scoredpop.ToList();

                Console.WriteLine(scorefunction(population[0]) + "-->" + scorefunction(population[population.Count - 1]) + ":" + population.Count);
                if (scorefunction(population[0]) == 0)
                    return population[0];
                newpopulation.Add(population[0]);
                newpopulation.Add(population[1]);

                while (newpopulation.Count < popsize)
                {
                    if (myRand.NextDouble() > pnew)
                        newpopulation.Add(mutation(crossover(population[selectindex()], population[selectindex()], crossprb, 1), parameterCount, mutprob));
                    else
                        newpopulation.Add(makerandomtree(parameterCount));
                }
                population = newpopulation;
            }
            return population[0];
        }
    }
}
