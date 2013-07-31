using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneticProgramming
{

    class Program
    {
        static void Main(string[] args)
        {
            Genetic run = new Genetic();
            var test = run.makerandomtree(2);
          
          //  Console.WriteLine(run.scorefunction(test,dataset));
           
           // var mutant = run.mutation(test, 2);
          //  Console.WriteLine(run.scorefunction(mutant, dataset));
            run.evolve(2, 500,100,0.2,0.4,0.1);
          //  var x=test.Evaluate(new List<dynamic> {5,3});
            //Console.WriteLine(x);
            Console.Read();
        }



    }
}
